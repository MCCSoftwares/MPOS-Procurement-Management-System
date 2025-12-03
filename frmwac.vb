Imports System.Data
Imports System.Data.SqlClient
Imports System.Drawing
Imports System.Reflection
Imports System.Drawing.Drawing2D
Imports System.Drawing.Text

Public Class frmwac

    '======================== TBL_WAC schema ========================
    Private Const COL_ID As String = "ID"
    Private Const COL_PID As String = "P_ID"                  ' nchar(10)
    Private Const COL_DESC As String = "DESCRIPTIONS"
    Private Const COL_DATE As String = "WAC_DATE"

    Private Const COL_R_Q As String = "RECEIPT_QTY"
    Private Const COL_R_TC As String = "RECEIPT_TOTAL_COST"

    Private Const COL_I_Q As String = "ISSUANCE_QTY"
    Private Const COL_I_UC As String = "ISSUANCE_UNIT_COST"
    Private Const COL_I_TC As String = "ISSUANCE_TOTAL_COST"

    Private Const COL_WAC_Q As String = "WAC_QTY"
    Private Const COL_WAC_UC As String = "WAC_UNIT_COST"
    Private Const COL_WAC_TC As String = "WAC_TOTAL_COST"

    Private Const COL_BAL_Q As String = "BAL_QTY"
    Private Const COL_BAL_UC As String = "BAL_UNIT_COST"
    Private Const COL_BAL_TC As String = "BAL_TOTAL_COST"

    '======================== State ========================
    Private _dt As DataTable
    Private _loading As Boolean = False
    Private _suppressSuggest As Boolean = False
    Private _selectedItem As InvItem = Nothing
    Private _savingRecalc As Boolean = False

    ' Cascade triggers (only these cause forward recompute)
    Private ReadOnly _recalcTriggers As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase) From {
        COL_R_Q, COL_R_TC, COL_I_Q
    }

    ' Decimal-only columns
    Private ReadOnly _numericCols As HashSet(Of String) =
        New HashSet(Of String)(StringComparer.OrdinalIgnoreCase) From {
            COL_R_Q, COL_R_TC, COL_I_Q, COL_I_UC, COL_I_TC,
            COL_WAC_Q, COL_WAC_UC, COL_WAC_TC,
            COL_BAL_Q, COL_BAL_UC, COL_BAL_TC
        }

    ' Suggest item
    Private Class InvItem
        Public Property P_ID As Integer
        Public Property Descriptions As String
        Public Overrides Function ToString() As String
            Return Descriptions
        End Function
    End Class

    '======================== Colors (close to your sheet) ========================
    Private ReadOnly C_BEIGE As Color = Color.FromArgb(246, 221, 164)    ' Dates
    Private ReadOnly C_PINK As Color = Color.FromArgb(255, 224, 224)     ' Receipt
    Private ReadOnly C_GREEN As Color = Color.FromArgb(223, 246, 223)    ' Issuance
    Private ReadOnly C_BLUE As Color = Color.FromArgb(214, 226, 243)     ' WAC Q/TC
    Private ReadOnly C_WAC_UC As Color = Color.FromArgb(228, 213, 183)   ' WAC UC (distinct)
    Private ReadOnly C_ORANGE As Color = Color.FromArgb(249, 222, 173)   ' Balance

    ' Group banner (top row) colors
    Private ReadOnly G_ITEM As Color = Color.FromArgb(255, 241, 168)
    Private ReadOnly G_WAC As Color = Color.FromArgb(198, 217, 240)
    Private ReadOnly G_BAL As Color = Color.FromArgb(248, 207, 166)

    ' Header layout
    Private Const TOP_HDR_HEIGHT As Integer = 24
    Private Const TOTAL_HDR_HEIGHT As Integer = 76
    Private _hdrPanel As DoubleBufferedPanel

    ' Rounding constants
    Private Const DEC As Integer = 2 ' all stored as DECIMAL(18,2)

    '======================== Load ========================
    Private Sub frmwac_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            PrepareComboYear()
            PrepareComboDesc() ' now loads all items from TBL_INVENTORY into the dropdown
            EnsureGridReady()
            InitHeaderPanel()
            UpdateLblWac(clearOnly:=True)
            SetStatus("Ready.", True)
        Catch ex As Exception
            SetStatus("Load error: " & ex.Message, False)
            MessageBox.Show("Load error: " & ex.Message, "WAC", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    '======================== Status ========================
    Private Sub SetStatus(text As String, Optional ok As Boolean? = Nothing)
        If lblstatus Is Nothing Then Return
        lblstatus.Text = text
        lblstatus.ForeColor = If(ok.HasValue, If(ok.Value, Color.SeaGreen, Color.Firebrick), Color.DimGray)
        lblstatus.Refresh()
    End Sub

    '======================== Year combo ========================
    Private Sub PrepareComboYear()
        comboyear.DropDownStyle = ComboBoxStyle.DropDownList
        Dim yrNow As Integer = Date.Today.Year
        If comboyear.Items.Count = 0 Then
            For y = yrNow - 3 To yrNow + 5
                comboyear.Items.Add(y.ToString())
            Next
        End If
        Dim yText = yrNow.ToString()
        If Not comboyear.Items.Contains(yText) Then comboyear.Items.Add(yText)
        comboyear.SelectedItem = yText
        AddHandler comboyear.SelectedIndexChanged, Sub() UpdateLblWac(clearOnly:=True)
    End Sub

    '======================== Description combo (NOW: fixed list from TBL_INVENTORY) ========================
    Private Async Sub PrepareComboDesc()
        With combodesc
            ' Switch to a strict dropdown list (no typing; just pick from items)
            .DropDownStyle = ComboBoxStyle.DropDownList
            .IntegralHeight = False
            .MaxDropDownItems = 16
            .FormattingEnabled = True
        End With

        ' Keep your handlers (no deletions). TextChanged will early-exit for DropDownList.
        AddHandler combodesc.TextChanged, AddressOf combodesc_TextChanged
        AddHandler combodesc.SelectionChangeCommitted, AddressOf combodesc_SelectionChangeCommitted
        AddHandler combodesc.Leave, Sub() combodesc.DroppedDown = False

        ' Load all items from TBL_INVENTORY
        Try
            Dim items As New List(Of InvItem)
            Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
                Await conn.OpenAsync()
                Using cmd As New SqlCommand("SELECT ID, DESCRIPTIONS FROM TBL_INVENTORY WITH (NOLOCK) ORDER BY DESCRIPTIONS;", conn)
                    Using rd = Await cmd.ExecuteReaderAsync()
                        While Await rd.ReadAsync()
                            items.Add(New InvItem With {
                                .P_ID = rd.GetInt32(0),
                                .Descriptions = rd.GetString(1)
                            })
                        End While
                    End Using
                End Using
            End Using

            combodesc.BeginUpdate()
            Try
                combodesc.Items.Clear()
                For Each it In items
                    combodesc.Items.Add(it)
                Next
            Finally
                combodesc.EndUpdate()
            End Try

            ' Do not preselect—let the user choose. Clear current backing state.
            _selectedItem = Nothing
            UpdateLblWac(clearOnly:=True)
        Catch ex As Exception
            SetStatus("Load inventory error: " & ex.Message, False)
        End Try
    End Sub

    Private Async Sub combodesc_TextChanged(sender As Object, e As EventArgs)
        ' If we're in list-only mode, skip the old search behavior entirely.
        If combodesc.DropDownStyle = ComboBoxStyle.DropDownList Then Exit Sub

        If _suppressSuggest Then Exit Sub
        If Not combodesc.Focused Then Exit Sub

        _selectedItem = Nothing
        UpdateLblWac(clearOnly:=True)

        Dim term = combodesc.Text.Trim()
        If term = "" Then
            combodesc.DroppedDown = False
            Exit Sub
        End If

        Try
            Dim items = Await SearchInventoryAsync(term)
            Dim keep = combodesc.Text
            Dim sel = combodesc.SelectionStart

            combodesc.BeginUpdate()
            Try
                combodesc.Items.Clear()
                For Each it In items
                    combodesc.Items.Add(it)
                Next
            Finally
                combodesc.EndUpdate()
            End Try

            _suppressSuggest = True
            combodesc.Text = keep
            combodesc.SelectionStart = Math.Min(sel, combodesc.Text.Length)
            combodesc.SelectionLength = 0
            _suppressSuggest = False

            combodesc.DroppedDown = (combodesc.Items.Count > 0 AndAlso combodesc.Focused)
            Cursor.Current = Cursors.IBeam
        Catch ex As Exception
            SetStatus("Suggest error: " & ex.Message, False)
        End Try
    End Sub

    Private Sub combodesc_SelectionChangeCommitted(sender As Object, e As EventArgs)
        Dim it = TryCast(combodesc.SelectedItem, InvItem)
        If it IsNot Nothing Then
            _suppressSuggest = True
            ' In DropDownList, Text is set by selection automatically; keep your original behavior.
            combodesc.Text = it.Descriptions
            combodesc.SelectionStart = combodesc.Text.Length
            combodesc.SelectionLength = 0
            combodesc.DroppedDown = False
            _suppressSuggest = False
            _selectedItem = it
            SetStatus("Selected: " & it.Descriptions, True)
            UpdateHeaderPanelLayout() : _hdrPanel.Invalidate()
            UpdateLblWac(clearOnly:=True)
        End If
    End Sub

    Private Async Function SearchInventoryAsync(term As String) As Threading.Tasks.Task(Of List(Of InvItem))
        Dim result As New List(Of InvItem)
        Using conn As New SqlConnection(frmmain.txtdb.Text)
            Await conn.OpenAsync()
            Dim sql = "SELECT TOP 50 ID, DESCRIPTIONS FROM TBL_INVENTORY WITH (NOLOCK) " &
                      "WHERE DESCRIPTIONS LIKE @like ORDER BY DESCRIPTIONS"
            Using cmd As New SqlCommand(sql, conn)
                cmd.Parameters.Add("@like", SqlDbType.NVarChar, 3500).Value = "%" & term & "%"
                Using rd = Await cmd.ExecuteReaderAsync()
                    While Await rd.ReadAsync()
                        result.Add(New InvItem With {.P_ID = rd.GetInt32(0), .Descriptions = rd.GetString(1)})
                    End While
                End Using
            End Using
        End Using
        Return result
    End Function

    '======================== View (create rows then load) ========================
    Private Async Sub cmdview_Click(sender As Object, e As EventArgs) Handles cmdview.Click
        Try
            If _selectedItem Is Nothing Then
                SetStatus("Please select an item (DESCRIPTIONS).", False)
                MessageBox.Show("Please select an item from TBL_INVENTORY.DESCRIPTIONS.", "WAC", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If
            If comboyear.SelectedItem Is Nothing Then
                SetStatus("Please select a year.", False)
                MessageBox.Show("Please select a year.", "WAC", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If

            Dim yr As Integer = Integer.Parse(comboyear.SelectedItem.ToString())
            SetStatus($"Checking rows for {yr}…")
            Dim added = Await EnsureWholeYearRowsAsync(_selectedItem.P_ID, _selectedItem.Descriptions, yr)

            SetStatus(If(added > 0, $"Creating rows… inserted {added}.", "Rows already exist. Loading…"))
            Await LoadYearAsync(_selectedItem.P_ID, yr)
            SetStatus($"Loaded {DataGridView1.Rows.Count} rows for {yr}.", True)

        Catch ex As Exception
            SetStatus("View error: " & ex.Message, False)
            MessageBox.Show("View error: " & ex.Message, "WAC", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Async Function EnsureWholeYearRowsAsync(invId As Integer, descp As String, year As Integer) As Threading.Tasks.Task(Of Integer)
        Dim startDate As New DateTime(year, 1, 1)
        Dim endDate As New DateTime(year, 12, 31)
        Dim pidText As String = invId.ToString()

        Using conn As New SqlConnection(frmmain.txtdb.Text)
            Await conn.OpenAsync()

            Dim existing As New HashSet(Of Date)()
            Using sel As New SqlCommand("SELECT WAC_DATE FROM TBL_WAC WHERE P_ID=@pid AND WAC_DATE BETWEEN @d0 AND @d1", conn)
                sel.Parameters.Add("@pid", SqlDbType.NChar, 10).Value = pidText
                sel.Parameters.AddWithValue("@d0", startDate)
                sel.Parameters.AddWithValue("@d1", endDate)
                Using rd = Await sel.ExecuteReaderAsync()
                    While Await rd.ReadAsync()
                        existing.Add(rd.GetDateTime(0).Date)
                    End While
                End Using
            End Using

            Dim inserted As Integer = 0
            Dim insSql As String =
                "INSERT INTO TBL_WAC (" &
                " P_ID, DESCRIPTIONS, WAC_DATE," &
                " RECEIPT_QTY, RECEIPT_TOTAL_COST," &
                " ISSUANCE_QTY, ISSUANCE_UNIT_COST, ISSUANCE_TOTAL_COST," &
                " WAC_QTY, WAC_UNIT_COST, WAC_TOTAL_COST," &
                " BAL_QTY, BAL_UNIT_COST, BAL_TOTAL_COST) " &
                "SELECT @pid, @desc, @dt," &
                " 0.00, 0.00, 0.00, 0.00, 0.00," &
                " 0.00, 0.00, 0.00," &
                " 0.00, 0.00, 0.00 " &
                "WHERE NOT EXISTS (SELECT 1 FROM TBL_WAC WHERE P_ID=@pid AND WAC_DATE=@dt);"

            Using tx = conn.BeginTransaction()
                Try
                    Using ins As New SqlCommand(insSql, conn, tx)
                        ins.Parameters.Add("@pid", SqlDbType.NChar, 10).Value = pidText
                        ins.Parameters.Add("@desc", SqlDbType.NVarChar, 3500).Value = descp
                        ins.Parameters.Add("@dt", SqlDbType.Date)
                        Dim d As Date = startDate
                        While d <= endDate
                            If Not existing.Contains(d) Then
                                ins.Parameters("@dt").Value = d
                                Await ins.ExecuteNonQueryAsync()
                                inserted += 1
                                If inserted Mod 30 = 0 Then SetStatus($"Creating rows… inserted {inserted}.")
                            End If
                            d = d.AddDays(1)
                        End While
                    End Using
                    tx.Commit()
                Catch
                    tx.Rollback()
                    Throw
                End Try
            End Using
            Return inserted
        End Using
    End Function

    Private Async Function LoadYearAsync(invId As Integer, year As Integer) As Threading.Tasks.Task
        _loading = True
        Try
            Dim pidText As String = invId.ToString()
            Using conn As New SqlConnection(frmmain.txtdb.Text)
                Await conn.OpenAsync()
                Dim sql As String =
                    "SELECT ID, P_ID, DESCRIPTIONS, WAC_DATE," &
                    " RECEIPT_QTY, RECEIPT_TOTAL_COST," &
                    " ISSUANCE_QTY, ISSUANCE_UNIT_COST, ISSUANCE_TOTAL_COST," &
                    " WAC_QTY, WAC_UNIT_COST, WAC_TOTAL_COST," &
                    " BAL_QTY, BAL_UNIT_COST, BAL_TOTAL_COST " &
                    "FROM TBL_WAC WITH (ROWLOCK, READCOMMITTEDLOCK) " &
                    "WHERE P_ID=@pid AND YEAR(WAC_DATE)=@yr ORDER BY WAC_DATE"
                Using da As New SqlDataAdapter(sql, conn)
                    da.SelectCommand.Parameters.Add("@pid", SqlDbType.NChar, 10).Value = pidText
                    da.SelectCommand.Parameters.AddWithValue("@yr", year)
                    _dt = New DataTable()
                    da.Fill(_dt)
                End Using
            End Using

            DataGridView1.DataSource = _dt

            _dt.DefaultView.AllowEdit = True
            For Each dc As DataColumn In _dt.Columns
                dc.ReadOnly = dc.ColumnName.Equals(COL_DATE, StringComparison.OrdinalIgnoreCase)
            Next
            DataGridView1.ReadOnly = False
            For Each c As DataGridViewColumn In DataGridView1.Columns
                c.ReadOnly = (c.Name = COL_DATE)
            Next

            UpdateHeaderPanelLayout()
            ' --- update the label with current WAC for this year ---
            Await UpdateLblWac()
        Finally
            _loading = False
        End Try
    End Function

    '======================== Grid + styles ========================
    Private Sub EnsureGridReady()
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
            .ReadOnly = False
            .EditMode = DataGridViewEditMode.EditOnEnter

            .BorderStyle = BorderStyle.FixedSingle
            .CellBorderStyle = DataGridViewCellBorderStyle.Single
            .ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single
            .GridColor = Color.FromArgb(120, 120, 120)

            .EnableHeadersVisualStyles = False
            .ColumnHeadersHeight = TOTAL_HDR_HEIGHT
            .ColumnHeadersDefaultCellStyle.Padding = New Padding(0, TOP_HDR_HEIGHT, 0, 0)
            .ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 9.0!, FontStyle.Bold)

            .DefaultCellStyle.Font = New Font("Segoe UI", 8.0!)
            .DefaultCellStyle.SelectionBackColor = Color.FromArgb(210, 222, 255)
            .DefaultCellStyle.SelectionForeColor = Color.Black

            .RowTemplate.Height = 34
            .RowTemplate.DefaultCellStyle.Padding = New Padding(0, 6, 0, 6)
        End With

        EnableDoubleBuffer(DataGridView1)

        ' hidden tech columns
        DataGridView1.Columns.Add(MkTextCol(COL_ID, "ID", COL_ID, 60, True))
        DataGridView1.Columns.Add(MkTextCol(COL_PID, "P_ID", COL_PID, 60, True))
        DataGridView1.Columns.Add(MkTextCol(COL_DESC, "DESCRIPTIONS", COL_DESC, 160, True))

        ' DATES
        Dim c_Date = MkTextCol(COL_DATE, "DATES", COL_DATE, 150, False)
        c_Date.DefaultCellStyle.Format = "MMMM, dd yyyy"
        c_Date.ReadOnly = True
        c_Date.DefaultCellStyle.BackColor = C_BEIGE
        c_Date.HeaderCell.Style.BackColor = C_BEIGE
        DataGridView1.Columns.Add(c_Date)

        ' Receipt
        DataGridView1.Columns.Add(MkNumCol(COL_R_Q, "R-Q", COL_R_Q, 80, C_PINK, DEC))
        DataGridView1.Columns.Add(MkNumCol(COL_R_TC, "R-TC", COL_R_TC, 100, C_PINK, DEC))

        ' Issuance
        DataGridView1.Columns.Add(MkNumCol(COL_I_Q, "I-Q", COL_I_Q, 80, C_GREEN, DEC))
        DataGridView1.Columns.Add(MkNumCol(COL_I_UC, "I-UC", COL_I_UC, 90, C_GREEN, DEC))
        DataGridView1.Columns.Add(MkNumCol(COL_I_TC, "I-TC", COL_I_TC, 100, C_GREEN, DEC))

        ' WAC (Q & TC blue, UC special)
        DataGridView1.Columns.Add(MkNumCol(COL_WAC_Q, "Q", COL_WAC_Q, 80, C_BLUE, DEC))
        DataGridView1.Columns.Add(MkNumCol(COL_WAC_UC, "UC", COL_WAC_UC, 90, C_WAC_UC, DEC))
        DataGridView1.Columns.Add(MkNumCol(COL_WAC_TC, "TC", COL_WAC_TC, 100, C_BLUE, DEC))

        ' Balance
        DataGridView1.Columns.Add(MkNumCol(COL_BAL_Q, "Q", COL_BAL_Q, 80, C_ORANGE, DEC))
        DataGridView1.Columns.Add(MkNumCol(COL_BAL_UC, "UC", COL_BAL_UC, 90, C_ORANGE, DEC))
        DataGridView1.Columns.Add(MkNumCol(COL_BAL_TC, "TC", COL_BAL_TC, 100, C_ORANGE, DEC))

        For Each c As DataGridViewColumn In DataGridView1.Columns
            c.ReadOnly = (c.Name = COL_DATE)
        Next

        ' hooks
        AddHandler DataGridView1.EditingControlShowing, AddressOf DataGridView1_EditingControlShowing
        AddHandler DataGridView1.CellValidating, AddressOf DataGridView1_CellValidating
        AddHandler DataGridView1.CellEndEdit, AddressOf DataGridView1_CellEndEdit
        AddHandler DataGridView1.RowValidated, AddressOf DataGridView1_RowValidated
        AddHandler DataGridView1.CurrentCellDirtyStateChanged, AddressOf DataGridView1_CurrentCellDirtyStateChanged
        AddHandler DataGridView1.CellBeginEdit, AddressOf DataGridView1_CellBeginEdit
    End Sub

    Private Sub EnableDoubleBuffer(dgv As DataGridView)
        Try
            Dim prop = GetType(DataGridView).GetProperty("DoubleBuffered", BindingFlags.Instance Or BindingFlags.NonPublic)
            prop?.SetValue(dgv, True, Nothing)
        Catch
        End Try
    End Sub

    Private Function MkTextCol(name As String, header As String, dataProp As String, width As Integer, hidden As Boolean) As DataGridViewTextBoxColumn
        Dim c As New DataGridViewTextBoxColumn With {
            .Name = name, .HeaderText = header, .DataPropertyName = dataProp, .Width = width, .Visible = Not hidden
        }
        c.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
        Return c
    End Function

    Private Function MkNumCol(name As String, header As String, dataProp As String, width As Integer, back As Color, decDigits As Integer) As DataGridViewTextBoxColumn
        Dim c = MkTextCol(name, header, dataProp, width, False)
        c.ValueType = GetType(Decimal)
        c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        c.DefaultCellStyle.Format = "N" & decDigits.ToString()
        c.DefaultCellStyle.BackColor = back
        c.HeaderCell.Style.BackColor = back
        Return c
    End Function

    '======================== Top banner (non-flicker overlay) ========================
    Private Sub InitHeaderPanel()
        If _hdrPanel Is Nothing Then
            _hdrPanel = New DoubleBufferedPanel() With {.Height = TOP_HDR_HEIGHT, .BackColor = Color.Transparent}
            DataGridView1.Controls.Add(_hdrPanel)
            _hdrPanel.BringToFront()
            AddHandler _hdrPanel.Paint, AddressOf HeaderPanel_Paint
        End If
        UpdateHeaderPanelLayout()
        AddHandler DataGridView1.ColumnWidthChanged, Sub() UpdateHeaderPanelLayout()
        AddHandler DataGridView1.Scroll, Sub() UpdateHeaderPanelLayout()
        AddHandler DataGridView1.SizeChanged, Sub() UpdateHeaderPanelLayout()
    End Sub

    Private Sub UpdateHeaderPanelLayout()
        If _hdrPanel Is Nothing OrElse DataGridView1.Columns.Count = 0 Then Return
        Dim baseRect = DataGridView1.GetCellDisplayRectangle(DataGridView1.Columns(COL_DATE).Index, -1, True)
        _hdrPanel.Left = baseRect.Left
        _hdrPanel.Top = 0
        _hdrPanel.Width = DataGridView1.DisplayRectangle.Width
        _hdrPanel.Height = TOP_HDR_HEIGHT
        _hdrPanel.Invalidate()
    End Sub

    Private Sub HeaderPanel_Paint(sender As Object, e As PaintEventArgs)
        If DataGridView1.Columns.Count = 0 Then Return
        e.Graphics.SmoothingMode = SmoothingMode.HighQuality
        e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit

        Dim baseRect = DataGridView1.GetCellDisplayRectangle(DataGridView1.Columns(COL_DATE).Index, -1, True)
        Dim baseX As Integer = baseRect.Left

        Dim FnRect As Func(Of String, String, Rectangle) =
            Function(firstCol As String, lastCol As String) As Rectangle
                Dim r1 = DataGridView1.GetCellDisplayRectangle(DataGridView1.Columns(firstCol).Index, -1, True)
                Dim r2 = DataGridView1.GetCellDisplayRectangle(DataGridView1.Columns(lastCol).Index, -1, True)
                Dim x As Integer = r1.Left - baseX
                Dim w As Integer = r2.Right - r1.Left
                Return New Rectangle(x, 0, Math.Max(0, w), TOP_HDR_HEIGHT)
            End Function

        Using pen As New Pen(Color.FromArgb(60, 60, 60)),
              brItem As New SolidBrush(G_ITEM),
              brWac As New SolidBrush(G_WAC),
              brBal As New SolidBrush(G_BAL),
              brBeige As New SolidBrush(C_BEIGE)

            Dim fmt As New StringFormat With {.Alignment = StringAlignment.Center, .LineAlignment = StringAlignment.Center}

            Dim rDates = FnRect(COL_DATE, COL_DATE)
            e.Graphics.FillRectangle(brBeige, rDates)
            e.Graphics.DrawRectangle(pen, rDates)
            e.Graphics.DrawString("", New Font("Segoe UI", 9.0!, FontStyle.Bold), Brushes.Black, rDates, fmt)

            Dim rItem = FnRect(COL_R_Q, COL_I_TC)
            e.Graphics.FillRectangle(brItem, rItem)
            e.Graphics.DrawRectangle(pen, rItem)
            Dim itemText As String = If(_selectedItem IsNot Nothing, _selectedItem.Descriptions, combodesc.Text)
            If String.IsNullOrWhiteSpace(itemText) Then itemText = ""
            e.Graphics.DrawString(itemText, New Font("Segoe UI", 8.0!, FontStyle.Bold), Brushes.Black, rItem, fmt)

            Dim rWac = FnRect(COL_WAC_Q, COL_WAC_TC)
            e.Graphics.FillRectangle(brWac, rWac)
            e.Graphics.DrawRectangle(pen, rWac)
            e.Graphics.DrawString("WAC AFTER EVERY RECEIPT", New Font("Segoe UI", 10.5!, FontStyle.Bold), Brushes.Black, rWac, fmt)

            Dim rBal = FnRect(COL_BAL_Q, COL_BAL_TC)
            e.Graphics.FillRectangle(brBal, rBal)
            e.Graphics.DrawRectangle(pen, rBal)
            e.Graphics.DrawString("BALANCE", New Font("Segoe UI", 10.5!, FontStyle.Bold), Brushes.Black, rBal, fmt)
        End Using
    End Sub

    Private Class DoubleBufferedPanel
        Inherits Panel
        Public Sub New()
            Me.DoubleBuffered = True
            Me.ResizeRedraw = True
        End Sub
    End Class

    '======================== Numeric-only + autosave triggers ========================
    Private Sub DataGridView1_EditingControlShowing(sender As Object, e As DataGridViewEditingControlShowingEventArgs)
        Dim tb = TryCast(e.Control, TextBox)
        If tb Is Nothing Then Exit Sub
        RemoveHandler tb.KeyPress, AddressOf NumericKeyPress

        Dim col = DataGridView1.CurrentCell.OwningColumn
        If _numericCols.Contains(col.Name) OrElse _numericCols.Contains(col.DataPropertyName) Then
            AddHandler tb.KeyPress, AddressOf NumericKeyPress
        End If
    End Sub

    Private Sub NumericKeyPress(sender As Object, e As KeyPressEventArgs)
        If Char.IsControl(e.KeyChar) Then Return
        If Char.IsDigit(e.KeyChar) Then Return
        If e.KeyChar = "."c Then
            Dim tb = DirectCast(sender, TextBox)
            If tb.Text.Contains(".") AndAlso tb.SelectionLength = 0 Then e.Handled = True
            Return
        End If
        e.Handled = True
    End Sub

    Private Sub DataGridView1_CellValidating(sender As Object, e As DataGridViewCellValidatingEventArgs)
        If _loading OrElse e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Return
        Dim col = DataGridView1.Columns(e.ColumnIndex)
        If col.Name = COL_DATE Then Return
        If _numericCols.Contains(col.Name) OrElse _numericCols.Contains(col.DataPropertyName) Then
            Dim txt = If(e.FormattedValue, "").ToString().Trim()
            If txt = "" Then Return
            Dim tmp As Decimal
            If Not Decimal.TryParse(txt, tmp) Then
                DataGridView1.Rows(e.RowIndex).ErrorText = "Please enter a decimal number."
                SetStatus("Invalid input (decimal only).", False)
                e.Cancel = True
            Else
                DataGridView1.Rows(e.RowIndex).ErrorText = Nothing
            End If
        End If
    End Sub

    '======================== FAST WAC ENGINE ========================
    Private Async Sub DataGridView1_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs)
        If _loading OrElse e.RowIndex < 0 Then Return

        ' Normalize blank numeric → 0
        Dim col = DataGridView1.Columns(e.ColumnIndex)
        If _numericCols.Contains(col.Name) OrElse _numericCols.Contains(col.DataPropertyName) Then
            Dim cell = DataGridView1.Rows(e.RowIndex).Cells(e.ColumnIndex)
            If cell.Value Is Nothing OrElse IsDBNull(cell.Value) OrElse cell.Value.ToString().Trim() = "" Then
                cell.Value = 0D
            End If
        End If

        Try
            Dim propName As String = If(String.IsNullOrEmpty(col.DataPropertyName), col.Name, col.DataPropertyName)

            If _recalcTriggers.Contains(propName) Then
                SetStatus($"Recalculating from {CDate(DataGridView1.Rows(e.RowIndex).Cells(COL_DATE).Value):MMM dd} …")
                Dim changedRows = RecalcFromRowCollectChanges(e.RowIndex)
                SetStatus($"Saving {changedRows.Count} row(s)…")
                Await SaveChangedRowsAsync(changedRows)
                SetStatus($"Saved {changedRows.Count} row(s).", True)
            Else
                ' No cascade needed; just save the edited row
                SetStatus("Saving row…")
                Await SaveChangedRowsAsync(New List(Of Integer) From {e.RowIndex})
                SetStatus("Saved.", True)
            End If

            ' refresh the WAC label from grid after any save
            Await UpdateLblWac()

        Catch ex As Exception
            SetStatus("Recalc/save error: " & ex.Message, False)
        End Try
    End Sub

    Private Sub DataGridView1_CurrentCellDirtyStateChanged(sender As Object, e As EventArgs)
        If DataGridView1.IsCurrentCellDirty Then
            DataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit)
        End If
    End Sub

    Private Async Sub DataGridView1_RowValidated(sender As Object, e As DataGridViewCellEventArgs)
        If _loading OrElse _savingRecalc OrElse e.RowIndex < 0 Then Return
        Try
            Await SaveChangedRowsAsync(New List(Of Integer) From {e.RowIndex})
            Await UpdateLblWac()
        Catch
        End Try
    End Sub

    Private Sub DataGridView1_CellBeginEdit(sender As Object, e As DataGridViewCellCancelEventArgs)
        If DataGridView1.Columns(e.ColumnIndex).Name = COL_DATE Then e.Cancel = True
    End Sub

    '---- Recalc forward (only set cells when changed) ----
    Private Function RecalcFromRowCollectChanges(startIdx As Integer) As List(Of Integer)
        Dim changed As New List(Of Integer)
        If _dt Is Nothing OrElse DataGridView1.Rows.Count = 0 Then Return changed

        _loading = True
        DataGridView1.SuspendLayout()
        Try
            Dim openQ As Decimal = 0D, openTC As Decimal = 0D

            If startIdx > 0 Then
                openQ = GetCellDec(DataGridView1.Rows(startIdx - 1), COL_BAL_Q)
                openTC = GetCellDec(DataGridView1.Rows(startIdx - 1), COL_BAL_TC)
            End If

            For i As Integer = startIdx To DataGridView1.Rows.Count - 1
                Dim r = DataGridView1.Rows(i)
                If r.IsNewRow Then Continue For

                Dim rq As Decimal = GetCellDec(r, COL_R_Q)
                Dim rtc As Decimal = GetCellDec(r, COL_R_TC)

                ' WAC after receipts
                Dim wq As Decimal = RoundDec(openQ + rq)
                Dim wtc As Decimal = RoundDec(openTC + rtc)
                Dim wuc As Decimal = If(wq > 0D, RoundDec(SafeDiv(wtc, wq)), If(openQ > 0D, RoundDec(SafeDiv(openTC, openQ)), 0D))

                ' Issuance (clamp to available)
                Dim iq As Decimal = GetCellDec(r, COL_I_Q)
                If iq > wq Then iq = wq
                Dim iuc As Decimal = wuc
                Dim itc As Decimal = RoundDec(iq * iuc)

                ' Balance
                Dim bq As Decimal = RoundDec(wq - iq)
                Dim buc As Decimal = wuc
                Dim btc As Decimal = RoundDec(bq * buc)

                ' Compare & set only if changed
                Dim rowChanged As Boolean = False
                rowChanged = rowChanged Or SetIfChanged(r, COL_WAC_Q, wq)
                rowChanged = rowChanged Or SetIfChanged(r, COL_WAC_TC, wtc)
                rowChanged = rowChanged Or SetIfChanged(r, COL_WAC_UC, wuc)
                rowChanged = rowChanged Or SetIfChanged(r, COL_I_Q, iq)
                rowChanged = rowChanged Or SetIfChanged(r, COL_I_UC, iuc)
                rowChanged = rowChanged Or SetIfChanged(r, COL_I_TC, itc)
                rowChanged = rowChanged Or SetIfChanged(r, COL_BAL_Q, bq)
                rowChanged = rowChanged Or SetIfChanged(r, COL_BAL_UC, buc)
                rowChanged = rowChanged Or SetIfChanged(r, COL_BAL_TC, btc)

                If rowChanged Then changed.Add(i)

                ' Carry forward
                openQ = bq : openTC = btc
            Next

        Finally
            DataGridView1.ResumeLayout()
            _loading = False
        End Try

        Return changed
    End Function

    ' Assign only if value changed (2-decimals tolerance)
    Private Function SetIfChanged(r As DataGridViewRow, col As String, newVal As Decimal) As Boolean
        Dim oldVal As Decimal = GetCellDec(r, col)
        Dim nv As Decimal = RoundDec(newVal)
        If Math.Abs(oldVal - nv) > 0.005D Then
            r.Cells(col).Value = nv
            Return True
        End If
        Return False
    End Function

    '---- Save only changed rows (single transaction) ----
    Private Async Function SaveChangedRowsAsync(idxs As List(Of Integer)) As Threading.Tasks.Task
        If idxs Is Nothing OrElse idxs.Count = 0 Then Return
        _savingRecalc = True
        Try
            Using conn As New SqlConnection(frmmain.txtdb.Text)
                Await conn.OpenAsync()
                Using tx = conn.BeginTransaction()
                    Dim sql As String =
                        "UPDATE TBL_WAC SET " &
                        " RECEIPT_QTY=@rq, RECEIPT_TOTAL_COST=@rtc," &
                        " ISSUANCE_QTY=@iq, ISSUANCE_UNIT_COST=@iuc, ISSUANCE_TOTAL_COST=@itc," &
                        " WAC_QTY=@wq, WAC_UNIT_COST=@wuc, WAC_TOTAL_COST=@wtc," &
                        " BAL_QTY=@bq, BAL_UNIT_COST=@buc, BAL_TOTAL_COST=@btc " &
                        "WHERE ID=@id"
                    Using cmd As New SqlCommand(sql, conn, tx)
                        ' decimal parameters with precision/scale
                        AddDecParam(cmd, "@rq") : AddDecParam(cmd, "@rtc")
                        AddDecParam(cmd, "@iq") : AddDecParam(cmd, "@iuc") : AddDecParam(cmd, "@itc")
                        AddDecParam(cmd, "@wq") : AddDecParam(cmd, "@wuc") : AddDecParam(cmd, "@wtc")
                        AddDecParam(cmd, "@bq") : AddDecParam(cmd, "@buc") : AddDecParam(cmd, "@btc")
                        cmd.Parameters.Add("@id", SqlDbType.Int)

                        For Each i In idxs
                            Dim r = DataGridView1.Rows(i)
                            If r.IsNewRow Then Continue For
                            cmd.Parameters("@rq").Value = GetCellDec(r, COL_R_Q)
                            cmd.Parameters("@rtc").Value = GetCellDec(r, COL_R_TC)
                            cmd.Parameters("@iq").Value = GetCellDec(r, COL_I_Q)
                            cmd.Parameters("@iuc").Value = GetCellDec(r, COL_I_UC)
                            cmd.Parameters("@itc").Value = GetCellDec(r, COL_I_TC)
                            cmd.Parameters("@wq").Value = GetCellDec(r, COL_WAC_Q)
                            cmd.Parameters("@wuc").Value = GetCellDec(r, COL_WAC_UC)
                            cmd.Parameters("@wtc").Value = GetCellDec(r, COL_WAC_TC)
                            cmd.Parameters("@bq").Value = GetCellDec(r, COL_BAL_Q)
                            cmd.Parameters("@buc").Value = GetCellDec(r, COL_BAL_UC)
                            cmd.Parameters("@btc").Value = GetCellDec(r, COL_BAL_TC)
                            cmd.Parameters("@id").Value = CInt(r.Cells(COL_ID).Value)
                            Await cmd.ExecuteNonQueryAsync()
                        Next
                        tx.Commit()
                    End Using
                End Using
            End Using
        Finally
            _savingRecalc = False
        End Try
    End Function

    Private Sub AddDecParam(cmd As SqlCommand, name As String)
        Dim p = cmd.Parameters.Add(name, SqlDbType.Decimal)
        p.Precision = 18
        p.Scale = 2
    End Sub

    '======================== WAC label ========================
    ' Updates lblwac with the latest non-zero WAC_UNIT_COST for the selected year.
    Private Async Function UpdateLblWac(Optional clearOnly As Boolean = False) As Threading.Tasks.Task
        If lblwac Is Nothing Then Return
        If clearOnly Then
            lblwac.Text = "WAC UC: —"
            lblwac.ForeColor = Color.DimGray
            Return
        End If

        Dim wacValue As Decimal = 0D

        ' Prefer grid (already loaded)
        If DataGridView1 IsNot Nothing AndAlso DataGridView1.Rows.Count > 0 Then
            For i As Integer = DataGridView1.Rows.Count - 1 To 0 Step -1
                Dim uc = GetCellDec(DataGridView1.Rows(i), COL_WAC_UC)
                Dim wq = GetCellDec(DataGridView1.Rows(i), COL_WAC_Q)
                Dim bq = GetCellDec(DataGridView1.Rows(i), COL_BAL_Q)
                If uc > 0D OrElse wq > 0D OrElse bq > 0D Then
                    wacValue = uc
                    Exit For
                End If
            Next
        ElseIf _selectedItem IsNot Nothing AndAlso comboyear.SelectedItem IsNot Nothing Then
            ' Fallback to DB (if grid not loaded)
            Using conn As New SqlConnection(frmmain.txtdb.Text)
                Await conn.OpenAsync()
                Dim sql = "SELECT TOP 1 WAC_UNIT_COST " &
                          "FROM TBL_WAC WHERE P_ID=@pid AND YEAR(WAC_DATE)=@yr " &
                          "AND (WAC_QTY>0 OR BAL_QTY>0 OR WAC_TOTAL_COST>0 OR BAL_TOTAL_COST>0) " &
                          "ORDER BY WAC_DATE DESC"
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.Add("@pid", SqlDbType.NChar, 10).Value = _selectedItem.P_ID.ToString()
                    cmd.Parameters.AddWithValue("@yr", Integer.Parse(comboyear.SelectedItem.ToString()))
                    Dim o = Await cmd.ExecuteScalarAsync()
                    If o IsNot Nothing AndAlso o IsNot DBNull.Value Then
                        wacValue = Convert.ToDecimal(o)
                    End If
                End Using
            End Using
        End If

        lblwac.Text = "WAC UC: " & wacValue.ToString("N2")
        lblwac.ForeColor = If(wacValue > 0D, Color.MediumSeaGreen, Color.DimGray)
    End Function

    '======================== Helpers ========================
    Private Function SafeDiv(a As Decimal, b As Decimal) As Decimal
        If b = 0D Then Return 0D
        Return a / b
    End Function

    Private Function RoundDec(v As Decimal) As Decimal
        Return Math.Round(v, DEC, MidpointRounding.AwayFromZero)
    End Function

    Private Function GetCellDec(r As DataGridViewRow, col As String) As Decimal
        Dim v = r.Cells(col).Value
        If v Is Nothing OrElse IsDBNull(v) Then Return 0D
        Dim d As Decimal
        If Decimal.TryParse(v.ToString(), d) Then Return d
        Return 0D
    End Function

End Class
