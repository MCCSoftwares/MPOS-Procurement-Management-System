<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmdeliveryfile2
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmdeliveryfile2))
        Me.lbladate = New System.Windows.Forms.Label()
        Me.txtttype = New System.Windows.Forms.ComboBox()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.dtiardate = New System.Windows.Forms.DateTimePicker()
        Me.lblidate = New System.Windows.Forms.Label()
        Me.comboscustodian = New System.Windows.Forms.ComboBox()
        Me.comboinspector = New System.Windows.Forms.ComboBox()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.cbinspected = New System.Windows.Forms.CheckBox()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.lblpodate = New System.Windows.Forms.Label()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.dtidate = New System.Windows.Forms.DateTimePicker()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.cbpartial = New System.Windows.Forms.CheckBox()
        Me.cbcomplete = New System.Windows.Forms.CheckBox()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.cmdadd = New System.Windows.Forms.Button()
        Me.txtino = New System.Windows.Forms.TextBox()
        Me.lblpoid = New System.Windows.Forms.Label()
        Me.lblprid = New System.Windows.Forms.Label()
        Me.lbliarid = New System.Windows.Forms.Label()
        Me.txtstatus = New System.Windows.Forms.TextBox()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.txtiarno = New System.Windows.Forms.TextBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.dtddelivery = New System.Windows.Forms.DateTimePicker()
        Me.txtfcluster = New System.Windows.Forms.TextBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.txtcname = New System.Windows.Forms.TextBox()
        Me.lbl = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.txtpdelivery = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.txtrcc = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.txtreqoff = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.txtpono = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.txtprno = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.cmddelete = New System.Windows.Forms.Button()
        Me.lbltitle = New System.Windows.Forms.Label()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.cmdupdate = New System.Windows.Forms.Button()
        Me.cmdsubmit = New System.Windows.Forms.Button()
        Me.DataGridView1 = New System.Windows.Forms.DataGridView()
        Me.cmdprint = New System.Windows.Forms.Button()
        Me.cmdclose = New System.Windows.Forms.Button()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.Panel1.SuspendLayout()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lbladate
        '
        Me.lbladate.AutoSize = True
        Me.lbladate.Font = New System.Drawing.Font("Calibri", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbladate.Location = New System.Drawing.Point(115, 53)
        Me.lbladate.Name = "lbladate"
        Me.lbladate.Size = New System.Drawing.Size(51, 15)
        Me.lbladate.TabIndex = 153
        Me.lbladate.Text = "lbladate"
        '
        'txtttype
        '
        Me.txtttype.BackColor = System.Drawing.Color.SeaShell
        Me.txtttype.FormattingEnabled = True
        Me.txtttype.Items.AddRange(New Object() {"Cash Advance", "Reimbursement"})
        Me.txtttype.Location = New System.Drawing.Point(12, 22)
        Me.txtttype.Name = "txtttype"
        Me.txtttype.Size = New System.Drawing.Size(223, 21)
        Me.txtttype.TabIndex = 176
        '
        'Label17
        '
        Me.Label17.AutoSize = True
        Me.Label17.Location = New System.Drawing.Point(9, 5)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(90, 13)
        Me.Label17.TabIndex = 174
        Me.Label17.Text = "Transaction Type"
        '
        'dtiardate
        '
        Me.dtiardate.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.dtiardate.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.dtiardate.Location = New System.Drawing.Point(444, 21)
        Me.dtiardate.Name = "dtiardate"
        Me.dtiardate.Size = New System.Drawing.Size(203, 22)
        Me.dtiardate.TabIndex = 173
        '
        'lblidate
        '
        Me.lblidate.AutoSize = True
        Me.lblidate.Font = New System.Drawing.Font("Calibri", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblidate.Location = New System.Drawing.Point(104, 52)
        Me.lblidate.Name = "lblidate"
        Me.lblidate.Size = New System.Drawing.Size(48, 15)
        Me.lblidate.TabIndex = 152
        Me.lblidate.Text = "lblidate"
        '
        'comboscustodian
        '
        Me.comboscustodian.FormattingEnabled = True
        Me.comboscustodian.Location = New System.Drawing.Point(777, 151)
        Me.comboscustodian.Name = "comboscustodian"
        Me.comboscustodian.Size = New System.Drawing.Size(285, 21)
        Me.comboscustodian.TabIndex = 172
        '
        'comboinspector
        '
        Me.comboinspector.FormattingEnabled = True
        Me.comboinspector.Location = New System.Drawing.Point(486, 151)
        Me.comboinspector.Name = "comboinspector"
        Me.comboinspector.Size = New System.Drawing.Size(285, 21)
        Me.comboinspector.TabIndex = 171
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.Location = New System.Drawing.Point(775, 134)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(289, 13)
        Me.Label16.TabIndex = 169
        Me.Label16.Text = "Supply and Property Custodian/Head/Immediate Supervisor"
        '
        'GroupBox1
        '
        Me.GroupBox1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox1.Controls.Add(Me.lblidate)
        Me.GroupBox1.Controls.Add(Me.Label10)
        Me.GroupBox1.Controls.Add(Me.cbinspected)
        Me.GroupBox1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox1.Location = New System.Drawing.Point(476, 668)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(405, 82)
        Me.GroupBox1.TabIndex = 182
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Inspection"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(22, 52)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(83, 13)
        Me.Label10.TabIndex = 151
        Me.Label10.Text = "Date Inspected:"
        '
        'cbinspected
        '
        Me.cbinspected.AutoSize = True
        Me.cbinspected.Location = New System.Drawing.Point(25, 28)
        Me.cbinspected.Name = "cbinspected"
        Me.cbinspected.Size = New System.Drawing.Size(354, 17)
        Me.cbinspected.TabIndex = 150
        Me.cbinspected.Text = "Inspected, verified and found in order as on quantity and specification" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        Me.cbinspected.UseVisualStyleBackColor = True
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(35, 54)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(82, 13)
        Me.Label11.TabIndex = 154
        Me.Label11.Text = "Date Accepted:"
        '
        'lblpodate
        '
        Me.lblpodate.AutoSize = True
        Me.lblpodate.Location = New System.Drawing.Point(747, 5)
        Me.lblpodate.Name = "lblpodate"
        Me.lblpodate.Size = New System.Drawing.Size(50, 13)
        Me.lblpodate.TabIndex = 168
        Me.lblpodate.Text = "lblpodate"
        Me.lblpodate.Visible = False
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Location = New System.Drawing.Point(483, 134)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(185, 13)
        Me.Label15.TabIndex = 166
        Me.Label15.Text = "Inspection Officer/Committee Member"
        '
        'dtidate
        '
        Me.dtidate.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.dtidate.Location = New System.Drawing.Point(227, 150)
        Me.dtidate.Name = "dtidate"
        Me.dtidate.Size = New System.Drawing.Size(253, 22)
        Me.dtidate.TabIndex = 165
        '
        'GroupBox2
        '
        Me.GroupBox2.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox2.Controls.Add(Me.lbladate)
        Me.GroupBox2.Controls.Add(Me.Label11)
        Me.GroupBox2.Controls.Add(Me.cbpartial)
        Me.GroupBox2.Controls.Add(Me.cbcomplete)
        Me.GroupBox2.Location = New System.Drawing.Point(889, 668)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(314, 81)
        Me.GroupBox2.TabIndex = 183
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Acceptance"
        '
        'cbpartial
        '
        Me.cbpartial.AutoSize = True
        Me.cbpartial.Location = New System.Drawing.Point(123, 28)
        Me.cbpartial.Name = "cbpartial"
        Me.cbpartial.Size = New System.Drawing.Size(169, 17)
        Me.cbpartial.TabIndex = 153
        Me.cbpartial.Text = "Partial (Please specify quanity)"
        Me.cbpartial.UseVisualStyleBackColor = True
        '
        'cbcomplete
        '
        Me.cbcomplete.AutoSize = True
        Me.cbcomplete.Location = New System.Drawing.Point(38, 28)
        Me.cbcomplete.Name = "cbcomplete"
        Me.cbcomplete.Size = New System.Drawing.Size(70, 17)
        Me.cbcomplete.TabIndex = 152
        Me.cbcomplete.Text = "Complete"
        Me.cbcomplete.UseVisualStyleBackColor = True
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Location = New System.Drawing.Point(224, 134)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(68, 13)
        Me.Label14.TabIndex = 164
        Me.Label14.Text = "Invoice Date"
        '
        'cmdadd
        '
        Me.cmdadd.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdadd.FlatAppearance.BorderSize = 0
        Me.cmdadd.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cmdadd.Image = CType(resources.GetObject("cmdadd.Image"), System.Drawing.Image)
        Me.cmdadd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdadd.Location = New System.Drawing.Point(12, 257)
        Me.cmdadd.Name = "cmdadd"
        Me.cmdadd.Size = New System.Drawing.Size(100, 46)
        Me.cmdadd.TabIndex = 185
        Me.cmdadd.Text = "Add"
        Me.cmdadd.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdadd.UseVisualStyleBackColor = True
        '
        'txtino
        '
        Me.txtino.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtino.Location = New System.Drawing.Point(14, 150)
        Me.txtino.Name = "txtino"
        Me.txtino.Size = New System.Drawing.Size(208, 22)
        Me.txtino.TabIndex = 163
        Me.txtino.TabStop = False
        '
        'lblpoid
        '
        Me.lblpoid.AutoSize = True
        Me.lblpoid.Location = New System.Drawing.Point(804, 5)
        Me.lblpoid.Name = "lblpoid"
        Me.lblpoid.Size = New System.Drawing.Size(37, 13)
        Me.lblpoid.TabIndex = 161
        Me.lblpoid.Text = "lblpoid"
        Me.lblpoid.Visible = False
        '
        'lblprid
        '
        Me.lblprid.AutoSize = True
        Me.lblprid.Location = New System.Drawing.Point(957, 5)
        Me.lblprid.Name = "lblprid"
        Me.lblprid.Size = New System.Drawing.Size(34, 13)
        Me.lblprid.TabIndex = 160
        Me.lblprid.Text = "lblprid"
        Me.lblprid.Visible = False
        '
        'lbliarid
        '
        Me.lbliarid.AutoSize = True
        Me.lbliarid.Location = New System.Drawing.Point(349, 4)
        Me.lbliarid.Name = "lbliarid"
        Me.lbliarid.Size = New System.Drawing.Size(36, 13)
        Me.lbliarid.TabIndex = 159
        Me.lbliarid.Text = "lbliarid"
        Me.lbliarid.Visible = False
        '
        'txtstatus
        '
        Me.txtstatus.BackColor = System.Drawing.Color.SeaShell
        Me.txtstatus.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtstatus.Location = New System.Drawing.Point(1060, 21)
        Me.txtstatus.Name = "txtstatus"
        Me.txtstatus.Size = New System.Drawing.Size(140, 22)
        Me.txtstatus.TabIndex = 158
        Me.txtstatus.TabStop = False
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Location = New System.Drawing.Point(11, 134)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(82, 13)
        Me.Label13.TabIndex = 162
        Me.Label13.Text = "Invoice Number"
        '
        'Panel2
        '
        Me.Panel2.BackColor = System.Drawing.Color.WhiteSmoke
        Me.Panel2.Controls.Add(Me.txtttype)
        Me.Panel2.Controls.Add(Me.Label17)
        Me.Panel2.Controls.Add(Me.dtiardate)
        Me.Panel2.Controls.Add(Me.comboscustodian)
        Me.Panel2.Controls.Add(Me.comboinspector)
        Me.Panel2.Controls.Add(Me.Label16)
        Me.Panel2.Controls.Add(Me.lblpodate)
        Me.Panel2.Controls.Add(Me.Label15)
        Me.Panel2.Controls.Add(Me.dtidate)
        Me.Panel2.Controls.Add(Me.Label14)
        Me.Panel2.Controls.Add(Me.txtino)
        Me.Panel2.Controls.Add(Me.Label13)
        Me.Panel2.Controls.Add(Me.lblpoid)
        Me.Panel2.Controls.Add(Me.lblprid)
        Me.Panel2.Controls.Add(Me.lbliarid)
        Me.Panel2.Controls.Add(Me.txtstatus)
        Me.Panel2.Controls.Add(Me.Label12)
        Me.Panel2.Controls.Add(Me.Label9)
        Me.Panel2.Controls.Add(Me.txtiarno)
        Me.Panel2.Controls.Add(Me.Label8)
        Me.Panel2.Controls.Add(Me.dtddelivery)
        Me.Panel2.Controls.Add(Me.txtfcluster)
        Me.Panel2.Controls.Add(Me.Label7)
        Me.Panel2.Controls.Add(Me.txtcname)
        Me.Panel2.Controls.Add(Me.lbl)
        Me.Panel2.Controls.Add(Me.Label6)
        Me.Panel2.Controls.Add(Me.txtpdelivery)
        Me.Panel2.Controls.Add(Me.Label5)
        Me.Panel2.Controls.Add(Me.txtrcc)
        Me.Panel2.Controls.Add(Me.Label4)
        Me.Panel2.Controls.Add(Me.txtreqoff)
        Me.Panel2.Controls.Add(Me.Label3)
        Me.Panel2.Controls.Add(Me.txtpono)
        Me.Panel2.Controls.Add(Me.Label2)
        Me.Panel2.Controls.Add(Me.txtprno)
        Me.Panel2.Controls.Add(Me.Label1)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel2.Location = New System.Drawing.Point(0, 51)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(1224, 179)
        Me.Panel2.TabIndex = 184
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Location = New System.Drawing.Point(1057, 5)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(37, 13)
        Me.Label12.TabIndex = 157
        Me.Label12.Text = "Status"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(441, 5)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(30, 13)
        Me.Label9.TabIndex = 142
        Me.Label9.Text = "Date"
        '
        'txtiarno
        '
        Me.txtiarno.BackColor = System.Drawing.Color.SeaShell
        Me.txtiarno.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtiarno.Location = New System.Drawing.Point(241, 21)
        Me.txtiarno.Name = "txtiarno"
        Me.txtiarno.Size = New System.Drawing.Size(197, 22)
        Me.txtiarno.TabIndex = 141
        Me.txtiarno.TabStop = False
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(238, 5)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(65, 13)
        Me.Label8.TabIndex = 140
        Me.Label8.Text = "IAR Number"
        '
        'dtddelivery
        '
        Me.dtddelivery.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.dtddelivery.Location = New System.Drawing.Point(811, 107)
        Me.dtddelivery.Name = "dtddelivery"
        Me.dtddelivery.Size = New System.Drawing.Size(253, 22)
        Me.dtddelivery.TabIndex = 139
        '
        'txtfcluster
        '
        Me.txtfcluster.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtfcluster.Location = New System.Drawing.Point(476, 64)
        Me.txtfcluster.Name = "txtfcluster"
        Me.txtfcluster.Size = New System.Drawing.Size(350, 22)
        Me.txtfcluster.TabIndex = 138
        Me.txtfcluster.TabStop = False
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(473, 48)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(66, 13)
        Me.Label7.TabIndex = 137
        Me.Label7.Text = "Fund Cluster"
        '
        'txtcname
        '
        Me.txtcname.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtcname.Location = New System.Drawing.Point(14, 107)
        Me.txtcname.Name = "txtcname"
        Me.txtcname.Size = New System.Drawing.Size(456, 22)
        Me.txtcname.TabIndex = 136
        Me.txtcname.TabStop = False
        '
        'lbl
        '
        Me.lbl.AutoSize = True
        Me.lbl.Location = New System.Drawing.Point(11, 91)
        Me.lbl.Name = "lbl"
        Me.lbl.Size = New System.Drawing.Size(76, 13)
        Me.lbl.TabIndex = 135
        Me.lbl.Text = "Supplier Name"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(808, 91)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(83, 13)
        Me.Label6.TabIndex = 133
        Me.Label6.Text = "Date of Delivery"
        '
        'txtpdelivery
        '
        Me.txtpdelivery.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtpdelivery.Location = New System.Drawing.Point(476, 107)
        Me.txtpdelivery.Name = "txtpdelivery"
        Me.txtpdelivery.Size = New System.Drawing.Size(329, 22)
        Me.txtpdelivery.TabIndex = 132
        Me.txtpdelivery.TabStop = False
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(473, 91)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(87, 13)
        Me.Label5.TabIndex = 131
        Me.Label5.Text = "Place of Delivery"
        '
        'txtrcc
        '
        Me.txtrcc.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtrcc.Location = New System.Drawing.Point(339, 64)
        Me.txtrcc.Name = "txtrcc"
        Me.txtrcc.Size = New System.Drawing.Size(131, 22)
        Me.txtrcc.TabIndex = 130
        Me.txtrcc.TabStop = False
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(336, 48)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(133, 13)
        Me.Label4.TabIndex = 129
        Me.Label4.Text = "Responsibility Center Code"
        '
        'txtreqoff
        '
        Me.txtreqoff.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtreqoff.Location = New System.Drawing.Point(13, 64)
        Me.txtreqoff.Name = "txtreqoff"
        Me.txtreqoff.Size = New System.Drawing.Size(320, 22)
        Me.txtreqoff.TabIndex = 128
        Me.txtreqoff.TabStop = False
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(10, 48)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(92, 13)
        Me.Label3.TabIndex = 127
        Me.Label3.Text = "Requesting Office"
        '
        'txtpono
        '
        Me.txtpono.BackColor = System.Drawing.Color.SeaShell
        Me.txtpono.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtpono.Location = New System.Drawing.Point(654, 21)
        Me.txtpono.Name = "txtpono"
        Me.txtpono.Size = New System.Drawing.Size(197, 22)
        Me.txtpono.TabIndex = 126
        Me.txtpono.TabStop = False
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(651, 5)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(62, 13)
        Me.Label2.TabIndex = 125
        Me.Label2.Text = "PO Number"
        '
        'txtprno
        '
        Me.txtprno.BackColor = System.Drawing.Color.SeaShell
        Me.txtprno.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtprno.Location = New System.Drawing.Point(857, 21)
        Me.txtprno.Name = "txtprno"
        Me.txtprno.Size = New System.Drawing.Size(197, 22)
        Me.txtprno.TabIndex = 124
        Me.txtprno.TabStop = False
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(854, 5)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(62, 13)
        Me.Label1.TabIndex = 123
        Me.Label1.Text = "PR Number"
        '
        'cmddelete
        '
        Me.cmddelete.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmddelete.FlatAppearance.BorderSize = 0
        Me.cmddelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cmddelete.Image = CType(resources.GetObject("cmddelete.Image"), System.Drawing.Image)
        Me.cmddelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmddelete.Location = New System.Drawing.Point(118, 257)
        Me.cmddelete.Name = "cmddelete"
        Me.cmddelete.Size = New System.Drawing.Size(100, 46)
        Me.cmddelete.TabIndex = 186
        Me.cmddelete.Text = "Delete"
        Me.cmddelete.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmddelete.UseVisualStyleBackColor = True
        '
        'lbltitle
        '
        Me.lbltitle.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.lbltitle.Font = New System.Drawing.Font("Calibri", 20.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbltitle.ForeColor = System.Drawing.Color.White
        Me.lbltitle.Location = New System.Drawing.Point(107, 9)
        Me.lbltitle.Name = "lbltitle"
        Me.lbltitle.Size = New System.Drawing.Size(1007, 33)
        Me.lbltitle.TabIndex = 0
        Me.lbltitle.Text = "INSPECTION AND ACCPTANCE REPORT"
        Me.lbltitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.FromArgb(CType(CType(35, Byte), Integer), CType(CType(40, Byte), Integer), CType(CType(44, Byte), Integer))
        Me.Panel1.Controls.Add(Me.lbltitle)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(1224, 51)
        Me.Panel1.TabIndex = 176
        '
        'cmdupdate
        '
        Me.cmdupdate.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cmdupdate.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdupdate.FlatAppearance.BorderSize = 0
        Me.cmdupdate.Image = CType(resources.GetObject("cmdupdate.Image"), System.Drawing.Image)
        Me.cmdupdate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdupdate.Location = New System.Drawing.Point(12, 666)
        Me.cmdupdate.Name = "cmdupdate"
        Me.cmdupdate.Size = New System.Drawing.Size(146, 46)
        Me.cmdupdate.TabIndex = 180
        Me.cmdupdate.Text = "Save"
        Me.cmdupdate.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdupdate.UseVisualStyleBackColor = True
        '
        'cmdsubmit
        '
        Me.cmdsubmit.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdsubmit.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdsubmit.Image = CType(resources.GetObject("cmdsubmit.Image"), System.Drawing.Image)
        Me.cmdsubmit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdsubmit.Location = New System.Drawing.Point(945, 251)
        Me.cmdsubmit.Name = "cmdsubmit"
        Me.cmdsubmit.Size = New System.Drawing.Size(255, 48)
        Me.cmdsubmit.TabIndex = 179
        Me.cmdsubmit.Text = "Import to Inventory and Complete Purchase"
        Me.cmdsubmit.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdsubmit.UseVisualStyleBackColor = True
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
        Me.DataGridView1.Location = New System.Drawing.Point(12, 305)
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.ReadOnly = True
        Me.DataGridView1.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        Me.DataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DataGridView1.Size = New System.Drawing.Size(1188, 353)
        Me.DataGridView1.TabIndex = 178
        '
        'cmdprint
        '
        Me.cmdprint.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdprint.FlatAppearance.BorderSize = 0
        Me.cmdprint.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cmdprint.Image = CType(resources.GetObject("cmdprint.Image"), System.Drawing.Image)
        Me.cmdprint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdprint.Location = New System.Drawing.Point(227, 257)
        Me.cmdprint.Name = "cmdprint"
        Me.cmdprint.Size = New System.Drawing.Size(120, 46)
        Me.cmdprint.TabIndex = 177
        Me.cmdprint.Text = "Print Preview"
        Me.cmdprint.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdprint.UseVisualStyleBackColor = True
        '
        'cmdclose
        '
        Me.cmdclose.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cmdclose.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdclose.FlatAppearance.BorderSize = 0
        Me.cmdclose.Image = CType(resources.GetObject("cmdclose.Image"), System.Drawing.Image)
        Me.cmdclose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdclose.Location = New System.Drawing.Point(164, 666)
        Me.cmdclose.Name = "cmdclose"
        Me.cmdclose.Size = New System.Drawing.Size(146, 46)
        Me.cmdclose.TabIndex = 181
        Me.cmdclose.Text = "Close"
        Me.cmdclose.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdclose.UseVisualStyleBackColor = True
        '
        'frmdeliveryfile2
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(1224, 763)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.cmdadd)
        Me.Controls.Add(Me.Panel2)
        Me.Controls.Add(Me.cmddelete)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.cmdupdate)
        Me.Controls.Add(Me.cmdsubmit)
        Me.Controls.Add(Me.DataGridView1)
        Me.Controls.Add(Me.cmdprint)
        Me.Controls.Add(Me.cmdclose)
        Me.Name = "frmdeliveryfile2"
        Me.Text = "frmdeliveryfile2"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        Me.Panel1.ResumeLayout(False)
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents lbladate As Label
    Friend WithEvents txtttype As ComboBox
    Friend WithEvents Label17 As Label
    Friend WithEvents dtiardate As DateTimePicker
    Friend WithEvents lblidate As Label
    Friend WithEvents comboscustodian As ComboBox
    Friend WithEvents comboinspector As ComboBox
    Friend WithEvents Label16 As Label
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents Label10 As Label
    Friend WithEvents cbinspected As CheckBox
    Friend WithEvents Label11 As Label
    Friend WithEvents lblpodate As Label
    Friend WithEvents Label15 As Label
    Friend WithEvents dtidate As DateTimePicker
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents cbpartial As CheckBox
    Friend WithEvents cbcomplete As CheckBox
    Friend WithEvents Label14 As Label
    Friend WithEvents cmdadd As Button
    Friend WithEvents txtino As TextBox
    Friend WithEvents lblpoid As Label
    Friend WithEvents lblprid As Label
    Friend WithEvents lbliarid As Label
    Friend WithEvents txtstatus As TextBox
    Friend WithEvents Label13 As Label
    Friend WithEvents Panel2 As Panel
    Friend WithEvents Label12 As Label
    Friend WithEvents Label9 As Label
    Friend WithEvents txtiarno As TextBox
    Friend WithEvents Label8 As Label
    Friend WithEvents dtddelivery As DateTimePicker
    Friend WithEvents txtfcluster As TextBox
    Friend WithEvents Label7 As Label
    Friend WithEvents txtcname As TextBox
    Friend WithEvents lbl As Label
    Friend WithEvents Label6 As Label
    Friend WithEvents txtpdelivery As TextBox
    Friend WithEvents Label5 As Label
    Friend WithEvents txtrcc As TextBox
    Friend WithEvents Label4 As Label
    Friend WithEvents txtreqoff As TextBox
    Friend WithEvents Label3 As Label
    Friend WithEvents txtpono As TextBox
    Friend WithEvents Label2 As Label
    Friend WithEvents txtprno As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents cmddelete As Button
    Friend WithEvents lbltitle As Label
    Friend WithEvents Panel1 As Panel
    Friend WithEvents cmdupdate As Button
    Friend WithEvents cmdsubmit As Button
    Friend WithEvents DataGridView1 As DataGridView
    Friend WithEvents cmdprint As Button
    Friend WithEvents cmdclose As Button
End Class
