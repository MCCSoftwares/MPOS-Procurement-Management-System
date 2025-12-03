Imports System.Data.SqlClient
Imports System.Net
Imports System.Net.Mail
Imports System.Net.Mime
Imports System.Security.Cryptography
Imports System.Text
Imports System.Linq
Imports System.Text.RegularExpressions   ' ← add this

Public Class frmlogin

    ' ─────────────────────────────────────────────────────────────────────
    ' Config / constants
    ' ─────────────────────────────────────────────────────────────────────
    Private Const MAX_ATTEMPTS As Integer = 5
    Private Const RESET_TTL_MINUTES As Integer = 10

    ' Gmail SMTP (use an APP PASSWORD)
    Private Const SMTP_HOST As String = "smtp.gmail.com"
    Private Const SMTP_PORT As Integer = 587
    Private Const SMTP_USER As String = "pmis.mpos@gmail.com"
    Private Const SMTP_PASS As String = "wekh rsri rszn dqll"   ' <<< REPLACE with Gmail App Password

    ' ─────────────────────────────────────────────────────────────────────
    ' State
    ' ─────────────────────────────────────────────────────────────────────
    Private loginAttempts As New Dictionary(Of String, Integer)
    Private loginName As String

    ' ─────────────────────────────────────────────────────────────────────
    ' UI events
    ' ─────────────────────────────────────────────────────────────────────
    Private Sub cmdLogin_Click(sender As Object, e As EventArgs) Handles cmdlogin.Click
        login()
    End Sub

    Public Sub login()
        Try
            Dim userID = txtid.Text.Trim()
            Dim pwd = txtpassword.Text.Trim()

            If String.IsNullOrEmpty(userID) OrElse String.IsNullOrEmpty(pwd) Then
                MessageBox.Show("Please enter both User ID and Password.", "Missing Credentials", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            If IsAccountInactive(userID) Then
                MessageBox.Show("Your account is inactive. Please contact your administrator.", "Account Locked", MessageBoxButtons.OK, MessageBoxIcon.Stop)
                Return
            End If

            If VerifyCredentials(userID, pwd) Then
                If loginAttempts.ContainsKey(userID) Then loginAttempts.Remove(userID)
                Dim ts As New frmtimesetup()
                Me.Dispose()
                ts.ShowDialog()
            Else
                IncrementAttempt(userID)
                Dim attemptsLeft = MAX_ATTEMPTS - loginAttempts(userID)
                If attemptsLeft > 0 Then
                    MessageBox.Show($"Invalid credentials. You have {attemptsLeft} attempt(s) remaining.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtpassword.Clear()
                Else
                    LockAccount(userID)
                    MessageBox.Show("You have reached the maximum number of attempts. Your account is now inactive.", "Account Locked", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            End If
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub

    ''' <summary>
    ''' Returns True if userID/password is valid AND status is Active.
    ''' Supports PBKDF2 hash format or legacy plaintext.
    ''' </summary>
    Private Function VerifyCredentials(userID As String, password As String) As Boolean
        Dim connStr = lbldb.Text
        Using conn As New SqlConnection(connStr)
            conn.Open()

            Const sql As String = "
            SELECT TOP 1
                ID,
                Login_Password,
                Login_Name,
                Login_Office,
                Login_LName,
                Login_OPos,
                Login_Position
            FROM TBL_LOGIN
            WHERE Login_UserID = @uid
              AND Login_Status = 'Active';
            "

            Using cmd As New SqlCommand(sql, conn)
                cmd.Parameters.Add("@uid", SqlDbType.NVarChar, 255).Value = userID
                Using reader = cmd.ExecuteReader()
                    If reader.Read() Then
                        Dim stored = reader("Login_Password").ToString()

                        Dim ok As Boolean = VerifyPasswordFlexible(stored, password)
                        If ok Then
                            Dim firstName = reader("Login_Name").ToString()
                            Dim lastName = reader("Login_LName").ToString()
                            Dim position = reader("Login_Position").ToString()
                            Dim Offsec = reader("Login_Office").ToString()
                            Dim userposition = reader("Login_OPos").ToString()
                            Dim userID1 = reader("ID").ToString()

                            My.Forms.frmmain.lblaname.Text = firstName & " " & lastName
                            My.Forms.frmmain.lblaposition.Text = position
                            My.Forms.frmmain.lblposition.Text = userposition
                            My.Forms.frmmain.lbluid.Text = userID1
                            My.Forms.frmmain.lbloffice.Text = Offsec
                            My.Forms.frmmain.lblhelloname.Text = "Welcome back, " & firstName
                            My.Forms.frmmain.lblhelloposition.Text = "You're logged in as " & position

                            If position = "Super-Admin" Then
                                My.Forms.frmmain.loadsuperadminUI()
                            ElseIf position = "Administrator" Then
                                My.Forms.frmmain.loadadminUI()
                            ElseIf position = "Procurement Officer" Then
                                My.Forms.frmmain.loadprocurementUI()
                            ElseIf position = "Supply Officer" Then
                                My.Forms.frmmain.loadsupplyUI()
                            ElseIf position = "End User" Then
                                My.Forms.frmmain.loadenduserUI()
                            End If

                            Return True
                        End If
                    End If
                End Using
            End Using
        End Using
        Return False
    End Function

    ''' <summary>Check if account is Inactive.</summary>
    Private Function IsAccountInactive(userID As String) As Boolean
        Try
            Dim connStr = lbldb.Text
            Using conn As New SqlConnection(connStr)
                conn.Open()
                Const sql As String = "
                    SELECT COUNT(1)
                    FROM TBL_LOGIN
                    WHERE Login_UserID = @uid
                      AND Login_Status = 'Inactive';
                "
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.Add("@uid", SqlDbType.NVarChar, 255).Value = userID
                    Return CInt(cmd.ExecuteScalar()) > 0
                End Using
            End Using
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Function

    Private Sub IncrementAttempt(userID As String)
        If Not loginAttempts.ContainsKey(userID) Then
            loginAttempts(userID) = 1
        Else
            loginAttempts(userID) += 1
        End If
    End Sub

    Private Sub LockAccount(userID As String)
        Dim connStr = lbldb.Text
        Using conn As New SqlConnection(connStr)
            conn.Open()
            Const sql As String = "
                UPDATE TBL_LOGIN
                SET Login_Status = 'Inactive'
                WHERE Login_UserID = @uid;
            "
            Using cmd As New SqlCommand(sql, conn)
                cmd.Parameters.Add("@uid", SqlDbType.NVarChar, 255).Value = userID
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Private Sub frmlogin_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If e.CloseReason = CloseReason.UserClosing Then
            Application.Exit()
        End If
    End Sub

    Private Sub txtPassword_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtpassword.KeyPress
        If e.KeyChar = ChrW(Keys.Enter) Then
            e.Handled = True
            login()
        End If
    End Sub

    Private Sub Label11_Click(sender As Object, e As EventArgs) Handles Label11.Click
        frmconsetup.Dispose()
        frmconsetup.ShowDialog()
    End Sub

    Private Sub Cmdexit_Click(sender As Object, e As EventArgs) Handles cmdexit.Click
        End
    End Sub

    Private Sub Label4_Click(sender As Object, e As EventArgs) Handles Label4.Click
        Try
            panellogin.Visible = False
            panelforgot.Visible = True
            ' Reset password hints whenever panel opens
            UpdateForgotPasswordHints()
        Catch
        End Try
    End Sub

    Private Sub Frmlogin_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        EnableDoubleBuffering(Me)
        panellogin.Visible = True
        panelforgot.Visible = False

        txtfcode.Enabled = False
        txtfcode.Text = "Request Code"
        txtfpassword.Enabled = False
        txtfrepassword.Enabled = False
        cmdchange.Enabled = False
        cmdsend.Text = "Send"

        ' Initialize label for hints (safe defaults)
        lblpasswordinfo.Text = ""
        lblpasswordinfo.ForeColor = Color.DarkRed

        ' Wire up live validation for forgot password boxes
        AddHandler txtfpassword.TextChanged, AddressOf ForgotPassword_TextChanged
        AddHandler txtfrepassword.TextChanged, AddressOf ForgotPassword_TextChanged
    End Sub

    ' ─────────────────────────────────────────────────────────────────────
    ' Forgot Password: Send → Verify
    ' ─────────────────────────────────────────────────────────────────────
    Private Sub cmdsend_Click(sender As Object, e As EventArgs) Handles cmdsend.Click
        Dim uid = txtfuserid.Text.Trim()
        Dim mailA = txtfemail.Text.Trim()

        If String.Equals(cmdsend.Text, "Send", StringComparison.OrdinalIgnoreCase) Then

            If uid = "" OrElse mailA = "" Then
                MessageBox.Show("Please enter both User ID and Email.", "Missing Data", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Using conn As New SqlConnection(lbldb.Text)
                conn.Open()

                ' Validate user+email
                Const sqlCheck = "
                    SELECT TOP 1 Login_Name
                    FROM TBL_LOGIN
                    WHERE Login_UserID = @uid
                      AND Login_Email  = @mail
                      AND Login_Status = 'Active';
                "
                Using cmd = New SqlCommand(sqlCheck, conn)
                    cmd.Parameters.Add("@uid", SqlDbType.NVarChar, 255).Value = uid
                    cmd.Parameters.Add("@mail", SqlDbType.NVarChar, 255).Value = mailA
                    Using rdr = cmd.ExecuteReader()
                        If Not rdr.Read() Then
                            MessageBox.Show("No active account found matching that User ID and Email.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Return
                        End If
                        loginName = rdr("Login_Name").ToString()
                    End Using
                End Using

                ' Invalidate any previous unconsumed codes for this user
                Using cmdClr As New SqlCommand("
                    UPDATE TBL_PWDRESET
                    SET Consumed = 1
                    WHERE Login_UserID = @uid AND Login_Email = @mail AND Consumed = 0;
                ", conn)
                    cmdClr.Parameters.Add("@uid", SqlDbType.NVarChar, 255).Value = uid
                    cmdClr.Parameters.Add("@mail", SqlDbType.NVarChar, 255).Value = mailA
                    cmdClr.ExecuteNonQuery()
                End Using

                ' Generate cryptographically-strong 6-digit code (leading zeros allowed)
                Dim code As String = GenerateSixDigitCode()

                ' Store only the hash with expiry
                Dim nowUtc As DateTime = DateTime.UtcNow
                Dim expUtc As DateTime = nowUtc.AddMinutes(RESET_TTL_MINUTES)
                Using cmdIns As New SqlCommand("
                    INSERT INTO TBL_PWDRESET (Login_UserID, Login_Email, CodeHash, ExpiresAt, Consumed, CreatedAt)
                    VALUES (@uid, @mail, @hash, @exp, 0, @now);
                ", conn)
                    cmdIns.Parameters.Add("@uid", SqlDbType.NVarChar, 255).Value = uid
                    cmdIns.Parameters.Add("@mail", SqlDbType.NVarChar, 255).Value = mailA
                    cmdIns.Parameters.Add("@hash", SqlDbType.VarBinary, 32).Value = ComputeSHA256(code)
                    cmdIns.Parameters.Add("@exp", SqlDbType.DateTime2).Value = expUtc
                    cmdIns.Parameters.Add("@now", SqlDbType.DateTime2).Value = nowUtc
                    cmdIns.ExecuteNonQuery()
                End Using

                ' Send email
                If Not SendResetEmail(mailA, loginName, code, RESET_TTL_MINUTES) Then
                    Return
                End If
            End Using

            ' Prep UI for verify
            txtfcode.Enabled = True
            txtfcode.Text = ""
            cmdsend.Text = "Verify"
            txtfuserid.Enabled = False
            txtfemail.Enabled = False
            txtfcode.Focus()

        ElseIf String.Equals(cmdsend.Text, "Verify", StringComparison.OrdinalIgnoreCase) Then
            Dim entered = txtfcode.Text.Trim()
            If entered.Length <> 6 OrElse Not entered.All(AddressOf Char.IsDigit) Then
                MessageBox.Show("Please enter the 6-digit code sent to your email.", "Invalid Code", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Dim uidV = txtfuserid.Text.Trim()
            Dim mailV = txtfemail.Text.Trim()

            Dim latestId As Integer = 0
            Dim latestHash As Byte() = Nothing

            Using conn As New SqlConnection(lbldb.Text)
                conn.Open()

                ' Get latest unexpired, unconsumed token
                Using cmdFind As New SqlCommand("
                    SELECT TOP 1 ID, CodeHash
                    FROM TBL_PWDRESET
                    WHERE Login_UserID = @uid
                      AND Login_Email  = @mail
                      AND Consumed     = 0
                      AND ExpiresAt    >= SYSUTCDATETIME()
                    ORDER BY CreatedAt DESC;
                ", conn)
                    cmdFind.Parameters.Add("@uid", SqlDbType.NVarChar, 255).Value = uidV
                    cmdFind.Parameters.Add("@mail", SqlDbType.NVarChar, 255).Value = mailV
                    Using rdr = cmdFind.ExecuteReader()
                        If rdr.Read() Then
                            latestId = CInt(rdr("ID"))
                            latestHash = DirectCast(rdr("CodeHash"), Byte())
                        End If
                    End Using
                End Using

                If latestId = 0 OrElse latestHash Is Nothing Then
                    MessageBox.Show("No active reset request found or the code has expired. Please send a new code.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If

                ' Compare hashes (constant-time)
                Dim ok = BytesEqual(ComputeSHA256(entered), latestHash)
                If Not ok Then
                    MessageBox.Show("Code does not match. Please try again.", "Invalid Code", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End If

                ' Mark consumed
                Using cmdUse As New SqlCommand("UPDATE TBL_PWDRESET SET Consumed = 1 WHERE ID = @id;", conn)
                    cmdUse.Parameters.Add("@id", SqlDbType.Int).Value = latestId
                    cmdUse.ExecuteNonQuery()
                End Using
            End Using

            MessageBox.Show("Code verified! Please enter your new password.", "Verified", MessageBoxButtons.OK, MessageBoxIcon.Information)
            txtfpassword.Enabled = True
            txtfrepassword.Enabled = True
            cmdchange.Enabled = False
            cmdsend.Enabled = False

            ' Show hints immediately for empty fields
            UpdateForgotPasswordHints()
        End If
    End Sub

    ' ─────────────────────────────────────────────────────────────────────
    ' Change password (after verify)
    ' ─────────────────────────────────────────────────────────────────────
    Private Sub cmdchange_Click(sender As Object, e As EventArgs) Handles cmdchange.Click
        Dim newPwd = txtfpassword.Text
        Dim newPwd2 = txtfrepassword.Text

        If newPwd = "" OrElse newPwd2 = "" Then
            MessageBox.Show("Please fill in both password fields.", "Missing Password", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        ElseIf Not IsPasswordStrong(newPwd) Then
            MessageBox.Show("Password doesn't meet the required rules.", "Weak Password", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            UpdateForgotPasswordHints()
            txtfpassword.Focus()
            Return
        ElseIf newPwd <> newPwd2 Then
            MessageBox.Show("Passwords do not match.", "Mismatch", MessageBoxButtons.OK, MessageBoxIcon.Error)
            UpdateForgotPasswordHints()
            txtfrepassword.Focus()
            Return
        End If

        Dim uid = txtfuserid.Text.Trim()

        ' Hash with PBKDF2 and save (keeps DB compatible by storing as text)
        Dim stored = HashPasswordPBKDF2(newPwd)   ' format: PBKDF2$<iter>$<saltB64>$<keyB64>

        Using conn As New SqlConnection(lbldb.Text)
            conn.Open()
            Using cmd As New SqlCommand("
                UPDATE TBL_LOGIN
                SET Login_Password = @p
                WHERE Login_UserID = @u;
            ", conn)
                cmd.Parameters.Add("@p", SqlDbType.NVarChar, 255).Value = stored
                cmd.Parameters.Add("@u", SqlDbType.NVarChar, 255).Value = uid
                cmd.ExecuteNonQuery()
            End Using

            ' (Optional) Consume any remaining tokens
            Using cmdTok As New SqlCommand("
                UPDATE TBL_PWDRESET
                SET Consumed = 1
                WHERE Login_UserID = @u AND Consumed = 0;
            ", conn)
                cmdTok.Parameters.Add("@u", SqlDbType.NVarChar, 255).Value = uid
                cmdTok.ExecuteNonQuery()
            End Using
        End Using

        MessageBox.Show("Password successfully updated!", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information)

        ResetForgotControls()
        panelforgot.Visible = False
        panellogin.Visible = True
    End Sub

    Private Sub cmdcancel_Click(sender As Object, e As EventArgs) Handles cmdcancel.Click
        ResetForgotControls()
        panelforgot.Visible = False
        panellogin.Visible = True
    End Sub

    Private Sub ResetForgotControls()
        txtfuserid.Enabled = True : txtfuserid.Clear()
        txtfemail.Enabled = True : txtfemail.Clear()
        txtfcode.Enabled = False : txtfcode.Text = "Request Code"
        txtfpassword.Enabled = False : txtfpassword.Clear()
        txtfrepassword.Enabled = False : txtfrepassword.Clear()
        cmdsend.Text = "Send"
        cmdsend.Enabled = True
        cmdchange.Enabled = False
        lblpasswordinfo.Text = ""
        lblpasswordinfo.ForeColor = Color.DarkRed
        loginName = String.Empty
    End Sub

    ' ─────────────────────────────────────────────────────────────────────
    ' Helpers: Email, RNG code, hashing
    ' ─────────────────────────────────────────────────────────────────────
    Private Function SendResetEmail(toEmail As String, displayName As String, code As String, ttlMins As Integer) As Boolean
        Try
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

            Dim htmlBody As String = BuildResetHtml(displayName, code, ttlMins)
            Dim textBody As String = BuildResetText(displayName, code, ttlMins)

            Using msg As New MailMessage()
                msg.From = New MailAddress(SMTP_USER, "PMIS Password Reset")
                msg.To.Add(New MailAddress(toEmail))
                msg.Subject = "Here's your PMIS password reset code"
                msg.BodyEncoding = Encoding.UTF8
                msg.SubjectEncoding = Encoding.UTF8

                Dim textView = AlternateView.CreateAlternateViewFromString(textBody, Encoding.UTF8, MediaTypeNames.Text.Plain)
                msg.AlternateViews.Add(textView)

                Dim htmlView = AlternateView.CreateAlternateViewFromString(htmlBody, Encoding.UTF8, MediaTypeNames.Text.Html)

                Dim logoData As Byte()
                Using wc As New WebClient()
                    logoData = wc.DownloadData("https://drive.google.com/uc?export=view&id=19rfKNI5T0hGahXXCd4C_U8a7lqoUjMIq")
                End Using
                Dim logoStream As New IO.MemoryStream(logoData)
                Dim logo As New LinkedResource(logoStream, MediaTypeNames.Image.Jpeg) With {
                    .ContentId = "logo",
                    .TransferEncoding = TransferEncoding.Base64
                }
                htmlView.LinkedResources.Add(logo)

                msg.AlternateViews.Add(htmlView)
                msg.IsBodyHtml = True

                Using smtp As New SmtpClient(SMTP_HOST, SMTP_PORT)
                    smtp.EnableSsl = True
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network
                    smtp.UseDefaultCredentials = False
                    smtp.Credentials = New NetworkCredential(SMTP_USER, SMTP_PASS)
                    smtp.Timeout = 20000
                    smtp.Send(msg)
                End Using
            End Using

            Return True

        Catch ex As Exception
            MessageBox.Show("Failed to send the reset email." & Environment.NewLine &
                            "Details: " & ex.Message, "Email Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End Try
    End Function

    Private Function BuildResetHtml(name As String, code As String, ttlMins As Integer) As String
        Dim safeName = WebUtility.HtmlEncode(If(name, ""))
        Dim safeCode = WebUtility.HtmlEncode(code)

        Return $"
<!DOCTYPE html>
<html>
<head>
  <meta charset='utf-8'>
  <meta name='viewport' content='width=device-width, initial-scale=1'>
  <title>Password Reset</title>
  <style>
    .wrapper {{ width:100%; background:#f5f7fb; padding:24px 0; font-family:Segoe UI, Arial, sans-serif; }}
    .card {{ max-width:560px; margin:0 auto; background:#ffffff; border-radius:10px; border:1px solid #e7ecf5; box-shadow:0 2px 8px rgba(16,24,40,.06); }}
    .header {{ text-align:center; padding:28px 24px 8px 24px; }}
    .logo {{ height:32px; display:inline-block; }}
    .body {{ padding:8px 28px 24px 28px; color:#1f2937; line-height:1.6; }}
    .title {{ font-size:18px; font-weight:700; margin:8px 0 6px 0; }}
    .lead {{ color:#4b5563; margin:0 0 18px 0; }}
    .code-box {{ text-align:center; background:#f3f6ff; border:1px dashed #93a6ff; color:#1f2a56; font-weight:800; font-size:24px; letter-spacing:6px; padding:16px; border-radius:8px; margin:14px 0 18px 0; }}
    .hint {{ color:#6b7280; font-size:13px; }}
    .footer {{ text-align:center; color:#9aa3af; font-size:12px; padding:18px 0 26px 0; }}
  </style>
</head>
<body>
  <div class='wrapper'>
    <div class='card'>
      <div class='header'>
        <img class='logo' src='cid:logo' alt='MPOS' style='width:160px; height:auto;' />
      </div>
      <div class='body'>
        <div class='title'>Password reset request</div>
        <p class='lead'>Hi {safeName}, we received a request to reset your Procurement Management and Inventory System (PMIS) password.</p>
        <div class='code-box'>{safeCode}</div>
        <p class='hint'>
          Enter the code above in the app to continue changing your password. This code expires in {ttlMins} minutes.
          If you didn’t request this, you can safely ignore this email.
        </p>
      </div>
      <div class='footer'>
        Procurement Management and Inventory System • Ministry of Public Order and Safety <br/>
        <span class='hint'>Developed by: MCC Softwares</span>
      </div>
    </div>
  </div>
</body>
</html>"
    End Function

    Private Function BuildResetText(name As String, code As String, ttlMins As Integer) As String
        Dim n = If(name, "")
        Return $"Hi {n},

Here is your MPOS password reset code:

    {code}

This code expires in {ttlMins} minutes.
If you didn’t request this, you can safely ignore this email.

– MPOS System"
    End Function

    Private Function GenerateSixDigitCode() As String
        Dim bytes(3) As Byte
        Using rng As RandomNumberGenerator = RandomNumberGenerator.Create()
            rng.GetBytes(bytes)
        End Using
        Dim val As UInteger = BitConverter.ToUInt32(bytes, 0)
        Return (val Mod 1000000UI).ToString("D6")
    End Function

    Private Function ComputeSHA256(text As String) As Byte()
        Dim data = Encoding.UTF8.GetBytes(text)
        Using algo As System.Security.Cryptography.SHA256 = System.Security.Cryptography.SHA256.Create()
            Return algo.ComputeHash(data)
        End Using
    End Function

    Private Function BytesEqual(a As Byte(), b As Byte()) As Boolean
        If a Is Nothing OrElse b Is Nothing OrElse a.Length <> b.Length Then Return False
        Dim diff As Integer = 0
        For i = 0 To a.Length - 1
            diff = diff Or (a(i) Xor b(i))
        Next
        Return diff = 0
    End Function

    ' Password hashing (PBKDF2-SHA256). Stored format: PBKDF2$10000$<saltB64>$<keyB64>
    Private Function HashPasswordPBKDF2(password As String) As String
        Const iterations As Integer = 10000
        Using pbkdf2 As New Rfc2898DeriveBytes(password, 16, iterations, HashAlgorithmName.SHA256)
            Dim salt = pbkdf2.Salt
            Dim key = pbkdf2.GetBytes(32)
            Return $"PBKDF2${iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(key)}"
        End Using
    End Function

    Private Function VerifyPasswordFlexible(stored As String, provided As String) As Boolean
        If stored.StartsWith("PBKDF2$", StringComparison.OrdinalIgnoreCase) Then
            Dim parts = stored.Split("$"c)
            If parts.Length <> 4 Then Return False
            Dim iter = Integer.Parse(parts(1))
            Dim salt = Convert.FromBase64String(parts(2))
            Dim key = Convert.FromBase64String(parts(3))
            Using pbkdf2 As New Rfc2898DeriveBytes(provided, salt, iter, HashAlgorithmName.SHA256)
                Dim test = pbkdf2.GetBytes(32)
                Return BytesEqual(test, key)
            End Using
        End If

        ' Legacy plaintext fallback
        Return stored = provided
    End Function

    Private Sub Cmdshow_Click(sender As Object, e As EventArgs) Handles cmdshow.Click
        If txtpassword.PasswordChar = ChrW(0) Then
            txtpassword.PasswordChar = "●"c
        Else
            txtpassword.PasswordChar = ChrW(0)
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If txtfpassword.PasswordChar = ChrW(0) Then
            txtfpassword.PasswordChar = "●"c
            txtfrepassword.PasswordChar = "●"c
        Else
            txtfpassword.PasswordChar = ChrW(0)
            txtfrepassword.PasswordChar = ChrW(0)
        End If
    End Sub

    ' ─────────────────────────────────────────────────────────────────────
    ' Password rules (Forgot Password panel)
    ' ─────────────────────────────────────────────────────────────────────
    Private Sub ForgotPassword_TextChanged(sender As Object, e As EventArgs)
        UpdateForgotPasswordHints()
    End Sub

    Private Function IsPasswordStrong(pwd As String) As Boolean
        If String.IsNullOrEmpty(pwd) Then Return False
        Dim hasLower As Boolean = Regex.IsMatch(pwd, "[a-z]")
        Dim hasUpper As Boolean = Regex.IsMatch(pwd, "[A-Z]")
        Dim hasDigit As Boolean = Regex.IsMatch(pwd, "\d")
        Dim minLen As Boolean = pwd.Length >= 8
        ' Optional special char:
        ' Dim hasSpecial As Boolean = Regex.IsMatch(pwd, "[^a-zA-Z0-9]")
        Return hasLower AndAlso hasUpper AndAlso hasDigit AndAlso minLen ' AndAlso hasSpecial
    End Function

    Private Sub UpdateForgotPasswordHints()
        ' If label or fields are not visible yet, just ensure defaults
        If lblpasswordinfo Is Nothing Then Return

        Dim pwd As String = txtfpassword.Text
        Dim rep As String = txtfrepassword.Text

        Dim hasLower As Boolean = Regex.IsMatch(pwd, "[a-z]")
        Dim hasUpper As Boolean = Regex.IsMatch(pwd, "[A-Z]")
        Dim hasDigit As Boolean = Regex.IsMatch(pwd, "\d")
        Dim minLen As Boolean = pwd.Length >= 8
        ' Dim hasSpecial As Boolean = Regex.IsMatch(pwd, "[^a-zA-Z0-9]") ' optional
        Dim match As Boolean = (pwd.Length > 0 AndAlso pwd = rep)

        Dim lines As New List(Of String)
        lines.Add((If(hasLower, "✓ ", "✗ ")) & "At least one lowercase letter")
        lines.Add((If(hasUpper, "✓ ", "✗ ")) & "At least one uppercase letter")
        lines.Add((If(hasDigit, "✓ ", "✗ ")) & "At least one number")
        lines.Add((If(minLen, "✓ ", "✗ ")) & "Minimum 8 characters")
        ' lines.Add((If(hasSpecial, "✓ ", "✗ ")) & "At least one special character")
        lines.Add((If(match, "✓ ", "✗ ")) & "Passwords match")

        lblpasswordinfo.Text = String.Join(Environment.NewLine, lines)

        Dim allOk As Boolean = hasLower AndAlso hasUpper AndAlso hasDigit AndAlso minLen AndAlso match
        cmdchange.Enabled = allOk

        lblpasswordinfo.ForeColor = If(allOk, Color.LightGreen, Color.DarkRed)
    End Sub

End Class
