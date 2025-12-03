' frmStockCard.vb
Imports System.Data
Imports System.Data.SqlClient
Imports System.Drawing
Imports System.Windows.Forms
Imports System.Reflection
Imports Microsoft.Reporting.WinForms

Public Class frmStockCard

    Private Sub frmStockCard_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        SetupGrid()
        FillYearRange()
        LoadDescriptions()
        combodesc.AutoCompleteMode = AutoCompleteMode.SuggestAppend
        combodesc.AutoCompleteSource = AutoCompleteSource.ListItems
    End Sub

    Private Sub combodesc_SelectedIndexChanged(sender As Object, e As EventArgs) Handles combodesc.SelectedIndexChanged
        If combodesc.SelectedIndex >= 0 Then
            LoadInventoryHeader(combodesc.Text)
            FillYearRange()
            TryPopulateGrid()
        End If
    End Sub

    Private Sub comboyear_SelectedIndexChanged(sender As Object, e As EventArgs) Handles comboyear.SelectedIndexChanged
        TryPopulateGrid()
    End Sub

    '──────────────────────────────────────────────────────────────────────────────
    ' GRID SETUP (centered + visible highlight + no flicker)
    '──────────────────────────────────────────────────────────────────────────────
    Private Sub SetupGrid()
        With DataGridView1
            .DataSource = Nothing
            .Rows.Clear()
            .Columns.Clear()
            .AutoGenerateColumns = False
            .RowHeadersVisible = False
            .AllowUserToAddRows = False
            .AllowUserToResizeRows = False
            .MultiSelect = False
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
            .ReadOnly = True

            .BorderStyle = BorderStyle.FixedSingle
            .CellBorderStyle = DataGridViewCellBorderStyle.Single
            .ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single

            Dim hdrColor As Color = Color.FromArgb(180, 200, 230)
            Dim cellBack As Color = Color.White
            Dim cellFore As Color = Color.Black
            Dim selBack As Color = Color.FromArgb(220, 235, 255)
            Dim selFore As Color = Color.Black

            .EnableHeadersVisualStyles = False

            ' Header style
            .ColumnHeadersDefaultCellStyle.BackColor = hdrColor
            .ColumnHeadersDefaultCellStyle.ForeColor = cellFore
            .ColumnHeadersDefaultCellStyle.SelectionBackColor = hdrColor
            .ColumnHeadersDefaultCellStyle.SelectionForeColor = cellFore
            .ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 10.0!, FontStyle.Bold)
            .ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .ColumnHeadersDefaultCellStyle.Padding = New Padding(0, 0, 0, 2)

            ' Cell style
            .DefaultCellStyle.Font = New Font("Segoe UI", 9.0!)
            .DefaultCellStyle.ForeColor = cellFore
            .DefaultCellStyle.BackColor = cellBack
            .RowsDefaultCellStyle.BackColor = cellBack
            .AlternatingRowsDefaultCellStyle.BackColor = cellBack
            .DefaultCellStyle.SelectionBackColor = selBack
            .DefaultCellStyle.SelectionForeColor = selFore
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

            .RowTemplate.Height = 34
            .RowTemplate.DefaultCellStyle.Padding = New Padding(0, 6, 0, 6)

            ' Give more space so the divider never clips the text
            .ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
            .ColumnHeadersHeight = 76
        End With

        ' Columns
        DataGridView1.Columns.Add(MakeCol("IAR_DATE", "IAR Date", 120, "MM/dd/yyyy"))
        DataGridView1.Columns.Add(MakeCol("IAR_SERIES", "IAR Series", 150))
        DataGridView1.Columns.Add(MakeCol("IAR_QTY", "Qty.", 70, "##,###"))

        DataGridView1.Columns.Add(MakeCol("RIS_IAR_DATE", "RIS/IAR Date", 120, "MM/dd/yyyy"))
        DataGridView1.Columns.Add(MakeCol("RIS_SERIES", "RIS Series", 150))
        DataGridView1.Columns.Add(MakeCol("ISSUED_TO", "Issued To (Office)", 220))
        DataGridView1.Columns.Add(MakeCol("ISS_QTY", "Qty.", 70, "##,###"))

        DataGridView1.Columns.Add(MakeCol("BAL_QTY", "Qty.", 70, "##,###"))

        EnableDoubleBuffering(DataGridView1)
    End Sub

    Private Function MakeCol(name As String, header As String, width As Integer, Optional fmt As String = Nothing) As DataGridViewTextBoxColumn
        Dim c As New DataGridViewTextBoxColumn()
        c.Name = name
        c.HeaderText = header
        c.DataPropertyName = name
        c.Width = width
        c.ReadOnly = True
        c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        c.SortMode = DataGridViewColumnSortMode.NotSortable
        If Not String.IsNullOrEmpty(fmt) Then c.DefaultCellStyle.Format = fmt
        Return c
    End Function

    Private Sub EnableDoubleBuffering(dgv As DataGridView)
        Dim t As Type = dgv.GetType()
        Dim pi = t.GetProperty("DoubleBuffered", BindingFlags.Instance Or BindingFlags.NonPublic)
        If pi IsNot Nothing Then pi.SetValue(dgv, True, Nothing)
    End Sub

    '──────────────────────────────────────────────────────────────────────────────
    ' GROUP HEADERS
    '──────────────────────────────────────────────────────────────────────────────
    Private Sub DataGridView1_Paint(sender As Object, e As PaintEventArgs) Handles DataGridView1.Paint
        If DataGridView1.Columns.Count = 0 Then Exit Sub

        Dim g As Graphics = e.Graphics
        g.TextRenderingHint = Drawing.Text.TextRenderingHint.ClearTypeGridFit

        Dim groupBack As Color = Color.FromArgb(211, 225, 242)
        Dim borderColor As Color = Color.FromArgb(160, 180, 210)
        Dim textBrush As Brush = Brushes.Black
        Dim font As New Font("Segoe UI", 10.0!, FontStyle.Bold)

        Dim topBandHeight As Integer = DataGridView1.ColumnHeadersHeight \ 2
        Dim bottomBandY As Integer = topBandHeight

        Dim rReceipts As Rectangle = GetGroupRect(0, 2)
        Dim rIssuances As Rectangle = GetGroupRect(3, 6)
        Dim rBalance As Rectangle = GetGroupRect(7, 7)

        rReceipts.Height = topBandHeight
        rIssuances.Height = topBandHeight
        rBalance.Height = topBandHeight

        Using back As New SolidBrush(groupBack)
            g.FillRectangle(back, rReceipts)
            g.FillRectangle(back, rIssuances)
            g.FillRectangle(back, rBalance)
        End Using
        Using p As New Pen(borderColor, 1)
            g.DrawRectangle(p, rReceipts)
            g.DrawRectangle(p, rIssuances)
            g.DrawRectangle(p, rBalance)
            g.DrawLine(p, 0, bottomBandY - 1, DataGridView1.Width, bottomBandY - 1)
        End Using

        DrawCenterText(g, "RECEIPTS", rReceipts, font, textBrush)
        DrawCenterText(g, "ISSUANCES", rIssuances, font, textBrush)
        DrawCenterText(g, "BALANCE", rBalance, font, textBrush)
    End Sub

    Private Sub DataGridView1_CellPainting(sender As Object, e As DataGridViewCellPaintingEventArgs) Handles DataGridView1.CellPainting
        If e.RowIndex = -1 AndAlso e.ColumnIndex >= 0 Then
            e.PaintBackground(e.ClipBounds, False)
            Dim halfY As Integer = DataGridView1.ColumnHeadersHeight \ 2
            Dim pad As Integer = 3
            Dim halfRect As New Rectangle(
                e.CellBounds.X,
                e.CellBounds.Y + halfY + pad,
                e.CellBounds.Width,
                e.CellBounds.Height - halfY - (pad * 2)
            )
            TextRenderer.DrawText(e.Graphics, e.FormattedValue?.ToString(),
                                  e.CellStyle.Font, halfRect, e.CellStyle.ForeColor,
                                  TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter Or TextFormatFlags.WordEllipsis)
            e.Handled = True
        End If
    End Sub

    Private Function GetGroupRect(first As Integer, last As Integer) As Rectangle
        Dim startRect = DataGridView1.GetCellDisplayRectangle(first, -1, True)
        Dim endRect = DataGridView1.GetCellDisplayRectangle(last, -1, True)
        Return New Rectangle(startRect.X, 0, endRect.Right - startRect.X - 1, DataGridView1.ColumnHeadersHeight \ 2)
    End Function

    Private Sub DrawCenterText(g As Graphics, text As String, rect As Rectangle, f As Font, brush As Brush)
        Dim sf As New StringFormat With {.Alignment = StringAlignment.Center, .LineAlignment = StringAlignment.Center}
        g.DrawString(text, f, brush, rect, sf)
    End Sub

    Private Sub DataGridView1_Resize(sender As Object, e As EventArgs) Handles DataGridView1.Resize
        DataGridView1.Invalidate()
    End Sub
    Private Sub DataGridView1_ColumnWidthChanged(sender As Object, e As DataGridViewColumnEventArgs) Handles DataGridView1.ColumnWidthChanged
        DataGridView1.Invalidate()
    End Sub

    '──────────────────────────────────────────────────────────────────────────────
    ' YEAR RANGE
    '──────────────────────────────────────────────────────────────────────────────
    Private Sub FillYearRange()
        comboyear.Items.Clear()
        For y As Integer = 2024 To 2033
            comboyear.Items.Add(y.ToString())
        Next
        Dim cur As Integer = Date.Now.Year
        If cur >= 2024 AndAlso cur <= 2033 Then
            comboyear.SelectedItem = cur.ToString()
        Else
            comboyear.SelectedIndex = 0
        End If
    End Sub

    '──────────────────────────────────────────────────────────────────────────────
    ' DATA LOADING
    '──────────────────────────────────────────────────────────────────────────────
    Private Sub LoadDescriptions()
        combodesc.Items.Clear()
        Using conn As New SqlConnection(frmmain.txtdb.Text)
            conn.Open()
            Using cmd As New SqlCommand("
                SELECT DESCRIPTIONS
                FROM TBL_INVENTORY WITH (NOLOCK)
                WHERE ISNULL(DESCRIPTIONS,'') <> ''
                ORDER BY DESCRIPTIONS;", conn)
                Using rd = cmd.ExecuteReader()
                    While rd.Read()
                        combodesc.Items.Add(rd.GetString(0))
                    End While
                End Using
            End Using
        End Using
    End Sub

    Private Sub LoadInventoryHeader(descText As String)
        txtino.Clear()
        txtunit.Clear()
        Using conn As New SqlConnection(frmmain.txtdb.Text)
            conn.Open()
            Using cmd As New SqlCommand("
                SELECT TOP 1 IPNO, UNITS
                FROM TBL_INVENTORY WITH (NOLOCK)
                WHERE DESCRIPTIONS = @DESC;", conn)
                cmd.Parameters.AddWithValue("@DESC", descText)
                Using rd = cmd.ExecuteReader()
                    If rd.Read() Then
                        txtino.Text = If(rd.IsDBNull(0), "", rd.GetString(0))
                        txtunit.Text = If(rd.IsDBNull(1), "", rd.GetString(1))
                    End If
                End Using
            End Using
        End Using
    End Sub

    Private Sub TryPopulateGrid()
        If combodesc.SelectedIndex < 0 OrElse comboyear.SelectedIndex < 0 Then Return
        PopulateGrid(combodesc.Text, CInt(comboyear.Text))
    End Sub

    Private Sub PopulateGrid(descText As String, yearVal As Integer)
        Dim receipts As New List(Of ReceiptRow)()
        Dim issuances As New List(Of IssuanceRow)()

        Using conn As New SqlConnection(frmmain.txtdb.Text)
            conn.Open()

            ' RECEIPTS
            Using cmd1 As New SqlCommand("
                SELECT I.IARDATE, I.IARNO, II.QUANTITY
                FROM TBL_IARITEMS II
                JOIN TBL_IAR I ON II.IARID = I.ID
                WHERE II.DESCRIPTIONS = @DESC
                  AND YEAR(I.IARDATE) = @YR
                ORDER BY I.IARDATE, I.IARNO;", conn)
                cmd1.Parameters.AddWithValue("@DESC", descText)
                cmd1.Parameters.AddWithValue("@YR", yearVal)
                Using rd = cmd1.ExecuteReader()
                    While rd.Read()
                        receipts.Add(New ReceiptRow With {
                            .IARDate = SafeGetDate(rd, 0),
                            .IARNo = SafeGetString(rd, 1),
                            .Qty = SafeGetDecimal(rd, 2)
                        })
                    End While
                End Using
            End Using

            ' ISSUANCES (now includes RI.SQUANTITY AS the balance to display)
            Using cmd2 As New SqlCommand("
                SELECT R.RIS_DATE, R.RIS_NO, R.REC_BY_NAME, RI.QUANTITY, RI.SQUANTITY
                FROM TBL_RISITEMS RI
                JOIN TBL_RIS R ON RI.RIS_NO = R.RIS_NO
                WHERE RI.DESCRIPTIONS = @DESC
                  AND YEAR(R.RIS_DATE) = @YR
                ORDER BY R.RIS_DATE, R.RIS_NO;", conn)
                cmd2.Parameters.AddWithValue("@DESC", descText)
                cmd2.Parameters.AddWithValue("@YR", yearVal)
                Using rd = cmd2.ExecuteReader()
                    While rd.Read()
                        issuances.Add(New IssuanceRow With {
                            .RISDate = SafeGetDate(rd, 0),
                            .RISNo = SafeGetString(rd, 1),
                            .IssuedTo = SafeGetString(rd, 2),
                            .Qty = SafeGetDecimal(rd, 3),
                            .BalQty = SafeGetDecimal(rd, 4)   ' <<<<<<<<<<<<<< use SQUANTITY
                        })
                    End While
                End Using
            End Using
        End Using

        ' Build display table
        Dim dt As New DataTable()
        dt.Columns.AddRange({
            New DataColumn("IAR_DATE", GetType(Date)),
            New DataColumn("IAR_SERIES", GetType(String)),
            New DataColumn("IAR_QTY", GetType(Decimal)),
            New DataColumn("RIS_IAR_DATE", GetType(Date)),
            New DataColumn("RIS_SERIES", GetType(String)),
            New DataColumn("ISSUED_TO", GetType(String)),
            New DataColumn("ISS_QTY", GetType(Decimal)),
            New DataColumn("BAL_QTY", GetType(Decimal))
        })

        Dim maxRows As Integer = Math.Max(receipts.Count, issuances.Count)
        Dim lastBal As Decimal = 0D

        For i As Integer = 0 To maxRows - 1
            Dim r As DataRow = dt.NewRow()

            If i < receipts.Count Then
                r("IAR_DATE") = receipts(i).IARDate
                r("IAR_SERIES") = receipts(i).IARNo
                r("IAR_QTY") = receipts(i).Qty
            End If

            If i < issuances.Count Then
                r("RIS_IAR_DATE") = issuances(i).RISDate
                r("RIS_SERIES") = issuances(i).RISNo
                r("ISSUED_TO") = issuances(i).IssuedTo
                r("ISS_QTY") = issuances(i).Qty

                ' Balance from TBL_RISITEMS.SQUANTITY (per your revision)
                lastBal = issuances(i).BalQty
                r("BAL_QTY") = lastBal
            Else
                ' No issuance on this row => carry forward last known balance
                r("BAL_QTY") = lastBal
            End If

            dt.Rows.Add(r)
        Next

        If dt.Rows.Count = 0 Then
            Dim empty As DataRow = dt.NewRow()
            empty("BAL_QTY") = 0D
            dt.Rows.Add(empty)
        End If

        DataGridView1.DataSource = dt
    End Sub

    '──────────────────────────────────────────────────────────────────────────────
    ' HELPERS
    '──────────────────────────────────────────────────────────────────────────────
    Private Structure ReceiptRow
        Public IARDate As Date
        Public IARNo As String
        Public Qty As Decimal
    End Structure

    Private Structure IssuanceRow
        Public RISDate As Date
        Public RISNo As String
        Public IssuedTo As String
        Public Qty As Decimal
        Public BalQty As Decimal   ' from TBL_RISITEMS.SQUANTITY
    End Structure

    Private Function SafeGetString(rd As SqlDataReader, i As Integer) As String
        If rd.IsDBNull(i) Then Return "" Else Return rd.GetString(i)
    End Function
    Private Function SafeGetDate(rd As SqlDataReader, i As Integer) As Date
        If rd.IsDBNull(i) Then Return Date.MinValue Else Return rd.GetDateTime(i)
    End Function
    Private Function SafeGetDecimal(rd As SqlDataReader, i As Integer) As Decimal
        If rd.IsDBNull(i) Then Return 0D Else Return Convert.ToDecimal(rd.GetValue(i))
    End Function

    ' ===================== PRINT (uses frmpprev as-is) =====================
    Private Sub cmdprint_Click(sender As Object, e As EventArgs) Handles cmdprint.Click
        Try
            ' 1) Fill your typed DataSet from the grid
            Dim ds As New DSSTOCKCARD()

            For Each r As DataGridViewRow In DataGridView1.Rows
                If r.IsNewRow Then Continue For

                Dim rw As DSSTOCKCARD.DTSTOCKCARDRow = ds.DTSTOCKCARD.NewDTSTOCKCARDRow()

                If IsDate(r.Cells("IAR_DATE").Value) Then rw.IAR_DATE = CDate(r.Cells("IAR_DATE").Value) Else rw.SetIAR_DATENull()
                If IsDate(r.Cells("RIS_IAR_DATE").Value) Then rw.RIS_IAR_DATE = CDate(r.Cells("RIS_IAR_DATE").Value) Else rw.SetRIS_IAR_DATENull()

                rw.IAR_SERIES = If(r.Cells("IAR_SERIES").Value, "").ToString()
                rw.RIS_SERIES = If(r.Cells("RIS_SERIES").Value, "").ToString()
                rw.ISSUED_TO = If(r.Cells("ISSUED_TO").Value, "").ToString()

                rw.IAR_QTY = ToDecimal(r.Cells("IAR_QTY").Value)
                rw.ISS_QTY = ToDecimal(r.Cells("ISS_QTY").Value)
                rw.BAL_QTY = ToDecimal(r.Cells("BAL_QTY").Value)

                ds.DTSTOCKCARD.AddDTSTOCKCARDRow(rw)
            Next

            ' 2) RDLC path and RDLC DataSet name
            Dim reportPath As String = System.IO.Path.Combine(Application.StartupPath, "Report\rptStockCard.rdlc")
            Dim rdlcDataSetName As String = "DSSTOCKCARD"   ' must match RDLC dataset name

            ' 3) Parameters
            Dim ps As ReportParameter() = {
                New ReportParameter("DESC", combodesc.Text),
                New ReportParameter("INO", txtino.Text),
                New ReportParameter("FCLUSTER", combofcluster.Text),
                 New ReportParameter("UNITS", txtunit.Text),
                New ReportParameter("REORDER", TXTEORDER.Text)
            }

            ' 4) Show in frmpprev
            Dim prev As New frmpprev()
            Dim rv As ReportViewer = FindReportViewer(prev)
            If rv Is Nothing Then Throw New InvalidOperationException("No ReportViewer found in frmpprev.")

            rv.Reset()
            rv.ProcessingMode = ProcessingMode.Local
            rv.LocalReport.ReportPath = reportPath
            rv.LocalReport.DataSources.Clear()

            Dim rds As New ReportDataSource(rdlcDataSetName, CType(ds.DTSTOCKCARD, DataTable))
            rv.LocalReport.DataSources.Add(rds)

            rv.LocalReport.SetParameters(ps)
            rv.ZoomMode = ZoomMode.PageWidth
            rv.SetDisplayMode(DisplayMode.PrintLayout)
            rv.ZoomMode = ZoomMode.Percent
            rv.ZoomPercent = 100
            rv.RefreshReport()

            prev.panelsubmit.Visible = False
            prev.ShowDialog(Me)

        Catch ex As Exception
            MessageBox.Show("Print error: " & ex.Message, "Report", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' =============== Helper: find ReportViewer inside frmpprev ===============
    Private Function FindReportViewer(container As Control) As Microsoft.Reporting.WinForms.ReportViewer
        For Each c As Control In container.Controls
            Dim rv = TryCast(c, Microsoft.Reporting.WinForms.ReportViewer)
            If rv IsNot Nothing Then Return rv
            Dim nested = FindReportViewer(c)
            If nested IsNot Nothing Then Return nested
        Next
        Return Nothing
    End Function

    Private Function ToDecimal(o As Object) As Decimal
        If o Is Nothing OrElse o Is DBNull.Value Then Return 0D
        Dim s As String = o.ToString().Trim()
        If s = "" Then Return 0D
        Dim d As Decimal
        If Decimal.TryParse(s, Globalization.NumberStyles.Any, Globalization.CultureInfo.CurrentCulture, d) Then Return d
        If Decimal.TryParse(s, Globalization.NumberStyles.Any, Globalization.CultureInfo.InvariantCulture, d) Then Return d
        Return 0D
    End Function

End Class
