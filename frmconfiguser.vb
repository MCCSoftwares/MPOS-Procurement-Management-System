Imports System.Data.SqlClient
Imports System.Text.RegularExpressions

Public Class frmconfiguser

    Private Sub frmconfiguser_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Start with a checklist visible and validation applied once
        UpdatePasswordHints()
    End Sub

    ' ─────────────────────────────────────────────────────────────────────────
    ' Live validation: update the checklist as the user types
    ' ─────────────────────────────────────────────────────────────────────────
    Private Sub txtpassword_TextChanged(sender As Object, e As EventArgs) Handles txtpassword.TextChanged
        UpdatePasswordHints()
    End Sub

    Private Sub txtrepassword_TextChanged(sender As Object, e As EventArgs) Handles txtrepassword.TextChanged
        UpdatePasswordHints()
    End Sub

    Private Sub combooffsec_KeyPress(sender As Object, e As KeyPressEventArgs) Handles combooffsec.KeyPress
        e.Handled = True
    End Sub

    Private Sub comboula_KeyPress(sender As Object, e As KeyPressEventArgs) Handles comboula.KeyPress
        e.Handled = True
    End Sub

    Private Sub cmdshow_Click(sender As Object, e As EventArgs) Handles cmdshow.Click
        If txtpassword.PasswordChar = ChrW(0) Then
            txtpassword.PasswordChar = "●"c
            txtrepassword.PasswordChar = "●"c
        Else
            txtpassword.PasswordChar = ChrW(0)
            txtrepassword.PasswordChar = ChrW(0)
        End If
    End Sub

    Private Sub cmdsave_Click(sender As Object, e As EventArgs) Handles cmdsave.Click
        If String.Equals(cmdsave.Text, "Save", StringComparison.OrdinalIgnoreCase) Then
            SaveUser()
        Else
            UpdateUser()
        End If
    End Sub

    ' ─────────────────────────────────────────────────────────────────────────
    ' Password rules helpers
    ' ─────────────────────────────────────────────────────────────────────────
    Private Function IsPasswordStrong(pwd As String) As Boolean
        If String.IsNullOrEmpty(pwd) Then Return False
        Dim hasLower As Boolean = Regex.IsMatch(pwd, "[a-z]")
        Dim hasUpper As Boolean = Regex.IsMatch(pwd, "[A-Z]")
        Dim hasDigit As Boolean = Regex.IsMatch(pwd, "\d")
        Dim minLen As Boolean = pwd.Length >= 8
        ' Optional: special character rule (uncomment to enforce)
        ' Dim hasSpecial As Boolean = Regex.IsMatch(pwd, "[^a-zA-Z0-9]")

        Return hasLower AndAlso hasUpper AndAlso hasDigit AndAlso minLen ' AndAlso hasSpecial
    End Function

    Private Sub UpdatePasswordHints()
        Dim pwd As String = txtpassword.Text
        Dim rep As String = txtrepassword.Text

        Dim hasLower As Boolean = Regex.IsMatch(pwd, "[a-z]")
        Dim hasUpper As Boolean = Regex.IsMatch(pwd, "[A-Z]")
        Dim hasDigit As Boolean = Regex.IsMatch(pwd, "\d")
        Dim minLen As Boolean = pwd.Length >= 8
        ' Optional: special character rule
        ' Dim hasSpecial As Boolean = Regex.IsMatch(pwd, "[^a-zA-Z0-9]")

        Dim match As Boolean = (pwd.Length > 0 AndAlso pwd = rep)

        ' Build checklist text
        Dim lines As New List(Of String)
        lines.Add((If(hasLower, "✓ ", "✗ ")) & "At least one lowercase letter")
        lines.Add((If(hasUpper, "✓ ", "✗ ")) & "At least one uppercase letter")
        lines.Add((If(hasDigit, "✓ ", "✗ ")) & "At least one number")
        lines.Add((If(minLen, "✓ ", "✗ ")) & "Minimum 8 characters")
        lines.Add((If(match, "✓ ", "✗ ")) & "Passwords match")

        lblpasswordinfo.Text = String.Join(Environment.NewLine, lines)

        ' Enable Save/Update only if all rules are satisfied
        Dim allOk As Boolean = hasLower AndAlso hasUpper AndAlso hasDigit AndAlso minLen AndAlso match
        cmdsave.Enabled = allOk

        ' Change label color
        If allOk Then
            lblpasswordinfo.ForeColor = Color.ForestGreen
        Else
            lblpasswordinfo.ForeColor = Color.DarkRed
        End If
    End Sub


    ' ─────────────────────────────────────────────────────────────────────────
    ' Save / Update with server-side password checks
    ' ─────────────────────────────────────────────────────────────────────────
    Public Sub SaveUser()
        Try
            ' 1) Field validation
            If txtfname.Text.Trim() = "" OrElse
               txtlname.Text.Trim() = "" OrElse
               txtemail.Text.Trim() = "" OrElse
               txtposition.Text.Trim() = "" OrElse
               combooffsec.Text = "" OrElse
               comboula.Text = "" OrElse
               txtuserid.Text.Trim() = "" OrElse
               txtpassword.Text = "" OrElse
               txtrepassword.Text = "" Then

                MessageBox.Show("All fields are required.", "Validation Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            ' 2) Password rules + match
            If Not IsPasswordStrong(txtpassword.Text) Then
                MessageBox.Show("Password doesn't meet the required rules.", "Validation Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtpassword.Focus()
                Return
            End If
            If txtpassword.Text <> txtrepassword.Text Then
                MessageBox.Show("Passwords do not match.", "Validation Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtrepassword.Focus()
                Return
            End If

            ' 3) Insert
            Dim newId As Integer
            Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
                conn.Open()

                Const sql As String =
"INSERT INTO TBL_LOGIN
 (Login_Name, Login_LName, Login_OPos, Login_Office, Login_Position,
  Login_UserID, Login_Password, Login_Email, Login_Status)
VALUES
 (@fname, @lname, @Oposition, @office, @position,
  @userid, @password, @email, 'Active');
SELECT SCOPE_IDENTITY();"

                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@fname", txtfname.Text.Trim())
                    cmd.Parameters.AddWithValue("@lname", txtlname.Text.Trim())
                    cmd.Parameters.AddWithValue("@Oposition", txtposition.Text.Trim())
                    cmd.Parameters.AddWithValue("@office", combooffsec.Text)
                    cmd.Parameters.AddWithValue("@position", comboula.Text)
                    cmd.Parameters.AddWithValue("@userid", txtuserid.Text.Trim())
                    cmd.Parameters.AddWithValue("@password", txtpassword.Text) ' (Consider hashing in the future)
                    cmd.Parameters.AddWithValue("@email", txtemail.Text.Trim())

                    newId = Convert.ToInt32(cmd.ExecuteScalar())
                End Using
            End Using

            lblid.Text = newId.ToString()
            cmdsave.Text = "Update"
            MessageBox.Show("User successfully saved.", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)

            My.Forms.frmconfig.LoadUsers()

        Catch ex As Exception
            MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Public Sub UpdateUser()
        Try
            ' 1) Field validation
            If txtfname.Text.Trim() = "" OrElse
               txtlname.Text.Trim() = "" OrElse
               txtemail.Text.Trim() = "" OrElse
               txtposition.Text.Trim() = "" OrElse
               combooffsec.Text = "" OrElse
               comboula.Text = "" OrElse
               txtuserid.Text.Trim() = "" OrElse
               txtpassword.Text = "" OrElse
               txtrepassword.Text = "" Then

                MessageBox.Show("All fields are required.", "Validation Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            ' 2) Password rules + match
            If Not IsPasswordStrong(txtpassword.Text) Then
                MessageBox.Show("Password doesn't meet the required rules.", "Validation Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtpassword.Focus()
                Return
            End If
            If txtpassword.Text <> txtrepassword.Text Then
                MessageBox.Show("Passwords do not match.", "Validation Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtrepassword.Focus()
                Return
            End If

            ' 3) Update
            Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
                conn.Open()

                Const sql As String =
"UPDATE TBL_LOGIN
   SET Login_Name     = @fname,
       Login_LName    = @lname,
       Login_OPos     = @Oposition,
       Login_Office   = @office,
       Login_Position = @position,
       Login_UserID   = @userid,
       Login_Password = @password,
       Login_Email    = @email
 WHERE ID = @id;"

                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@fname", txtfname.Text.Trim())
                    cmd.Parameters.AddWithValue("@lname", txtlname.Text.Trim())
                    cmd.Parameters.AddWithValue("@Oposition", txtposition.Text.Trim())
                    cmd.Parameters.AddWithValue("@office", combooffsec.Text)
                    cmd.Parameters.AddWithValue("@position", comboula.Text)
                    cmd.Parameters.AddWithValue("@userid", txtuserid.Text.Trim())
                    cmd.Parameters.AddWithValue("@password", txtpassword.Text) ' (Consider hashing in the future)
                    cmd.Parameters.AddWithValue("@email", txtemail.Text.Trim())
                    cmd.Parameters.AddWithValue("@id", Convert.ToInt32(lblid.Text))

                    cmd.ExecuteNonQuery()
                End Using
            End Using

            MessageBox.Show("User successfully updated.", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)

            My.Forms.frmconfig.LoadUsers()

        Catch ex As Exception
            MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ─────────────────────────────────────────────────────────────────────────
    ' LoadUser (unchanged except for structure)
    ' ─────────────────────────────────────────────────────────────────────────
    Public Sub LoadUser()
        Try
            Dim userId As Integer
            If Not Integer.TryParse(lblid.Text.Trim(), userId) Then
                MessageBox.Show("Invalid user ID.", "Load User", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
                conn.Open()

                Using cmd As New SqlCommand("
                    SELECT Login_Name, Login_LName, Login_Email,
                           Login_Office, Login_OPos, Login_Position,
                           Login_UserID
                    FROM TBL_LOGIN
                    WHERE ID = @id;", conn)

                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = userId

                    Using rdr As SqlDataReader = cmd.ExecuteReader()
                        If rdr.Read() Then
                            txtfname.Text = rdr("Login_Name").ToString().Trim()
                            txtlname.Text = rdr("Login_LName").ToString().Trim()
                            txtemail.Text = rdr("Login_Email").ToString().Trim()
                            combooffsec.Text = rdr("Login_Office").ToString().Trim()
                            txtposition.Text = rdr("Login_OPos").ToString().Trim()
                            comboula.Text = rdr("Login_Position").ToString().Trim()
                            txtuserid.Text = rdr("Login_UserID").ToString().Trim()

                            Dim pos As String = comboula.Text.ToLowerInvariant()
                            Dim isAdmin As Boolean = (pos = "administrator" OrElse pos = "super-admin")
                            combooffsec.Enabled = isAdmin
                            txtposition.Enabled = isAdmin
                            comboula.Enabled = isAdmin
                        Else
                            MessageBox.Show("User not found.", "Load User", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        End If
                    End Using
                End Using
            End Using

        Catch ex As Exception
            MessageBox.Show("Error loading user: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub cmdclose_Click(sender As Object, e As EventArgs) Handles cmdclose.Click
        Me.Close()
    End Sub

End Class
