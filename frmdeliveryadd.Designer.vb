<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmdeliveryadd
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmdeliveryadd))
        Me.cmdconfirm = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.txtprno = New System.Windows.Forms.TextBox()
        Me.txtpono = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.txtreqoff = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.txtrcenter = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.txtpdelivery = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.txtddelivery = New System.Windows.Forms.TextBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.txtcname = New System.Windows.Forms.TextBox()
        Me.lbl = New System.Windows.Forms.Label()
        Me.cmdclose = New System.Windows.Forms.Button()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.lbltitle = New System.Windows.Forms.Label()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.lblpoid = New System.Windows.Forms.Label()
        Me.lblprid = New System.Windows.Forms.Label()
        Me.lblpodate = New System.Windows.Forms.Label()
        Me.txtfcluster = New System.Windows.Forms.TextBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.txtttype = New System.Windows.Forms.TextBox()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Panel1.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel2.SuspendLayout()
        Me.SuspendLayout()
        '
        'cmdconfirm
        '
        Me.cmdconfirm.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cmdconfirm.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdconfirm.FlatAppearance.BorderSize = 0
        Me.cmdconfirm.Image = CType(resources.GetObject("cmdconfirm.Image"), System.Drawing.Image)
        Me.cmdconfirm.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdconfirm.Location = New System.Drawing.Point(31, 440)
        Me.cmdconfirm.Name = "cmdconfirm"
        Me.cmdconfirm.Size = New System.Drawing.Size(225, 46)
        Me.cmdconfirm.TabIndex = 98
        Me.cmdconfirm.Text = "Confirmed"
        Me.cmdconfirm.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(259, 78)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(62, 13)
        Me.Label1.TabIndex = 99
        Me.Label1.Text = "PR Number"
        '
        'txtprno
        '
        Me.txtprno.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtprno.Location = New System.Drawing.Point(262, 94)
        Me.txtprno.Name = "txtprno"
        Me.txtprno.Size = New System.Drawing.Size(225, 22)
        Me.txtprno.TabIndex = 100
        Me.txtprno.TabStop = False
        '
        'txtpono
        '
        Me.txtpono.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtpono.Location = New System.Drawing.Point(31, 94)
        Me.txtpono.Name = "txtpono"
        Me.txtpono.Size = New System.Drawing.Size(225, 22)
        Me.txtpono.TabIndex = 102
        Me.txtpono.TabStop = False
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(28, 78)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(62, 13)
        Me.Label2.TabIndex = 101
        Me.Label2.Text = "PO Number"
        '
        'txtreqoff
        '
        Me.txtreqoff.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtreqoff.Location = New System.Drawing.Point(31, 182)
        Me.txtreqoff.Name = "txtreqoff"
        Me.txtreqoff.Size = New System.Drawing.Size(320, 22)
        Me.txtreqoff.TabIndex = 104
        Me.txtreqoff.TabStop = False
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(28, 166)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(92, 13)
        Me.Label3.TabIndex = 103
        Me.Label3.Text = "Requesting Office"
        '
        'txtrcenter
        '
        Me.txtrcenter.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtrcenter.Location = New System.Drawing.Point(357, 182)
        Me.txtrcenter.Name = "txtrcenter"
        Me.txtrcenter.Size = New System.Drawing.Size(131, 22)
        Me.txtrcenter.TabIndex = 106
        Me.txtrcenter.TabStop = False
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(354, 166)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(133, 13)
        Me.Label4.TabIndex = 105
        Me.Label4.Text = "Responsibility Center Code"
        '
        'txtpdelivery
        '
        Me.txtpdelivery.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtpdelivery.Location = New System.Drawing.Point(31, 319)
        Me.txtpdelivery.Name = "txtpdelivery"
        Me.txtpdelivery.Size = New System.Drawing.Size(290, 22)
        Me.txtpdelivery.TabIndex = 108
        Me.txtpdelivery.TabStop = False
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(28, 303)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(87, 13)
        Me.Label5.TabIndex = 107
        Me.Label5.Text = "Place of Delivery"
        '
        'txtddelivery
        '
        Me.txtddelivery.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtddelivery.Location = New System.Drawing.Point(326, 319)
        Me.txtddelivery.Name = "txtddelivery"
        Me.txtddelivery.Size = New System.Drawing.Size(162, 22)
        Me.txtddelivery.TabIndex = 110
        Me.txtddelivery.TabStop = False
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(323, 303)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(83, 13)
        Me.Label6.TabIndex = 109
        Me.Label6.Text = "Date of Delivery"
        '
        'txtcname
        '
        Me.txtcname.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtcname.Location = New System.Drawing.Point(31, 272)
        Me.txtcname.Name = "txtcname"
        Me.txtcname.Size = New System.Drawing.Size(456, 22)
        Me.txtcname.TabIndex = 112
        Me.txtcname.TabStop = False
        '
        'lbl
        '
        Me.lbl.AutoSize = True
        Me.lbl.Location = New System.Drawing.Point(28, 256)
        Me.lbl.Name = "lbl"
        Me.lbl.Size = New System.Drawing.Size(76, 13)
        Me.lbl.TabIndex = 111
        Me.lbl.Text = "Supplier Name"
        '
        'cmdclose
        '
        Me.cmdclose.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cmdclose.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdclose.FlatAppearance.BorderSize = 0
        Me.cmdclose.Image = CType(resources.GetObject("cmdclose.Image"), System.Drawing.Image)
        Me.cmdclose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdclose.Location = New System.Drawing.Point(262, 440)
        Me.cmdclose.Name = "cmdclose"
        Me.cmdclose.Size = New System.Drawing.Size(225, 46)
        Me.cmdclose.TabIndex = 113
        Me.cmdclose.Text = "Close"
        Me.cmdclose.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdclose.UseVisualStyleBackColor = True
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.FromArgb(CType(CType(35, Byte), Integer), CType(CType(40, Byte), Integer), CType(CType(44, Byte), Integer))
        Me.Panel1.Controls.Add(Me.lbltitle)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(513, 51)
        Me.Panel1.TabIndex = 114
        '
        'lbltitle
        '
        Me.lbltitle.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.lbltitle.Font = New System.Drawing.Font("Calibri", 20.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbltitle.ForeColor = System.Drawing.Color.White
        Me.lbltitle.Location = New System.Drawing.Point(0, 9)
        Me.lbltitle.Name = "lbltitle"
        Me.lbltitle.Size = New System.Drawing.Size(512, 33)
        Me.lbltitle.TabIndex = 0
        Me.lbltitle.Text = "CONFIRM ORDER AND DELIVERY INFO"
        Me.lbltitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'PictureBox1
        '
        Me.PictureBox1.BackgroundImage = CType(resources.GetObject("PictureBox1.BackgroundImage"), System.Drawing.Image)
        Me.PictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.PictureBox1.Location = New System.Drawing.Point(16, 24)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(29, 27)
        Me.PictureBox1.TabIndex = 115
        Me.PictureBox1.TabStop = False
        '
        'Label8
        '
        Me.Label8.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label8.ForeColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.Label8.Location = New System.Drawing.Point(51, 14)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(381, 53)
        Me.Label8.TabIndex = 116
        Me.Label8.Text = "Please review the details carefully before confirming the order and delivery Info" &
    "rmation. This action will update the following statuses:" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "• PR → [PO] Delivery" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) &
    ""
        '
        'Panel2
        '
        Me.Panel2.BackColor = System.Drawing.Color.FromArgb(CType(CType(226, Byte), Integer), CType(CType(236, Byte), Integer), CType(CType(250, Byte), Integer))
        Me.Panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel2.Controls.Add(Me.PictureBox1)
        Me.Panel2.Controls.Add(Me.Label8)
        Me.Panel2.ForeColor = System.Drawing.SystemColors.AppWorkspace
        Me.Panel2.Location = New System.Drawing.Point(31, 351)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(457, 76)
        Me.Panel2.TabIndex = 117
        '
        'lblpoid
        '
        Me.lblpoid.AutoSize = True
        Me.lblpoid.Location = New System.Drawing.Point(113, 64)
        Me.lblpoid.Name = "lblpoid"
        Me.lblpoid.Size = New System.Drawing.Size(37, 13)
        Me.lblpoid.TabIndex = 118
        Me.lblpoid.Text = "lblpoid"
        Me.lblpoid.Visible = False
        '
        'lblprid
        '
        Me.lblprid.AutoSize = True
        Me.lblprid.Location = New System.Drawing.Point(172, 64)
        Me.lblprid.Name = "lblprid"
        Me.lblprid.Size = New System.Drawing.Size(34, 13)
        Me.lblprid.TabIndex = 119
        Me.lblprid.Text = "lblprid"
        Me.lblprid.Visible = False
        '
        'lblpodate
        '
        Me.lblpodate.AutoSize = True
        Me.lblpodate.Location = New System.Drawing.Point(378, 64)
        Me.lblpodate.Name = "lblpodate"
        Me.lblpodate.Size = New System.Drawing.Size(50, 13)
        Me.lblpodate.TabIndex = 120
        Me.lblpodate.Text = "lblpodate"
        Me.lblpodate.Visible = False
        '
        'txtfcluster
        '
        Me.txtfcluster.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtfcluster.Location = New System.Drawing.Point(31, 226)
        Me.txtfcluster.Name = "txtfcluster"
        Me.txtfcluster.Size = New System.Drawing.Size(456, 22)
        Me.txtfcluster.TabIndex = 122
        Me.txtfcluster.TabStop = False
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(28, 210)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(66, 13)
        Me.Label7.TabIndex = 121
        Me.Label7.Text = "Fund Cluster"
        '
        'txtttype
        '
        Me.txtttype.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtttype.Location = New System.Drawing.Point(31, 137)
        Me.txtttype.Name = "txtttype"
        Me.txtttype.Size = New System.Drawing.Size(320, 22)
        Me.txtttype.TabIndex = 124
        Me.txtttype.TabStop = False
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(28, 121)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(90, 13)
        Me.Label9.TabIndex = 123
        Me.Label9.Text = "Transaction Type"
        '
        'frmdeliveryadd
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(513, 503)
        Me.Controls.Add(Me.txtttype)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.txtfcluster)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.lblpodate)
        Me.Controls.Add(Me.lblprid)
        Me.Controls.Add(Me.lblpoid)
        Me.Controls.Add(Me.Panel2)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.cmdclose)
        Me.Controls.Add(Me.txtcname)
        Me.Controls.Add(Me.lbl)
        Me.Controls.Add(Me.txtddelivery)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.txtpdelivery)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.txtrcenter)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.txtreqoff)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.txtpono)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.txtprno)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.cmdconfirm)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmdeliveryadd"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Confirm Delivery Information"
        Me.Panel1.ResumeLayout(False)
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel2.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents cmdconfirm As Button
    Friend WithEvents Label1 As Label
    Friend WithEvents txtprno As TextBox
    Friend WithEvents txtpono As TextBox
    Friend WithEvents Label2 As Label
    Friend WithEvents txtreqoff As TextBox
    Friend WithEvents Label3 As Label
    Friend WithEvents txtrcenter As TextBox
    Friend WithEvents Label4 As Label
    Friend WithEvents txtpdelivery As TextBox
    Friend WithEvents Label5 As Label
    Friend WithEvents txtddelivery As TextBox
    Friend WithEvents Label6 As Label
    Friend WithEvents txtcname As TextBox
    Friend WithEvents lbl As Label
    Friend WithEvents cmdclose As Button
    Friend WithEvents Panel1 As Panel
    Friend WithEvents lbltitle As Label
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents Label8 As Label
    Friend WithEvents Panel2 As Panel
    Friend WithEvents lblpoid As Label
    Friend WithEvents lblprid As Label
    Friend WithEvents lblpodate As Label
    Friend WithEvents txtfcluster As TextBox
    Friend WithEvents Label7 As Label
    Friend WithEvents txtttype As TextBox
    Friend WithEvents Label9 As Label
End Class
