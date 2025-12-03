<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frminventoryimport
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frminventoryimport))
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.lbltitle = New System.Windows.Forms.Label()
        Me.DataGridView1 = New System.Windows.Forms.DataGridView()
        Me.cmdimport = New System.Windows.Forms.Button()
        Me.lblprid = New System.Windows.Forms.Label()
        Me.lbliarno = New System.Windows.Forms.Label()
        Me.Panel1.SuspendLayout()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.DarkSlateGray
        Me.Panel1.Controls.Add(Me.lbltitle)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(1128, 51)
        Me.Panel1.TabIndex = 35
        '
        'lbltitle
        '
        Me.lbltitle.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.lbltitle.Font = New System.Drawing.Font("Calibri", 20.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbltitle.ForeColor = System.Drawing.Color.White
        Me.lbltitle.Location = New System.Drawing.Point(59, 9)
        Me.lbltitle.Name = "lbltitle"
        Me.lbltitle.Size = New System.Drawing.Size(1007, 33)
        Me.lbltitle.TabIndex = 0
        Me.lbltitle.Text = "IMPORT PROPERTY TO INVENTORY"
        Me.lbltitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
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
        Me.DataGridView1.Location = New System.Drawing.Point(10, 109)
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.ReadOnly = True
        Me.DataGridView1.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        Me.DataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DataGridView1.Size = New System.Drawing.Size(1109, 632)
        Me.DataGridView1.TabIndex = 146
        '
        'cmdimport
        '
        Me.cmdimport.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdimport.FlatAppearance.BorderSize = 0
        Me.cmdimport.Image = CType(resources.GetObject("cmdimport.Image"), System.Drawing.Image)
        Me.cmdimport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdimport.Location = New System.Drawing.Point(946, 57)
        Me.cmdimport.Name = "cmdimport"
        Me.cmdimport.Size = New System.Drawing.Size(173, 46)
        Me.cmdimport.TabIndex = 147
        Me.cmdimport.Text = "Import to Inventory"
        Me.cmdimport.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdimport.UseVisualStyleBackColor = True
        '
        'lblprid
        '
        Me.lblprid.AutoSize = True
        Me.lblprid.Location = New System.Drawing.Point(36, 79)
        Me.lblprid.Name = "lblprid"
        Me.lblprid.Size = New System.Drawing.Size(0, 13)
        Me.lblprid.TabIndex = 148
        Me.lblprid.Visible = False
        '
        'lbliarno
        '
        Me.lbliarno.AutoSize = True
        Me.lbliarno.Location = New System.Drawing.Point(282, 60)
        Me.lbliarno.Name = "lbliarno"
        Me.lbliarno.Size = New System.Drawing.Size(40, 13)
        Me.lbliarno.TabIndex = 149
        Me.lbliarno.Text = "lbliarno"
        Me.lbliarno.Visible = False
        '
        'frminventoryimport
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(1128, 753)
        Me.Controls.Add(Me.lbliarno)
        Me.Controls.Add(Me.lblprid)
        Me.Controls.Add(Me.cmdimport)
        Me.Controls.Add(Me.DataGridView1)
        Me.Controls.Add(Me.Panel1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MinimizeBox = False
        Me.Name = "frminventoryimport"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Import Property to Inventory"
        Me.Panel1.ResumeLayout(False)
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Panel1 As Panel
    Friend WithEvents lbltitle As Label
    Friend WithEvents DataGridView1 As DataGridView
    Friend WithEvents cmdimport As Button
    Friend WithEvents lblprid As Label
    Friend WithEvents lbliarno As Label
End Class
