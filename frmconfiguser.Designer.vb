<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmconfiguser
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
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmconfiguser))
        Me.txtfname = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.txtlname = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.combooffsec = New System.Windows.Forms.ComboBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.comboula = New System.Windows.Forms.ComboBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.txtuserid = New System.Windows.Forms.TextBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.txtpassword = New System.Windows.Forms.TextBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.txtemail = New System.Windows.Forms.TextBox()
        Me.cmdsave = New System.Windows.Forms.Button()
        Me.cmdclose = New System.Windows.Forms.Button()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.txtrepassword = New System.Windows.Forms.TextBox()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.lbltitle = New System.Windows.Forms.Label()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.lblid = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.txtposition = New System.Windows.Forms.TextBox()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.lblpasswordinfo = New System.Windows.Forms.Label()
        Me.cmdshow = New System.Windows.Forms.Button()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.Panel4 = New System.Windows.Forms.Panel()
        Me.Panel1.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.Panel3.SuspendLayout()
        Me.Panel4.SuspendLayout()
        Me.SuspendLayout()
        '
        'txtfname
        '
        Me.txtfname.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtfname.Location = New System.Drawing.Point(16, 30)
        Me.txtfname.Name = "txtfname"
        Me.txtfname.Size = New System.Drawing.Size(331, 24)
        Me.txtfname.TabIndex = 0
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(13, 14)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(57, 13)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "First Name"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(13, 58)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(58, 13)
        Me.Label2.TabIndex = 3
        Me.Label2.Text = "Last Name"
        '
        'txtlname
        '
        Me.txtlname.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtlname.Location = New System.Drawing.Point(16, 74)
        Me.txtlname.Name = "txtlname"
        Me.txtlname.Size = New System.Drawing.Size(331, 24)
        Me.txtlname.TabIndex = 1
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(14, 143)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(76, 13)
        Me.Label3.TabIndex = 37
        Me.Label3.Text = "Office/Section"
        '
        'combooffsec
        '
        Me.combooffsec.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.combooffsec.FormattingEnabled = True
        Me.combooffsec.Items.AddRange(New Object() {"OFFICE OF THE MINISTER", "OFFICE OF THE DEPUTY MINISTER", "OFFICE OF DIRECTOR GENERAL", "ADMINISTRATIVE AND FINANCE DIVISION (CAO)", "ACCOUNTING SECTION", "BUDGET SECTION", "PROCUREMENT MANAGEMENT SECTION", "CASH SECTION", "ARCHIVES AND RECORDS SECTION", "HUMAN RESOURCE MANAGEMENT SECTION", "SUPPLY SECTION", "GENERAL SERVICES SECTION", "LEGAL AND LEGISLATIVE LIAISON SECTION", "PLANNING SECTION", "INFORMATION AND COMMUNICATION SECTION", "INTERNAL AUDIT SECTION", "BANGSAMORO PEACE RECONCILIATION AND UNIFICATION SERVICES", "PEACE EDUCATION DIVISION", "ALTERNATIVE DISPUTE RESOLUTION DIVISION (CAO-V)", "COMMUNITY AFFAIRS SECTION (MAG)", "COMMUNITY AFFAIRS SECTION (LDS)", "COMMUNITY AFFAIRS SECTION (SGA)", "COMMUNITY AFFAIRS SECTION (BAS)", "COMMUNITY AFFAIRS SECTION (SUL)", "COMMUNITY AFFAIRS SECTION (TAW)", "HOME AFFAIRS SERVICES", "LAW ENFORCEMENT COORDINATION DIVISION", "CIVILIAN INTELLIGENCE AND INVESTIGATION DIVISION"})
        Me.combooffsec.Location = New System.Drawing.Point(17, 159)
        Me.combooffsec.Name = "combooffsec"
        Me.combooffsec.Size = New System.Drawing.Size(331, 26)
        Me.combooffsec.TabIndex = 3
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(14, 5)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(83, 13)
        Me.Label4.TabIndex = 39
        Me.Label4.Text = "Level of Access"
        '
        'comboula
        '
        Me.comboula.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.comboula.FormattingEnabled = True
        Me.comboula.Items.AddRange(New Object() {"Administrator", "Procurement Officer", "Supply Officer", "End User"})
        Me.comboula.Location = New System.Drawing.Point(17, 23)
        Me.comboula.Name = "comboula"
        Me.comboula.Size = New System.Drawing.Size(330, 26)
        Me.comboula.TabIndex = 6
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(14, 9)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(43, 13)
        Me.Label5.TabIndex = 41
        Me.Label5.Text = "User ID"
        '
        'txtuserid
        '
        Me.txtuserid.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtuserid.Location = New System.Drawing.Point(17, 25)
        Me.txtuserid.Name = "txtuserid"
        Me.txtuserid.Size = New System.Drawing.Size(330, 26)
        Me.txtuserid.TabIndex = 6
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(14, 52)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(53, 13)
        Me.Label6.TabIndex = 43
        Me.Label6.Text = "Password"
        '
        'txtpassword
        '
        Me.txtpassword.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtpassword.Location = New System.Drawing.Point(17, 68)
        Me.txtpassword.Name = "txtpassword"
        Me.txtpassword.PasswordChar = Global.Microsoft.VisualBasic.ChrW(9679)
        Me.txtpassword.Size = New System.Drawing.Size(296, 26)
        Me.txtpassword.TabIndex = 7
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(13, 101)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(194, 13)
        Me.Label7.TabIndex = 45
        Me.Label7.Text = "Email (This is used to Reset Passwords)"
        '
        'txtemail
        '
        Me.txtemail.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtemail.Location = New System.Drawing.Point(16, 117)
        Me.txtemail.Name = "txtemail"
        Me.txtemail.Size = New System.Drawing.Size(331, 24)
        Me.txtemail.TabIndex = 2
        '
        'cmdsave
        '
        Me.cmdsave.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cmdsave.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdsave.FlatAppearance.BorderSize = 0
        Me.cmdsave.Image = CType(resources.GetObject("cmdsave.Image"), System.Drawing.Image)
        Me.cmdsave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdsave.Location = New System.Drawing.Point(42, 632)
        Me.cmdsave.Name = "cmdsave"
        Me.cmdsave.Size = New System.Drawing.Size(234, 46)
        Me.cmdsave.TabIndex = 9
        Me.cmdsave.Text = "Save"
        Me.cmdsave.UseVisualStyleBackColor = True
        '
        'cmdclose
        '
        Me.cmdclose.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cmdclose.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdclose.FlatAppearance.BorderSize = 0
        Me.cmdclose.Image = CType(resources.GetObject("cmdclose.Image"), System.Drawing.Image)
        Me.cmdclose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdclose.Location = New System.Drawing.Point(282, 632)
        Me.cmdclose.Name = "cmdclose"
        Me.cmdclose.Size = New System.Drawing.Size(120, 46)
        Me.cmdclose.TabIndex = 10
        Me.cmdclose.Text = "Close"
        Me.cmdclose.UseVisualStyleBackColor = True
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(14, 174)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(90, 13)
        Me.Label8.TabIndex = 66
        Me.Label8.Text = "Retype-Password"
        '
        'txtrepassword
        '
        Me.txtrepassword.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtrepassword.Location = New System.Drawing.Point(17, 190)
        Me.txtrepassword.Name = "txtrepassword"
        Me.txtrepassword.PasswordChar = Global.Microsoft.VisualBasic.ChrW(9679)
        Me.txtrepassword.Size = New System.Drawing.Size(330, 26)
        Me.txtrepassword.TabIndex = 8
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.FromArgb(CType(CType(35, Byte), Integer), CType(CType(40, Byte), Integer), CType(CType(44, Byte), Integer))
        Me.Panel1.Controls.Add(Me.lbltitle)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(441, 62)
        Me.Panel1.TabIndex = 68
        '
        'lbltitle
        '
        Me.lbltitle.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.lbltitle.Font = New System.Drawing.Font("Calibri", 20.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbltitle.ForeColor = System.Drawing.Color.White
        Me.lbltitle.Location = New System.Drawing.Point(12, 14)
        Me.lbltitle.Name = "lbltitle"
        Me.lbltitle.Size = New System.Drawing.Size(417, 33)
        Me.lbltitle.TabIndex = 2
        Me.lbltitle.Text = "Account"
        Me.lbltitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblid
        '
        Me.lblid.AutoSize = True
        Me.lblid.Location = New System.Drawing.Point(288, 2)
        Me.lblid.Name = "lblid"
        Me.lblid.Size = New System.Drawing.Size(0, 13)
        Me.lblid.TabIndex = 69
        Me.lblid.Visible = False
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(14, 187)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(44, 13)
        Me.Label9.TabIndex = 71
        Me.Label9.Text = "Position"
        '
        'txtposition
        '
        Me.txtposition.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtposition.Location = New System.Drawing.Point(17, 203)
        Me.txtposition.Name = "txtposition"
        Me.txtposition.Size = New System.Drawing.Size(330, 24)
        Me.txtposition.TabIndex = 4
        '
        'Panel2
        '
        Me.Panel2.BackColor = System.Drawing.Color.WhiteSmoke
        Me.Panel2.Controls.Add(Me.lblpasswordinfo)
        Me.Panel2.Controls.Add(Me.cmdshow)
        Me.Panel2.Controls.Add(Me.Label8)
        Me.Panel2.Controls.Add(Me.txtrepassword)
        Me.Panel2.Controls.Add(Me.Label6)
        Me.Panel2.Controls.Add(Me.txtpassword)
        Me.Panel2.Controls.Add(Me.Label5)
        Me.Panel2.Controls.Add(Me.txtuserid)
        Me.Panel2.Location = New System.Drawing.Point(41, 389)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(362, 236)
        Me.Panel2.TabIndex = 72
        '
        'lblpasswordinfo
        '
        Me.lblpasswordinfo.AutoSize = True
        Me.lblpasswordinfo.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblpasswordinfo.ForeColor = System.Drawing.Color.DarkRed
        Me.lblpasswordinfo.Location = New System.Drawing.Point(19, 98)
        Me.lblpasswordinfo.Name = "lblpasswordinfo"
        Me.lblpasswordinfo.Size = New System.Drawing.Size(47, 12)
        Me.lblpasswordinfo.TabIndex = 68
        Me.lblpasswordinfo.Text = "Password"
        '
        'cmdshow
        '
        Me.cmdshow.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdshow.Image = CType(resources.GetObject("cmdshow.Image"), System.Drawing.Image)
        Me.cmdshow.Location = New System.Drawing.Point(318, 67)
        Me.cmdshow.Name = "cmdshow"
        Me.cmdshow.Size = New System.Drawing.Size(28, 28)
        Me.cmdshow.TabIndex = 67
        Me.cmdshow.UseVisualStyleBackColor = True
        '
        'Panel3
        '
        Me.Panel3.BackColor = System.Drawing.Color.WhiteSmoke
        Me.Panel3.Controls.Add(Me.Label9)
        Me.Panel3.Controls.Add(Me.txtposition)
        Me.Panel3.Controls.Add(Me.lblid)
        Me.Panel3.Controls.Add(Me.Label7)
        Me.Panel3.Controls.Add(Me.txtemail)
        Me.Panel3.Controls.Add(Me.Label3)
        Me.Panel3.Controls.Add(Me.combooffsec)
        Me.Panel3.Controls.Add(Me.Label2)
        Me.Panel3.Controls.Add(Me.txtlname)
        Me.Panel3.Controls.Add(Me.Label1)
        Me.Panel3.Controls.Add(Me.txtfname)
        Me.Panel3.Location = New System.Drawing.Point(41, 71)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(361, 239)
        Me.Panel3.TabIndex = 73
        '
        'Panel4
        '
        Me.Panel4.BackColor = System.Drawing.Color.WhiteSmoke
        Me.Panel4.Controls.Add(Me.Label4)
        Me.Panel4.Controls.Add(Me.comboula)
        Me.Panel4.Location = New System.Drawing.Point(41, 316)
        Me.Panel4.Name = "Panel4"
        Me.Panel4.Size = New System.Drawing.Size(360, 65)
        Me.Panel4.TabIndex = 5
        '
        'frmconfiguser
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(441, 695)
        Me.Controls.Add(Me.Panel4)
        Me.Controls.Add(Me.Panel3)
        Me.Controls.Add(Me.Panel2)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.cmdsave)
        Me.Controls.Add(Me.cmdclose)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmconfiguser"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "User Account"
        Me.Panel1.ResumeLayout(False)
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        Me.Panel3.ResumeLayout(False)
        Me.Panel3.PerformLayout()
        Me.Panel4.ResumeLayout(False)
        Me.Panel4.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents txtfname As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents txtlname As TextBox
    Friend WithEvents Label3 As Label
    Friend WithEvents combooffsec As ComboBox
    Friend WithEvents Label4 As Label
    Friend WithEvents comboula As ComboBox
    Friend WithEvents Label5 As Label
    Friend WithEvents txtuserid As TextBox
    Friend WithEvents Label6 As Label
    Friend WithEvents txtpassword As TextBox
    Friend WithEvents Label7 As Label
    Friend WithEvents txtemail As TextBox
    Friend WithEvents cmdsave As Button
    Friend WithEvents cmdclose As Button
    Friend WithEvents Label8 As Label
    Friend WithEvents txtrepassword As TextBox
    Friend WithEvents ToolTip1 As ToolTip
    Friend WithEvents Panel1 As Panel
    Friend WithEvents lbltitle As Label
    Friend WithEvents lblid As Label
    Friend WithEvents Label9 As Label
    Friend WithEvents txtposition As TextBox
    Friend WithEvents Panel2 As Panel
    Friend WithEvents cmdshow As Button
    Friend WithEvents Panel3 As Panel
    Friend WithEvents Panel4 As Panel
    Friend WithEvents lblpasswordinfo As Label
End Class
