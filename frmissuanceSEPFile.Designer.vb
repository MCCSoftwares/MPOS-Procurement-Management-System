<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmissuanceSEPFile
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmissuanceSEPFile))
        Me.cmdadd = New System.Windows.Forms.Button()
        Me.cmdsave = New System.Windows.Forms.Button()
        Me.cmddelete = New System.Windows.Forms.Button()
        Me.cmdprint = New System.Windows.Forms.Button()
        Me.comboiby = New System.Windows.Forms.ComboBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.comborby = New System.Windows.Forms.ComboBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.combofcluster = New System.Windows.Forms.ComboBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.LBLTTYPE = New System.Windows.Forms.Label()
        Me.dticsdate = New System.Windows.Forms.DateTimePicker()
        Me.lblid = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.txticsno = New System.Windows.Forms.TextBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.txtpentity = New System.Windows.Forms.TextBox()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.DataGridView1 = New System.Windows.Forms.DataGridView()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.cmdclose = New System.Windows.Forms.Button()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Panel2.SuspendLayout()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'cmdadd
        '
        Me.cmdadd.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdadd.FlatAppearance.BorderSize = 0
        Me.cmdadd.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cmdadd.Image = CType(resources.GetObject("cmdadd.Image"), System.Drawing.Image)
        Me.cmdadd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdadd.Location = New System.Drawing.Point(12, 181)
        Me.cmdadd.Name = "cmdadd"
        Me.cmdadd.Size = New System.Drawing.Size(77, 46)
        Me.cmdadd.TabIndex = 138
        Me.cmdadd.Text = "Add"
        Me.cmdadd.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdadd.UseVisualStyleBackColor = True
        '
        'cmdsave
        '
        Me.cmdsave.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cmdsave.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdsave.FlatAppearance.BorderSize = 0
        Me.cmdsave.Image = CType(resources.GetObject("cmdsave.Image"), System.Drawing.Image)
        Me.cmdsave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdsave.Location = New System.Drawing.Point(10, 615)
        Me.cmdsave.Name = "cmdsave"
        Me.cmdsave.Size = New System.Drawing.Size(120, 46)
        Me.cmdsave.TabIndex = 136
        Me.cmdsave.Text = "Save"
        Me.cmdsave.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdsave.UseVisualStyleBackColor = True
        '
        'cmddelete
        '
        Me.cmddelete.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmddelete.FlatAppearance.BorderSize = 0
        Me.cmddelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cmddelete.Image = CType(resources.GetObject("cmddelete.Image"), System.Drawing.Image)
        Me.cmddelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmddelete.Location = New System.Drawing.Point(106, 181)
        Me.cmddelete.Name = "cmddelete"
        Me.cmddelete.Size = New System.Drawing.Size(78, 46)
        Me.cmddelete.TabIndex = 135
        Me.cmddelete.Text = "Delete"
        Me.cmddelete.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmddelete.UseVisualStyleBackColor = True
        '
        'cmdprint
        '
        Me.cmdprint.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdprint.FlatAppearance.BorderSize = 0
        Me.cmdprint.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cmdprint.Image = CType(resources.GetObject("cmdprint.Image"), System.Drawing.Image)
        Me.cmdprint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdprint.Location = New System.Drawing.Point(190, 181)
        Me.cmdprint.Name = "cmdprint"
        Me.cmdprint.Size = New System.Drawing.Size(83, 46)
        Me.cmdprint.TabIndex = 134
        Me.cmdprint.Text = "Print"
        Me.cmdprint.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdprint.UseVisualStyleBackColor = True
        '
        'comboiby
        '
        Me.comboiby.FormattingEnabled = True
        Me.comboiby.Items.AddRange(New Object() {"Regular Funds", "Special Development Fund", "Transitional Development Impact Fund", "Supplemental Fund"})
        Me.comboiby.Location = New System.Drawing.Point(423, 70)
        Me.comboiby.Name = "comboiby"
        Me.comboiby.Size = New System.Drawing.Size(399, 21)
        Me.comboiby.TabIndex = 128
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(420, 55)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(56, 13)
        Me.Label5.TabIndex = 129
        Me.Label5.Text = "Issued By:"
        '
        'comborby
        '
        Me.comborby.FormattingEnabled = True
        Me.comborby.Items.AddRange(New Object() {"Regular Funds", "Special Development Fund", "Transitional Development Impact Fund", "Supplemental Fund"})
        Me.comborby.Location = New System.Drawing.Point(7, 70)
        Me.comborby.Name = "comborby"
        Me.comborby.Size = New System.Drawing.Size(410, 21)
        Me.comborby.TabIndex = 126
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(4, 55)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(71, 13)
        Me.Label3.TabIndex = 127
        Me.Label3.Text = "Received By:"
        '
        'combofcluster
        '
        Me.combofcluster.FormattingEnabled = True
        Me.combofcluster.Items.AddRange(New Object() {"Regular Funds", "Special Development Fund", "Transitional Development Impact Fund", "Supplemental Fund"})
        Me.combofcluster.Location = New System.Drawing.Point(512, 25)
        Me.combofcluster.Name = "combofcluster"
        Me.combofcluster.Size = New System.Drawing.Size(394, 21)
        Me.combofcluster.TabIndex = 122
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(509, 10)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(75, 13)
        Me.Label4.TabIndex = 123
        Me.Label4.Text = "• Fund Cluster"
        '
        'LBLTTYPE
        '
        Me.LBLTTYPE.AutoSize = True
        Me.LBLTTYPE.Location = New System.Drawing.Point(1093, 25)
        Me.LBLTTYPE.Name = "LBLTTYPE"
        Me.LBLTTYPE.Size = New System.Drawing.Size(0, 13)
        Me.LBLTTYPE.TabIndex = 121
        Me.LBLTTYPE.Visible = False
        '
        'dticsdate
        '
        Me.dticsdate.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.dticsdate.Location = New System.Drawing.Point(193, 25)
        Me.dticsdate.Name = "dticsdate"
        Me.dticsdate.Size = New System.Drawing.Size(121, 20)
        Me.dticsdate.TabIndex = 120
        '
        'lblid
        '
        Me.lblid.AutoSize = True
        Me.lblid.Location = New System.Drawing.Point(970, 55)
        Me.lblid.Name = "lblid"
        Me.lblid.Size = New System.Drawing.Size(0, 13)
        Me.lblid.TabIndex = 119
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(190, 9)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(54, 13)
        Me.Label10.TabIndex = 86
        Me.Label10.Text = "SEP Date"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(6, 9)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(64, 13)
        Me.Label9.TabIndex = 84
        Me.Label9.Text = "ICS Number"
        '
        'txticsno
        '
        Me.txticsno.BackColor = System.Drawing.Color.SeaShell
        Me.txticsno.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txticsno.Location = New System.Drawing.Point(9, 25)
        Me.txticsno.Name = "txticsno"
        Me.txticsno.Size = New System.Drawing.Size(175, 20)
        Me.txticsno.TabIndex = 83
        Me.txticsno.TabStop = False
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(317, 9)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(64, 13)
        Me.Label7.TabIndex = 82
        Me.Label7.Text = "Entity Name"
        '
        'txtpentity
        '
        Me.txtpentity.BackColor = System.Drawing.Color.SeaShell
        Me.txtpentity.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtpentity.Location = New System.Drawing.Point(320, 25)
        Me.txtpentity.Name = "txtpentity"
        Me.txtpentity.Size = New System.Drawing.Size(186, 20)
        Me.txtpentity.TabIndex = 81
        Me.txtpentity.Text = "Ministry of Public Order & Safety"
        '
        'Panel2
        '
        Me.Panel2.BackColor = System.Drawing.Color.WhiteSmoke
        Me.Panel2.Controls.Add(Me.comboiby)
        Me.Panel2.Controls.Add(Me.Label5)
        Me.Panel2.Controls.Add(Me.comborby)
        Me.Panel2.Controls.Add(Me.Label3)
        Me.Panel2.Controls.Add(Me.combofcluster)
        Me.Panel2.Controls.Add(Me.Label4)
        Me.Panel2.Controls.Add(Me.LBLTTYPE)
        Me.Panel2.Controls.Add(Me.dticsdate)
        Me.Panel2.Controls.Add(Me.lblid)
        Me.Panel2.Controls.Add(Me.Label10)
        Me.Panel2.Controls.Add(Me.Label9)
        Me.Panel2.Controls.Add(Me.txticsno)
        Me.Panel2.Controls.Add(Me.Label7)
        Me.Panel2.Controls.Add(Me.txtpentity)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel2.Location = New System.Drawing.Point(0, 62)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(1101, 108)
        Me.Panel2.TabIndex = 133
        '
        'DataGridView1
        '
        Me.DataGridView1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView1.Location = New System.Drawing.Point(12, 228)
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.Size = New System.Drawing.Size(1077, 381)
        Me.DataGridView1.TabIndex = 132
        '
        'Label1
        '
        Me.Label1.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.Label1.Font = New System.Drawing.Font("Calibri", 20.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.White
        Me.Label1.Location = New System.Drawing.Point(30, 14)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(1031, 33)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "INVENTORY CUSTODIAN SLIP"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'cmdclose
        '
        Me.cmdclose.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cmdclose.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdclose.FlatAppearance.BorderSize = 0
        Me.cmdclose.Image = CType(resources.GetObject("cmdclose.Image"), System.Drawing.Image)
        Me.cmdclose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdclose.Location = New System.Drawing.Point(136, 615)
        Me.cmdclose.Name = "cmdclose"
        Me.cmdclose.Size = New System.Drawing.Size(120, 46)
        Me.cmdclose.TabIndex = 137
        Me.cmdclose.Text = "Close"
        Me.cmdclose.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdclose.UseVisualStyleBackColor = True
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.FromArgb(CType(CType(35, Byte), Integer), CType(CType(40, Byte), Integer), CType(CType(44, Byte), Integer))
        Me.Panel1.Controls.Add(Me.Label1)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(1101, 62)
        Me.Panel1.TabIndex = 131
        '
        'frmissuanceSEPFile
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(1101, 673)
        Me.Controls.Add(Me.cmdadd)
        Me.Controls.Add(Me.cmdsave)
        Me.Controls.Add(Me.cmddelete)
        Me.Controls.Add(Me.cmdprint)
        Me.Controls.Add(Me.Panel2)
        Me.Controls.Add(Me.DataGridView1)
        Me.Controls.Add(Me.cmdclose)
        Me.Controls.Add(Me.Panel1)
        Me.MinimizeBox = False
        Me.Name = "frmissuanceSEPFile"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Inventory Custodian Slip"
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents cmdadd As Button
    Friend WithEvents cmdsave As Button
    Friend WithEvents cmddelete As Button
    Friend WithEvents cmdprint As Button
    Friend WithEvents comboiby As ComboBox
    Friend WithEvents Label5 As Label
    Friend WithEvents comborby As ComboBox
    Friend WithEvents Label3 As Label
    Friend WithEvents combofcluster As ComboBox
    Friend WithEvents Label4 As Label
    Friend WithEvents LBLTTYPE As Label
    Friend WithEvents dticsdate As DateTimePicker
    Friend WithEvents lblid As Label
    Friend WithEvents Label10 As Label
    Friend WithEvents Label9 As Label
    Friend WithEvents txticsno As TextBox
    Friend WithEvents Label7 As Label
    Friend WithEvents txtpentity As TextBox
    Friend WithEvents Panel2 As Panel
    Friend WithEvents DataGridView1 As DataGridView
    Friend WithEvents Label1 As Label
    Friend WithEvents cmdclose As Button
    Friend WithEvents Panel1 As Panel
End Class
