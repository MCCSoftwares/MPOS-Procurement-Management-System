<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmabstract
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmabstract))
        Me.FlowLayoutPanel1 = New System.Windows.Forms.FlowLayoutPanel()
        Me.cmdnew = New System.Windows.Forms.Button()
        Me.cmdopen = New System.Windows.Forms.Button()
        Me.cmddelete = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.DataGridView1 = New System.Windows.Forms.DataGridView()
        Me.cmdrefresh = New System.Windows.Forms.Button()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Comboyear = New System.Windows.Forms.ComboBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Combomproc = New System.Windows.Forms.ComboBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Combooffice = New System.Windows.Forms.ComboBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.combostatus = New System.Windows.Forms.ComboBox()
        Me.cmdsearch = New System.Windows.Forms.Button()
        Me.txtsearch = New System.Windows.Forms.TextBox()
        Me.lblpage = New System.Windows.Forms.Label()
        Me.cmdnext = New System.Windows.Forms.Button()
        Me.cmdprev = New System.Windows.Forms.Button()
        Me.FlowLayoutPanel1.SuspendLayout()
        Me.Panel1.SuspendLayout()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'FlowLayoutPanel1
        '
        Me.FlowLayoutPanel1.BackColor = System.Drawing.Color.WhiteSmoke
        Me.FlowLayoutPanel1.Controls.Add(Me.cmdnew)
        Me.FlowLayoutPanel1.Controls.Add(Me.cmdopen)
        Me.FlowLayoutPanel1.Controls.Add(Me.cmddelete)
        Me.FlowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.FlowLayoutPanel1.Location = New System.Drawing.Point(0, 62)
        Me.FlowLayoutPanel1.Margin = New System.Windows.Forms.Padding(1)
        Me.FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        Me.FlowLayoutPanel1.Size = New System.Drawing.Size(1264, 52)
        Me.FlowLayoutPanel1.TabIndex = 63
        '
        'cmdnew
        '
        Me.cmdnew.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdnew.FlatAppearance.BorderSize = 0
        Me.cmdnew.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cmdnew.Image = CType(resources.GetObject("cmdnew.Image"), System.Drawing.Image)
        Me.cmdnew.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdnew.Location = New System.Drawing.Point(3, 3)
        Me.cmdnew.Name = "cmdnew"
        Me.cmdnew.Size = New System.Drawing.Size(163, 46)
        Me.cmdnew.TabIndex = 0
        Me.cmdnew.Text = "Create New"
        Me.cmdnew.UseVisualStyleBackColor = True
        '
        'cmdopen
        '
        Me.cmdopen.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdopen.FlatAppearance.BorderSize = 0
        Me.cmdopen.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cmdopen.Image = CType(resources.GetObject("cmdopen.Image"), System.Drawing.Image)
        Me.cmdopen.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdopen.Location = New System.Drawing.Point(172, 3)
        Me.cmdopen.Name = "cmdopen"
        Me.cmdopen.Size = New System.Drawing.Size(72, 46)
        Me.cmdopen.TabIndex = 7
        Me.cmdopen.Text = "Open"
        Me.cmdopen.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdopen.UseVisualStyleBackColor = True
        '
        'cmddelete
        '
        Me.cmddelete.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmddelete.FlatAppearance.BorderSize = 0
        Me.cmddelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cmddelete.Image = CType(resources.GetObject("cmddelete.Image"), System.Drawing.Image)
        Me.cmddelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmddelete.Location = New System.Drawing.Point(250, 3)
        Me.cmddelete.Name = "cmddelete"
        Me.cmddelete.Size = New System.Drawing.Size(78, 46)
        Me.cmddelete.TabIndex = 9
        Me.cmddelete.Text = "Delete"
        Me.cmddelete.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmddelete.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Calibri", 20.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.White
        Me.Label1.Location = New System.Drawing.Point(529, 3)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(190, 33)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Abstract of Bids"
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.FromArgb(CType(CType(35, Byte), Integer), CType(CType(40, Byte), Integer), CType(CType(44, Byte), Integer))
        Me.Panel1.Controls.Add(Me.Label1)
        Me.Panel1.Controls.Add(Me.Label8)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(1264, 62)
        Me.Panel1.TabIndex = 72
        '
        'Label8
        '
        Me.Label8.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.Label8.Font = New System.Drawing.Font("Calibri", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label8.ForeColor = System.Drawing.Color.White
        Me.Label8.Location = New System.Drawing.Point(3, 28)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(1256, 33)
        Me.Label8.TabIndex = 3
        Me.Label8.Text = "Consolidate all supplier quotations into a clear, side-by-side summary for quick " &
    "comparison and decision-making"
        Me.Label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
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
        Me.DataGridView1.Location = New System.Drawing.Point(6, 175)
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.ReadOnly = True
        Me.DataGridView1.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        Me.DataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DataGridView1.Size = New System.Drawing.Size(1246, 456)
        Me.DataGridView1.TabIndex = 73
        '
        'cmdrefresh
        '
        Me.cmdrefresh.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdrefresh.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdrefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cmdrefresh.Image = CType(resources.GetObject("cmdrefresh.Image"), System.Drawing.Image)
        Me.cmdrefresh.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdrefresh.Location = New System.Drawing.Point(1167, 143)
        Me.cmdrefresh.Name = "cmdrefresh"
        Me.cmdrefresh.Size = New System.Drawing.Size(84, 26)
        Me.cmdrefresh.TabIndex = 121
        Me.cmdrefresh.Text = "Refresh"
        Me.cmdrefresh.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdrefresh.UseVisualStyleBackColor = True
        '
        'Label6
        '
        Me.Label6.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label6.AutoSize = True
        Me.Label6.ForeColor = System.Drawing.SystemColors.ControlDarkDark
        Me.Label6.Location = New System.Drawing.Point(864, 132)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(29, 13)
        Me.Label6.TabIndex = 120
        Me.Label6.Text = "Year"
        '
        'Comboyear
        '
        Me.Comboyear.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Comboyear.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Comboyear.FormattingEnabled = True
        Me.Comboyear.Location = New System.Drawing.Point(864, 145)
        Me.Comboyear.Name = "Comboyear"
        Me.Comboyear.Size = New System.Drawing.Size(132, 23)
        Me.Comboyear.TabIndex = 119
        '
        'Label5
        '
        Me.Label5.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label5.AutoSize = True
        Me.Label5.ForeColor = System.Drawing.SystemColors.ControlDarkDark
        Me.Label5.Location = New System.Drawing.Point(314, 132)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(109, 13)
        Me.Label5.TabIndex = 118
        Me.Label5.Text = "Mode of Procurement"
        '
        'Combomproc
        '
        Me.Combomproc.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Combomproc.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Combomproc.FormattingEnabled = True
        Me.Combomproc.Location = New System.Drawing.Point(317, 145)
        Me.Combomproc.Name = "Combomproc"
        Me.Combomproc.Size = New System.Drawing.Size(240, 23)
        Me.Combomproc.TabIndex = 117
        '
        'Label4
        '
        Me.Label4.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label4.AutoSize = True
        Me.Label4.ForeColor = System.Drawing.SystemColors.ControlDarkDark
        Me.Label4.Location = New System.Drawing.Point(558, 132)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(76, 13)
        Me.Label4.TabIndex = 116
        Me.Label4.Text = "Office/Section"
        '
        'Combooffice
        '
        Me.Combooffice.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Combooffice.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Combooffice.FormattingEnabled = True
        Me.Combooffice.Location = New System.Drawing.Point(561, 145)
        Me.Combooffice.Name = "Combooffice"
        Me.Combooffice.Size = New System.Drawing.Size(299, 23)
        Me.Combooffice.TabIndex = 115
        '
        'Label3
        '
        Me.Label3.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label3.AutoSize = True
        Me.Label3.ForeColor = System.Drawing.SystemColors.ControlDarkDark
        Me.Label3.Location = New System.Drawing.Point(998, 132)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(37, 13)
        Me.Label3.TabIndex = 114
        Me.Label3.Text = "Status"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.ForeColor = System.Drawing.SystemColors.ControlDarkDark
        Me.Label2.Location = New System.Drawing.Point(5, 129)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(41, 13)
        Me.Label2.TabIndex = 113
        Me.Label2.Text = "Search"
        '
        'combostatus
        '
        Me.combostatus.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.combostatus.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.combostatus.FormattingEnabled = True
        Me.combostatus.Location = New System.Drawing.Point(1001, 145)
        Me.combostatus.Name = "combostatus"
        Me.combostatus.Size = New System.Drawing.Size(161, 23)
        Me.combostatus.TabIndex = 112
        '
        'cmdsearch
        '
        Me.cmdsearch.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdsearch.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.cmdsearch.Image = CType(resources.GetObject("cmdsearch.Image"), System.Drawing.Image)
        Me.cmdsearch.Location = New System.Drawing.Point(310, 143)
        Me.cmdsearch.Name = "cmdsearch"
        Me.cmdsearch.Size = New System.Drawing.Size(33, 26)
        Me.cmdsearch.TabIndex = 111
        Me.cmdsearch.UseVisualStyleBackColor = True
        '
        'txtsearch
        '
        Me.txtsearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtsearch.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtsearch.Location = New System.Drawing.Point(6, 143)
        Me.txtsearch.Name = "txtsearch"
        Me.txtsearch.Size = New System.Drawing.Size(305, 26)
        Me.txtsearch.TabIndex = 110
        '
        'lblpage
        '
        Me.lblpage.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lblpage.AutoSize = True
        Me.lblpage.Location = New System.Drawing.Point(169, 645)
        Me.lblpage.Name = "lblpage"
        Me.lblpage.Size = New System.Drawing.Size(0, 13)
        Me.lblpage.TabIndex = 124
        '
        'cmdnext
        '
        Me.cmdnext.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cmdnext.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdnext.Image = CType(resources.GetObject("cmdnext.Image"), System.Drawing.Image)
        Me.cmdnext.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdnext.Location = New System.Drawing.Point(80, 636)
        Me.cmdnext.Name = "cmdnext"
        Me.cmdnext.Size = New System.Drawing.Size(74, 33)
        Me.cmdnext.TabIndex = 123
        Me.cmdnext.Text = "Next"
        Me.cmdnext.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdnext.UseVisualStyleBackColor = True
        '
        'cmdprev
        '
        Me.cmdprev.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cmdprev.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdprev.Image = CType(resources.GetObject("cmdprev.Image"), System.Drawing.Image)
        Me.cmdprev.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdprev.Location = New System.Drawing.Point(6, 636)
        Me.cmdprev.Name = "cmdprev"
        Me.cmdprev.Size = New System.Drawing.Size(74, 33)
        Me.cmdprev.TabIndex = 122
        Me.cmdprev.Text = "Back"
        Me.cmdprev.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdprev.UseVisualStyleBackColor = True
        '
        'frmabstract
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(1264, 675)
        Me.Controls.Add(Me.lblpage)
        Me.Controls.Add(Me.cmdnext)
        Me.Controls.Add(Me.cmdprev)
        Me.Controls.Add(Me.cmdrefresh)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Comboyear)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Combomproc)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Combooffice)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.combostatus)
        Me.Controls.Add(Me.cmdsearch)
        Me.Controls.Add(Me.txtsearch)
        Me.Controls.Add(Me.DataGridView1)
        Me.Controls.Add(Me.FlowLayoutPanel1)
        Me.Controls.Add(Me.Panel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmabstract"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        Me.FlowLayoutPanel1.ResumeLayout(False)
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents FlowLayoutPanel1 As FlowLayoutPanel
    Friend WithEvents cmdnew As Button
    Friend WithEvents cmdopen As Button
    Friend WithEvents cmddelete As Button
    Friend WithEvents Label1 As Label
    Friend WithEvents Panel1 As Panel
    Friend WithEvents DataGridView1 As DataGridView
    Friend WithEvents Label8 As Label
    Friend WithEvents cmdrefresh As Button
    Friend WithEvents Label6 As Label
    Friend WithEvents Comboyear As ComboBox
    Friend WithEvents Label5 As Label
    Friend WithEvents Combomproc As ComboBox
    Friend WithEvents Label4 As Label
    Friend WithEvents Combooffice As ComboBox
    Friend WithEvents Label3 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents combostatus As ComboBox
    Friend WithEvents cmdsearch As Button
    Friend WithEvents txtsearch As TextBox
    Friend WithEvents lblpage As Label
    Friend WithEvents cmdnext As Button
    Friend WithEvents cmdprev As Button
End Class
