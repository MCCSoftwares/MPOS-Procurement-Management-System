<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmRCPPE
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmRCPPE))
        Me.FlowLayoutPanel1 = New System.Windows.Forms.FlowLayoutPanel()
        Me.cmdsignatories = New System.Windows.Forms.Button()
        Me.cmdprint = New System.Windows.Forms.Button()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.DataGridView1 = New System.Windows.Forms.DataGridView()
        Me.combotype = New System.Windows.Forms.ComboBox()
        Me.comboenduser = New System.Windows.Forms.ComboBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.txtpardate = New System.Windows.Forms.TextBox()
        Me.txtfcluster = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.FlowLayoutPanel1.SuspendLayout()
        Me.Panel1.SuspendLayout()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'FlowLayoutPanel1
        '
        Me.FlowLayoutPanel1.BackColor = System.Drawing.Color.WhiteSmoke
        Me.FlowLayoutPanel1.Controls.Add(Me.cmdsignatories)
        Me.FlowLayoutPanel1.Controls.Add(Me.cmdprint)
        Me.FlowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.FlowLayoutPanel1.Location = New System.Drawing.Point(0, 62)
        Me.FlowLayoutPanel1.Margin = New System.Windows.Forms.Padding(1)
        Me.FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        Me.FlowLayoutPanel1.Size = New System.Drawing.Size(1347, 51)
        Me.FlowLayoutPanel1.TabIndex = 25
        '
        'cmdsignatories
        '
        Me.cmdsignatories.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdsignatories.FlatAppearance.BorderSize = 0
        Me.cmdsignatories.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cmdsignatories.Image = CType(resources.GetObject("cmdsignatories.Image"), System.Drawing.Image)
        Me.cmdsignatories.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdsignatories.Location = New System.Drawing.Point(3, 3)
        Me.cmdsignatories.Name = "cmdsignatories"
        Me.cmdsignatories.Size = New System.Drawing.Size(148, 46)
        Me.cmdsignatories.TabIndex = 8
        Me.cmdsignatories.Text = "Manage Signatories"
        Me.cmdsignatories.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdsignatories.UseVisualStyleBackColor = True
        '
        'cmdprint
        '
        Me.cmdprint.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdprint.FlatAppearance.BorderSize = 0
        Me.cmdprint.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cmdprint.Image = CType(resources.GetObject("cmdprint.Image"), System.Drawing.Image)
        Me.cmdprint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdprint.Location = New System.Drawing.Point(157, 3)
        Me.cmdprint.Name = "cmdprint"
        Me.cmdprint.Size = New System.Drawing.Size(83, 46)
        Me.cmdprint.TabIndex = 9
        Me.cmdprint.Text = "Print"
        Me.cmdprint.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdprint.UseVisualStyleBackColor = True
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.FromArgb(CType(CType(35, Byte), Integer), CType(CType(40, Byte), Integer), CType(CType(44, Byte), Integer))
        Me.Panel1.Controls.Add(Me.Label1)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(1347, 62)
        Me.Panel1.TabIndex = 26
        '
        'Label1
        '
        Me.Label1.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Calibri", 20.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.White
        Me.Label1.Location = New System.Drawing.Point(292, 16)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(722, 33)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Report on the Physical Count of Property, Plant, and Equipment"
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
        Me.DataGridView1.BackgroundColor = System.Drawing.Color.LightGray
        Me.DataGridView1.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None
        Me.DataGridView1.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        Me.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView1.Location = New System.Drawing.Point(5, 207)
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.ReadOnly = True
        Me.DataGridView1.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        Me.DataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullColumnSelect
        Me.DataGridView1.Size = New System.Drawing.Size(1337, 555)
        Me.DataGridView1.TabIndex = 27
        '
        'combotype
        '
        Me.combotype.FormattingEnabled = True
        Me.combotype.Location = New System.Drawing.Point(5, 138)
        Me.combotype.Name = "combotype"
        Me.combotype.Size = New System.Drawing.Size(363, 21)
        Me.combotype.TabIndex = 28
        '
        'comboenduser
        '
        Me.comboenduser.FormattingEnabled = True
        Me.comboenduser.Location = New System.Drawing.Point(5, 180)
        Me.comboenduser.Name = "comboenduser"
        Me.comboenduser.Size = New System.Drawing.Size(363, 21)
        Me.comboenduser.TabIndex = 29
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(2, 122)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(67, 13)
        Me.Label2.TabIndex = 30
        Me.Label2.Text = "Type of PPE"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(2, 164)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(56, 13)
        Me.Label3.TabIndex = 31
        Me.Label3.Text = "For Which"
        '
        'txtpardate
        '
        Me.txtpardate.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtpardate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtpardate.Location = New System.Drawing.Point(1032, 181)
        Me.txtpardate.Name = "txtpardate"
        Me.txtpardate.Size = New System.Drawing.Size(310, 20)
        Me.txtpardate.TabIndex = 32
        '
        'txtfcluster
        '
        Me.txtfcluster.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtfcluster.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtfcluster.Location = New System.Drawing.Point(1032, 139)
        Me.txtfcluster.Name = "txtfcluster"
        Me.txtfcluster.Size = New System.Drawing.Size(310, 20)
        Me.txtfcluster.TabIndex = 33
        '
        'Label4
        '
        Me.Label4.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(1029, 123)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(66, 13)
        Me.Label4.TabIndex = 34
        Me.Label4.Text = "Fund Cluster"
        '
        'Label5
        '
        Me.Label5.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(1029, 165)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(55, 13)
        Me.Label5.TabIndex = 35
        Me.Label5.Text = "PAR Date"
        '
        'frmRCPPE
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(1347, 774)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.txtfcluster)
        Me.Controls.Add(Me.txtpardate)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.comboenduser)
        Me.Controls.Add(Me.combotype)
        Me.Controls.Add(Me.DataGridView1)
        Me.Controls.Add(Me.FlowLayoutPanel1)
        Me.Controls.Add(Me.Panel1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmRCPPE"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Report on the Physical Count of Property, Plant, and Equipment"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        Me.FlowLayoutPanel1.ResumeLayout(False)
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents FlowLayoutPanel1 As FlowLayoutPanel
    Friend WithEvents cmdsignatories As Button
    Friend WithEvents cmdprint As Button
    Friend WithEvents Panel1 As Panel
    Friend WithEvents Label1 As Label
    Friend WithEvents DataGridView1 As DataGridView
    Friend WithEvents combotype As ComboBox
    Friend WithEvents comboenduser As ComboBox
    Friend WithEvents Label2 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents txtpardate As TextBox
    Friend WithEvents txtfcluster As TextBox
    Friend WithEvents Label4 As Label
    Friend WithEvents Label5 As Label
End Class
