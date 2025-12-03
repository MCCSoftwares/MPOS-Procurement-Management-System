' frmrsmi.vb
Imports System.Data
Imports System.Data.SqlClient
Imports System.Drawing
Imports System.Globalization
Imports System.Linq
Imports System.Threading.Tasks
Imports System.Windows.Forms

Public Class frmrsmi

    ' ─────────────────────────────────────────────────────────────────────────────
    ' Column constants (main grid)
    ' ─────────────────────────────────────────────────────────────────────────────
    Private Const COL_RISNO As String = "RISNO"
    Private Const COL_RCC As String = "RESP_CENTER_CODE"
    Private Const COL_STOCKNO As String = "STOCKNO"
    Private Const COL_DESC As String = "ITEM_DESC"
    Private Const COL_UNIT As String = "UNIT"
    Private Const COL_QTY As String = "QTY_ISSUED"
    Private Const COL_UC As String = "UNIT_COST"
    Private Const COL_AMT As String = "AMOUNT"

    ' Recapitulation columns (DataGridView2 datasource column names)
    Private Const COL_R_STOCKNO As String = "STOCK_NO"           ' display alias only
    Private Const COL_R_QTY As String = "QUANTITY"
    Private Const COL_R_UC As String = "UNIT_COST"
    Private Const COL_R_TC As String = "TOTAL_COST"
    Private Const COL_R_UACS As String = "UACS_OBJECT_CODE"

    ' ─────────────────────────────────────────────────────────────────────────────
    ' Form Load
    ' ─────────────────────────────────────────────────────────────────────────────
    Private Sub frmrsmi_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        SetupRSMIGrid()
        SetupRecapGrid()
        comboyear.Text = Year(Now)

        ' Preselect current month/year (optional)
        If comboyear IsNot Nothing AndAlso comboyear.Items.Count = 0 Then
            For y As Integer = Date.Now.Year - 5 To Date.Now.Year + 1
                comboyear.Items.Add(y.ToString())
            Next
            comboyear.Text = Date.Now.Year.ToString()
        End If

        If combomonth IsNot Nothing AndAlso combomonth.Items.Count = 0 Then
            For m As Integer = 1 To 12
                combomonth.Items.Add(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m))
            Next
            combomonth.Text = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Date.Now.Month)
        End If
    End Sub

    ' ─────────────────────────────────────────────────────────────────────────────
    ' VIEW click: validate, load detail + recap
    ' ─────────────────────────────────────────────────────────────────────────────
    Private Async Sub cmdview_Click(sender As Object, e As EventArgs) Handles cmdview.Click
        Try
            Dim fcluster As String = SafeText(combofcluster.Text)
            If String.IsNullOrWhiteSpace(fcluster) Then
                MessageBox.Show("Please select a Fund Cluster.", "RSMI", MessageBoxButtons.OK, MessageBoxIcon.Information)
                combofcluster.Focus()
                Return
            End If

            Dim yStr As String = SafeText(comboyear.Text)
            Dim year As Integer
            If Not Integer.TryParse(yStr, year) OrElse year < 1900 OrElse year > 9999 Then
                MessageBox.Show("Please select a valid Year.", "RSMI", MessageBoxButtons.OK, MessageBoxIcon.Information)
                comboyear.Focus()
                Return
            End If

            Dim mm2 As String = ResolveMonth2(combomonth.Text)
            If String.IsNullOrEmpty(mm2) Then
                MessageBox.Show("Please select a valid Month.", "RSMI", MessageBoxButtons.OK, MessageBoxIcon.Information)
                combomonth.Focus()
                Return
            End If

            Dim eom As Date = GetEndOfMonth(year, Integer.Parse(mm2))

            ToggleUI(False)

            ' 1) Detailed list (Grid1)
            Dim dt As DataTable = Await QueryRSMIAsync(fcluster, year.ToString("0000"), mm2, eom).ConfigureAwait(True)
            DataGridView1.DataSource = dt
            If DataGridView1.Columns.Contains(COL_DESC) Then
                DataGridView1.Columns(COL_DESC).AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            End If

            ' 2) Recapitulation (Grid2)
            Dim recap As DataTable = Await BuildRecapitulationAsync(dt).ConfigureAwait(True)
            DataGridView2.DataSource = recap
            If DataGridView2.Columns.Contains(COL_R_STOCKNO) Then
                DataGridView2.Columns(COL_R_STOCKNO).AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            End If

        Catch ex As Exception
            MessageBox.Show("Failed to load RSMI data." & vbCrLf & ex.Message, "RSMI", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            ToggleUI(True)
        End Try
    End Sub

    ' ─────────────────────────────────────────────────────────────────────────────
    ' SQL loader (parameterized, WAC ≤ EOM via OUTER APPLY, join by DESCRIPTIONS)
    ' ─────────────────────────────────────────────────────────────────────────────
    Private Async Function QueryRSMIAsync(fcluster As String,
                                          year4 As String,
                                          mm2 As String,
                                          eom As Date) As Task(Of DataTable)

        Dim sql As String =
"WITH ris AS (
   SELECT
       r.RIS_NO,
       r.RCCODE,
       r.STOCK_NO,
       r.DESCRIPTIONS,
       r.UNIT,
       r.QUANTITY
   FROM dbo.TBL_RISITEMS r WITH (NOLOCK)
   WHERE
       r.FCLUSTER = @FCLUSTER
       AND LEFT(r.RIS_NO, 4) = @YEAR4
       AND SUBSTRING(r.RIS_NO, 6, 2) = @MM2
)
SELECT
   ris.RIS_NO       AS RISNO,
   ris.RCCODE       AS RESP_CENTER_CODE,
   ris.STOCK_NO     AS STOCKNO,
   ris.DESCRIPTIONS AS ITEM_DESC,
   ris.UNIT         AS UNIT,
   ris.QUANTITY     AS QTY_ISSUED,
   ISNULL(wac.WAC_UNIT_COST, 0) AS UNIT_COST,
   CAST(ISNULL(wac.WAC_UNIT_COST, 0) * ris.QUANTITY AS DECIMAL(18,2)) AS AMOUNT
