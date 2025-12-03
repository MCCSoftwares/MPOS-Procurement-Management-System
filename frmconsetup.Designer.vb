<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmconsetup
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmconsetup))
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.txtsqlpassword = New System.Windows.Forms.TextBox()
        Me.txtsqluser = New System.Windows.Forms.TextBox()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.txtipport = New System.Windows.Forms.TextBox()
        Me.txtipadd = New System.Windows.Forms.TextBox()
        Me.cmdsave = New System.Windows.Forms.Button()
        Me.cmdclose = New System.Windows.Forms.Button()
        Me.cmdcheck = New System.Windows.Forms.Button()
        Me.Panel1.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.SuspendLayout()
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.FromArgb(CType(CType(35, Byte), Integer), CType(CType(40, Byte), Integer), CType(CType(44, Byte), Integer))
        Me.Panel1.Controls.Add(Me.Label1)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(421, 62)
        Me.Panel1.TabIndex = 25
        '
        'Label1
        '
        Me.Label1.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Calibri", 20.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.White
        Me.Label1.Location = New System.Drawing.Point(62, 18)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(283, 33)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Network Configurations"
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.Label3)
        Me.GroupBox1.Controls.Add(Me.Label2)
        Me.GroupBox1.Controls.Add(Me.txtsqlpassword)
        Me.GroupBox1.Controls.Add(Me.txtsqluser)
        Me.GroupBox1.Location = New System.Drawing.Point(14, 126)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(394, 143)
        Me.GroupBox1.TabIndex = 26
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Server Login"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(73, 79)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(53, 13)
        Me.Label3.TabIndex = 3
        Me.Label3.Text = "Password"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(73, 35)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(79, 13)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "SQL Username"
        '
        'txtsqlpassword
        '
        Me.txtsqlpassword.Location = New System.Drawing.Point(76, 93)
        Me.txtsqlpassword.Name = "txtsqlpassword"
        Me.txtsqlpassword.PasswordChar = Global.Microsoft.VisualBasic.ChrW(8226)
        Me.txtsqlpassword.Size = New System.Drawing.Size(223, 20)
        Me.txtsqlpassword.TabIndex = 1
        '
        'txtsqluser
        '
        Me.txtsqluser.Location = New System.Drawing.Point(76, 51)
        Me.txtsqluser.Name = "txtsqluser"
        Me.txtsqluser.Size = New System.Drawing.Size(223, 20)
        Me.txtsqluser.TabIndex = 0
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.Label4)
        Me.GroupBox2.Controls.Add(Me.Label5)
        Me.GroupBox2.Controls.Add(Me.txtipport)
        Me.GroupBox2.Controls.Add(Me.txtipadd)
        Me.GroupBox2.Location = New System.Drawing.Point(14, 275)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(394, 122)
        Me.GroupBox2.TabIndex = 27
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "IP/TCP"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(73, 67)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(26, 13)
        Me.Label4.TabIndex = 7
        Me.Label4.Text = "Port"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(73, 23)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(92, 13)
        Me.Label5.TabIndex = 6
        Me.Label5.Text = "Server IP Address"
        '
        'txtipport
        '
        Me.txtipport.Location = New System.Drawing.Point(76, 81)
        Me.txtipport.Name = "txtipport"
        Me.txtipport.Size = New System.Drawing.Size(223, 20)
        Me.txtipport.TabIndex = 5
        '
        'txtipadd
        '
        Me.txtipadd.Location = New System.Drawing.Point(76, 39)
        Me.txtipadd.Name = "txtipadd"
        Me.txtipadd.Size = New System.Drawing.Size(223, 20)
        Me.txtipadd.TabIndex = 4
        '
        'cmdsave
        '
        Me.cmdsave.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cmdsave.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdsave.FlatAppearance.BorderSize = 0
        Me.cmdsave.Image = CType(resources.GetObject("cmdsave.Image"), System.Drawing.Image)
        Me.cmdsave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdsave.Location = New System.Drawing.Point(12, 406)
        Me.cmdsave.Name = "cmdsave"
        Me.cmdsave.Size = New System.Drawing.Size(195, 46)
        Me.cmdsave.TabIndex = 28
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
        Me.cmdclose.Location = New System.Drawing.Point(213, 406)
        Me.cmdclose.Name = "cmdclose"
        Me.cmdclose.Size = New System.Drawing.Size(195, 46)
        Me.cmdclose.TabIndex = 29
        Me.cmdclose.Text = "Close"
        Me.cmdclose.UseVisualStyleBackColor = True
        '
        'cmdcheck
        '
        Me.cmdcheck.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdcheck.Location = New System.Drawing.Point(143, 74)
        Me.cmdcheck.Name = "cmdcheck"
        Me.cmdcheck.Size = New System.Drawing.Size(138, 41)
        Me.cmdcheck.TabIndex = 30
        Me.cmdcheck.Text = "Check Status"
        Me.cmdcheck.UseVisualStyleBackColor = True
        '
        'frmconsetup
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(421, 464)
        Me.Controls.Add(Me.cmdcheck)
        Me.Controls.Add(Me.cmdsave)
        Me.Controls.Add(Me.cmdclose)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.Panel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmconsetup"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Network Configurations"
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents Panel1 As Panel
    Friend WithEvents Label1 As Label
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents Label3 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents txtsqlpassword As TextBox
    Friend WithEvents txtsqluser As TextBox
    Friend WithEvents Label4 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents txtipport As TextBox
    Friend WithEvents txtipadd As TextBox
    Friend WithEvents cmdsave As Button
    Friend WithEvents cmdclose As Button
    Friend WithEvents cmdcheck As Button
End Class
