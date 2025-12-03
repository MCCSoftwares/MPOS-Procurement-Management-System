<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmconfig
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmconfig))
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.cmdactivate = New System.Windows.Forms.Button()
        Me.cmdblock = New System.Windows.Forms.Button()
        Me.cmddelete = New System.Windows.Forms.Button()
        Me.cmdedit = New System.Windows.Forms.Button()
        Me.cmdnew = New System.Windows.Forms.Button()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.DataGridView1 = New System.Windows.Forms.DataGridView()
        Me.TabPage2 = New System.Windows.Forms.TabPage()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.txtsupply = New System.Windows.Forms.TextBox()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.txtbacsec = New System.Windows.Forms.TextBox()
        Me.txtaccountant = New System.Windows.Forms.TextBox()
        Me.txtminister = New System.Windows.Forms.TextBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.txtbacchair = New System.Windows.Forms.TextBox()
        Me.txtchair = New System.Windows.Forms.TextBox()
        Me.cmdsave = New System.Windows.Forms.Button()
        Me.txtmem3 = New System.Windows.Forms.TextBox()
        Me.cmdclose = New System.Windows.Forms.Button()
        Me.txtmem2 = New System.Windows.Forms.TextBox()
        Me.txtmem1 = New System.Windows.Forms.TextBox()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.TabPage3 = New System.Windows.Forms.TabPage()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.combosignatories = New System.Windows.Forms.ComboBox()
        Me.cmdsdelete = New System.Windows.Forms.Button()
        Me.cmdsedit = New System.Windows.Forms.Button()
        Me.cmdsnew = New System.Windows.Forms.Button()
        Me.panelsignatories = New System.Windows.Forms.Panel()
        Me.txtsnpos = New System.Windows.Forms.TextBox()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.combostype = New System.Windows.Forms.ComboBox()
        Me.cmdsscancel = New System.Windows.Forms.Button()
        Me.cmdssave = New System.Windows.Forms.Button()
        Me.DataGridView2 = New System.Windows.Forms.DataGridView()
        Me.Panel1.SuspendLayout()
        Me.TabControl1.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabPage2.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.TabPage3.SuspendLayout()
        Me.panelsignatories.SuspendLayout()
        CType(Me.DataGridView2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.FromArgb(CType(CType(35, Byte), Integer), CType(CType(40, Byte), Integer), CType(CType(44, Byte), Integer))
        Me.Panel1.Controls.Add(Me.Label1)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(975, 62)
        Me.Panel1.TabIndex = 25
        '
        'Label1
        '
        Me.Label1.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Calibri", 20.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.White
        Me.Label1.Location = New System.Drawing.Point(405, 15)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(178, 33)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Configurations"
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.TabPage1)
        Me.TabControl1.Controls.Add(Me.TabPage2)
        Me.TabControl1.Controls.Add(Me.TabPage3)
        Me.TabControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TabControl1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TabControl1.HotTrack = True
        Me.TabControl1.Location = New System.Drawing.Point(0, 62)
        Me.TabControl1.Multiline = True
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.Padding = New System.Drawing.Point(6, 15)
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(975, 560)
        Me.TabControl1.TabIndex = 26
        '
        'TabPage1
        '
        Me.TabPage1.BackColor = System.Drawing.Color.White
        Me.TabPage1.Controls.Add(Me.cmdactivate)
        Me.TabPage1.Controls.Add(Me.cmdblock)
        Me.TabPage1.Controls.Add(Me.cmddelete)
        Me.TabPage1.Controls.Add(Me.cmdedit)
        Me.TabPage1.Controls.Add(Me.cmdnew)
        Me.TabPage1.Controls.Add(Me.Label3)
        Me.TabPage1.Controls.Add(Me.Label2)
        Me.TabPage1.Controls.Add(Me.DataGridView1)
        Me.TabPage1.Cursor = System.Windows.Forms.Cursors.Hand
        Me.TabPage1.ForeColor = System.Drawing.Color.Black
        Me.TabPage1.Location = New System.Drawing.Point(4, 49)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(967, 507)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "Accounts"
        Me.TabPage1.ToolTipText = "Manage user account creation and access."
        '
        'cmdactivate
        '
        Me.cmdactivate.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdactivate.FlatAppearance.BorderSize = 0
        Me.cmdactivate.Image = CType(resources.GetObject("cmdactivate.Image"), System.Drawing.Image)
        Me.cmdactivate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdactivate.Location = New System.Drawing.Point(445, 55)
        Me.cmdactivate.Name = "cmdactivate"
        Me.cmdactivate.Size = New System.Drawing.Size(105, 46)
        Me.cmdactivate.TabIndex = 128
        Me.cmdactivate.Text = "Activate"
        Me.cmdactivate.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdactivate.UseVisualStyleBackColor = True
        '
        'cmdblock
        '
        Me.cmdblock.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdblock.FlatAppearance.BorderSize = 0
        Me.cmdblock.Image = CType(resources.GetObject("cmdblock.Image"), System.Drawing.Image)
        Me.cmdblock.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdblock.Location = New System.Drawing.Point(348, 55)
        Me.cmdblock.Name = "cmdblock"
        Me.cmdblock.Size = New System.Drawing.Size(96, 46)
        Me.cmdblock.TabIndex = 127
        Me.cmdblock.Text = "Block"
        Me.cmdblock.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdblock.UseVisualStyleBackColor = True
        '
        'cmddelete
        '
        Me.cmddelete.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmddelete.FlatAppearance.BorderSize = 0
        Me.cmddelete.Image = CType(resources.GetObject("cmddelete.Image"), System.Drawing.Image)
        Me.cmddelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmddelete.Location = New System.Drawing.Point(250, 55)
        Me.cmddelete.Name = "cmddelete"
        Me.cmddelete.Size = New System.Drawing.Size(96, 46)
        Me.cmddelete.TabIndex = 126
        Me.cmddelete.Text = "Delete"
        Me.cmddelete.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmddelete.UseVisualStyleBackColor = True
        '
        'cmdedit
        '
        Me.cmdedit.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdedit.FlatAppearance.BorderSize = 0
        Me.cmdedit.Image = CType(resources.GetObject("cmdedit.Image"), System.Drawing.Image)
        Me.cmdedit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdedit.Location = New System.Drawing.Point(160, 55)
        Me.cmdedit.Name = "cmdedit"
        Me.cmdedit.Size = New System.Drawing.Size(88, 46)
        Me.cmdedit.TabIndex = 125
        Me.cmdedit.Text = "Edit"
        Me.cmdedit.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdedit.UseVisualStyleBackColor = True
        '
        'cmdnew
        '
        Me.cmdnew.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdnew.FlatAppearance.BorderSize = 0
        Me.cmdnew.Image = CType(resources.GetObject("cmdnew.Image"), System.Drawing.Image)
        Me.cmdnew.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdnew.Location = New System.Drawing.Point(6, 55)
        Me.cmdnew.Name = "cmdnew"
        Me.cmdnew.Size = New System.Drawing.Size(152, 46)
        Me.cmdnew.TabIndex = 124
        Me.cmdnew.Text = "Create Account"
        Me.cmdnew.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdnew.UseVisualStyleBackColor = True
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.Location = New System.Drawing.Point(8, 34)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(214, 15)
        Me.Label3.TabIndex = 123
        Me.Label3.Text = "Manage all accounts and their access"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(8, 15)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(82, 19)
        Me.Label2.TabIndex = 122
        Me.Label2.Text = "Accounts"
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
        Me.DataGridView1.Location = New System.Drawing.Point(6, 104)
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.ReadOnly = True
        Me.DataGridView1.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        Me.DataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DataGridView1.Size = New System.Drawing.Size(951, 395)
        Me.DataGridView1.TabIndex = 121
        '
        'TabPage2
        '
        Me.TabPage2.AutoScroll = True
        Me.TabPage2.BackColor = System.Drawing.Color.White
        Me.TabPage2.Controls.Add(Me.Label16)
        Me.TabPage2.Controls.Add(Me.GroupBox2)
        Me.TabPage2.Controls.Add(Me.GroupBox1)
        Me.TabPage2.Location = New System.Drawing.Point(4, 49)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(967, 507)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "Signatories"
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.Location = New System.Drawing.Point(906, 502)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(20, 16)
        Me.Label16.TabIndex = 3
        Me.Label16.Text = "    "
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.txtsupply)
        Me.GroupBox2.Controls.Add(Me.Label12)
        Me.GroupBox2.Controls.Add(Me.txtbacsec)
        Me.GroupBox2.Controls.Add(Me.txtaccountant)
        Me.GroupBox2.Controls.Add(Me.txtminister)
        Me.GroupBox2.Controls.Add(Me.Label6)
        Me.GroupBox2.Controls.Add(Me.Label5)
        Me.GroupBox2.Controls.Add(Me.Label4)
        Me.GroupBox2.Location = New System.Drawing.Point(6, 35)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(933, 134)
        Me.GroupBox2.TabIndex = 2
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Head/Minister"
        '
        'txtsupply
        '
        Me.txtsupply.Location = New System.Drawing.Point(8, 130)
        Me.txtsupply.Multiline = True
        Me.txtsupply.Name = "txtsupply"
        Me.txtsupply.Size = New System.Drawing.Size(292, 40)
        Me.txtsupply.TabIndex = 99
        Me.txtsupply.Visible = False
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label12.Location = New System.Drawing.Point(8, 110)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(289, 13)
        Me.Label12.TabIndex = 98
        Me.Label12.Text = "Supply and Property Custodian/Head/Immediate Supervisor"
        Me.Label12.Visible = False
        '
        'txtbacsec
        '
        Me.txtbacsec.Location = New System.Drawing.Point(630, 51)
        Me.txtbacsec.Multiline = True
        Me.txtbacsec.Name = "txtbacsec"
        Me.txtbacsec.Size = New System.Drawing.Size(292, 40)
        Me.txtbacsec.TabIndex = 97
        Me.txtbacsec.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'txtaccountant
        '
        Me.txtaccountant.Location = New System.Drawing.Point(317, 51)
        Me.txtaccountant.Multiline = True
        Me.txtaccountant.Name = "txtaccountant"
        Me.txtaccountant.Size = New System.Drawing.Size(292, 40)
        Me.txtaccountant.TabIndex = 96
        Me.txtaccountant.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'txtminister
        '
        Me.txtminister.Location = New System.Drawing.Point(8, 51)
        Me.txtminister.Multiline = True
        Me.txtminister.Name = "txtminister"
        Me.txtminister.Size = New System.Drawing.Size(292, 40)
        Me.txtminister.TabIndex = 95
        Me.txtminister.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(627, 31)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(103, 16)
        Me.Label6.TabIndex = 5
        Me.Label6.Text = "BAC Secretariat"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(317, 31)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(74, 16)
        Me.Label5.TabIndex = 3
        Me.Label5.Text = "Accountant"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(5, 31)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(54, 16)
        Me.Label4.TabIndex = 1
        Me.Label4.Text = "Minister"
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.txtbacchair)
        Me.GroupBox1.Controls.Add(Me.txtchair)
        Me.GroupBox1.Controls.Add(Me.cmdsave)
        Me.GroupBox1.Controls.Add(Me.txtmem3)
        Me.GroupBox1.Controls.Add(Me.cmdclose)
        Me.GroupBox1.Controls.Add(Me.txtmem2)
        Me.GroupBox1.Controls.Add(Me.txtmem1)
        Me.GroupBox1.Controls.Add(Me.Label11)
        Me.GroupBox1.Controls.Add(Me.Label10)
        Me.GroupBox1.Controls.Add(Me.Label9)
        Me.GroupBox1.Controls.Add(Me.Label8)
        Me.GroupBox1.Controls.Add(Me.Label7)
        Me.GroupBox1.Location = New System.Drawing.Point(8, 190)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(931, 284)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Abstract of Bids : Signatories (200 Thousand Above)"
        '
        'txtbacchair
        '
        Me.txtbacchair.Location = New System.Drawing.Point(469, 158)
        Me.txtbacchair.Multiline = True
        Me.txtbacchair.Name = "txtbacchair"
        Me.txtbacchair.Size = New System.Drawing.Size(292, 40)
        Me.txtbacchair.TabIndex = 100
        Me.txtbacchair.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'txtchair
        '
        Me.txtchair.Location = New System.Drawing.Point(160, 158)
        Me.txtchair.Multiline = True
        Me.txtchair.Name = "txtchair"
        Me.txtchair.Size = New System.Drawing.Size(292, 40)
        Me.txtchair.TabIndex = 99
        Me.txtchair.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'cmdsave
        '
        Me.cmdsave.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cmdsave.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdsave.FlatAppearance.BorderSize = 0
        Me.cmdsave.Image = CType(resources.GetObject("cmdsave.Image"), System.Drawing.Image)
        Me.cmdsave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdsave.Location = New System.Drawing.Point(160, 220)
        Me.cmdsave.Name = "cmdsave"
        Me.cmdsave.Size = New System.Drawing.Size(292, 46)
        Me.cmdsave.TabIndex = 93
        Me.cmdsave.Text = "Save Changes"
        Me.cmdsave.UseVisualStyleBackColor = True
        '
        'txtmem3
        '
        Me.txtmem3.Location = New System.Drawing.Point(630, 78)
        Me.txtmem3.Multiline = True
        Me.txtmem3.Name = "txtmem3"
        Me.txtmem3.Size = New System.Drawing.Size(292, 40)
        Me.txtmem3.TabIndex = 98
        Me.txtmem3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'cmdclose
        '
        Me.cmdclose.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cmdclose.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdclose.FlatAppearance.BorderSize = 0
        Me.cmdclose.Image = CType(resources.GetObject("cmdclose.Image"), System.Drawing.Image)
        Me.cmdclose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdclose.Location = New System.Drawing.Point(469, 220)
        Me.cmdclose.Name = "cmdclose"
        Me.cmdclose.Size = New System.Drawing.Size(292, 46)
        Me.cmdclose.TabIndex = 94
        Me.cmdclose.Text = "Close"
        Me.cmdclose.UseVisualStyleBackColor = True
        '
        'txtmem2
        '
        Me.txtmem2.Location = New System.Drawing.Point(317, 77)
        Me.txtmem2.Multiline = True
        Me.txtmem2.Name = "txtmem2"
        Me.txtmem2.Size = New System.Drawing.Size(292, 40)
        Me.txtmem2.TabIndex = 97
        Me.txtmem2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'txtmem1
        '
        Me.txtmem1.Location = New System.Drawing.Point(8, 78)
        Me.txtmem1.Multiline = True
        Me.txtmem1.Name = "txtmem1"
        Me.txtmem1.Size = New System.Drawing.Size(292, 40)
        Me.txtmem1.TabIndex = 96
        Me.txtmem1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(466, 139)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(111, 16)
        Me.Label11.TabIndex = 11
        Me.Label11.Text = "BAC Chairperson"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(157, 139)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(81, 16)
        Me.Label10.TabIndex = 9
        Me.Label10.Text = "Chairperson"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(627, 58)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(70, 16)
        Me.Label9.TabIndex = 7
        Me.Label9.Text = "Member C"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(317, 58)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(70, 16)
        Me.Label8.TabIndex = 5
        Me.Label8.Text = "Member B"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(5, 58)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(70, 16)
        Me.Label7.TabIndex = 3
        Me.Label7.Text = "Member A"
        '
        'TabPage3
        '
        Me.TabPage3.BackColor = System.Drawing.Color.White
        Me.TabPage3.Controls.Add(Me.Label13)
        Me.TabPage3.Controls.Add(Me.combosignatories)
        Me.TabPage3.Controls.Add(Me.cmdsdelete)
        Me.TabPage3.Controls.Add(Me.cmdsedit)
        Me.TabPage3.Controls.Add(Me.cmdsnew)
        Me.TabPage3.Controls.Add(Me.panelsignatories)
        Me.TabPage3.Controls.Add(Me.DataGridView2)
        Me.TabPage3.Location = New System.Drawing.Point(4, 49)
        Me.TabPage3.Name = "TabPage3"
        Me.TabPage3.Size = New System.Drawing.Size(967, 507)
        Me.TabPage3.TabIndex = 2
        Me.TabPage3.Text = "Other Signatories"
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label13.Location = New System.Drawing.Point(26, 39)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(50, 15)
        Me.Label13.TabIndex = 131
        Me.Label13.Text = "Filter By"
        '
        'combosignatories
        '
        Me.combosignatories.FormattingEnabled = True
        Me.combosignatories.Location = New System.Drawing.Point(29, 58)
        Me.combosignatories.Name = "combosignatories"
        Me.combosignatories.Size = New System.Drawing.Size(297, 24)
        Me.combosignatories.TabIndex = 130
        '
        'cmdsdelete
        '
        Me.cmdsdelete.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdsdelete.FlatAppearance.BorderSize = 0
        Me.cmdsdelete.Image = CType(resources.GetObject("cmdsdelete.Image"), System.Drawing.Image)
        Me.cmdsdelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdsdelete.Location = New System.Drawing.Point(758, 37)
        Me.cmdsdelete.Name = "cmdsdelete"
        Me.cmdsdelete.Size = New System.Drawing.Size(96, 46)
        Me.cmdsdelete.TabIndex = 129
        Me.cmdsdelete.Text = "Delete"
        Me.cmdsdelete.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdsdelete.UseVisualStyleBackColor = True
        '
        'cmdsedit
        '
        Me.cmdsedit.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdsedit.FlatAppearance.BorderSize = 0
        Me.cmdsedit.Image = CType(resources.GetObject("cmdsedit.Image"), System.Drawing.Image)
        Me.cmdsedit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdsedit.Location = New System.Drawing.Point(668, 37)
        Me.cmdsedit.Name = "cmdsedit"
        Me.cmdsedit.Size = New System.Drawing.Size(88, 46)
        Me.cmdsedit.TabIndex = 128
        Me.cmdsedit.Text = "Edit"
        Me.cmdsedit.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdsedit.UseVisualStyleBackColor = True
        '
        'cmdsnew
        '
        Me.cmdsnew.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdsnew.FlatAppearance.BorderSize = 0
        Me.cmdsnew.Image = CType(resources.GetObject("cmdsnew.Image"), System.Drawing.Image)
        Me.cmdsnew.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdsnew.Location = New System.Drawing.Point(514, 37)
        Me.cmdsnew.Name = "cmdsnew"
        Me.cmdsnew.Size = New System.Drawing.Size(152, 46)
        Me.cmdsnew.TabIndex = 127
        Me.cmdsnew.Text = "Create New"
        Me.cmdsnew.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdsnew.UseVisualStyleBackColor = True
        '
        'panelsignatories
        '
        Me.panelsignatories.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.panelsignatories.Controls.Add(Me.txtsnpos)
        Me.panelsignatories.Controls.Add(Me.Label15)
        Me.panelsignatories.Controls.Add(Me.Label14)
        Me.panelsignatories.Controls.Add(Me.combostype)
        Me.panelsignatories.Controls.Add(Me.cmdsscancel)
        Me.panelsignatories.Controls.Add(Me.cmdssave)
        Me.panelsignatories.Location = New System.Drawing.Point(514, 86)
        Me.panelsignatories.Name = "panelsignatories"
        Me.panelsignatories.Size = New System.Drawing.Size(432, 249)
        Me.panelsignatories.TabIndex = 123
        '
        'txtsnpos
        '
        Me.txtsnpos.Location = New System.Drawing.Point(32, 104)
        Me.txtsnpos.Multiline = True
        Me.txtsnpos.Name = "txtsnpos"
        Me.txtsnpos.Size = New System.Drawing.Size(366, 56)
        Me.txtsnpos.TabIndex = 136
        Me.txtsnpos.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label15.Location = New System.Drawing.Point(29, 86)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(169, 15)
        Me.Label15.TabIndex = 135
        Me.Label15.Text = "Name and Position / Category"
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label14.Location = New System.Drawing.Point(29, 34)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(152, 15)
        Me.Label14.TabIndex = 133
        Me.Label14.Text = "Signatory Type or Category"
        '
        'combostype
        '
        Me.combostype.FormattingEnabled = True
        Me.combostype.Items.AddRange(New Object() {"Inspection Committee", "Inventory Committee", "Disposal Committee", "PR: Signatories", "AOB: End User Heads", "AOB: Section Heads", "AOB: Approval"})
        Me.combostype.Location = New System.Drawing.Point(32, 53)
        Me.combostype.Name = "combostype"
        Me.combostype.Size = New System.Drawing.Size(366, 24)
        Me.combostype.TabIndex = 132
        '
        'cmdsscancel
        '
        Me.cmdsscancel.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdsscancel.FlatAppearance.BorderSize = 0
        Me.cmdsscancel.Image = CType(resources.GetObject("cmdsscancel.Image"), System.Drawing.Image)
        Me.cmdsscancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdsscancel.Location = New System.Drawing.Point(218, 166)
        Me.cmdsscancel.Name = "cmdsscancel"
        Me.cmdsscancel.Size = New System.Drawing.Size(180, 46)
        Me.cmdsscancel.TabIndex = 131
        Me.cmdsscancel.Text = "Cancel"
        Me.cmdsscancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdsscancel.UseVisualStyleBackColor = True
        '
        'cmdssave
        '
        Me.cmdssave.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdssave.FlatAppearance.BorderSize = 0
        Me.cmdssave.Image = CType(resources.GetObject("cmdssave.Image"), System.Drawing.Image)
        Me.cmdssave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdssave.Location = New System.Drawing.Point(32, 166)
        Me.cmdssave.Name = "cmdssave"
        Me.cmdssave.Size = New System.Drawing.Size(180, 46)
        Me.cmdssave.TabIndex = 130
        Me.cmdssave.Text = "Save"
        Me.cmdssave.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdssave.UseVisualStyleBackColor = True
        '
        'DataGridView2
        '
        Me.DataGridView2.AllowUserToAddRows = False
        Me.DataGridView2.AllowUserToDeleteRows = False
        Me.DataGridView2.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DataGridView2.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.DataGridView2.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells
        Me.DataGridView2.BackgroundColor = System.Drawing.Color.White
        Me.DataGridView2.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.[Single]
        Me.DataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView2.Location = New System.Drawing.Point(29, 86)
        Me.DataGridView2.Name = "DataGridView2"
        Me.DataGridView2.ReadOnly = True
        Me.DataGridView2.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        Me.DataGridView2.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DataGridView2.Size = New System.Drawing.Size(481, 413)
        Me.DataGridView2.TabIndex = 122
        '
        'frmconfig
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(975, 622)
        Me.Controls.Add(Me.TabControl1)
        Me.Controls.Add(Me.Panel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmconfig"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Configurations"
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.TabControl1.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.TabPage1.PerformLayout()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabPage2.ResumeLayout(False)
        Me.TabPage2.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.TabPage3.ResumeLayout(False)
        Me.TabPage3.PerformLayout()
        Me.panelsignatories.ResumeLayout(False)
        Me.panelsignatories.PerformLayout()
        CType(Me.DataGridView2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Label1 As Label
    Friend WithEvents Panel1 As Panel
    Friend WithEvents TabControl1 As TabControl
    Friend WithEvents TabPage1 As TabPage
    Friend WithEvents TabPage2 As TabPage
    Friend WithEvents Label3 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents DataGridView1 As DataGridView
    Friend WithEvents cmdactivate As Button
    Friend WithEvents cmdblock As Button
    Friend WithEvents cmddelete As Button
    Friend WithEvents cmdedit As Button
    Friend WithEvents cmdnew As Button
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents Label6 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents Label11 As Label
    Friend WithEvents Label10 As Label
    Friend WithEvents Label9 As Label
    Friend WithEvents Label8 As Label
    Friend WithEvents Label7 As Label
    Friend WithEvents cmdsave As Button
    Friend WithEvents cmdclose As Button
    Friend WithEvents txtminister As TextBox
    Friend WithEvents txtbacsec As TextBox
    Friend WithEvents txtaccountant As TextBox
    Friend WithEvents txtbacchair As TextBox
    Friend WithEvents txtchair As TextBox
    Friend WithEvents txtmem3 As TextBox
    Friend WithEvents txtmem2 As TextBox
    Friend WithEvents txtmem1 As TextBox
    Friend WithEvents txtsupply As TextBox
    Friend WithEvents Label12 As Label
    Friend WithEvents TabPage3 As TabPage
    Friend WithEvents cmdsdelete As Button
    Friend WithEvents cmdsedit As Button
    Friend WithEvents cmdsnew As Button
    Friend WithEvents panelsignatories As Panel
    Friend WithEvents DataGridView2 As DataGridView
    Friend WithEvents Label13 As Label
    Friend WithEvents combosignatories As ComboBox
    Friend WithEvents txtsnpos As TextBox
    Friend WithEvents Label15 As Label
    Friend WithEvents Label14 As Label
    Friend WithEvents combostype As ComboBox
    Friend WithEvents cmdsscancel As Button
    Friend WithEvents cmdssave As Button
    Friend WithEvents Label16 As Label
End Class
