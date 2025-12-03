<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmPOFile
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmPOFile))
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.lbltitle = New System.Windows.Forms.Label()
        Me.lbltotal = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.txtpurpose = New System.Windows.Forms.TextBox()
        Me.cmdsave = New System.Windows.Forms.Button()
        Me.cmdclose = New System.Windows.Forms.Button()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.txtstatus = New System.Windows.Forms.TextBox()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.DataGridView1 = New System.Windows.Forms.DataGridView()
        Me.cmdprint = New System.Windows.Forms.Button()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.txtncompany = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.txtcaddress = New System.Windows.Forms.TextBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.txtctin = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.txtpono = New System.Windows.Forms.TextBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.txtpdelivery = New System.Windows.Forms.TextBox()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.dtddelivery = New System.Windows.Forms.DateTimePicker()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.txtmproc = New System.Windows.Forms.TextBox()
        Me.txtfcluster = New System.Windows.Forms.TextBox()
        Me.lblprid = New System.Windows.Forms.Label()
        Me.lblprno = New System.Windows.Forms.Label()
        Me.lblsupname = New System.Windows.Forms.Label()
        Me.lblpoid = New System.Windows.Forms.Label()
        Me.cmdscancel = New System.Windows.Forms.Button()
        Me.cmdsubmit = New System.Windows.Forms.Button()
        Me.dtpodate = New System.Windows.Forms.DateTimePicker()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.Panel1.SuspendLayout()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel2.SuspendLayout()
        Me.SuspendLayout()
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.FromArgb(CType(CType(35, Byte), Integer), CType(CType(40, Byte), Integer), CType(CType(44, Byte), Integer))
        Me.Panel1.Controls.Add(Me.lbltitle)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(1025, 51)
        Me.Panel1.TabIndex = 66
        '
        'lbltitle
        '
        Me.lbltitle.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.lbltitle.Font = New System.Drawing.Font("Calibri", 20.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbltitle.ForeColor = System.Drawing.Color.White
        Me.lbltitle.Location = New System.Drawing.Point(5, 9)
        Me.lbltitle.Name = "lbltitle"
        Me.lbltitle.Size = New System.Drawing.Size(1014, 33)
        Me.lbltitle.TabIndex = 0
        Me.lbltitle.Text = "PURCHASE ORDER | PR #:"
        Me.lbltitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lbltotal
        '
        Me.lbltotal.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lbltotal.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbltotal.Location = New System.Drawing.Point(752, 659)
        Me.lbltotal.Name = "lbltotal"
        Me.lbltotal.Size = New System.Drawing.Size(264, 34)
        Me.lbltotal.TabIndex = 101
        Me.lbltotal.Text = "0.00"
        Me.lbltotal.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(14, 87)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(46, 13)
        Me.Label3.TabIndex = 98
        Me.Label3.Text = "Purpose"
        '
        'txtpurpose
        '
        Me.txtpurpose.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtpurpose.Location = New System.Drawing.Point(17, 103)
        Me.txtpurpose.Multiline = True
        Me.txtpurpose.Name = "txtpurpose"
        Me.txtpurpose.Size = New System.Drawing.Size(359, 57)
        Me.txtpurpose.TabIndex = 97
        '
        'cmdsave
        '
        Me.cmdsave.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cmdsave.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdsave.FlatAppearance.BorderSize = 0
        Me.cmdsave.Image = CType(resources.GetObject("cmdsave.Image"), System.Drawing.Image)
        Me.cmdsave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdsave.Location = New System.Drawing.Point(13, 666)
        Me.cmdsave.Name = "cmdsave"
        Me.cmdsave.Size = New System.Drawing.Size(120, 46)
        Me.cmdsave.TabIndex = 95
        Me.cmdsave.Text = "Save"
        Me.cmdsave.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdsave.UseVisualStyleBackColor = True
        '
        'cmdclose
        '
        Me.cmdclose.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cmdclose.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdclose.FlatAppearance.BorderSize = 0
        Me.cmdclose.Image = CType(resources.GetObject("cmdclose.Image"), System.Drawing.Image)
        Me.cmdclose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdclose.Location = New System.Drawing.Point(139, 666)
        Me.cmdclose.Name = "cmdclose"
        Me.cmdclose.Size = New System.Drawing.Size(120, 46)
        Me.cmdclose.TabIndex = 96
        Me.cmdclose.Text = "Close"
        Me.cmdclose.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdclose.UseVisualStyleBackColor = True
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Location = New System.Drawing.Point(742, 4)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(37, 13)
        Me.Label12.TabIndex = 94
        Me.Label12.Text = "Status"
        '
        'txtstatus
        '
        Me.txtstatus.BackColor = System.Drawing.Color.SeaShell
        Me.txtstatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtstatus.Location = New System.Drawing.Point(745, 20)
        Me.txtstatus.Name = "txtstatus"
        Me.txtstatus.Size = New System.Drawing.Size(152, 20)
        Me.txtstatus.TabIndex = 93
        Me.txtstatus.TabStop = False
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(200, 4)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(60, 13)
        Me.Label10.TabIndex = 90
        Me.Label10.Text = "Date of PO"
        '
        'DataGridView1
        '
        Me.DataGridView1.AllowUserToAddRows = False
        Me.DataGridView1.AllowUserToDeleteRows = False
        Me.DataGridView1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.DataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells
        Me.DataGridView1.BackgroundColor = System.Drawing.Color.White
        Me.DataGridView1.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.[Single]
        Me.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView1.Location = New System.Drawing.Point(18, 279)
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.ReadOnly = True
        Me.DataGridView1.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        Me.DataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DataGridView1.Size = New System.Drawing.Size(998, 377)
        Me.DataGridView1.TabIndex = 80
        '
        'cmdprint
        '
        Me.cmdprint.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdprint.FlatAppearance.BorderSize = 0
        Me.cmdprint.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cmdprint.Image = CType(resources.GetObject("cmdprint.Image"), System.Drawing.Image)
        Me.cmdprint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdprint.Location = New System.Drawing.Point(18, 232)
        Me.cmdprint.Name = "cmdprint"
        Me.cmdprint.Size = New System.Drawing.Size(120, 46)
        Me.cmdprint.TabIndex = 78
        Me.cmdprint.Text = "Print Preview"
        Me.cmdprint.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdprint.UseVisualStyleBackColor = True
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(381, 125)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(66, 13)
        Me.Label4.TabIndex = 79
        Me.Label4.Text = "Fund Cluster"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(14, 46)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(48, 13)
        Me.Label1.TabIndex = 104
        Me.Label1.Text = "Supplier:"
        '
        'txtncompany
        '
        Me.txtncompany.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtncompany.Location = New System.Drawing.Point(17, 62)
        Me.txtncompany.Name = "txtncompany"
        Me.txtncompany.Size = New System.Drawing.Size(356, 20)
        Me.txtncompany.TabIndex = 103
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(380, 46)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(45, 13)
        Me.Label2.TabIndex = 106
        Me.Label2.Text = "Address"
        '
        'txtcaddress
        '
        Me.txtcaddress.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtcaddress.Location = New System.Drawing.Point(383, 62)
        Me.txtcaddress.Name = "txtcaddress"
        Me.txtcaddress.Size = New System.Drawing.Size(409, 20)
        Me.txtcaddress.TabIndex = 105
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(795, 46)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(45, 13)
        Me.Label8.TabIndex = 108
        Me.Label8.Text = "TIN No."
        '
        'txtctin
        '
        Me.txtctin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtctin.Location = New System.Drawing.Point(798, 62)
        Me.txtctin.Name = "txtctin"
        Me.txtctin.Size = New System.Drawing.Size(161, 20)
        Me.txtctin.TabIndex = 107
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(14, 4)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(65, 13)
        Me.Label5.TabIndex = 110
        Me.Label5.Text = "PO Number:"
        '
        'txtpono
        '
        Me.txtpono.BackColor = System.Drawing.Color.SeaShell
        Me.txtpono.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtpono.Location = New System.Drawing.Point(17, 20)
        Me.txtpono.Name = "txtpono"
        Me.txtpono.Size = New System.Drawing.Size(175, 20)
        Me.txtpono.TabIndex = 109
        Me.txtpono.TabStop = False
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(380, 87)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(87, 13)
        Me.Label6.TabIndex = 112
        Me.Label6.Text = "Place of Delivery"
        '
        'txtpdelivery
        '
        Me.txtpdelivery.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtpdelivery.Location = New System.Drawing.Point(383, 103)
        Me.txtpdelivery.Name = "txtpdelivery"
        Me.txtpdelivery.Size = New System.Drawing.Size(320, 20)
        Me.txtpdelivery.TabIndex = 111
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(706, 87)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(83, 13)
        Me.Label9.TabIndex = 114
        Me.Label9.Text = "Date of Delivery"
        '
        'dtddelivery
        '
        Me.dtddelivery.Location = New System.Drawing.Point(709, 103)
        Me.dtddelivery.Name = "dtddelivery"
        Me.dtddelivery.Size = New System.Drawing.Size(250, 20)
        Me.dtddelivery.TabIndex = 115
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(416, 4)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(109, 13)
        Me.Label7.TabIndex = 120
        Me.Label7.Text = "Mode of Procurement"
        '
        'txtmproc
        '
        Me.txtmproc.BackColor = System.Drawing.Color.SeaShell
        Me.txtmproc.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtmproc.Location = New System.Drawing.Point(419, 20)
        Me.txtmproc.Name = "txtmproc"
        Me.txtmproc.Size = New System.Drawing.Size(320, 20)
        Me.txtmproc.TabIndex = 119
        Me.txtmproc.TabStop = False
        '
        'txtfcluster
        '
        Me.txtfcluster.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtfcluster.Location = New System.Drawing.Point(383, 140)
        Me.txtfcluster.Name = "txtfcluster"
        Me.txtfcluster.Size = New System.Drawing.Size(320, 20)
        Me.txtfcluster.TabIndex = 121
        Me.txtfcluster.TabStop = False
        '
        'lblprid
        '
        Me.lblprid.AutoSize = True
        Me.lblprid.Location = New System.Drawing.Point(257, 229)
        Me.lblprid.Name = "lblprid"
        Me.lblprid.Size = New System.Drawing.Size(34, 13)
        Me.lblprid.TabIndex = 122
        Me.lblprid.Text = "lbprlid"
        Me.lblprid.Visible = False
        '
        'lblprno
        '
        Me.lblprno.AutoSize = True
        Me.lblprno.Location = New System.Drawing.Point(338, 229)
        Me.lblprno.Name = "lblprno"
        Me.lblprno.Size = New System.Drawing.Size(36, 13)
        Me.lblprno.TabIndex = 123
        Me.lblprno.Text = "lbprno"
        Me.lblprno.Visible = False
        '
        'lblsupname
        '
        Me.lblsupname.AutoSize = True
        Me.lblsupname.Location = New System.Drawing.Point(432, 229)
        Me.lblsupname.Name = "lblsupname"
        Me.lblsupname.Size = New System.Drawing.Size(60, 13)
        Me.lblsupname.TabIndex = 124
        Me.lblsupname.Text = "lblsupname"
        Me.lblsupname.Visible = False
        '
        'lblpoid
        '
        Me.lblpoid.AutoSize = True
        Me.lblpoid.Location = New System.Drawing.Point(97, 0)
        Me.lblpoid.Name = "lblpoid"
        Me.lblpoid.Size = New System.Drawing.Size(0, 13)
        Me.lblpoid.TabIndex = 125
        Me.lblpoid.Visible = False
        '
        'cmdscancel
        '
        Me.cmdscancel.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdscancel.Enabled = False
        Me.cmdscancel.Image = CType(resources.GetObject("cmdscancel.Image"), System.Drawing.Image)
        Me.cmdscancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdscancel.Location = New System.Drawing.Point(843, 225)
        Me.cmdscancel.Name = "cmdscancel"
        Me.cmdscancel.Size = New System.Drawing.Size(173, 48)
        Me.cmdscancel.TabIndex = 127
        Me.cmdscancel.Text = "Cancel"
        Me.cmdscancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdscancel.UseVisualStyleBackColor = True
        '
        'cmdsubmit
        '
        Me.cmdsubmit.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdsubmit.Enabled = False
        Me.cmdsubmit.Image = CType(resources.GetObject("cmdsubmit.Image"), System.Drawing.Image)
        Me.cmdsubmit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdsubmit.Location = New System.Drawing.Point(668, 225)
        Me.cmdsubmit.Name = "cmdsubmit"
        Me.cmdsubmit.Size = New System.Drawing.Size(173, 48)
        Me.cmdsubmit.TabIndex = 126
        Me.cmdsubmit.Text = "Mark as Complete"
        Me.cmdsubmit.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdsubmit.UseVisualStyleBackColor = True
        '
        'dtpodate
        '
        Me.dtpodate.Location = New System.Drawing.Point(203, 20)
        Me.dtpodate.Name = "dtpodate"
        Me.dtpodate.Size = New System.Drawing.Size(210, 20)
        Me.dtpodate.TabIndex = 128
        '
        'Panel2
        '
        Me.Panel2.BackColor = System.Drawing.Color.WhiteSmoke
        Me.Panel2.Controls.Add(Me.dtpodate)
        Me.Panel2.Controls.Add(Me.lblpoid)
        Me.Panel2.Controls.Add(Me.txtfcluster)
        Me.Panel2.Controls.Add(Me.Label7)
        Me.Panel2.Controls.Add(Me.txtmproc)
        Me.Panel2.Controls.Add(Me.dtddelivery)
        Me.Panel2.Controls.Add(Me.Label9)
        Me.Panel2.Controls.Add(Me.Label6)
        Me.Panel2.Controls.Add(Me.txtpdelivery)
        Me.Panel2.Controls.Add(Me.Label5)
        Me.Panel2.Controls.Add(Me.txtpono)
        Me.Panel2.Controls.Add(Me.Label8)
        Me.Panel2.Controls.Add(Me.txtctin)
        Me.Panel2.Controls.Add(Me.Label2)
        Me.Panel2.Controls.Add(Me.txtcaddress)
        Me.Panel2.Controls.Add(Me.Label1)
        Me.Panel2.Controls.Add(Me.txtncompany)
        Me.Panel2.Controls.Add(Me.Label3)
        Me.Panel2.Controls.Add(Me.txtpurpose)
        Me.Panel2.Controls.Add(Me.Label12)
        Me.Panel2.Controls.Add(Me.txtstatus)
        Me.Panel2.Controls.Add(Me.Label10)
        Me.Panel2.Controls.Add(Me.Label4)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel2.Location = New System.Drawing.Point(0, 51)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(1025, 168)
        Me.Panel2.TabIndex = 129
        '
        'frmPOFile
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(1025, 722)
        Me.Controls.Add(Me.Panel2)
        Me.Controls.Add(Me.cmdscancel)
        Me.Controls.Add(Me.cmdsubmit)
        Me.Controls.Add(Me.lblsupname)
        Me.Controls.Add(Me.lblprno)
        Me.Controls.Add(Me.lblprid)
        Me.Controls.Add(Me.lbltotal)
        Me.Controls.Add(Me.cmdsave)
        Me.Controls.Add(Me.cmdclose)
        Me.Controls.Add(Me.DataGridView1)
        Me.Controls.Add(Me.cmdprint)
        Me.Controls.Add(Me.Panel1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MinimizeBox = False
        Me.Name = "frmPOFile"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Purchase Order"
        Me.Panel1.ResumeLayout(False)
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Panel1 As Panel
    Friend WithEvents lbltitle As Label
    Friend WithEvents lbltotal As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents txtpurpose As TextBox
    Friend WithEvents cmdsave As Button
    Friend WithEvents cmdclose As Button
    Friend WithEvents Label12 As Label
    Friend WithEvents txtstatus As TextBox
    Friend WithEvents Label10 As Label
    Friend WithEvents DataGridView1 As DataGridView
    Friend WithEvents cmdprint As Button
    Friend WithEvents Label4 As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents txtncompany As TextBox
    Friend WithEvents Label2 As Label
    Friend WithEvents txtcaddress As TextBox
    Friend WithEvents Label8 As Label
    Friend WithEvents txtctin As TextBox
    Friend WithEvents Label5 As Label
    Friend WithEvents txtpono As TextBox
    Friend WithEvents Label6 As Label
    Friend WithEvents txtpdelivery As TextBox
    Friend WithEvents Label9 As Label
    Friend WithEvents dtddelivery As DateTimePicker
    Friend WithEvents Label7 As Label
    Friend WithEvents txtmproc As TextBox
    Friend WithEvents txtfcluster As TextBox
    Friend WithEvents lblprid As Label
    Friend WithEvents lblprno As Label
    Friend WithEvents lblsupname As Label
    Friend WithEvents lblpoid As Label
    Friend WithEvents cmdscancel As Button
    Friend WithEvents cmdsubmit As Button
    Friend WithEvents dtpodate As DateTimePicker
    Friend WithEvents Panel2 As Panel
End Class
