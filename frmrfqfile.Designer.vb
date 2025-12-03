<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmrfqfile
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmrfqfile))
        Me.lbltitle = New System.Windows.Forms.Label()
        Me.cmdsave = New System.Windows.Forms.Button()
        Me.cmdclose = New System.Windows.Forms.Button()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.txtstatus = New System.Windows.Forms.TextBox()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.txtprdate = New System.Windows.Forms.TextBox()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.txtprno = New System.Windows.Forms.TextBox()
        Me.cmdaddsup = New System.Windows.Forms.Button()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.txtpentity = New System.Windows.Forms.TextBox()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.dtdcanvass = New System.Windows.Forms.DateTimePicker()
        Me.cmdedit = New System.Windows.Forms.Button()
        Me.cmddelete = New System.Windows.Forms.Button()
        Me.lblprid = New System.Windows.Forms.Label()
        Me.lblid = New System.Windows.Forms.Label()
        Me.DataGridView1 = New System.Windows.Forms.DataGridView()
        Me.cmdprint = New System.Windows.Forms.Button()
        Me.cmdsubmit = New System.Windows.Forms.Button()
        Me.cmdscancel = New System.Windows.Forms.Button()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.LBLTTYPE = New System.Windows.Forms.Label()
        Me.dtrecieved = New System.Windows.Forms.DateTimePicker()
        Me.txtoeuser = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.txtfund = New System.Windows.Forms.TextBox()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.txtpurpose = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.txtcanby = New System.Windows.Forms.TextBox()
        Me.dtsdate = New System.Windows.Forms.DateTimePicker()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.combomproc = New System.Windows.Forms.ComboBox()
        Me.Panel1.SuspendLayout()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel2.SuspendLayout()
        Me.SuspendLayout()
        '
        'lbltitle
        '
        Me.lbltitle.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.lbltitle.Font = New System.Drawing.Font("Calibri", 20.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbltitle.ForeColor = System.Drawing.Color.White
        Me.lbltitle.Location = New System.Drawing.Point(73, 9)
        Me.lbltitle.Name = "lbltitle"
        Me.lbltitle.Size = New System.Drawing.Size(1065, 33)
        Me.lbltitle.TabIndex = 0
        Me.lbltitle.Text = "REQUEST FOR QUOTATION | PR #:"
        Me.lbltitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'cmdsave
        '
        Me.cmdsave.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cmdsave.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdsave.FlatAppearance.BorderSize = 0
        Me.cmdsave.Image = CType(resources.GetObject("cmdsave.Image"), System.Drawing.Image)
        Me.cmdsave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdsave.Location = New System.Drawing.Point(15, 606)
        Me.cmdsave.Name = "cmdsave"
        Me.cmdsave.Size = New System.Drawing.Size(120, 46)
        Me.cmdsave.TabIndex = 91
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
        Me.cmdclose.Location = New System.Drawing.Point(141, 606)
        Me.cmdclose.Name = "cmdclose"
        Me.cmdclose.Size = New System.Drawing.Size(120, 46)
        Me.cmdclose.TabIndex = 92
        Me.cmdclose.Text = "Close"
        Me.cmdclose.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdclose.UseVisualStyleBackColor = True
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Location = New System.Drawing.Point(920, 9)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(37, 13)
        Me.Label12.TabIndex = 90
        Me.Label12.Text = "Status"
        '
        'txtstatus
        '
        Me.txtstatus.BackColor = System.Drawing.Color.SeaShell
        Me.txtstatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtstatus.Location = New System.Drawing.Point(920, 25)
        Me.txtstatus.Name = "txtstatus"
        Me.txtstatus.Size = New System.Drawing.Size(152, 20)
        Me.txtstatus.TabIndex = 89
        Me.txtstatus.TabStop = False
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(326, 9)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(71, 13)
        Me.Label11.TabIndex = 88
        Me.Label11.Text = "PR Received"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(190, 9)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(48, 13)
        Me.Label10.TabIndex = 86
        Me.Label10.Text = "PR Date"
        '
        'txtprdate
        '
        Me.txtprdate.BackColor = System.Drawing.Color.SeaShell
        Me.txtprdate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtprdate.Location = New System.Drawing.Point(190, 25)
        Me.txtprdate.Name = "txtprdate"
        Me.txtprdate.Size = New System.Drawing.Size(133, 20)
        Me.txtprdate.TabIndex = 85
        Me.txtprdate.TabStop = False
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(6, 9)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(65, 13)
        Me.Label9.TabIndex = 84
        Me.Label9.Text = "PR Number:"
        '
        'txtprno
        '
        Me.txtprno.BackColor = System.Drawing.Color.SeaShell
        Me.txtprno.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtprno.Location = New System.Drawing.Point(9, 25)
        Me.txtprno.Name = "txtprno"
        Me.txtprno.Size = New System.Drawing.Size(175, 20)
        Me.txtprno.TabIndex = 83
        Me.txtprno.TabStop = False
        '
        'cmdaddsup
        '
        Me.cmdaddsup.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdaddsup.FlatAppearance.BorderSize = 0
        Me.cmdaddsup.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cmdaddsup.Image = CType(resources.GetObject("cmdaddsup.Image"), System.Drawing.Image)
        Me.cmdaddsup.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdaddsup.Location = New System.Drawing.Point(15, 218)
        Me.cmdaddsup.Name = "cmdaddsup"
        Me.cmdaddsup.Size = New System.Drawing.Size(122, 46)
        Me.cmdaddsup.TabIndex = 71
        Me.cmdaddsup.Text = "Add Supplier"
        Me.cmdaddsup.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdaddsup.UseVisualStyleBackColor = True
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(674, 9)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(124, 13)
        Me.Label7.TabIndex = 82
        Me.Label7.Text = "Name of Procuring Entity"
        '
        'txtpentity
        '
        Me.txtpentity.BackColor = System.Drawing.Color.SeaShell
        Me.txtpentity.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtpentity.Location = New System.Drawing.Point(677, 25)
        Me.txtpentity.Name = "txtpentity"
        Me.txtpentity.Size = New System.Drawing.Size(237, 20)
        Me.txtpentity.TabIndex = 81
        Me.txtpentity.Text = "Ministry of Public Order & Safety"
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.FromArgb(CType(CType(35, Byte), Integer), CType(CType(40, Byte), Integer), CType(CType(44, Byte), Integer))
        Me.Panel1.Controls.Add(Me.lbltitle)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(1211, 51)
        Me.Panel1.TabIndex = 65
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Location = New System.Drawing.Point(495, 9)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(86, 13)
        Me.Label13.TabIndex = 96
        Me.Label13.Text = "Date of Canvass"
        '
        'dtdcanvass
        '
        Me.dtdcanvass.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.dtdcanvass.Location = New System.Drawing.Point(498, 25)
        Me.dtdcanvass.Name = "dtdcanvass"
        Me.dtdcanvass.Size = New System.Drawing.Size(173, 20)
        Me.dtdcanvass.TabIndex = 113
        '
        'cmdedit
        '
        Me.cmdedit.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdedit.FlatAppearance.BorderSize = 0
        Me.cmdedit.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cmdedit.Image = CType(resources.GetObject("cmdedit.Image"), System.Drawing.Image)
        Me.cmdedit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdedit.Location = New System.Drawing.Point(139, 218)
        Me.cmdedit.Name = "cmdedit"
        Me.cmdedit.Size = New System.Drawing.Size(74, 46)
        Me.cmdedit.TabIndex = 114
        Me.cmdedit.Text = "Open"
        Me.cmdedit.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdedit.UseVisualStyleBackColor = True
        '
        'cmddelete
        '
        Me.cmddelete.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmddelete.FlatAppearance.BorderSize = 0
        Me.cmddelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cmddelete.Image = CType(resources.GetObject("cmddelete.Image"), System.Drawing.Image)
        Me.cmddelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmddelete.Location = New System.Drawing.Point(219, 218)
        Me.cmddelete.Name = "cmddelete"
        Me.cmddelete.Size = New System.Drawing.Size(79, 46)
        Me.cmddelete.TabIndex = 115
        Me.cmddelete.Text = "Delete"
        Me.cmddelete.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmddelete.UseVisualStyleBackColor = True
        '
        'lblprid
        '
        Me.lblprid.AutoSize = True
        Me.lblprid.Location = New System.Drawing.Point(110, 10)
        Me.lblprid.Name = "lblprid"
        Me.lblprid.Size = New System.Drawing.Size(0, 13)
        Me.lblprid.TabIndex = 118
        Me.lblprid.UseWaitCursor = True
        Me.lblprid.Visible = False
        '
        'lblid
        '
        Me.lblid.AutoSize = True
        Me.lblid.Location = New System.Drawing.Point(1036, 0)
        Me.lblid.Name = "lblid"
        Me.lblid.Size = New System.Drawing.Size(0, 13)
        Me.lblid.TabIndex = 119
        Me.lblid.Visible = False
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
        Me.DataGridView1.Location = New System.Drawing.Point(18, 264)
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.ReadOnly = True
        Me.DataGridView1.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        Me.DataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DataGridView1.Size = New System.Drawing.Size(1179, 336)
        Me.DataGridView1.TabIndex = 120
        '
        'cmdprint
        '
        Me.cmdprint.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdprint.FlatAppearance.BorderSize = 0
        Me.cmdprint.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cmdprint.Image = CType(resources.GetObject("cmdprint.Image"), System.Drawing.Image)
        Me.cmdprint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdprint.Location = New System.Drawing.Point(304, 218)
        Me.cmdprint.Name = "cmdprint"
        Me.cmdprint.Size = New System.Drawing.Size(130, 46)
        Me.cmdprint.TabIndex = 121
        Me.cmdprint.Text = "Print Empty RFQ"
        Me.cmdprint.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdprint.UseVisualStyleBackColor = True
        '
        'cmdsubmit
        '
        Me.cmdsubmit.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdsubmit.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdsubmit.Enabled = False
        Me.cmdsubmit.Image = CType(resources.GetObject("cmdsubmit.Image"), System.Drawing.Image)
        Me.cmdsubmit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdsubmit.Location = New System.Drawing.Point(849, 212)
        Me.cmdsubmit.Name = "cmdsubmit"
        Me.cmdsubmit.Size = New System.Drawing.Size(173, 48)
        Me.cmdsubmit.TabIndex = 122
        Me.cmdsubmit.Text = "Mark as Complete"
        Me.cmdsubmit.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdsubmit.UseVisualStyleBackColor = True
        '
        'cmdscancel
        '
        Me.cmdscancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdscancel.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdscancel.Enabled = False
        Me.cmdscancel.Image = CType(resources.GetObject("cmdscancel.Image"), System.Drawing.Image)
        Me.cmdscancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdscancel.Location = New System.Drawing.Point(1024, 212)
        Me.cmdscancel.Name = "cmdscancel"
        Me.cmdscancel.Size = New System.Drawing.Size(173, 48)
        Me.cmdscancel.TabIndex = 123
        Me.cmdscancel.Text = "Cancel"
        Me.cmdscancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdscancel.UseVisualStyleBackColor = True
        '
        'Panel2
        '
        Me.Panel2.BackColor = System.Drawing.Color.WhiteSmoke
        Me.Panel2.Controls.Add(Me.LBLTTYPE)
        Me.Panel2.Controls.Add(Me.dtrecieved)
        Me.Panel2.Controls.Add(Me.lblid)
        Me.Panel2.Controls.Add(Me.lblprid)
        Me.Panel2.Controls.Add(Me.txtoeuser)
        Me.Panel2.Controls.Add(Me.Label4)
        Me.Panel2.Controls.Add(Me.dtdcanvass)
        Me.Panel2.Controls.Add(Me.txtfund)
        Me.Panel2.Controls.Add(Me.Label15)
        Me.Panel2.Controls.Add(Me.txtpurpose)
        Me.Panel2.Controls.Add(Me.Label3)
        Me.Panel2.Controls.Add(Me.Label5)
        Me.Panel2.Controls.Add(Me.Label2)
        Me.Panel2.Controls.Add(Me.txtcanby)
        Me.Panel2.Controls.Add(Me.dtsdate)
        Me.Panel2.Controls.Add(Me.Label13)
        Me.Panel2.Controls.Add(Me.Label12)
        Me.Panel2.Controls.Add(Me.txtstatus)
        Me.Panel2.Controls.Add(Me.Label11)
        Me.Panel2.Controls.Add(Me.Label10)
        Me.Panel2.Controls.Add(Me.txtprdate)
        Me.Panel2.Controls.Add(Me.Label9)
        Me.Panel2.Controls.Add(Me.txtprno)
        Me.Panel2.Controls.Add(Me.Label7)
        Me.Panel2.Controls.Add(Me.Label1)
        Me.Panel2.Controls.Add(Me.combomproc)
        Me.Panel2.Controls.Add(Me.txtpentity)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel2.Location = New System.Drawing.Point(0, 51)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(1211, 155)
        Me.Panel2.TabIndex = 124
        '
        'LBLTTYPE
        '
        Me.LBLTTYPE.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.LBLTTYPE.AutoSize = True
        Me.LBLTTYPE.Location = New System.Drawing.Point(1093, 25)
        Me.LBLTTYPE.Name = "LBLTTYPE"
        Me.LBLTTYPE.Size = New System.Drawing.Size(0, 13)
        Me.LBLTTYPE.TabIndex = 121
        Me.LBLTTYPE.Visible = False
        '
        'dtrecieved
        '
        Me.dtrecieved.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.dtrecieved.Location = New System.Drawing.Point(329, 25)
        Me.dtrecieved.Name = "dtrecieved"
        Me.dtrecieved.Size = New System.Drawing.Size(163, 20)
        Me.dtrecieved.TabIndex = 120
        '
        'txtoeuser
        '
        Me.txtoeuser.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtoeuser.Location = New System.Drawing.Point(427, 77)
        Me.txtoeuser.Name = "txtoeuser"
        Me.txtoeuser.Size = New System.Drawing.Size(251, 20)
        Me.txtoeuser.TabIndex = 117
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(424, 61)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(84, 13)
        Me.Label4.TabIndex = 116
        Me.Label4.Text = "Office/End-User"
        '
        'txtfund
        '
        Me.txtfund.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtfund.Location = New System.Drawing.Point(684, 77)
        Me.txtfund.Name = "txtfund"
        Me.txtfund.Size = New System.Drawing.Size(255, 20)
        Me.txtfund.TabIndex = 105
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Location = New System.Drawing.Point(681, 61)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(36, 13)
        Me.Label15.TabIndex = 104
        Me.Label15.Text = "Funds"
        '
        'txtpurpose
        '
        Me.txtpurpose.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtpurpose.Location = New System.Drawing.Point(9, 75)
        Me.txtpurpose.Multiline = True
        Me.txtpurpose.Name = "txtpurpose"
        Me.txtpurpose.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtpurpose.Size = New System.Drawing.Size(408, 62)
        Me.txtpurpose.TabIndex = 103
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(6, 58)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(46, 13)
        Me.Label3.TabIndex = 102
        Me.Label3.Text = "Purpose"
        '
        'Label5
        '
        Me.Label5.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(681, 101)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(77, 13)
        Me.Label5.TabIndex = 78
        Me.Label5.Text = "Canvassed by:"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(424, 102)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(118, 13)
        Me.Label2.TabIndex = 101
        Me.Label2.Text = "Quote Submission Date"
        '
        'txtcanby
        '
        Me.txtcanby.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtcanby.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtcanby.Location = New System.Drawing.Point(684, 117)
        Me.txtcanby.Name = "txtcanby"
        Me.txtcanby.Size = New System.Drawing.Size(513, 20)
        Me.txtcanby.TabIndex = 77
        '
        'dtsdate
        '
        Me.dtsdate.Location = New System.Drawing.Point(427, 117)
        Me.dtsdate.Name = "dtsdate"
        Me.dtsdate.Size = New System.Drawing.Size(251, 20)
        Me.dtsdate.TabIndex = 100
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(942, 61)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(109, 13)
        Me.Label1.TabIndex = 67
        Me.Label1.Text = "Mode of Procurement"
        '
        'combomproc
        '
        Me.combomproc.FormattingEnabled = True
        Me.combomproc.Items.AddRange(New Object() {"Competitive Bidding", "Limited Source Bidding", "Competitive Dialogue", "Unsolicited Offer with Bid Matching", "Direct Contracting", "Direct Acquisition", "Repeat Order", "Small Value Procurement", "Negotiated Procurement", "Negotiated Procurement: Two Failed Biddings", "Negotiated Procurement: Emergency Cases", "Negotiated Procurement: Scientific, Scholarly or Artistic Work, Exclusive Technol" &
                "ogy", "Negotiated Procurement: Lease of Real Property and Venue", "Negotiated Procurement: Direct Retail Purchase of Petroleum Fuel, Oil and Lubrica" &
                "nt Products, Electronic Charging Devices, and Online Subscriptions", "Negotiated Procurement: Media Services", "Direct Sales", "Direct Procurement for Science, Technology and Innovation"})
        Me.combomproc.Location = New System.Drawing.Point(945, 77)
        Me.combomproc.Name = "combomproc"
        Me.combomproc.Size = New System.Drawing.Size(252, 21)
        Me.combomproc.TabIndex = 66
        Me.combomproc.Text = "Small Value Procurement"
        '
        'frmrfqfile
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(1211, 664)
        Me.Controls.Add(Me.Panel2)
        Me.Controls.Add(Me.cmdscancel)
        Me.Controls.Add(Me.cmdsubmit)
        Me.Controls.Add(Me.cmdprint)
        Me.Controls.Add(Me.DataGridView1)
        Me.Controls.Add(Me.cmddelete)
        Me.Controls.Add(Me.cmdedit)
        Me.Controls.Add(Me.cmdsave)
        Me.Controls.Add(Me.cmdclose)
        Me.Controls.Add(Me.cmdaddsup)
        Me.Controls.Add(Me.Panel1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmrfqfile"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Request for Quotation"
        Me.Panel1.ResumeLayout(False)
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lbltitle As Label
    Friend WithEvents cmdsave As Button
    Friend WithEvents cmdclose As Button
    Friend WithEvents Label12 As Label
    Friend WithEvents txtstatus As TextBox
    Friend WithEvents Label11 As Label
    Friend WithEvents Label10 As Label
    Friend WithEvents txtprdate As TextBox
    Friend WithEvents Label9 As Label
    Friend WithEvents txtprno As TextBox
    Friend WithEvents cmdaddsup As Button
    Friend WithEvents Label7 As Label
    Friend WithEvents txtpentity As TextBox
    Friend WithEvents Panel1 As Panel
    Friend WithEvents Label13 As Label
    Friend WithEvents dtdcanvass As DateTimePicker
    Friend WithEvents cmdedit As Button
    Friend WithEvents cmddelete As Button
    Friend WithEvents lblprid As Label
    Friend WithEvents lblid As Label
    Friend WithEvents DataGridView1 As DataGridView
    Friend WithEvents cmdprint As Button
    Friend WithEvents cmdsubmit As Button
    Friend WithEvents cmdscancel As Button
    Friend WithEvents Panel2 As Panel
    Friend WithEvents dtrecieved As DateTimePicker
    Friend WithEvents LBLTTYPE As Label
    Friend WithEvents txtoeuser As TextBox
    Friend WithEvents Label4 As Label
    Friend WithEvents txtfund As TextBox
    Friend WithEvents Label15 As Label
    Friend WithEvents txtpurpose As TextBox
    Friend WithEvents Label3 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents txtcanby As TextBox
    Friend WithEvents dtsdate As DateTimePicker
    Friend WithEvents Label1 As Label
    Friend WithEvents combomproc As ComboBox
End Class
