<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmrisfile
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmrisfile))
        Me.dtdate = New System.Windows.Forms.DateTimePicker()
        Me.comboappby = New System.Windows.Forms.ComboBox()
        Me.txtrccode = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.txtpurpose = New System.Windows.Forms.TextBox()
        Me.lbltitle = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.txtRisNo = New System.Windows.Forms.TextBox()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.combodivision = New System.Windows.Forms.ComboBox()
        Me.comborecby = New System.Windows.Forms.ComboBox()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.comboissby = New System.Windows.Forms.ComboBox()
        Me.comboreqby = New System.Windows.Forms.ComboBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.combofcluster = New System.Windows.Forms.ComboBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.combooffsec = New System.Windows.Forms.ComboBox()
        Me.txtename = New System.Windows.Forms.TextBox()
        Me.lbltotal = New System.Windows.Forms.Label()
        Me.lblid = New System.Windows.Forms.Label()
        Me.cmdsave = New System.Windows.Forms.Button()
        Me.cmdclose = New System.Windows.Forms.Button()
        Me.DataGridView1 = New System.Windows.Forms.DataGridView()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.cmdprint = New System.Windows.Forms.Button()
        Me.cmdadd = New System.Windows.Forms.Button()
        Me.cmddelete = New System.Windows.Forms.Button()
        Me.Panel2.SuspendLayout()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'dtdate
        '
        Me.dtdate.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.dtdate.Location = New System.Drawing.Point(198, 26)
        Me.dtdate.Name = "dtdate"
        Me.dtdate.Size = New System.Drawing.Size(109, 20)
        Me.dtdate.TabIndex = 3
        '
        'comboappby
        '
        Me.comboappby.FormattingEnabled = True
        Me.comboappby.Location = New System.Drawing.Point(898, 69)
        Me.comboappby.Name = "comboappby"
        Me.comboappby.Size = New System.Drawing.Size(336, 21)
        Me.comboappby.TabIndex = 12
        '
        'txtrccode
        '
        Me.txtrccode.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtrccode.Location = New System.Drawing.Point(1139, 25)
        Me.txtrccode.Name = "txtrccode"
        Me.txtrccode.Size = New System.Drawing.Size(95, 21)
        Me.txtrccode.TabIndex = 9
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(11, 53)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(46, 13)
        Me.Label3.TabIndex = 64
        Me.Label3.Text = "Purpose"
        '
        'txtpurpose
        '
        Me.txtpurpose.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtpurpose.Location = New System.Drawing.Point(14, 69)
        Me.txtpurpose.Multiline = True
        Me.txtpurpose.Name = "txtpurpose"
        Me.txtpurpose.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtpurpose.Size = New System.Drawing.Size(520, 59)
        Me.txtpurpose.TabIndex = 10
        '
        'lbltitle
        '
        Me.lbltitle.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.lbltitle.Font = New System.Drawing.Font("Calibri", 20.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbltitle.ForeColor = System.Drawing.Color.White
        Me.lbltitle.Location = New System.Drawing.Point(118, 9)
        Me.lbltitle.Name = "lbltitle"
        Me.lbltitle.Size = New System.Drawing.Size(1007, 33)
        Me.lbltitle.TabIndex = 0
        Me.lbltitle.Text = "REQUISITION AND ISSUE SLIP"
        Me.lbltitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(198, 11)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(70, 13)
        Me.Label10.TabIndex = 56
        Me.Label10.Text = "Date Created"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(11, 10)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(68, 13)
        Me.Label9.TabIndex = 54
        Me.Label9.Text = "RIS Number:"
        '
        'txtRisNo
        '
        Me.txtRisNo.BackColor = System.Drawing.Color.SeaShell
        Me.txtRisNo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtRisNo.Location = New System.Drawing.Point(14, 26)
        Me.txtRisNo.Name = "txtRisNo"
        Me.txtRisNo.Size = New System.Drawing.Size(180, 20)
        Me.txtRisNo.TabIndex = 2
        Me.txtRisNo.TabStop = False
        '
        'Panel2
        '
        Me.Panel2.BackColor = System.Drawing.Color.WhiteSmoke
        Me.Panel2.Controls.Add(Me.Label12)
        Me.Panel2.Controls.Add(Me.combodivision)
        Me.Panel2.Controls.Add(Me.comborecby)
        Me.Panel2.Controls.Add(Me.Label11)
        Me.Panel2.Controls.Add(Me.comboissby)
        Me.Panel2.Controls.Add(Me.comboreqby)
        Me.Panel2.Controls.Add(Me.Label8)
        Me.Panel2.Controls.Add(Me.dtdate)
        Me.Panel2.Controls.Add(Me.comboappby)
        Me.Panel2.Controls.Add(Me.txtrccode)
        Me.Panel2.Controls.Add(Me.Label3)
        Me.Panel2.Controls.Add(Me.txtpurpose)
        Me.Panel2.Controls.Add(Me.Label10)
        Me.Panel2.Controls.Add(Me.Label9)
        Me.Panel2.Controls.Add(Me.txtRisNo)
        Me.Panel2.Controls.Add(Me.Label7)
        Me.Panel2.Controls.Add(Me.Label6)
        Me.Panel2.Controls.Add(Me.Label5)
        Me.Panel2.Controls.Add(Me.combofcluster)
        Me.Panel2.Controls.Add(Me.Label4)
        Me.Panel2.Controls.Add(Me.Label2)
        Me.Panel2.Controls.Add(Me.Label1)
        Me.Panel2.Controls.Add(Me.combooffsec)
        Me.Panel2.Controls.Add(Me.txtename)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel2.Location = New System.Drawing.Point(0, 51)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(1247, 145)
        Me.Panel2.TabIndex = 88
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Location = New System.Drawing.Point(679, 9)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(44, 13)
        Me.Label12.TabIndex = 72
        Me.Label12.Text = "Division"
        '
        'combodivision
        '
        Me.combodivision.FormattingEnabled = True
        Me.combodivision.Location = New System.Drawing.Point(679, 25)
        Me.combodivision.Name = "combodivision"
        Me.combodivision.Size = New System.Drawing.Size(212, 21)
        Me.combodivision.TabIndex = 71
        '
        'comborecby
        '
        Me.comborecby.FormattingEnabled = True
        Me.comborecby.Location = New System.Drawing.Point(899, 109)
        Me.comborecby.Name = "comborecby"
        Me.comborecby.Size = New System.Drawing.Size(336, 21)
        Me.comborecby.TabIndex = 70
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(896, 93)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(154, 13)
        Me.Label11.TabIndex = 69
        Me.Label11.Text = "Received By: (Name - Position)"
        '
        'comboissby
        '
        Me.comboissby.FormattingEnabled = True
        Me.comboissby.Location = New System.Drawing.Point(555, 109)
        Me.comboissby.Name = "comboissby"
        Me.comboissby.Size = New System.Drawing.Size(336, 21)
        Me.comboissby.TabIndex = 68
        '
        'comboreqby
        '
        Me.comboreqby.FormattingEnabled = True
        Me.comboreqby.Location = New System.Drawing.Point(555, 68)
        Me.comboreqby.Name = "comboreqby"
        Me.comboreqby.Size = New System.Drawing.Size(336, 21)
        Me.comboreqby.TabIndex = 67
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(552, 93)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(139, 13)
        Me.Label8.TabIndex = 66
        Me.Label8.Text = "Issued By: (Name - Position)"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(310, 10)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(67, 13)
        Me.Label7.TabIndex = 52
        Me.Label7.Text = "Entitty Name"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(895, 53)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(193, 13)
        Me.Label6.TabIndex = 50
        Me.Label6.Text = "Approving Signatorial: (Name - Position)"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(552, 53)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(160, 13)
        Me.Label5.TabIndex = 48
        Me.Label5.Text = "Requested By: (Name - Position)"
        '
        'combofcluster
        '
        Me.combofcluster.FormattingEnabled = True
        Me.combofcluster.Items.AddRange(New Object() {"Regular Funds", "Special Development Fund", "Transitional Development Impact Fund", "Supplemental Fund"})
        Me.combofcluster.Location = New System.Drawing.Point(426, 25)
        Me.combofcluster.Name = "combofcluster"
        Me.combofcluster.Size = New System.Drawing.Size(247, 21)
        Me.combofcluster.TabIndex = 7
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(423, 6)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(75, 13)
        Me.Label4.TabIndex = 44
        Me.Label4.Text = "• Fund Cluster"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(1136, 9)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(53, 13)
        Me.Label2.TabIndex = 37
        Me.Label2.Text = "RC Code:"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(894, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(85, 13)
        Me.Label1.TabIndex = 35
        Me.Label1.Text = "• Office/Section"
        '
        'combooffsec
        '
        Me.combooffsec.FormattingEnabled = True
        Me.combooffsec.Items.AddRange(New Object() {"OFFICE OF THE MINISTER", "OFFICE OF THE DEPUTY MINISTER", "OFFICE OF DIRECTOR GENERAL", "ADMINISTRATIVE AND FINANCE DIVISION (CAO)", "ACCOUNTING SECTION", "BUDGET SECTION", "PROCUREMENT MANAGEMENT SECTION", "CASH SECTION", "ARCHIVES AND RECORDS SECTION", "HUMAN RESOURCE MANAGEMENT SECTION", "SUPPLY SECTION", "GENERAL SERVICES SECTION", "LEGAL AND LEGISLATIVE LIAISON SECTION", "PLANNING SECTION", "INFORMATION AND COMMUNICATION SECTION", "INTERNAL AUDIT SECTION", "BANGSAMORO PEACE RECONCILIATION AND UNIFICATION SERVICES", "PEACE EDUCATION DIVISION", "ALTERNATIVE DISPUTE RESOLUTION DIVISION (CAO-V)", "COMMUNITY AFFAIRS SECTION (MAG)", "COMMUNITY AFFAIRS SECTION (LDS)", "COMMUNITY AFFAIRS SECTION (SGA)", "COMMUNITY AFFAIRS SECTION (BAS)", "COMMUNITY AFFAIRS SECTION (SUL)", "COMMUNITY AFFAIRS SECTION (TAW)", "HOME AFFAIRS SERVICES", "LAW ENFORCEMENT COORDINATION DIVISION", "CIVILIAN INTELLIGENCE AND INVESTIGATION DIVISION"})
        Me.combooffsec.Location = New System.Drawing.Point(897, 25)
        Me.combooffsec.Name = "combooffsec"
        Me.combooffsec.Size = New System.Drawing.Size(238, 21)
        Me.combooffsec.TabIndex = 8
        '
        'txtename
        '
        Me.txtename.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtename.Location = New System.Drawing.Point(313, 26)
        Me.txtename.Name = "txtename"
        Me.txtename.Size = New System.Drawing.Size(110, 20)
        Me.txtename.TabIndex = 6
        Me.txtename.Text = "MPOS-BARMM"
        '
        'lbltotal
        '
        Me.lbltotal.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lbltotal.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbltotal.Location = New System.Drawing.Point(1035, 679)
        Me.lbltotal.Name = "lbltotal"
        Me.lbltotal.Size = New System.Drawing.Size(251, 34)
        Me.lbltotal.TabIndex = 87
        Me.lbltotal.Text = "0.00"
        Me.lbltotal.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblid
        '
        Me.lblid.AutoSize = True
        Me.lblid.Location = New System.Drawing.Point(884, 49)
        Me.lblid.Name = "lblid"
        Me.lblid.Size = New System.Drawing.Size(0, 13)
        Me.lblid.TabIndex = 86
        Me.lblid.Visible = False
        '
        'cmdsave
        '
        Me.cmdsave.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cmdsave.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdsave.FlatAppearance.BorderSize = 0
        Me.cmdsave.Image = CType(resources.GetObject("cmdsave.Image"), System.Drawing.Image)
        Me.cmdsave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdsave.Location = New System.Drawing.Point(11, 682)
        Me.cmdsave.Name = "cmdsave"
        Me.cmdsave.Size = New System.Drawing.Size(120, 46)
        Me.cmdsave.TabIndex = 83
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
        Me.cmdclose.Location = New System.Drawing.Point(137, 682)
        Me.cmdclose.Name = "cmdclose"
        Me.cmdclose.Size = New System.Drawing.Size(120, 46)
        Me.cmdclose.TabIndex = 84
        Me.cmdclose.Text = "Close"
        Me.cmdclose.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdclose.UseVisualStyleBackColor = True
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
        Me.DataGridView1.Location = New System.Drawing.Point(12, 248)
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.ReadOnly = True
        Me.DataGridView1.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        Me.DataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DataGridView1.Size = New System.Drawing.Size(1223, 428)
        Me.DataGridView1.TabIndex = 82
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.FromArgb(CType(CType(35, Byte), Integer), CType(CType(40, Byte), Integer), CType(CType(44, Byte), Integer))
        Me.Panel1.Controls.Add(Me.lbltitle)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(1247, 51)
        Me.Panel1.TabIndex = 85
        '
        'cmdprint
        '
        Me.cmdprint.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdprint.FlatAppearance.BorderSize = 0
        Me.cmdprint.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cmdprint.Image = CType(resources.GetObject("cmdprint.Image"), System.Drawing.Image)
        Me.cmdprint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdprint.Location = New System.Drawing.Point(201, 202)
        Me.cmdprint.Name = "cmdprint"
        Me.cmdprint.Size = New System.Drawing.Size(78, 46)
        Me.cmdprint.TabIndex = 89
        Me.cmdprint.Text = "Print"
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
        Me.cmdadd.Location = New System.Drawing.Point(11, 202)
        Me.cmdadd.Name = "cmdadd"
        Me.cmdadd.Size = New System.Drawing.Size(78, 46)
        Me.cmdadd.TabIndex = 90
        Me.cmdadd.Text = "Add"
        Me.cmdadd.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdadd.UseVisualStyleBackColor = True
        '
        'cmddelete
        '
        Me.cmddelete.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmddelete.FlatAppearance.BorderSize = 0
        Me.cmddelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cmddelete.Image = CType(resources.GetObject("cmddelete.Image"), System.Drawing.Image)
        Me.cmddelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmddelete.Location = New System.Drawing.Point(95, 202)
        Me.cmddelete.Name = "cmddelete"
        Me.cmddelete.Size = New System.Drawing.Size(99, 46)
        Me.cmddelete.TabIndex = 91
        Me.cmddelete.Text = "Delete"
        Me.cmddelete.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmddelete.UseVisualStyleBackColor = True
        '
        'frmrisfile
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(1247, 740)
        Me.Controls.Add(Me.cmddelete)
        Me.Controls.Add(Me.cmdadd)
        Me.Controls.Add(Me.cmdprint)
        Me.Controls.Add(Me.Panel2)
        Me.Controls.Add(Me.lbltotal)
        Me.Controls.Add(Me.lblid)
        Me.Controls.Add(Me.cmdsave)
        Me.Controls.Add(Me.cmdclose)
        Me.Controls.Add(Me.DataGridView1)
        Me.Controls.Add(Me.Panel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmrisfile"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = " Requisition and Issue Slip"
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents dtdate As DateTimePicker
    Friend WithEvents comboappby As ComboBox
    Friend WithEvents txtrccode As TextBox
    Friend WithEvents Label3 As Label
    Friend WithEvents txtpurpose As TextBox
    Friend WithEvents lbltitle As Label
    Friend WithEvents Label10 As Label
    Friend WithEvents Label9 As Label
    Friend WithEvents txtRisNo As TextBox
    Friend WithEvents Panel2 As Panel
    Friend WithEvents Label7 As Label
    Friend WithEvents Label6 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents combofcluster As ComboBox
    Friend WithEvents Label4 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents combooffsec As ComboBox
    Friend WithEvents txtename As TextBox
    Friend WithEvents lbltotal As Label
    Friend WithEvents lblid As Label
    Friend WithEvents cmdsave As Button
    Friend WithEvents cmdclose As Button
    Friend WithEvents DataGridView1 As DataGridView
    Friend WithEvents Panel1 As Panel
    Friend WithEvents cmdprint As Button
    Friend WithEvents comborecby As ComboBox
    Friend WithEvents Label11 As Label
    Friend WithEvents comboissby As ComboBox
    Friend WithEvents comboreqby As ComboBox
    Friend WithEvents Label8 As Label
    Friend WithEvents Label12 As Label
    Friend WithEvents combodivision As ComboBox
    Friend WithEvents cmdadd As Button
    Friend WithEvents cmddelete As Button
End Class
