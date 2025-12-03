Imports System.Data
Imports System.Data.SqlClient
Public Class frmconfig

    Private _selectedId As Integer = -1
    Private _isLoading As Boolean = False
    Private _wiringDone As Boolean = False

    ' Fixed choices for NEW/EDIT only
    Private ReadOnly _allowedTypes As String() = {
        "Inspection Committee",
        "Inventory Committee",
        "Disposal Committee",
        "Employee",
        "PR: Signatories",
        "PR: Category",
        "AOB: End User Heads",
        "AOB: Section Heads",
        "AOB: Approval"
    }



    Private Sub TabPage1_Click(sender As Object, e As EventArgs) Handles TabPage1.Click

    End Sub

    Private Sub frmconfig_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            ' ───────────────────────────────────────────────
            ' Reset / base config
            ' ───────────────────────────────────────────────
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

                ' 🚫 Flat, borderless look
                .BorderStyle = BorderStyle.None
                .CellBorderStyle = DataGridViewCellBorderStyle.None
                .ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
                .AdvancedCellBorderStyle.Left = DataGridViewAdvancedCellBorderStyle.None
                .AdvancedCellBorderStyle.Right = DataGridViewAdvancedCellBorderStyle.None
                .AdvancedCellBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.None
                .AdvancedCellBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.None
            End With

            ' ───────────────────────────────────────────────
            ' Columns (keep bindings; widths optional)
            ' ───────────────────────────────────────────────
            DataGridView1.ColumnCount = 9

            With DataGridView1
                ' 0) ID (hidden)
                .Columns(0).Name = "ID"
                .Columns(0).HeaderText = "ID"
                .Columns(0).DataPropertyName = "ID"
                .Columns(0).Visible = False
                .Columns(0).SortMode = DataGridViewColumnSortMode.NotSortable

                ' 1) First Name
                .Columns(1).Name = "Login_Name"
                .Columns(1).HeaderText = "First Name"
                .Columns(1).DataPropertyName = "Login_Name"
                .Columns(1).SortMode = DataGridViewColumnSortMode.NotSortable

                ' 2) Last Name
                .Columns(2).Name = "Login_LName"
                .Columns(2).HeaderText = "Last Name"
                .Columns(2).DataPropertyName = "Login_LName"
                .Columns(2).SortMode = DataGridViewColumnSortMode.NotSortable

                ' 3) Office
                .Columns(3).Name = "Login_Office"
                .Columns(3).HeaderText = "Office"
                .Columns(3).DataPropertyName = "Login_Office"
                .Columns(3).DefaultCellStyle.WrapMode = DataGridViewTriState.True
                .Columns(3).SortMode = DataGridViewColumnSortMode.NotSortable

                ' 4) Position
                .Columns(4).Name = "Login_OPos"
                .Columns(4).HeaderText = "Position"
                .Columns(4).DataPropertyName = "Login_OPos"
                .Columns(4).DefaultCellStyle.WrapMode = DataGridViewTriState.True
                .Columns(4).SortMode = DataGridViewColumnSortMode.NotSortable

                ' 5) User ID
                .Columns(5).Name = "Login_UserID"
                .Columns(5).Name = "Login_UserID"
                .Columns(5).HeaderText = "User ID"
                .Columns(5).DataPropertyName = "Login_UserID"
                .Columns(5).SortMode = DataGridViewColumnSortMode.NotSortable

                ' 6) Email
                .Columns(6).Name = "Login_Email"
                .Columns(6).HeaderText = "Email"
                .Columns(6).DataPropertyName = "Login_Email"
                .Columns(6).SortMode = DataGridViewColumnSortMode.NotSortable

                ' 4) Position
                .Columns(7).Name = "Login_Position"
                .Columns(7).HeaderText = "Access Level"
                .Columns(7).DataPropertyName = "Login_Position"
                .Columns(7).DefaultCellStyle.WrapMode = DataGridViewTriState.True
                .Columns(7).SortMode = DataGridViewColumnSortMode.NotSortable

                ' 7) Status
                .Columns(8).Name = "Login_Status"
                .Columns(8).HeaderText = "Status"
                .Columns(8).DataPropertyName = "Login_Status"
                .Columns(8).SortMode = DataGridViewColumnSortMode.NotSortable
            End With

            ' Optional: autosize to keep things tidy without scroll jitter
            With DataGridView1
                .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
                .Columns(1).FillWeight = 110
                .Columns(2).FillWeight = 110
                .Columns(3).FillWeight = 130
                .Columns(4).FillWeight = 130
                .Columns(5).FillWeight = 100
                .Columns(6).FillWeight = 170
                .Columns(7).FillWeight = 80
            End With

            ' ───────────────────────────────────────────────
            ' Styling  (match your new design)
            ' ───────────────────────────────────────────────
            Dim hdrColor As Color = Color.FromArgb(73, 94, 252)

            With DataGridView1
                .EnableHeadersVisualStyles = False

                ' Header
                .ColumnHeadersDefaultCellStyle.BackColor = hdrColor
                .ColumnHeadersDefaultCellStyle.ForeColor = Color.White
                .ColumnHeadersDefaultCellStyle.SelectionBackColor = hdrColor
                .ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White
                .ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 9.0!, FontStyle.Bold)
                .ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
                .ColumnHeadersHeight = 52
                .ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing

                ' Rows
                .DefaultCellStyle.Font = New Font("Segoe UI", 9.0!)
                .DefaultCellStyle.ForeColor = Color.Black
                .RowsDefaultCellStyle.BackColor = Color.White
                .AlternatingRowsDefaultCellStyle.BackColor = Color.White
                .DefaultCellStyle.SelectionBackColor = Color.FromArgb(210, 222, 255)
                .DefaultCellStyle.SelectionForeColor = Color.Black

                ' Row spacing
                .RowTemplate.Height = 34
                .RowTemplate.DefaultCellStyle.Padding = New Padding(0, 6, 0, 6)
            End With

            ' ───────────────────────────────────────────────
            ' SIGNATORIES
            ' ───────────────────────────────────────────────

            Try
                _isLoading = True

                ConfigureGrid()
                EnsureComboTypeSeeded() ' never DB-bind combostype
                BindFilterTypes()       ' load DISTINCT types for combosignatories (filter)

                panelsignatories.Enabled = False
                cmdssave.Text = "Save"

                WireHandlersOnce()

                ' Load using current filter or first item if available
                If combosignatories.Items.Count > 0 Then
                    combosignatories.SelectedIndex = 0
                Else
                    LoadApproversForType("") ' empty list
                End If

            Catch ex As Exception
                MessageBox.Show("Form Load Error: " & ex.Message, "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error)
            Finally
                _isLoading = False
            End Try



            ' ───────────────────────────────────────────────
            ' Load data
            ' ───────────────────────────────────────────────

            loadconfigs()
            LoadUsers()

        Catch ex As Exception
            MessageBox.Show("Form Load Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub WireHandlersOnce()
        If _wiringDone Then Return

        AddHandler combosignatories.SelectedIndexChanged, AddressOf combosignatories_SelectedIndexChanged
        AddHandler DataGridView2.CellClick, AddressOf Grid_CellClick
        AddHandler DataGridView2.KeyUp, AddressOf Grid_KeyUp ' keyboard selection support

        _wiringDone = True
    End Sub

    Private Sub SeedTypeChoices()
        combostype.DropDownStyle = ComboBoxStyle.DropDownList
        combostype.Items.Clear()
        combostype.Items.AddRange(_allowedTypes)

        ' Optional: if your filter combo has no data from DB yet, seed it so users can filter immediately
        If combosignatories.Items.Count = 0 Then
            combosignatories.DropDownStyle = ComboBoxStyle.DropDownList
            combosignatories.Items.Clear()
            combosignatories.Items.AddRange(_allowedTypes)
        End If
    End Sub
    Private Sub EnsureComboTypeSeeded()
        combostype.DataSource = Nothing ' protect against accidental DataSource use
        combostype.DropDownStyle = ComboBoxStyle.DropDownList
        If combostype.Items.Count <> _allowedTypes.Length Then
            combostype.Items.Clear()
            combostype.Items.AddRange(_allowedTypes)
        End If
    End Sub


    ' ─────────────────────────────────────────────────────────────────────────────
    ' Styling + Columns (matches your flat look)
    ' ─────────────────────────────────────────────────────────────────────────────
    Private Sub ConfigureGrid()
        With DataGridView2
            .DataSource = Nothing
            .Rows.Clear()
            .Columns.Clear()
            .AutoGenerateColumns = False
            .RowHeadersVisible = False
            .AllowUserToAddRows = False
            .AllowUserToResizeRows = False
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
            .MultiSelect = False

            .BorderStyle = BorderStyle.None
            .CellBorderStyle = DataGridViewCellBorderStyle.None
            .ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
            .AdvancedCellBorderStyle.Left = DataGridViewAdvancedCellBorderStyle.None
            .AdvancedCellBorderStyle.Right = DataGridViewAdvancedCellBorderStyle.None
            .AdvancedCellBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.None
            .AdvancedCellBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.None

            .ColumnCount = 3
            .Columns(0).Name = "ID" : .Columns(0).HeaderText = "ID" : .Columns(0).DataPropertyName = "ID" : .Columns(0).Visible = False
            .Columns(1).Name = "TYPE" : .Columns(1).HeaderText = "TYPE" : .Columns(1).DataPropertyName = "TYPE" : .Columns(1).Visible = False
            .Columns(2).Name = "CNAMES" : .Columns(2).HeaderText = "Name and Position" : .Columns(2).DataPropertyName = "CNAMES"
            .Columns(2).AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill

            With DataGridView2.Columns("CNAMES")
                .DefaultCellStyle.WrapMode = DataGridViewTriState.True
            End With

            With DataGridView2
                .AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells
            End With

            Dim hdrColor As Color = Color.FromArgb(73, 94, 252)
            .EnableHeadersVisualStyles = False
            .ColumnHeadersDefaultCellStyle.BackColor = hdrColor
            .ColumnHeadersDefaultCellStyle.ForeColor = Color.White
            .ColumnHeadersDefaultCellStyle.SelectionBackColor = hdrColor
            .ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White
            .ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 11.0!, FontStyle.Bold)
            .ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            .ColumnHeadersHeight = 52
            .ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing

            .DefaultCellStyle.Font = New Font("Segoe UI", 10.0!)
            .DefaultCellStyle.ForeColor = Color.Black
            .RowsDefaultCellStyle.BackColor = Color.White
            .AlternatingRowsDefaultCellStyle.BackColor = Color.White
            .DefaultCellStyle.SelectionBackColor = Color.FromArgb(210, 222, 255)
            .DefaultCellStyle.SelectionForeColor = Color.Black
            .RowTemplate.Height = 34
            .RowTemplate.DefaultCellStyle.Padding = New Padding(0, 6, 0, 6)
        End With
    End Sub


    Private Sub combosignatories_SelectedIndexChanged(sender As Object, e As EventArgs) Handles combosignatories.SelectedIndexChanged
        If _isLoading Then Return
        LoadApproversForType(TryCast(combosignatories.Text, String).Trim())
    End Sub

    Private Sub LoadApproversForType(typeValue As String)
        Try
            Dim dt As New DataTable()
            Using conn As New SqlConnection(frmmain.txtdb.Text)
                conn.Open()
                Using da As New SqlDataAdapter("
                    SELECT ID, TYPE, CNAMES
                    FROM TBL_APPROVER
                    WHERE (@t = '' AND (TYPE IS NULL OR TYPE = '')) OR TYPE = @t
                    ORDER BY CNAMES;", conn)
                    da.SelectCommand.Parameters.Add("@t", SqlDbType.NVarChar, 100).Value = If(typeValue, "")
                    da.Fill(dt)
                End Using
            End Using
            DataGridView2.DataSource = dt

            _selectedId = -1
            If DataGridView2.Rows.Count > 0 Then
                DataGridView2.ClearSelection()
                DataGridView2.Rows(0).Selected = True
                SyncInputsFromCurrentRow()
            Else
                ClearInputs()
            End If

        Catch ex As Exception
            MessageBox.Show("Load Error: " & ex.Message, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub Grid_CellClick(sender As Object, e As DataGridViewCellEventArgs)
        If e.RowIndex < 0 Then Return
        SyncInputsFromCurrentRow()
    End Sub

    Private Sub Grid_KeyUp(sender As Object, e As KeyEventArgs)
        If e.KeyCode = Keys.Up OrElse e.KeyCode = Keys.Down Then
            SyncInputsFromCurrentRow()
        End If
    End Sub

    Private Sub SyncInputsFromCurrentRow()
        If DataGridView2.CurrentRow Is Nothing Then
            _selectedId = -1
            ClearInputs()
            Return
        End If

        Dim r = DataGridView2.CurrentRow
        Dim idObj = r.Cells("ID").Value
        If idObj Is Nothing OrElse Not Integer.TryParse(idObj.ToString(), _selectedId) Then
            _selectedId = -1
        End If

        Dim t As String = If(TryCast(r.Cells("TYPE").Value, String), "").Trim()
        Dim c As String = If(TryCast(r.Cells("CNAMES").Value, String), "").Trim()

        EnsureComboTypeSeeded()
        Dim idx = Array.IndexOf(_allowedTypes, t)
        If idx >= 0 Then
            combostype.SelectedIndex = idx
        Else
            combostype.SelectedIndex = -1
            combostype.Text = ""
        End If

        txtsnpos.Text = c
    End Sub


    Private Sub ClearInputs()
        EnsureComboTypeSeeded()
        combostype.SelectedIndex = -1
        combostype.Text = ""
        txtsnpos.Clear()
    End Sub

    ' ─────────────────────────────────────────────────────────────────────────────
    ' Load DISTINCT Types into a ComboBox
    ' ─────────────────────────────────────────────────────────────────────────────
    Private Sub BindSignatoryTypes(cbo As ComboBox)
        cbo.Items.Clear()

        Using conn As New SqlConnection(frmmain.txtdb.Text)
            conn.Open()
            Using cmd As New SqlCommand("
                SELECT DISTINCT TYPE
                FROM TBL_APPROVER
                WHERE TYPE IS NOT NULL AND LTRIM(RTRIM(TYPE)) <> ''
                ORDER BY TYPE;", conn)
                Using rdr = cmd.ExecuteReader()
                    While rdr.Read()
                        cbo.Items.Add(rdr("TYPE").ToString())
                    End While
                End Using
            End Using
        End Using
    End Sub

    Private Sub BindFilterTypes()
        combosignatories.DataSource = Nothing
        combosignatories.Items.Clear()

        Using conn As New SqlConnection(frmmain.txtdb.Text)
            conn.Open()
            Using cmd As New SqlCommand("
                SELECT DISTINCT TYPE
                FROM TBL_APPROVER
                WHERE TYPE IS NOT NULL AND LTRIM(RTRIM(TYPE)) <> ''
                ORDER BY TYPE;", conn)
                Using rdr = cmd.ExecuteReader()
                    While rdr.Read()
                        combosignatories.Items.Add(rdr("TYPE").ToString())
                    End While
                End Using
            End Using
        End Using

        ' Optional: ensure the 4 fixed types also appear as filters if DB is empty
        If combosignatories.Items.Count = 0 Then
            combosignatories.Items.AddRange(_allowedTypes)
        End If

        combosignatories.DropDownStyle = ComboBoxStyle.DropDownList
    End Sub


    Public Sub LoadUsers()
        Try
            Dim dt As New DataTable()

            Using conn As New SqlConnection(frmmain.txtdb.Text)
                conn.Open()

                Using cmd As New SqlCommand("
                SELECT ID,
                       Login_Name,
                       Login_LName,
                       Login_Office,
                       Login_Position,
                       Login_OPos,
                       Login_UserID,
                       Login_Email,
                       Login_Status
                FROM dbo.TBL_LOGIN
                WHERE ISNULL(Login_Position, '') <> @role
                ORDER BY Login_Name, Login_LName;", conn)

                    cmd.Parameters.Add("@role", SqlDbType.NVarChar, 50).Value = "Super-Admin"

                    Using da As New SqlDataAdapter(cmd)
                        da.Fill(dt)
                    End Using
                End Using
            End Using

            DataGridView1.DataSource = dt

        Catch ex As Exception
            MessageBox.Show(ex.ToString(), "Error Loading Users",
                        MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub Cmdnew_Click(sender As Object, e As EventArgs) Handles cmdnew.Click
        Try
            frmconfiguser.Dispose()
            frmconfiguser.lbltitle.Text = "Create New Account"
            frmconfiguser.ShowDialog()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub cmdEdit_Click(sender As Object, e As EventArgs) Handles cmdedit.Click
        ' 1) Grab the selected ID
        If DataGridView1.CurrentRow Is Nothing Then
            MessageBox.Show("Please select a user to edit.", "No Selection",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim row = DataGridView1.CurrentRow
        Dim userId = Convert.ToInt32(row.Cells("ID").Value)
        Dim fname = row.Cells("Login_Name").Value.ToString()
        Dim lname = row.Cells("Login_LName").Value.ToString()
        Dim office = row.Cells("Login_Office").Value.ToString()
        Dim pos = row.Cells("Login_Position").Value.ToString()
        Dim Opos = row.Cells("Login_OPos").Value.ToString()
        Dim uID = row.Cells("Login_UserID").Value.ToString()
        Dim email = row.Cells("Login_Email").Value.ToString()

        ' 2) **Create** a fresh form object
        Dim editForm As New frmconfiguser()

        ' 3) **Populate** its fields on *that* instance
        With editForm
            .lblid.Text = userId.ToString()
            .lblid.Text = userId.ToString()
            .txtposition.Text = Opos.ToString()
            .txtfname.Text = fname
            .txtlname.Text = lname
            .combooffsec.Text = office
            .comboula.Text = pos
            .txtuserid.Text = uID
            .txtemail.Text = email
            .lbltitle.Text = "Update Account"
            .txtpassword.Clear()     ' do not prefill passwords
            .txtrepassword.Clear()

            .cmdsave.Text = "Update"
        End With

        ' 4) Show *this* instance
        editForm.ShowDialog()

        ' 5) Refresh your user grid
        LoadUsers()
    End Sub

    Private Sub cmddelete_Click(sender As Object, e As EventArgs) Handles cmddelete.Click
        ' 1) Ensure a row is selected
        If DataGridView1.CurrentRow Is Nothing Then
            MessageBox.Show("Please select a user to delete.",
                            "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' 2) Get the selected user's ID and name
        Dim row = DataGridView1.CurrentRow
        Dim userId = Convert.ToInt32(row.Cells("ID").Value)
        Dim fullName = $"{row.Cells("Login_Name").Value} {row.Cells("Login_LName").Value}"

        ' 3) Ask for confirmation
        Dim resp = MessageBox.Show(
            $"Are you sure you want to delete the user ""{fullName}""? This cannot be undone.",
            "Confirm Deletion",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question)

        If resp <> DialogResult.Yes Then
            Return
        End If

        ' 4) Perform the delete
        Try
            Using conn As New SqlConnection(frmmain.txtdb.Text)
                conn.Open()
                Using cmd As New SqlCommand("DELETE FROM TBL_LOGIN WHERE ID = @id", conn)
                    cmd.Parameters.AddWithValue("@id", userId)
                    cmd.ExecuteNonQuery()
                End Using
            End Using

            MessageBox.Show("User deleted successfully.",
                            "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information)

            ' 5) Refresh the grid
            LoadUsers()

        Catch ex As Exception
            MessageBox.Show(ex.Message, "Error Deleting User",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub cmdblock_Click(sender As Object, e As EventArgs) Handles cmdblock.Click
        ' 1) Ensure a row is selected
        If DataGridView1.CurrentRow Is Nothing Then
            MessageBox.Show("Please select a user to block.", "No Selection",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' 2) Get the selected user's ID, name and current status
        Dim row = DataGridView1.CurrentRow
        Dim userId = Convert.ToInt32(row.Cells("ID").Value)
        Dim fullName = $"{row.Cells("Login_Name").Value} {row.Cells("Login_LName").Value}"
        Dim status = row.Cells("Login_Status").Value.ToString()

        ' 3) If already inactive, inform and exit
        If status.Equals("Inactive", StringComparison.OrdinalIgnoreCase) Then
            MessageBox.Show($"User '{fullName}' is already inactive.",
                        "Already Inactive", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        ' 4) Ask for confirmation, explaining what will happen
        Dim resp = MessageBox.Show(
        $"Are you sure you want to block user '{fullName}'? " &
        "This will set their account status to Inactive and prevent them from logging in.",
        "Confirm Block User",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Warning)

        If resp <> DialogResult.Yes Then
            Return
        End If

        ' 5) Perform the update to set Login_Status = 'Inactive'
        Try
            Using conn As New SqlConnection(frmmain.txtdb.Text)
                conn.Open()
                Using cmd As New SqlCommand(
                "UPDATE TBL_LOGIN SET Login_Status = 'Inactive' WHERE ID = @id", conn)
                    cmd.Parameters.AddWithValue("@id", userId)
                    cmd.ExecuteNonQuery()
                End Using
            End Using

            MessageBox.Show($"User '{fullName}' has been blocked and is now inactive.",
                        "User Blocked", MessageBoxButtons.OK, MessageBoxIcon.Information)

            ' 6) Refresh the grid
            LoadUsers()

        Catch ex As Exception
            MessageBox.Show(ex.Message, "Error Blocking User",
                        MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub cmdactivate_Click(sender As Object, e As EventArgs) Handles cmdactivate.Click
        ' 1) Ensure a row is selected
        If DataGridView1.CurrentRow Is Nothing Then
            MessageBox.Show("Please select a user to activate.", "No Selection",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' 2) Get the selected user's ID, name and current status
        Dim row = DataGridView1.CurrentRow
        Dim userId = Convert.ToInt32(row.Cells("ID").Value)
        Dim fullName = $"{row.Cells("Login_Name").Value} {row.Cells("Login_LName").Value}"
        Dim status = row.Cells("Login_Status").Value.ToString()

        ' 3) If already active, inform and exit
        If status.Equals("Active", StringComparison.OrdinalIgnoreCase) Then
            MessageBox.Show($"User '{fullName}' is already active.",
                            "Already Active", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        ' 4) Ask for confirmation, explaining what will happen
        Dim resp = MessageBox.Show(
            $"Are you sure you want to activate user '{fullName}'? " &
            "This will set their account status to Active and allow them to log in.",
            "Confirm Activate User",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question)

        If resp <> DialogResult.Yes Then
            Return
        End If

        ' 5) Perform the update to set Login_Status = 'Active'
        Try
            Using conn As New SqlConnection(frmmain.txtdb.Text)
                conn.Open()
                Using cmd As New SqlCommand(
                    "UPDATE TBL_LOGIN SET Login_Status = 'Active' WHERE ID = @id", conn)
                    cmd.Parameters.AddWithValue("@id", userId)
                    cmd.ExecuteNonQuery()
                End Using
            End Using

            MessageBox.Show($"User '{fullName}' has been activated and can now log in.",
                            "User Activated", MessageBoxButtons.OK, MessageBoxIcon.Information)

            ' 6) Refresh the grid
            LoadUsers()

        Catch ex As Exception
            MessageBox.Show(ex.Message, "Error Activating User",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub TextBox6_TextChanged(sender As Object, e As EventArgs)

    End Sub

    Private Sub TextBox6_KeyPress(sender As Object, e As KeyPressEventArgs)
        e.Handled = True
    End Sub

    Private Sub cmdsave_Click(sender As Object, e As EventArgs) Handles cmdsave.Click
        Try
            Using conn As New SqlClient.SqlConnection(My.Forms.frmmain.txtdb.Text)
                conn.Open()

                ' 1) Check if a config row already exists
                Dim existsSql = "SELECT COUNT(1) FROM TBL_CONFIG;"
                Dim existsCmd As New SqlClient.SqlCommand(existsSql, conn)
                Dim hasRow As Boolean = (Convert.ToInt32(existsCmd.ExecuteScalar()) > 0)

                If hasRow Then
                    ' 2a) UPDATE the existing row
                    Dim updateSql = "
UPDATE TBL_CONFIG
   SET MINISTER    = @minister,
       ACCOUNTANT  = @accountant,
       BACSEC      = @bacsec,
       MEM1        = @mem1,
       MEM2        = @mem2,
       MEM3        = @mem3,
       CHAIRPERSON = @chair,
       BACCHAIR    = @bacchair
 WHERE ID = (SELECT TOP 1 ID FROM TBL_CONFIG);"
                    Using cmd As New SqlClient.SqlCommand(updateSql, conn)
                        cmd.Parameters.Add("@minister", SqlDbType.NVarChar, 150).Value = If(String.IsNullOrWhiteSpace(txtminister.Text), DBNull.Value, txtminister.Text.Trim())
                        cmd.Parameters.Add("@accountant", SqlDbType.NVarChar, 150).Value = If(String.IsNullOrWhiteSpace(txtaccountant.Text), DBNull.Value, txtaccountant.Text.Trim())
                        cmd.Parameters.Add("@bacsec", SqlDbType.NVarChar, 150).Value = If(String.IsNullOrWhiteSpace(txtbacsec.Text), DBNull.Value, txtbacsec.Text.Trim())
                        cmd.Parameters.Add("@mem1", SqlDbType.NVarChar, 150).Value = If(String.IsNullOrWhiteSpace(txtmem1.Text), DBNull.Value, txtmem1.Text)
                        cmd.Parameters.Add("@mem2", SqlDbType.NVarChar, 150).Value = If(String.IsNullOrWhiteSpace(txtmem2.Text), DBNull.Value, txtmem2.Text)
                        cmd.Parameters.Add("@mem3", SqlDbType.NVarChar, 150).Value = If(String.IsNullOrWhiteSpace(txtmem3.Text), DBNull.Value, txtmem3.Text)
                        cmd.Parameters.Add("@chair", SqlDbType.NVarChar, 150).Value = If(String.IsNullOrWhiteSpace(txtchair.Text), DBNull.Value, txtchair.Text)
                        cmd.Parameters.Add("@bacchair", SqlDbType.NVarChar, 150).Value = If(String.IsNullOrWhiteSpace(txtbacchair.Text), DBNull.Value, txtbacchair.Text)
                        cmd.ExecuteNonQuery()
                    End Using
                Else
                    ' 2b) INSERT a new row
                    Dim insertSql = "
INSERT INTO TBL_CONFIG
 (MINISTER, ACCOUNTANT, BACSEC, MEM1, MEM2, MEM3, CHAIRPERSON, BACCHAIR)
VALUES
 (@minister, @accountant, @bacsec, @mem1, @mem2, @mem3, @chair, @bacchair);"
                    Using cmd As New SqlClient.SqlCommand(insertSql, conn)
                        cmd.Parameters.Add("@minister", SqlDbType.NVarChar, 150).Value = If(String.IsNullOrWhiteSpace(txtminister.Text), DBNull.Value, txtminister.Text.Trim())
                        cmd.Parameters.Add("@accountant", SqlDbType.NVarChar, 150).Value = If(String.IsNullOrWhiteSpace(txtaccountant.Text), DBNull.Value, txtaccountant.Text.Trim())
                        cmd.Parameters.Add("@bacsec", SqlDbType.NVarChar, 150).Value = If(String.IsNullOrWhiteSpace(txtbacsec.Text), DBNull.Value, txtbacsec.Text.Trim())
                        cmd.Parameters.Add("@mem1", SqlDbType.NVarChar, 150).Value = If(String.IsNullOrWhiteSpace(txtmem1.Text), DBNull.Value, txtmem1.Text)
                        cmd.Parameters.Add("@mem2", SqlDbType.NVarChar, 150).Value = If(String.IsNullOrWhiteSpace(txtmem2.Text), DBNull.Value, txtmem2.Text)
                        cmd.Parameters.Add("@mem3", SqlDbType.NVarChar, 150).Value = If(String.IsNullOrWhiteSpace(txtmem3.Text), DBNull.Value, txtmem3.Text)
                        cmd.Parameters.Add("@chair", SqlDbType.NVarChar, 150).Value = If(String.IsNullOrWhiteSpace(txtchair.Text), DBNull.Value, txtchair.Text)
                        cmd.Parameters.Add("@bacchair", SqlDbType.NVarChar, 150).Value = If(String.IsNullOrWhiteSpace(txtbacchair.Text), DBNull.Value, txtbacchair.Text)
                        cmd.ExecuteNonQuery()
                    End Using
                End If
            End Using

            MessageBox.Show("Configuration saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show("Error saving configuration: " & ex.ToString, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


    Private Sub Cmdclose_Click(sender As Object, e As EventArgs) Handles cmdclose.Click
        Me.Dispose()
    End Sub

    Private Sub TabPage2_Click(sender As Object, e As EventArgs) Handles TabPage2.Click

    End Sub
    Private Sub loadconfigs()
        Try
            Using conn As New SqlClient.SqlConnection(My.Forms.frmmain.txtdb.Text)
                conn.Open()

                ' Grab the single config row (if any)
                Const sql As String = "
SELECT TOP 1
       MINISTER,
       ACCOUNTANT,
       BACSEC,
       MEM1,
       MEM2,
       MEM3,
       CHAIRPERSON,
       BACCHAIR,
       SUPPLYOFF,
       ACTIVSTATUS
  FROM TBL_CONFIG;"

                Using cmd As New SqlClient.SqlCommand(sql, conn)
                    Using rdr = cmd.ExecuteReader()
                        If rdr.Read() Then
                            ' Load each field, allowing nulls
                            txtminister.Text = If(rdr.IsDBNull(0), "", rdr.GetString(0))
                            txtaccountant.Text = If(rdr.IsDBNull(1), "", rdr.GetString(1))
                            txtbacsec.Text = If(rdr.IsDBNull(2), "", rdr.GetString(2))
                            txtmem1.Text = If(rdr.IsDBNull(3), "", rdr.GetString(3))
                            txtmem2.Text = If(rdr.IsDBNull(4), "", rdr.GetString(4))
                            txtmem3.Text = If(rdr.IsDBNull(5), "", rdr.GetString(5))
                            txtchair.Text = If(rdr.IsDBNull(6), "", rdr.GetString(6))
                            txtsupply.Text = If(rdr.IsDBNull(8), "", rdr.GetString(8))
                            txtbacchair.Text = If(rdr.IsDBNull(7), "", rdr.GetString(7))

                        End If
                    End Using
                End Using
            End Using

        Catch ex As Exception
            MessageBox.Show("Error loading configuration: " & ex.Message,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub DataGridView2_SelectionChanged(sender As Object, e As EventArgs) Handles DataGridView2.SelectionChanged
        If DataGridView2.CurrentRow Is Nothing OrElse DataGridView2.CurrentRow.Index < 0 Then Return

        EnsureComboTypeSeeded()

        Dim t As String = If(TryCast(DataGridView2.CurrentRow.Cells("TYPE").Value, String), "").Trim()
        Dim c As String = If(TryCast(DataGridView2.CurrentRow.Cells("CNAMES").Value, String), "").Trim()

        Dim idx = Array.IndexOf(_allowedTypes, t)
        If idx >= 0 Then
            combostype.SelectedIndex = idx
        Else
            combostype.SelectedIndex = -1
            combostype.Text = ""
        End If

        txtsnpos.Text = c
    End Sub

    ' ─────────────────────────────────────────────────────────────────────────────
    ' NEW
    ' ─────────────────────────────────────────────────────────────────────────────
    Private Sub cmdsnew_Click(sender As Object, e As EventArgs) Handles cmdsnew.Click
        panelsignatories.Enabled = True
        cmdssave.Text = "Save"

        EnsureComboTypeSeeded()

        ' Prefill type with current filter if valid; else pick first
        Dim t = combosignatories.Text.Trim()
        Dim idx = Array.IndexOf(_allowedTypes, t)
        If idx >= 0 Then
            combostype.SelectedIndex = idx
        ElseIf combostype.Items.Count > 0 Then
            combostype.SelectedIndex = 0
        End If

        txtsnpos.Clear()
        txtsnpos.Focus()
        combostype.DroppedDown = True ' UX sugar
    End Sub

    ' ─────────────────────────────────────────────────────────────────────────────
    ' EDIT
    ' ─────────────────────────────────────────────────────────────────────────────
    Private Sub cmdsedit_Click(sender As Object, e As EventArgs) Handles cmdsedit.Click
        If _selectedId <= 0 OrElse DataGridView2.CurrentRow Is Nothing Then
            MessageBox.Show("Please select a row to edit.", "Info",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        panelsignatories.Enabled = True
        cmdssave.Text = "Update"

        EnsureComboTypeSeeded()
        ' Make sure combostype points to a valid allowed item
        Dim t = combostype.Text.Trim()
        Dim idx = Array.IndexOf(_allowedTypes, t)
        If idx >= 0 Then combostype.SelectedIndex = idx
        combostype.Focus()
    End Sub

    ' ─────────────────────────────────────────────────────────────────────────────
    ' SAVE (Insert/Update)
    ' ─────────────────────────────────────────────────────────────────────────────
    Private Sub cmdssave_Click(sender As Object, e As EventArgs) Handles cmdssave.Click
        Try
            EnsureComboTypeSeeded()

            Dim typ As String = combostype.Text.Trim()
            Dim cns As String = txtsnpos.Text.Trim()

            If Array.IndexOf(_allowedTypes, typ) < 0 Then
                MessageBox.Show("Please select a valid Type.", "Validation",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning)
                combostype.DroppedDown = True
                Return
            End If
            If String.IsNullOrWhiteSpace(cns) Then
                MessageBox.Show("Please enter Name and Position.", "Validation",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtsnpos.Focus() : Return
            End If

            Using conn As New SqlConnection(frmmain.txtdb.Text)
                conn.Open()
                Using tran = conn.BeginTransaction()

                    ' Duplicate guard (TYPE + CNAMES)
                    Using cmdDup As New SqlCommand("
                        SELECT COUNT(*) FROM TBL_APPROVER
                        WHERE TYPE = @t AND CNAMES = @c
                        AND (@isUpdate = 0 OR ID <> @id);", conn, tran)
                        cmdDup.Parameters.Add("@t", SqlDbType.NVarChar, 100).Value = typ
                        cmdDup.Parameters.Add("@c", SqlDbType.NVarChar, 500).Value = cns
                        cmdDup.Parameters.Add("@isUpdate", SqlDbType.Bit).Value =
                            If(String.Equals(cmdssave.Text, "Update", StringComparison.OrdinalIgnoreCase), 1, 0)
                        cmdDup.Parameters.Add("@id", SqlDbType.Int).Value = If(_selectedId > 0, _selectedId, 0)

                        If CInt(cmdDup.ExecuteScalar()) > 0 Then
                            tran.Rollback()
                            MessageBox.Show("Duplicate entry for this Type and Name/Position.", "Duplicate",
                                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            Return
                        End If
                    End Using

                    If String.Equals(cmdssave.Text, "Update", StringComparison.OrdinalIgnoreCase) Then
                        If _selectedId <= 0 Then
                            tran.Rollback()
                            MessageBox.Show("No row selected for update.", "Error",
                                            MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Return
                        End If

                        Using cmdUp As New SqlCommand("
                            UPDATE TBL_APPROVER
                               SET TYPE = @t, CNAMES = @c
                             WHERE ID = @id;", conn, tran)
                            cmdUp.Parameters.Add("@t", SqlDbType.NVarChar, 100).Value = typ
                            cmdUp.Parameters.Add("@c", SqlDbType.NVarChar, 500).Value = cns
                            cmdUp.Parameters.Add("@id", SqlDbType.Int).Value = _selectedId
                            cmdUp.ExecuteNonQuery()
                        End Using
                    Else
                        Using cmdIn As New SqlCommand("
                            INSERT INTO TBL_APPROVER (TYPE, CNAMES)
                            VALUES (@t, @c);", conn, tran)
                            cmdIn.Parameters.Add("@t", SqlDbType.NVarChar, 100).Value = typ
                            cmdIn.Parameters.Add("@c", SqlDbType.NVarChar, 500).Value = cns
                            cmdIn.ExecuteNonQuery()
                        End Using
                    End If

                    tran.Commit()
                End Using
            End Using

            ' Refresh filter and grid
            Dim keepFilter As String = combosignatories.Text
            BindFilterTypes()
            If combosignatories.Items.Contains(keepFilter) Then
                combosignatories.Text = keepFilter
            Else
                combosignatories.Text = typ
            End If
            LoadApproversForType(combosignatories.Text.Trim())

            ' Reset panel
            panelsignatories.Enabled = False
            ClearInputs()
            cmdssave.Text = "Save"

            MessageBox.Show("Record saved successfully.", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MessageBox.Show("Save Error: " & ex.Message, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ─────────────────────────────────────────────────────────────────────────────
    ' CANCEL
    ' ─────────────────────────────────────────────────────────────────────────────


    ' ─────────────────────────────────────────────────────────────────────────────
    ' DELETE
    ' ─────────────────────────────────────────────────────────────────────────────
    Private Sub cmdsdelete_Click(sender As Object, e As EventArgs) Handles cmdsdelete.Click
        Try
            If _selectedId <= 0 OrElse DataGridView2.CurrentRow Is Nothing Then
                MessageBox.Show("Please select a row to delete.", "Info",
                                MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If

            If MessageBox.Show("Delete selected signatory?", "Confirm",
                               MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then Return

            Using conn As New SqlConnection(frmmain.txtdb.Text)
                conn.Open()
                Using cmd As New SqlCommand("DELETE FROM TBL_APPROVER WHERE ID = @id;", conn)
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = _selectedId
                    cmd.ExecuteNonQuery()
                End Using
            End Using

            LoadApproversForType(combosignatories.Text.Trim())
            _selectedId = -1
            ClearInputs()

        Catch ex As Exception
            MessageBox.Show("Delete Error: " & ex.Message, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


    Private Sub Cmdsscancel_Click(sender As Object, e As EventArgs) Handles cmdsscancel.Click
        panelsignatories.Enabled = False
        ClearInputs()
        cmdssave.Text = "Save"
    End Sub

    Private Sub DataGridView2_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView2.CellContentClick

    End Sub

    Private Sub TabPage3_Click(sender As Object, e As EventArgs) Handles TabPage3.Click

    End Sub
End Class