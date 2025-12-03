<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmlogin
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmlogin))
        Me.txtid = New System.Windows.Forms.TextBox()
        Me.txtpassword = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.cmdlogin = New System.Windows.Forms.Button()
        Me.panelforgot = New System.Windows.Forms.Panel()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.cmdcancel = New System.Windows.Forms.Button()
        Me.lblmessage = New System.Windows.Forms.Label()
        Me.cmdchange = New System.Windows.Forms.Button()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.txtfrepassword = New System.Windows.Forms.TextBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.txtfpassword = New System.Windows.Forms.TextBox()
        Me.cmdsend = New System.Windows.Forms.Button()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.txtfcode = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.txtfemail = New System.Windows.Forms.TextBox()
        Me.txtfuserid = New System.Windows.Forms.TextBox()
        Me.lbldb = New System.Windows.Forms.Label()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.cmdexit = New System.Windows.Forms.Button()
        Me.panellogin = New System.Windows.Forms.Panel()
        Me.cmdshow = New System.Windows.Forms.Button()
        Me.lblpasswordinfo = New System.Windows.Forms.Label()
        Me.panelforgot.SuspendLayout()
        Me.panellogin.SuspendLayout()
        Me.SuspendLayout()
        '
        'txtid
        '
        Me.txtid.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtid.Location = New System.Drawing.Point(9, 32)
        Me.txtid.Name = "txtid"
        Me.txtid.Size = New System.Drawing.Size(317, 26)
        Me.txtid.TabIndex = 0
        '
        'txtpassword
        '
        Me.txtpassword.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtpassword.Location = New System.Drawing.Point(9, 82)
        Me.txtpassword.Name = "txtpassword"
        Me.txtpassword.PasswordChar = Global.Microsoft.VisualBasic.ChrW(9679)
        Me.txtpassword.Size = New System.Drawing.Size(283, 26)
        Me.txtpassword.TabIndex = 1
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.BackColor = System.Drawing.Color.Transparent
        Me.Label1.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.White
        Me.Label1.Location = New System.Drawing.Point(6, 16)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(49, 15)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "User ID"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.BackColor = System.Drawing.Color.Transparent
        Me.Label2.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.Color.White
        Me.Label2.Location = New System.Drawing.Point(6, 66)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(63, 15)
        Me.Label2.TabIndex = 3
        Me.Label2.Text = "Password"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.BackColor = System.Drawing.Color.Transparent
        Me.Label4.Cursor = System.Windows.Forms.Cursors.Hand
        Me.Label4.Font = New System.Drawing.Font("Arial", 9.0!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Underline), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.ForeColor = System.Drawing.Color.Gold
        Me.Label4.Location = New System.Drawing.Point(9, 117)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(104, 15)
        Me.Label4.TabIndex = 5
        Me.Label4.Text = "Forgot Password"
        '
        'cmdlogin
        '
        Me.cmdlogin.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdlogin.Image = CType(resources.GetObject("cmdlogin.Image"), System.Drawing.Image)
        Me.cmdlogin.Location = New System.Drawing.Point(9, 147)
        Me.cmdlogin.Name = "cmdlogin"
        Me.cmdlogin.Size = New System.Drawing.Size(172, 38)
        Me.cmdlogin.TabIndex = 6
        Me.cmdlogin.UseVisualStyleBackColor = True
        '
        'panelforgot
        '
        Me.panelforgot.BackColor = System.Drawing.Color.Transparent
        Me.panelforgot.Controls.Add(Me.lblpasswordinfo)
        Me.panelforgot.Controls.Add(Me.Button1)
        Me.panelforgot.Controls.Add(Me.cmdcancel)
        Me.panelforgot.Controls.Add(Me.lblmessage)
        Me.panelforgot.Controls.Add(Me.cmdchange)
        Me.panelforgot.Controls.Add(Me.Label9)
        Me.panelforgot.Controls.Add(Me.txtfrepassword)
        Me.panelforgot.Controls.Add(Me.Label8)
        Me.panelforgot.Controls.Add(Me.txtfpassword)
        Me.panelforgot.Controls.Add(Me.cmdsend)
        Me.panelforgot.Controls.Add(Me.Label7)
        Me.panelforgot.Controls.Add(Me.txtfcode)
        Me.panelforgot.Controls.Add(Me.Label5)
        Me.panelforgot.Controls.Add(Me.Label6)
        Me.panelforgot.Controls.Add(Me.txtfemail)
        Me.panelforgot.Controls.Add(Me.txtfuserid)
        Me.panelforgot.Location = New System.Drawing.Point(530, 12)
        Me.panelforgot.Name = "panelforgot"
        Me.panelforgot.Size = New System.Drawing.Size(343, 407)
        Me.panelforgot.TabIndex = 7
        Me.panelforgot.Visible = False
        '
        'Button1
        '
        Me.Button1.Cursor = System.Windows.Forms.Cursors.Hand
        Me.Button1.Image = CType(resources.GetObject("Button1.Image"), System.Drawing.Image)
        Me.Button1.Location = New System.Drawing.Point(300, 210)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(28, 28)
        Me.Button1.TabIndex = 17
        Me.Button1.UseVisualStyleBackColor = True
        '
        'cmdcancel
        '
        Me.cmdcancel.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdcancel.Image = CType(resources.GetObject("cmdcancel.Image"), System.Drawing.Image)
        Me.cmdcancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdcancel.Location = New System.Drawing.Point(180, 357)
        Me.cmdcancel.Name = "cmdcancel"
        Me.cmdcancel.Size = New System.Drawing.Size(150, 38)
        Me.cmdcancel.TabIndex = 16
        Me.cmdcancel.Text = "Cancel"
        Me.cmdcancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdcancel.UseVisualStyleBackColor = True
        '
        'lblmessage
        '
        Me.lblmessage.BackColor = System.Drawing.Color.Transparent
        Me.lblmessage.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblmessage.ForeColor = System.Drawing.Color.White
        Me.lblmessage.Location = New System.Drawing.Point(13, 161)
        Me.lblmessage.Name = "lblmessage"
        Me.lblmessage.Size = New System.Drawing.Size(295, 34)
        Me.lblmessage.TabIndex = 15
        Me.lblmessage.Text = "An email with a reset code will be sent to your registered email. Please check yo" &
    "ur Inbox or Spam."
        '
        'cmdchange
        '
        Me.cmdchange.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdchange.Image = CType(resources.GetObject("cmdchange.Image"), System.Drawing.Image)
        Me.cmdchange.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdchange.Location = New System.Drawing.Point(13, 357)
        Me.cmdchange.Name = "cmdchange"
        Me.cmdchange.Size = New System.Drawing.Size(161, 38)
        Me.cmdchange.TabIndex = 8
        Me.cmdchange.Text = "Reset Password"
        Me.cmdchange.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdchange.UseVisualStyleBackColor = True
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.BackColor = System.Drawing.Color.Transparent
        Me.Label9.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label9.ForeColor = System.Drawing.Color.White
        Me.Label9.Location = New System.Drawing.Point(10, 303)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(111, 15)
        Me.Label9.TabIndex = 14
        Me.Label9.Text = "Re-Type Password"
        '
        'txtfrepassword
        '
        Me.txtfrepassword.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtfrepassword.Location = New System.Drawing.Point(13, 319)
        Me.txtfrepassword.Name = "txtfrepassword"
        Me.txtfrepassword.PasswordChar = Global.Microsoft.VisualBasic.ChrW(9679)
        Me.txtfrepassword.Size = New System.Drawing.Size(315, 26)
        Me.txtfrepassword.TabIndex = 13
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.BackColor = System.Drawing.Color.Transparent
        Me.Label8.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label8.ForeColor = System.Drawing.Color.White
        Me.Label8.Location = New System.Drawing.Point(10, 195)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(91, 15)
        Me.Label8.TabIndex = 12
        Me.Label8.Text = "New Password"
        '
        'txtfpassword
        '
        Me.txtfpassword.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtfpassword.Location = New System.Drawing.Point(13, 211)
        Me.txtfpassword.Name = "txtfpassword"
        Me.txtfpassword.PasswordChar = Global.Microsoft.VisualBasic.ChrW(9679)
        Me.txtfpassword.Size = New System.Drawing.Size(283, 26)
        Me.txtfpassword.TabIndex = 11
        '
        'cmdsend
        '
        Me.cmdsend.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdsend.Location = New System.Drawing.Point(221, 130)
        Me.cmdsend.Name = "cmdsend"
        Me.cmdsend.Size = New System.Drawing.Size(109, 30)
        Me.cmdsend.TabIndex = 10
        Me.cmdsend.Text = "Send Reset Code"
        Me.cmdsend.UseVisualStyleBackColor = True
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.BackColor = System.Drawing.Color.Transparent
        Me.Label7.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label7.ForeColor = System.Drawing.Color.White
        Me.Label7.Location = New System.Drawing.Point(10, 116)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(73, 15)
        Me.Label7.TabIndex = 9
        Me.Label7.Text = "Reset Code"
        '
        'txtfcode
        '
        Me.txtfcode.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtfcode.Location = New System.Drawing.Point(13, 132)
        Me.txtfcode.Name = "txtfcode"
        Me.txtfcode.Size = New System.Drawing.Size(202, 26)
        Me.txtfcode.TabIndex = 8
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.BackColor = System.Drawing.Color.Transparent
        Me.Label5.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.ForeColor = System.Drawing.Color.White
        Me.Label5.Location = New System.Drawing.Point(10, 67)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(39, 15)
        Me.Label5.TabIndex = 7
        Me.Label5.Text = "Email"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.BackColor = System.Drawing.Color.Transparent
        Me.Label6.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.ForeColor = System.Drawing.Color.White
        Me.Label6.Location = New System.Drawing.Point(10, 17)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(49, 15)
        Me.Label6.TabIndex = 6
        Me.Label6.Text = "User ID"
        '
        'txtfemail
        '
        Me.txtfemail.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtfemail.Location = New System.Drawing.Point(13, 83)
        Me.txtfemail.Name = "txtfemail"
        Me.txtfemail.Size = New System.Drawing.Size(317, 26)
        Me.txtfemail.TabIndex = 5
        '
        'txtfuserid
        '
        Me.txtfuserid.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtfuserid.Location = New System.Drawing.Point(13, 33)
        Me.txtfuserid.Name = "txtfuserid"
        Me.txtfuserid.Size = New System.Drawing.Size(317, 26)
        Me.txtfuserid.TabIndex = 4
        '
        'lbldb
        '
        Me.lbldb.Location = New System.Drawing.Point(521, 423)
        Me.lbldb.Name = "lbldb"
        Me.lbldb.Size = New System.Drawing.Size(97, 19)
        Me.lbldb.TabIndex = 8
        Me.lbldb.Visible = False
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.BackColor = System.Drawing.Color.Transparent
        Me.Label11.Cursor = System.Windows.Forms.Cursors.Hand
        Me.Label11.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label11.ForeColor = System.Drawing.Color.White
        Me.Label11.Location = New System.Drawing.Point(638, 422)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(136, 15)
        Me.Label11.TabIndex = 9
        Me.Label11.Text = "Network Configurations"
        '
        'cmdexit
        '
        Me.cmdexit.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdexit.Image = CType(resources.GetObject("cmdexit.Image"), System.Drawing.Image)
        Me.cmdexit.Location = New System.Drawing.Point(187, 147)
        Me.cmdexit.Name = "cmdexit"
        Me.cmdexit.Size = New System.Drawing.Size(139, 38)
        Me.cmdexit.TabIndex = 10
        Me.cmdexit.UseVisualStyleBackColor = True
        '
        'panellogin
        '
        Me.panellogin.BackColor = System.Drawing.Color.Transparent
        Me.panellogin.Controls.Add(Me.cmdshow)
        Me.panellogin.Controls.Add(Me.cmdexit)
        Me.panellogin.Controls.Add(Me.cmdlogin)
        Me.panellogin.Controls.Add(Me.Label4)
        Me.panellogin.Controls.Add(Me.Label2)
        Me.panellogin.Controls.Add(Me.Label1)
        Me.panellogin.Controls.Add(Me.txtpassword)
        Me.panellogin.Controls.Add(Me.txtid)
        Me.panellogin.Location = New System.Drawing.Point(540, 117)
        Me.panellogin.Name = "panellogin"
        Me.panellogin.Size = New System.Drawing.Size(350, 211)
        Me.panellogin.TabIndex = 11
        '
        'cmdshow
        '
        Me.cmdshow.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdshow.Image = CType(resources.GetObject("cmdshow.Image"), System.Drawing.Image)
        Me.cmdshow.Location = New System.Drawing.Point(298, 81)
        Me.cmdshow.Name = "cmdshow"
        Me.cmdshow.Size = New System.Drawing.Size(28, 28)
        Me.cmdshow.TabIndex = 11
        Me.cmdshow.UseVisualStyleBackColor = True
        '
        'lblpasswordinfo
        '
        Me.lblpasswordinfo.AutoSize = True
        Me.lblpasswordinfo.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblpasswordinfo.ForeColor = System.Drawing.Color.LightGreen
        Me.lblpasswordinfo.Location = New System.Drawing.Point(13, 240)
        Me.lblpasswordinfo.Name = "lblpasswordinfo"
        Me.lblpasswordinfo.Size = New System.Drawing.Size(47, 12)
        Me.lblpasswordinfo.TabIndex = 69
        Me.lblpasswordinfo.Text = "Password"
        '
        'frmlogin
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackgroundImage = CType(resources.GetObject("$this.BackgroundImage"), System.Drawing.Image)
        Me.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.ClientSize = New System.Drawing.Size(891, 446)
        Me.Controls.Add(Me.Label11)
        Me.Controls.Add(Me.lbldb)
        Me.Controls.Add(Me.panellogin)
        Me.Controls.Add(Me.panelforgot)
        Me.DoubleBuffered = True
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmlogin"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Login to System"
        Me.panelforgot.ResumeLayout(False)
        Me.panelforgot.PerformLayout()
        Me.panellogin.ResumeLayout(False)
        Me.panellogin.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents txtid As TextBox
    Friend WithEvents txtpassword As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents cmdlogin As Button
    Friend WithEvents panelforgot As Panel
    Friend WithEvents lblmessage As Label
    Friend WithEvents cmdchange As Button
    Friend WithEvents Label9 As Label
    Friend WithEvents txtfrepassword As TextBox
    Friend WithEvents Label8 As Label
    Friend WithEvents txtfpassword As TextBox
    Friend WithEvents cmdsend As Button
    Friend WithEvents Label7 As Label
    Friend WithEvents txtfcode As TextBox
    Friend WithEvents Label5 As Label
    Friend WithEvents Label6 As Label
    Friend WithEvents txtfemail As TextBox
    Friend WithEvents txtfuserid As TextBox
    Friend WithEvents lbldb As Label
    Friend WithEvents Label11 As Label
    Friend WithEvents cmdexit As Button
    Friend WithEvents panellogin As Panel
    Friend WithEvents cmdcancel As Button
    Friend WithEvents cmdshow As Button
    Friend WithEvents Button1 As Button
    Friend WithEvents lblpasswordinfo As Label
End Class
