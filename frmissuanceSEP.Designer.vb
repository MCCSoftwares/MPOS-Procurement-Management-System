<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmissuancesep
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmissuancesep))
        Me.lblpage = New System.Windows.Forms.Label()
        Me.cmdnext = New System.Windows.Forms.Button()
        Me.cmdprev = New System.Windows.Forms.Button()
        Me.cmdrefresh = New System.Windows.Forms.Button()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Comboyear = New System.Windows.Forms.ComboBox()
        Me.cmdnew = New System.Windows.Forms.Button()
        Me.cmdmanage = New System.Windows.Forms.Button()
        Me.cmddelete = New System.Windows.Forms.Button()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Combosep = New System.Windows.Forms.ComboBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Combooffice = New System.Windows.Forms.ComboBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.cmdsearch = New System.Windows.Forms.Button()
        Me.txtsearch = New System.Windows.Forms.TextBox()
        Me.FlowLayoutPanel1 = New System.Windows.Forms.FlowLayoutPanel()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.DataGridView1 = New System.Windows.Forms.DataGridView()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.FlowLayoutPanel1.SuspendLayout()
        Me.Panel1.SuspendLayout()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lblpage
        '
        Me.lblpage.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lblpage.AutoSize = True
        Me.lblpage.Location = New System.Drawing.Point(165, 837)
        Me.lblpage.Name = "lblpage"
        Me.lblpage.Size = New System.Drawing.Size(0, 13)
        Me.lblpage.TabIndex = 128
        '
        'cmdnext
        '
        Me.cmdnext.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cmdnext.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdnext.Image = CType(resources.GetObject("cmdnext.Image"), System.Drawing.Image)
        Me.cmdnext.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdnext.Location = New System.Drawing.Point(85, 827)
        Me.cmdnext.Name = "cmdnext"
        Me.cmdnext.Size = New System.Drawing.Size(74, 33)
        Me.cmdnext.TabIndex = 127
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
        Me.cmdprev.Location = New System.Drawing.Point(11, 827)
        Me.cmdprev.Name = "cmdprev"
        Me.cmdprev.Size = New System.Drawing.Size(74, 33)
        Me.cmdprev.TabIndex = 126
        Me.cmdprev.Text = "Back"
        Me.cmdprev.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdprev.UseVisualStyleBackColor = True
        '
        'cmdrefresh
        '
        Me.cmdrefresh.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdrefresh.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdrefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cmdrefresh.Image = CType(resources.GetObject("cmdrefresh.Image"), System.Drawing.Image)
        Me.cmdrefresh.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdrefresh.Location = New System.Drawing.Point(1377, 137)
        Me.cmdrefresh.Name = "cmdrefresh"
        Me.cmdrefresh.Size = New System.Drawing.Size(84, 26)
        Me.cmdrefresh.TabIndex = 125
        Me.cmdrefresh.Text = "Refresh"
        Me.cmdrefresh.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdrefresh.UseVisualStyleBackColor = True
        '
        'Label6
        '
        Me.Label6.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label6.AutoSize = True
        Me.Label6.ForeColor = System.Drawing.SystemColors.ControlDarkDark
        Me.Label6.Location = New System.Drawing.Point(1239, 124)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(29, 13)
        Me.Label6.TabIndex = 124
        Me.Label6.Text = "Year"
        '
        'Label1
        '
        Me.Label1.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.Label1.Font = New System.Drawing.Font("Calibri", 20.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.White
        Me.Label1.Location = New System.Drawing.Point(6, 14)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(1455, 33)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "SEMI-EXPENDABLE PROPERTY"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Comboyear
        '
        Me.Comboyear.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Comboyear.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Comboyear.FormattingEnabled = True
        Me.Comboyear.Location = New System.Drawing.Point(1239, 139)
        Me.Comboyear.Name = "Comboyear"
        Me.Comboyear.Size = New System.Drawing.Size(132, 23)
        Me.Comboyear.TabIndex = 123
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
        Me.cmdnew.Size = New System.Drawing.Size(132, 46)
        Me.cmdnew.TabIndex = 7
        Me.cmdnew.Text = "Add Item"
        Me.cmdnew.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdnew.UseVisualStyleBackColor = True
        '
        'cmdmanage
        '
        Me.cmdmanage.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdmanage.FlatAppearance.BorderSize = 0
        Me.cmdmanage.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cmdmanage.Image = CType(resources.GetObject("cmdmanage.Image"), System.Drawing.Image)
        Me.cmdmanage.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdmanage.Location = New System.Drawing.Point(141, 3)
        Me.cmdmanage.Name = "cmdmanage"
        Me.cmdmanage.Size = New System.Drawing.Size(123, 46)
        Me.cmdmanage.TabIndex = 9
        Me.cmdmanage.Text = "Manage"
        Me.cmdmanage.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdmanage.UseVisualStyleBackColor = True
        '
        'cmddelete
        '
        Me.cmddelete.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmddelete.FlatAppearance.BorderSize = 0
        Me.cmddelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cmddelete.Image = CType(resources.GetObject("cmddelete.Image"), System.Drawing.Image)
        Me.cmddelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmddelete.Location = New System.Drawing.Point(270, 3)
        Me.cmddelete.Name = "cmddelete"
        Me.cmddelete.Size = New System.Drawing.Size(90, 46)
        Me.cmddelete.TabIndex = 10
        Me.cmddelete.Text = "Delete"
        Me.cmddelete.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmddelete.UseVisualStyleBackColor = True
        '
        'Button2
        '
        Me.Button2.Cursor = System.Windows.Forms.Cursors.Hand
        Me.Button2.FlatAppearance.BorderSize = 0
        Me.Button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.Button2.Image = CType(resources.GetObject("Button2.Image"), System.Drawing.Image)
        Me.Button2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.Button2.Location = New System.Drawing.Point(366, 3)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(85, 46)
        Me.Button2.TabIndex = 11
        Me.Button2.Text = "Print"
        Me.Button2.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.Button2.UseVisualStyleBackColor = True
        Me.Button2.Visible = False
        '
        'Label5
        '
        Me.Label5.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label5.AutoSize = True
        Me.Label5.ForeColor = System.Drawing.SystemColors.ControlDarkDark
        Me.Label5.Location = New System.Drawing.Point(569, 123)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(67, 13)
        Me.Label5.TabIndex = 122
        Me.Label5.Text = "Type of SEP"
        '
        'Combosep
        '
        Me.Combosep.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Combosep.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Combosep.FormattingEnabled = True
        Me.Combosep.Location = New System.Drawing.Point(572, 139)
        Me.Combosep.Name = "Combosep"
        Me.Combosep.Size = New System.Drawing.Size(340, 23)
        Me.Combosep.TabIndex = 121
        '
        'Label4
        '
        Me.Label4.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label4.AutoSize = True
        Me.Label4.ForeColor = System.Drawing.SystemColors.ControlDarkDark
        Me.Label4.Location = New System.Drawing.Point(913, 124)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(76, 13)
        Me.Label4.TabIndex = 120
        Me.Label4.Text = "Office/Section"
        '
        'Combooffice
        '
        Me.Combooffice.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Combooffice.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Combooffice.FormattingEnabled = True
        Me.Combooffice.Location = New System.Drawing.Point(916, 139)
        Me.Combooffice.Name = "Combooffice"
        Me.Combooffice.Size = New System.Drawing.Size(319, 23)
        Me.Combooffice.TabIndex = 119
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.ForeColor = System.Drawing.SystemColors.ControlDarkDark
        Me.Label2.Location = New System.Drawing.Point(13, 123)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(41, 13)
        Me.Label2.TabIndex = 118
        Me.Label2.Text = "Search"
        '
        'cmdsearch
        '
        Me.cmdsearch.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdsearch.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.cmdsearch.Image = CType(resources.GetObject("cmdsearch.Image"), System.Drawing.Image)
        Me.cmdsearch.Location = New System.Drawing.Point(318, 137)
        Me.cmdsearch.Name = "cmdsearch"
        Me.cmdsearch.Size = New System.Drawing.Size(33, 26)
        Me.cmdsearch.TabIndex = 117
        Me.cmdsearch.UseVisualStyleBackColor = True
        '
        'txtsearch
        '
        Me.txtsearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtsearch.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtsearch.Location = New System.Drawing.Point(14, 137)
        Me.txtsearch.Name = "txtsearch"
        Me.txtsearch.Size = New System.Drawing.Size(305, 26)
        Me.txtsearch.TabIndex = 116
        '
        'FlowLayoutPanel1
        '
        Me.FlowLayoutPanel1.BackColor = System.Drawing.Color.WhiteSmoke
        Me.FlowLayoutPanel1.Controls.Add(Me.cmdnew)
        Me.FlowLayoutPanel1.Controls.Add(Me.cmdmanage)
        Me.FlowLayoutPanel1.Controls.Add(Me.cmddelete)
        Me.FlowLayoutPanel1.Controls.Add(Me.Button2)
        Me.FlowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.FlowLayoutPanel1.Location = New System.Drawing.Point(0, 62)
        Me.FlowLayoutPanel1.Margin = New System.Windows.Forms.Padding(1)
        Me.FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        Me.FlowLayoutPanel1.Size = New System.Drawing.Size(1473, 52)
        Me.FlowLayoutPanel1.TabIndex = 115
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.FromArgb(CType(CType(35, Byte), Integer), CType(CType(40, Byte), Integer), CType(CType(44, Byte), Integer))
        Me.Panel1.Controls.Add(Me.Label1)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(1473, 62)
        Me.Panel1.TabIndex = 114
        '
        'DataGridView1
        '
        Me.DataGridView1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DataGridView1.BackgroundColor = System.Drawing.Color.White
        Me.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView1.Location = New System.Drawing.Point(12, 169)
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.Size = New System.Drawing.Size(1449, 652)
        Me.DataGridView1.TabIndex = 113
        '
        'Label3
        '
        Me.Label3.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label3.BackColor = System.Drawing.Color.Salmon
        Me.Label3.ForeColor = System.Drawing.Color.Black
        Me.Label3.Location = New System.Drawing.Point(1202, 824)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(113, 17)
        Me.Label3.TabIndex = 129
        Me.Label3.Text = "Damage/For Disposal"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label7
        '
        Me.Label7.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label7.BackColor = System.Drawing.Color.Gold
        Me.Label7.ForeColor = System.Drawing.Color.Black
        Me.Label7.Location = New System.Drawing.Point(1321, 824)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(67, 17)
        Me.Label7.TabIndex = 130
        Me.Label7.Text = "Missing"
        Me.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label8
        '
        Me.Label8.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label8.BackColor = System.Drawing.Color.SteelBlue
        Me.Label8.ForeColor = System.Drawing.Color.Black
        Me.Label8.Location = New System.Drawing.Point(1394, 824)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(67, 17)
        Me.Label8.TabIndex = 131
        Me.Label8.Text = "For Repair"
        Me.Label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'frmissuancesep
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(1473, 868)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.lblpage)
        Me.Controls.Add(Me.cmdnext)
        Me.Controls.Add(Me.cmdprev)
        Me.Controls.Add(Me.cmdrefresh)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Comboyear)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Combosep)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Combooffice)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.cmdsearch)
        Me.Controls.Add(Me.txtsearch)
        Me.Controls.Add(Me.FlowLayoutPanel1)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.DataGridView1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "frmissuancesep"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "frmissuanceSEP"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        Me.FlowLayoutPanel1.ResumeLayout(False)
        Me.Panel1.ResumeLayout(False)
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblpage As Label
    Friend WithEvents cmdnext As Button
    Friend WithEvents cmdprev As Button
    Friend WithEvents cmdrefresh As Button
    Friend WithEvents Label6 As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents Comboyear As ComboBox
    Friend WithEvents cmdnew As Button
    Friend WithEvents cmdmanage As Button
    Friend WithEvents cmddelete As Button
    Friend WithEvents Button2 As Button
    Friend WithEvents Label5 As Label
    Friend WithEvents Combosep As ComboBox
    Friend WithEvents Label4 As Label
    Friend WithEvents Combooffice As ComboBox
    Friend WithEvents Label2 As Label
    Friend WithEvents cmdsearch As Button
    Friend WithEvents txtsearch As TextBox
    Friend WithEvents FlowLayoutPanel1 As FlowLayoutPanel
    Friend WithEvents Panel1 As Panel
    Friend WithEvents DataGridView1 As DataGridView
    Friend WithEvents Label3 As Label
    Friend WithEvents Label7 As Label
    Friend WithEvents Label8 As Label
End Class
