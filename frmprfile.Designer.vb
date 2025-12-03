<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmprfile
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmprfile))
        Me.cmdsave = New System.Windows.Forms.Button()
        Me.cmdclose = New System.Windows.Forms.Button()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.txtstatus = New System.Windows.Forms.TextBox()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.txttime = New System.Windows.Forms.TextBox()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.txtprno = New System.Windows.Forms.TextBox()
        Me.DataGridView1 = New System.Windows.Forms.DataGridView()
        Me.cmdprint = New System.Windows.Forms.Button()
        Me.cmdadd = New System.Windows.Forms.Button()
        Me.cmdedit = New System.Windows.Forms.Button()
        Me.cmddelete = New System.Windows.Forms.Button()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.txtreqby = New System.Windows.Forms.TextBox()
        Me.combofcluster = New System.Windows.Forms.ComboBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.combooffsec = New System.Windows.Forms.ComboBox()
        Me.lbltitle = New System.Windows.Forms.Label()
        Me.txtename = New System.Windows.Forms.TextBox()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.txtpurpose = New System.Windows.Forms.TextBox()
        Me.cmdaddapp = New System.Windows.Forms.Button()
        Me.lblid = New System.Windows.Forms.Label()
        Me.lbltotal = New System.Windows.Forms.Label()
        Me.cmdsubmit = New System.Windows.Forms.Button()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.combotype = New System.Windows.Forms.ComboBox()
        Me.comborcenter = New System.Windows.Forms.TextBox()
        Me.txtappby = New System.Windows.Forms.ComboBox()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.dtdate = New System.Windows.Forms.DateTimePicker()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.combocategory = New System.Windows.Forms.ComboBox()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel1.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.SuspendLayout()
        '
        'cmdsave
        '
        Me.cmdsave.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cmdsave.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdsave.FlatAppearance.BorderSize = 0
        Me.cmdsave.Image = CType(resources.GetObject("cmdsave.Image"), System.Drawing.Image)
        Me.cmdsave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdsave.Location = New System.Drawing.Point(15, 686)
        Me.cmdsave.Name = "cmdsave"
        Me.cmdsave.Size = New System.Drawing.Size(120, 46)
        Me.cmdsave.TabIndex = 20
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
        Me.cmdclose.Location = New System.Drawing.Point(141, 686)
        Me.cmdclose.Name = "cmdclose"
        Me.cmdclose.Size = New System.Drawing.Size(120, 46)
        Me.cmdclose.TabIndex = 21
        Me.cmdclose.Text = "Close"
        Me.cmdclose.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdclose.UseVisualStyleBackColor = True
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Location = New System.Drawing.Point(1057, 10)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(37, 13)
        Me.Label12.TabIndex = 60
        Me.Label12.Text = "Status"
        '
        'txtstatus
        '
        Me.txtstatus.BackColor = System.Drawing.Color.SeaShell
        Me.txtstatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtstatus.Location = New System.Drawing.Point(1057, 26)
        Me.txtstatus.Name = "txtstatus"
        Me.txtstatus.Size = New System.Drawing.Size(152, 20)
        Me.txtstatus.TabIndex = 5
        Me.txtstatus.TabStop = False
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(962, 10)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(70, 13)
        Me.Label11.TabIndex = 58
        Me.Label11.Text = "Time Created"
        '
        'txttime
        '
        Me.txttime.BackColor = System.Drawing.Color.SeaShell
        Me.txttime.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txttime.Location = New System.Drawing.Point(962, 26)
        Me.txttime.Name = "txttime"
        Me.txttime.Size = New System.Drawing.Size(89, 20)
        Me.txttime.TabIndex = 4
        Me.txttime.TabStop = False
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(781, 10)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(70, 13)
        Me.Label10.TabIndex = 56
        Me.Label10.Text = "Date Created"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(597, 10)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(65, 13)
        Me.Label9.TabIndex = 54
        Me.Label9.Text = "PR Number:"
        '
        'txtprno
        '
        Me.txtprno.BackColor = System.Drawing.Color.SeaShell
        Me.txtprno.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtprno.Location = New System.Drawing.Point(600, 26)
        Me.txtprno.Name = "txtprno"
        Me.txtprno.Size = New System.Drawing.Size(175, 20)
        Me.txtprno.TabIndex = 2
        Me.txtprno.TabStop = False
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
        Me.DataGridView1.Location = New System.Drawing.Point(15, 291)
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.ReadOnly = True
        Me.DataGridView1.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        Me.DataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DataGridView1.Size = New System.Drawing.Size(1193, 389)
        Me.DataGridView1.TabIndex = 19
        '
        'cmdprint
        '
        Me.cmdprint.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdprint.FlatAppearance.BorderSize = 0
        Me.cmdprint.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cmdprint.Image = CType(resources.GetObject("cmdprint.Image"), System.Drawing.Image)
        Me.cmdprint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdprint.Location = New System.Drawing.Point(380, 242)
        Me.cmdprint.Name = "cmdprint"
        Me.cmdprint.Size = New System.Drawing.Size(200, 46)
        Me.cmdprint.TabIndex = 17
        Me.cmdprint.Text = "Preview and Submit for Approval"
        Me.cmdprint.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdprint.UseVisualStyleBackColor = True
        '
        'cmdadd
        '
        Me.cmdadd.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdadd.FlatAppearance.BorderSize = 0
        Me.cmdadd.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cmdadd.Image = CType(resources.GetObject("cmdadd.Image"), System.Drawing.Image)
        Me.cmdadd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdadd.Location = New System.Drawing.Point(17, 242)
        Me.cmdadd.Name = "cmdadd"
        Me.cmdadd.Size = New System.Drawing.Size(77, 46)
        Me.cmdadd.TabIndex = 13
        Me.cmdadd.Text = "Add"
        Me.cmdadd.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdadd.UseVisualStyleBackColor = True
        '
        'cmdedit
        '
        Me.cmdedit.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdedit.FlatAppearance.BorderSize = 0
        Me.cmdedit.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cmdedit.Image = CType(resources.GetObject("cmdedit.Image"), System.Drawing.Image)
        Me.cmdedit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdedit.Location = New System.Drawing.Point(226, 242)
        Me.cmdedit.Name = "cmdedit"
        Me.cmdedit.Size = New System.Drawing.Size(64, 46)
        Me.cmdedit.TabIndex = 15
        Me.cmdedit.Text = "Edit"
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
        Me.cmddelete.Location = New System.Drawing.Point(296, 242)
        Me.cmddelete.Name = "cmddelete"
        Me.cmddelete.Size = New System.Drawing.Size(78, 46)
        Me.cmddelete.TabIndex = 16
        Me.cmddelete.Text = "Delete"
        Me.cmddelete.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmddelete.UseVisualStyleBackColor = True
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(11, 53)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(67, 13)
        Me.Label7.TabIndex = 52
        Me.Label7.Text = "Entitty Name"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(869, 115)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(193, 13)
        Me.Label6.TabIndex = 50
        Me.Label6.Text = "Approving Signatorial: (Name - Position)"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(540, 115)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(160, 13)
        Me.Label5.TabIndex = 48
        Me.Label5.Text = "Requested By: (Name - Position)"
        '
        'txtreqby
        '
        Me.txtreqby.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtreqby.Location = New System.Drawing.Point(543, 131)
        Me.txtreqby.Name = "txtreqby"
        Me.txtreqby.Size = New System.Drawing.Size(323, 20)
        Me.txtreqby.TabIndex = 11
        '
        'combofcluster
        '
        Me.combofcluster.FormattingEnabled = True
        Me.combofcluster.Items.AddRange(New Object() {"Regular Funds", "Special Development Fund", "Transitional Development Impact Fund", "Supplemental Fund"})
        Me.combofcluster.Location = New System.Drawing.Point(287, 69)
        Me.combofcluster.Name = "combofcluster"
        Me.combofcluster.Size = New System.Drawing.Size(307, 21)
        Me.combofcluster.TabIndex = 7
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(284, 50)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(75, 13)
        Me.Label4.TabIndex = 44
        Me.Label4.Text = "• Fund Cluster"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(1019, 53)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(136, 13)
        Me.Label2.TabIndex = 37
        Me.Label2.Text = "Responsibility Center Code:"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(597, 53)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(85, 13)
        Me.Label1.TabIndex = 35
        Me.Label1.Text = "• Office/Section"
        '
        'combooffsec
        '
        Me.combooffsec.FormattingEnabled = True
        Me.combooffsec.Items.AddRange(New Object() {"OFFICE OF THE MINISTER", "OFFICE OF THE DEPUTY MINISTER", "OFFICE OF DIRECTOR GENERAL", "ADMINISTRATIVE AND FINANCE DIVISION (CAO)", "ACCOUNTING SECTION", "BUDGET SECTION", "PROCUREMENT MANAGEMENT SECTION", "CASH SECTION", "ARCHIVES AND RECORDS SECTION", "HUMAN RESOURCE MANAGEMENT SECTION", "SUPPLY SECTION", "GENERAL SERVICES SECTION", "LEGAL AND LEGISLATIVE LIAISON SECTION", "PLANNING SECTION", "INFORMATION AND COMMUNICATION SECTION", "INTERNAL AUDIT SECTION", "BANGSAMORO PEACE RECONCILIATION AND UNIFICATION SERVICES", "PEACE EDUCATION DIVISION", "ALTERNATIVE DISPUTE RESOLUTION DIVISION (CAO-V)", "COMMUNITY AFFAIRS SECTION (MAG)", "COMMUNITY AFFAIRS SECTION (LDS)", "COMMUNITY AFFAIRS SECTION (SGA)", "COMMUNITY AFFAIRS SECTION (BAS)", "COMMUNITY AFFAIRS SECTION (SUL)", "COMMUNITY AFFAIRS SECTION (TAW)", "HOME AFFAIRS SERVICES", "LAW ENFORCEMENT COORDINATION DIVISION", "CIVILIAN INTELLIGENCE AND INVESTIGATION DIVISION"})
        Me.combooffsec.Location = New System.Drawing.Point(600, 69)
        Me.combooffsec.Name = "combooffsec"
        Me.combooffsec.Size = New System.Drawing.Size(416, 21)
        Me.combooffsec.TabIndex = 8
        '
        'lbltitle
        '
        Me.lbltitle.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.lbltitle.Font = New System.Drawing.Font("Calibri", 20.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbltitle.ForeColor = System.Drawing.Color.White
        Me.lbltitle.Location = New System.Drawing.Point(106, 9)
        Me.lbltitle.Name = "lbltitle"
        Me.lbltitle.Size = New System.Drawing.Size(1007, 33)
        Me.lbltitle.TabIndex = 0
        Me.lbltitle.Text = "PURCHASE REQUEST | PR #:"
        Me.lbltitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'txtename
        '
        Me.txtename.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtename.Location = New System.Drawing.Point(14, 69)
        Me.txtename.Name = "txtename"
        Me.txtename.Size = New System.Drawing.Size(267, 20)
        Me.txtename.TabIndex = 6
        Me.txtename.Text = "MPOS-BARMM"
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.FromArgb(CType(CType(35, Byte), Integer), CType(CType(40, Byte), Integer), CType(CType(44, Byte), Integer))
        Me.Panel1.Controls.Add(Me.lbltitle)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(1222, 51)
        Me.Panel1.TabIndex = 33
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(11, 92)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(46, 13)
        Me.Label3.TabIndex = 64
        Me.Label3.Text = "Purpose"
        '
        'txtpurpose
        '
        Me.txtpurpose.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtpurpose.Location = New System.Drawing.Point(14, 108)
        Me.txtpurpose.Multiline = True
        Me.txtpurpose.Name = "txtpurpose"
        Me.txtpurpose.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtpurpose.Size = New System.Drawing.Size(520, 59)
        Me.txtpurpose.TabIndex = 10
        '
        'cmdaddapp
        '
        Me.cmdaddapp.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdaddapp.FlatAppearance.BorderSize = 0
        Me.cmdaddapp.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cmdaddapp.Image = CType(resources.GetObject("cmdaddapp.Image"), System.Drawing.Image)
        Me.cmdaddapp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdaddapp.Location = New System.Drawing.Point(100, 242)
        Me.cmdaddapp.Name = "cmdaddapp"
        Me.cmdaddapp.Size = New System.Drawing.Size(120, 46)
        Me.cmdaddapp.TabIndex = 14
        Me.cmdaddapp.Text = "Add from APP"
        Me.cmdaddapp.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdaddapp.UseVisualStyleBackColor = True
        '
        'lblid
        '
        Me.lblid.AutoSize = True
        Me.lblid.Location = New System.Drawing.Point(893, 53)
        Me.lblid.Name = "lblid"
        Me.lblid.Size = New System.Drawing.Size(0, 13)
        Me.lblid.TabIndex = 68
        Me.lblid.Visible = False
        '
        'lbltotal
        '
        Me.lbltotal.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lbltotal.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbltotal.Location = New System.Drawing.Point(957, 683)
        Me.lbltotal.Name = "lbltotal"
        Me.lbltotal.Size = New System.Drawing.Size(251, 34)
        Me.lbltotal.TabIndex = 69
        Me.lbltotal.Text = "0.00"
        Me.lbltotal.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'cmdsubmit
        '
        Me.cmdsubmit.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdsubmit.BackColor = System.Drawing.Color.White
        Me.cmdsubmit.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdsubmit.FlatAppearance.BorderSize = 0
        Me.cmdsubmit.Image = CType(resources.GetObject("cmdsubmit.Image"), System.Drawing.Image)
        Me.cmdsubmit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdsubmit.Location = New System.Drawing.Point(1018, 242)
        Me.cmdsubmit.Name = "cmdsubmit"
        Me.cmdsubmit.Size = New System.Drawing.Size(190, 46)
        Me.cmdsubmit.TabIndex = 18
        Me.cmdsubmit.Text = "Cancel Approval Request"
        Me.cmdsubmit.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdsubmit.UseVisualStyleBackColor = False
        Me.cmdsubmit.Visible = False
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(11, 10)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(161, 13)
        Me.Label8.TabIndex = 72
        Me.Label8.Text = "• Type of Purchase/Transaction"
        '
        'combotype
        '
        Me.combotype.BackColor = System.Drawing.Color.SeaShell
        Me.combotype.FormattingEnabled = True
        Me.combotype.Items.AddRange(New Object() {"Direct Payment", "Cash Advance", "Reimbursement"})
        Me.combotype.Location = New System.Drawing.Point(14, 26)
        Me.combotype.Name = "combotype"
        Me.combotype.Size = New System.Drawing.Size(248, 21)
        Me.combotype.TabIndex = 0
        '
        'comborcenter
        '
        Me.comborcenter.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.comborcenter.Location = New System.Drawing.Point(1022, 69)
        Me.comborcenter.Name = "comborcenter"
        Me.comborcenter.Size = New System.Drawing.Size(186, 21)
        Me.comborcenter.TabIndex = 9
        '
        'txtappby
        '
        Me.txtappby.FormattingEnabled = True
        Me.txtappby.Location = New System.Drawing.Point(872, 131)
        Me.txtappby.Name = "txtappby"
        Me.txtappby.Size = New System.Drawing.Size(336, 21)
        Me.txtappby.TabIndex = 12
        '
        'Panel2
        '
        Me.Panel2.BackColor = System.Drawing.Color.WhiteSmoke
        Me.Panel2.Controls.Add(Me.Label13)
        Me.Panel2.Controls.Add(Me.combocategory)
        Me.Panel2.Controls.Add(Me.dtdate)
        Me.Panel2.Controls.Add(Me.txtappby)
        Me.Panel2.Controls.Add(Me.comborcenter)
        Me.Panel2.Controls.Add(Me.Label8)
        Me.Panel2.Controls.Add(Me.combotype)
        Me.Panel2.Controls.Add(Me.Label3)
        Me.Panel2.Controls.Add(Me.txtpurpose)
        Me.Panel2.Controls.Add(Me.Label12)
        Me.Panel2.Controls.Add(Me.txtstatus)
        Me.Panel2.Controls.Add(Me.Label11)
        Me.Panel2.Controls.Add(Me.txttime)
        Me.Panel2.Controls.Add(Me.Label10)
        Me.Panel2.Controls.Add(Me.Label9)
        Me.Panel2.Controls.Add(Me.txtprno)
        Me.Panel2.Controls.Add(Me.Label7)
        Me.Panel2.Controls.Add(Me.Label6)
        Me.Panel2.Controls.Add(Me.Label5)
        Me.Panel2.Controls.Add(Me.txtreqby)
        Me.Panel2.Controls.Add(Me.combofcluster)
        Me.Panel2.Controls.Add(Me.Label4)
        Me.Panel2.Controls.Add(Me.Label2)
        Me.Panel2.Controls.Add(Me.Label1)
        Me.Panel2.Controls.Add(Me.combooffsec)
        Me.Panel2.Controls.Add(Me.txtename)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel2.Location = New System.Drawing.Point(0, 51)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(1222, 180)
        Me.Panel2.TabIndex = 75
        '
        'dtdate
        '
        Me.dtdate.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.dtdate.Location = New System.Drawing.Point(781, 26)
        Me.dtdate.Name = "dtdate"
        Me.dtdate.Size = New System.Drawing.Size(176, 20)
        Me.dtdate.TabIndex = 3
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Location = New System.Drawing.Point(265, 10)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(121, 13)
        Me.Label13.TabIndex = 77
        Me.Label13.Text = "• Procurement Category"
        '
        'combocategory
        '
        Me.combocategory.BackColor = System.Drawing.Color.SeaShell
        Me.combocategory.FormattingEnabled = True
        Me.combocategory.Items.AddRange(New Object() {"ACTIVITY SUPPLIES AND OTHER MATERIALS", "CONSTRUCTION MATERIALS", "FURNITURE AND FIXTURES", "GENERAL MERCHANDISE", "HOTEL, LODGING, AND MEETING FACILITIES", "ICT EQUIPMENT", "MEALS/CATERING SERVICES", "OFFICE EQUIPMENT AND OTHER MATERIALS", "OFFICE SUPPLIES AND OTHER MATERIALS", "PETROLEUM, OIL, AND LUBRICANTS", "PRINTING SERVICES", "REPAIRS AND MAINTENANCE", "SERVICE PROVIDER", "SUBSCRIPTION", "TOKENS AND AWARD", "TRANSPORTATION SERVICES"})
        Me.combocategory.Location = New System.Drawing.Point(268, 26)
        Me.combocategory.Name = "combocategory"
        Me.combocategory.Size = New System.Drawing.Size(326, 21)
        Me.combocategory.TabIndex = 1
        '
        'frmprfile
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(1222, 744)
        Me.Controls.Add(Me.Panel2)
        Me.Controls.Add(Me.lbltotal)
        Me.Controls.Add(Me.lblid)
        Me.Controls.Add(Me.cmdsubmit)
        Me.Controls.Add(Me.cmdaddapp)
        Me.Controls.Add(Me.cmdsave)
        Me.Controls.Add(Me.cmdclose)
        Me.Controls.Add(Me.DataGridView1)
        Me.Controls.Add(Me.cmdprint)
        Me.Controls.Add(Me.cmdadd)
        Me.Controls.Add(Me.cmdedit)
        Me.Controls.Add(Me.cmddelete)
        Me.Controls.Add(Me.Panel1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmprfile"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Purchase Request | PR No.:"
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel1.ResumeLayout(False)
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents cmdsave As Button
    Friend WithEvents cmdclose As Button
    Friend WithEvents Label12 As Label
    Friend WithEvents txtstatus As TextBox
    Friend WithEvents Label11 As Label
    Friend WithEvents txttime As TextBox
    Friend WithEvents Label10 As Label
    Friend WithEvents Label9 As Label
    Friend WithEvents txtprno As TextBox
    Friend WithEvents DataGridView1 As DataGridView
    Friend WithEvents cmdprint As Button
    Friend WithEvents cmdadd As Button
    Friend WithEvents cmdedit As Button
    Friend WithEvents cmddelete As Button
    Friend WithEvents Label7 As Label
    Friend WithEvents Label6 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents txtreqby As TextBox
    Friend WithEvents combofcluster As ComboBox
    Friend WithEvents Label4 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents combooffsec As ComboBox
    Friend WithEvents lbltitle As Label
    Friend WithEvents txtename As TextBox
    Friend WithEvents Panel1 As Panel
    Friend WithEvents Label3 As Label
    Friend WithEvents txtpurpose As TextBox
    Friend WithEvents cmdaddapp As Button
    Friend WithEvents lblid As Label
    Friend WithEvents lbltotal As Label
    Friend WithEvents cmdsubmit As Button
    Friend WithEvents Label8 As Label
    Friend WithEvents combotype As ComboBox
    Friend WithEvents comborcenter As TextBox
    Friend WithEvents txtappby As ComboBox
    Friend WithEvents Panel2 As Panel
    Friend WithEvents dtdate As DateTimePicker
    Friend WithEvents Label13 As Label
    Friend WithEvents combocategory As ComboBox
End Class