FROM ris
OUTER APPLY (
   SELECT TOP (1)
       w.WAC_UNIT_COST
   FROM dbo.TBL_WAC w WITH (NOLOCK)
   WHERE
       w.DESCRIPTIONS = ris.DESCRIPTIONS
       AND w.WAC_DATE <= @EOM
   ORDER BY w.WAC_DATE DESC, w.ID DESC
) wac
ORDER BY ris.RIS_NO, ris.DESCRIPTIONS;"

        Dim dt As New DataTable()
        Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
            Await conn.OpenAsync().ConfigureAwait(False)
            Using cmd As New SqlCommand(sql, conn)
                cmd.Parameters.Add("@FCLUSTER", SqlDbType.NVarChar, 50).Value = fcluster
                cmd.Parameters.Add("@YEAR4", SqlDbType.Char, 4).Value = year4
                cmd.Parameters.Add("@MM2", SqlDbType.Char, 2).Value = mm2
                cmd.Parameters.Add("@EOM", SqlDbType.Date).Value = eom.Date

                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using
            End Using
        End Using
        Return dt
    End Function

    ' ─────────────────────────────────────────────────────────────────────────────
    ' Build Recapitulation (group by STOCK_NO + UNIT_COST, sum QTY & AMT)
    ' and join ACODE from TBL_INVENTORY by IPNO
    ' ─────────────────────────────────────────────────────────────────────────────
    Private Async Function BuildRecapitulationAsync(src As DataTable) As Task(Of DataTable)
        Dim recap As New DataTable()
        recap.Columns.Add(COL_R_STOCKNO, GetType(String))
        recap.Columns.Add(COL_R_QTY, GetType(Decimal))
        recap.Columns.Add(COL_R_UC, GetType(Decimal))
        recap.Columns.Add(COL_R_TC, GetType(Decimal))
        recap.Columns.Add(COL_R_UACS, GetType(String))

        If src Is Nothing OrElse src.Rows.Count = 0 Then Return recap

        ' In-memory group: by StockNo + UnitCost
        Dim groups = From r In src.AsEnumerable()
                     Let sNo = GetStr(r(COL_STOCKNO))
                     Let uCost = GetDec(r(COL_UC))
                     Group r By Stock = sNo, UC = uCost Into grp = Group
                     Select New With {
                         .StockNo = Stock,
                         .UnitCost = UC,
                         .Qty = grp.Sum(Function(x) GetDec(x(COL_QTY))),
                         .TotalCost = grp.Sum(Function(x) GetDec(x(COL_AMT)))
                     }

        ' Fetch ACODE by IPNO (== StockNo from RIS)
        Dim ipnos As List(Of String) = groups.Select(Function(g) g.StockNo).Distinct().Where(Function(s) s IsNot Nothing AndAlso s <> "").ToList()
        Dim uacsMap As Dictionary(Of String, String) = Await GetUACSByIPNOAsync(ipnos).ConfigureAwait(False)

        ' Fill recap table
        For Each g In groups
            Dim row As DataRow = recap.NewRow()
            row(COL_R_STOCKNO) = g.StockNo
            row(COL_R_QTY) = g.Qty
            row(COL_R_UC) = g.UnitCost
            row(COL_R_TC) = g.TotalCost
            row(COL_R_UACS) = If(uacsMap.ContainsKey(g.StockNo), uacsMap(g.StockNo), "")
            recap.Rows.Add(row)
        Next

        ' Sort recap (optional)
        recap.DefaultView.Sort = $"{COL_R_STOCKNO}, {COL_R_UC}"
        recap = recap.DefaultView.ToTable()
        Return recap
    End Function

    ' ─────────────────────────────────────────────────────────────────────────────
    ' Get ACODE from TBL_INVENTORY for list of IPNO values
    ' ─────────────────────────────────────────────────────────────────────────────
    Private Async Function GetUACSByIPNOAsync(ipnos As List(Of String)) As Task(Of Dictionary(Of String, String))
        Dim map As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
        If ipnos Is Nothing OrElse ipnos.Count = 0 Then Return map

        Dim dt As New DataTable()
        Dim inClause As String = String.Join(",", ipnos.Select(Function(s, i) "@p" & i))
        Dim sql As String = $"SELECT IPNO, ACODE FROM dbo.TBL_INVENTORY WITH (NOLOCK) WHERE IPNO IN ({inClause})"

        Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
            Await conn.OpenAsync().ConfigureAwait(False)
            Using cmd As New SqlCommand(sql, conn)
                For i As Integer = 0 To ipnos.Count - 1
                    cmd.Parameters.AddWithValue("@p" & i, ipnos(i))
                Next
                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using
            End Using
        End Using

        For Each r As DataRow In dt.Rows
            Dim ip As String = SafeText(TryCast(r("IPNO"), String))
            Dim ac As String = SafeText(TryCast(r("ACODE"), String))
            If ip <> "" AndAlso Not map.ContainsKey(ip) Then map.Add(ip, ac)
        Next
        Return map
    End Function

    ' ─────────────────────────────────────────────────────────────────────────────
    ' Helpers
    ' ─────────────────────────────────────────────────────────────────────────────
    Private Function ResolveMonth2(monthInput As String) As String
        If String.IsNullOrWhiteSpace(monthInput) Then Return Nothing
        Dim n As Integer
        If Integer.TryParse(monthInput.Trim(), n) AndAlso n >= 1 AndAlso n <= 12 Then
            Return n.ToString("00")
        End If
        Try
            Dim dt As Date = Date.ParseExact(monthInput.Trim(), "MMMM", CultureInfo.CurrentCulture)
            Return dt.Month.ToString("00")
        Catch
            Try
                Dim dt2 As Date = Date.ParseExact(monthInput.Trim(), "MMM", CultureInfo.CurrentCulture)
                Return dt2.Month.ToString("00")
            Catch
                Return Nothing
            End Try
        End Try
    End Function

    Private Function GetEndOfMonth(y As Integer, m As Integer) As Date
        Dim firstNext As New Date(If(m = 12, y + 1, y), If(m = 12, 1, m + 1), 1)
        Return firstNext.AddDays(-1)
    End Function

    Private Function SafeText(s As String) As String
        If s Is Nothing Then Return ""
        Return s.Trim()
    End Function

    Private Function GetDec(o As Object) As Decimal
        If o Is Nothing OrElse IsDBNull(o) Then Return 0D
        Return Convert.ToDecimal(o, CultureInfo.InvariantCulture)
    End Function

    Private Function GetStr(o As Object) As String
        If o Is Nothing OrElse IsDBNull(o) Then Return ""
        Return o.ToString().Trim()
    End Function

    Private Sub ToggleUI(enabled As Boolean)
        Try
            cmdview.Enabled = enabled
            combofcluster.Enabled = enabled
            combomonth.Enabled = enabled
            comboyear.Enabled = enabled
            Cursor = If(enabled, Cursors.Default, Cursors.WaitCursor)
        Catch
        End Try
    End Sub

    ' ─────────────────────────────────────────────────────────────────────────────
    ' GRID 1: Detailed RSMI (style baseline)
    ' ─────────────────────────────────────────────────────────────────────────────
    Private Sub SetupRSMIGrid()
        EnableDoubleBuffering(DataGridView1)

        With DataGridView1
            .DataSource = Nothing
            .Rows.Clear()
            .Columns.Clear()
            .AutoGenerateColumns = False
            .RowHeadersVisible = False
            .AllowUserToAddRows = False
            .AllowUserToResizeRows = False
            .MultiSelect = True
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect

            ' Flat look + header band space
            .BorderStyle = BorderStyle.None
            .CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
            .ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single
            .EnableHeadersVisualStyles = False
            .ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing
            .ColumnHeadersHeight = 64

            Dim hdrBlue As Color = Color.FromArgb(188, 210, 232)
            .ColumnHeadersDefaultCellStyle.BackColor = hdrBlue
            .ColumnHeadersDefaultCellStyle.ForeColor = Color.Black
            .ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 10.0!, FontStyle.Bold)
            .ColumnHeadersDefaultCellStyle.Padding = New Padding(0, 24, 0, 0)
            .ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

            ' ⬇️ Row styles + visible selection
            Dim selBack As Color = Color.FromArgb(230, 236, 250) ' subtle light-blue
            .DefaultCellStyle.Font = New Font("Segoe UI", 9.0!)
            .DefaultCellStyle.ForeColor = Color.Black
            .DefaultCellStyle.BackColor = Color.White
            .RowsDefaultCellStyle.BackColor = Color.White
            .AlternatingRowsDefaultCellStyle.BackColor = Color.White
            .RowsDefaultCellStyle.SelectionBackColor = selBack
            .RowsDefaultCellStyle.SelectionForeColor = Color.Black
            .AlternatingRowsDefaultCellStyle.SelectionBackColor = selBack
            .AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.Black
            .DefaultCellStyle.SelectionBackColor = selBack           ' current cell
            .DefaultCellStyle.SelectionForeColor = Color.Black

            ' Keep header from turning blue on select
            .ColumnHeadersDefaultCellStyle.SelectionBackColor = .ColumnHeadersDefaultCellStyle.BackColor
            .ColumnHeadersDefaultCellStyle.SelectionForeColor = .ColumnHeadersDefaultCellStyle.ForeColor

            .RowTemplate.Height = 34
            .GridColor = Color.FromArgb(225, 232, 240)
        End With

        DataGridView1.Columns.Add(MakeTextCol(COL_RISNO, "RIS No.", 120, DataGridViewContentAlignment.MiddleLeft))
        DataGridView1.Columns.Add(MakeTextCol(COL_RCC, "Responsibility Center Code", 220, DataGridViewContentAlignment.MiddleLeft))
        DataGridView1.Columns.Add(MakeTextCol(COL_STOCKNO, "Stock No.", 140, DataGridViewContentAlignment.MiddleLeft))
        DataGridView1.Columns.Add(MakeTextCol(COL_DESC, "Item Description", 320, DataGridViewContentAlignment.MiddleLeft, wrap:=True))
        DataGridView1.Columns.Add(MakeTextCol(COL_UNIT, "Unit", 80, DataGridViewContentAlignment.MiddleCenter))
        DataGridView1.Columns.Add(MakeTextCol(COL_QTY, "Quantity Issued", 120, DataGridViewContentAlignment.MiddleRight, fmt:="#,##0"))
        DataGridView1.Columns.Add(MakeTextCol(COL_UC, "Unit Cost", 120, DataGridViewContentAlignment.MiddleRight, fmt:="#,##0.00"))
        DataGridView1.Columns.Add(MakeTextCol(COL_AMT, "Amount", 140, DataGridViewContentAlignment.MiddleRight, fmt:="#,##0.00"))

        ' Grouped header band for Grid1
        AddHandler DataGridView1.Paint, AddressOf Dgv1_PaintGroupedHeader
        AddHandler DataGridView1.Scroll, Sub() DataGridView1.Invalidate()
        AddHandler DataGridView1.ColumnWidthChanged, Sub() DataGridView1.Invalidate()
        AddHandler DataGridView1.Resize, Sub() DataGridView1.Invalidate()
    End Sub



    ' ─────────────────────────────────────────────────────────────────────────────
    ' GRID 2: Recapitulation (same look as Grid1, centered columns)
    ' ─────────────────────────────────────────────────────────────────────────────
    Private Sub SetupRecapGrid()
        EnableDoubleBuffering(DataGridView2)

        With DataGridView2
            .DataSource = Nothing
            .Rows.Clear()
            .Columns.Clear()
            .AutoGenerateColumns = False
            .RowHeadersVisible = False
            .AllowUserToAddRows = False
            .AllowUserToResizeRows = False
            .ReadOnly = True
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect

            ' Same look as Grid1 + band space
            .BorderStyle = BorderStyle.None
            .CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
            .ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single
            .EnableHeadersVisualStyles = False
            .ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing
            .ColumnHeadersHeight = 64

            Dim hdrBlue As Color = Color.FromArgb(188, 210, 232)
            .ColumnHeadersDefaultCellStyle.BackColor = hdrBlue
            .ColumnHeadersDefaultCellStyle.ForeColor = Color.Black
            .ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 10.0!, FontStyle.Bold)
            .ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .ColumnHeadersDefaultCellStyle.Padding = New Padding(0, 24, 0, 0)

            ' ⬇️ Row styles + visible selection
            Dim selBack As Color = Color.FromArgb(230, 236, 250)
            .DefaultCellStyle.Font = New Font("Segoe UI", 9.0!)
            .DefaultCellStyle.ForeColor = Color.Black
            .DefaultCellStyle.BackColor = Color.White
            .RowsDefaultCellStyle.BackColor = Color.White
            .AlternatingRowsDefaultCellStyle.BackColor = Color.White
            .RowsDefaultCellStyle.SelectionBackColor = selBack
            .RowsDefaultCellStyle.SelectionForeColor = Color.Black
            .AlternatingRowsDefaultCellStyle.SelectionBackColor = selBack
            .AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.Black
            .DefaultCellStyle.SelectionBackColor = selBack
            .DefaultCellStyle.SelectionForeColor = Color.Black

            ' Keep header from turning blue on select
            .ColumnHeadersDefaultCellStyle.SelectionBackColor = .ColumnHeadersDefaultCellStyle.BackColor
            .ColumnHeadersDefaultCellStyle.SelectionForeColor = .ColumnHeadersDefaultCellStyle.ForeColor

            .RowTemplate.Height = 32
            .GridColor = Color.FromArgb(225, 232, 240)
        End With

        ' Centered columns
        DataGridView2.Columns.Add(MakeTextCol(COL_R_STOCKNO, "Stock No.", 220, DataGridViewContentAlignment.MiddleCenter))
        DataGridView2.Columns.Add(MakeTextCol(COL_R_QTY, "Quantity", 120, DataGridViewContentAlignment.MiddleCenter, fmt:="#,##0"))
        DataGridView2.Columns.Add(MakeTextCol(COL_R_UC, "Unit Cost", 120, DataGridViewContentAlignment.MiddleCenter, fmt:="#,##0.00"))
        DataGridView2.Columns.Add(MakeTextCol(COL_R_TC, "Total Cost", 140, DataGridViewContentAlignment.MiddleCenter, fmt:="#,##0.00"))
        DataGridView2.Columns.Add(MakeTextCol(COL_R_UACS, "UACS Object Code", 160, DataGridViewContentAlignment.MiddleCenter))

        ' Header band painter for Recapitulation
        AddHandler DataGridView2.Paint, AddressOf Dgv2_PaintBand
        AddHandler DataGridView2.Scroll, Sub() DataGridView2.Invalidate()
        AddHandler DataGridView2.ColumnWidthChanged, Sub() DataGridView2.Invalidate()
        AddHandler DataGridView2.Resize, Sub() DataGridView2.Invalidate()
    End Sub



    Private Function MakeTextCol(prop As String,
                                 header As String,
                                 width As Integer,
                                 align As DataGridViewContentAlignment,
                                 Optional fmt As String = Nothing,
                                 Optional wrap As Boolean = False) As DataGridViewTextBoxColumn

        Dim c As New DataGridViewTextBoxColumn With {
            .Name = prop,
            .HeaderText = header,
            .DataPropertyName = prop,
            .Width = width,
            .ReadOnly = True,
            .SortMode = DataGridViewColumnSortMode.NotSortable
        }
        c.DefaultCellStyle.Alignment = align
        If Not String.IsNullOrEmpty(fmt) Then c.DefaultCellStyle.Format = fmt
        If wrap Then c.DefaultCellStyle.WrapMode = DataGridViewTriState.True
        Return c
    End Function

    ' ─────────────────────────────────────────────────────────────────────────────
    ' Header painters
    ' ─────────────────────────────────────────────────────────────────────────────
    Private Sub Dgv1_PaintGroupedHeader(sender As Object, e As PaintEventArgs)
        Dim dgv = DirectCast(sender, DataGridView)
        If dgv.Columns.Count = 0 Then Return

        Dim bandHeight As Integer = 24
        Dim fontBand As New Font("Segoe UI", 9.5!, FontStyle.Regular)
        Dim backColor As Color = Color.FromArgb(214, 228, 242)
        Dim penLine As New Pen(Color.FromArgb(160, 180, 200), 1)

        Dim headerRect As Rectangle = dgv.DisplayRectangle
        headerRect.Height = dgv.ColumnHeadersHeight
        Using b As New SolidBrush(backColor)
            e.Graphics.FillRectangle(b, New Rectangle(headerRect.X, headerRect.Y, headerRect.Width, bandHeight))
        End Using

        Dim spanLeft As Rectangle = GetSpan(dgv, 0, Math.Min(5, dgv.Columns.Count - 1))
        CenterText(e.Graphics, spanLeft, "To be filled up by the Supply and/or Property Division/Unit", fontBand, Brushes.Black)

        If dgv.Columns.Count >= 8 Then
            Dim spanRight As Rectangle = GetSpan(dgv, 6, 7)
            e.Graphics.DrawLine(penLine, spanRight.Left, headerRect.Y, spanRight.Left, headerRect.Y + bandHeight)
            CenterText(e.Graphics, spanRight, "To be filled up by the Accounting Division/Unit", fontBand, Brushes.Black)
        End If

        e.Graphics.DrawLine(penLine, headerRect.Left, headerRect.Y + bandHeight, headerRect.Right, headerRect.Y + bandHeight)
    End Sub

    Private Sub Dgv2_PaintBand(sender As Object, e As PaintEventArgs)
        Dim dgv = DirectCast(sender, DataGridView)
        If dgv.Columns.Count = 0 Then Return

        Dim bandHeight As Integer = 24
        Dim fontBand As New Font("Segoe UI", 10.0!, FontStyle.Bold)
        Dim backColor As Color = Color.FromArgb(214, 228, 242)
        Dim penLine As New Pen(Color.FromArgb(160, 180, 200), 1)

        Dim headerRect As Rectangle = dgv.DisplayRectangle
        headerRect.Height = dgv.ColumnHeadersHeight
        Using b As New SolidBrush(backColor)
            e.Graphics.FillRectangle(b, New Rectangle(headerRect.X, headerRect.Y, headerRect.Width, bandHeight))
        End Using

        Dim spanAll As Rectangle = GetSpan(dgv, 0, dgv.Columns.Count - 1)
        CenterText(e.Graphics, spanAll, "RECAPITULATION", fontBand, Brushes.Black)

        e.Graphics.DrawLine(penLine, headerRect.Left, headerRect.Y + bandHeight, headerRect.Right, headerRect.Y + bandHeight)
    End Sub

    Private Function GetSpan(dgv As DataGridView, firstColIdx As Integer, lastColIdx As Integer) As Rectangle
        Dim left As Integer = dgv.GetCellDisplayRectangle(firstColIdx, -1, True).Left
        Dim right As Integer = dgv.GetCellDisplayRectangle(lastColIdx, -1, True).Right
        Return New Rectangle(left, dgv.DisplayRectangle.Y, right - left, 24)
    End Function

    Private Sub CenterText(g As Graphics, r As Rectangle, text As String, f As Font, br As Brush)
        Dim sf As New StringFormat With {
            .Alignment = StringAlignment.Center,
            .LineAlignment = StringAlignment.Center,
            .Trimming = StringTrimming.EllipsisCharacter,
            .FormatFlags = StringFormatFlags.NoWrap
        }
        g.DrawString(text, f, br, r, sf)
    End Sub

    ' ─────────────────────────────────────────────────────────────────────────────
    ' Double buffering
    ' ─────────────────────────────────────────────────────────────────────────────
    Private Sub EnableDoubleBuffering(dgv As DataGridView)
        Try
            dgv.GetType().InvokeMember("DoubleBuffered",
                                       Reflection.BindingFlags.NonPublic Or
                                       Reflection.BindingFlags.Instance Or
                                       Reflection.BindingFlags.SetProperty,
                                       Nothing, dgv, New Object() {True})
            dgv.GetType().InvokeMember("ResizeRedraw",
                                       Reflection.BindingFlags.NonPublic Or
                                       Reflection.BindingFlags.Instance Or
                                       Reflection.BindingFlags.SetProperty,
                                       Nothing, dgv, New Object() {True})
        Catch
        End Try
    End Sub

    Private Sub Panel2_Paint(sender As Object, e As PaintEventArgs) Handles Panel2.Paint

    End Sub

    Private Sub cmdprint_Click(sender As Object, e As EventArgs) Handles cmdprint.Click
        Try
            ' ─────────────────────────────────────────────────────────────────────────
            ' Validate inputs / data presence
            ' ─────────────────────────────────────────────────────────────────────────
            Dim fcluster As String = SafeText(combofcluster.Text)
            If String.IsNullOrWhiteSpace(fcluster) Then
                MessageBox.Show("Please select a Fund Cluster (FCLUSTER).", "RSMI", MessageBoxButtons.OK, MessageBoxIcon.Information)
                combofcluster.Focus()
                Exit Sub
            End If

            Dim yStr As String = SafeText(comboyear.Text)
            Dim year As Integer
            If Not Integer.TryParse(yStr, year) OrElse year < 1900 OrElse year > 9999 Then
                MessageBox.Show("Please select a valid Year.", "RSMI", MessageBoxButtons.OK, MessageBoxIcon.Information)
                comboyear.Focus()
                Exit Sub
            End If

            Dim mm2 As String = ResolveMonth2(combomonth.Text)
            If String.IsNullOrEmpty(mm2) Then
                MessageBox.Show("Please select a valid Month.", "RSMI", MessageBoxButtons.OK, MessageBoxIcon.Information)
                combomonth.Focus()
                Exit Sub
            End If

            If DataGridView1 Is Nothing OrElse DataGridView1.Rows.Count = 0 Then
                MessageBox.Show("No detailed RSMI data to print. Click VIEW first.", "RSMI", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Exit Sub
            End If

            If DataGridView2 Is Nothing OrElse DataGridView2.Rows.Count = 0 Then
                MessageBox.Show("No Recapitulation data to print. Click VIEW first.", "RSMI", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Exit Sub
            End If

            ' ─────────────────────────────────────────────────────────────────────────
            ' Build DSRSMI.DTRSMI
            ' ─────────────────────────────────────────────────────────────────────────
            Dim dtRSMI As New DataTable("DTRSMI")
            dtRSMI.Columns.Add("RIS_NO", GetType(String))
            dtRSMI.Columns.Add("RCCODE", GetType(String))
            dtRSMI.Columns.Add("STOCK_NO", GetType(String))
            dtRSMI.Columns.Add("DESC", GetType(String))
            dtRSMI.Columns.Add("UNIT", GetType(String))
            dtRSMI.Columns.Add("QISSUED", GetType(Decimal))
            dtRSMI.Columns.Add("UCOST", GetType(Decimal))
            dtRSMI.Columns.Add("AMOUNT", GetType(Decimal))

            For Each r As DataGridViewRow In DataGridView1.Rows
                If r.IsNewRow Then Continue For
                Dim newRow = dtRSMI.NewRow()
                newRow("RIS_NO") = GetStr(r.Cells(COL_RISNO).Value)
                newRow("RCCODE") = GetStr(r.Cells(COL_RCC).Value)
                newRow("STOCK_NO") = GetStr(r.Cells(COL_STOCKNO).Value)
                newRow("DESC") = GetStr(r.Cells(COL_DESC).Value)
                newRow("UNIT") = GetStr(r.Cells(COL_UNIT).Value)
                newRow("QISSUED") = GetDec(r.Cells(COL_QTY).Value)
                newRow("UCOST") = GetDec(r.Cells(COL_UC).Value)
                newRow("AMOUNT") = GetDec(r.Cells(COL_AMT).Value)
                dtRSMI.Rows.Add(newRow)
            Next

            ' ─────────────────────────────────────────────────────────────────────────
            ' Build DSRSMI.DTRECAP  (RDLC dataset name: DSRSMIRECAP)
            ' ─────────────────────────────────────────────────────────────────────────
            Dim dtRecap As New DataTable("DTRECAP")
            dtRecap.Columns.Add("STOCK_NO", GetType(String))
            dtRecap.Columns.Add("QUANTITY", GetType(Decimal))
            dtRecap.Columns.Add("UCOST", GetType(Decimal))
            dtRecap.Columns.Add("TCOST", GetType(Decimal))
            dtRecap.Columns.Add("UACSOCODE", GetType(String))

            For Each r As DataGridViewRow In DataGridView2.Rows
                If r.IsNewRow Then Continue For
                Dim newRow = dtRecap.NewRow()
                newRow("STOCK_NO") = GetStr(r.Cells(COL_R_STOCKNO).Value)
                newRow("QUANTITY") = GetDec(r.Cells(COL_R_QTY).Value)
                newRow("UCOST") = GetDec(r.Cells(COL_R_UC).Value)
                newRow("TCOST") = GetDec(r.Cells(COL_R_TC).Value)
                newRow("UACSOCODE") = GetStr(r.Cells(COL_R_UACS).Value)
                dtRecap.Rows.Add(newRow)
            Next

            ' ─────────────────────────────────────────────────────────────────────────
            ' Prepare ReportViewer on frmpprev (no code inside that form)
            ' ─────────────────────────────────────────────────────────────────────────
            Dim rdlcPath As String = System.IO.Path.Combine(Application.StartupPath, "report", "rptrsmi.rdlc")
            If Not System.IO.File.Exists(rdlcPath) Then
                MessageBox.Show("Report file not found: " & rdlcPath, "RSMI", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            Dim monthText As String = SafeText(combomonth.Text)
            Dim monthParamValue As String = monthText & " " & year.ToString("0000")
            Dim snoPrefix As String = year.ToString("0000") & "-" & mm2 & "-"   ' e.g., "2025-01-"

            Dim prm As New List(Of Microsoft.Reporting.WinForms.ReportParameter) From {
                New Microsoft.Reporting.WinForms.ReportParameter("FCLUSTER", fcluster, True),
                New Microsoft.Reporting.WinForms.ReportParameter("SNO", snoPrefix, True),
                New Microsoft.Reporting.WinForms.ReportParameter("MONTH", monthParamValue, True)
            }

            Dim preview As New frmpprev()
            ' Find an existing ReportViewer control on frmpprev (no code-behind needed)
            Dim rv As Microsoft.Reporting.WinForms.ReportViewer = FindReportViewer(preview)
            If rv Is Nothing Then
                preview.Dispose()
                MessageBox.Show("No ReportViewer found on frmpprev. Please drop a ReportViewer control (e.g., name it ReportViewer1).", "RSMI", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            ' Configure viewer
            rv.Reset()
            rv.ProcessingMode = Microsoft.Reporting.WinForms.ProcessingMode.Local
            rv.LocalReport.ReportPath = rdlcPath
            rv.LocalReport.DataSources.Clear()
            rv.LocalReport.DataSources.Add(New Microsoft.Reporting.WinForms.ReportDataSource("DSRSMI", dtRSMI))
            rv.LocalReport.DataSources.Add(New Microsoft.Reporting.WinForms.ReportDataSource("DSRSMIRECAP", dtRecap))
            rv.LocalReport.SetParameters(prm)

            rv.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout)
            rv.ZoomMode = Microsoft.Reporting.WinForms.ZoomMode.Percent
            rv.ZoomPercent = 100

            rv.RefreshReport()

            preview.StartPosition = FormStartPosition.CenterScreen
            preview.WindowState = FormWindowState.Maximized
            preview.panelsubmit.Visible = False
            preview.ShowDialog()
            preview.Dispose()

            ' RDLC tip for serial number per page:
            '   =Parameters!SNO.Value & Format(Globals!PageNumber, "0000")
            ' yields 2025-01-0001, 2025-01-0002, ...
        Catch ex As Exception
            MessageBox.Show("Failed to render RSMI report." & vbCrLf & ex.Message, "RSMI", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' Find the first ReportViewer control on a form (no code required in frmpprev)
    Private Function FindReportViewer(container As Control) As Microsoft.Reporting.WinForms.ReportViewer
        For Each c As Control In container.Controls
            If TypeOf c Is Microsoft.Reporting.WinForms.ReportViewer Then
                Return DirectCast(c, Microsoft.Reporting.WinForms.ReportViewer)
            End If
            Dim nested = FindReportViewer(c)
            If nested IsNot Nothing Then Return nested
        Next
        Return Nothing
    End Function

End Class
