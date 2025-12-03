Imports System.ComponentModel
Imports System.Data.SqlClient
Imports System.IO
Imports System.Net
Imports Microsoft.Reporting.WinForms
Public Class frmprfile
    ' Map Office/Section names → their codes
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

    Private Sub Frmprfile_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            ' ----------------------------------------------------------------------
            ' Base / reset
            ' ----------------------------------------------------------------------
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

                ' 🔲 No borders (same look as Frmpr)
                .BorderStyle = BorderStyle.None
                .CellBorderStyle = DataGridViewCellBorderStyle.None
                .ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
                .AdvancedCellBorderStyle.Left = DataGridViewAdvancedCellBorderStyle.None
                .AdvancedCellBorderStyle.Right = DataGridViewAdvancedCellBorderStyle.None
                .AdvancedCellBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.None
                .AdvancedCellBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.None
            End With

            ' ----------------------------------------------------------------------
            ' Columns  (kept as-is; only visual tweaks later)
            ' ----------------------------------------------------------------------
            DataGridView1.ColumnCount = 7

            With DataGridView1
                .Columns(0).Name = "ID"
                .Columns(0).HeaderText = "ID"
                .Columns(0).DataPropertyName = "ID"
                .Columns(0).Visible = False

                .Columns(1).Name = "PR_ITEMNO"
                .Columns(1).HeaderText = "ITEM NO."
                .Columns(1).DataPropertyName = "PR_ITEMNO"
                .Columns(1).Width = 55
                .Columns(1).SortMode = DataGridViewColumnSortMode.NotSortable
                .Columns(1).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

                .Columns(2).Name = "PR_UNIT"
                .Columns(2).HeaderText = "UNIT"
                .Columns(2).DataPropertyName = "PR_UNIT"
                .Columns(2).Width = 70
                .Columns(2).SortMode = DataGridViewColumnSortMode.NotSortable
                .Columns(2).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
                .Columns(2).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

                .Columns(3).Name = "PR_DESC"
                .Columns(3).HeaderText = "ITEM DESCRIPTION"
                .Columns(3).DataPropertyName = "PR_DESC"
                .Columns(3).Width = 420
                .Columns(3).SortMode = DataGridViewColumnSortMode.NotSortable
                .Columns(3).DefaultCellStyle.WrapMode = DataGridViewTriState.True

                .Columns(4).Name = "PR_QTY"
                .Columns(4).HeaderText = "QUANTITY"
                .Columns(4).DataPropertyName = "PR_QTY"
                .Columns(4).Width = 90
                .Columns(4).SortMode = DataGridViewColumnSortMode.NotSortable
                .Columns(4).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

                .Columns(5).Name = "PR_UCOST"
                .Columns(5).HeaderText = "UNIT COST"
                .Columns(5).DataPropertyName = "PR_UCOST"
                .Columns(5).Width = 120
                .Columns(5).DefaultCellStyle.Format = "N2"
                .Columns(5).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight
                .Columns(5).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                .Columns(5).SortMode = DataGridViewColumnSortMode.NotSortable


                .Columns(6).Name = "PR_TCOST"
                .Columns(6).HeaderText = "TOTAL COST"
                .Columns(6).DataPropertyName = "PR_TCOST"
                .Columns(6).Width = 140
                .Columns(6).DefaultCellStyle.Format = "N2"
                .Columns(6).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight
                .Columns(6).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                .Columns(6).SortMode = DataGridViewColumnSortMode.NotSortable
            End With

            ' ----------------------------------------------------------------------
            ' Styling (match the modern look of Frmpr)
            ' ----------------------------------------------------------------------
            Dim hdrColor As Color = Color.FromArgb(73, 94, 252)

            With DataGridView1
                .EnableHeadersVisualStyles = False

                ' Header
                .ColumnHeadersDefaultCellStyle.BackColor = hdrColor
                .ColumnHeadersDefaultCellStyle.ForeColor = Color.White
                .ColumnHeadersDefaultCellStyle.SelectionBackColor = hdrColor
                .ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White
                .ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 10.0!, FontStyle.Bold)
                .ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
                .ColumnHeadersHeight = 52
                .ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing

                ' Rows
                .DefaultCellStyle.Font = New Font("Segoe UI", 9.0!)
                .DefaultCellStyle.ForeColor = Color.Black
                .RowsDefaultCellStyle.BackColor = Color.White
                .AlternatingRowsDefaultCellStyle.BackColor = Color.White   ' no alt color
                .DefaultCellStyle.SelectionBackColor = Color.FromArgb(210, 222, 255)
                .DefaultCellStyle.SelectionForeColor = Color.Black

                ' Row spacing
                .RowTemplate.Height = 34
                .RowTemplate.DefaultCellStyle.Padding = New Padding(0, 6, 0, 6)
            End With

            ' Smooth painting
            EnableDoubleBuffering(DataGridView1)

            ' ----------------------------------------------------------------------
            ' Load data if needed
            ' ----------------------------------------------------------------------
            If lblid.Text <> "" Then
                LoadPRItems()
            End If
            LoadAppByNames()
            LoadCategory()

        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub
    ' ============================================================================

    ' (Reuse your existing helper)
    Private Sub EnableDoubleBuffering(dgv As DataGridView)
        Dim dgvType = dgv.GetType()
        Dim pi = dgvType.GetProperty("DoubleBuffered", Reflection.BindingFlags.Instance Or Reflection.BindingFlags.NonPublic)
        If pi IsNot Nothing Then pi.SetValue(dgv, True, Nothing)
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles cmdadd.Click
        Try
            frmpradd.Dispose()
            ' frmpradd.LoadUnits()
            frmpradd.ShowDialog()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub Cmdsave_Click(sender As Object, e As EventArgs) Handles cmdsave.Click
        Try
            If lblid.Text = Nothing Then
                If combotype.Text = "" Or comborcenter.Text = "" Or combofcluster.Text = "" Or combocategory.Text = "" Then
                    MsgBox("Fields marked with • are required.", vbExclamation, "Message")
                Else
                    createnewpr()
                End If

            Else
                    updatepr()
            End If

        Catch ex As Exception

        End Try
    End Sub


    Public Sub GeneratePRNo(newID As Integer)
        Try
            ' ─────────────────────────────────────────────────────────
            ' 0) Resolve FCODE from Fund Cluster + Receiving Center
            ' ─────────────────────────────────────────────────────────
            Dim fcluster As String = If(combofcluster IsNot Nothing, combofcluster.Text.Trim(), "")
            Dim FCODE As String = ""

            Select Case fcluster
                Case "Regular Funds"
                    FCODE = If(comborcenter IsNot Nothing, comborcenter.Text.Trim(), "")
                Case "Special Development Fund"
                    FCODE = "MPOS-SDF"
                Case "Transitional Development Impact Fund"
                    FCODE = "MPOS-TDIF"
                Case "Supplemental Fund"
                    FCODE = "MPOS-SF"
                Case Else
                    MessageBox.Show("Please select a valid Fund Cluster.", "Generate PR No",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Sub
            End Select

            If String.IsNullOrWhiteSpace(FCODE) Then
                MessageBox.Show("Please select a valid Receiving Center / FCODE.", "Generate PR No",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            ' ─────────────────────────────────────────────────────────
            ' 1) Use dtdate.Value for year & month (NOT system time)
            ' ─────────────────────────────────────────────────────────
            Dim d As DateTime = dtdate.Value
            Dim currentYear As String = d.ToString("yyyy")
            Dim currentMonth As String = d.ToString("MM")

            ' ─────────────────────────────────────────────────────────
            ' 2) Compute next sequence per (FCODE, Year) under SERIALIZABLE
            '    Series increments across months; resets only when year changes.
            ' ─────────────────────────────────────────────────────────
            Dim newPRNO As String

            Using conn As New SqlConnection(frmmain.txtdb.Text)
                conn.Open()

                Using tx As SqlTransaction = conn.BeginTransaction(IsolationLevel.Serializable)
                    Try
                        ' Prefix excludes month to allow continuous series across months
                        ' PRNO format is FCODE-YYYY-MM-SSSS → we match FCODE-YYYY-
                        Dim prefix As String = FCODE & "-" & currentYear & "-"

                        Dim sqlGet As String =
                        "SELECT ISNULL(MAX(CAST(RIGHT(PRNO, 4) AS INT)), 0) " &
                        "FROM TBL_PR WITH (UPDLOCK, HOLDLOCK) " &
                        "WHERE PRNO LIKE @Prefix + '%'"

                        Dim nextSeq As Integer
                        Using cmdGet As New SqlCommand(sqlGet, conn, tx)
                            cmdGet.Parameters.Add("@Prefix", SqlDbType.NVarChar, 200).Value = prefix
                            Dim currMax As Object = cmdGet.ExecuteScalar()
                            nextSeq = Convert.ToInt32(currMax) + 1
                        End Using

                        newPRNO = $"{FCODE}-{currentYear}-{currentMonth}-{nextSeq:D4}"

                        Dim sqlUpd As String =
                        "UPDATE TBL_PR SET PRNO = @PRNO " &
                        "WHERE ID = @ID;"

                        Using cmdUpd As New SqlCommand(sqlUpd, conn, tx)
                            cmdUpd.Parameters.Add("@PRNO", SqlDbType.NVarChar, 100).Value = newPRNO
                            cmdUpd.Parameters.Add("@ID", SqlDbType.Int).Value = newID
                            cmdUpd.ExecuteNonQuery()
                        End Using

                        tx.Commit()
                    Catch
                        Try : tx.Rollback() : Catch : End Try
                        Throw
                    End Try
                End Using
            End Using

            ' ─────────────────────────────────────────────────────────
            ' 3) UI + notification
            ' ─────────────────────────────────────────────────────────
            txtprno.Text = newPRNO
            NotificationHelper.AddNotification($"{frmmain.lblaname.Text.Trim()} created Purchase Request Number {newPRNO}.")

            cmdadd.Enabled = True
            cmdaddapp.Enabled = True
            cmddelete.Enabled = True
            cmdedit.Enabled = True
            cmdprint.Enabled = True

        Catch ex As Exception
            MessageBox.Show("Failed to generate PRNO: " & ex.Message,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub





    Public Sub createnewpr()
        Try
            If txtreqby.Text.ToString = "" Then
                txtreqby.Text = frmmain.lblaname.Text & " - " & frmmain.lblposition.Text
            Else

            End If


            Dim insertQuery As String = "INSERT INTO TBL_PR (PRDATE,PTTYPE, PRTIME, PRSTATUS, ENAMES, FCLUSTER, OFFSEC, RCCODE, PRPURPOSE, REQBY, APPBY, CATEGORY) " &
                            "VALUES (@PRDATE, @PTTYPE, @PRTIME, @PRSTATUS, @ENAMES, @FCLUSTER, @OFFSEC, @RCCODE, @PRPURPOSE, @REQBY, @APPBY, @CATEGORY); " &
                            "SELECT SCOPE_IDENTITY();"

            Dim newID As Integer = 0

            Using conn As New SqlConnection(frmmain.txtdb.Text)
                conn.Open()
                Using cmd As New SqlCommand(insertQuery, conn)
                    cmd.Parameters.AddWithValue("@PRDATE", dtdate.Value.Date)
                    cmd.Parameters.AddWithValue("@PTTYPE", combotype.Text)
                    cmd.Parameters.AddWithValue("@PRTIME", DateTime.Now.ToString("hh:mm tt"))
                    cmd.Parameters.AddWithValue("@PRSTATUS", "Pending")
                    cmd.Parameters.AddWithValue("@ENAMES", txtename.Text)
                    cmd.Parameters.AddWithValue("@FCLUSTER", combofcluster.Text)
                    cmd.Parameters.AddWithValue("@OFFSEC", combooffsec.Text)
                    cmd.Parameters.AddWithValue("@RCCODE", comborcenter.Text)
                    cmd.Parameters.AddWithValue("@PRPURPOSE", txtpurpose.Text)
                    cmd.Parameters.AddWithValue("@REQBY", txtreqby.Text)
                    cmd.Parameters.AddWithValue("@APPBY", txtappby.Text)
                    cmd.Parameters.AddWithValue("@CATEGORY", combocategory.Text)
                    newID = Convert.ToInt32(cmd.ExecuteScalar())
                End Using
                conn.Close()
            End Using

            lblid.Text = newID
            'dtdate.Text = DateTime.Now.Date
            'txttime.Text = DateTime.Now.ToString("hh:mm tt")
            txtstatus.Text = "Pending"
            GeneratePRNo(newID)
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try

    End Sub

    Public Sub updatepr()
        Try
            ' ✅ Validate ID before continuing
            If Not Integer.TryParse(lblid.Text, Nothing) Then
                MessageBox.Show("Invalid record ID.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            Dim PRID As Integer = Convert.ToInt32(lblid.Text)

            Using conn As New SqlConnection(frmmain.txtdb.Text)
                conn.Open()

                Dim query As String = "
                UPDATE TBL_PR SET 
                    ENAMES = @ENAMES,
                    PTTYPE = @PTTYPE,
                    FCLUSTER = @FCLUSTER,
                    OFFSEC = @OFFSEC,
                    RCCODE = @RCCODE,
                    PRPURPOSE = @PRPURPOSE,
                    REQBY = @REQBY,
                    APPBY = @APPBY,
                    CATEGORY= @CATEGORY
                WHERE ID = @ID
            "

                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@ENAMES", txtename.Text.Trim())
                    cmd.Parameters.AddWithValue("@PTTYPE", combotype.Text)
                    cmd.Parameters.AddWithValue("@FCLUSTER", combofcluster.Text.Trim())
                    cmd.Parameters.AddWithValue("@OFFSEC", combooffsec.Text.Trim())
                    cmd.Parameters.AddWithValue("@RCCODE", comborcenter.Text.Trim())
                    cmd.Parameters.AddWithValue("@PRPURPOSE", txtpurpose.Text.Trim())
                    cmd.Parameters.AddWithValue("@REQBY", txtreqby.Text.Trim())
                    cmd.Parameters.AddWithValue("@APPBY", txtappby.Text.Trim())
                    cmd.Parameters.AddWithValue("@CATEGORY", combocategory.Text.Trim())
                    cmd.Parameters.AddWithValue("@ID", PRID)

                    Dim rowsAffected As Integer = cmd.ExecuteNonQuery()

                    If rowsAffected > 0 Then
                        MessageBox.Show("PR updated successfully!", "Update Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        frmpr.loadrecords()
                    Else
                        MessageBox.Show("Record not found. Update failed.", "Update Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End If
                End Using

                conn.Close()
            End Using

        Catch ex As Exception
            MessageBox.Show("Error during update: " & ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Public Sub LoadPRData(ByVal PRID As Integer)
        Try
            Using conn As New SqlConnection(frmmain.txtdb.Text)
                conn.Open()

                Dim query As String = "SELECT * FROM TBL_PR WHERE ID = @ID"
                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@ID", PRID)

                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        If reader.Read() Then
                            '— ID and PRNO —
                            lblid.Text = reader("ID").ToString()
                            combotype.Text = reader("PTTYPE").ToString()
                            txtprno.Text = reader("PRNO").ToString()

                            '— Date & Time —
                            dtdate.Value = Format(reader("PRDATE"), ("MM/dd/yyyy"))
                            txttime.Text = Format(DateTime.Today.Add(reader.GetTimeSpan(reader.GetOrdinal("PRTIME"))), "hh:mm tt")


                            '— Status —
                            txtstatus.Text = reader("PRSTATUS").ToString()

                            '— Header details —
                            txtename.Text = reader("ENAMES").ToString()
                            combofcluster.Text = reader("FCLUSTER").ToString()
                            combooffsec.Text = reader("OFFSEC").ToString()
                            comborcenter.Text = reader("RCCODE").ToString()
                            txtpurpose.Text = reader("PRPURPOSE").ToString()
                            combocategory.Text = reader("CATEGORY").ToString()

                            '— Request / Approval —
                            txtreqby.Text = reader("REQBY").ToString()
                            txtappby.Text = reader("APPBY").ToString()

                            'To not allow edit on approval status

                            If txtstatus.Text = "[P.R] Approved" Then
                                cmdsubmit.Visible = False
                            ElseIf txtstatus.text = "[P.R] For Approval" Then
                                cmdsubmit.Visible = True


                            End If

                        Else
                            MessageBox.Show("PR record not found.", "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        End If
                    End Using
                End Using
                conn.Close()


            End Using




            If txtstatus.Text = "Pending" Then
                ' Optional: refresh list / lock controls
                cmdadd.Enabled = True
                cmdaddapp.Enabled = True
                cmdedit.Enabled = True
                cmddelete.Enabled = True
                cmdsave.Enabled = True
                cmdsubmit.Text = "Submit for Approval"
                cmdsubmit.BackColor = Color.Honeydew
                dtdate.Enabled = False

            Else
                cmdadd.Enabled = False
                cmdaddapp.Enabled = False
                cmdedit.Enabled = False
                cmddelete.Enabled = False
                cmdsave.Enabled = False
                cmdsubmit.Text = "Cancel Approval"
                cmdsubmit.BackColor = Color.Snow
                dtdate.Enabled = False
            End If

        Catch ex As Exception
            MessageBox.Show("Failed to load PR data: " & ex.ToString, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Public Sub LoadPRItems()
        ' 1) Parse the PRID
        Dim prId As Integer
        If Not Integer.TryParse(lblid.Text, prId) Then Return

        Try
            Using conn As New SqlConnection(frmmain.txtdb.Text)
                conn.Open()

                Dim sql As String = "
                SELECT ID,
                       PR_ITEMNO AS [PR_ITEMNO],
                       PR_UNIT   AS [PR_UNIT],
                       PR_DESC   AS [PR_DESC],
                       PR_QTY    AS [PR_QTY],
                       PR_UCOST  AS [PR_UCOST],
                       PR_TCOST  AS [PR_TCOST]
                FROM TBL_PRITEMS
                WHERE PRID = @PRID
                ORDER BY PR_ITEMNO;"

                Using da As New SqlDataAdapter(sql, conn)
                    da.SelectCommand.Parameters.AddWithValue("@PRID", prId)
                    Dim dt As New DataTable()
                    da.Fill(dt)

                    ' 3) Bind grid
                    DataGridView1.AutoGenerateColumns = False
                    DataGridView1.DataSource = dt

                    ' 4) Sum PR_TCOST
                    Dim total As Decimal = 0D
                    If dt.Rows.Count > 0 Then
                        ' Safe compute (handles DBNull)
                        total = Convert.ToDecimal(
                        dt.Compute("SUM(PR_TCOST)", "PR_TCOST IS NOT NULL"))
                    End If

                    ' 5) Show in label, format ##,###.##
                    lbltotal.Text = "SUB TOTAL: " & total.ToString("#,##0.00")
                    lbltotal.Tag = total   ' keep raw value if you need it later
                End Using
            End Using

        Catch ex As Exception
            MessageBox.Show("Failed to load PR items: " & ex.Message,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


    Private Sub txtprno_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtprno.KeyPress
        e.Handled = True
    End Sub

    Private Sub txtdate_KeyPress(sender As Object, e As KeyPressEventArgs)
        e.Handled = True
    End Sub

    Private Sub txttime_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txttime.KeyPress
        e.Handled = True
    End Sub

    Private Sub txtstatus_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtstatus.KeyPress
        e.Handled = True
    End Sub

    Private Sub Cmddelete_Click(sender As Object, e As EventArgs) Handles cmddelete.Click
        Try
            DeletePRItem()
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub

    Public Sub DeletePRItem()
        ' 1) Ensure at least one row is selected
        If DataGridView1.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select one or more items to delete.", "Delete Item", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' 2) Gather selected IDs and line‐numbers
        Dim ids As New List(Of Integer)
        Dim lines As New List(Of String)
        For Each row As DataGridViewRow In DataGridView1.SelectedRows
            Dim rawId = row.Cells("ID").Value
            Dim rawLine = row.Cells("PR_DESC").Value
            Dim idVal As Integer
            If rawId IsNot Nothing AndAlso Integer.TryParse(rawId.ToString(), idVal) Then
                ids.Add(idVal)
                lines.Add(rawLine?.ToString())
            End If
        Next

        If ids.Count = 0 Then
            MessageBox.Show("No valid items selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        ' 3) Confirm deletion listing line‐numbers
        Dim msg = $"Delete this item(s)?{vbCrLf}{String.Join(", ", lines)}"
        If MessageBox.Show(msg, "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
            Return
        End If

        ' 4) Get parent PR ID
        Dim prId As Integer
        If Not Integer.TryParse(lblid.Text, prId) Then
            MessageBox.Show("Invalid PR ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        ' 5) Delete & resequence in one transaction
        Try
            Using conn As New SqlConnection(frmmain.txtdb.Text)
                conn.Open()
                Using tran = conn.BeginTransaction()
                    ' a) Delete each selected item
                    Using cmdDel As New SqlCommand("DELETE FROM TBL_PRITEMS WHERE ID = @ID", conn, tran)
                        cmdDel.Parameters.Add("@ID", SqlDbType.Int)
                        For Each idVal In ids
                            cmdDel.Parameters("@ID").Value = idVal
                            cmdDel.ExecuteNonQuery()
                        Next
                    End Using

                    ' b) Resequence remaining items
                    Using cmdSeq As New SqlCommand("
                    WITH Numbered AS (
                      SELECT ID,
                             ROW_NUMBER() OVER (ORDER BY PR_ITEMNO) AS NewNo
                      FROM TBL_PRITEMS
                      WHERE PRID = @PRID
                    )
                    UPDATE T
                    SET PR_ITEMNO = N.NewNo
                    FROM TBL_PRITEMS AS T
                    INNER JOIN Numbered AS N ON T.ID = N.ID;
                ", conn, tran)
                        cmdSeq.Parameters.AddWithValue("@PRID", prId)
                        cmdSeq.ExecuteNonQuery()
                    End Using

                    tran.Commit()
                End Using
            End Using

            ' 6) Refresh UI
            LoadPRItems()
            frmpradd.UpdatePRTotals(prId)
            MessageBox.Show("Selected item(s) deleted and sequence updated.", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MessageBox.Show("Error deleting items: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub Cmdedit_Click(sender As Object, e As EventArgs) Handles cmdedit.Click
        Try
            frmpradd.Dispose()
            ' 1) Make sure a row is selected
            If DataGridView1.CurrentRow Is Nothing Then
                MessageBox.Show("Please select an item to edit.", "Edit Item", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            ' 2) Read the item ID from the hidden ID column
            Dim rawId = DataGridView1.CurrentRow.Cells("ID").Value
            Dim itemId As Integer
            If rawId Is Nothing OrElse Not Integer.TryParse(rawId.ToString(), itemId) Then
                MessageBox.Show("Could not determine item ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            ' 3) Tell frmpradd to load that item

            Using prfileadd As New frmpradd()
                ' prfileadd.LoadUnits()
                prfileadd.LoadItemData(itemId)

                ' 4) Show the add/edit form
                prfileadd.ShowDialog()
            End Using
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub

    Private Sub combooffsec_SelectedIndexChanged(sender As Object, e As EventArgs) _
        Handles combooffsec.SelectedIndexChanged

        Dim selectedOffice As String = combooffsec.SelectedItem?.ToString()
        Dim code As String = ""

        ' Look up the code (will remain empty if not found)
        If selectedOffice IsNot Nothing AndAlso officeCodes.TryGetValue(selectedOffice, code) Then
            comborcenter.Text = code
        Else
            comborcenter.Text = ""
        End If
    End Sub

    Private Sub frmprfile_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        cmdclose.PerformClick()

    End Sub

    Private Sub Cmdclose_Click(sender As Object, e As EventArgs) Handles cmdclose.Click
        Me.Dispose()
    End Sub

    Private Sub Cmdprint_Click(sender As Object, e As EventArgs) Handles cmdprint.Click
        Try
            ' 1) Validate PR ID
            Dim prId As Integer
            If Not Integer.TryParse(lblid.Text, prId) Then
                MessageBox.Show("Invalid PR selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            ' 2) Load details
            Dim dt As New DataTable()
            Using conn As New SqlConnection(frmmain.txtdb.Text)
                conn.Open()
                Const sql As String = "
                SELECT PR_ITEMNO, PR_UNIT, PR_DESC, PR_QTY, PR_UCOST, PR_TCOST
                FROM TBL_PRITEMS
                WHERE PRID = @PRID
                ORDER BY PR_ITEMNO;"
                Using da As New SqlDataAdapter(sql, conn)
                    da.SelectCommand.Parameters.AddWithValue("@PRID", prId)
                    da.Fill(dt)
                End Using
            End Using

            ' 3) Prepare parameters (REQBY/APPBY as HTML with <br/>)
            Dim reqHtml As String = FormatSignatureForHtml(txtreqby.Text)
            Dim appHtml As String = FormatSignatureForHtml(txtappby.Text)

            Dim parms As New List(Of ReportParameter) From {
            New ReportParameter("PRNO", txtprno.Text.Trim()),
            New ReportParameter("ENAME", txtename.Text.Trim()),
            New ReportParameter("FCLUSTER", combofcluster.Text.Trim()),
            New ReportParameter("OFFSEC", combooffsec.Text.Trim()),
            New ReportParameter("RESCODE", comborcenter.Text.Trim()),
            New ReportParameter("DATES", dtdate.Value.ToString("MM/dd/yyyy")),
            New ReportParameter("PURPOSE", txtpurpose.Text.Trim()),
            New ReportParameter("REQBY", reqHtml),
            New ReportParameter("APPBY", appHtml)
        }

            ' 4) Show the preview
            Using preview As New frmpprev()
                With preview.ReportViewer1
                    .ProcessingMode = ProcessingMode.Local
                    .LocalReport.ReportPath = Path.Combine(Application.StartupPath, "Report", "rptpr.rdlc")
                    .LocalReport.DataSources.Clear()
                    .LocalReport.DataSources.Add(New ReportDataSource("DSPR", dt))
                    .LocalReport.SetParameters(parms)
                    .SetDisplayMode(DisplayMode.PrintLayout)
                    .ZoomMode = ZoomMode.Percent
                    .RefreshReport()
                End With

                preview.lbltransaction.Text = "PR"
                preview.panelsubmit.Visible = (txtstatus.Text = "Pending")
                preview.ShowDialog()
            End Using

        Catch ex As Exception
            MessageBox.Show(ex.ToString(), "Print Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ─────────────────────────────────────────────────────────────────────────
    ' Helpers
    ' ─────────────────────────────────────────────────────────────────────────

    ''' <summary>
    ''' Converts "Name - Position" to "Name&lt;br/&gt;Position" for HTML RDLC params.
    ''' Also converts any CR/LF to &lt;br/&gt; and HTML-encodes safely.
    ''' </summary>
    Private Function FormatSignatureForHtml(input As String) As String
        If input Is Nothing Then Return ""

        Dim t As String = input.Trim()

        ' Normalize CRLFs
        t = t.Replace(vbCrLf, vbLf).Replace(vbCr, vbLf)

        ' If " - " separator exists and no line breaks, split it
        Dim parts() As String
        If t.Contains(" - ") Then
            parts = t.Split(New String() {" - "}, 2, StringSplitOptions.None)
        Else
            parts = t.Split({vbLf}, 2, StringSplitOptions.None)
        End If

        Dim namePart As String = WebUtility.HtmlEncode(parts(0).Trim())
        Dim posPart As String = If(parts.Length > 1, WebUtility.HtmlEncode(parts(1).Trim()), "")

        If String.IsNullOrEmpty(posPart) Then
            ' Only name, bold it
            Return $"<b>{namePart}</b>"
        Else
            ' Name bold + line break + position normal
            Return $"<b>{namePart}</b><br/>{posPart}"
        End If
    End Function

    ''' <summary>
    ''' HTML-encodes text using System.Web when available; falls back to a minimal encoder.
    ''' </summary>
    ''' 
    Private Function SafeHtmlEncode(s As String) As String
        Return WebUtility.HtmlEncode(s)
    End Function



    Private Sub cmdsubmit_Click(sender As Object, e As EventArgs) Handles cmdsubmit.Click

        If lblid.Text = "" Then
            MessageBox.Show("No PR selected.", "Submit", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        Dim ask = MessageBox.Show(
        "This Purchase Request will be removed from Approval list and changes Status to Pending." &
        vbCrLf & vbCrLf & "Do you want to continue?",
        "Cancel Approval",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Question)

        If ask <> DialogResult.Yes Then Exit Sub

        Try
            Using conn As New SqlClient.SqlConnection(frmmain.txtdb.Text)
                conn.Open()

                ' ────────── DELETE matching approval row ──────────
                Const deleteSql As String = "
                DELETE FROM TBL_APPROVAL
                 WHERE PRID = @prid
                   AND APPROVAL_TYPE = @type;"
                Using delCmd As New SqlClient.SqlCommand(deleteSql, conn)
                    delCmd.Parameters.AddWithValue("@prid", Convert.ToInt32(lblid.Text))
                    delCmd.Parameters.AddWithValue("@type", "Purchase Request")
                    delCmd.ExecuteNonQuery()
                End Using

                ' ────────── UPDATE Purchase Request status ──────────
                Const updateSql As String = "
                UPDATE TBL_PR
                   SET PRSTATUS = 'Pending'
                 WHERE ID = @id;"
                Using updCmd As New SqlClient.SqlCommand(updateSql, conn)
                    updCmd.Parameters.AddWithValue("@id", Convert.ToInt32(lblid.Text))
                    updCmd.ExecuteNonQuery()
                End Using
            End Using

            MessageBox.Show("Approval request has been cancelled.", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information)

            ' Optional: refresh UI
            Me.cmdadd.Enabled = True
            Me.cmdaddapp.Enabled = True
            Me.cmdedit.Enabled = True
            Me.cmddelete.Enabled = True
            Me.cmdsave.Enabled = True
            Me.txtstatus.Text = "Pending"
            Me.cmdsubmit.Visible = False

            frmpr.loadrecords()

            NotificationHelper.AddNotification(
            $"Purchase Request for PR Number {txtprno.Text.Trim()} has cancelled the approval request by {frmmain.lblaname.Text.Trim()}.")

        Catch ex As Exception
            MessageBox.Show("Error cancelling approval: " & ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub


    Private Sub LoadAppByNames()
        Dim connStr As String = My.Forms.frmmain.txtdb.Text ' Or your connection string
        Using conn As New SqlConnection(connStr)
            conn.Open()
            Dim sql As String = "SELECT CNAMES FROM TBL_APPROVER WHERE TYPE = 'PR: Signatories'"
            Using cmd As New SqlCommand(sql, conn)
                Using rdr As SqlDataReader = cmd.ExecuteReader()
                    txtappby.Items.Clear()
                    While rdr.Read()
                        Dim raw As String = rdr("CNAMES").ToString()
                        Dim display As String = raw.Replace(vbCrLf, " - ") ' or " | "
                        txtappby.Items.Add(display)
                    End While
                End Using
            End Using
        End Using
    End Sub


    Private Sub LoadCategory()
        Dim connStr As String = My.Forms.frmmain.txtdb.Text ' Or your connection string
        Using conn As New SqlConnection(connStr)
            conn.Open()
            Dim sql As String = "SELECT CNAMES FROM TBL_APPROVER WHERE TYPE = 'PR: Category'"
            Using cmd As New SqlCommand(sql, conn)
                Using rdr As SqlDataReader = cmd.ExecuteReader()
                    combocategory.Items.Clear()
                    While rdr.Read()
                        Dim raw As String = rdr("CNAMES").ToString()
                        Dim display As String = raw.Replace(vbCrLf, " - ") ' or " | "
                        combocategory.Items.Add(display)
                    End While
                End Using
            End Using
        End Using
    End Sub

    Private Sub combooffsec_KeyPress(sender As Object, e As KeyPressEventArgs) Handles combooffsec.KeyPress
        e.Handled = True
    End Sub

    Private Sub combofcluster_KeyPress(sender As Object, e As KeyPressEventArgs) Handles combofcluster.KeyPress
        e.Handled = True
    End Sub

    Private Sub combotype_KeyPress(sender As Object, e As KeyPressEventArgs) Handles combotype.KeyPress
        e.Handled = True
    End Sub


    Private Sub comborcenter_KeyPress(sender As Object, e As KeyPressEventArgs) Handles comborcenter.KeyPress
        e.Handled = True
    End Sub

    Private Sub Txtappby_SelectedIndexChanged(sender As Object, e As EventArgs) Handles txtappby.SelectedIndexChanged

    End Sub

    Private Sub txtappby_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtappby.KeyPress
        e.Handled = True
    End Sub

    Private Sub Combocategory_SelectedIndexChanged(sender As Object, e As EventArgs) Handles combocategory.SelectedIndexChanged

    End Sub

    Private Sub combocategory_KeyPress(sender As Object, e As KeyPressEventArgs) Handles combocategory.KeyPress
        e.Handled = True
    End Sub
End Class