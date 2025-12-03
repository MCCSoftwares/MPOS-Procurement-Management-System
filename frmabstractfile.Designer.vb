<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmabstractfile
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmabstractfile))
        Me.Label3 = New System.Windows.Forms.Label()
        Me.lbltitle = New System.Windows.Forms.Label()
        Me.txtpurpose = New System.Windows.Forms.TextBox()
        Me.cmdsave = New System.Windows.Forms.Button()
        Me.cmdclose = New System.Windows.Forms.Button()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.txtstatus = New System.Windows.Forms.TextBox()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.txtprno = New System.Windows.Forms.TextBox()
        Me.cmdprint = New System.Windows.Forms.Button()
        Me.cmdrefresh = New System.Windows.Forms.Button()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.lblprid = New System.Windows.Forms.Label()
        Me.lblid = New System.Windows.Forms.Label()
        Me.lblrfqid = New System.Windows.Forms.Label()
        Me.DataGridView1 = New System.Windows.Forms.DataGridView()
        Me.cmdscancel = New System.Windows.Forms.Button()
        Me.cmdsubmit = New System.Windows.Forms.Button()
        Me.txteuser = New System.Windows.Forms.TextBox()
        Me.panelless = New System.Windows.Forms.Panel()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.comboapproval = New System.Windows.Forms.ComboBox()
        Me.comboshead = New System.Windows.Forms.ComboBox()
        Me.comboenduser = New System.Windows.Forms.ComboBox()
        Me.txtmproc = New System.Windows.Forms.TextBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.txtaobrno = New System.Windows.Forms.TextBox()
        Me.DTDAbs = New System.Windows.Forms.DateTimePicker()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.LBLTTYPE = New System.Windows.Forms.Label()
        Me.txtbacreso = New System.Windows.Forms.TextBox()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.dtdeadline = New System.Windows.Forms.DateTimePicker()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.lblappbudget = New System.Windows.Forms.Label()
        Me.Panel1.SuspendLayout()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.panelless.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.SuspendLayout()
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(12, 51)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(46, 13)
        Me.Label3.TabIndex = 94
        Me.Label3.Text = "Purpose"
        '
        'lbltitle
        '
        Me.lbltitle.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.lbltitle.Font = New System.Drawing.Font("Calibri", 20.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbltitle.ForeColor = System.Drawing.Color.White
        Me.lbltitle.Location = New System.Drawing.Point(31, 9)
        Me.lbltitle.Name = "lbltitle"
        Me.lbltitle.Size = New System.Drawing.Size(1004, 33)
        Me.lbltitle.TabIndex = 0
        Me.lbltitle.Text = "ABSTRACT OF BIDS | PR #:"
        Me.lbltitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'txtpurpose
        '
        Me.txtpurpose.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtpurpose.Location = New System.Drawing.Point(15, 67)
        Me.txtpurpose.Multiline = True
        Me.txtpurpose.Name = "txtpurpose"
        Me.txtpurpose.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtpurpose.Size = New System.Drawing.Size(386, 61)
        Me.txtpurpose.TabIndex = 93
        '
        'cmdsave
        '
        Me.cmdsave.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cmdsave.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdsave.FlatAppearance.BorderSize = 0
        Me.cmdsave.Image = CType(resources.GetObject("cmdsave.Image"), System.Drawing.Image)
        Me.cmdsave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdsave.Location = New System.Drawing.Point(15, 660)
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
        Me.cmdclose.Location = New System.Drawing.Point(141, 660)
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
        Me.Label12.Location = New System.Drawing.Point(669, 9)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(37, 13)
        Me.Label12.TabIndex = 90
        Me.Label12.Text = "Status"
        '
        'txtstatus
        '
        Me.txtstatus.BackColor = System.Drawing.Color.SeaShell
        Me.txtstatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtstatus.Location = New System.Drawing.Point(669, 25)
        Me.txtstatus.Name = "txtstatus"
        Me.txtstatus.Size = New System.Drawing.Size(152, 20)
        Me.txtstatus.TabIndex = 89
        Me.txtstatus.TabStop = False
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(730, 52)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(51, 13)
        Me.Label11.TabIndex = 88
        Me.Label11.Text = "End-User"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(419, 9)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(84, 13)
        Me.Label10.TabIndex = 86
        Me.Label10.Text = "Date of Abstract"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(219, 9)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(65, 13)
        Me.Label9.TabIndex = 84
        Me.Label9.Text = "PR Number:"
        '
        'txtprno
        '
        Me.txtprno.BackColor = System.Drawing.Color.SeaShell
        Me.txtprno.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtprno.Location = New System.Drawing.Point(222, 25)
        Me.txtprno.Name = "txtprno"
        Me.txtprno.Size = New System.Drawing.Size(191, 20)
        Me.txtprno.TabIndex = 83
        Me.txtprno.TabStop = False
        '
        'cmdprint
        '
        Me.cmdprint.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdprint.FlatAppearance.BorderSize = 0
        Me.cmdprint.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cmdprint.Image = CType(resources.GetObject("cmdprint.Image"), System.Drawing.Image)
        Me.cmdprint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdprint.Location = New System.Drawing.Point(171, 257)
        Me.cmdprint.Name = "cmdprint"
        Me.cmdprint.Size = New System.Drawing.Size(113, 46)
        Me.cmdprint.TabIndex = 73
        Me.cmdprint.Text = "Print Preview"
        Me.cmdprint.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdprint.UseVisualStyleBackColor = True
        '
        'cmdrefresh
        '
        Me.cmdrefresh.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdrefresh.FlatAppearance.BorderSize = 0
        Me.cmdrefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cmdrefresh.Image = CType(resources.GetObject("cmdrefresh.Image"), System.Drawing.Image)
        Me.cmdrefresh.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdrefresh.Location = New System.Drawing.Point(14, 258)
        Me.cmdrefresh.Name = "cmdrefresh"
        Me.cmdrefresh.Size = New System.Drawing.Size(151, 46)
        Me.cmdrefresh.TabIndex = 71
        Me.cmdrefresh.Text = "Refresh Abstract"
        Me.cmdrefresh.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdrefresh.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(415, 52)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(109, 13)
        Me.Label2.TabIndex = 69
        Me.Label2.Text = "Mode of Procurement"
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.FromArgb(CType(CType(35, Byte), Integer), CType(CType(40, Byte), Integer), CType(CType(44, Byte), Integer))
        Me.Panel1.Controls.Add(Me.lbltitle)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(1063, 51)
        Me.Panel1.TabIndex = 65
        '
        'lblprid
        '
        Me.lblprid.AutoSize = True
        Me.lblprid.Location = New System.Drawing.Point(302, 9)
        Me.lblprid.Name = "lblprid"
        Me.lblprid.Size = New System.Drawing.Size(0, 13)
        Me.lblprid.TabIndex = 103
        Me.lblprid.Visible = False
        '
        'lblid
        '
        Me.lblid.AutoSize = True
        Me.lblid.Location = New System.Drawing.Point(951, 20)
        Me.lblid.Name = "lblid"
        Me.lblid.Size = New System.Drawing.Size(0, 13)
        Me.lblid.TabIndex = 105
        Me.lblid.Visible = False
        '
        'lblrfqid
        '
        Me.lblrfqid.AutoSize = True
        Me.lblrfqid.Location = New System.Drawing.Point(194, 9)
        Me.lblrfqid.Name = "lblrfqid"
        Me.lblrfqid.Size = New System.Drawing.Size(0, 13)
        Me.lblrfqid.TabIndex = 106
        Me.lblrfqid.Visible = False
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
        Me.DataGridView1.Location = New System.Drawing.Point(12, 306)
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.ReadOnly = True
        Me.DataGridView1.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        Me.DataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DataGridView1.Size = New System.Drawing.Size(1039, 348)
        Me.DataGridView1.TabIndex = 109
        '
        'cmdscancel
        '
        Me.cmdscancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdscancel.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdscancel.Image = CType(resources.GetObject("cmdscancel.Image"), System.Drawing.Image)
        Me.cmdscancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdscancel.Location = New System.Drawing.Point(878, 256)
        Me.cmdscancel.Name = "cmdscancel"
        Me.cmdscancel.Size = New System.Drawing.Size(173, 48)
        Me.cmdscancel.TabIndex = 125
        Me.cmdscancel.Text = "Cancel"
        Me.cmdscancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdscancel.UseVisualStyleBackColor = True
        '
        'cmdsubmit
        '
        Me.cmdsubmit.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdsubmit.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdsubmit.Image = CType(resources.GetObject("cmdsubmit.Image"), System.Drawing.Image)
        Me.cmdsubmit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdsubmit.Location = New System.Drawing.Point(703, 256)
        Me.cmdsubmit.Name = "cmdsubmit"
        Me.cmdsubmit.Size = New System.Drawing.Size(173, 48)
        Me.cmdsubmit.TabIndex = 124
        Me.cmdsubmit.Text = "Mark as Complete"
        Me.cmdsubmit.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdsubmit.UseVisualStyleBackColor = True
        '
        'txteuser
        '
        Me.txteuser.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txteuser.Location = New System.Drawing.Point(733, 67)
        Me.txteuser.Name = "txteuser"
        Me.txteuser.Size = New System.Drawing.Size(318, 20)
        Me.txteuser.TabIndex = 126
        '
        'panelless
        '
        Me.panelless.BackColor = System.Drawing.Color.Gainsboro
        Me.panelless.Controls.Add(Me.Label5)
        Me.panelless.Controls.Add(Me.Label4)
        Me.panelless.Controls.Add(Me.Label1)
        Me.panelless.Controls.Add(Me.comboapproval)
        Me.panelless.Controls.Add(Me.comboshead)
        Me.panelless.Controls.Add(Me.comboenduser)
        Me.panelless.Enabled = False
        Me.panelless.Location = New System.Drawing.Point(14, 134)
        Me.panelless.Name = "panelless"
        Me.panelless.Size = New System.Drawing.Size(1036, 61)
        Me.panelless.TabIndex = 127
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(691, 12)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(49, 13)
        Me.Label5.TabIndex = 130
        Me.Label5.Text = "Approval"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(348, 12)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(77, 13)
        Me.Label4.TabIndex = 129
        Me.Label4.Text = "Section Heads"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(5, 12)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(80, 13)
        Me.Label1.TabIndex = 128
        Me.Label1.Text = "End-User Head"
        '
        'comboapproval
        '
        Me.comboapproval.FormattingEnabled = True
        Me.comboapproval.Location = New System.Drawing.Point(694, 28)
        Me.comboapproval.Name = "comboapproval"
        Me.comboapproval.Size = New System.Drawing.Size(337, 21)
        Me.comboapproval.TabIndex = 2
        '
        'comboshead
        '
        Me.comboshead.FormattingEnabled = True
        Me.comboshead.Location = New System.Drawing.Point(351, 28)
        Me.comboshead.Name = "comboshead"
        Me.comboshead.Size = New System.Drawing.Size(337, 21)
        Me.comboshead.TabIndex = 1
        '
        'comboenduser
        '
        Me.comboenduser.FormattingEnabled = True
        Me.comboenduser.Location = New System.Drawing.Point(8, 28)
        Me.comboenduser.Name = "comboenduser"
        Me.comboenduser.Size = New System.Drawing.Size(337, 21)
        Me.comboenduser.TabIndex = 0
        '
        'txtmproc
        '
        Me.txtmproc.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtmproc.Location = New System.Drawing.Point(418, 67)
        Me.txtmproc.Name = "txtmproc"
        Me.txtmproc.Size = New System.Drawing.Size(309, 20)
        Me.txtmproc.TabIndex = 128
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(12, 9)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(80, 13)
        Me.Label6.TabIndex = 130
        Me.Label6.Text = "AOBR Number:"
        '
        'txtaobrno
        '
        Me.txtaobrno.BackColor = System.Drawing.Color.SeaShell
        Me.txtaobrno.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtaobrno.Location = New System.Drawing.Point(15, 25)
        Me.txtaobrno.Name = "txtaobrno"
        Me.txtaobrno.Size = New System.Drawing.Size(201, 20)
        Me.txtaobrno.TabIndex = 129
        Me.txtaobrno.TabStop = False
        '
        'DTDAbs
        '
        Me.DTDAbs.Location = New System.Drawing.Point(418, 25)
        Me.DTDAbs.Name = "DTDAbs"
        Me.DTDAbs.Size = New System.Drawing.Size(246, 20)
        Me.DTDAbs.TabIndex = 131
        '
        'Panel2
        '
        Me.Panel2.BackColor = System.Drawing.Color.WhiteSmoke
        Me.Panel2.Controls.Add(Me.LBLTTYPE)
        Me.Panel2.Controls.Add(Me.txtbacreso)
        Me.Panel2.Controls.Add(Me.Label13)
        Me.Panel2.Controls.Add(Me.Label8)
        Me.Panel2.Controls.Add(Me.dtdeadline)
        Me.Panel2.Controls.Add(Me.DTDAbs)
        Me.Panel2.Controls.Add(Me.Label6)
        Me.Panel2.Controls.Add(Me.lblrfqid)
        Me.Panel2.Controls.Add(Me.txtaobrno)
        Me.Panel2.Controls.Add(Me.txtmproc)
        Me.Panel2.Controls.Add(Me.panelless)
        Me.Panel2.Controls.Add(Me.txteuser)
        Me.Panel2.Controls.Add(Me.lblid)
        Me.Panel2.Controls.Add(Me.lblprid)
        Me.Panel2.Controls.Add(Me.Label3)
        Me.Panel2.Controls.Add(Me.txtpurpose)
        Me.Panel2.Controls.Add(Me.Label12)
        Me.Panel2.Controls.Add(Me.txtstatus)
        Me.Panel2.Controls.Add(Me.Label11)
        Me.Panel2.Controls.Add(Me.Label10)
        Me.Panel2.Controls.Add(Me.Label9)
        Me.Panel2.Controls.Add(Me.txtprno)
        Me.Panel2.Controls.Add(Me.Label2)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel2.Location = New System.Drawing.Point(0, 51)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(1063, 201)
        Me.Panel2.TabIndex = 132
        '
        'LBLTTYPE
        '
        Me.LBLTTYPE.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.LBLTTYPE.AutoSize = True
        Me.LBLTTYPE.Location = New System.Drawing.Point(869, 28)
        Me.LBLTTYPE.Name = "LBLTTYPE"
        Me.LBLTTYPE.Size = New System.Drawing.Size(0, 13)
        Me.LBLTTYPE.TabIndex = 136
        Me.LBLTTYPE.Visible = False
        '
        'txtbacreso
        '
        Me.txtbacreso.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtbacreso.Location = New System.Drawing.Point(733, 108)
        Me.txtbacreso.Name = "txtbacreso"
        Me.txtbacreso.Size = New System.Drawing.Size(318, 20)
        Me.txtbacreso.TabIndex = 135
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Location = New System.Drawing.Point(730, 93)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(101, 13)
        Me.Label13.TabIndex = 134
        Me.Label13.Text = "BAC Resolution No."
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(415, 92)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(49, 13)
        Me.Label8.TabIndex = 133
        Me.Label8.Text = "Deadline"
        '
        'dtdeadline
        '
        Me.dtdeadline.Location = New System.Drawing.Point(418, 108)
        Me.dtdeadline.Name = "dtdeadline"
        Me.dtdeadline.Size = New System.Drawing.Size(309, 20)
        Me.dtdeadline.TabIndex = 132
        '
        'Label7
        '
        Me.Label7.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(725, 660)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(180, 13)
        Me.Label7.TabIndex = 133
        Me.Label7.Text = "Approving Budget for the Contractor:"
        '
        'lblappbudget
        '
        Me.lblappbudget.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblappbudget.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblappbudget.Location = New System.Drawing.Point(901, 656)
        Me.lblappbudget.Name = "lblappbudget"
        Me.lblappbudget.Size = New System.Drawing.Size(149, 20)
        Me.lblappbudget.TabIndex = 134
        Me.lblappbudget.Text = "0.00"
        Me.lblappbudget.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'frmabstractfile
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(1063, 721)
        Me.Controls.Add(Me.lblappbudget)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.Panel2)
        Me.Controls.Add(Me.cmdscancel)
        Me.Controls.Add(Me.cmdsubmit)
        Me.Controls.Add(Me.DataGridView1)
        Me.Controls.Add(Me.cmdsave)
        Me.Controls.Add(Me.cmdclose)
        Me.Controls.Add(Me.cmdprint)
        Me.Controls.Add(Me.cmdrefresh)
        Me.Controls.Add(Me.Panel1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MinimizeBox = False
        Me.Name = "frmabstractfile"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Abstract of Bids"
        Me.Panel1.ResumeLayout(False)
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.panelless.ResumeLayout(False)
        Me.panelless.PerformLayout()
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Label3 As Label
    Friend WithEvents lbltitle As Label
    Friend WithEvents txtpurpose As TextBox
    Friend WithEvents cmdsave As Button
    Friend WithEvents cmdclose As Button
    Friend WithEvents Label12 As Label
    Friend WithEvents txtstatus As TextBox
    Friend WithEvents Label11 As Label
    Friend WithEvents Label10 As Label
    Friend WithEvents Label9 As Label
    Friend WithEvents txtprno As TextBox
    Friend WithEvents cmdprint As Button
    Friend WithEvents cmdrefresh As Button
    Friend WithEvents Label2 As Label
    Friend WithEvents Panel1 As Panel
    Friend WithEvents lblprid As Label
    Friend WithEvents lblid As Label
    Friend WithEvents lblrfqid As Label
    Friend WithEvents DataGridView1 As DataGridView
    Friend WithEvents cmdscancel As Button
    Friend WithEvents cmdsubmit As Button
    Friend WithEvents txteuser As TextBox
    Friend WithEvents panelless As Panel
    Friend WithEvents comboenduser As ComboBox
    Friend WithEvents Label5 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents comboapproval As ComboBox
    Friend WithEvents comboshead As ComboBox
    Friend WithEvents txtmproc As TextBox
    Friend WithEvents Label6 As Label
    Friend WithEvents txtaobrno As TextBox
    Friend WithEvents DTDAbs As DateTimePicker
    Friend WithEvents Panel2 As Panel
    Friend WithEvents txtbacreso As TextBox
    Friend WithEvents Label13 As Label
    Friend WithEvents Label8 As Label
    Friend WithEvents dtdeadline As DateTimePicker
    Friend WithEvents Label7 As Label
    Friend WithEvents lblappbudget As Label
    Friend WithEvents LBLTTYPE As Label
End Class
