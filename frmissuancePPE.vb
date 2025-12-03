Imports System.Data.SqlClient
Imports System.Text
Imports System.Drawing.Imaging
Imports System.IO

Public Class frmissuancePPE
    ' ── Filters & paging state ─────────────────────────────────────────────────────
    Private _lastSearch As String = ""
    Private _lastType As String = "All"
    Private _lastSection As String = "All"
    Private _lastYear As String = "All"
    Private _suppressFilterEvents As Boolean = False

    Private Const PageSize As Integer = 100
    Private _pageIndex As Integer = 1
    Private _totalRows As Integer = 0
    Private _totalPages As Integer = 1

    ' ── Backing data & adapters ──────────────────────────────────────────────────
    Private _ppeTable As New DataTable()
    Private _ppeAdapter As SqlDataAdapter

    ' Lookups
    Private _employeeNames As New List(Of String)()
    Private _supplierNames As New List(Of String)()


    ' Columns that allow free typing even if they are ComboBox
    Private ReadOnly _freeTypeComboCols As HashSet(Of String) =
    New HashSet(Of String)(StringComparer.OrdinalIgnoreCase) From {
        "section",
        "type_of_ppe",
        "supplier",
        "ptr_to",
        "current_end_user"
    }

    ' --- Button layout for the equipment_picture cell ---
    Private ReadOnly _btnWidth As Integer = 76
    Private ReadOnly _btnHeight As Integer = 22
    Private ReadOnly _btnSpacing As Integer = 8  ' gap between buttons

    ' Per-column background colors (initialized in ApplyPPEColumnColors)
    Private _colPalette As Dictionary(Of String, Color) = Nothing
    ' Tracks the active ComboBox editor to wire/unwire events
    Private WithEvents _activeCombo As DataGridViewComboBoxEditingControl = Nothing

    ' ── Date picker editor for DATE ACQUIRED ────────────────────────────────────────
    Private WithEvents _dtp As DateTimePicker = Nothing
    Private ReadOnly _dateColName As String = "date_acquired"
    Private _suppressDtpValChanged As Boolean = False




    ' Return rectangles RELATIVE to the top-left of the cell (0,0)
    ' Return rectangles RELATIVE to the top-left of the cell (0,0)
    Private Function GetPicButtonRectsRelative(cellSize As Size, hasPic As Boolean) _
