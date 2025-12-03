' frmrisfile.vb
Imports System.Data
Imports System.Data.SqlClient
Imports System.Threading.Tasks
Imports System.Drawing
Imports System.Globalization
Imports System.Linq
Imports Microsoft.Reporting.WinForms
Imports System.IO

Public Class frmrisfile

    '──────────────────────────────────────────────────────────────────────────────
    ' Per-row persisted state (prevents duplicates and handles desc/IP changes)
    '──────────────────────────────────────────────────────────────────────────────
    Private Class RowState
        Public Property Saving As Boolean
        Public Property SavedQty As Decimal
        Public Property SavedIPNO As String
        Public Property SavedDesc As String
        Public Property SavedUnit As String
        Public Property SavedRemarks As String
    End Class

    '──────────────────────────────────────────────────────────────────────────────
    ' Form state
    '──────────────────────────────────────────────────────────────────────────────
    Private _isNew As Boolean = True
    Private _risId As Integer = 0
    Private _risNo As String = ""
    Private _suppressEvents As Boolean = False
    Private _inLoad As Boolean = False
    Private _inSave As Boolean = False
    Private _lastHiRow As Integer = -1
    Private _suppressAutoEdit As Boolean = True   ' block auto-edit during load

    ' DataGridView columns
    Private Const COL_ID As String = "ID"
    Private Const COL_STOCKNO As String = "STOCK_NO"
    Private Const COL_UNIT As String = "UNIT"
    Private Const COL_DESC As String = "DESCRIPTIONS"
    Private Const COL_QTY As String = "QUANTITY"
    Private Const COL_AVAIL_Y As String = "AVAIL_Y"
    Private Const COL_AVAIL_N As String = "AVAIL_N"
    Private Const COL_SQTY As String = "SQUANTITY"
    Private Const COL_REMARKS As String = "REMARKS"

    ' Caches
    Private _inventory As DataTable
    Private _descBase As DataTable

    ' Deadlock/timeout retry policy
    Private Const RETRIES As Integer = 3
    Private Shared ReadOnly RETRY_DELAYS_MS As Integer() = {120, 280, 600}
    Private Shared Function IsRetryableSql(ex As SqlException) As Boolean
        Return ex.Number = 1205 OrElse ex.Number = 1222 ' deadlock / lock request timeout
    End Function

    ' Office/Section → Code map
    Private ReadOnly officeCodes As New Dictionary(Of String, String) From {
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
        {"ALTERNATIVE DISPUTE RESOLUTION DIVISION (CAO-V)", "ADRD (COA V)"},
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

    '──────────────────────────────────────────────────────────────────────────────
    ' Load
    '──────────────────────────────────────────────────────────────────────────────
    Private Async Sub frmrisfile_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            _inLoad = True

            ConfigureGrid()

            txtRisNo.ReadOnly = True
            txtRisNo.Text = ""
            cmdprint.Enabled = False
            DataGridView1.Enabled = False
            cmdsave.Text = "Save"

            combooffsec.Items.Clear()
            combooffsec.Items.AddRange(officeCodes.Keys.ToArray())
            AddHandler combooffsec.SelectionChangeCommitted, AddressOf OfficeChanged
            AddHandler combooffsec.SelectedIndexChanged, AddressOf OfficeChanged
            AddHandler combooffsec.TextChanged, AddressOf OfficeChanged

            Await LoadApproverCombosAsync()
            _inventory = Await LoadInventoryAsync()

            AddHandler dtdate.ValueChanged, AddressOf dtdate_ValueChanged
            AddHandler cmdadd.Click, AddressOf cmdadd_Click
            AddHandler cmddelete.Click, AddressOf cmddelete_Click
        Catch ex As Exception
            MessageBox.Show("Load error: " & ex.Message, "frmrisfile", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            _inLoad = False
        End Try
    End Sub

    '──────────────────────────────────────────────────────────────────────────────
    ' Grid style & columns
    '──────────────────────────────────────────────────────────────────────────────
    Private Sub ConfigureGrid()
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
            .EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2
            .ReadOnly = False

            ' Flat look
            .BorderStyle = BorderStyle.None
            .CellBorderStyle = DataGridViewCellBorderStyle.None
            .ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
            .AdvancedCellBorderStyle.Left = DataGridViewAdvancedCellBorderStyle.None
            .AdvancedCellBorderStyle.Right = DataGridViewAdvancedCellBorderStyle.None
            .AdvancedCellBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.None
            .AdvancedCellBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.None

            Dim hdrColor As Color = Color.FromArgb(73, 94, 252)
            .EnableHeadersVisualStyles = False
            .ColumnHeadersDefaultCellStyle.BackColor = hdrColor
            .ColumnHeadersDefaultCellStyle.ForeColor = Color.White
            .ColumnHeadersDefaultCellStyle.SelectionBackColor = hdrColor
            .ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White
            .ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 9.0!, FontStyle.Bold)
            .ColumnHeadersHeight = 52
            .ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing

            .DefaultCellStyle.Font = New Font("Segoe UI", 8.0!)
            .DefaultCellStyle.ForeColor = Color.Black
            .RowsDefaultCellStyle.BackColor = Color.White
            .AlternatingRowsDefaultCellStyle.BackColor = Color.White
            .DefaultCellStyle.SelectionBackColor = Color.FromArgb(210, 222, 255)
            .DefaultCellStyle.SelectionForeColor = Color.Black
            .RowTemplate.Height = 30
            .RowTemplate.DefaultCellStyle.Padding = New Padding(0, 4, 0, 4)
        End With

        ' Columns
        DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.Name = COL_ID, .HeaderText = "ID", .Visible = False})
        DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.Name = COL_STOCKNO, .HeaderText = "Stock No.", .Width = 150})
        DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.Name = COL_UNIT, .HeaderText = "Unit", .Width = 100})

        ' ───────────────────────────────────────────────
        ' Description (ComboBox Column)
        ' ───────────────────────────────────────────────
        Dim cDesc As New DataGridViewComboBoxColumn With {
    .Name = COL_DESC,
    .HeaderText = "Description",
    .FlatStyle = FlatStyle.Flat,
    .DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton,
    .AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
    .Width = 520, ' Increase this value as needed
    .MinimumWidth = 520
}
        DataGridView1.Columns.Add(cDesc)


        Dim qtyCol As New DataGridViewTextBoxColumn With {.Name = COL_QTY, .HeaderText = "Quantity", .Width = 80}
        qtyCol.DefaultCellStyle = New DataGridViewCellStyle With {.Alignment = DataGridViewContentAlignment.MiddleCenter, .Format = "N2"}
        DataGridView1.Columns.Add(qtyCol)

        Dim ay As New DataGridViewCheckBoxColumn With {.Name = COL_AVAIL_Y, .HeaderText = "Avail Yes", .Width = 80}
        Dim an As New DataGridViewCheckBoxColumn With {.Name = COL_AVAIL_N, .HeaderText = "Avail No", .Width = 80}
        ay.DefaultCellStyle = New DataGridViewCellStyle With {.Alignment = DataGridViewContentAlignment.MiddleCenter}
        an.DefaultCellStyle = New DataGridViewCellStyle With {.Alignment = DataGridViewContentAlignment.MiddleCenter}
        DataGridView1.Columns.Add(ay)
        DataGridView1.Columns.Add(an)

        Dim sq As New DataGridViewTextBoxColumn With {.Name = COL_SQTY, .HeaderText = "Stock Qty", .Width = 80, .ReadOnly = True}
        sq.DefaultCellStyle = New DataGridViewCellStyle With {.Alignment = DataGridViewContentAlignment.MiddleCenter, .Format = "N2"}
        DataGridView1.Columns.Add(sq)

        DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.Name = COL_REMARKS, .HeaderText = "Remarks", .Width = 260})

        DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None


        ' Header alignment
        DataGridView1.Columns(COL_QTY).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
        DataGridView1.Columns(COL_AVAIL_Y).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
        DataGridView1.Columns(COL_AVAIL_N).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
        DataGridView1.Columns(COL_SQTY).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter

        ' Handlers
        AddHandler DataGridView1.CellValueChanged, AddressOf Grid_CellValueChanged
        AddHandler DataGridView1.CurrentCellDirtyStateChanged, AddressOf Grid_CurrentCellDirtyStateChanged
        AddHandler DataGridView1.EditingControlShowing, AddressOf Grid_EditingControlShowing
        AddHandler DataGridView1.CellEndEdit, AddressOf Grid_CellEndEdit
        AddHandler DataGridView1.CellEnter, AddressOf Grid_CellEnter
        AddHandler DataGridView1.CurrentCellChanged, AddressOf Grid_CurrentCellChanged
        AddHandler DataGridView1.CellValidated, AddressOf Grid_CellValidated
        AddHandler DataGridView1.DataError, Sub(s, ev) ev.ThrowException = False
    End Sub

    ' Row highlight
    Private Sub Grid_CurrentCellChanged(sender As Object, e As EventArgs)
        Try
            If _lastHiRow >= 0 AndAlso _lastHiRow < DataGridView1.Rows.Count Then
                DataGridView1.Rows(_lastHiRow).DefaultCellStyle.BackColor = Color.White
            End If
            If DataGridView1.CurrentCell Is Nothing Then Return
            Dim rIdx = DataGridView1.CurrentCell.RowIndex
            If rIdx >= 0 AndAlso rIdx < DataGridView1.Rows.Count Then
                DataGridView1.Rows(rIdx).DefaultCellStyle.BackColor = Color.FromArgb(235, 244, 255)
                _lastHiRow = rIdx
            End If
        Catch
        End Try
    End Sub

    Private Sub Grid_CellEnter(sender As Object, e As DataGridViewCellEventArgs)
        If _suppressAutoEdit OrElse e.RowIndex < 0 Then Return
        Dim colName = DataGridView1.Columns(e.ColumnIndex).Name
        If colName = COL_STOCKNO OrElse colName = COL_UNIT OrElse colName = COL_QTY OrElse colName = COL_REMARKS OrElse colName = COL_DESC Then
            DataGridView1.BeginEdit(True)
        End If
        If colName = COL_DESC Then
            Dim cb = TryCast(DataGridView1.EditingControl, DataGridViewComboBoxEditingControl)
            If cb IsNot Nothing Then
                cb.DropDownStyle = ComboBoxStyle.DropDown
                cb.AutoCompleteSource = AutoCompleteSource.ListItems
                cb.AutoCompleteMode = AutoCompleteMode.SuggestAppend
                cb.IntegralHeight = False
                cb.DroppedDown = True
            End If
        End If
    End Sub

    '──────────────────────────────────────────────────────────────────────────────
    ' Office/Section code binding
    '──────────────────────────────────────────────────────────────────────────────
    Private Sub OfficeChanged(sender As Object, e As EventArgs)
        Dim name As String = Nothing
        If combooffsec.SelectedItem IsNot Nothing Then name = TryCast(combooffsec.SelectedItem, String)
        If String.IsNullOrWhiteSpace(name) Then name = combooffsec.Text
        Dim code As String = ""
        If Not String.IsNullOrWhiteSpace(name) AndAlso officeCodes.TryGetValue(name, code) Then
            txtrccode.Text = code
        Else
            txtrccode.Clear()
        End If
    End Sub

    Private Sub dtdate_ValueChanged(sender As Object, e As EventArgs)
        ' RIS No generated on Save only.
    End Sub

    '──────────────────────────────────────────────────────────────────────────────
    ' Approvers & Inventory loaders
    '──────────────────────────────────────────────────────────────────────────────
    Private Async Function LoadApproverCombosAsync() As Task
        Dim sql As String = "SELECT CNAMES FROM TBL_APPROVER WITH (NOLOCK) WHERE TYPE='Employee' ORDER BY CNAMES;"
        Dim tbl As New DataTable
        Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
            Await conn.OpenAsync()
            Using da As New SqlDataAdapter(sql, conn)
                da.Fill(tbl)
            End Using
        End Using

        Dim list As New List(Of String)
        For Each r As DataRow In tbl.Rows
            Dim s As String = Convert.ToString(r("CNAMES"))
            s = s.Replace(vbCrLf, " | ").Replace(vbLf, " | ").Replace(vbCr, " | ")
            list.Add(s)
        Next

        Dim targets = {comboreqby, comboappby, comboissby, comborecby}
        For Each cb In targets
            cb.Items.Clear()
            cb.Items.AddRange(list.ToArray())
        Next
    End Function

    Private Async Function LoadInventoryAsync() As Task(Of DataTable)
        Dim sql As String =
            "SELECT ID, PTYPE, DESCRIPTIONS, ATITLE, ACODE, IPNO, UNITS, QTY, STATUS, REMARKS 
             FROM TBL_INVENTORY WITH (NOLOCK)
             WHERE ISNULL(DESCRIPTIONS,'')<>'' AND ISNULL(QTY,0) > 0
             ORDER BY DESCRIPTIONS;"

        Dim tbl As New DataTable
        Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
            Await conn.OpenAsync()
            Using da As New SqlDataAdapter(sql, conn)
                da.Fill(tbl)
            End Using
        End Using

        _inventory = tbl

        _descBase = New DataTable()
        _descBase.Columns.Add("DESCRIPTIONS", GetType(String))
        For Each s In _inventory.AsEnumerable().Select(Function(r) Convert.ToString(r("DESCRIPTIONS"))).Distinct()
            _descBase.Rows.Add(s)
        Next

        Dim descCol = CType(DataGridView1.Columns(COL_DESC), DataGridViewComboBoxColumn)
        descCol.DataSource = _descBase
        descCol.DisplayMember = "DESCRIPTIONS"
        descCol.ValueMember = "DESCRIPTIONS"

        Return _inventory
    End Function

    '──────────────────────────────────────────────────────────────────────────────
    ' Public: open existing RIS (for frmRIS → cmdedit)
    '──────────────────────────────────────────────────────────────────────────────
    Public Async Function LoadExistingAsync(risNo As String) As Task
        If String.IsNullOrWhiteSpace(risNo) Then
            MessageBox.Show("RIS No. is missing.", "Open RIS", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        _inLoad = True
        _suppressAutoEdit = True

        If comboreqby.Items.Count = 0 Then Await LoadApproverCombosAsync()
        If _inventory Is Nothing Then _inventory = Await LoadInventoryAsync()

        ' Load header
        Dim sql As String =
        "SELECT TOP 1
             RIS_NO, RIS_DATE, RIS_DIVISION, RIS_OFFICE, RIS_RCODE, FCLUSTER, PURPOSES,
             REQ_BY_NAME, REQ_BY_DESIGNATION,
             APP_BY_NAME, APP_BY_DESIGNATION,
             ISS_BY_NAME, ISS_BY_DESIGNATION,
             REC_BY_NAME, REC_BY_DESIGNATION
         FROM TBL_RIS WITH (NOLOCK)
         WHERE RIS_NO=@RIS_NO;"

        Dim found As Boolean = False
        Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
            Await conn.OpenAsync()
            Using cmd As New SqlCommand(sql, conn)
                cmd.Parameters.Add("@RIS_NO", SqlDbType.NVarChar, 50).Value = risNo
                Using rd = Await cmd.ExecuteReaderAsync()
                    If Await rd.ReadAsync() Then
                        found = True
                        txtRisNo.Text = Convert.ToString(rd("RIS_NO"))
                        dtdate.Value = If(IsDBNull(rd("RIS_DATE")), Date.Today, Convert.ToDateTime(rd("RIS_DATE")))
                        combodivision.Text = If(IsDBNull(rd("RIS_DIVISION")), "", Convert.ToString(rd("RIS_DIVISION")))
                        combooffsec.Text = If(IsDBNull(rd("RIS_OFFICE")), "", Convert.ToString(rd("RIS_OFFICE")))
                        txtrccode.Text = If(IsDBNull(rd("RIS_RCODE")), "", Convert.ToString(rd("RIS_RCODE")))
                        combofcluster.Text = If(IsDBNull(rd("FCLUSTER")), "", Convert.ToString(rd("FCLUSTER")))
                        txtpurpose.Text = If(IsDBNull(rd("PURPOSES")), "", Convert.ToString(rd("PURPOSES")))

                        Dim reqName = If(IsDBNull(rd("REQ_BY_NAME")), "", Convert.ToString(rd("REQ_BY_NAME")))
                        Dim reqDes = If(IsDBNull(rd("REQ_BY_DESIGNATION")), "", Convert.ToString(rd("REQ_BY_DESIGNATION")))
                        comboreqby.Text = If(String.IsNullOrWhiteSpace(reqDes), reqName, $"{reqName} | {reqDes}")

                        Dim appName = If(IsDBNull(rd("APP_BY_NAME")), "", Convert.ToString(rd("APP_BY_NAME")))
                        Dim appDes = If(IsDBNull(rd("APP_BY_DESIGNATION")), "", Convert.ToString(rd("APP_BY_DESIGNATION")))
                        comboappby.Text = If(String.IsNullOrWhiteSpace(appDes), appName, $"{appName} | {appDes}")

                        Dim issName = If(IsDBNull(rd("ISS_BY_NAME")), "", Convert.ToString(rd("ISS_BY_NAME")))
                        Dim issDes = If(IsDBNull(rd("ISS_BY_DESIGNATION")), "", Convert.ToString(rd("ISS_BY_DESIGNATION")))
                        comboissby.Text = If(String.IsNullOrWhiteSpace(issDes), issName, $"{issName} | {issDes}")

                        Dim recName = If(IsDBNull(rd("REC_BY_NAME")), "", Convert.ToString(rd("REC_BY_NAME")))
                        Dim recDes = If(IsDBNull(rd("REC_BY_DESIGNATION")), "", Convert.ToString(rd("REC_BY_DESIGNATION")))
                        comborecby.Text = If(String.IsNullOrWhiteSpace(recDes), recName, $"{recName} | {recDes}")
                    End If
                End Using
            End Using
        End Using

        If Not found Then
            _inLoad = False
            _suppressAutoEdit = False
            MessageBox.Show("RIS not found.", "Open RIS", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        _isNew = False
        _risNo = risNo
        cmdsave.Text = "Update"
        cmdprint.Enabled = True
        DataGridView1.Enabled = True

        Await LoadItemsAsync()

        DataGridView1.ClearSelection()
        DataGridView1.CurrentCell = Nothing
        _suppressAutoEdit = False
        _inLoad = False
    End Function

    '──────────────────────────────────────────────────────────────────────────────
    ' RIS number generator (inside Save transaction)
    '──────────────────────────────────────────────────────────────────────────────
    Private Async Function GenerateNextRisNoAsync(conn As SqlConnection, tran As SqlTransaction) As Task(Of String)
        Dim d As Date = dtdate.Value.Date
        Dim yr As Integer = d.Year
        Dim mm As Integer = d.Month

        Dim nextSeries As Integer = 1
        Using cmd As New SqlCommand("
            SELECT @S = ISNULL(MAX(CAST(RIGHT(RIS_NO,4) AS INT)), 0) + 1
            FROM TBL_RIS WITH (UPDLOCK, HOLDLOCK)
            WHERE YEAR(RIS_DATE) = @Y;", conn, tran)
            cmd.Parameters.Add("@Y", SqlDbType.Int).Value = yr
            cmd.Parameters.Add("@S", SqlDbType.Int).Direction = ParameterDirection.Output
            Await cmd.ExecuteNonQueryAsync()
            nextSeries = CInt(cmd.Parameters("@S").Value)
        End Using

        Return $"{yr}-{mm.ToString("00")}-{nextSeries.ToString("0000")}"
    End Function

    '──────────────────────────────────────────────────────────────────────────────
    ' Save/Update parent
    '──────────────────────────────────────────────────────────────────────────────
    Private Async Sub cmdsave_Click(sender As Object, e As EventArgs) Handles cmdsave.Click
        If _inSave Then Return
        _inSave = True
        _suppressEvents = True
        Try
            ' ───────────────────────────────────────────────
            ' Required header fields BEFORE RIS creation
            ' ───────────────────────────────────────────────
            If String.IsNullOrWhiteSpace(combooffsec.Text) Then
                MessageBox.Show("Office/Section is required.", "RIS", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If
            If String.IsNullOrWhiteSpace(combofcluster.Text) Then
                MessageBox.Show("Fund Cluster is required.", "RIS", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            ' Commit any active edits
            If DataGridView1.IsCurrentCellInEditMode Then
                DataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit)
                DataGridView1.EndEdit()
            End If
            Validate()

            ' Save header (generate RIS_NO if new)
            Await SaveOrUpdateParentAsync()

            ' Upsert all rows once
            For Each r As DataGridViewRow In DataGridView1.Rows
                If r.IsNewRow Then Continue For
                Dim desc As String = Convert.ToString(r.Cells(COL_DESC).Value)
                If Not String.IsNullOrWhiteSpace(desc) Then
                    Await UpsertItemAsync(r)
                End If
            Next

            ' Update ISSUEDTO for all items to current comborecby (name only)
            Await UpdateIssuedToAllAsync(txtRisNo.Text, GetNamePart(comborecby.Text))

            ' Reload grid from DB
            Await LoadItemsAsync()

            DataGridView1.ClearSelection()
            DataGridView1.CurrentCell = Nothing

            cmdprint.Enabled = True
            DataGridView1.Enabled = True
            cmdsave.Text = "Update"
            _isNew = False

            If _inventory Is Nothing Then _inventory = Await LoadInventoryAsync()

            MessageBox.Show("RIS has been successfully saved/updated.", "RIS", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MessageBox.Show("Save error: " & ex.Message, "RIS Save", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            _suppressEvents = False
            _inSave = False
        End Try
    End Sub

    Private Async Function UpdateIssuedToAllAsync(risNo As String, issuedToName As String) As Task
        If String.IsNullOrWhiteSpace(risNo) Then Exit Function
        Try
            Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
                Await conn.OpenAsync()
                Using cmd As New SqlCommand("
                UPDATE TBL_RISITEMS
                SET ISSUEDTO = @ISSUEDTO
                WHERE RIS_NO = @RIS_NO;", conn)
                    cmd.Parameters.Add("@ISSUEDTO", SqlDbType.NVarChar, 350).Value = issuedToName
                    cmd.Parameters.Add("@RIS_NO", SqlDbType.NVarChar, 250).Value = risNo
                    Await cmd.ExecuteNonQueryAsync()
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Update ISSUEDTO error: " & ex.Message, "RIS Update", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End Try
    End Function


    Private Async Function SaveOrUpdateParentAsync() As Task
        ' Secondary safety check (in case called from elsewhere)
        If String.IsNullOrWhiteSpace(combooffsec.Text) Then
            Throw New ApplicationException("Office/Section is required.")
        End If
        If String.IsNullOrWhiteSpace(combofcluster.Text) Then
            Throw New ApplicationException("Fund Cluster is required.")
        End If

        Dim risOffice As String = If(combooffsec.SelectedItem Is Nothing, combooffsec.Text, combooffsec.SelectedItem.ToString())
        Dim risRCode As String = If(officeCodes.ContainsKey(risOffice), officeCodes(risOffice), txtrccode.Text)

        Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
            Await conn.OpenAsync()
            Using tran = conn.BeginTransaction(IsolationLevel.Serializable)
                Try
                    If _isNew OrElse String.IsNullOrWhiteSpace(txtRisNo.Text) Then
                        Dim newRisNo As String = Await GenerateNextRisNoAsync(conn, tran)

                        Using cmd As New SqlCommand("
                        INSERT INTO TBL_RIS
                          (RIS_NO, RIS_DATE, RIS_DIVISION, RIS_OFFICE, RIS_RCODE, FCLUSTER, PURPOSES,
                           REQ_BY_NAME, REQ_BY_DESIGNATION, REQ_BY_DATE,
                           APP_BY_NAME, APP_BY_DESIGNATION, APP_BY_DATE,
                           ISS_BY_NAME, ISS_BY_DESIGNATION, ISS_BY_DATE,
                           REC_BY_NAME, REC_BY_DESIGNATION, REC_BY_DATE)
                        VALUES
                          (@RIS_NO, @RIS_DATE, @DIV, @OFF, @RCODE, @FCLUSTER, @PURPOSES,
                           @RNAME, @RDES, @RDATE,
                           @ANAME, @ADES, @ADATE,
                           @ISNAME, @ISDES, @ISDATE,
                           @RENAME, @REDES, @REDATE);
                        SELECT SCOPE_IDENTITY();", conn, tran)

                            cmd.Parameters.Add("@RIS_NO", SqlDbType.NVarChar, 50).Value = newRisNo
                            cmd.Parameters.Add("@RIS_DATE", SqlDbType.Date).Value = dtdate.Value.Date
                            cmd.Parameters.Add("@DIV", SqlDbType.NVarChar, 250).Value = If(String.IsNullOrWhiteSpace(combodivision.Text), CType(DBNull.Value, Object), combodivision.Text)
                            cmd.Parameters.Add("@OFF", SqlDbType.NVarChar, 250).Value = risOffice
                            cmd.Parameters.Add("@RCODE", SqlDbType.NVarChar, 50).Value = If(String.IsNullOrWhiteSpace(risRCode), CType(DBNull.Value, Object), risRCode)
                            cmd.Parameters.Add("@FCLUSTER", SqlDbType.NVarChar, 250).Value = combofcluster.Text
                            cmd.Parameters.Add("@PURPOSES", SqlDbType.NVarChar, 1250).Value = If(String.IsNullOrWhiteSpace(txtpurpose.Text), CType(DBNull.Value, Object), txtpurpose.Text)

                            Dim req = SplitNameDesignation(comboreqby.Text)
                            Dim app = SplitNameDesignation(comboappby.Text)
                            Dim iss = SplitNameDesignation(comboissby.Text)
                            Dim rec = SplitNameDesignation(comborecby.Text)

                            cmd.Parameters.Add("@RNAME", SqlDbType.NVarChar, 250).Value = req.Name
                            cmd.Parameters.Add("@RDES", SqlDbType.NVarChar, 250).Value = req.Desig
                            cmd.Parameters.Add("@RDATE", SqlDbType.Date).Value = dtdate.Value.Date

                            cmd.Parameters.Add("@ANAME", SqlDbType.NVarChar, 250).Value = app.Name
                            cmd.Parameters.Add("@ADES", SqlDbType.NVarChar, 250).Value = app.Desig
                            cmd.Parameters.Add("@ADATE", SqlDbType.Date).Value = dtdate.Value.Date

                            cmd.Parameters.Add("@ISNAME", SqlDbType.NVarChar, 250).Value = iss.Name
                            cmd.Parameters.Add("@ISDES", SqlDbType.NVarChar, 250).Value = iss.Desig
                            cmd.Parameters.Add("@ISDATE", SqlDbType.Date).Value = dtdate.Value.Date

                            cmd.Parameters.Add("@RENAME", SqlDbType.NVarChar, 250).Value = rec.Name
                            cmd.Parameters.Add("@REDES", SqlDbType.NVarChar, 250).Value = rec.Desig
                            cmd.Parameters.Add("@REDATE", SqlDbType.Date).Value = dtdate.Value.Date

                            _risId = Convert.ToInt32(Await cmd.ExecuteScalarAsync())
                            _risNo = newRisNo
                            txtRisNo.Text = _risNo
                        End Using
                    Else
                        Using cmd As New SqlCommand("
                        UPDATE TBL_RIS SET
                           RIS_DATE=@RIS_DATE,
                           RIS_DIVISION=@DIV,
                           RIS_OFFICE=@OFF, RIS_RCODE=@RCODE,
                           FCLUSTER=@FCLUSTER, PURPOSES=@PURPOSES,
                           REQ_BY_NAME=@RNAME, REQ_BY_DESIGNATION=@RDES, REQ_BY_DATE=@RDATE,
                           APP_BY_NAME=@ANAME, APP_BY_DESIGNATION=@ADES, APP_BY_DATE=@ADATE,
                           ISS_BY_NAME=@ISNAME, ISS_BY_DESIGNATION=@ISDES, ISS_BY_DATE=@ISDATE,
                           REC_BY_NAME=@RENAME, REC_BY_DESIGNATION=@REDES, REC_BY_DATE=@REDATE
                        WHERE RIS_NO=@RIS_NO;", conn, tran)

                            cmd.Parameters.Add("@RIS_NO", SqlDbType.NVarChar, 50).Value = txtRisNo.Text
                            cmd.Parameters.Add("@RIS_DATE", SqlDbType.Date).Value = dtdate.Value.Date
                            cmd.Parameters.Add("@DIV", SqlDbType.NVarChar, 250).Value = If(String.IsNullOrWhiteSpace(combodivision.Text), CType(DBNull.Value, Object), combodivision.Text)
                            cmd.Parameters.Add("@OFF", SqlDbType.NVarChar, 250).Value = combooffsec.Text
                            cmd.Parameters.Add("@RCODE", SqlDbType.NVarChar, 50).Value = If(String.IsNullOrWhiteSpace(txtrccode.Text), CType(DBNull.Value, Object), txtrccode.Text)
                            cmd.Parameters.Add("@FCLUSTER", SqlDbType.NVarChar, 250).Value = combofcluster.Text
                            cmd.Parameters.Add("@PURPOSES", SqlDbType.NVarChar, 1250).Value = If(String.IsNullOrWhiteSpace(txtpurpose.Text), CType(DBNull.Value, Object), txtpurpose.Text)

                            Dim req = SplitNameDesignation(comboreqby.Text)
                            Dim app = SplitNameDesignation(comboappby.Text)
                            Dim iss = SplitNameDesignation(comboissby.Text)
                            Dim rec = SplitNameDesignation(comborecby.Text)

                            cmd.Parameters.Add("@RNAME", SqlDbType.NVarChar, 250).Value = req.Name
                            cmd.Parameters.Add("@RDES", SqlDbType.NVarChar, 250).Value = req.Desig
                            cmd.Parameters.Add("@RDATE", SqlDbType.Date).Value = dtdate.Value.Date
                            cmd.Parameters.Add("@ANAME", SqlDbType.NVarChar, 250).Value = app.Name
                            cmd.Parameters.Add("@ADES", SqlDbType.NVarChar, 250).Value = app.Desig
                            cmd.Parameters.Add("@ADATE", SqlDbType.Date).Value = dtdate.Value.Date
                            cmd.Parameters.Add("@ISNAME", SqlDbType.NVarChar, 250).Value = iss.Name
                            cmd.Parameters.Add("@ISDES", SqlDbType.NVarChar, 250).Value = iss.Desig
                            cmd.Parameters.Add("@ISDATE", SqlDbType.Date).Value = dtdate.Value.Date
                            cmd.Parameters.Add("@RENAME", SqlDbType.NVarChar, 250).Value = rec.Name
                            cmd.Parameters.Add("@REDES", SqlDbType.NVarChar, 250).Value = rec.Desig
                            cmd.Parameters.Add("@REDATE", SqlDbType.Date).Value = dtdate.Value.Date

                            If Await cmd.ExecuteNonQueryAsync() = 0 Then Throw New ApplicationException("RIS not found for update.")
                        End Using
                    End If

                    tran.Commit()
                Catch
                    Try : tran.Rollback() : Catch : End Try
                    Throw
                End Try
            End Using
        End Using
    End Function


    Private Function SplitNameDesignation(s As String) As (Name As String, Desig As String)
        If String.IsNullOrWhiteSpace(s) Then Return ("", "")
        Dim parts = s.Split("|"c)
        If parts.Length >= 2 Then
            Return (parts(0).Trim(), parts(1).Trim())
        Else
            Return (s.Trim(), "")
        End If
    End Function

    '──────────────────────────────────────────────────────────────────────────────
    ' Load items (keeps description visible even if OOS)
    '──────────────────────────────────────────────────────────────────────────────
    Private Async Function LoadItemsAsync() As Task
        _inLoad = True
        _suppressEvents = True
        Try
            DataGridView1.Rows.Clear()

            Dim sql As String =
                "SELECT ID, RIS_NO, STOCK_NO, UNIT, DESCRIPTIONS, QUANTITY, SAVAILABLE, SQUANTITY, REMARKS
                 FROM TBL_RISITEMS WITH (NOLOCK)
                 WHERE RIS_NO=@RIS_NO
                 ORDER BY ID;"

            Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
                Await conn.OpenAsync()
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.Add("@RIS_NO", SqlDbType.NVarChar, 250).Value = txtRisNo.Text
                    Using rd = Await cmd.ExecuteReaderAsync()
                        While Await rd.ReadAsync()
                            Dim idx = DataGridView1.Rows.Add()
                            Dim r = DataGridView1.Rows(idx)

                            Dim stockNo As String = Convert.ToString(rd("STOCK_NO"))
                            Dim unit As String = Convert.ToString(rd("UNIT"))
                            Dim desc As String = Convert.ToString(rd("DESCRIPTIONS"))
                            Dim qty As Decimal = Convert.ToDecimal(rd("QUANTITY"))
                            Dim remarks As String = Convert.ToString(rd("REMARKS"))

                            r.Cells(COL_ID).Value = rd("ID")
                            r.Cells(COL_STOCKNO).Value = stockNo
                            r.Cells(COL_UNIT).Value = unit

                            Dim cbCell = CType(r.Cells(COL_DESC), DataGridViewComboBoxCell)
                            If Not DescBaseContains(desc) Then
                                Dim dt As DataTable = _descBase.Copy()
                                dt.Rows.Add(desc)
                                cbCell.DataSource = dt
                            Else
                                cbCell.DataSource = _descBase
                            End If
                            cbCell.DisplayMember = "DESCRIPTIONS"
                            cbCell.ValueMember = "DESCRIPTIONS"
                            r.Cells(COL_DESC).Value = desc

                            r.Cells(COL_QTY).Value = qty
                            Dim sa As String = Convert.ToString(rd("SAVAILABLE"))
                            r.Cells(COL_AVAIL_Y).Value = String.Equals(sa, "YES", StringComparison.OrdinalIgnoreCase)
                            r.Cells(COL_AVAIL_N).Value = String.Equals(sa, "NO", StringComparison.OrdinalIgnoreCase)
                            r.Cells(COL_SQTY).Value = Convert.ToDecimal(rd("SQUANTITY"))
                            r.Cells(COL_REMARKS).Value = remarks

                            ' Track the saved state
                            r.Tag = New RowState With {
                                .SavedQty = qty,
                                .SavedIPNO = stockNo,
                                .SavedDesc = desc,
                                .SavedUnit = unit,
                                .SavedRemarks = remarks,
                                .Saving = False
                            }
                        End While
                    End Using
                End Using
            End Using
        Finally
            _suppressEvents = False
            _inLoad = False
        End Try
    End Function

    Private Function DescBaseContains(desc As String) As Boolean
        If _descBase Is Nothing OrElse String.IsNullOrWhiteSpace(desc) Then Return False
        For Each dr As DataRow In _descBase.Rows
            If String.Equals(Convert.ToString(dr("DESCRIPTIONS")), desc, StringComparison.OrdinalIgnoreCase) Then
                Return True
            End If
        Next
        Return False
    End Function

    '──────────────────────────────────────────────────────────────────────────────
    ' Add/Delete rows
    '──────────────────────────────────────────────────────────────────────────────
    Private Sub cmdadd_Click(sender As Object, e As EventArgs)
        If _isNew OrElse String.IsNullOrWhiteSpace(txtRisNo.Text) Then
            MessageBox.Show("Save the RIS first before adding items.", "Save required", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Dim idx = DataGridView1.Rows.Add()
        Dim r = DataGridView1.Rows(idx)
        r.Cells(COL_QTY).Value = 1D
        r.Cells(COL_AVAIL_Y).Value = False
        r.Cells(COL_AVAIL_N).Value = False
        r.Cells(COL_SQTY).Value = 0D
        r.Tag = New RowState With {.SavedQty = 0D, .SavedIPNO = "", .SavedDesc = "", .SavedUnit = "", .SavedRemarks = "", .Saving = False}

        Dim cbCell = CType(r.Cells(COL_DESC), DataGridViewComboBoxCell)
        cbCell.DataSource = _descBase
        cbCell.DisplayMember = "DESCRIPTIONS"
        cbCell.ValueMember = "DESCRIPTIONS"

        DataGridView1.ClearSelection()
        DataGridView1.CurrentCell = Nothing
    End Sub

    Private Async Sub cmddelete_Click(sender As Object, e As EventArgs)
        If _inSave OrElse _inLoad Then Return

        Dim rows As New List(Of DataGridViewRow)
        Dim seen As New HashSet(Of Integer)

        If DataGridView1.SelectedRows IsNot Nothing AndAlso DataGridView1.SelectedRows.Count > 0 Then
            For Each r As DataGridViewRow In DataGridView1.SelectedRows
                If Not r.IsNewRow AndAlso seen.Add(r.Index) Then rows.Add(r)
            Next
        End If

        If rows.Count = 0 AndAlso DataGridView1.SelectedCells IsNot Nothing AndAlso DataGridView1.SelectedCells.Count > 0 Then
            For Each c As DataGridViewCell In DataGridView1.SelectedCells
                Dim r = DataGridView1.Rows(c.RowIndex)
                If Not r.IsNewRow AndAlso seen.Add(r.Index) Then rows.Add(r)
            Next
        End If

        If rows.Count = 0 AndAlso DataGridView1.CurrentRow IsNot Nothing AndAlso Not DataGridView1.CurrentRow.IsNewRow Then
            If seen.Add(DataGridView1.CurrentRow.Index) Then rows.Add(DataGridView1.CurrentRow)
        End If

        If rows.Count = 0 Then
            MessageBox.Show("Select at least one row to delete.", "Delete", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        If MessageBox.Show("Delete selected row(s)? Stock will be returned to inventory.", "Confirm Delete",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
            Return
        End If

        _suppressEvents = True
        Try
            For Each r In rows
                Dim hasValidId As Boolean = False
                Dim rawId = If(r.Cells(COL_ID).Value, Nothing)
                If rawId IsNot Nothing AndAlso rawId IsNot DBNull.Value Then
                    Dim idTmp As Integer
                    hasValidId = Integer.TryParse(rawId.ToString(), idTmp) AndAlso idTmp > 0
                End If

                If Not hasValidId Then
                    If Not r.IsNewRow Then DataGridView1.Rows.Remove(r)
                Else
                    Await DeleteRowAndRestockAsync(r)
                End If
            Next
        Finally
            _suppressEvents = False
        End Try

        DataGridView1.ClearSelection()
        DataGridView1.CurrentCell = Nothing
    End Sub

    '──────────────────────────────────────────────────────────────────────────────
    ' Grid events (debounced)
    '──────────────────────────────────────────────────────────────────────────────
    Private Sub Grid_CurrentCellDirtyStateChanged(sender As Object, e As EventArgs)
        If _suppressEvents OrElse _inSave OrElse _inLoad Then Exit Sub
        If Not DataGridView1.IsCurrentCellDirty Then Exit Sub
        Dim col = DataGridView1.Columns(DataGridView1.CurrentCell.ColumnIndex)
        If TypeOf DataGridView1.CurrentCell Is DataGridViewCheckBoxCell _
           OrElse String.Equals(col.Name, COL_DESC, StringComparison.OrdinalIgnoreCase) Then
            DataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit)
        End If
    End Sub

    Private Async Sub Grid_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs)
        If _suppressEvents OrElse _inSave OrElse _inLoad Then Return
        If e.RowIndex < 0 Then Return

        Dim row = DataGridView1.Rows(e.RowIndex)
        Dim col = DataGridView1.Columns(e.ColumnIndex).Name

        Try
            If col = COL_DESC Then
                Dim desc As String = Convert.ToString(row.Cells(COL_DESC).Value)
                If String.IsNullOrWhiteSpace(desc) Then Return

                ' Lookup inventory row from cache
                Dim inv = _inventory.Select("DESCRIPTIONS = " & Quote(desc))
                If inv.Length > 0 Then
                    Dim ipno As String = Convert.ToString(inv(0)("IPNO"))
                    Dim unit As String = Convert.ToString(inv(0)("UNITS"))
                    Dim invQty As Decimal = SafeToDecimal(inv(0)("QTY"))

                    _suppressEvents = True
                    row.Cells(COL_STOCKNO).Value = ipno
                    row.Cells(COL_UNIT).Value = unit

                    ' Ensure qty is at least 1, but do NOT fire another change
                    Dim qty As Decimal = SafeToDecimal(row.Cells(COL_QTY).Value)
                    If qty <= 0D Then
                        row.Cells(COL_QTY).Value = 1D
                        qty = 1D
                    End If
                    _suppressEvents = False

                    ' Preview remaining immediately (UI), min 0
                    row.Cells(COL_SQTY).Value = Math.Max(0D, invQty - qty)

                    If String.IsNullOrWhiteSpace(txtRisNo.Text) Then
                        MessageBox.Show("Save the RIS first before adding items.", "Save required", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        Return
                    End If

                    Await UpsertItemAsync(row)
                End If

            ElseIf col = COL_QTY OrElse col = COL_AVAIL_Y OrElse col = COL_AVAIL_N _
                OrElse col = COL_STOCKNO OrElse col = COL_UNIT Then

                If col = COL_AVAIL_Y OrElse col = COL_AVAIL_N Then
                    _suppressEvents = True
                    If col = COL_AVAIL_Y Then
                        row.Cells(COL_AVAIL_N).Value = Not CBool(row.Cells(COL_AVAIL_Y).Value)
                    Else
                        row.Cells(COL_AVAIL_Y).Value = Not CBool(row.Cells(COL_AVAIL_N).Value)
                    End If
                    _suppressEvents = False
                End If

                If String.IsNullOrWhiteSpace(txtRisNo.Text) Then Return

                ' Quick preview of Stock Qty on qty change
                If col = COL_QTY OrElse col = COL_DESC Then
                    Dim desc As String = Convert.ToString(row.Cells(COL_DESC).Value)
                    If Not String.IsNullOrWhiteSpace(desc) Then
                        Dim inv = _inventory.Select("DESCRIPTIONS = " & Quote(desc))
                        If inv.Length > 0 Then
                            Dim invQty As Decimal = SafeToDecimal(inv(0)("QTY"))
                            Dim qty As Decimal = SafeToDecimal(row.Cells(COL_QTY).Value)
                            row.Cells(COL_SQTY).Value = Math.Max(0D, invQty - qty)
                        End If
                    End If
                End If

                Await UpsertItemAsync(row)
            End If

        Catch ex As Exception
            MessageBox.Show("Edit error: " & ex.Message, "RIS Items", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End Try
    End Sub

    Private Async Sub Grid_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs)
        If _suppressEvents OrElse _inSave OrElse _inLoad Then Exit Sub
        If e.RowIndex < 0 Then Exit Sub

        DataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit)
        DataGridView1.EndEdit()

        ' Keep as a safety net; UpsertItemAsync itself will no-op if nothing changed
        If String.IsNullOrWhiteSpace(txtRisNo.Text) Then Exit Sub
        Await UpsertItemAsync(DataGridView1.Rows(e.RowIndex))
    End Sub

    Private Async Sub Grid_CellValidated(sender As Object, e As DataGridViewCellEventArgs)
        If _suppressEvents OrElse _inSave OrElse _inLoad Then Exit Sub
        If e.RowIndex < 0 Then Exit Sub
        If String.IsNullOrWhiteSpace(txtRisNo.Text) Then Exit Sub

        Dim name = DataGridView1.Columns(e.ColumnIndex).Name
        If name = COL_REMARKS OrElse name = COL_QTY OrElse name = COL_STOCKNO OrElse name = COL_UNIT Then
            DataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit)
            DataGridView1.EndEdit()
            Await UpsertItemAsync(DataGridView1.Rows(e.RowIndex))
        End If
    End Sub

    Private Sub Grid_EditingControlShowing(sender As Object, e As DataGridViewEditingControlShowingEventArgs)
        If DataGridView1.CurrentCell IsNot Nothing AndAlso DataGridView1.Columns(DataGridView1.CurrentCell.ColumnIndex).Name = COL_QTY Then
            Dim tb = TryCast(e.Control, TextBox)
            If tb IsNot Nothing Then
                RemoveHandler tb.KeyPress, AddressOf Qty_KeyPress
                AddHandler tb.KeyPress, AddressOf Qty_KeyPress
            End If
        End If

        If DataGridView1.CurrentCell IsNot Nothing AndAlso DataGridView1.Columns(DataGridView1.CurrentCell.ColumnIndex).Name = COL_DESC Then
            Dim cb = TryCast(e.Control, DataGridViewComboBoxEditingControl)
            If cb IsNot Nothing Then
                cb.DropDownStyle = ComboBoxStyle.DropDown
                cb.AutoCompleteSource = AutoCompleteSource.ListItems
                cb.AutoCompleteMode = AutoCompleteMode.SuggestAppend
                cb.IntegralHeight = False
            End If
        End If
    End Sub

    Private Sub Qty_KeyPress(sender As Object, e As KeyPressEventArgs)
        Dim dec = Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator
        If Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsDigit(e.KeyChar) AndAlso e.KeyChar.ToString() <> dec Then
            e.Handled = True
        End If
    End Sub

    '──────────────────────────────────────────────────────────────────────────────
    ' Upsert item + inventory sync (handles desc/IP change & de-bounces)
    '──────────────────────────────────────────────────────────────────────────────

    Private Async Function UpsertItemAsync(row As DataGridViewRow) As Task
        Dim desc As String = Convert.ToString(row.Cells(COL_DESC).Value)
        If String.IsNullOrWhiteSpace(desc) Then Return

        Dim ipno As String = Convert.ToString(row.Cells(COL_STOCKNO).Value)
        Dim unit As String = Convert.ToString(row.Cells(COL_UNIT).Value)
        Dim qty As Decimal = SafeToDecimal(row.Cells(COL_QTY).Value)
        Dim remarks As String = Convert.ToString(row.Cells(COL_REMARKS).Value)
        If qty < 0D Then qty = 0D

        Dim st As RowState = TryCast(row.Tag, RowState)
        If st Is Nothing Then
            st = New RowState With {.SavedQty = 0D, .SavedIPNO = "", .SavedDesc = "", .SavedUnit = "", .SavedRemarks = "", .Saving = False}
            row.Tag = st
        End If

        ' Prevent overlapping saves for the same row
        If st.Saving Then Return
        st.Saving = True

        Try
            Dim existingId As Integer = If(row.Cells(COL_ID).Value Is Nothing OrElse row.Cells(COL_ID).Value Is DBNull.Value, 0, Convert.ToInt32(row.Cells(COL_ID).Value))
            Dim ipChanged As Boolean = Not String.Equals(st.SavedIPNO, ipno, StringComparison.OrdinalIgnoreCase)

            ' Early no-op if nothing changed
            If existingId > 0 AndAlso Not ipChanged AndAlso st.SavedQty = qty AndAlso
           String.Equals(st.SavedUnit, unit, StringComparison.OrdinalIgnoreCase) AndAlso
           String.Equals(If(st.SavedRemarks, ""), If(remarks, ""), StringComparison.Ordinal) AndAlso
           String.Equals(st.SavedDesc, desc, StringComparison.Ordinal) Then
                Return
            End If

            Dim attempts As Integer = 0
            Do
                Dim shouldRetry As Boolean = False
                Try
                    Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
                        Await conn.OpenAsync()
                        Using tran = conn.BeginTransaction(IsolationLevel.ReadCommitted)
                            Try
                                Using cmdPrio As New SqlCommand("SET DEADLOCK_PRIORITY LOW; SET LOCK_TIMEOUT 5000;", conn, tran)
                                    Await cmdPrio.ExecuteNonQueryAsync()
                                End Using

                                ' Current stock for new/old items
                                Dim curNew As Decimal = 0D
                                If Not String.IsNullOrWhiteSpace(ipno) Then
                                    Using cmdGetNew As New SqlCommand("
                                    SELECT QTY FROM TBL_INVENTORY WITH (ROWLOCK, UPDLOCK)
                                    WHERE IPNO=@IPNO;", conn, tran)
                                        cmdGetNew.Parameters.Add("@IPNO", SqlDbType.NVarChar, 250).Value = ipno
                                        Dim o = Await cmdGetNew.ExecuteScalarAsync()
                                        curNew = If(o Is Nothing OrElse o Is DBNull.Value, 0D, SafeToDecimal(o))
                                    End Using
                                End If

                                Dim curOld As Decimal = 0D
                                If ipChanged AndAlso Not String.IsNullOrWhiteSpace(st.SavedIPNO) Then
                                    Using cmdGetOld As New SqlCommand("
                                    SELECT QTY FROM TBL_INVENTORY WITH (ROWLOCK, UPDLOCK)
                                    WHERE IPNO=@IPNO;", conn, tran)
                                        cmdGetOld.Parameters.Add("@IPNO", SqlDbType.NVarChar, 250).Value = st.SavedIPNO
                                        Dim o = Await cmdGetOld.ExecuteScalarAsync()
                                        curOld = If(o Is Nothing OrElse o Is DBNull.Value, 0D, SafeToDecimal(o))
                                    End Using
                                End If

                                ' Stock checks
                                If existingId = 0 Then
                                    If Not String.IsNullOrWhiteSpace(ipno) AndAlso curNew < qty Then
                                        Throw New ApplicationException($"Insufficient stock for {desc}. Available: {curNew:N2}, Needed: {qty:N2}")
                                    End If
                                ElseIf ipChanged Then
                                    If Not String.IsNullOrWhiteSpace(ipno) AndAlso curNew < qty Then
                                        Throw New ApplicationException($"Insufficient stock for {desc}. Available: {curNew:N2}, Needed: {qty:N2}")
                                    End If
                                Else
                                    Dim delta As Decimal = qty - st.SavedQty
                                    If delta > 0D AndAlso curNew < delta Then
                                        Throw New ApplicationException($"Insufficient stock for {desc}. Available: {curNew:N2}, Additional needed: {delta:N2}")
                                    End If
                                End If

                                ' Upsert + Inventory
                                Dim newInvAfter As Decimal = curNew
                                If existingId = 0 Then
                                    ' INSERT RIS row (includes ISSUEDTO from comborecby)
                                    Using cmdIns As New SqlCommand("
                                    INSERT INTO TBL_RISITEMS
                                      (RIS_NO, STOCK_NO, UNIT, DESCRIPTIONS, QUANTITY, SAVAILABLE, SQUANTITY, REMARKS, FCLUSTER, RCCODE, ISSUEDTO)
                                    VALUES
                                      (@RIS_NO, @STOCK_NO, @UNIT, @DESC, @QTY, @SAVAIL, @SQTY, @REMARKS, @FCLUSTER, @RCCODE, @ISSUEDTO);
                                    SELECT SCOPE_IDENTITY();", conn, tran)

                                        cmdIns.Parameters.Add("@RIS_NO", SqlDbType.NVarChar, 250).Value = txtRisNo.Text
                                        cmdIns.Parameters.Add("@STOCK_NO", SqlDbType.NVarChar, 250).Value = ipno
                                        cmdIns.Parameters.Add("@UNIT", SqlDbType.NVarChar, 250).Value = unit
                                        cmdIns.Parameters.Add("@DESC", SqlDbType.NVarChar, 2500).Value = desc
                                        Dim pQ = cmdIns.Parameters.Add("@QTY", SqlDbType.Decimal) : pQ.Precision = 18 : pQ.Scale = 2 : pQ.Value = qty

                                        ' Deduct full qty from new item
                                        If Not String.IsNullOrWhiteSpace(ipno) AndAlso qty <> 0D Then
                                            Using cmdInv As New SqlCommand("
                                            UPDATE TBL_INVENTORY WITH (ROWLOCK)
                                            SET QTY = QTY - @QTY
                                            WHERE IPNO=@IPNO;", conn, tran)
                                                Dim p = cmdInv.Parameters.Add("@QTY", SqlDbType.Decimal) : p.Precision = 18 : p.Scale = 2 : p.Value = qty
                                                cmdInv.Parameters.Add("@IPNO", SqlDbType.NVarChar, 250).Value = ipno
                                                Await cmdInv.ExecuteNonQueryAsync()
                                            End Using
                                            newInvAfter = curNew - qty
                                        End If

                                        cmdIns.Parameters.Add("@SAVAIL", SqlDbType.NVarChar, 50).Value = If(newInvAfter > 0D, "YES", "NO")
                                        Dim pSQ = cmdIns.Parameters.Add("@SQTY", SqlDbType.Decimal) : pSQ.Precision = 18 : pSQ.Scale = 2 : pSQ.Value = newInvAfter
                                        cmdIns.Parameters.Add("@REMARKS", SqlDbType.NVarChar, 550).Value = If(String.IsNullOrWhiteSpace(remarks), CType(DBNull.Value, Object), remarks)

                                        ' Existing fields
                                        cmdIns.Parameters.Add("@FCLUSTER", SqlDbType.NVarChar, 250).Value = combofcluster.Text
                                        cmdIns.Parameters.Add("@RCCODE", SqlDbType.NVarChar, 50).Value = txtrccode.Text

                                        ' NEW: ISSUEDTO from comborecby
                                        cmdIns.Parameters.Add("@ISSUEDTO", SqlDbType.NVarChar, 350).Value = comborecby.Text.Trim()

                                        Dim newId = Convert.ToInt32(Await cmdIns.ExecuteScalarAsync())
                                        row.Cells(COL_ID).Value = newId
                                    End Using

                                ElseIf ipChanged Then
                                    ' Return old to OLD, deduct full from NEW, then update row
                                    If Not String.IsNullOrWhiteSpace(st.SavedIPNO) AndAlso st.SavedQty <> 0D Then
                                        Using cmdOld As New SqlCommand("
                                        UPDATE TBL_INVENTORY WITH (ROWLOCK)
                                        SET QTY = QTY + @QTY
                                        WHERE IPNO=@IPNO;", conn, tran)
                                            Dim p = cmdOld.Parameters.Add("@QTY", SqlDbType.Decimal) : p.Precision = 18 : p.Scale = 2 : p.Value = st.SavedQty
                                            cmdOld.Parameters.Add("@IPNO", SqlDbType.NVarChar, 250).Value = st.SavedIPNO
                                            Await cmdOld.ExecuteNonQueryAsync()
                                        End Using
                                    End If

                                    If Not String.IsNullOrWhiteSpace(ipno) AndAlso qty <> 0D Then
                                        Using cmdNew As New SqlCommand("
                                        UPDATE TBL_INVENTORY WITH (ROWLOCK)
                                        SET QTY = QTY - @QTY
                                        WHERE IPNO=@IPNO;", conn, tran)
                                            Dim p = cmdNew.Parameters.Add("@QTY", SqlDbType.Decimal) : p.Precision = 18 : p.Scale = 2 : p.Value = qty
                                            cmdNew.Parameters.Add("@IPNO", SqlDbType.NVarChar, 250).Value = ipno
                                            Await cmdNew.ExecuteNonQueryAsync()
                                        End Using
                                        newInvAfter = curNew - qty
                                    End If

                                    Using cmdUpd As New SqlCommand("
                                    UPDATE TBL_RISITEMS SET
                                        STOCK_NO=@STOCK_NO, UNIT=@UNIT, DESCRIPTIONS=@DESC,
                                        QUANTITY=@QTY, SAVAILABLE=@SAVAIL, SQUANTITY=@SQTY, REMARKS=@REMARKS,
                                        FCLUSTER=@FCLUSTER, RCCODE=@RCCODE, ISSUEDTO=@ISSUEDTO
                                    WHERE ID=@ID;", conn, tran)

                                        cmdUpd.Parameters.Add("@STOCK_NO", SqlDbType.NVarChar, 250).Value = ipno
                                        cmdUpd.Parameters.Add("@UNIT", SqlDbType.NVarChar, 250).Value = unit
                                        cmdUpd.Parameters.Add("@DESC", SqlDbType.NVarChar, 2500).Value = desc
                                        Dim pQ2 = cmdUpd.Parameters.Add("@QTY", SqlDbType.Decimal) : pQ2.Precision = 18 : pQ2.Scale = 2 : pQ2.Value = qty
                                        cmdUpd.Parameters.Add("@SAVAIL", SqlDbType.NVarChar, 50).Value = If(newInvAfter > 0D, "YES", "NO")
                                        Dim pSQ2 = cmdUpd.Parameters.Add("@SQTY", SqlDbType.Decimal) : pSQ2.Precision = 18 : pSQ2.Scale = 2 : pSQ2.Value = newInvAfter
                                        cmdUpd.Parameters.Add("@REMARKS", SqlDbType.NVarChar, 550).Value = If(String.IsNullOrWhiteSpace(remarks), CType(DBNull.Value, Object), remarks)

                                        ' Existing fields
                                        cmdUpd.Parameters.Add("@FCLUSTER", SqlDbType.NVarChar, 250).Value = combofcluster.Text
                                        cmdUpd.Parameters.Add("@RCCODE", SqlDbType.NVarChar, 50).Value = txtrccode.Text

                                        ' NEW: ISSUEDTO from comborecby
                                        cmdUpd.Parameters.Add("@ISSUEDTO", SqlDbType.NVarChar, 350).Value = comborecby.Text.Trim()

                                        cmdUpd.Parameters.Add("@ID", SqlDbType.Int).Value = existingId
                                        Await cmdUpd.ExecuteNonQueryAsync()
                                    End Using

                                Else
                                    ' Same item: adjust delta only, then update
                                    Dim delta As Decimal = qty - st.SavedQty
                                    If delta <> 0D AndAlso Not String.IsNullOrWhiteSpace(ipno) Then
                                        Using cmdInv As New SqlCommand("
                                        UPDATE TBL_INVENTORY WITH (ROWLOCK)
                                        SET QTY = QTY - @DELTA
                                        WHERE IPNO=@IPNO;", conn, tran)
                                            Dim pD = cmdInv.Parameters.Add("@DELTA", SqlDbType.Decimal) : pD.Precision = 18 : pD.Scale = 2 : pD.Value = delta
                                            cmdInv.Parameters.Add("@IPNO", SqlDbType.NVarChar, 250).Value = ipno
                                            Await cmdInv.ExecuteNonQueryAsync()
                                        End Using
                                        newInvAfter = curNew - delta
                                    Else
                                        newInvAfter = curNew
                                    End If

                                    Using cmdUpd As New SqlCommand("
                                    UPDATE TBL_RISITEMS SET
                                        STOCK_NO=@STOCK_NO, UNIT=@UNIT, DESCRIPTIONS=@DESC,
                                        QUANTITY=@QTY, SAVAILABLE=@SAVAIL, SQUANTITY=@SQTY, REMARKS=@REMARKS,
                                        FCLUSTER=@FCLUSTER, RCCODE=@RCCODE, ISSUEDTO=@ISSUEDTO
                                    WHERE ID=@ID;", conn, tran)

                                        cmdUpd.Parameters.Add("@STOCK_NO", SqlDbType.NVarChar, 250).Value = ipno
                                        cmdUpd.Parameters.Add("@UNIT", SqlDbType.NVarChar, 250).Value = unit
                                        cmdUpd.Parameters.Add("@DESC", SqlDbType.NVarChar, 2500).Value = desc
                                        Dim pQ3 = cmdUpd.Parameters.Add("@QTY", SqlDbType.Decimal) : pQ3.Precision = 18 : pQ3.Scale = 2 : pQ3.Value = qty
                                        cmdUpd.Parameters.Add("@SAVAIL", SqlDbType.NVarChar, 50).Value = If(newInvAfter > 0D, "YES", "NO")
                                        Dim pSQ3 = cmdUpd.Parameters.Add("@SQTY", SqlDbType.Decimal) : pSQ3.Precision = 18 : pSQ3.Scale = 2 : pSQ3.Value = newInvAfter
                                        cmdUpd.Parameters.Add("@REMARKS", SqlDbType.NVarChar, 550).Value = If(String.IsNullOrWhiteSpace(remarks), CType(DBNull.Value, Object), remarks)

                                        ' Existing fields
                                        cmdUpd.Parameters.Add("@FCLUSTER", SqlDbType.NVarChar, 250).Value = combofcluster.Text
                                        cmdUpd.Parameters.Add("@RCCODE", SqlDbType.NVarChar, 50).Value = txtrccode.Text

                                        ' NEW: ISSUEDTO from comborecby
                                        cmdUpd.Parameters.Add("@ISSUEDTO", SqlDbType.NVarChar, 350).Value = comborecby.Text.Trim()

                                        cmdUpd.Parameters.Add("@ID", SqlDbType.Int).Value = existingId
                                        Await cmdUpd.ExecuteNonQueryAsync()
                                    End Using
                                End If

                                tran.Commit()

                                ' UI reflect
                                row.Cells(COL_SQTY).Value = Math.Max(0D, newInvAfter)
                                _suppressEvents = True
                                row.Cells(COL_AVAIL_Y).Value = (newInvAfter > 0D)
                                row.Cells(COL_AVAIL_N).Value = Not (newInvAfter > 0D)
                                _suppressEvents = False

                                ' Persist new saved state
                                st.SavedQty = qty
                                st.SavedIPNO = ipno
                                st.SavedDesc = desc
                                st.SavedUnit = unit
                                st.SavedRemarks = If(remarks, "")
                                row.Tag = st

                                Return
                            Catch
                                Try : tran.Rollback() : Catch : End Try
                                Throw
                            End Try
                        End Using
                    End Using

                Catch ex As SqlException
                    If IsRetryableSql(ex) AndAlso attempts < RETRIES Then
                        shouldRetry = True
                    Else
                        MessageBox.Show("The item couldn't be saved right now due to concurrent updates. Please try again.",
                                    "Save conflict", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        Return
                    End If
                Catch ex As ApplicationException
                    MessageBox.Show(ex.Message, "Inventory", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    _suppressEvents = True
                    row.Cells(COL_DESC).Value = st.SavedDesc
                    row.Cells(COL_STOCKNO).Value = st.SavedIPNO
                    row.Cells(COL_UNIT).Value = st.SavedUnit
                    row.Cells(COL_QTY).Value = st.SavedQty
                    row.Cells(COL_REMARKS).Value = st.SavedRemarks
                    _suppressEvents = False
                    Return
                Catch
                    MessageBox.Show("The item couldn't be saved right now. Please try again.",
                                "Save error", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Return
                End Try

                If shouldRetry Then
                    Await Task.Delay(RETRY_DELAYS_MS(attempts))
                    attempts += 1
                End If
            Loop While attempts <= RETRIES

        Finally
            st.Saving = False
            row.Tag = st
        End Try
    End Function



    '──────────────────────────────────────────────────────────────────────────────
    ' Delete row + restock (uses saved state)
    '──────────────────────────────────────────────────────────────────────────────
    Private Async Function DeleteRowAndRestockAsync(row As DataGridViewRow) As Task
        Dim existingId As Integer = 0
        Dim rawId = If(row.Cells(COL_ID).Value, Nothing)
        If rawId IsNot Nothing AndAlso rawId IsNot DBNull.Value Then
            Integer.TryParse(rawId.ToString(), existingId)
        End If

        Dim st As RowState = TryCast(row.Tag, RowState)
        Dim qty As Decimal = If(st IsNot Nothing, st.SavedQty, SafeToDecimal(row.Cells(COL_QTY).Value))
        Dim ipno As String = If(st IsNot Nothing, st.SavedIPNO, Convert.ToString(row.Cells(COL_STOCKNO).Value))

        If qty < 0D Then qty = 0D
        If ipno Is Nothing Then ipno = ""

        Dim attempts As Integer = 0
        Do
            Dim shouldRetry As Boolean = False
            Try
                Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
                    Await conn.OpenAsync()
                    Using tran = conn.BeginTransaction(IsolationLevel.ReadCommitted)
                        Try
                            Using setPrio As New SqlCommand("SET DEADLOCK_PRIORITY LOW; SET LOCK_TIMEOUT 5000;", conn, tran)
                                setPrio.CommandTimeout = 10
                                Await setPrio.ExecuteNonQueryAsync()
                            End Using

                            If existingId > 0 Then
                                Using cmdDel As New SqlCommand("DELETE FROM TBL_RISITEMS WHERE ID=@ID;", conn, tran)
                                    cmdDel.CommandTimeout = 30
                                    cmdDel.Parameters.Add("@ID", SqlDbType.Int).Value = existingId
                                    Await cmdDel.ExecuteNonQueryAsync()
                                End Using
                            End If

                            If qty > 0D AndAlso Not String.IsNullOrWhiteSpace(ipno) Then
                                Using cmdInv As New SqlCommand("
                                    UPDATE TBL_INVENTORY WITH (ROWLOCK, UPDLOCK)
                                    SET QTY = QTY + @QTY WHERE IPNO=@IPNO;", conn, tran)
                                    cmdInv.CommandTimeout = 30
                                    Dim p = cmdInv.Parameters.Add("@QTY", SqlDbType.Decimal) : p.Precision = 18 : p.Scale = 2 : p.Value = qty
                                    cmdInv.Parameters.Add("@IPNO", SqlDbType.NVarChar, 250).Value = ipno
                                    Await cmdInv.ExecuteNonQueryAsync()
                                End Using
                            End If

                            tran.Commit()
                        Catch
                            Try : tran.Rollback() : Catch : End Try
                            Throw
                        End Try
                    End Using
                End Using

                If Not row.IsNewRow AndAlso row.DataGridView IsNot Nothing Then
                    row.DataGridView.Rows.Remove(row)
                End If
                Return

            Catch ex As SqlException
                If IsRetryableSql(ex) AndAlso attempts < RETRIES Then
                    shouldRetry = True
                Else
                    MessageBox.Show("Delete failed: " & ex.Message, "Delete item", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Return
                End If
            Catch ex As Exception
                MessageBox.Show("Delete error: " & ex.Message, "Delete item", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End Try

            If shouldRetry Then
                Await Task.Delay(RETRY_DELAYS_MS(attempts))
                attempts += 1
            End If
        Loop While attempts <= RETRIES
    End Function

    '──────────────────────────────────────────────────────────────────────────────
    ' Helpers
    '──────────────────────────────────────────────────────────────────────────────
    Private Function Quote(s As String) As String
        If s Is Nothing Then s = ""
        Return "'" & s.Replace("'", "''") & "'"
    End Function

    Private Function SafeToDecimal(v As Object) As Decimal
        If v Is Nothing OrElse v Is DBNull.Value Then Return 0D
        Dim s As String = Convert.ToString(v)
        Dim d As Decimal
        If Decimal.TryParse(s, NumberStyles.Any, CultureInfo.CurrentCulture, d) Then
            Return d
        End If
        If Decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, d) Then
            Return d
        End If
        Return 0D
    End Function

    '──────────────────────────────────────────────────────────────────────────────
    '                            PRINTING (RDLC)
    '──────────────────────────────────────────────────────────────────────────────
    Private Sub cmdprint_Click(sender As Object, e As EventArgs) Handles cmdprint.Click
        Try
            Dim dt As DataTable = BuildRisTableFromGrid()
            If dt.Rows.Count = 0 Then
                MessageBox.Show("No items to print.", "RIS Print", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If

            Dim parms As New List(Of ReportParameter) From {
                New ReportParameter("ename", txtename.Text.Trim()),
                New ReportParameter("fcluster", combofcluster.Text.Trim()),
                New ReportParameter("division", combodivision.Text.Trim()),
                New ReportParameter("office", combooffsec.Text.Trim()),
                New ReportParameter("rccode", txtrccode.Text.Trim()),
                New ReportParameter("risno", txtRisNo.Text.Trim()),
                New ReportParameter("iarno", SafeFindText("txtiarno")),
                New ReportParameter("purpose", txtpurpose.Text.Trim()),
                New ReportParameter("rname", GetNamePart(comboreqby.Text)),
                New ReportParameter("rdesignation", GetPosPart(comboreqby.Text)),
                New ReportParameter("aname", GetNamePart(comboappby.Text)),
                New ReportParameter("adesignation", GetPosPart(comboappby.Text)),
                New ReportParameter("iname", GetNamePart(comboissby.Text)),
                New ReportParameter("idesignation", GetPosPart(comboissby.Text)),
                New ReportParameter("recname", GetNamePart(comborecby.Text)),
                New ReportParameter("recdesignation", GetPosPart(comborecby.Text)),
                New ReportParameter("recdate", dtdate.Value.ToString("MM/dd/yyyy"))
            }

            Using preview As New frmpprev()
                With preview.ReportViewer1
                    .ProcessingMode = ProcessingMode.Local
                    .LocalReport.ReportPath = Path.Combine(Application.StartupPath, "Report", "rptris.rdlc")
                    .LocalReport.DataSources.Clear()
                    Dim names = .LocalReport.GetDataSourceNames()
                    If names IsNot Nothing AndAlso names.Count > 0 Then
                        For Each n In names
                            .LocalReport.DataSources.Add(New ReportDataSource(n, dt))
                        Next
                    Else
                        .LocalReport.DataSources.Add(New ReportDataSource("DTRIS", dt))
                    End If
                    .LocalReport.SetParameters(parms)
                    .SetDisplayMode(DisplayMode.PrintLayout)
                    .ZoomMode = ZoomMode.Percent
                    .ZoomPercent = 100
                    .RefreshReport()
                End With
                preview.lbltransaction.Text = "RIS"
                preview.panelsubmit.Visible = False
                preview.ShowDialog(Me)
            End Using

        Catch ex As Exception
            MessageBox.Show(ex.ToString(), "RIS Print Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Function BuildRisTableFromGrid() As DataTable
        Dim dt As New DataTable("DTRIS")
        dt.Columns.Add("STOCK_NO", GetType(String))
        dt.Columns.Add("UNIT", GetType(String))
        dt.Columns.Add("DESCRIPTIONS", GetType(String))
        dt.Columns.Add("QUANTITY", GetType(Decimal))
        dt.Columns.Add("SQUANTITY", GetType(Decimal))
        dt.Columns.Add("REMARKS", GetType(String))
        dt.Columns.Add("SAVAILABLE", GetType(Boolean))

        Dim colMap As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
        For i As Integer = 0 To DataGridView1.Columns.Count - 1
            colMap(DataGridView1.Columns(i).Name) = i
        Next

        For Each r As DataGridViewRow In DataGridView1.Rows
            If r.IsNewRow Then Continue For
            Dim dr = dt.NewRow()
            dr("STOCK_NO") = Convert.ToString(GetCellValue(r, colMap, COL_STOCKNO))
            dr("UNIT") = Convert.ToString(GetCellValue(r, colMap, COL_UNIT))
            dr("DESCRIPTIONS") = Convert.ToString(GetCellValue(r, colMap, COL_DESC))
            dr("QUANTITY") = SafeToDecimal(GetCellValue(r, colMap, COL_QTY))
            dr("SQUANTITY") = SafeToDecimal(GetCellValue(r, colMap, COL_SQTY))
            dr("REMARKS") = Convert.ToString(GetCellValue(r, colMap, COL_REMARKS))

            Dim yesVal As Object = GetCellValue(r, colMap, COL_AVAIL_Y)
            Dim noVal As Object = GetCellValue(r, colMap, COL_AVAIL_N)
            Dim sav As Boolean = ToBool(yesVal)
            If Not sav AndAlso ToBool(noVal) Then sav = False
            dr("SAVAILABLE") = sav

            dt.Rows.Add(dr)
        Next

        Return dt
    End Function

    Private Function GetCellValue(row As DataGridViewRow,
                                  colMap As Dictionary(Of String, Integer),
                                  prefName As String,
                                  Optional fallbackIndex As Integer = -1) As Object
        If row Is Nothing Then Return Nothing
        If colMap IsNot Nothing AndAlso colMap.ContainsKey(prefName) Then
            Dim i = colMap(prefName)
            If i >= 0 AndAlso i < row.Cells.Count Then Return row.Cells(i).Value
        End If
        If fallbackIndex >= 0 AndAlso fallbackIndex < row.Cells.Count Then
            Return row.Cells(fallbackIndex).Value
        End If
        Return Nothing
    End Function

    Private Function ToBool(o As Object) As Boolean
        If o Is Nothing OrElse o Is DBNull.Value Then Return False
        Dim s = Convert.ToString(o).Trim().ToLowerInvariant()
        Select Case s
            Case "true", "1", "yes", "y", "☑" : Return True
            Case "false", "0", "no", "n", "☐" : Return False
        End Select
        Dim b As Boolean
        If Boolean.TryParse(s, b) Then Return b
        Return False
    End Function

    Private Function GetNamePart(raw As String) As String
        If String.IsNullOrWhiteSpace(raw) Then Return ""
        Dim p = raw.Split("|"c)
        Return p(0).Trim()
    End Function

    Private Function GetPosPart(raw As String) As String
        If String.IsNullOrWhiteSpace(raw) Then Return ""
        Dim p = raw.Split("|"c)
        If p.Length >= 2 Then Return p(1).Trim()
        Return ""
    End Function

    Private Function SafeFindText(ctrlName As String) As String
        Try
            Dim c = Me.Controls.Find(ctrlName, True).FirstOrDefault()
            If c IsNot Nothing Then Return c.Text.Trim()
        Catch
        End Try
        Return ""
    End Function

End Class

