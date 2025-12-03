Imports System.Data
Imports System.Data.SqlClient
Imports System.Drawing
Imports System.IO
Imports System.Text
Imports System.Diagnostics
Imports System.Drawing.Imaging
Imports System.Globalization
Imports System.Text.RegularExpressions

Public Class frmissuancesep

    ' ─────────────────────────────────────────────────────────────────────────────
    ' Filters & paging state
    ' ─────────────────────────────────────────────────────────────────────────────
    Private _lastSearch As String = ""
    Private _lastType As String = "All"          ' Combosep -> type_of_sep
    Private _lastSection As String = "All"       ' Combooffice -> section_division
    Private _lastYear As String = "All"          ' Comboyear -> YEAR(date_procured)
    Private _suppressFilterEvents As Boolean = False

    Private Const PageSize As Integer = 100
    Private _pageIndex As Integer = 1
    Private _totalRows As Integer = 0
    Private _totalPages As Integer = 1

    ' Backing table & adapter (we DO NOT fetch the image blob in the adapter)
    Private _sepTable As New DataTable()
    Private _sepAdapter As SqlDataAdapter

    ' Lookups
    Private _supplierList As New List(Of String)()
    Private _employeeNames As New List(Of String)() ' clean names from TBL_APPROVER (TYPE='Employee')

    ' Combos that allow free typing
    Private ReadOnly _freeTypeComboCols As HashSet(Of String) =
        New HashSet(Of String)(StringComparer.OrdinalIgnoreCase) From {
            "type_of_sep", "supplier_name", "section_division",
            "locations", "ptr_to", "type_of_fixed_asset", "current_end_user"
        }

    ' Column paletteF
    Private _colPalette As Dictionary(Of String, Color) = Nothing

    ' Picture button layout inside the equipment_picture host column
    Private ReadOnly _btnWidth As Integer = 76
    Private ReadOnly _btnHeight As Integer = 22
    Private ReadOnly _btnSpacing As Integer = 8

    ' Track active Combo editor
    Private _activeCombo As DataGridViewComboBoxEditingControl = Nothing

    ' Track active TextBox editor (for numeric key filter)
    Private WithEvents _activeTextBox As TextBox = Nothing


    ' ── Row palette (base “blush” like your screenshot) ────────────────────────────
    Private ReadOnly _rowBase As Color = Color.FromArgb(244, 217, 223)   ' default row
    Private ReadOnly _rowAlt As Color = Color.FromArgb(248, 227, 232)   ' optional alt
    Private ReadOnly _selBack As Color = Color.FromArgb(210, 222, 255)   ' your selection

    ' In–cell DatePicker for DATE PROCURED (read-only cell; edited via picker)
    Private WithEvents _dtp As New DateTimePicker() With {
        .Format = DateTimePickerFormat.Custom,
        .CustomFormat = "yyyy-MM-dd",
        .Visible = False
    }
    Private _dtpCell As (Row As Integer, Col As Integer)
    Private _employeeList As New List(Of String)()
    Private _bs As New BindingSource()


    ' SECTION → RESPONSIBILITY CODE CENTER
    Private ReadOnly _sectionToRcc As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase) From {
        {"OFFICE OF THE MINISTER", "OM"},
        {"OFFICE OF THE DEPUTY MINISTER", "ODM"},
        {"OFFICE OF DIRECTOR GENERAL", "ODG"},
        {"ADMINISTRATIVE AND FINANCE DIVISION (CAO)", "AFD"},
        {"ACCOUNTING SECTION", "AS"},
        {"BUDGET SECTION", "BS"},
        {"PROCUREMENT MANAGEMENT SECTION", "PMS"},
        {"CASH SECTION", "CS"},
        {"ARCHIVES AND RECORDS SECTION", "ARS"},
        {"HUMAN RESOURCE MANAGEMENT SECTION", "HR"},
        {"SUPPLY SECTION", "SS"},
        {"GENERAL SERVICES SECTION", "GSS"},
        {"LEGAL AND LEGISLATIVE LIAISON SECTION", "LLLS"},
        {"PLANNING SECTION", "PS"},
        {"INFORMATION AND COMMUNICATION SECTION", "ICS"},
        {"INTERNAL AUDIT SECTION", "IAS"},
        {"BANGSAMORO PEACE RECONCILIATION AND UNIFICATION SERVICES", "BPRUS"},
        {"PEACE EDUCATION DIVISION", "PED"},
        {"ALTERNATIVE DISPUTE RESOLUTION DIVISION (CAO-V)", "ADRD (CAO V)"},
        {"COMMUNITY AFFAIRS SECTION (MAG)", "ADRD (MAG)"},
        {"COMMUNITY AFFAIRS SECTION (LDS)", "ADRD (LDS)"},
        {"COMMUNITY AFFAIRS SECTION (SGA)", "ADRD (SGA)"},
        {"COMMUNITY AFFAIRS SECTION (BAS)", "ADRD (BAS)"},
        {"COMMUNITY AFFAIRS SECTION (SUL)", "ADRD (SUL)"},
        {"COMMUNITY AFFAIRS SECTION (TAW)", "ADRD (TAW)"},
        {"HOME AFFAIRS SERVICES", "HAS"},
        {"LAW ENFORCEMENT COORDINATION DIVISION", "LECD"},
        {"CIVILIAN INTELLIGENCE AND INVESTIGATION DIVISION", "CIID"}
    }



    Private Function SectionList() As IEnumerable(Of String)
        Return _sectionToRcc.Keys.OrderBy(Function(s) s, StringComparer.OrdinalIgnoreCase)
    End Function

    ' ─────────────────────────────────────────────────────────────────────────────
    ' Helpers: picture host layout (relative rects)
    ' ─────────────────────────────────────────────────────────────────────────────
    Private Function GetPicButtonRectsRelative(cellSize As Size, hasPic As Boolean) _
        As (R1 As Rectangle, R2 As Rectangle?, R3 As Rectangle?)
        Dim y As Integer = (cellSize.Height - _btnHeight) \ 2
        If hasPic Then
            Dim count As Integer = 3
            Dim totalW = (_btnWidth * count) + (_btnSpacing * (count - 1))
            Dim startX = (cellSize.Width - totalW) \ 2
            Dim r1 As New Rectangle(startX, y, _btnWidth, _btnHeight)                                    ' View
            Dim r2 As New Rectangle(startX + (_btnWidth + _btnSpacing), y, _btnWidth, _btnHeight)        ' Replace
            Dim r3 As New Rectangle(startX + 2 * (_btnWidth + _btnSpacing), y, _btnWidth, _btnHeight)    ' Remove
            Return (r1, r2, r3)
        Else
            Dim startX = (cellSize.Width - _btnWidth) \ 2
            Dim r1 As New Rectangle(startX, y, _btnWidth, _btnHeight)                                     ' Upload
            Return (r1, Nothing, Nothing)
        End If
    End Function

    ' ─────────────────────────────────────────────────────────────────────────────
    ' Load
    ' ─────────────────────────────────────────────────────────────────────────────
    Private Sub frmissuancesep_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadLookups()
        ConfigureSEPGrid()
        BuildAdapter()
        LoadFilterCombos()
        ResetAndSearch()
        ApplySEPColumnColors(DataGridView1)
    End Sub

    ' ─────────────────────────────────────────────────────────────────────────────
    ' Lookups (Suppliers + Employees)
    ' ─────────────────────────────────────────────────────────────────────────────
    Private Sub LoadLookups()
        _supplierList.Clear()
        _employeeNames.Clear()

        Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
            conn.Open()

            ' ---------- SUPPLIERS ----------
            Using cmd As New SqlCommand("
            SELECT DISTINCT LTRIM(RTRIM(RFQ_NCOMPANY))
            FROM TBL_RFQSUPPLIER
            WHERE LTRIM(RTRIM(ISNULL(RFQ_NCOMPANY,'')))<>'' 
            ORDER BY 1;", conn)
                Using r = cmd.ExecuteReader()
                    While r.Read()
                        _supplierList.Add(r.GetString(0).Trim())
                    End While
                End Using
            End Using

            Using cmd As New SqlCommand("
            SELECT DISTINCT LTRIM(RTRIM(supplier_name))
            FROM TBL_SEP
            WHERE LTRIM(RTRIM(ISNULL(supplier_name,'')))<>'' 
            ORDER BY 1;", conn)
                Using r = cmd.ExecuteReader()
                    While r.Read()
                        Dim s = r.GetString(0).Trim()
                        If Not _supplierList.Any(Function(x) x.Equals(s, StringComparison.OrdinalIgnoreCase)) Then
                            _supplierList.Add(s)
                        End If
                    End While
                End Using
            End Using

            ' ---------- EMPLOYEES: CNAMES (TYPE=Employee) → NAME ONLY ----------
            Dim empSet As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

            Using cmd As New SqlCommand("
            SELECT CNAMES
            FROM TBL_APPROVER
            WHERE TYPE='Employee'
            ORDER BY CNAMES;", conn)

                Using r = cmd.ExecuteReader()
                    While r.Read()
                        If Not r.IsDBNull(0) Then
                            Dim raw As String = r.GetString(0)          ' e.g. "NAME{CRLF}POSITION" or "NAME  POSITION"
                            Dim nameOnly As String = ExtractNameOnly(raw)
                            If nameOnly <> "" Then empSet.Add(nameOnly)
                        End If
                    End While
                End Using
            End Using

            _employeeNames = empSet.
                         Where(Function(s) Not String.IsNullOrWhiteSpace(s)).
                         Select(Function(s) s.Trim()).
                         OrderBy(Function(s) s, StringComparer.OrdinalIgnoreCase).
                         ToList()
        End Using
    End Sub




    ' Returns only the NAME from CNAMES (drops POSITION even if newline is missing)
    Private Function ExtractNameOnly(raw As String) As String
        If String.IsNullOrEmpty(raw) Then Return ""

        ' Normalize odd whitespaces (tabs, NBSP)
        Dim s As String = raw.Replace(vbTab, " ").Replace(ChrW(&HA0), " ")

        ' Convert any kind of line separator to LF: CRLF, CR, NEL(0x85), LS(2028), PS(2029)
        s = s.Replace(vbCrLf, vbLf).Replace(vbCr, vbLf).Replace(vbLf, vbLf)
        s = s.Replace(ChrW(&H85), vbLf).Replace(ChrW(&H2028), vbLf).Replace(ChrW(&H2029), vbLf)

        ' If we have any newline, keep only the first line (the name)
        Dim nlIdx As Integer = s.IndexOf(vbLf, StringComparison.Ordinal)
        Dim firstLine As String = If(nlIdx >= 0, s.Substring(0, nlIdx), s)

        ' If no newline was found, try to cut off before "position-looking" text:
        '   - two or more spaces (name  ␠␠  POSITION...)
        '   - a single space followed by 2+ uppercase letters (e.g., " HRM", " OFFICER")
        If nlIdx < 0 Then
            Dim m = Regex.Match(firstLine, "^(.*?)(?=\s{2,}|\s+[A-Z]{2,}(?:\s|$))")
            If m.Success AndAlso m.Groups(1).Value.Trim().Length > 0 Then
                firstLine = m.Groups(1).Value
            End If
        End If

        ' Collapse internal double spaces inside the name
        firstLine = Regex.Replace(firstLine, "\s{2,}", " ")

        Return firstLine.Trim()
    End Function


    ' ─────────────────────────────────────────────────────────────────────────────
    ' Filters UI
    ' ─────────────────────────────────────────────────────────────────────────────
    Private Sub LoadFilterCombos()
        _suppressFilterEvents = True

        Combooffice.Items.Clear() : Combooffice.Items.Add("All")
        Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
            conn.Open()
            Using cmd As New SqlCommand("
                SELECT DISTINCT LTRIM(RTRIM(section_division))
                FROM TBL_SEP
                WHERE LTRIM(RTRIM(ISNULL(section_division,'')))<>'' 
                ORDER BY 1;", conn)
                Using r = cmd.ExecuteReader()
                    While r.Read()
                        Combooffice.Items.Add(r.GetString(0))
                    End While
                End Using
            End Using
        End Using
        Combooffice.SelectedIndex = 0

        Combosep.Items.Clear() : Combosep.Items.Add("All")
        Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
            conn.Open()
            Using cmd As New SqlCommand("
                SELECT DISTINCT LTRIM(RTRIM(type_of_sep))
                FROM TBL_SEP
                WHERE LTRIM(RTRIM(ISNULL(type_of_sep,'')))<>'' 
                ORDER BY 1;", conn)
                Using r = cmd.ExecuteReader()
                    While r.Read()
                        Combosep.Items.Add(r.GetString(0))
                    End While
                End Using
            End Using
        End Using
        Combosep.SelectedIndex = 0

        Comboyear.Items.Clear() : Comboyear.Items.Add("All")
        Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
            conn.Open()
            Using cmd As New SqlCommand("
                SELECT DISTINCT YEAR(date_procured) 
                FROM TBL_SEP
                WHERE date_procured IS NOT NULL
                ORDER BY 1 DESC;", conn)
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

    Private Sub ResetAndSearch()
        _pageIndex = 1
        ApplyFiltersAndLoad()
    End Sub

    Private Sub ApplyFiltersAndLoad()
        _lastSearch = txtsearch.Text.Trim()
        _lastType = If(Combosep.SelectedItem Is Nothing, "All", Combosep.SelectedItem.ToString())
        _lastSection = If(Combooffice.SelectedItem Is Nothing, "All", Combooffice.SelectedItem.ToString())
        _lastYear = If(Comboyear.SelectedItem Is Nothing, "All", Comboyear.SelectedItem.ToString())
        LoadPage()
    End Sub

    ' ─────────────────────────────────────────────────────────────────────────────
    ' WHERE builder
    ' ─────────────────────────────────────────────────────────────────────────────
    Private Sub BuildWhere(ByRef whereSql As String, ByRef parms As List(Of SqlParameter))
        Dim sb As New StringBuilder(" WHERE 1=1 ")
        Dim ps As New List(Of SqlParameter)

        If Not String.IsNullOrWhiteSpace(_lastSearch) Then
            sb.Append("
              AND (
                    descriptions LIKE @q OR property_number LIKE @q OR serial_number LIKE @q OR
                    supplier_name LIKE @q OR model LIKE @q OR rlqdp_no LIKE @q OR
                    locations LIKE @q OR section_division LIKE @q OR current_end_user LIKE @q OR ptr_to LIKE @q
                  )")
            ps.Add(New SqlParameter("@q", SqlDbType.VarChar, 500) With {.Value = "%" & _lastSearch & "%"})
        End If

        If _lastType <> "All" AndAlso _lastType <> "" Then
            sb.Append(" AND type_of_sep = @type")
            ps.Add(New SqlParameter("@type", SqlDbType.VarChar, 200) With {.Value = _lastType})
        End If

        If _lastSection <> "All" AndAlso _lastSection <> "" Then
            sb.Append(" AND section_division = @section")
            ps.Add(New SqlParameter("@section", SqlDbType.VarChar, 200) With {.Value = _lastSection})
        End If

        If _lastYear <> "All" AndAlso _lastYear <> "" Then
            sb.Append(" AND date_procured IS NOT NULL AND YEAR(date_procured) = @yr")
            ps.Add(New SqlParameter("@yr", SqlDbType.Int) With {.Value = CInt(_lastYear)})
        End If

        whereSql = sb.ToString()
        parms = ps
    End Sub

    ' ─────────────────────────────────────────────────────────────────────────────
    ' Paging loader  (IMPORTANT: use DATALENGTH for blobs; don't select the image)
    ' ─────────────────────────────────────────────────────────────────────────────
    Private Sub LoadPage()
        Try
            Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
                conn.Open()

                Dim whereSql As String = ""
                Dim ps As List(Of SqlParameter) = Nothing
                BuildWhere(whereSql, ps)

                Using cmdCount As New SqlCommand("SELECT COUNT(*) FROM TBL_SEP" & whereSql & ";", conn)
                    AddClonedParameters(cmdCount, ps)
                    _totalRows = Convert.ToInt32(cmdCount.ExecuteScalar())
                End Using

                _totalPages = Math.Max(1, CInt(Math.Ceiling(_totalRows / CDbl(PageSize))))
                If _pageIndex < 1 Then _pageIndex = 1
                If _pageIndex > _totalPages Then _pageIndex = _totalPages
                Dim offset As Integer = (_pageIndex - 1) * PageSize

                Dim selectSql As String =
                    "SELECT id, ics_no, ics_date, iar_no, ris_no, type_of_sep, date_procured, " &
                    "descriptions, property_number, life_span, serial_number, qty, unit, unit_cost, total_cost, " &
                    "par_mr_to, position, section_division, supplier_name, ptr_to, type_of_fixed_asset, prepared_by, " &
                    "ics_docs, sticker_tagging, conditions, locations, rlqdp_no, model, remarks, " &
                    "responsibility_code_center, current_end_user, unit_cost_missing_damage, sep_card, " &
                    "CASE WHEN DATALENGTH(equipment_picture) > 0 THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS has_picture " &
                    "FROM TBL_SEP " & whereSql &
                    " ORDER BY ISNULL(ics_date,'1900-01-01') DESC, id DESC " &
                    " OFFSET @off ROWS FETCH NEXT @ps ROWS ONLY;"

                Using da As New SqlDataAdapter(selectSql, conn)
                    AddClonedParameters(da.SelectCommand, ps)
                    da.SelectCommand.Parameters.Add("@off", SqlDbType.Int).Value = offset
                    da.SelectCommand.Parameters.Add("@ps", SqlDbType.Int).Value = PageSize

                    _sepTable.Clear()
                    da.Fill(_sepTable)

                    ' Coerce "1"/"0" string flags into bools for the grid
                    CoerceCheckColumn01(_sepTable, "ics_docs")
                    CoerceCheckColumn01(_sepTable, "sticker_tagging")
                    CoerceCheckColumn01(_sepTable, "sep_card")
                End Using
            End Using

            DataGridView1.DataSource = _sepTable
            ' Bind via BindingSource (do this only once; reuse the same instance)
            If DataGridView1.DataSource Is Nothing OrElse Not Object.ReferenceEquals(DataGridView1.DataSource, _bs) Then
                _bs.DataSource = _sepTable
                DataGridView1.DataSource = _bs
            Else
                ' If already bound, just reset the data
                _bs.DataSource = _sepTable
            End If

            ' Make sure each Combo column has all values from this page (prevents Combo errors)
            EnsureColumnHasAllValues("type_of_sep")
            EnsureColumnHasAllValues("par_mr_to")
            EnsureColumnHasAllValues("position")
            EnsureColumnHasAllValues("section_division")
            EnsureColumnHasAllValues("supplier_name")
            EnsureColumnHasAllValues("ptr_to")
            EnsureColumnHasAllValues("type_of_fixed_asset")
            EnsureColumnHasAllValues("prepared_by")
            EnsureColumnHasAllValues("conditions")
            EnsureColumnHasAllValues("locations")
            EnsureColumnHasAllValues("current_end_user")

            ' Keep PTR TO and CURRENT END-USER filled with employee list
            UpdateComboColumnItems("ptr_to", _employeeNames)
            UpdateComboColumnItems("current_end_user", _employeeNames)

            lblpage.Text = $"{_pageIndex} / {_totalPages}   ({_totalRows:#,0} rows)"
            cmdprev.Enabled = (_pageIndex > 1)
            cmdnext.Enabled = (_pageIndex < _totalPages)

            DataGridView1.Invalidate() ' repaint picture buttons

        Catch ex As Exception
            MessageBox.Show("LoadPage error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub CoerceCheckColumn01(dt As DataTable, colName As String)
        If dt Is Nothing OrElse Not dt.Columns.Contains(colName) Then Exit Sub
        For Each row As DataRow In dt.Rows
            Dim b As Boolean = False
            If Not row.IsNull(colName) Then
                Dim s As String = row(colName).ToString().Trim().ToLowerInvariant()
                b = (s = "1" OrElse s = "y" OrElse s = "yes" OrElse s = "true")
            End If
            row(colName) = b
        Next
    End Sub

    ' ─────────────────────────────────────────────────────────────────────────────
    ' Grid configuration
    ' ─────────────────────────────────────────────────────────────────────────────
    Private Sub ConfigureSEPGrid()
        Dim g = DataGridView1
        With g
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

            ' Rows (base look)
            .DefaultCellStyle.Font = New Font("Segoe UI", 8.0!)
            .DefaultCellStyle.ForeColor = Color.Black
            .RowsDefaultCellStyle.BackColor = _rowBase
            .AlternatingRowsDefaultCellStyle.BackColor = _rowAlt
            .DefaultCellStyle.SelectionBackColor = _selBack
            .DefaultCellStyle.SelectionForeColor = Color.Black
            .RowTemplate.Height = 34
            .RowTemplate.DefaultCellStyle.Padding = New Padding(0, 6, 0, 6)

            ' IMPORTANT: fix space key editing bug
            .EditMode = DataGridViewEditMode.EditOnKeystroke
        End With

        ' start edit if space pressed and not yet in edit mode
        AddHandler DataGridView1.KeyDown, AddressOf DataGridView1_KeyDown_StartEditOnSpace

        EnableDoubleBuffering(g, True)

        ' Columns (ID hidden)
        AddTextCol(g, "id", "ID", "id", 60, visible:=False)

        ' Left/frozen block
        AddTextCol(g, "ics_no", "ICS NO.", "ics_no", 110, ro:=True)
        AddTextCol(g, "ics_date", "ICS DATE", "ics_date", 90, fmt:="MM/dd/yyyy", ro:=True)
        AddTextCol(g, "iar_no", "IAR NO.", "iar_no", 90)
        AddTextCol(g, "ris_no", "RIS NO.", "ris_no", 90)

        AddComboCol(g, "type_of_sep", "TYPE OF SEP", "type_of_sep", New String() {" "}, 220, allowTyping:=True)

        ' DATE PROCURED: read-only (no manual typing) but editable via overlayed DateTimePicker
        AddTextCol(g, "date_procured", "DATE PROCURED", "date_procured", 120, fmt:="yyyy-MM-dd", ro:=True)
        g.Columns("date_procured").ValueType = GetType(Date)

        AddTextCol(g, "descriptions", "DESCRIPTION", "descriptions", 300, ro:=True)
        AddTextCol(g, "property_number", "PROPERTY NUMBER", "property_number", 160, ro:=True)

        ' Scrollable remainder
        AddTextCol(g, "life_span", "LIFE SPAN", "life_span", 90)
        AddTextCol(g, "serial_number", "SERIAL NUMBER", "serial_number", 180)
        AddTextCol(g, "qty", "QTY", "qty", 70, fmt:="#,##0.##", align:=DataGridViewContentAlignment.MiddleCenter, ro:=True)
        AddTextCol(g, "unit", "UNIT", "unit", 80, ro:=True)
        AddTextCol(g, "unit_cost", "UNIT COST", "unit_cost", 120, fmt:="#,##0.00", align:=DataGridViewContentAlignment.MiddleCenter, ro:=True)
        AddTextCol(g, "total_cost", "TOTAL COST", "total_cost", 120, fmt:="#,##0.00", align:=DataGridViewContentAlignment.MiddleCenter, ro:=True)

        ' PAR/MR TO & POSITION (read-only text)
        AddTextCol(g, "par_mr_to", "PAR/MR TO", "par_mr_to", 180, ro:=True)
        AddTextCol(g, "position", "POSITION", "position", 160, ro:=True)

        ' SECTION/DIVISION combo, typed allowed
        AddComboCol(g, "section_division", "SECTION/DIVISION", "section_division",
                    SectionList(), 220, allowTyping:=True)

        ' SUPPLIER NAME combo
        AddComboCol(g, "supplier_name", "SUPPLIER NAME", "supplier_name", _supplierList, 220, allowTyping:=True)

        ' PTR TO and CURRENT END-USER: both use employee list; both allow typing
        ' PTR TO and CURRENT END-USER: both use employee list; both allow typing
        AddComboCol(g, "ptr_to", "PTR TO", "ptr_to", _employeeNames, 180, allowTyping:=True)
        AddComboCol(g, "current_end_user", "CURRENT END-USER", "current_end_user", _employeeNames, 200, allowTyping:=True)



        ' TYPE OF FIXED ASSET
        AddComboCol(g, "type_of_fixed_asset", "TYPE OF FIXED ASSET", "type_of_fixed_asset", Array.Empty(Of String)(), 160, allowTyping:=True)

        AddTextCol(g, "prepared_by", "PREPARED BY", "prepared_by", 160, ro:=True)

        AddCheckCol(g, "ics_docs", "ICS DOCS", "ics_docs", 100)
        AddCheckCol(g, "sticker_tagging", "STICKER TAGGING", "sticker_tagging", 140)

        AddComboCol(g, "conditions", "CONDITION", "conditions",
                    New String() {"", "DAMAGE", "MISSING", "FOR REPAIR", "FOR REPAIR, STILL USING IT", "DEFECTIVE",
                                  "FOR REPLACEMENT", "ALREADY DISPOSED", "IN GOOD CONDITION", "NEEDING REPAIR",
                                  "UNSERVICEABLE", "OBSOLETE", "NO LONGER NEEDED"}, 180, allowTyping:=True)

        AddComboCol(g, "locations", "LOCATION", "locations", Array.Empty(Of String)(), 180, allowTyping:=True)
        AddTextCol(g, "rlqdp_no", "RLSDDP NO.", "rlqdp_no", 120)

        ' MODEL is a plain TextBox column (NOT a ComboBox)
        AddTextCol(g, "model", "MODEL", "model", 150)



        ' --- Picture host (unbound) + hidden has_picture flag (bound) ---
        Dim colPicHost As New DataGridViewTextBoxColumn With {
            .Name = "equipment_picture",
            .HeaderText = "PICTURE OF THE EQUIPMENT W/ PROPERTY STICKER",
            .Width = 360,
            .ReadOnly = True,
            .SortMode = DataGridViewColumnSortMode.NotSortable
        }
        g.Columns.Add(colPicHost)

        Dim colHasPic As New DataGridViewCheckBoxColumn With {
            .Name = "has_picture",
            .HeaderText = "has_picture",
            .DataPropertyName = "has_picture",
            .Visible = False,
            .Width = 10
        }
        g.Columns.Add(colHasPic)

        ' The rest
        AddTextCol(g, "remarks", "REMARKS", "remarks", 220)
        AddTextCol(g, "responsibility_code_center", "RESPONSIBILITY CODE CENTER", "responsibility_code_center", 200)

        ' Numeric text column: allow only digits + one dot; validate/normalize
        AddTextCol(g, "unit_cost_missing_damage", "UNIT COST (MISSING/DAMAGE)", "unit_cost_missing_damage",
                   180, fmt:="#,##0.00", align:=DataGridViewContentAlignment.MiddleCenter)

        AddCheckCol(g, "sep_card", "SEP CARD", "sep_card", 100)

        ' Freeze to DESCRIPTION
        FreezeThrough(g, "descriptions")

        ' Lock visuals
        For Each col As DataGridViewColumn In g.Columns
            If col.ReadOnly Then
                col.DefaultCellStyle.BackColor = Color.FromArgb(245, 246, 252)
                col.DefaultCellStyle.ForeColor = Color.FromArgb(70, 70, 70)
            End If
        Next

        With DataGridView1
            .CellBorderStyle = DataGridViewCellBorderStyle.Single
            .GridColor = Color.FromArgb(109, 148, 165)
        End With

        ' --- DatePicker host for DATE PROCURED ---
        If Not DataGridView1.Controls.Contains(_dtp) Then
            DataGridView1.Controls.Add(_dtp)
        End If

        ' Show/hide/reposition logic for the picker tied to read-only cell
        AddHandler DataGridView1.CellEnter, AddressOf DataGridView1_CellEnter
        AddHandler DataGridView1.CellClick, AddressOf DataGridView1_CellClick
        AddHandler DataGridView1.Scroll, AddressOf Grid_RepositionDatePicker
        AddHandler DataGridView1.ColumnWidthChanged, AddressOf Grid_RepositionDatePicker
        AddHandler DataGridView1.SizeChanged, AddressOf Grid_RepositionDatePicker

        ' Handlers
        AddHandler g.EditingControlShowing, AddressOf DataGridView1_EditingControlShowing
        AddHandler g.RowValidated, AddressOf DataGridView1_RowValidated
        AddHandler g.UserDeletingRow, AddressOf DataGridView1_UserDeletingRow
        AddHandler g.DataError, AddressOf DataGridView1_DataError
        AddHandler g.CellFormatting, AddressOf DataGridView1_CellFormatting
        AddHandler g.CurrentCellDirtyStateChanged, AddressOf DataGridView1_CurrentCellDirtyStateChanged
        AddHandler g.CellValueChanged, AddressOf DataGridView1_CellValueChanged

        ' NEW: strict numeric validation for unit_cost_missing_damage
        AddHandler g.CellValidating, AddressOf DataGridView1_CellValidating_Numeric
        AddHandler g.CellValidated, AddressOf DataGridView1_CellValidated_Numeric

        AddHandler g.CellEndEdit, AddressOf DataGridView1_CellEndEdit
        ' AddHandler _activeCombo.SelectionChangeCommitted, AddressOf Combo_SelectionChangeCommitted



    End Sub
    Private Sub Combo_SelectionChangeCommitted(sender As Object, e As EventArgs)
        If DataGridView1.CurrentCell Is Nothing Then Exit Sub

        Dim cb = TryCast(sender, ComboBox)
        If cb Is Nothing Then Exit Sub

        ' 1) Put the selected item (or text) into the current cell explicitly
        Dim newVal As String = If(TryCast(cb.SelectedItem, String), cb.Text)?.Trim()
        DataGridView1.CurrentCell.Value = newVal

        ' 2) Tell the grid the cell is dirty and commit
        DataGridView1.NotifyCurrentCellDirty(True)
        DataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit)

        ' 3) Finalize binding + save this row
        If _bs IsNot Nothing Then _bs.EndEdit()
        PersistRowAt(DataGridView1.CurrentCell.RowIndex)
    End Sub



    Private Sub DataGridView1_KeyDown_StartEditOnSpace(sender As Object, e As KeyEventArgs)
        If e.KeyCode = Keys.Space Then
            Dim dgv = DirectCast(sender, DataGridView)
            If dgv.CurrentCell IsNot Nothing AndAlso Not dgv.IsCurrentCellInEditMode Then
                dgv.BeginEdit(True)
            End If
        End If
    End Sub

    Private Sub Grid_RowPrePaint(sender As Object, e As DataGridViewRowPrePaintEventArgs)
        Dim g = DirectCast(sender, DataGridView)
        If e.RowIndex < 0 OrElse g.DataSource Is Nothing Then Return

        Dim row = g.Rows(e.RowIndex)
        Dim cond As String = If(TryCast(row.Cells("conditions").Value, String), "").ToUpperInvariant().Trim()

        row.DefaultCellStyle.BackColor = _rowBase
        row.DefaultCellStyle.ForeColor = Color.Black
        row.DefaultCellStyle.SelectionBackColor = _selBack
        row.DefaultCellStyle.SelectionForeColor = Color.Black

        Select Case cond
            Case "DAMAGE" : row.DefaultCellStyle.BackColor = Color.Salmon
            Case "MISSING" : row.DefaultCellStyle.BackColor = Color.Gold
            Case "FOR REPAIR"
                row.DefaultCellStyle.BackColor = Color.SteelBlue
                row.DefaultCellStyle.ForeColor = Color.White
                row.DefaultCellStyle.SelectionForeColor = Color.White
        End Select
    End Sub

    Private Sub DataGridView1_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs)
        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Exit Sub
        Dim dgv = DirectCast(sender, DataGridView)
        Dim col = dgv.Columns(e.ColumnIndex)
        Dim colName = col.Name

        ' 1) Your existing Section → RCC mapping
        If String.Equals(colName, "section_division", StringComparison.OrdinalIgnoreCase) Then
            Dim sec As String = If(TryCast(dgv.Rows(e.RowIndex).Cells("section_division").Value, String), "").Trim()
            If sec <> "" Then
                Dim code As String = Nothing
                If _sectionToRcc.TryGetValue(sec, code) Then
                    dgv.Rows(e.RowIndex).Cells("responsibility_code_center").Value = code
                    Dim view As DataRowView = TryCast(dgv.Rows(e.RowIndex).DataBoundItem, DataRowView)
                    If view IsNot Nothing Then view.Row("responsibility_code_center") = code
                End If
            End If
        End If

        ' 2) For ANY editable column (not ReadOnly), persist immediately
        If Not col.ReadOnly Then
            PersistRowAt(e.RowIndex)
        End If
    End Sub


    Private Sub EnableDoubleBuffering(dgv As DataGridView, enable As Boolean)
        Dim pi = GetType(DataGridView).GetProperty("DoubleBuffered", Reflection.BindingFlags.Instance Or Reflection.BindingFlags.NonPublic)
        If pi IsNot Nothing Then pi.SetValue(dgv, enable, Nothing)
    End Sub

    ' Add column helpers
    Private Sub AddTextCol(dgv As DataGridView, name As String, header As String, prop As String,
                           Optional width As Integer = 140,
                           Optional fmt As String = Nothing,
                           Optional align As DataGridViewContentAlignment = DataGridViewContentAlignment.MiddleLeft,
                           Optional visible As Boolean = True,
                           Optional ro As Boolean = False)
        Dim c As New DataGridViewTextBoxColumn()
        c.Name = name : c.HeaderText = header : c.DataPropertyName = prop
        c.Width = width : c.Visible = visible : c.ReadOnly = ro
        c.DefaultCellStyle.Alignment = align
        If Not String.IsNullOrEmpty(fmt) Then c.DefaultCellStyle.Format = fmt
        dgv.Columns.Add(c)
    End Sub

    Private Sub AddComboCol(dgv As DataGridView, name As String, header As String, prop As String,
                            items As IEnumerable(Of String),
                            Optional width As Integer = 160,
                            Optional allowTyping As Boolean = False,
                            Optional ro As Boolean = False)
        Dim c As New DataGridViewComboBoxColumn()
        c.Name = name : c.HeaderText = header : c.DataPropertyName = prop : c.Width = width
        c.FlatStyle = FlatStyle.Flat
        c.DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton
        c.Sorted = True
        c.Items.AddRange(items.ToArray())
        c.DisplayStyleForCurrentCellOnly = True
        c.AutoComplete = True
        c.ReadOnly = ro
        c.ValueType = GetType(String)
        dgv.Columns.Add(c)
        If allowTyping Then _freeTypeComboCols.Add(name)
    End Sub

    Private Sub AddCheckCol(dgv As DataGridView, name As String, header As String, prop As String,
                            Optional width As Integer = 120)
        Dim c As New DataGridViewCheckBoxColumn()
        c.Name = name : c.HeaderText = header : c.DataPropertyName = prop : c.Width = width
        c.TrueValue = True : c.FalseValue = False : c.IndeterminateValue = False
        dgv.Columns.Add(c)
    End Sub

    Private Sub FreezeThrough(dgv As DataGridView, colName As String)
        Dim idx As Integer = dgv.Columns(colName).Index
        For i As Integer = 0 To idx
            dgv.Columns(i).Frozen = True
        Next
    End Sub

    ' ─────────────────────────────────────────────────────────────────────────────
    ' DataAdapter (NO blob column in adapter SQL)
    ' ─────────────────────────────────────────────────────────────────────────────
    Private Sub BuildAdapter()
        Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
            conn.Open()

            Dim selectSql As String =
                "SELECT id, ics_no, ics_date, iar_no, ris_no, type_of_sep, date_procured, descriptions, property_number, life_span, " &
                "serial_number, qty, unit, unit_cost, total_cost, par_mr_to, position, section_division, supplier_name, ptr_to, " &
                "type_of_fixed_asset, prepared_by, ics_docs, sticker_tagging, conditions, locations, rlqdp_no, model, remarks, " &
                "responsibility_code_center, current_end_user, unit_cost_missing_damage, sep_card " &
                "FROM TBL_SEP WITH (UPDLOCK, ROWLOCK);"

            _sepAdapter = New SqlDataAdapter(selectSql, conn)

            ' INSERT
            Dim insertSql As String =
                "INSERT INTO TBL_SEP (ics_no, ics_date, iar_no, ris_no, type_of_sep, date_procured, descriptions, property_number, life_span, " &
                "serial_number, qty, unit, unit_cost, total_cost, par_mr_to, position, section_division, supplier_name, ptr_to, " &
                "type_of_fixed_asset, prepared_by, ics_docs, sticker_tagging, conditions, locations, rlqdp_no, model, remarks, " &
                "responsibility_code_center, current_end_user, unit_cost_missing_damage, sep_card) " &
                "VALUES (@ics_no, @ics_date, @iar_no, @ris_no, @type_of_sep, @date_procured, @descriptions, @property_number, @life_span, " &
                "@serial_number, @qty, @unit, @unit_cost, @total_cost, @par_mr_to, @position, @section_division, @supplier_name, @ptr_to, " &
                "@type_of_fixed_asset, @prepared_by, @ics_docs, @sticker_tagging, @conditions, @locations, @rlqdp_no, @model, @remarks, " &
                "@responsibility_code_center, @current_end_user, @unit_cost_missing_damage, @sep_card); " &
                "SET @id = SCOPE_IDENTITY();"

            Dim insertCmd As New SqlCommand(insertSql, conn)
            AddAllParams(insertCmd)
            Dim idParamInsert = insertCmd.Parameters.Add("@id", SqlDbType.Int)
            idParamInsert.Direction = ParameterDirection.Output
            _sepAdapter.InsertCommand = insertCmd
            AddHandler _sepAdapter.RowUpdating, AddressOf SepAdapter_RowUpdating

            ' UPDATE
            Dim updateSql As String =
                "UPDATE TBL_SEP SET ics_no=@ics_no, ics_date=@ics_date, iar_no=@iar_no, ris_no=@ris_no, type_of_sep=@type_of_sep, date_procured=@date_procured, " &
                "descriptions=@descriptions, property_number=@property_number, life_span=@life_span, serial_number=@serial_number, qty=@qty, unit=@unit, " &
                "unit_cost=@unit_cost, total_cost=@total_cost, par_mr_to=@par_mr_to, position=@position, section_division=@section_division, supplier_name=@supplier_name, " &
                "ptr_to=@ptr_to, type_of_fixed_asset=@type_of_fixed_asset, prepared_by=@prepared_by, ics_docs=@ics_docs, sticker_tagging=@sticker_tagging, " &
                "conditions=@conditions, locations=@locations, rlqdp_no=@rlqdp_no, model=@model, remarks=@remarks, " &
                "responsibility_code_center=@responsibility_code_center, current_end_user=@current_end_user, unit_cost_missing_damage=@unit_cost_missing_damage, sep_card=@sep_card " &
                "WHERE id=@id;"

            Dim updateCmd As New SqlCommand(updateSql, conn)
            AddAllParams(updateCmd)
            updateCmd.Parameters.Add("@id", SqlDbType.Int, 4, "id")
            _sepAdapter.UpdateCommand = updateCmd
            AddHandler _sepAdapter.RowUpdating, AddressOf SepAdapter_RowUpdating

            ' DELETE
            Dim deleteCmd As New SqlCommand("DELETE FROM TBL_SEP WHERE id=@id;", conn)
            deleteCmd.Parameters.Add("@id", SqlDbType.Int, 4, "id")
            _sepAdapter.DeleteCommand = deleteCmd
        End Using
    End Sub

    Private Sub AddAllParams(cmd As SqlCommand)
        cmd.Parameters.Add("@ics_no", SqlDbType.VarChar, 50, "ics_no")
        cmd.Parameters.Add("@ics_date", SqlDbType.Date, 0, "ics_date")
        cmd.Parameters.Add("@iar_no", SqlDbType.VarChar, 50, "iar_no")
        cmd.Parameters.Add("@ris_no", SqlDbType.VarChar, 50, "ris_no")
        cmd.Parameters.Add("@type_of_sep", SqlDbType.VarChar, 200, "type_of_sep")
        cmd.Parameters.Add("@date_procured", SqlDbType.Date, 0, "date_procured")
        cmd.Parameters.Add("@descriptions", SqlDbType.VarChar, 3500, "descriptions")
        cmd.Parameters.Add("@property_number", SqlDbType.VarChar, 100, "property_number")
        cmd.Parameters.Add("@life_span", SqlDbType.VarChar, 150, "life_span")
        cmd.Parameters.Add("@serial_number", SqlDbType.VarChar, 150, "serial_number")
        cmd.Parameters.Add("@qty", SqlDbType.Decimal, 0, "qty").Precision = 18 : cmd.Parameters("@qty").Scale = 2
        cmd.Parameters.Add("@unit", SqlDbType.VarChar, 50, "unit")
        cmd.Parameters.Add("@unit_cost", SqlDbType.Decimal, 0, "unit_cost").Precision = 18 : cmd.Parameters("@unit_cost").Scale = 2
        cmd.Parameters.Add("@total_cost", SqlDbType.Decimal, 0, "total_cost").Precision = 18 : cmd.Parameters("@total_cost").Scale = 2
        cmd.Parameters.Add("@par_mr_to", SqlDbType.VarChar, 150, "par_mr_to")
        cmd.Parameters.Add("@position", SqlDbType.VarChar, 150, "position")
        cmd.Parameters.Add("@section_division", SqlDbType.VarChar, 200, "section_division")
        cmd.Parameters.Add("@supplier_name", SqlDbType.VarChar, 150, "supplier_name")
        cmd.Parameters.Add("@ptr_to", SqlDbType.VarChar, 150, "ptr_to")
        cmd.Parameters.Add("@type_of_fixed_asset", SqlDbType.VarChar, 100, "type_of_fixed_asset")
        cmd.Parameters.Add("@prepared_by", SqlDbType.VarChar, 150, "prepared_by")
        cmd.Parameters.Add("@ics_docs", SqlDbType.VarChar, 10, "ics_docs")               ' stored as "1"/"0"
        cmd.Parameters.Add("@sticker_tagging", SqlDbType.VarChar, 10, "sticker_tagging") ' stored as "1"/"0"
        cmd.Parameters.Add("@conditions", SqlDbType.VarChar, 50, "conditions")
        cmd.Parameters.Add("@locations", SqlDbType.VarChar, 150, "locations")
        cmd.Parameters.Add("@rlqdp_no", SqlDbType.VarChar, 50, "rlqdp_no")
        cmd.Parameters.Add("@model", SqlDbType.VarChar, 100, "model")
        cmd.Parameters.Add("@remarks", SqlDbType.VarChar, 255, "remarks")
        cmd.Parameters.Add("@responsibility_code_center", SqlDbType.VarChar, 100, "responsibility_code_center")
        cmd.Parameters.Add("@current_end_user", SqlDbType.VarChar, 150, "current_end_user")
        cmd.Parameters.Add("@unit_cost_missing_damage", SqlDbType.Decimal, 0, "unit_cost_missing_damage").Precision = 18 : cmd.Parameters("@unit_cost_missing_damage").Scale = 2
        cmd.Parameters.Add("@sep_card", SqlDbType.VarChar, 10, "sep_card")
    End Sub

    Private Sub SepAdapter_RowUpdating(sender As Object, e As SqlRowUpdatingEventArgs)
        ' Convert checkbox Booleans to "1"/"0" strings for varchar columns
        Dim f = Function(v As Object) As String
                    If v Is Nothing OrElse v Is DBNull.Value Then Return "0"
                    If TypeOf v Is Boolean Then Return If(CBool(v), "1", "0")
                    Dim s = v.ToString().Trim().ToLowerInvariant()
                    Return If(s = "1" OrElse s = "true" OrElse s = "y" OrElse s = "yes", "1", "0")
                End Function
        If e.Command.Parameters.Contains("@ics_docs") Then e.Command.Parameters("@ics_docs").Value = f(e.Row("ics_docs"))
        If e.Command.Parameters.Contains("@sticker_tagging") Then e.Command.Parameters("@sticker_tagging").Value = f(e.Row("sticker_tagging"))
        If e.Command.Parameters.Contains("@sep_card") Then e.Command.Parameters("@sep_card").Value = f(e.Row("sep_card"))
    End Sub

    Private Sub DataGridView1_CurrentCellDirtyStateChanged(sender As Object, e As EventArgs)
        Dim dgv = DirectCast(sender, DataGridView)
        If dgv.CurrentCell Is Nothing OrElse Not dgv.IsCurrentCellDirty Then Exit Sub

        Dim col = dgv.Columns(dgv.CurrentCell.ColumnIndex)

        ' ✅ Only auto-commit for checkbox & combobox.
        If TypeOf col Is DataGridViewCheckBoxColumn OrElse TypeOf col Is DataGridViewComboBoxColumn Then
            dgv.CommitEdit(DataGridViewDataErrorContexts.Commit)
            Dim r As Integer = dgv.CurrentCell.RowIndex
            BeginInvoke(Sub()
                            If _bs IsNot Nothing Then _bs.EndEdit()
                            PersistRowAt(r)
                        End Sub)
        End If

        ' ❌ Do NOT commit here for text box columns.
    End Sub


    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs)
        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Exit Sub
        Dim dgv = DirectCast(sender, DataGridView)
        Dim col = dgv.Columns(e.ColumnIndex)

        If TypeOf col Is DataGridViewCheckBoxColumn Then
            ' Commit immediately and persist
            dgv.CommitEdit(DataGridViewDataErrorContexts.Commit)
            If _bs IsNot Nothing Then _bs.EndEdit()
            PersistRowAt(e.RowIndex)
        End If
    End Sub



    Private Sub DataGridView1_RowValidated(sender As Object, e As DataGridViewCellEventArgs)
        If DataGridView1.DataSource Is Nothing OrElse e.RowIndex < 0 Then Return
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
                _sepAdapter.InsertCommand.Connection = conn
                _sepAdapter.UpdateCommand.Connection = conn
                _sepAdapter.DeleteCommand.Connection = conn

                Dim tmp As DataTable = row.Table.Clone()
                tmp.ImportRow(row)
                _sepAdapter.Update(tmp)

                If row.RowState = DataRowState.Added Then
                    row("id") = tmp.Rows(0)("id")
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
                _sepAdapter.InsertCommand.Connection = conn
                _sepAdapter.UpdateCommand.Connection = conn
                _sepAdapter.DeleteCommand.Connection = conn
                _sepAdapter.Update(_sepTable)
                _sepTable.AcceptChanges()
            End Using
        Catch ex As Exception
            MessageBox.Show("Save error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ─────────────────────────────────────────────────────────────────────────────
    ' Search/filters/paging
    ' ─────────────────────────────────────────────────────────────────────────────
    Private Sub cmdsearch_Click(sender As Object, e As EventArgs) Handles cmdsearch.Click
        ResetAndSearch()
    End Sub
    Private Sub txtsearch_KeyDown(sender As Object, e As KeyEventArgs) Handles txtsearch.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.Handled = True : e.SuppressKeyPress = True
            ResetAndSearch()
        End If
    End Sub
    Private Sub Combosep_SelectedIndexChanged(sender As Object, e As EventArgs) Handles Combosep.SelectedIndexChanged
        If _suppressFilterEvents Then Return
        ResetAndSearch()
    End Sub
    Private Sub Combooffice_SelectedIndexChanged(sender As Object, e As EventArgs) Handles Combooffice.SelectedIndexChanged
        If _suppressFilterEvents Then Return
        ResetAndSearch()
    End Sub
    Private Sub Comboyear_SelectedIndexChanged(sender As Object, e As EventArgs) Handles Comboyear.SelectedIndexChanged
        If _suppressFilterEvents Then Return
        ResetAndSearch()
    End Sub

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

    Private Sub cmdrefresh_Click(sender As Object, e As EventArgs) Handles cmdrefresh.Click
        Try
            If DataGridView1.IsCurrentCellDirty Then DataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit)
            DataGridView1.EndEdit()
            If _sepTable IsNot Nothing AndAlso _sepTable.GetChanges() IsNot Nothing Then SaveChanges()

            ' reload lookups
            LoadLookups()

            ' refresh combo lists
            UpdateComboColumnItems("supplier_name", _supplierList)
            UpdateComboColumnItems("ptr_to", _employeeNames)
            UpdateComboColumnItems("current_end_user", _employeeNames)

            LoadPage()
            DataGridView1.AllowUserToAddRows = False
        Catch ex As Exception
            MessageBox.Show("Refresh error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ─────────────────────────────────────────────────────────────────────────────
    ' Combo typing & DataError suppression
    ' ─────────────────────────────────────────────────────────────────────────────
    Private Sub DataGridView1_EditingControlShowing(sender As Object, e As DataGridViewEditingControlShowingEventArgs)
        ' Current column name (safe even if no current cell)
        Dim curColName As String =
        If(DataGridView1.CurrentCell Is Nothing,
           "",
           DataGridView1.Columns(DataGridView1.CurrentCell.ColumnIndex).Name)

        ' ── Detach previous TextBox numeric filter ─────────────────
        If _activeTextBox IsNot Nothing Then
            RemoveHandler _activeTextBox.KeyPress, AddressOf NumericTextBox_KeyPress
            _activeTextBox = Nothing
        End If

        ' If somehow a TextBox appears for date_procured, make it readonly
        If curColName = "date_procured" Then
            Dim tb = TryCast(e.Control, TextBox)
            If tb IsNot Nothing Then
                tb.ReadOnly = True
                tb.Cursor = Cursors.Default
            End If
        End If

        ' Numeric typing guard for unit_cost_missing_damage
        If curColName = "unit_cost_missing_damage" Then
            Dim tb = TryCast(e.Control, TextBox)
            If tb IsNot Nothing Then
                _activeTextBox = tb
                AddHandler _activeTextBox.KeyPress, AddressOf NumericTextBox_KeyPress
            End If
        End If

        ' ── Combo editor setup ─────────────────────────────────────
        Dim cb = TryCast(e.Control, DataGridViewComboBoxEditingControl)
        If cb Is Nothing Then
            ' Not a combo editor in this cell
            _activeCombo = Nothing
            Return
        End If

        ' Detach handlers from the previous combo (if any)
        If _activeCombo IsNot Nothing Then
            RemoveHandler _activeCombo.SelectionChangeCommitted, AddressOf Combo_SelectionChangeCommitted
            RemoveHandler _activeCombo.Validated, AddressOf Combo_Validated
            RemoveHandler _activeCombo.KeyDown, AddressOf Combo_KeyDown
        End If

        ' Track the new combo
        _activeCombo = cb

        ' Allow free-typing only for whitelisted columns
        Dim freeType As Boolean = _freeTypeComboCols.Contains(curColName)
        _activeCombo.DropDownStyle = If(freeType, ComboBoxStyle.DropDown, ComboBoxStyle.DropDownList)

        ' Attach handlers for instant-save behavior
        AddHandler _activeCombo.SelectionChangeCommitted, AddressOf Combo_SelectionChangeCommitted
        AddHandler _activeCombo.Validated, AddressOf Combo_Validated
        AddHandler _activeCombo.KeyDown, AddressOf Combo_KeyDown
    End Sub

    ' Allow only digits, one decimal point, and control keys
    Private Sub NumericTextBox_KeyPress(sender As Object, e As KeyPressEventArgs)
        Dim tb As TextBox = DirectCast(sender, TextBox)

        ' Allow control keys (backspace, delete, arrows, etc.)
        If Char.IsControl(e.KeyChar) Then Return

        ' Allow digits
        If Char.IsDigit(e.KeyChar) Then Return

        ' Allow one decimal point (if not already in text or being replaced)
        If e.KeyChar = "."c Then
            If tb.Text.Contains(".") AndAlso Not tb.SelectedText.Contains(".") Then
                e.Handled = True
            End If
            Return
        End If

        ' Block everything else (letters, commas, spaces, etc.)
        e.Handled = True
    End Sub


    Private Sub Combo_KeyDown(sender As Object, e As KeyEventArgs)
        If e.KeyCode = Keys.Enter Then
            e.Handled = True : e.SuppressKeyPress = True
            CommitComboEdit()
        End If
    End Sub

    Private Sub Combo_Validated(sender As Object, e As EventArgs)
        CommitComboEdit()
    End Sub

    Private Sub CommitComboEdit()
        If DataGridView1.CurrentCell Is Nothing OrElse _activeCombo Is Nothing Then Return
        Dim colName = DataGridView1.Columns(DataGridView1.CurrentCell.ColumnIndex).Name
        If Not _freeTypeComboCols.Contains(colName) Then Return

        Dim newText As String = If(_activeCombo.Text, String.Empty).Trim()
        If newText = "" Then Return

        Dim col = TryCast(DataGridView1.Columns(colName), DataGridViewComboBoxColumn)
        If col IsNot Nothing Then
            Dim exists = col.Items.Cast(Of Object)().
                Any(Function(x) String.Equals(CStr(x), newText, StringComparison.OrdinalIgnoreCase))
            If Not exists Then col.Items.Add(newText)
        End If

        DataGridView1.CurrentCell.Value = newText
        DataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit)
        DataGridView1.EndEdit()

        Dim view As DataRowView = TryCast(DataGridView1.CurrentRow?.DataBoundItem, DataRowView)
        If view IsNot Nothing Then
            Dim row As DataRow = view.Row
            row(colName) = newText

            If String.Equals(colName, "section_division", StringComparison.OrdinalIgnoreCase) Then
                Dim code As String = Nothing
                If _sectionToRcc.TryGetValue(newText, code) Then
                    DataGridView1.CurrentRow.Cells("responsibility_code_center").Value = code
                    row("responsibility_code_center") = code
                End If
            End If

            SaveRow(row)
        End If
    End Sub

    Private Sub DataGridView1_DataError(sender As Object, e As DataGridViewDataErrorEventArgs)
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
        e.Cancel = False
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
        EnsureColumnHasAllValues(colName)
    End Sub

    Private Sub EnsureColumnHasAllValues(colName As String)
        If Not DataGridView1.Columns.Contains(colName) Then Exit Sub
        Dim col = TryCast(DataGridView1.Columns(colName), DataGridViewComboBoxColumn)
        If col Is Nothing Then Exit Sub

        Dim setItems As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
        For Each it In col.Items : setItems.Add(If(it, "").ToString()) : Next

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

    ' ─────────────────────────────────────────────────────────────────────────────
    ' Column palette + Condition-based row color
    ' ─────────────────────────────────────────────────────────────────────────────
    Private Sub ApplySEPColumnColors(g As DataGridView)
        _colPalette = New Dictionary(Of String, Color)(StringComparer.OrdinalIgnoreCase) From {
            {"ics_no", Color.FromArgb(218, 205, 222)},
            {"ics_date", Color.FromArgb(218, 205, 222)},
            {"iar_no", Color.FromArgb(218, 205, 222)},
            {"ris_no", Color.FromArgb(218, 205, 222)},
            {"type_of_sep", Color.FromArgb(210, 203, 222)},
            {"date_procured", Color.FromArgb(244, 217, 217)},
            {"descriptions", Color.FromArgb(246, 225, 215)},
            {"property_number", Color.FromArgb(250, 234, 208)},
            {"life_span", Color.FromArgb(216, 232, 243)},
            {"serial_number", Color.FromArgb(244, 219, 223)},
            {"qty", Color.FromArgb(211, 235, 239)},
            {"unit", Color.FromArgb(214, 238, 243)},
            {"unit_cost", Color.FromArgb(251, 234, 210)},
            {"total_cost", Color.FromArgb(253, 227, 196)},
            {"par_mr_to", Color.FromArgb(239, 208, 217)},
            {"position", Color.FromArgb(201, 185, 200)},
            {"section_division", Color.FromArgb(220, 238, 241)},
            {"supplier_name", Color.FromArgb(212, 233, 238)},
            {"ptr_to", Color.FromArgb(200, 190, 208)},
            {"type_of_fixed_asset", Color.FromArgb(210, 203, 222)},
            {"prepared_by", Color.FromArgb(224, 224, 232)},
            {"ics_docs", Color.FromArgb(251, 224, 232)},
            {"sticker_tagging", Color.FromArgb(251, 224, 232)},
            {"conditions", Color.FromArgb(215, 204, 219)},
            {"locations", Color.FromArgb(229, 229, 235)},
            {"rlqdp_no", Color.FromArgb(229, 229, 235)},
            {"model", Color.FromArgb(209, 196, 218)},
            {"equipment_picture", Color.FromArgb(214, 203, 221)},
            {"remarks", Color.FromArgb(215, 204, 219)},
            {"responsibility_code_center", Color.FromArgb(224, 224, 232)},
            {"current_end_user", Color.FromArgb(229, 229, 235)},
            {"unit_cost_missing_damage", Color.FromArgb(251, 234, 210)},
            {"sep_card", Color.FromArgb(251, 224, 232)}
        }
        If g IsNot Nothing Then g.Invalidate()
    End Sub

    Private Sub DataGridView1_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs)
        If e.RowIndex < 0 Then Return
        Dim dgv = DirectCast(sender, DataGridView)
        Dim colName = dgv.Columns(e.ColumnIndex).Name

        ' Row color by Condition (overrides palette)
        Dim cond As String = ""
        Dim cv = dgv.Rows(e.RowIndex).Cells("conditions").Value
        If cv IsNot Nothing AndAlso cv IsNot DBNull.Value Then cond = cv.ToString().Trim().ToUpperInvariant()

        If cond = "DAMAGE" Then
            e.CellStyle.BackColor = Color.Salmon
            e.CellStyle.ForeColor = Color.Black
            Return
        ElseIf cond = "MISSING" Then
            e.CellStyle.BackColor = Color.Gold
            e.CellStyle.ForeColor = Color.Black
            Return
        ElseIf cond = "FOR REPAIR" Then
            e.CellStyle.BackColor = Color.SteelBlue
            e.CellStyle.ForeColor = Color.White
            Return
        ElseIf colName = "ics_date" OrElse colName = "date_procured" Then
            Dim dt As Date
            If Date.TryParse(e.Value?.ToString(), dt) Then
                e.Value = dt.ToString("yyyy-MM-dd")
                e.FormattingApplied = True
            End If
        End If

        ' Otherwise, use per-column palette
        If _colPalette IsNot Nothing AndAlso _colPalette.ContainsKey(colName) Then
            e.CellStyle.BackColor = _colPalette(colName)
        End If
    End Sub

    ' ─────────────────────────────────────────────────────────────────────────────
    ' Numeric validation hooks for unit_cost_missing_damage
    ' ─────────────────────────────────────────────────────────────────────────────
    Private Sub DataGridView1_CellValidating_Numeric(sender As Object, e As DataGridViewCellValidatingEventArgs)
        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Exit Sub
        Dim dgv = DirectCast(sender, DataGridView)
        Dim colName = dgv.Columns(e.ColumnIndex).Name
        If colName <> "unit_cost_missing_damage" Then Exit Sub

        Dim s As String = If(e.FormattedValue, "").ToString().Trim()

        ' Allow blank
        If s = "" Then
            Return
        End If

        ' Must be a valid double with '.' decimal
        Dim val As Double
        If Not Double.TryParse(s, NumberStyles.AllowDecimalPoint Or NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, val) Then
            e.Cancel = True
            MessageBox.Show("Please enter a valid number (digits and optional '.' decimal).", "Invalid value", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    Private Sub DataGridView1_CellValidated_Numeric(sender As Object, e As DataGridViewCellEventArgs)
        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Exit Sub
        Dim dgv = DirectCast(sender, DataGridView)
        Dim colName = dgv.Columns(e.ColumnIndex).Name
        If colName <> "unit_cost_missing_damage" Then Exit Sub

        Dim cell = dgv.Rows(e.RowIndex).Cells(e.ColumnIndex)
        Dim s As String = If(cell.Value, "").ToString().Trim()
        If s = "" Then
            cell.Value = DBNull.Value
            Exit Sub
        End If

        Dim val As Double
        If Double.TryParse(s, NumberStyles.AllowDecimalPoint Or NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, val) Then
            ' Normalize to decimal with 2 places
            cell.Value = Math.Round(val, 2)
        Else
            ' If somehow invalid got through, clear it
            cell.Value = DBNull.Value
        End If
    End Sub

    ' ─────────────────────────────────────────────────────────────────────────────
    ' Picture buttons (drawn inside "equipment_picture" column) — BLOB version
    ' ─────────────────────────────────────────────────────────────────────────────
    Private Sub DataGridView1_CellPainting(sender As Object, e As DataGridViewCellPaintingEventArgs) _
        Handles DataGridView1.CellPainting

        If e.RowIndex < 0 Then Return
        Dim dgv = DirectCast(sender, DataGridView)
        If dgv.Columns(e.ColumnIndex).Name <> "equipment_picture" Then Return

        If _colPalette IsNot Nothing AndAlso _colPalette.ContainsKey("equipment_picture") Then
            e.CellStyle.BackColor = _colPalette("equipment_picture")
        End If

        e.Handled = True
        e.Paint(e.CellBounds, DataGridViewPaintParts.Background Or DataGridViewPaintParts.Border)

        Dim hasPic As Boolean = False
        If dgv.Columns.Contains("has_picture") Then
            Dim v = dgv.Rows(e.RowIndex).Cells("has_picture").Value
            If v IsNot Nothing AndAlso v IsNot DBNull.Value Then hasPic = Convert.ToBoolean(v)
        End If

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

    Private Async Sub DataGridView1_CellMouseClick(sender As Object, e As DataGridViewCellMouseEventArgs) _
        Handles DataGridView1.CellMouseClick

        If e.RowIndex < 0 OrElse e.Button <> MouseButtons.Left Then Return
        Dim dgv = DirectCast(sender, DataGridView)
        If dgv.Columns(e.ColumnIndex).Name <> "equipment_picture" Then Return

        Dim idObj = dgv.Rows(e.RowIndex).Cells("id").Value
        If idObj Is Nothing OrElse idObj Is DBNull.Value Then Return
        Dim id As Integer = CInt(idObj)

        Dim hasPic As Boolean = False
        If dgv.Columns.Contains("has_picture") Then
            Dim v = dgv.Rows(e.RowIndex).Cells("has_picture").Value
            If v IsNot Nothing AndAlso v IsNot DBNull.Value Then hasPic = Convert.ToBoolean(v)
        End If

        Dim ptRel As Point = e.Location
        Dim cellSize As Size = dgv.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, True).Size
        Dim rel = GetPicButtonRectsRelative(cellSize, hasPic)

        If hasPic Then
            If rel.R1.Contains(ptRel) Then
                ' View
                Dim bytes = Await LoadSEPImageBytesAsync(id)
                If bytes Is Nothing OrElse bytes.Length = 0 Then
                    MessageBox.Show("No image stored for this row.", "Image", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Return
                End If
                Dim tmp = Path.Combine(Path.GetTempPath(), $"sep_{id}.jpg")
                File.WriteAllBytes(tmp, bytes)
                Process.Start(New ProcessStartInfo(tmp) With {.UseShellExecute = True})

            ElseIf rel.R2.HasValue AndAlso rel.R2.Value.Contains(ptRel) Then
                ' Replace
                If Await PromptAndSaveImageAsync(id, dgv.Rows(e.RowIndex)) Then
                    dgv.Rows(e.RowIndex).Cells("has_picture").Value = True
                    dgv.InvalidateRow(e.RowIndex)
                End If

            ElseIf rel.R3.HasValue AndAlso rel.R3.Value.Contains(ptRel) Then
                ' Remove
                If MessageBox.Show("Remove the stored image for this row?", "Remove Image",
                                   MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                    Await UpdateSEPImageAsync(id, Nothing)  ' NULL blob
                    dgv.Rows(e.RowIndex).Cells("has_picture").Value = False
                    dgv.InvalidateRow(e.RowIndex)
                End If
            End If
        Else
            If rel.R1.Contains(ptRel) Then
                If Await PromptAndSaveImageAsync(id, dgv.Rows(e.RowIndex)) Then
                    dgv.Rows(e.RowIndex).Cells("has_picture").Value = True
                    dgv.InvalidateRow(e.RowIndex)
                End If
            End If
        End If
    End Sub

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

        Dim ptRel As Point = e.Location
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
        If DataGridView1.Columns.Contains("equipment_picture") Then
            If e.ScrollOrientation = ScrollOrientation.VerticalScroll _
           OrElse e.ScrollOrientation = ScrollOrientation.HorizontalScroll Then
                DataGridView1.InvalidateColumn(DataGridView1.Columns("equipment_picture").Index)
            End If
        End If
    End Sub

    ' ─────────────────────────────────────────────────────────────────────────────
    ' Image I/O (blob; resize + compress before save)
    ' ─────────────────────────────────────────────────────────────────────────────
    Private Async Function LoadSEPImageBytesAsync(id As Integer) As Task(Of Byte())
        Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
            Await conn.OpenAsync()
            Using cmd As New SqlCommand("SELECT equipment_picture FROM TBL_SEP WITH (NOLOCK) WHERE id=@id;", conn)
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = id
                Dim obj = Await cmd.ExecuteScalarAsync()
                If obj Is Nothing OrElse obj Is DBNull.Value Then Return Nothing
                Return CType(obj, Byte())
            End Using
        End Using
    End Function

    Private Async Function UpdateSEPImageAsync(id As Integer, jpegBytes As Byte()) As Task
        Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
            Await conn.OpenAsync()
            Using cmd As New SqlCommand("UPDATE TBL_SEP SET equipment_picture=@img WHERE id=@id;", conn)
                Dim p = cmd.Parameters.Add("@img", SqlDbType.VarBinary, If(jpegBytes Is Nothing, 0, jpegBytes.Length))
                p.Value = If(jpegBytes, CType(DBNull.Value, Object))
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

    Private Function ResizeImage(src As Image, maxWidth As Integer, maxHeight As Integer) As Image
        Dim ratioX As Double = maxWidth / src.Width
        Dim ratioY As Double = maxHeight / src.Height
        Dim ratio As Double = Math.Min(ratioX, ratioY)
        If ratio >= 1 Then Return New Bitmap(src)
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
                If sizeKB >= targetMinKB OrElse q = 10 Then Exit For
            End If
        Next
        Return best
    End Function

    Private Async Function PromptAndSaveImageAsync(id As Integer, row As DataGridViewRow) As Task(Of Boolean)
        Using ofd As New OpenFileDialog()
            ofd.Title = "Select an image"
            ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif|All Files|*.*"
            ofd.RestoreDirectory = True
            If ofd.ShowDialog(Me) = DialogResult.OK Then
                Using src As Image = Image.FromFile(ofd.FileName)
                    Using resized As Image = ResizeImage(src, 1024, 768)
                        Dim jpegBytes = CompressToTargetSize(resized, 40, 70) ' ~40–70 KB
                        Await UpdateSEPImageAsync(id, jpegBytes)
                        row.Cells("has_picture").Value = True
                        Return True
                    End Using
                End Using
            End If
        End Using
        Return False
    End Function

    ' ─────────────────────────────────────────────────────────────────────────────
    ' Utility
    ' ─────────────────────────────────────────────────────────────────────────────
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

    ' ─────────────────────────────────────────────────────────────────────────────
    ' Date Picker plumbing (read-only DATE PROCURED column)
    ' ─────────────────────────────────────────────────────────────────────────────
    Private Sub DataGridView1_CellEnter(sender As Object, e As DataGridViewCellEventArgs)
        Dim g = DirectCast(sender, DataGridView)
        If e.RowIndex >= 0 AndAlso g.Columns(e.ColumnIndex).Name = "date_procured" Then
            ShowDatePickerAtCell(e.RowIndex, e.ColumnIndex)
        Else
            _dtp.Visible = False
            _dtpCell = (-1, -1)
        End If
    End Sub

    Private Sub DataGridView1_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs)
        PersistRowAt(e.RowIndex)
    End Sub


    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs)
        Dim g = DirectCast(sender, DataGridView)
        If e.RowIndex >= 0 AndAlso e.ColumnIndex >= 0 AndAlso g.Columns(e.ColumnIndex).Name = "date_procured" Then
            ShowDatePickerAtCell(e.RowIndex, e.ColumnIndex)
        End If
    End Sub

    Private Sub ShowDatePickerAtCell(rowIndex As Integer, colIndex As Integer)
        _dtp.Font = DataGridView1.DefaultCellStyle.Font
        _dtp.Margin = Padding.Empty
        _dtpCell = (rowIndex, colIndex)

        Dim v = DataGridView1.Rows(rowIndex).Cells(colIndex).Value
        Dim d As Date
        If v IsNot Nothing AndAlso v IsNot DBNull.Value AndAlso Date.TryParse(v.ToString(), d) Then
            _dtp.Value = d
        Else
            _dtp.Value = Date.Today
        End If

        PositionDatePicker(rowIndex, colIndex)
        _dtp.Visible = True
        _dtp.BringToFront()
    End Sub

    ' Apply picked date to cell + DataRow + save immediately
    Private Sub ApplyPickedDateToCellAndSave(r As Integer, c As Integer)
        If r < 0 OrElse c < 0 Then Exit Sub
        If DataGridView1.Columns(c).Name <> "date_procured" Then Exit Sub

        Dim picked As Date = _dtp.Value.Date

        ' 1) Update grid cell
        DataGridView1.Rows(r).Cells(c).Value = picked

        ' 2) Update underlying row & persist
        Dim view As DataRowView = TryCast(DataGridView1.Rows(r).DataBoundItem, DataRowView)
        If view IsNot Nothing Then
            view.Row("date_procured") = picked
            SaveRow(view.Row)
        End If

        ' 3) Refresh visuals
        DataGridView1.InvalidateCell(c, r)
    End Sub

    Private Sub _dtp_ValueChanged(sender As Object, e As EventArgs) Handles _dtp.ValueChanged
        If _dtpCell.Row < 0 Then Return
        ApplyPickedDateToCellAndSave(_dtpCell.Row, _dtpCell.Col)
    End Sub

    Private Sub _dtp_CloseUp(sender As Object, e As EventArgs) Handles _dtp.CloseUp
        If _dtpCell.Row < 0 Then Return
        ApplyPickedDateToCellAndSave(_dtpCell.Row, _dtpCell.Col)
        _dtp.Visible = False
        _dtpCell = (-1, -1)
    End Sub

    ' Keep the picker glued to the cell when scrolling/resizing
    Private Sub Grid_RepositionDatePicker(sender As Object, e As EventArgs)
        If Not _dtp.Visible OrElse DataGridView1.CurrentCell Is Nothing Then Return
        If DataGridView1.Columns(_dtpCell.Col).Name <> "date_procured" Then
            _dtp.Visible = False : _dtpCell = (-1, -1) : Return
        End If
        PositionDatePicker(_dtpCell.Row, _dtpCell.Col)
    End Sub

    ' Center & size the DateTimePicker to the DATE PROCURED cell
    Private Sub PositionDatePicker(rowIndex As Integer, colIndex As Integer)
        If rowIndex < 0 OrElse colIndex < 0 Then Exit Sub
        Dim r = DataGridView1.GetCellDisplayRectangle(colIndex, rowIndex, True)

        Const pad As Integer = 3
        Dim w As Integer = Math.Max(60, r.Width - (pad * 2))
        Dim h As Integer = Math.Max(22, r.Height - (pad * 2))

        Dim x As Integer = r.X + (r.Width - w) \ 2
        Dim y As Integer = r.Y + (r.Height - h) \ 2

        _dtp.SetBounds(x, y, w, h)
    End Sub

    ' Save the DataRow bound to a given grid row
    Private Sub PersistRowAt(rowIndex As Integer)
        If rowIndex < 0 Then Exit Sub
        ' 1) End edit in the binding layer
        If _bs IsNot Nothing Then _bs.EndEdit()

        ' 2) Resolve the DataRowView
        Dim view As DataRowView = TryCast(DataGridView1.Rows(rowIndex).DataBoundItem, DataRowView)
        If view Is Nothing Then Exit Sub

        ' 3) End edit at the view level (ensures RowState = Modified)
        view.EndEdit()

        Dim row As DataRow = view.Row
        If row.RowState = DataRowState.Unchanged Then
            ' In rare cases (checkbox/combobox) it can still be Unchanged—force it
            row.SetModified()
        End If

        SaveRow(row)
    End Sub

    Private Sub Cmdnew_Click(sender As Object, e As EventArgs) Handles cmdnew.Click
        Try
            frmissuanceSEPFile.Dispose()
            frmissuanceSEPFile.cmdadd.Enabled = True
            frmissuanceSEPFile.cmddelete.Enabled = True
            'frmissuanceSEPFile.cmdprint.Enabled = False
            frmissuanceSEPFile.ShowDialog()
        Catch
        End Try
    End Sub

    Private Function CurrentIcsNoFromGrid() As String
        Dim g = Me.DataGridView1
        If g Is Nothing OrElse g.Rows.Count = 0 Then Return ""

        ' Prefer selected row, fallback to current row
        Dim r As DataGridViewRow = Nothing
        If g.SelectedRows IsNot Nothing AndAlso g.SelectedRows.Count > 0 Then
            r = g.SelectedRows(0)
        Else
            r = g.CurrentRow
        End If
        If r Is Nothing OrElse r.IsNewRow Then Return ""

        ' Try common column names (exact + case-insensitive)
        Dim candidates = New String() {"ics_no", "ICS_NO", "colICSNo", "ICSNo"}
        Dim colName As String = Nothing

        For Each key As String In candidates
            If g.Columns.Contains(key) Then
                colName = key : Exit For
            End If
            For Each c As DataGridViewColumn In g.Columns
                If String.Equals(c.Name, key, StringComparison.OrdinalIgnoreCase) Then
                    colName = c.Name : Exit For
                End If
            Next
            If colName IsNot Nothing Then Exit For
        Next

        ' Optional: fall back by HeaderText if needed
        If colName Is Nothing Then
            For Each c As DataGridViewColumn In g.Columns
                If String.Equals(c.HeaderText, "ICS No", StringComparison.OrdinalIgnoreCase) Then
                    colName = c.Name : Exit For
                End If
            Next
        End If

        If colName Is Nothing Then Return ""

        Dim v = r.Cells(colName).Value
        Return If(v Is Nothing OrElse v Is DBNull.Value, "", v.ToString().Trim())
    End Function



    Private Async Sub cmdmanage_Click(sender As Object, e As EventArgs) Handles cmdmanage.Click
        Dim icsNo As String = CurrentIcsNoFromGrid()
        If String.IsNullOrWhiteSpace(icsNo) Then
            MessageBox.Show("Please select an ICS to manage.", "No ICS selected",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Using f As New frmissuanceSEPFile()
            f.StartPosition = FormStartPosition.CenterParent
            Await f.OpenForManageAsync(icsNo)
            f.ShowDialog(Me)
        End Using
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
            Dim desc As String = Convert.ToString(r.Cells("descriptions").Value) ' ✅ TBL_SEP uses "descriptions"
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
                    "WHERE LTRIM(RTRIM(inv.DESCRIPTIONS)) = LTRIM(RTRIM(@desc));"

                    Dim cmdUpd As New SqlClient.SqlCommand(sqlUpd, conn, tx)
                    Dim pAdd = cmdUpd.Parameters.Add("@add", SqlDbType.Decimal)
                    pAdd.Precision = 18 : pAdd.Scale = 2
                    Dim pDesc = cmdUpd.Parameters.Add("@desc", SqlDbType.NVarChar, 3500)

                    Dim cmdDel As New SqlClient.SqlCommand("DELETE FROM TBL_SEP WHERE ID = @id;", conn, tx)
                    cmdDel.Parameters.Add("@id", SqlDbType.Int)

                    ' Process each selected row: update inventory then delete
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

                        ' Always delete the SEP row
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

End Class
