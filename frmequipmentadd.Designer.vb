<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FrmEquipmentAdd
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmEquipmentAdd))
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.txtdesc = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.txtatitle = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.txtacode = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.txtipno = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.txtunit = New System.Windows.Forms.TextBox()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.cmdsave = New System.Windows.Forms.Button()
        Me.cmdclose = New System.Windows.Forms.Button()
        Me.LinkLblselect = New System.Windows.Forms.LinkLabel()
        Me.LinkLbldelete = New System.Windows.Forms.LinkLabel()
        Me.lblid = New System.Windows.Forms.Label()
        Me.lblCharCount = New System.Windows.Forms.Label()
        Me.Panel1.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.FromArgb(CType(CType(35, Byte), Integer), CType(CType(40, Byte), Integer), CType(CType(44, Byte), Integer))
        Me.Panel1.Controls.Add(Me.Label8)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(731, 51)
        Me.Panel1.TabIndex = 36
        '
        'Label8
        '
        Me.Label8.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("Calibri", 20.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label8.ForeColor = System.Drawing.Color.White
        Me.Label8.Location = New System.Drawing.Point(196, 9)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(357, 33)
        Me.Label8.TabIndex = 0
        Me.Label8.Text = "ITEM/SUPPLIES INFORMATION"
        '
        'txtdesc
        '
        Me.txtdesc.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtdesc.Location = New System.Drawing.Point(30, 97)
        Me.txtdesc.MaxLength = 4500
        Me.txtdesc.Multiline = True
        Me.txtdesc.Name = "txtdesc"
        Me.txtdesc.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtdesc.Size = New System.Drawing.Size(383, 192)
        Me.txtdesc.TabIndex = 37
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(27, 81)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(196, 13)
        Me.Label1.TabIndex = 38
        Me.Label1.Text = "Office Supplies & Equipment Description"
        Me.Label1.UseMnemonic = False
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(27, 309)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(70, 13)
        Me.Label2.TabIndex = 40
        Me.Label2.Text = "Account Title"
        Me.Label2.UseMnemonic = False
        '
        'txtatitle
        '
        Me.txtatitle.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtatitle.Location = New System.Drawing.Point(30, 325)
        Me.txtatitle.Name = "txtatitle"
        Me.txtatitle.Size = New System.Drawing.Size(383, 22)
        Me.txtatitle.TabIndex = 39
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(27, 351)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(75, 13)
        Me.Label3.TabIndex = 42
        Me.Label3.Text = "Account Code"
        Me.Label3.UseMnemonic = False
        '
        'txtacode
        '
        Me.txtacode.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtacode.Location = New System.Drawing.Point(30, 367)
        Me.txtacode.Name = "txtacode"
        Me.txtacode.Size = New System.Drawing.Size(383, 22)
        Me.txtacode.TabIndex = 41
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(27, 393)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(91, 13)
        Me.Label4.TabIndex = 44
        Me.Label4.Text = "Item/Property No."
        Me.Label4.UseMnemonic = False
        '
        'txtipno
        '
        Me.txtipno.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtipno.Location = New System.Drawing.Point(30, 409)
        Me.txtipno.Name = "txtipno"
        Me.txtipno.Size = New System.Drawing.Size(383, 22)
        Me.txtipno.TabIndex = 43
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(27, 434)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(26, 13)
        Me.Label5.TabIndex = 46
        Me.Label5.Text = "Unit"
        Me.Label5.UseMnemonic = False
        '
        'txtunit
        '
        Me.txtunit.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtunit.Location = New System.Drawing.Point(30, 450)
        Me.txtunit.Name = "txtunit"
        Me.txtunit.Size = New System.Drawing.Size(383, 22)
        Me.txtunit.TabIndex = 45
        '
        'PictureBox1
        '
        Me.PictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.PictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.PictureBox1.Location = New System.Drawing.Point(419, 96)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(299, 235)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.PictureBox1.TabIndex = 47
        Me.PictureBox1.TabStop = False
        '
        'cmdsave
        '
        Me.cmdsave.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cmdsave.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdsave.FlatAppearance.BorderSize = 0
        Me.cmdsave.Image = CType(resources.GetObject("cmdsave.Image"), System.Drawing.Image)
        Me.cmdsave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdsave.Location = New System.Drawing.Point(30, 481)
        Me.cmdsave.Name = "cmdsave"
        Me.cmdsave.Size = New System.Drawing.Size(195, 46)
        Me.cmdsave.TabIndex = 48
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
        Me.cmdclose.Location = New System.Drawing.Point(231, 481)
        Me.cmdclose.Name = "cmdclose"
        Me.cmdclose.Size = New System.Drawing.Size(182, 46)
        Me.cmdclose.TabIndex = 49
        Me.cmdclose.Text = "Close"
        Me.cmdclose.UseVisualStyleBackColor = True
        '
        'LinkLblselect
        '
        Me.LinkLblselect.AutoSize = True
        Me.LinkLblselect.Location = New System.Drawing.Point(416, 336)
        Me.LinkLblselect.Name = "LinkLblselect"
        Me.LinkLblselect.Size = New System.Drawing.Size(68, 13)
        Me.LinkLblselect.TabIndex = 50
        Me.LinkLblselect.TabStop = True
        Me.LinkLblselect.Text = "Select Photo"
        '
        'LinkLbldelete
        '
        Me.LinkLbldelete.AutoSize = True
        Me.LinkLbldelete.LinkColor = System.Drawing.Color.Red
        Me.LinkLbldelete.Location = New System.Drawing.Point(649, 336)
        Me.LinkLbldelete.Name = "LinkLbldelete"
        Me.LinkLbldelete.Size = New System.Drawing.Size(69, 13)
        Me.LinkLbldelete.TabIndex = 51
        Me.LinkLbldelete.TabStop = True
        Me.LinkLbldelete.Text = "Delete Photo"
        '
        'lblid
        '
        Me.lblid.AutoSize = True
        Me.lblid.Location = New System.Drawing.Point(520, 66)
        Me.lblid.Name = "lblid"
        Me.lblid.Size = New System.Drawing.Size(13, 13)
        Me.lblid.TabIndex = 52
        Me.lblid.Text = "0"
        Me.lblid.Visible = False
        '
        'lblCharCount
        '
        Me.lblCharCount.Font = New System.Drawing.Font("Calibri", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblCharCount.Location = New System.Drawing.Point(256, 290)
        Me.lblCharCount.Name = "lblCharCount"
        Me.lblCharCount.Size = New System.Drawing.Size(156, 19)
        Me.lblCharCount.TabIndex = 53
        Me.lblCharCount.Text = "3500/3500 Characters left"
        Me.lblCharCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'FrmEquipmentAdd
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(731, 556)
        Me.Controls.Add(Me.lblCharCount)
        Me.Controls.Add(Me.lblid)
        Me.Controls.Add(Me.LinkLbldelete)
        Me.Controls.Add(Me.LinkLblselect)
        Me.Controls.Add(Me.cmdsave)
        Me.Controls.Add(Me.cmdclose)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.txtunit)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.txtipno)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.txtacode)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.txtatitle)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.txtdesc)
        Me.Controls.Add(Me.Panel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "FrmEquipmentAdd"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Item/Equipment Information"
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Panel1 As Panel
    Friend WithEvents Label8 As Label
    Friend WithEvents txtdesc As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents txtatitle As TextBox
    Friend WithEvents Label3 As Label
    Friend WithEvents txtacode As TextBox
    Friend WithEvents Label4 As Label
    Friend WithEvents txtipno As TextBox
    Friend WithEvents Label5 As Label
    Friend WithEvents txtunit As TextBox
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents cmdsave As Button
    Friend WithEvents cmdclose As Button
    Friend WithEvents LinkLblselect As LinkLabel
    Friend WithEvents LinkLbldelete As LinkLabel
    Friend WithEvents lblid As Label
    Friend WithEvents lblCharCount As Label
End Class