As (R1 As Rectangle, R2 As Rectangle?, R3 As Rectangle?)

        Dim y As Integer = (cellSize.Height - _btnHeight) \ 2

        If hasPic Then
            ' View | Replace | Remove
            Dim count As Integer = 3
            Dim totalW = (_btnWidth * count) + (_btnSpacing * (count - 1))
            Dim startX = (cellSize.Width - totalW) \ 2

            Dim r1 As New Rectangle(startX, y, _btnWidth, _btnHeight)                                    ' View
            Dim r2 As New Rectangle(startX + (_btnWidth + _btnSpacing), y, _btnWidth, _btnHeight)        ' Replace
            Dim r3 As New Rectangle(startX + 2 * (_btnWidth + _btnSpacing), y, _btnWidth, _btnHeight)    ' Remove
            Return (r1, r2, r3)
        Else
            ' Upload only
            Dim startX = (cellSize.Width - _btnWidth) \ 2
            Dim r1 As New Rectangle(startX, y, _btnWidth, _btnHeight)                                     ' Upload
            Return (r1, Nothing, Nothing)
        End If
    End Function


    ' ─────────────────────────────────────────────────────────────────────────────
    ' Form load
    ' ─────────────────────────────────────────────────────────────────────────────
    Private Sub frmissuance_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadLookups()
        ConfigurePPEGrid()
        BuildAdapter()
        LoadFilterCombos()
        ResetAndSearch()
        ApplyPPEColumnColors(DataGridView1)
    End Sub

    Private Sub LoadFilterCombos()
        _suppressFilterEvents = True

        ' SECTION (fixed list + All)
        Combooffice.Items.Clear()
        Combooffice.Items.Add("All")
        Combooffice.Items.AddRange(GetSectionList().ToArray())
        Combooffice.SelectedIndex = 0

        ' TYPE OF PPE (DB DISTINCT + fixed + All)
        Combotppe.Items.Clear()
        Combotppe.Items.Add("All")
        Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
            conn.Open()
            Using cmd As New SqlCommand("
                SELECT DISTINCT LTRIM(RTRIM(type_of_ppe)) 
                FROM TBL_PPE 
                WHERE LTRIM(RTRIM(ISNULL(type_of_ppe,''))) <> '' 
                ORDER BY 1;", conn)
                Using r = cmd.ExecuteReader()
                    While r.Read()
                        Combotppe.Items.Add(r.GetString(0))
                    End While
                End Using
            End Using
        End Using
        For Each s In GetTypeOfPPEList()
            If Not Combotppe.Items.Cast(Of Object)().Any(Function(x) String.Equals(CStr(x), s, StringComparison.OrdinalIgnoreCase)) Then
                Combotppe.Items.Add(s)
            End If
        Next
        Combotppe.SelectedIndex = 0

        ' YEAR from PAR DATE (DESC) + All
        Comboyear.Items.Clear()
        Comboyear.Items.Add("All")
        Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
            conn.Open()
            Using cmd As New SqlCommand("
                SELECT DISTINCT YEAR(par_date) AS YR 
                FROM TBL_PPE 
                WHERE par_date IS NOT NULL 
                ORDER BY YR DESC;", conn)
                Using r = cmd.ExecuteReader()
                    While r.Read()
                        Comboyear.Items.Add(r.GetInt32(0).ToString())
                    End While
                End Using
            End Using
        End Using
        Comboyear.SelectedIndex = 0

        _suppressFilterEvents = False
    End Sub

    ' Normalize a single checkbox column’s True/False values to match the DataColumn type
    Private Sub FixCheckBinding(colName As String)
        If _ppeTable Is Nothing Then Exit Sub
        If Not _ppeTable.Columns.Contains(colName) Then Exit Sub
        If Not DataGridView1.Columns.Contains(colName) Then Exit Sub

        Dim dc As DataColumn = _ppeTable.Columns(colName)
        Dim c As DataGridViewCheckBoxColumn = TryCast(DataGridView1.Columns(colName), DataGridViewCheckBoxColumn)
        If c Is Nothing Then Exit Sub

        ' Map True/False based on the DataColumn’s type
        If dc.DataType Is GetType(Boolean) Then
            c.ValueType = GetType(Boolean)
            c.TrueValue = True
            c.FalseValue = False
            c.IndeterminateValue = False
        ElseIf dc.DataType Is GetType(Byte) OrElse dc.DataType Is GetType(Int16) _
        OrElse dc.DataType Is GetType(Int32) OrElse dc.DataType Is GetType(Int64) _
        OrElse dc.DataType Is GetType(Decimal) OrElse dc.DataType Is GetType(Double) Then
            c.ValueType = GetType(Integer)
            c.TrueValue = 1
            c.FalseValue = 0
            c.IndeterminateValue = DBNull.Value
        Else
            ' Treat as string NVARCHAR(…) storing "1"/"0"
            c.ValueType = GetType(String)
            c.TrueValue = "1"
            c.FalseValue = "0"
            c.IndeterminateValue = DBNull.Value
        End If
    End Sub

    Private Sub NormalizeCheckColumns()
        FixCheckBinding("par_copy")
        FixCheckBinding("sticker_tagging")

    End Sub


    Private Sub BuildWhere(ByRef whereSql As String, ByRef parms As List(Of SqlParameter))
        Dim sb As New StringBuilder(" WHERE 1=1 ")
        Dim ps As New List(Of SqlParameter)

        If Not String.IsNullOrWhiteSpace(_lastSearch) Then
            sb.Append("
              AND (
                    par_no LIKE @q OR iar_no LIKE @q OR ris_no LIKE @q OR
                    description LIKE @q OR property_number LIKE @q OR serial_number LIKE @q OR
                    remarks LIKE @q OR supplier LIKE @q OR par_to LIKE @q OR
                    current_end_user LIKE @q OR location LIKE @q OR model_no LIKE @q OR
                    section LIKE @q OR type_of_ppe LIKE @q
                  )")
            ps.Add(New SqlParameter("@q", SqlDbType.VarChar, 200) With {.Value = "%" & _lastSearch & "%"})
        End If

        If _lastType <> "All" AndAlso _lastType <> "" Then
            sb.Append(" AND type_of_ppe = @type_of_ppe")
            ps.Add(New SqlParameter("@type_of_ppe", SqlDbType.VarChar, 200) With {.Value = _lastType})
        End If

        If _lastSection <> "All" AndAlso _lastSection <> "" Then
            sb.Append(" AND section = @section")
            ps.Add(New SqlParameter("@section", SqlDbType.VarChar, 200) With {.Value = _lastSection})
        End If

        If _lastYear <> "All" AndAlso _lastYear <> "" Then
            sb.Append(" AND par_date IS NOT NULL AND YEAR(par_date) = @yr")
            ps.Add(New SqlParameter("@yr", SqlDbType.Int) With {.Value = CInt(_lastYear)})
        End If

        whereSql = sb.ToString()
        parms = ps
    End Sub

    ' --- paging loader: uses BuildWhere(...) ---
    Private Sub LoadPage()
        Try
            Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
                conn.Open()

                Dim whereSql As String = ""
                Dim ps As List(Of SqlParameter) = Nothing
                BuildWhere(whereSql, ps)

                ' 1) COUNT
                Using cmdCount As New SqlCommand("SELECT COUNT(*) FROM TBL_PPE" & whereSql & ";", conn)
                    AddClonedParameters(cmdCount, ps)
                    _totalRows = Convert.ToInt32(cmdCount.ExecuteScalar())
                End Using

                _totalPages = Math.Max(1, CInt(Math.Ceiling(_totalRows / CDbl(PageSize))))
                If _pageIndex < 1 Then _pageIndex = 1
                If _pageIndex > _totalPages Then _pageIndex = _totalPages
                Dim offset As Integer = (_pageIndex - 1) * PageSize

                ' 2) PAGE DATA (NO blob; we return a has_picture flag)
                Dim selectSql As String =
                    "SELECT ID, par_no, par_date, iar_no, ris_no, type_of_ppe, type_of_fixed_asset, " &
                    "date_acquired, description, property_number, qty, unit, serial_number, unit_cost, " &
                    "total_cost, life_span, par_to, position, supplier, ptr_to, par_copy, sticker_tagging, " &
                    "remarks, prepared_by, section, " &
                    "CASE WHEN DATALENGTH(equipment_picture) > 0 THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS has_picture, " &
                    "location, current_end_user, model_no " &
                    "FROM TBL_PPE " & whereSql &
                    " ORDER BY par_date DESC, par_no ASC " &
                    " OFFSET @off ROWS FETCH NEXT @ps ROWS ONLY;"

                Using da As New SqlDataAdapter(selectSql, conn)
                    AddClonedParameters(da.SelectCommand, ps)
                    da.SelectCommand.Parameters.Add("@off", SqlDbType.Int).Value = offset
                    da.SelectCommand.Parameters.Add("@ps", SqlDbType.Int).Value = PageSize

                    _ppeTable.Clear()
                    da.Fill(_ppeTable)

                    CoerceCheckColumn(_ppeTable, "par_copy")
                    CoerceCheckColumn(_ppeTable, "sticker_tagging")
                End Using
            End Using

            DataGridView1.DataSource = _ppeTable
            lblpage.Text = $"{_pageIndex} / {_totalPages}   ({_totalRows:#,0} rows)"
            cmdprev.Enabled = (_pageIndex > 1)
            cmdnext.Enabled = (_pageIndex < _totalPages)
        Catch ex As Exception
            MessageBox.Show("LoadPage error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' Converts a column to clean Booleans at runtime (accepts 1/0, "true"/"false", "y"/"n", etc.)
    Private Sub CoerceCheckColumn(dt As DataTable, colName As String)
        If dt Is Nothing OrElse Not dt.Columns.Contains(colName) Then Exit Sub

        ' If it’s already Boolean, nothing to do.
        If dt.Columns(colName).DataType Is GetType(Boolean) Then Exit Sub

        For Each row As DataRow In dt.Rows
            Dim b As Boolean = False
            If Not row.IsNull(colName) Then
                Dim v = row(colName)
                If TypeOf v Is Boolean Then
                    b = CBool(v)
                ElseIf TypeOf v Is Integer Then
                    b = (CInt(v) <> 0)
                Else
                    Dim s As String = v.ToString().Trim().ToLowerInvariant()
                    b = (s = "1" OrElse s = "true" OrElse s = "y" OrElse s = "yes")
                End If
            End If
            row(colName) = b
        Next
    End Sub


    Private Sub ResetAndSearch()
        _pageIndex = 1
        ApplyFiltersAndLoad()
    End Sub

    Private Sub ApplyFiltersAndLoad()
        _lastSearch = txtsearch.Text.Trim()
        _lastType = If(Combotppe.SelectedItem Is Nothing, "All", Combotppe.SelectedItem.ToString())
        _lastSection = If(Combooffice.SelectedItem Is Nothing, "All", Combooffice.SelectedItem.ToString())
        _lastYear = If(Comboyear.SelectedItem Is Nothing, "All", Comboyear.SelectedItem.ToString())
        LoadPage()
    End Sub

    ' Search & filters
    Private Sub cmdsearch_Click(sender As Object, e As EventArgs) Handles cmdsearch.Click
        ResetAndSearch()
    End Sub
    Private Sub txtsearch_KeyDown(sender As Object, e As KeyEventArgs) Handles txtsearch.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.Handled = True : e.SuppressKeyPress = True
            ResetAndSearch()
        End If
    End Sub
    Private Sub combotppe_SelectedIndexChanged(sender As Object, e As EventArgs) Handles Combotppe.SelectedIndexChanged
        If _suppressFilterEvents Then Return
        ResetAndSearch()
    End Sub
    Private Sub combooffice_SelectedIndexChanged(sender As Object, e As EventArgs) Handles Combooffice.SelectedIndexChanged
        If _suppressFilterEvents Then Return
        ResetAndSearch()
    End Sub
    Private Sub comboyear_SelectedIndexChanged(sender As Object, e As EventArgs) Handles Comboyear.SelectedIndexChanged
        If _suppressFilterEvents Then Return
        ResetAndSearch()
    End Sub

    ' Paging
    Private Sub cmdprev_Click(sender As Object, e As EventArgs) Handles cmdprev.Click
        If _pageIndex > 1 Then
            _pageIndex -= 1
            LoadPage()
        End If
    End Sub
    Private Sub cmdnext_Click(sender As Object, e As EventArgs) Handles cmdnext.Click
        If _pageIndex < _totalPages Then
            _pageIndex += 1
            LoadPage()
        End If
    End Sub

    ' ─────────────────────────────────────────────────────────────────────────────
    ' Grid helpers
    ' ─────────────────────────────────────────────────────────────────────────────
    Private Sub AddTextCol(dgv As DataGridView, name As String, header As String, prop As String,
                           Optional width As Integer = 140,
                           Optional fmt As String = Nothing,
                           Optional align As DataGridViewContentAlignment = DataGridViewContentAlignment.MiddleLeft,
                           Optional visible As Boolean = True)
        Dim c As New DataGridViewTextBoxColumn()
        c.Name = name : c.HeaderText = header : c.DataPropertyName = prop
        c.Width = width : c.Visible = visible
        c.DefaultCellStyle.Alignment = align
        If Not String.IsNullOrEmpty(fmt) Then c.DefaultCellStyle.Format = fmt
        dgv.Columns.Add(c)
    End Sub

    Private Sub AddComboCol(dgv As DataGridView, name As String, header As String, prop As String,
                            items As IEnumerable(Of String),
                            Optional width As Integer = 160,
                            Optional allowTyping As Boolean = False)
        Dim c As New DataGridViewComboBoxColumn()
        c.Name = name : c.HeaderText = header : c.DataPropertyName = prop : c.Width = width
        c.FlatStyle = FlatStyle.Flat
        c.DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton
        c.Sorted = True
        c.Items.AddRange(items.ToArray())
        c.DisplayStyleForCurrentCellOnly = True
        c.AutoComplete = True
        dgv.Columns.Add(c)
        If allowTyping Then _freeTypeComboCols.Add(name)
    End Sub

    Private Sub AddCheckCol(dgv As DataGridView, name As String, header As String, prop As String,
                        Optional width As Integer = 120)
        Dim c As New DataGridViewCheckBoxColumn()
        c.Name = name
        c.HeaderText = header
        c.DataPropertyName = prop
        c.Width = width
        c.ThreeState = False
        dgv.Columns.Add(c)
    End Sub


    Private Sub FreezeThrough(dgv As DataGridView, colName As String)
        Dim idx As Integer = dgv.Columns(colName).Index
        For i As Integer = 0 To idx
            dgv.Columns(i).Frozen = True
        Next
    End Sub

    Private Sub ConfigurePPEGrid()
        With DataGridView1
            .DataSource = Nothing : .Rows.Clear() : .Columns.Clear()
            .AutoGenerateColumns = False
            .RowHeadersVisible = False
            .AllowUserToAddRows = False
            .AllowUserToDeleteRows = True
            .AllowUserToResizeRows = False
            .MultiSelect = True
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect

            .BorderStyle = BorderStyle.None
            .CellBorderStyle = DataGridViewCellBorderStyle.None
            .ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
            .AdvancedCellBorderStyle.Left = DataGridViewAdvancedCellBorderStyle.None
            .AdvancedCellBorderStyle.Right = DataGridViewAdvancedCellBorderStyle.None
            .AdvancedCellBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.None
            .AdvancedCellBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.None

            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None
            .ScrollBars = ScrollBars.Both
            .DefaultCellStyle.WrapMode = DataGridViewTriState.False

            Dim hdrColor As Color = Color.FromArgb(73, 94, 252)
            .EnableHeadersVisualStyles = False
            .ColumnHeadersDefaultCellStyle.BackColor = hdrColor
            .ColumnHeadersDefaultCellStyle.ForeColor = Color.White
            .ColumnHeadersDefaultCellStyle.SelectionBackColor = hdrColor
            .ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White
            .ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 8.0!, FontStyle.Bold)
            .ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            .ColumnHeadersHeight = 52
            .ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing

            .DefaultCellStyle.Font = New Font("Segoe UI", 8.0!)
            .DefaultCellStyle.ForeColor = Color.Black
            .RowsDefaultCellStyle.BackColor = Color.White
            .AlternatingRowsDefaultCellStyle.BackColor = Color.White
            .DefaultCellStyle.SelectionBackColor = Color.FromArgb(210, 222, 255)
            .DefaultCellStyle.SelectionForeColor = Color.Black
            .RowTemplate.Height = 34
            .RowTemplate.DefaultCellStyle.Padding = New Padding(0, 6, 0, 6)
        End With

        EnableDoubleBuffering(DataGridView1, True)

        ' ID (hidden)
        AddTextCol(DataGridView1, "ID", "ID", "ID", 60, visible:=False)

        ' Left/frozen block
        AddTextCol(DataGridView1, "par_no", "PAR NO.", "par_no", 100)
        AddTextCol(DataGridView1, "par_date", "PAR DATE", "par_date", 80, "MM/dd/yyyy")
        AddTextCol(DataGridView1, "iar_no", "IAR NO.", "iar_no", 80)
        AddTextCol(DataGridView1, "ris_no", "RIS NO.", "ris_no", 80)

        ' TYPE OF PPE (Combo, allow typing)
        AddComboCol(DataGridView1, "type_of_ppe", "TYPE OF PPE", "type_of_ppe",
                    GetTypeOfPPEList(), 220, allowTyping:=True)

        AddTextCol(DataGridView1, "type_of_fixed_asset", "TYPE OF FIXED ASSET", "type_of_fixed_asset", 100)
        AddTextCol(DataGridView1, "date_acquired", "DATE ACQUIRED", "date_acquired", 100, "MM/dd/yyyy")
        AddTextCol(DataGridView1, "description", "DESCRIPTION", "description", 300)

        ' Scrollable remainder
        AddTextCol(DataGridView1, "property_number", "PROPERTY NUMBER", "property_number", 160)
        AddTextCol(DataGridView1, "qty", "QTY", "qty", 80, "#,##0", DataGridViewContentAlignment.MiddleCenter)
        AddTextCol(DataGridView1, "unit", "UNIT", "unit", 100)
        AddTextCol(DataGridView1, "serial_number", "SERIAL NUMBER", "serial_number", 180)
        AddTextCol(DataGridView1, "unit_cost", "UNIT COST", "unit_cost", 130, "#,##0.00", DataGridViewContentAlignment.MiddleCenter)
        AddTextCol(DataGridView1, "total_cost", "TOTAL COST", "total_cost", 140, "#,##0.00", DataGridViewContentAlignment.MiddleCenter)
        AddTextCol(DataGridView1, "life_span", "LIFE SPAN", "life_span", 120)

        ' --- equipment_picture column: UNBOUND placeholder where we draw buttons ---
        ' ... existing columns above ...

        ' --- Picture column (UNBOUND; we draw buttons here) ---
        ' Unbound display column (only hosts our buttons)
        Dim colPicHost As New DataGridViewTextBoxColumn With {
    .Name = "equipment_picture",
    .HeaderText = "PICTURE OF THE EQUIPMENT W/ PROPERTY STICKER",
    .Width = 360,
    .ReadOnly = True,                                      ' <- important: stop edit control from appearing
    .SortMode = DataGridViewColumnSortMode.NotSortable
}

        DataGridView1.Columns.Add(colPicHost)

        ' Hidden flag bound to your SELECT CASE (has_picture)
        Dim colHasPic As New DataGridViewCheckBoxColumn With {
        .Name = "has_picture",
        .HeaderText = "has_picture",
        .DataPropertyName = "has_picture",
        .Visible = False,
        .Width = 10
}
        DataGridView1.Columns.Add(colHasPic)

        ' ... the rest of your columns continue here ...


        ' The rest
        AddTextCol(DataGridView1, "par_to", "PAR TO", "par_to", 200)
        AddTextCol(DataGridView1, "position", "POSITION", "position", 160)
        AddComboCol(DataGridView1, "supplier", "SUPPLIER", "supplier", _supplierNames, 220, allowTyping:=True)
        AddComboCol(DataGridView1, "ptr_to", "PTR TO", "ptr_to", _employeeNames, 200, allowTyping:=True)
        AddCheckCol(DataGridView1, "par_copy", "PAR COPY", "par_copy", 120)
        AddCheckCol(DataGridView1, "sticker_tagging", "STICKER TAGGING", "sticker_tagging", 150)
        AddTextCol(DataGridView1, "remarks", "REMARKS", "remarks", 220)
        AddTextCol(DataGridView1, "prepared_by", "PREPARED BY", "prepared_by", 180)
        AddComboCol(DataGridView1, "section", "SECTION", "section", GetSectionList(), 260, allowTyping:=True)
        AddTextCol(DataGridView1, "location", "LOCATION", "location", 180)
        AddComboCol(DataGridView1, "current_end_user", "CURRENT END-USER", "current_end_user", _employeeNames, 220, allowTyping:=True)
        AddTextCol(DataGridView1, "model_no", "MODEL NO.", "model_no", 160)
        'AddTextCol(DataGridView1, "date_acquired", "DATE ACQUIRED", "date_acquired", 120, "MM/dd/yyyy")

        If DataGridView1.Columns.Contains(_dateColName) Then
            DataGridView1.Columns(_dateColName).ReadOnly = True   ' block manual typing
        End If

        ' Freeze to DESCRIPTION
        FreezeThrough(DataGridView1, "description")

        ' Lock some columns (read-only look)
        Dim lockedCols As String() = {"par_no", "par_date", "description", "qty", "unit_cost", "total_cost", "property_number", "par_to", "position", "prepared_by", "Unit"}
        For Each colName In lockedCols
            If DataGridView1.Columns.Contains(colName) Then
                Dim c = DataGridView1.Columns(colName)
                c.ReadOnly = True
                c.DefaultCellStyle.BackColor = Color.FromArgb(245, 246, 252)
                c.DefaultCellStyle.ForeColor = Color.FromArgb(70, 70, 70)
            End If
        Next

        With DataGridView1
            .CellBorderStyle = DataGridViewCellBorderStyle.Single
            .GridColor = Color.FromArgb(109, 148, 165)
            .EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2
        End With

        AddHandler DataGridView1.EditingControlShowing, AddressOf DataGridView1_EditingControlShowing
        AddHandler DataGridView1.RowValidated, AddressOf DataGridView1_RowValidated
        AddHandler DataGridView1.UserDeletingRow, AddressOf DataGridView1_UserDeletingRow
        AddHandler DataGridView1.CellValueChanged, AddressOf DataGridView1_CellValueChanged
        AddHandler DataGridView1.CurrentCellDirtyStateChanged, AddressOf DataGridView1_CurrentCellDirtyStateChanged
        AddHandler DataGridView1.CellEnter, AddressOf Grid_MaybeShowDatePicker
        AddHandler DataGridView1.CellClick, AddressOf Grid_MaybeShowDatePicker
        AddHandler DataGridView1.CellLeave, AddressOf Grid_HideDatePicker
        AddHandler DataGridView1.ColumnWidthChanged, AddressOf Grid_RepositionDatePicker
        AddHandler DataGridView1.RowHeightChanged, AddressOf Grid_RepositionDatePicker
        AddHandler DataGridView1.SizeChanged, AddressOf Grid_RepositionDatePicker

        SetupDatePicker()
        NormalizeComboColumns()

    End Sub

    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) _
    Handles DataGridView1.CellContentClick

        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Exit Sub
        If Not TypeOf DataGridView1.Columns(e.ColumnIndex) Is DataGridViewCheckBoxColumn Then Exit Sub

        ' Push the toggle to the data row immediately
        DataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit)
        CommitAllEdits()

        Dim view = TryCast(DataGridView1.Rows(e.RowIndex).DataBoundItem, DataRowView)
        If view IsNot Nothing Then
            view.EndEdit()
            SaveRow(view.Row)
        End If
    End Sub


    Private Sub DataGridView1_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) _
    Handles DataGridView1.CellEndEdit

        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Return
        Dim colName = DataGridView1.Columns(e.ColumnIndex).Name
        If Not _freeTypeComboCols.Contains(colName) Then Return

        Dim cell = DataGridView1.Rows(e.RowIndex).Cells(e.ColumnIndex)
        Dim textInCell As String = Convert.ToString(cell.Value).Trim()

        ' If a ComboBox was open, its Text may be fresher than the cell.Value here
        Dim cb = TryCast(DataGridView1.EditingControl, DataGridViewComboBoxEditingControl)
        If cb IsNot Nothing Then
            Dim picked = If(TryCast(cb.SelectedItem, String), cb.Text)
            If Not String.IsNullOrWhiteSpace(picked) Then textInCell = picked.Trim()
        End If

        If textInCell = "" Then Return
        CommitComboEditAt(e.RowIndex, e.ColumnIndex, textInCell)
    End Sub





    Private Sub SetupDatePicker()
        If _dtp IsNot Nothing Then Return
        _dtp = New DateTimePicker() With {
        .Format = DateTimePickerFormat.Custom,
        .CustomFormat = "MM/dd/yyyy",
        .Visible = False,
        .Font = DataGridView1.DefaultCellStyle.Font,
        .ShowUpDown = False
    }
        ' Prevent typing into the grid cell; we only allow picking via this control.
        _dtp.MaxDate = New Date(9998, 12, 31)
        _dtp.MinDate = New Date(1900, 1, 1)
        DataGridView1.Controls.Add(_dtp)

        AddHandler _dtp.ValueChanged, AddressOf Dtp_ValueChanged
        AddHandler _dtp.CloseUp, AddressOf Dtp_CloseUp
        AddHandler _dtp.LostFocus, Sub() _dtp.Visible = False
    End Sub

    Private Sub Grid_MaybeShowDatePicker(sender As Object, e As DataGridViewCellEventArgs)
        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Return
        Dim dgv = DataGridView1
        If dgv.Columns(e.ColumnIndex).Name <> _dateColName Then
            If _dtp IsNot Nothing Then _dtp.Visible = False
            Return
        End If

        ' Position & size the picker to fill the cell
        Dim rect = dgv.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, True)
        If rect.Width <= 0 OrElse rect.Height <= 0 Then
            _dtp.Visible = False : Return
        End If

        _dtp.Bounds = New Rectangle(rect.X + 1, rect.Y + 1, Math.Max(2, rect.Width - 2), Math.Max(2, rect.Height - 2))

        ' Load current cell value (if any)
        _suppressDtpValChanged = True
        Try
            Dim v = dgv.Rows(e.RowIndex).Cells(_dateColName).Value
            If v Is Nothing OrElse v Is DBNull.Value Then
                _dtp.Value = Date.Today
            Else
                Dim dt As Date
                If Date.TryParse(CStr(v), dt) Then
                    _dtp.Value = dt
                Else
                    _dtp.Value = Date.Today
                End If
            End If
        Finally
            _suppressDtpValChanged = False
        End Try

        _dtp.Visible = True
        _dtp.BringToFront()
        _dtp.Focus()
    End Sub

    Private Sub Grid_HideDatePicker(sender As Object, e As DataGridViewCellEventArgs)
        If _dtp IsNot Nothing Then _dtp.Visible = False
    End Sub

    Private Sub Grid_RepositionDatePicker(sender As Object, e As EventArgs)
        If _dtp Is Nothing OrElse Not _dtp.Visible Then Return
        Dim cell = DataGridView1.CurrentCell
        If cell Is Nothing Then _dtp.Visible = False : Return
        If DataGridView1.Columns(cell.ColumnIndex).Name <> _dateColName Then _dtp.Visible = False : Return

        Dim rect = DataGridView1.GetCellDisplayRectangle(cell.ColumnIndex, cell.RowIndex, True)
        If rect.Width <= 0 OrElse rect.Height <= 0 Then
            _dtp.Visible = False : Return
        End If
        _dtp.Bounds = New Rectangle(rect.X + 1, rect.Y + 1, Math.Max(2, rect.Width - 2), Math.Max(2, rect.Height - 2))
    End Sub

    Private Sub Dtp_ValueChanged(sender As Object, e As EventArgs)
        If _suppressDtpValChanged Then Return
        Dim cell = DataGridView1.CurrentCell
        If cell Is Nothing Then Return
        If DataGridView1.Columns(cell.ColumnIndex).Name <> _dateColName Then Return

        Dim picked As Date = _dtp.Value.Date

        ' Write into the grid cell UI
        cell.Value = picked

        ' Force all bindings to push the value to the DataRowView
        CommitAllEdits()

        ' Save the current row immediately
        Dim view As DataRowView = TryCast(DataGridView1.Rows(cell.RowIndex).DataBoundItem, DataRowView)
        If view IsNot Nothing Then
            ' (write again at the data level to be explicit and mark Modified)
            view.Row(_dateColName) = picked
            view.EndEdit()                 ' ensure RowState = Modified
            SaveRow(view.Row)              ' your adapter-based saver
        End If
    End Sub


    Private Sub Dtp_CloseUp(sender As Object, e As EventArgs)
        ' Hide after selection
        If _dtp IsNot Nothing Then _dtp.Visible = False

        ' Extra safety: if we're still on the date cell, force a commit/save
        Dim cell = DataGridView1.CurrentCell
        If cell Is Nothing Then Return
        If DataGridView1.Columns(cell.ColumnIndex).Name <> _dateColName Then Return

        CommitAllEdits()
        Dim view As DataRowView = TryCast(DataGridView1.Rows(cell.RowIndex).DataBoundItem, DataRowView)
        If view IsNot Nothing Then
            view.EndEdit()
            SaveRow(view.Row)
        End If
    End Sub



    Private Sub DataGridView1_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) _
    Handles DataGridView1.CellFormatting

        If e.RowIndex < 0 Then Return
        If _colPalette Is Nothing Then Return

        Dim dgv = DirectCast(sender, DataGridView)
        Dim colName = dgv.Columns(e.ColumnIndex).Name

        ' Per-column background color (like your sheet)
        Dim bg As Color
        If _colPalette.TryGetValue(colName, bg) Then
            e.CellStyle.BackColor = bg
            ' keep your global selection color; don't override SelectionBackColor here
        End If
    End Sub


    Private Sub EnableDoubleBuffering(dgv As DataGridView, enable As Boolean)
        Dim pi = GetType(DataGridView).GetProperty("DoubleBuffered", Reflection.BindingFlags.Instance Or Reflection.BindingFlags.NonPublic)
        If pi IsNot Nothing Then pi.SetValue(dgv, enable, Nothing)
    End Sub

    ' ─────────────────────────────────────────────────────────────────────────────
    ' Lookups
    ' ─────────────────────────────────────────────────────────────────────────────
    Private Sub LoadLookups()
        _employeeNames.Clear()
        _supplierNames.Clear()

        LoadApproverEmployeeNames()

        Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
            conn.Open()
            Using cmd As New SqlCommand("
                SELECT DISTINCT LTRIM(RTRIM(RFQ_NCOMPANY)) AS Name
                FROM TBL_RFQSUPPLIER
                WHERE LTRIM(RTRIM(ISNULL(RFQ_NCOMPANY,'')))<>'' 
                ORDER BY Name;", conn)
                Using r = cmd.ExecuteReader()
                    While r.Read()
                        _supplierNames.Add(r.GetString(0).Trim())
                    End While
                End Using
            End Using
        End Using
    End Sub

    Private Function GetSectionList() As IEnumerable(Of String)
        Return New List(Of String) From {
            "OFFICE OF THE MINISTER",
            "OFFICE OF THE DEPUTY MINISTER",
            "OFFICE OF DIRECTOR GENERAL",
            "ADMINISTRATIVE AND FINANCE DIVISION (CAO)",
            "ACCOUNTING SECTION",
            "BUDGET SECTION",
            "PROCUREMENT MANAGEMENT SECTION",
            "CASH SECTION",
            "ARCHIVES AND RECORDS SECTION",
            "HUMAN RESOURCE MANAGEMENT SECTION",
            "SUPPLY SECTION",
            "GENERAL SERVICES SECTION",
            "LEGAL AND LEGISLATIVE LIAISON SECTION",
            "PLANNING SECTION",
            "INFORMATION AND COMMUNICATION SECTION",
            "INTERNAL AUDIT SECTION",
            "BANGSAMORO PEACE RECONCILIATION AND UNIFICATION SERVICES",
            "PEACE EDUCATION DIVISION",
            "ALTERNATIVE DISPUTE RESOLUTION DIVISION (CAO-V)",
            "COMMUNITY AFFAIRS SECTION (MAG)",
            "COMMUNITY AFFAIRS SECTION (LDS)",
            "COMMUNITY AFFAIRS SECTION (SGA)",
            "COMMUNITY AFFAIRS SECTION (BAS)",
            "COMMUNITY AFFAIRS SECTION (SUL)",
            "COMMUNITY AFFAIRS SECTION (TAW)",
            "HOME AFFAIRS SERVICES",
            "LAW ENFORCEMENT COORDINATION DIVISION",
            "CIVILIAN INTELLIGENCE AND INVESTIGATION DIVISION"
        }
    End Function

    Private Function GetTypeOfPPEList() As IEnumerable(Of String)
        Return New List(Of String) From {
            " ",
            "MOTOR VEHICLE",
            "INFORMATION AND COMMUNICATIONS TECHNOLOGY EQUIPMENT",
            "OFFICE EQUIPMENT",
            "MACHINERY",
            "WATERCRAFT",
            "OTHER TRANSPORTATION EQUIPMENT",
            "FURNITURE & FIXTURES",
            "LEASED ASSETS IMPROVEMENT-BUILDING",
            "CONSTRUCTION IN PROGRESS-INFRASTRUCTURE"
        }
    End Function

    ' Allow typing into certain combo cells
    Private Sub DataGridView1_EditingControlShowing(sender As Object, e As DataGridViewEditingControlShowingEventArgs)
        ' Unhook old
        If _activeCombo IsNot Nothing Then
            RemoveHandler _activeCombo.Validated, AddressOf Combo_Validated
            RemoveHandler _activeCombo.KeyDown, AddressOf Combo_KeyDown
            RemoveHandler _activeCombo.SelectionChangeCommitted, AddressOf Combo_SelectionChangeCommitted
        End If

        _activeCombo = TryCast(e.Control, DataGridViewComboBoxEditingControl)

        If _activeCombo Is Nothing Then Return
        If DataGridView1.CurrentCell Is Nothing Then Return

        Dim colName = DataGridView1.Columns(DataGridView1.CurrentCell.ColumnIndex).Name
        Dim freeType As Boolean = _freeTypeComboCols.Contains(colName)
        _activeCombo.DropDownStyle = If(freeType, ComboBoxStyle.DropDown, ComboBoxStyle.DropDownList)

        AddHandler _activeCombo.Validated, AddressOf Combo_Validated
        AddHandler _activeCombo.KeyDown, AddressOf Combo_KeyDown
        AddHandler _activeCombo.SelectionChangeCommitted, AddressOf Combo_SelectionChangeCommitted
    End Sub


    ' Fires immediately when a user picks an item from a dropdown
    Private Sub Combo_SelectionChangeCommitted(sender As Object, e As EventArgs)
        If DataGridView1.CurrentCell Is Nothing Then Exit Sub

        Dim r As Integer = DataGridView1.CurrentCell.RowIndex
        Dim c As Integer = DataGridView1.CurrentCell.ColumnIndex
        Dim colName As String = DataGridView1.Columns(c).Name

        ' We only manage the columns that we allow free typing / combos we care about
        If Not _freeTypeComboCols.Contains(colName) Then Exit Sub

        Dim cb = TryCast(sender, DataGridViewComboBoxEditingControl)
        Dim chosen As String = ""
        If cb IsNot Nothing Then
            ' Prefer SelectedItem when available, fallback to Text
            chosen = If(TryCast(cb.SelectedItem, String), cb.Text)
        End If
        chosen = If(chosen, "").Trim()
        If chosen = "" Then Exit Sub

        CommitComboEditAt(r, c, chosen)
    End Sub





    ' ─────────────────────────────────────────────────────────────────────────────
    ' DataAdapter (no equipment_picture in adapter SQL)
    ' ─────────────────────────────────────────────────────────────────────────────
    Private Sub BuildAdapter()
        Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
            conn.Open()

            Dim selectSql As String =
                "SELECT ID, par_no, par_date, iar_no, ris_no, type_of_ppe, type_of_fixed_asset, " &
                "date_acquired, description, property_number, qty, unit, serial_number, unit_cost, " &
                "total_cost, life_span, par_to, position, supplier, ptr_to, par_copy, sticker_tagging, " &
                "remarks, prepared_by, section, location, current_end_user, model_no " &
                "FROM TBL_PPE WITH (UPDLOCK, ROWLOCK);"

            _ppeAdapter = New SqlDataAdapter(selectSql, conn)

            Dim insertSql As String =
                "INSERT INTO TBL_PPE " &
                "(par_no, par_date, iar_no, ris_no, type_of_ppe, type_of_fixed_asset, date_acquired, description, " &
                " property_number, qty, unit, serial_number, unit_cost, total_cost, life_span, par_to, position, supplier, ptr_to, " &
                " par_copy, sticker_tagging, remarks, prepared_by, section, location, current_end_user, model_no) " &
                "VALUES (@par_no, @par_date, @iar_no, @ris_no, @type_of_ppe, @type_of_fixed_asset, @date_acquired, @description, " &
                " @property_number, @qty, @unit, @serial_number, @unit_cost, @total_cost, @life_span, @par_to, @position, @supplier, @ptr_to, " &
                " @par_copy, @sticker_tagging, @remarks, @prepared_by, @section, @location, @current_end_user, @model_no); " &
                "SET @ID = SCOPE_IDENTITY();"

            Dim insertCmd As New SqlCommand(insertSql, conn)
            AddAllParams(insertCmd)
            Dim idParamInsert = insertCmd.Parameters.Add("@ID", SqlDbType.Int)
            idParamInsert.Direction = ParameterDirection.Output
            _ppeAdapter.InsertCommand = insertCmd
            AddHandler _ppeAdapter.RowUpdating, AddressOf PpeAdapter_RowUpdating

            Dim updateSql As String =
                "UPDATE TBL_PPE SET " &
                "par_no=@par_no, par_date=@par_date, iar_no=@iar_no, ris_no=@ris_no, type_of_ppe=@type_of_ppe, type_of_fixed_asset=@type_of_fixed_asset, " &
                "date_acquired=@date_acquired, description=@description, property_number=@property_number, qty=@qty, unit=@unit, serial_number=@serial_number, " &
                "unit_cost=@unit_cost, total_cost=@total_cost, life_span=@life_span, par_to=@par_to, position=@position, supplier=@supplier, ptr_to=@ptr_to, " &
                "par_copy=@par_copy, sticker_tagging=@sticker_tagging, remarks=@remarks, prepared_by=@prepared_by, section=@section, " &
                "location=@location, current_end_user=@current_end_user, model_no=@model_no " &
                "WHERE ID=@ID;"

            Dim updateCmd As New SqlCommand(updateSql, conn)
            AddAllParams(updateCmd)
            updateCmd.Parameters.Add("@ID", SqlDbType.Int, 4, "ID")
            _ppeAdapter.UpdateCommand = updateCmd
            AddHandler _ppeAdapter.RowUpdating, AddressOf PpeAdapter_RowUpdating

            Dim deleteCmd As New SqlCommand("DELETE FROM TBL_PPE WHERE ID=@ID;", conn)
            deleteCmd.Parameters.Add("@ID", SqlDbType.Int, 4, "ID")
            _ppeAdapter.DeleteCommand = deleteCmd
            AddHandler _ppeAdapter.RowUpdating, AddressOf PpeAdapter_RowUpdating
        End Using
    End Sub

    Private Sub AddAllParams(cmd As SqlCommand)
        cmd.Parameters.Add("@par_no", SqlDbType.VarChar, 100, "par_no")
        cmd.Parameters.Add("@par_date", SqlDbType.Date, 0, "par_date")
        cmd.Parameters.Add("@iar_no", SqlDbType.VarChar, 100, "iar_no")
        cmd.Parameters.Add("@ris_no", SqlDbType.VarChar, 100, "ris_no")
        cmd.Parameters.Add("@type_of_ppe", SqlDbType.VarChar, 200, "type_of_ppe")
        cmd.Parameters.Add("@type_of_fixed_asset", SqlDbType.VarChar, 200, "type_of_fixed_asset")
        cmd.Parameters.Add("@date_acquired", SqlDbType.Date, 0, "date_acquired")
        cmd.Parameters.Add("@description", SqlDbType.VarChar, 1000, "description")
        cmd.Parameters.Add("@property_number", SqlDbType.VarChar, 120, "property_number")
        cmd.Parameters.Add("@qty", SqlDbType.Int, 0, "qty")
        cmd.Parameters.Add("@unit", SqlDbType.VarChar, 50, "unit")
        cmd.Parameters.Add("@serial_number", SqlDbType.VarChar, 200, "serial_number")
        cmd.Parameters.Add("@unit_cost", SqlDbType.Decimal, 0, "unit_cost").Precision = 18 : cmd.Parameters("@unit_cost").Scale = 2
        cmd.Parameters.Add("@total_cost", SqlDbType.Decimal, 0, "total_cost").Precision = 18 : cmd.Parameters("@total_cost").Scale = 2
        cmd.Parameters.Add("@life_span", SqlDbType.VarChar, 100, "life_span")
        cmd.Parameters.Add("@par_to", SqlDbType.VarChar, 150, "par_to")
        cmd.Parameters.Add("@position", SqlDbType.VarChar, 150, "position")
        cmd.Parameters.Add("@supplier", SqlDbType.VarChar, 200, "supplier")
        cmd.Parameters.Add("@ptr_to", SqlDbType.VarChar, 150, "ptr_to")
        cmd.Parameters.Add("@par_copy", SqlDbType.Bit, 0, "par_copy")
        cmd.Parameters.Add("@sticker_tagging", SqlDbType.Bit, 0, "sticker_tagging")
        cmd.Parameters.Add("@remarks", SqlDbType.VarChar, 500, "remarks")
        cmd.Parameters.Add("@prepared_by", SqlDbType.VarChar, 150, "prepared_by")
        cmd.Parameters.Add("@section", SqlDbType.VarChar, 200, "section")
        cmd.Parameters.Add("@location", SqlDbType.VarChar, 200, "location")
        cmd.Parameters.Add("@current_end_user", SqlDbType.VarChar, 200, "current_end_user")
        cmd.Parameters.Add("@model_no", SqlDbType.VarChar, 200, "model_no")
    End Sub

    ' ─────────────────────────────────────────────────────────────────────────────
    ' Save-as-you-go logic
    ' ─────────────────────────────────────────────────────────────────────────────
    Private Sub DataGridView1_CurrentCellDirtyStateChanged(sender As Object, e As EventArgs)
        If DataGridView1.IsCurrentCellDirty Then
            DataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit)
            CommitAllEdits()   ' NEW: make sure binding sees the change
        End If
    End Sub


    Private Sub DataGridView1_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs)
        If e.RowIndex < 0 Then Return
        Dim col = DataGridView1.Columns(e.ColumnIndex).Name
        If col = "qty" OrElse col = "unit_cost" Then
            Dim r = DataGridView1.Rows(e.RowIndex)
            Dim qty As Integer = 0
            Dim unitCost As Decimal = 0D
            Integer.TryParse(CStr(r.Cells("qty").Value), qty)
            Decimal.TryParse(CStr(r.Cells("unit_cost").Value), unitCost)
            r.Cells("total_cost").Value = qty * unitCost
        End If
    End Sub

    Private Sub DataGridView1_RowValidated(sender As Object, e As DataGridViewCellEventArgs)

        CommitAllEdits()
        If DataGridView1.DataSource Is Nothing Then Return
        Dim view As DataRowView = TryCast(DataGridView1.Rows(e.RowIndex).DataBoundItem, DataRowView)
        If view Is Nothing Then Return
        Dim row As DataRow = view.Row
        If row.RowState = DataRowState.Added OrElse row.RowState = DataRowState.Modified Then
            SaveRow(row)
        End If
    End Sub

    Private Sub DataGridView1_UserDeletingRow(sender As Object, e As DataGridViewRowCancelEventArgs)
        Dim view As DataRowView = TryCast(e.Row.DataBoundItem, DataRowView)
        If view Is Nothing Then Return
        Dim row As DataRow = view.Row
        row.Delete()
        SaveChanges()
    End Sub

    Private Sub SaveRow(row As DataRow)
        Try
            Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
                conn.Open()
                _ppeAdapter.InsertCommand.Connection = conn
                _ppeAdapter.UpdateCommand.Connection = conn
                _ppeAdapter.DeleteCommand.Connection = conn

                Dim tmp As DataTable = row.Table.Clone()
                tmp.ImportRow(row)
                _ppeAdapter.Update(tmp)

                If row.RowState = DataRowState.Added Then
                    row("ID") = tmp.Rows(0)("ID")
                End If

                row.AcceptChanges()
            End Using
        Catch ex As Exception
            MessageBox.Show("Save error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub SaveChanges()
        Try
            Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
                conn.Open()
                _ppeAdapter.InsertCommand.Connection = conn
                _ppeAdapter.UpdateCommand.Connection = conn
                _ppeAdapter.DeleteCommand.Connection = conn
                _ppeAdapter.Update(_ppeTable)
                _ppeTable.AcceptChanges()
            End Using
        Catch ex As Exception
            MessageBox.Show("Save error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' Handle missing ComboBox values gracefully
    Private Sub DataGridView1_DataError(sender As Object, e As DataGridViewDataErrorEventArgs) _
        Handles DataGridView1.DataError
        e.ThrowException = False
        Dim dgv = DirectCast(sender, DataGridView)
        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Exit Sub
        Dim col = TryCast(dgv.Columns(e.ColumnIndex), DataGridViewComboBoxColumn)
        If col Is Nothing Then Exit Sub
        Dim val = dgv.Rows(e.RowIndex).Cells(e.ColumnIndex).Value
        If val Is Nothing OrElse val Is DBNull.Value Then Exit Sub
        Dim s = val.ToString().Trim()
        If s = "" Then Exit Sub
        If Not col.Items.Cast(Of Object)().Any(Function(x) String.Equals(x.ToString(), s, StringComparison.OrdinalIgnoreCase)) Then
            col.Items.Add(s)
        End If
    End Sub

    Private Sub DataGridView1_DataBindingComplete(sender As Object, e As DataGridViewBindingCompleteEventArgs) _
        Handles DataGridView1.DataBindingComplete

        EnsureColumnHasAllValues("par_to")
        EnsureColumnHasAllValues("supplier")
        EnsureColumnHasAllValues("section")
        EnsureColumnHasAllValues("type_of_ppe")
        EnsureColumnHasAllValues("ptr_to")             ' NEW
        EnsureColumnHasAllValues("current_end_user")   ' NEW
    End Sub


    Private Sub EnsureColumnHasAllValues(colName As String)
        If Not DataGridView1.Columns.Contains(colName) Then Exit Sub
        Dim col = TryCast(DataGridView1.Columns(colName), DataGridViewComboBoxColumn)
        If col Is Nothing Then Exit Sub

        Dim setItems As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
        For Each it In col.Items : setItems.Add(it.ToString()) : Next

        For Each row As DataGridViewRow In DataGridView1.Rows
            If row.IsNewRow Then Continue For
            Dim v = row.Cells(colName).Value
            If v Is Nothing OrElse v Is DBNull.Value Then Continue For
            Dim s = v.ToString().Trim()
            If s <> "" AndAlso Not setItems.Contains(s) Then
                col.Items.Add(s) : setItems.Add(s)
            End If
        Next
    End Sub

    Private Function ToBool(v As Object) As Boolean
        If v Is Nothing OrElse v Is DBNull.Value Then Return False
        If TypeOf v Is Boolean Then Return CBool(v)
        If TypeOf v Is Integer Then Return (CInt(v) <> 0)
        Dim s As String = v.ToString().Trim().ToLowerInvariant()
        If s = "" Then Return False
        If s = "1" OrElse s = "y" OrElse s = "yes" OrElse s = "true" Then Return True
        If s = "0" OrElse s = "n" OrElse s = "no" OrElse s = "false" Then Return False
        Return True
    End Function

    Private Sub PpeAdapter_RowUpdating(sender As Object, e As SqlRowUpdatingEventArgs)
        Dim pc As Boolean = ToBool(e.Row("par_copy"))
        Dim st As Boolean = ToBool(e.Row("sticker_tagging"))

        If e.Command.Parameters.Contains("@par_copy") Then
            ' If target column is BIT this will be fine; if NVARCHAR SQL Server will cast 1/0 → '1'/'0'
            e.Command.Parameters("@par_copy").Value = If(pc, 1, 0)
        End If
        If e.Command.Parameters.Contains("@sticker_tagging") Then
            e.Command.Parameters("@sticker_tagging").Value = If(st, 1, 0)
        End If
    End Sub


    Private Sub OptimizeGridPerformance()
        Dim dgv = DataGridView1
        dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None
        dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None
        For Each col As DataGridViewColumn In dgv.Columns
            col.SortMode = DataGridViewColumnSortMode.NotSortable
            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            Dim ccb = TryCast(col, DataGridViewComboBoxColumn)
            If ccb IsNot Nothing Then
                ccb.DisplayStyleForCurrentCellOnly = True
                ccb.DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton
                ccb.FlatStyle = FlatStyle.Flat
            End If
            Dim cxb = TryCast(col, DataGridViewCheckBoxColumn)
            If cxb IsNot Nothing Then cxb.FlatStyle = FlatStyle.Flat
        Next
        dgv.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing
        dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        dgv.DefaultCellStyle.WrapMode = DataGridViewTriState.False
        dgv.RowsDefaultCellStyle.WrapMode = DataGridViewTriState.False
        dgv.AlternatingRowsDefaultCellStyle.WrapMode = DataGridViewTriState.False
        dgv.CellBorderStyle = DataGridViewCellBorderStyle.Single
    End Sub

    Private Sub AddClonedParameters(cmd As SqlCommand, src As IEnumerable(Of SqlParameter))
        For Each p In src
            Dim q As New SqlParameter()
            q.ParameterName = p.ParameterName
            q.SqlDbType = p.SqlDbType
            q.Size = p.Size
            q.Precision = p.Precision
            q.Scale = p.Scale
            q.Direction = ParameterDirection.Input
            q.Value = If(p.Value, DBNull.Value)
            cmd.Parameters.Add(q)
        Next
    End Sub

    Private Sub UpdateComboColumnItems(colName As String, items As IEnumerable(Of String))
        Dim col = TryCast(DataGridView1.Columns(colName), DataGridViewComboBoxColumn)
        If col Is Nothing Then Exit Sub
        Dim list = items.Where(Function(s) Not String.IsNullOrWhiteSpace(s)).
                        Select(Function(s) s.Trim()).
                        Distinct(StringComparer.OrdinalIgnoreCase).
                        OrderBy(Function(s) s, StringComparer.OrdinalIgnoreCase).ToList()
        col.Items.Clear()
        col.Items.AddRange(list.ToArray())
        For Each r As DataRow In _ppeTable.Rows
            If r.RowState = DataRowState.Deleted Then Continue For
            Dim v As String = If(TryCast(r(colName), String), "").Trim()
            If v = "" Then Continue For
            If Not col.Items.Cast(Of Object)().Any(Function(it) String.Equals(CStr(it), v, StringComparison.OrdinalIgnoreCase)) Then
                col.Items.Add(v)
            End If
        Next
    End Sub

    Private Sub cmdrefresh_Click(sender As Object, e As EventArgs) Handles cmdrefresh.Click
        Try
            If DataGridView1.IsCurrentCellDirty Then DataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit)
            DataGridView1.EndEdit()
            If _ppeTable IsNot Nothing AndAlso _ppeTable.GetChanges() IsNot Nothing Then SaveChanges()
            LoadLookups()
            UpdateComboColumnItems("par_to", _employeeNames)
            UpdateComboColumnItems("supplier", _supplierNames)
            UpdateComboColumnItems("ptr_to", _employeeNames)             ' NEW
            UpdateComboColumnItems("current_end_user", _employeeNames)
            LoadPage()
            DataGridView1.AllowUserToAddRows = False
        Catch ex As Exception
            MessageBox.Show("Refresh error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub Cmdnew_Click(sender As Object, e As EventArgs) Handles cmdnew.Click
        Try
            frmissuancePPEFile.Dispose()
            frmissuancePPEFile.cmdadd.Enabled = False
            frmissuancePPEFile.cmddelete.Enabled = False
            frmissuancePPEFile.cmdprint.Enabled = False
            frmissuancePPEFile.ShowDialog()
        Catch
        End Try
    End Sub

    Private Function CurrentParNoFromGrid() As String
        Dim g = Me.DataGridView1
        If g Is Nothing Then Return Nothing
        Dim r = If(g.CurrentRow, If(g.SelectedRows.Cast(Of DataGridViewRow)().FirstOrDefault(), Nothing))
        If r Is Nothing Then Return Nothing
        Dim colName = If(g.Columns.Contains("par_no"), "par_no", If(g.Columns.Contains("PAR_NO"), "PAR_NO", Nothing))
        If colName Is Nothing Then Return Nothing
        Dim v = r.Cells(colName).Value
        Return If(v Is Nothing, Nothing, v.ToString().Trim())
    End Function

    Private Async Sub cmdmanage_Click(sender As Object, e As EventArgs) Handles cmdmanage.Click
        Dim parNo As String = CurrentParNoFromGrid()
        If String.IsNullOrWhiteSpace(parNo) Then
            MessageBox.Show("Please select a PAR to manage.", "No PAR selected", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If
        Using f As New frmissuancePPEFile()
            f.StartPosition = FormStartPosition.CenterParent
            Await f.InitializeForManage(parNo)
            f.ShowDialog(Me)
        End Using
    End Sub

    ' ─────────────────────────────────────────────────────────────────────────────
    ' Column color palette (applied to column DefaultCellStyle)
    ' ─────────────────────────────────────────────────────────────────────────────
    Private Sub ApplyPPEColumnColors(g As DataGridView)
        ' Map: column name -> background color (from your sheet)
        _colPalette = New Dictionary(Of String, Color)(StringComparer.OrdinalIgnoreCase) From {
        {"par_no", Color.FromArgb(201, 230, 232)},
        {"par_date", Color.FromArgb(218, 205, 222)},
        {"iar_no", Color.FromArgb(218, 205, 222)},
        {"ris_no", Color.FromArgb(218, 205, 222)},
        {"type_of_ppe", Color.FromArgb(210, 203, 222)},
        {"type_of_fixed_asset", Color.FromArgb(210, 203, 222)},
        {"date_acquired", Color.FromArgb(244, 217, 217)},
        {"description", Color.FromArgb(246, 225, 215)},
        {"property_number", Color.FromArgb(250, 234, 208)},
        {"qty", Color.FromArgb(211, 235, 239)},
        {"unit", Color.FromArgb(214, 238, 243)},
        {"serial_number", Color.FromArgb(244, 219, 223)},
        {"unit_cost", Color.FromArgb(251, 234, 210)},
        {"total_cost", Color.FromArgb(253, 227, 196)},
        {"life_span", Color.FromArgb(216, 232, 243)},
        {"par_to", Color.FromArgb(239, 208, 217)},
        {"position", Color.FromArgb(201, 185, 200)},
        {"supplier", Color.FromArgb(212, 233, 238)},
        {"ptr_to", Color.FromArgb(200, 190, 208)},
        {"par_copy", Color.FromArgb(251, 224, 232)},
        {"sticker_tagging", Color.FromArgb(251, 224, 232)},
        {"remarks", Color.FromArgb(215, 204, 219)},
        {"prepared_by", Color.FromArgb(224, 224, 232)},
        {"section", Color.FromArgb(220, 238, 241)},
        {"equipment_picture", Color.FromArgb(214, 203, 221)},
        {"location", Color.FromArgb(229, 229, 235)},
        {"current_end_user", Color.FromArgb(229, 229, 235)},
        {"model_no", Color.FromArgb(209, 196, 218)}
    }

        ' Refresh so CellFormatting applies the palette immediately
        If g IsNot Nothing Then g.Invalidate()
    End Sub


    ' ─────────────────────────────────────────────────────────────────────────────
    ' Draw and click the buttons INSIDE equipment_picture column
    ' ─────────────────────────────────────────────────────────────────────────────
    Private Sub DataGridView1_CellPainting(sender As Object, e As DataGridViewCellPaintingEventArgs) _
    Handles DataGridView1.CellPainting

        If e.RowIndex < 0 Then Return
        Dim dgv = DirectCast(sender, DataGridView)
        If dgv.Columns(e.ColumnIndex).Name <> "equipment_picture" Then Return

        ' Apply your palette color if set
        If _colPalette IsNot Nothing AndAlso _colPalette.ContainsKey("equipment_picture") Then
            e.CellStyle.BackColor = _colPalette("equipment_picture")
        End If

        e.Handled = True
        e.Paint(e.CellBounds, DataGridViewPaintParts.Background Or DataGridViewPaintParts.Border)

        ' Determine if row has a picture
        Dim hasPic As Boolean = False
        If dgv.Columns.Contains("has_picture") Then
            Dim v = dgv.Rows(e.RowIndex).Cells("has_picture").Value
            If v IsNot Nothing AndAlso v IsNot DBNull.Value Then hasPic = Convert.ToBoolean(v)
        End If

        ' RELATIVE rects → offset to ABSOLUTE for drawing
        Dim rel = GetPicButtonRectsRelative(e.CellBounds.Size, hasPic)

        Dim r1 As Rectangle = rel.R1 : r1.Offset(e.CellBounds.Location)
        ButtonRenderer.DrawButton(e.Graphics, r1, VisualStyles.PushButtonState.Default)
        TextRenderer.DrawText(e.Graphics,
                          If(hasPic, "View", "Upload"),
                          e.CellStyle.Font, r1, Color.Black,
                          TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)

        If hasPic AndAlso rel.R2.HasValue Then
            Dim r2 As Rectangle = rel.R2.Value : r2.Offset(e.CellBounds.Location)
            ButtonRenderer.DrawButton(e.Graphics, r2, VisualStyles.PushButtonState.Default)
            TextRenderer.DrawText(e.Graphics, "Replace",
                              e.CellStyle.Font, r2, Color.Black,
                              TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)
        End If

        If hasPic AndAlso rel.R3.HasValue Then
            Dim r3 As Rectangle = rel.R3.Value : r3.Offset(e.CellBounds.Location)
            ButtonRenderer.DrawButton(e.Graphics, r3, VisualStyles.PushButtonState.Default)
            TextRenderer.DrawText(e.Graphics, "Remove",
                              e.CellStyle.Font, r3, Color.Black,
                              TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)
        End If
    End Sub



    Private Sub DataGridView1_CellParsing(sender As Object, e As DataGridViewCellParsingEventArgs) _
    Handles DataGridView1.CellParsing

        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Return
        Dim colName = DataGridView1.Columns(e.ColumnIndex).Name
        If Not _freeTypeComboCols.Contains(colName) Then Return

        ' Coerce to string cleanly
        Dim s As String = If(TryCast(e.Value, String), Convert.ToString(e.Value))
        If s Is Nothing Then s = ""
        s = s.Trim()
        e.Value = s
        e.ParsingApplied = True
    End Sub



    ' =======================
    ' CLICK: use e.Location directly
    ' =======================
    Private Async Sub DataGridView1_CellMouseClick(sender As Object, e As DataGridViewCellMouseEventArgs) _
    Handles DataGridView1.CellMouseClick

        If e.RowIndex < 0 OrElse e.Button <> MouseButtons.Left Then Return
        Dim dgv = DirectCast(sender, DataGridView)
        If dgv.Columns(e.ColumnIndex).Name <> "equipment_picture" Then Return

        Dim idObj = dgv.Rows(e.RowIndex).Cells("ID").Value
        If idObj Is Nothing OrElse idObj Is DBNull.Value Then Return
        Dim id As Integer = CInt(idObj)

        Dim hasPic As Boolean = False
        If dgv.Columns.Contains("has_picture") Then
            Dim v = dgv.Rows(e.RowIndex).Cells("has_picture").Value
            If v IsNot Nothing AndAlso v IsNot DBNull.Value Then hasPic = Convert.ToBoolean(v)
        End If

        ' e.Location is already cell-relative
        Dim ptRel As Point = e.Location
        Dim cellSize As Size = dgv.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, True).Size
        Dim rel = GetPicButtonRectsRelative(cellSize, hasPic)

        If hasPic Then
            If rel.R1.Contains(ptRel) Then
                ' View
                Dim bytes = Await LoadPPEImageBytesAsync(id)
                If bytes Is Nothing OrElse bytes.Length = 0 Then
                    MessageBox.Show("No image stored for this row.", "Image", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Return
                End If
                Dim tmp = IO.Path.Combine(IO.Path.GetTempPath(), $"ppe_{id}.jpg")
                IO.File.WriteAllBytes(tmp, bytes)
                Process.Start(New ProcessStartInfo(tmp) With {.UseShellExecute = True})

            ElseIf rel.R2.HasValue AndAlso rel.R2.Value.Contains(ptRel) Then
                ' Replace
                Await PromptAndSaveImageAsync(id, dgv.Rows(e.RowIndex))

            ElseIf rel.R3.HasValue AndAlso rel.R3.Value.Contains(ptRel) Then
                ' Remove (set NULL)
                If MessageBox.Show("Remove the stored image for this row?", "Remove Image",
                               MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                    Await UpdatePPEImageAsync(id, Nothing)        ' <- writes NULL
                    dgv.Rows(e.RowIndex).Cells("has_picture").Value = False
                    DataGridView1.InvalidateRow(e.RowIndex)
                End If
            End If
        Else
            ' Upload
            If rel.R1.Contains(ptRel) Then
                Await PromptAndSaveImageAsync(id, dgv.Rows(e.RowIndex))
            End If
        End If
    End Sub



    ' =======================
    ' HOVER: use e.Location directly for the hand cursor
    ' =======================
    Private Sub DataGridView1_CellMouseMove(sender As Object, e As DataGridViewCellMouseEventArgs) _
    Handles DataGridView1.CellMouseMove

        Dim dgv = DirectCast(sender, DataGridView)
        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 _
       OrElse dgv.Columns(e.ColumnIndex).Name <> "equipment_picture" Then
            dgv.Cursor = Cursors.Default : Return
        End If

        Dim hasPic As Boolean = False
        If dgv.Columns.Contains("has_picture") Then
            Dim v = dgv.Rows(e.RowIndex).Cells("has_picture").Value
            If v IsNot Nothing AndAlso v IsNot DBNull.Value Then hasPic = Convert.ToBoolean(v)
        End If

        Dim ptRel As Point = e.Location               ' cell-relative already
        Dim cellSize As Size = dgv.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, True).Size
        Dim rel = GetPicButtonRectsRelative(cellSize, hasPic)

        Dim over As Boolean = rel.R1.Contains(ptRel) _
                          OrElse (rel.R2.HasValue AndAlso rel.R2.Value.Contains(ptRel)) _
                          OrElse (rel.R3.HasValue AndAlso rel.R3.Value.Contains(ptRel))

        dgv.Cursor = If(over, Cursors.Hand, Cursors.Default)
    End Sub



    Private Sub DataGridView1_MouseLeave(sender As Object, e As EventArgs) _
    Handles DataGridView1.MouseLeave
        DataGridView1.Cursor = Cursors.Default
    End Sub

    Private Sub DataGridView1_Scroll(sender As Object, e As ScrollEventArgs) _
    Handles DataGridView1.Scroll

        ' Keep the picture buttons redrawn
        If DataGridView1.Columns.Contains("equipment_picture") Then
            If e.ScrollOrientation = ScrollOrientation.VerticalScroll _
        OrElse e.ScrollOrientation = ScrollOrientation.HorizontalScroll Then
                DataGridView1.InvalidateColumn(DataGridView1.Columns("equipment_picture").Index)
            End If
        End If

        ' Reposition/hide the date picker when scrolling
        If _dtp IsNot Nothing AndAlso _dtp.Visible Then
            Grid_RepositionDatePicker(sender, EventArgs.Empty)
        End If
    End Sub





    ' ─────────────────────────────────────────────────────────────────────────────
    ' Image I/O (blob only on demand; JPEG at ~80% quality)
    ' ─────────────────────────────────────────────────────────────────────────────
    Private Async Function LoadPPEImageBytesAsync(id As Integer) As Task(Of Byte())
        Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
            Await conn.OpenAsync()
            Using cmd As New SqlCommand("SELECT equipment_picture FROM TBL_PPE WITH (NOLOCK) WHERE ID=@id;", conn)
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = id
                Dim obj = Await cmd.ExecuteScalarAsync()
                If obj Is Nothing OrElse obj Is DBNull.Value Then Return Nothing
                Return CType(obj, Byte())
            End Using
        End Using
    End Function

    Private Async Function UpdatePPEImageAsync(id As Integer, jpegBytes As Byte()) As Task
        Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
            Await conn.OpenAsync()
            Using cmd As New SqlCommand("UPDATE TBL_PPE SET equipment_picture=@img WHERE ID=@id;", conn)
                cmd.Parameters.Add("@img", SqlDbType.VarBinary, If(jpegBytes Is Nothing, 0, jpegBytes.Length)).Value =
                If(jpegBytes, CType(DBNull.Value, Object))
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = id
                Await cmd.ExecuteNonQueryAsync()
            End Using
        End Using
    End Function


    Private Function ImageToJpegBytes(img As Image, quality As Long) As Byte()
        If quality < 1L Then quality = 1L
        If quality > 100L Then quality = 100L
        Dim enc = ImageCodecInfo.GetImageEncoders().First(Function(c) c.MimeType = "image/jpeg")
        Dim ep As New EncoderParameters(1)
        ep.Param(0) = New EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality)
        Using ms As New MemoryStream()
            img.Save(ms, enc, ep)
            Return ms.ToArray()
        End Using
    End Function

    Private Async Function PromptAndSaveImageAsync(id As Integer, row As DataGridViewRow) As Task
        Using ofd As New OpenFileDialog()
            ofd.Title = "Select an image"
            ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif|All Files|*.*"
            ofd.RestoreDirectory = True

            If ofd.ShowDialog(Me) = DialogResult.OK Then
                Using src As Image = Image.FromFile(ofd.FileName)

                    ' Step 1: resize (downscale if needed)
                    Using resized As Image = ResizeImage(src, 1024, 768) ' adjust max size as you like

                        ' Step 2: compress iteratively until 40–70 KB
                        Dim jpegBytes = CompressToTargetSize(resized, 40, 70)

                        ' Step 3: save to DB
                        Await UpdatePPEImageAsync(id, jpegBytes)
                        row.Cells("has_picture").Value = True
                        DataGridView1.InvalidateRow(row.Index)
                    End Using
                End Using
            End If
        End Using
    End Function

    Private Function ResizeImage(src As Image, maxWidth As Integer, maxHeight As Integer) As Image
        Dim ratioX As Double = maxWidth / src.Width
        Dim ratioY As Double = maxHeight / src.Height
        Dim ratio As Double = Math.Min(ratioX, ratioY)

        If ratio >= 1 Then
            ' No resize needed
            Return New Bitmap(src)
        End If

        Dim newW As Integer = CInt(src.Width * ratio)
        Dim newH As Integer = CInt(src.Height * ratio)

        Dim bmp As New Bitmap(newW, newH)
        Using g As Graphics = Graphics.FromImage(bmp)
            g.CompositingQuality = Drawing2D.CompositingQuality.HighQuality
            g.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
            g.SmoothingMode = Drawing2D.SmoothingMode.HighQuality
            g.DrawImage(src, 0, 0, newW, newH)
        End Using
        Return bmp
    End Function

    Private Function CompressToTargetSize(img As Image,
                                      Optional targetMinKB As Integer = 40,
                                      Optional targetMaxKB As Integer = 70) As Byte()

        Dim result As Byte() = Nothing
        Dim best As Byte() = Nothing

        For q As Long = 80 To 10 Step -10
            result = ImageToJpegBytes(img, q)
            Dim sizeKB = result.Length \ 1024

            best = result
            If sizeKB <= targetMaxKB Then
                If sizeKB >= targetMinKB OrElse q = 10 Then
                    Exit For
                End If
            End If
        Next

        Return best
    End Function

    ' Commit & save when user presses Enter inside the combo editor
    ' 3) Enter commits immediately
    Private Sub Combo_KeyDown(sender As Object, e As KeyEventArgs)
        If e.KeyCode = Keys.Enter Then
            e.Handled = True : e.SuppressKeyPress = True
            CommitComboEdit()
        End If
    End Sub


    ' Commit & save when the editor loses focus
    Private Sub Combo_Validated(sender As Object, e As EventArgs)
        CommitComboEdit()
    End Sub

    ' Core logic: add new text to list, commit edit, and save row
    ' 5) Commit + save + add to dropdown if new
    Private Sub CommitComboEdit()
        If DataGridView1.CurrentCell Is Nothing OrElse _activeCombo Is Nothing Then Exit Sub

        Dim colName = DataGridView1.Columns(DataGridView1.CurrentCell.ColumnIndex).Name
        If Not _freeTypeComboCols.Contains(colName) Then Exit Sub

        Dim newText As String = If(_activeCombo.Text, String.Empty).Trim()
        If newText = "" Then Exit Sub

        ' Ensure the dropdown contains the new text
        Dim col = TryCast(DataGridView1.Columns(colName), DataGridViewComboBoxColumn)
        If col IsNot Nothing Then
            Dim exists = col.Items.Cast(Of Object)().
            Any(Function(x) String.Equals(CStr(x), newText, StringComparison.OrdinalIgnoreCase))
            If Not exists Then
                col.Items.Add(newText)

                ' Keep backing lists in sync
                If String.Equals(colName, "supplier", StringComparison.OrdinalIgnoreCase) Then
                    If Not _supplierNames.Any(Function(s) String.Equals(s, newText, StringComparison.OrdinalIgnoreCase)) Then
                        _supplierNames.Add(newText)
                    End If
                ElseIf String.Equals(colName, "ptr_to", StringComparison.OrdinalIgnoreCase) _
                OrElse String.Equals(colName, "current_end_user", StringComparison.OrdinalIgnoreCase) Then
                    If Not _employeeNames.Any(Function(s) String.Equals(s, newText, StringComparison.OrdinalIgnoreCase)) Then
                        _employeeNames.Add(newText)
                    End If
                End If
            End If
        End If

        ' Write value into the cell
        DataGridView1.CurrentCell.Value = newText
        DataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit)
        DataGridView1.EndEdit()

        ' Now check if the row is bound to a DataRowView
        Dim view As DataRowView = TryCast(DataGridView1.CurrentRow?.DataBoundItem, DataRowView)
        If view Is Nothing Then Exit Sub   ' <── prevent NullReference crash

        Dim row As DataRow = view.Row
        row(colName) = newText
        SaveRow(row)
    End Sub


    ' New: index-based, null-safe commit that does not rely on CurrentCell
    Private Sub CommitComboEditAt(rowIndex As Integer, colIndex As Integer, newText As String)
        ' ── Guards ───────────────────────────────────────────────────────────────────
        If rowIndex < 0 OrElse colIndex < 0 Then Exit Sub
        If DataGridView1.Rows.Count = 0 Then Exit Sub
        newText = If(newText, "").Trim()
        If newText = "" Then Exit Sub   ' nothing to save

        ' ── Resolve column / treat-as-combo flag ─────────────────────────────────────
        Dim colName As String = DataGridView1.Columns(colIndex).Name
        Dim comboCol As DataGridViewComboBoxColumn =
        TryCast(DataGridView1.Columns(colIndex), DataGridViewComboBoxColumn)
        Dim treatAsCombo As Boolean = (comboCol IsNot Nothing)

        ' ── 1) Ensure dropdown contains the value (only if it's a real combo) ───────
        If treatAsCombo Then
            Dim exists = comboCol.Items.Cast(Of Object)().
            Any(Function(x) String.Equals(CStr(x), newText, StringComparison.OrdinalIgnoreCase))
            If Not exists Then
                comboCol.Items.Add(newText)

                ' Keep backing lists in sync for known combos
                If String.Equals(colName, "supplier", StringComparison.OrdinalIgnoreCase) Then
                    If Not _supplierNames.Any(Function(s) String.Equals(s, newText, StringComparison.OrdinalIgnoreCase)) Then
                        _supplierNames.Add(newText)
                    End If
                ElseIf colName.Equals("ptr_to", StringComparison.OrdinalIgnoreCase) _
                OrElse colName.Equals("current_end_user", StringComparison.OrdinalIgnoreCase) Then
                    If Not _employeeNames.Any(Function(s) String.Equals(s, newText, StringComparison.OrdinalIgnoreCase)) Then
                        _employeeNames.Add(newText)
                    End If
                End If
            End If
        End If

        ' ── 2) Write into the grid cell (don’t rely on CurrentCell) ─────────────────
        Dim rowObj = DataGridView1.Rows(rowIndex)
        rowObj.Cells(colIndex).Value = newText

        ' ── 3) Push editor → binding manager (ensures DataRowView sees the change) ──
        CommitAllEdits()

        ' ── 4) Save the bound row (null-safe) ────────────────────────────────────────
        Dim view As DataRowView = TryCast(rowObj.DataBoundItem, DataRowView)
        If view Is Nothing Then Exit Sub

        ' Write again at the data level to ensure RowState = Modified
        view.Row(colName) = newText
        view.EndEdit()
        SaveRow(view.Row)
    End Sub




    Private Sub cmddelete_Click(sender As Object, e As EventArgs) Handles cmddelete.Click
        If DataGridView1.Rows.Count = 0 Then
            MessageBox.Show("Nothing to delete.", "Delete", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        ' Gather selected rows (fallback to current)
        Dim rows As IEnumerable(Of DataGridViewRow)
        If DataGridView1.SelectedRows.Count > 0 Then
            rows = DataGridView1.SelectedRows.Cast(Of DataGridViewRow)()
        ElseIf DataGridView1.CurrentRow IsNot Nothing AndAlso Not DataGridView1.CurrentRow.IsNewRow Then
            rows = {DataGridView1.CurrentRow}
        Else
            MessageBox.Show("Please select at least one row.", "Delete", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        ' Prepare list (ID, Desc, Qty)
        Dim items As New List(Of (ID As Integer, Desc As String, Qty As Decimal))
        For Each r In rows
            If r.IsNewRow Then Continue For
            Dim idObj = r.Cells("ID").Value
            If idObj Is Nothing OrElse idObj Is DBNull.Value Then Continue For

            Dim id As Integer = CInt(idObj)
            Dim desc As String = Convert.ToString(r.Cells("description").Value)
            If desc IsNot Nothing Then desc = desc.Trim()

            Dim qtyDec As Decimal = 0D
            Decimal.TryParse(Convert.ToString(r.Cells("qty").Value), qtyDec)
            If qtyDec < 0D Then qtyDec = 0D

            items.Add((id, If(desc, String.Empty), qtyDec))
        Next
        If items.Count = 0 Then
            MessageBox.Show("No valid rows selected.", "Delete", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        If MessageBox.Show($"Delete {items.Count} selected Property?{Environment.NewLine}" &
                       "This will also return their quantities to the Inventory.",
                       "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) <> DialogResult.Yes Then
            Return
        End If

        cmddelete.Enabled = False
        Dim deletedCount As Integer = 0

        Try
            Using conn As New SqlClient.SqlConnection(My.Forms.frmmain.txtdb.Text)
                conn.Open()
                Using tx = conn.BeginTransaction(IsolationLevel.ReadCommitted)
                    ' Make errors abort immediately
                    Using cmdOn As New SqlClient.SqlCommand("SET XACT_ABORT ON; SET NOCOUNT ON;", conn, tx)
                        cmdOn.ExecuteNonQuery()
                    End Using

                    ' Reusable commands
                    Dim sqlUpd As String =
                    "UPDATE inv WITH (ROWLOCK, UPDLOCK) " &
                    "SET inv.QTY = ISNULL(inv.QTY, 0) + @add " &
                    "FROM TBL_INVENTORY AS inv " &
                    "WHERE LTRIM(RTRIM(inv.DESCRIPTIONS)) = LTRIM(RTRIM(@desc));"   ' <-- DESCRIPTIONS (plural)

                    Dim cmdUpd As New SqlClient.SqlCommand(sqlUpd, conn, tx)
                    Dim pAdd = cmdUpd.Parameters.Add("@add", SqlDbType.Decimal)
                    pAdd.Precision = 18 : pAdd.Scale = 2
                    Dim pDesc = cmdUpd.Parameters.Add("@desc", SqlDbType.NVarChar, 3500)

                    Dim cmdDel As New SqlClient.SqlCommand("DELETE FROM TBL_PPE WHERE ID = @id;", conn, tx)
                    cmdDel.Parameters.Add("@id", SqlDbType.Int)

                    ' Process each selected row: try update → show message → delete anyway
                    For Each it In items
                        Dim found As Boolean = False

                        If Not String.IsNullOrWhiteSpace(it.Desc) AndAlso it.Qty > 0D Then
                            pAdd.Value = it.Qty
                            pDesc.Value = it.Desc
                            Dim affected As Integer = cmdUpd.ExecuteNonQuery()
                            If affected > 0 Then
                                found = True
                            End If
                        End If

                        ' Always delete the PPE row
                        cmdDel.Parameters("@id").Value = it.ID
                        deletedCount += cmdDel.ExecuteNonQuery()
                    Next

                    tx.Commit()
                End Using
            End Using

            ' Refresh grid
            LoadPage()

            If deletedCount > 0 Then
                MessageBox.Show($"{deletedCount} row(s) deleted.", "Deleted",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If

        Catch ex As Exception
            MessageBox.Show("Delete error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            cmddelete.Enabled = True
        End Try
    End Sub

    ' Force-commit any pending edits from the grid to the bound DataRowView
    ' Push any pending editor changes → DataRowView
    Private Sub CommitAllEdits()
        Try
            If DataGridView1.IsCurrentCellDirty Then
                DataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit)
            End If
            DataGridView1.EndEdit()

            Dim cm As CurrencyManager = TryCast(Me.BindingContext(DataGridView1.DataSource), CurrencyManager)
            If cm IsNot Nothing Then cm.EndCurrentEdit()

            If _ppeTable IsNot Nothing Then
                Dim cm2 As CurrencyManager = TryCast(Me.BindingContext(_ppeTable), CurrencyManager)
                If cm2 IsNot Nothing Then cm2.EndCurrentEdit()
            End If
        Catch
            ' ignore
        End Try
    End Sub



    Private Sub frmissuancePPE_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        ' If the picker is visible, we still commit and save the current row
        If _dtp IsNot Nothing AndAlso _dtp.Visible Then
            CommitAllEdits()
        End If

        ' Push any pending changes then persist table-level changes (if any)
        CommitAllEdits()

        ' If there are pending DataRow changes, persist now
        If _ppeTable IsNot Nothing AndAlso _ppeTable.GetChanges() IsNot Nothing Then
            SaveChanges()
        End If
    End Sub

    ' Keep only the NAME portion (DB is "NAME{newline}POSITION")
    Private Function NameOnlyFromCNames(raw As String) As String
        If String.IsNullOrWhiteSpace(raw) Then Return ""
        Dim s = raw.Replace(vbCrLf, vbLf)
        Dim parts = s.Split(New String() {vbLf}, StringSplitOptions.RemoveEmptyEntries)
        Return parts(0).Trim() ' first line is the NAME
    End Function

    Private Sub LoadApproverEmployeeNames()
        _employeeNames.Clear()
        Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
            conn.Open()
            Using cmd As New SqlCommand("
            SELECT CNAMES 
            FROM TBL_APPROVER WITH (NOLOCK)
            WHERE TYPE = 'Employee' AND LTRIM(RTRIM(ISNULL(CNAMES,''))) <> ''
            ORDER BY CNAMES;", conn)
                Using r = cmd.ExecuteReader()
                    Dim setNames As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
                    While r.Read()
                        Dim raw As String = r.GetString(0)
                        Dim nameOnly As String = NameOnlyFromCNames(raw)
                        If nameOnly <> "" AndAlso setNames.Add(nameOnly) Then
                            _employeeNames.Add(nameOnly)
                        End If
                    End While
                End Using
            End Using
        End Using
        _employeeNames.Sort(StringComparer.OrdinalIgnoreCase)
    End Sub

    ' Force these combo columns to be pure string → string binding
    Private Sub NormalizeComboColumns()
        Dim names = New String() {"supplier", "ptr_to", "current_end_user", "section", "type_of_ppe"}
        For Each n In names
            If DataGridView1.Columns.Contains(n) Then
                Dim c = TryCast(DataGridView1.Columns(n), DataGridViewComboBoxColumn)
                If c IsNot Nothing Then
                    c.ValueType = GetType(String)
                    c.DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton
                    c.DisplayStyleForCurrentCellOnly = True
                    c.FlatStyle = FlatStyle.Flat
                End If
            End If
        Next
    End Sub


End Class
