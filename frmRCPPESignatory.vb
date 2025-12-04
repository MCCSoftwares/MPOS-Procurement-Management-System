Imports System.Data
Imports System.Data.SqlClient

Public Class FRMRCPPESignatory

    ' Single-row config ID
    Private _currentId As Integer = 0

    ' CNAMES (full text) → parsed POSITION (if any, from the same string)
    Private _employeePositions As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)

    '────────────────────────────────────────────────────────
    '  FORM LOAD
    '────────────────────────────────────────────────────────
    Private Sub FRMRCPPESignatory_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            LoadCombosFromApprover()
            LoadExistingSignatorySetup()
        Catch ex As Exception
            MessageBox.Show("Error initializing signatory form:" & Environment.NewLine & ex.Message,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    '────────────────────────────────────────────────────────
    '  LOAD COMBO LISTS FROM TBL_APPROVER
    '────────────────────────────────────────────────────────
    Private Sub LoadCombosFromApprover()
        Dim dtInventoryCommittee As New DataTable()
        Dim dtEmployees As New DataTable()
        _employeePositions.Clear()

        Using conn As New SqlConnection(frmmain.txtdb.Text)
            conn.Open()

            ' Inventory Committee (for Sig1–Sig10) → CNAMES only
            Const sqlCommittee As String =
                "SELECT CNAMES " &
                "FROM TBL_APPROVER WITH (NOLOCK) " &
                "WHERE TYPE = @TYPE " &
                "ORDER BY CNAMES;"

            Using da As New SqlDataAdapter(sqlCommittee, conn)
                da.SelectCommand.Parameters.Add("@TYPE", SqlDbType.NVarChar, 50).Value = "Inventory Committee"
                da.Fill(dtInventoryCommittee)
            End Using

            ' Employees (for Approved_by) → CNAMES
            Const sqlEmployee As String =
                "SELECT CNAMES " &
                "FROM TBL_APPROVER WITH (NOLOCK) " &
                "WHERE TYPE = @TYPE " &
                "ORDER BY CNAMES;"

            Using da As New SqlDataAdapter(sqlEmployee, conn)
                da.SelectCommand.Parameters.Add("@TYPE", SqlDbType.NVarChar, 50).Value = "Employee"
                da.Fill(dtEmployees)
            End Using
        End Using

        ' Helper array for Sig1–Sig10 combos
        Dim allSigCombos As ComboBox() = {
            combosig1, combosig2, combosig3, combosig4, combosig5,
            combosig6, combosig7, combosig8, combosig9, combosig10
        }

        ' Bind Inventory Committee list → CNAMES only
        For Each cbo As ComboBox In allSigCombos
            cbo.DataSource = Nothing
            cbo.Items.Clear()
            cbo.AutoCompleteMode = AutoCompleteMode.SuggestAppend
            cbo.AutoCompleteSource = AutoCompleteSource.ListItems

            cbo.DisplayMember = "CNAMES"
            cbo.ValueMember = "CNAMES"
            cbo.DataSource = dtInventoryCommittee.Copy()
            cbo.SelectedIndex = -1
            cbo.Text = String.Empty
        Next

        ' Approved_by combo (Employees)
        comboapproved.DataSource = Nothing
        comboapproved.Items.Clear()
        comboapproved.AutoCompleteMode = AutoCompleteMode.SuggestAppend
        comboapproved.AutoCompleteSource = AutoCompleteSource.ListItems
        comboapproved.DisplayMember = "CNAMES"
        comboapproved.ValueMember = "CNAMES"
        comboapproved.DataSource = dtEmployees
        comboapproved.SelectedIndex = -1
        comboapproved.Text = String.Empty

        ' Build dictionary CNAMES → POSITION (parsed if string has newline)
        For Each row As DataRow In dtEmployees.Rows
            Dim fullText As String = Convert.ToString(row("CNAMES")).Trim()
            If String.IsNullOrEmpty(fullText) Then Continue For

            Dim posPart As String = String.Empty
            Dim parts As String() = fullText.Split(New String() {vbCrLf, vbLf}, StringSplitOptions.None)
            If parts.Length >= 2 Then
                posPart = parts(1).Trim()
            End If

            If _employeePositions.ContainsKey(fullText) Then
                _employeePositions(fullText) = posPart
            Else
                _employeePositions.Add(fullText, posPart)
            End If
        Next
    End Sub

    '────────────────────────────────────────────────────────
    '  WHEN APPROVED NAME CHANGES → AUTO FILL POSITION
    '────────────────────────────────────────────────────────
    Private Sub comboapproved_SelectedIndexChanged(sender As Object, e As EventArgs) Handles comboapproved.SelectedIndexChanged
        Try
            Dim fullText As String = comboapproved.Text.Trim()

            If String.IsNullOrEmpty(fullText) Then
                txtapppos.Text = String.Empty
                Return
            End If

            ' Try dictionary first
            If _employeePositions.ContainsKey(fullText) Then
                txtapppos.Text = _employeePositions(fullText)
                Return
            End If

            ' Fallback: parse text directly by newline
            Dim posPart As String = String.Empty
            Dim parts As String() = fullText.Split(New String() {vbCrLf, vbLf}, StringSplitOptions.None)
            If parts.Length >= 2 Then
                posPart = parts(1).Trim()
            End If

            txtapppos.Text = posPart
        Catch ex As Exception
            ' Non-fatal UI issue
        End Try
    End Sub

    '────────────────────────────────────────────────────────
    '  LOAD EXISTING SIGNATORY SETUP (TBL_RCPPE_SIGNATORY)
    '────────────────────────────────────────────────────────
    Private Sub LoadExistingSignatorySetup()
        _currentId = 0

        Using conn As New SqlConnection(frmmain.txtdb.Text)
            conn.Open()

            Const sql As String =
                "SELECT TOP (1) * " &
                "FROM TBL_RCPPE_SIGNATORY WITH (NOLOCK) " &
                "ORDER BY ID ASC;"

            Using cmd As New SqlCommand(sql, conn)
                Using rdr As SqlDataReader = cmd.ExecuteReader()
                    If rdr.Read() Then
                        _currentId = Convert.ToInt32(rdr("ID"))

                        combosig1.Text = SafeGetString(rdr, "Sig1")
                        combosig2.Text = SafeGetString(rdr, "Sig2")
                        combosig3.Text = SafeGetString(rdr, "Sig3")
                        combosig4.Text = SafeGetString(rdr, "Sig4")
                        combosig5.Text = SafeGetString(rdr, "Sig5")
                        combosig6.Text = SafeGetString(rdr, "Sig6")
                        combosig7.Text = SafeGetString(rdr, "Sig7")
                        combosig8.Text = SafeGetString(rdr, "Sig8")
                        combosig9.Text = SafeGetString(rdr, "Sig9")
                        combosig10.Text = SafeGetString(rdr, "Sig10")

                        comboapproved.Text = SafeGetString(rdr, "Approved_by")
                        txtverifiedby.Text = SafeGetString(rdr, "Verified_by")

                        txtsigpos1.Text = SafeGetString(rdr, "Sig1_pos")
                        txtsigpos2.Text = SafeGetString(rdr, "Sig2_pos")
                        txtsigpos3.Text = SafeGetString(rdr, "Sig3_pos")
                        txtsigpos4.Text = SafeGetString(rdr, "Sig4_pos")
                        txtsigpos5.Text = SafeGetString(rdr, "Sig5_pos")
                        txtsigpos6.Text = SafeGetString(rdr, "Sig6_pos")
                        txtsigpos7.Text = SafeGetString(rdr, "Sig7_pos")
                        txtsigpos8.Text = SafeGetString(rdr, "Sig8_pos")
                        txtsigpos9.Text = SafeGetString(rdr, "Sig9_pos")
                        txtsigpos10.Text = SafeGetString(rdr, "Sig10_pos")

                        ' NEW: load app_pos & veri_pos
                        txtapppos.Text = SafeGetString(rdr, "app_pos")
                        txtveripos.Text = SafeGetString(rdr, "veri_pos")
                    Else
                        _currentId = 0
                    End If
                End Using
            End Using
        End Using

        ' Auto-fill app_pos from comboapproved (if stored text has newline)
        comboapproved_SelectedIndexChanged(Nothing, EventArgs.Empty)
    End Sub

    Private Function SafeGetString(rdr As SqlDataReader, colName As String) As String
        Dim idx As Integer = rdr.GetOrdinal(colName)
        If rdr.IsDBNull(idx) Then Return String.Empty
        Return rdr.GetString(idx).Trim()
    End Function

    '────────────────────────────────────────────────────────
    '  SAVE BUTTON
    '────────────────────────────────────────────────────────
    Private Sub cmdsave_Click(sender As Object, e As EventArgs) Handles cmdsave.Click
        Try
            SaveSignatorySetup()
            MessageBox.Show("Signatory configuration has been saved.",
                            "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Me.DialogResult = DialogResult.OK
        Catch ex As Exception
            MessageBox.Show("Error saving signatory setup:" & Environment.NewLine & ex.Message,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub SaveSignatorySetup()
        ' Collect values
        Dim sig1 As String = combosig1.Text.Trim()
        Dim sig2 As String = combosig2.Text.Trim()
        Dim sig3 As String = combosig3.Text.Trim()
        Dim sig4 As String = combosig4.Text.Trim()
        Dim sig5 As String = combosig5.Text.Trim()
        Dim sig6 As String = combosig6.Text.Trim()
        Dim sig7 As String = combosig7.Text.Trim()
        Dim sig8 As String = combosig8.Text.Trim()
        Dim sig9 As String = combosig9.Text.Trim()
        Dim sig10 As String = combosig10.Text.Trim()

        Dim approvedBy As String = comboapproved.Text.Trim()
        Dim verifiedBy As String = txtverifiedby.Text.Trim()

        Dim sigpos1 As String = txtsigpos1.Text.Trim()
        Dim sigpos2 As String = txtsigpos2.Text.Trim()
        Dim sigpos3 As String = txtsigpos3.Text.Trim()
        Dim sigpos4 As String = txtsigpos4.Text.Trim()
        Dim sigpos5 As String = txtsigpos5.Text.Trim()
        Dim sigpos6 As String = txtsigpos6.Text.Trim()
        Dim sigpos7 As String = txtsigpos7.Text.Trim()
        Dim sigpos8 As String = txtsigpos8.Text.Trim()
        Dim sigpos9 As String = txtsigpos9.Text.Trim()
        Dim sigpos10 As String = txtsigpos10.Text.Trim()

        ' NEW: app_pos & veri_pos
        Dim appPos As String = txtapppos.Text.Trim()
        Dim veriPos As String = txtveripos.Text.Trim()

        Using conn As New SqlConnection(frmmain.txtdb.Text)
            conn.Open()

            If _currentId = 0 Then
                ' INSERT
                Const sqlInsert As String =
                    "INSERT INTO TBL_RCPPE_SIGNATORY (" &
                    " Sig1, Sig2, Sig3, Sig4, Sig5, Sig6, Sig7, Sig8, Sig9, Sig10, " &
                    " Approved_by, Verified_by, " &
                    " Sig1_pos, Sig2_pos, Sig3_pos, Sig4_pos, Sig5_pos, Sig6_pos, Sig7_pos, Sig8_pos, Sig9_pos, Sig10_pos, " &
                    " app_pos, veri_pos" &
                    ") VALUES (" &
                    " @Sig1, @Sig2, @Sig3, @Sig4, @Sig5, @Sig6, @Sig7, @Sig8, @Sig9, @Sig10, " &
                    " @Approved_by, @Verified_by, " &
                    " @Sig1_pos, @Sig2_pos, @Sig3_pos, @Sig4_pos, @Sig5_pos, @Sig6_pos, @Sig7_pos, @Sig8_pos, @Sig9_pos, @Sig10_pos, " &
                    " @App_pos, @Veri_pos" &
                    "); " &
                    "SELECT SCOPE_IDENTITY();"

                Using cmd As New SqlCommand(sqlInsert, conn)
                    AddCommonParameters(cmd,
                                        sig1, sig2, sig3, sig4, sig5,
                                        sig6, sig7, sig8, sig9, sig10,
                                        approvedBy, verifiedBy,
                                        sigpos1, sigpos2, sigpos3, sigpos4, sigpos5,
                                        sigpos6, sigpos7, sigpos8, sigpos9, sigpos10,
                                        appPos, veriPos)

                    Dim newIdObj As Object = cmd.ExecuteScalar()
                    _currentId = Convert.ToInt32(newIdObj)
                End Using
            Else
                ' UPDATE
                Const sqlUpdate As String =
                    "UPDATE TBL_RCPPE_SIGNATORY SET " &
                    " Sig1 = @Sig1, " &
                    " Sig2 = @Sig2, " &
                    " Sig3 = @Sig3, " &
                    " Sig4 = @Sig4, " &
                    " Sig5 = @Sig5, " &
                    " Sig6 = @Sig6, " &
                    " Sig7 = @Sig7, " &
                    " Sig8 = @Sig8, " &
                    " Sig9 = @Sig9, " &
                    " Sig10 = @Sig10, " &
                    " Approved_by = @Approved_by, " &
                    " Verified_by = @Verified_by, " &
                    " Sig1_pos = @Sig1_pos, " &
                    " Sig2_pos = @Sig2_pos, " &
                    " Sig3_pos = @Sig3_pos, " &
                    " Sig4_pos = @Sig4_pos, " &
                    " Sig5_pos = @Sig5_pos, " &
                    " Sig6_pos = @Sig6_pos, " &
                    " Sig7_pos = @Sig7_pos, " &
                    " Sig8_pos = @Sig8_pos, " &
                    " Sig9_pos = @Sig9_pos, " &
                    " Sig10_pos = @Sig10_pos, " &
                    " app_pos = @App_pos, " &
                    " veri_pos = @Veri_pos " &
                    "WHERE ID = @ID;"

                Using cmd As New SqlCommand(sqlUpdate, conn)
                    AddCommonParameters(cmd,
                                        sig1, sig2, sig3, sig4, sig5,
                                        sig6, sig7, sig8, sig9, sig10,
                                        approvedBy, verifiedBy,
                                        sigpos1, sigpos2, sigpos3, sigpos4, sigpos5,
                                        sigpos6, sigpos7, sigpos8, sigpos9, sigpos10,
                                        appPos, veriPos)

                    cmd.Parameters.Add("@ID", SqlDbType.Int).Value = _currentId
                    cmd.ExecuteNonQuery()
                End Using
            End If
        End Using
    End Sub

    Private Sub AddCommonParameters(cmd As SqlCommand,
                                    sig1 As String, sig2 As String, sig3 As String, sig4 As String, sig5 As String,
                                    sig6 As String, sig7 As String, sig8 As String, sig9 As String, sig10 As String,
                                    approvedBy As String, verifiedBy As String,
                                    sigpos1 As String, sigpos2 As String, sigpos3 As String, sigpos4 As String, sigpos5 As String,
                                    sigpos6 As String, sigpos7 As String, sigpos8 As String, sigpos9 As String, sigpos10 As String,
                                    appPos As String, veriPos As String)

        cmd.Parameters.Clear()

        cmd.Parameters.Add("@Sig1", SqlDbType.NVarChar, 500).Value = sig1
        cmd.Parameters.Add("@Sig2", SqlDbType.NVarChar, 500).Value = sig2
        cmd.Parameters.Add("@Sig3", SqlDbType.NVarChar, 500).Value = sig3
        cmd.Parameters.Add("@Sig4", SqlDbType.NVarChar, 500).Value = sig4
        cmd.Parameters.Add("@Sig5", SqlDbType.NVarChar, 500).Value = sig5
        cmd.Parameters.Add("@Sig6", SqlDbType.NVarChar, 500).Value = sig6
        cmd.Parameters.Add("@Sig7", SqlDbType.NVarChar, 500).Value = sig7
        cmd.Parameters.Add("@Sig8", SqlDbType.NVarChar, 500).Value = sig8
        cmd.Parameters.Add("@Sig9", SqlDbType.NVarChar, 500).Value = sig9
        cmd.Parameters.Add("@Sig10", SqlDbType.NVarChar, 500).Value = sig10

        cmd.Parameters.Add("@Approved_by", SqlDbType.NVarChar, 500).Value = approvedBy
        cmd.Parameters.Add("@Verified_by", SqlDbType.NVarChar, 500).Value = verifiedBy

        cmd.Parameters.Add("@Sig1_pos", SqlDbType.NVarChar, 250).Value = sigpos1
        cmd.Parameters.Add("@Sig2_pos", SqlDbType.NVarChar, 250).Value = sigpos2
        cmd.Parameters.Add("@Sig3_pos", SqlDbType.NVarChar, 250).Value = sigpos3
        cmd.Parameters.Add("@Sig4_pos", SqlDbType.NVarChar, 250).Value = sigpos4
        cmd.Parameters.Add("@Sig5_pos", SqlDbType.NVarChar, 250).Value = sigpos5
        cmd.Parameters.Add("@Sig6_pos", SqlDbType.NVarChar, 250).Value = sigpos6
        cmd.Parameters.Add("@Sig7_pos", SqlDbType.NVarChar, 250).Value = sigpos7
        cmd.Parameters.Add("@Sig8_pos", SqlDbType.NVarChar, 250).Value = sigpos8
        cmd.Parameters.Add("@Sig9_pos", SqlDbType.NVarChar, 250).Value = sigpos9
        cmd.Parameters.Add("@Sig10_pos", SqlDbType.NVarChar, 250).Value = sigpos10

        ' NEW: parameters for app_pos & veri_pos
        cmd.Parameters.Add("@App_pos", SqlDbType.NVarChar, 250).Value = appPos
        cmd.Parameters.Add("@Veri_pos", SqlDbType.NVarChar, 250).Value = veriPos
    End Sub

    '────────────────────────────────────────────────────────
    '  CLOSE BUTTON
    '────────────────────────────────────────────────────────
    Private Sub cmdclose_Click(sender As Object, e As EventArgs) Handles cmdclose.Click
        Me.Close()
    End Sub

End Class
