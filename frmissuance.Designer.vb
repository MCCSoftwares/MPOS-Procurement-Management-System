<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmissuance
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmissuance))
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.FlowLayoutPanel1 = New System.Windows.Forms.FlowLayoutPanel()
        Me.cmdppe = New System.Windows.Forms.Button()
        Me.cmdsep = New System.Windows.Forms.Button()
        Me.FlowLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Panel2
        '
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel2.Location = New System.Drawing.Point(0, 50)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(1546, 800)
        Me.Panel2.TabIndex = 1
        '
        'FlowLayoutPanel1
        '
        Me.FlowLayoutPanel1.BackColor = System.Drawing.Color.Gainsboro
        Me.FlowLayoutPanel1.Controls.Add(Me.cmdppe)
        Me.FlowLayoutPanel1.Controls.Add(Me.cmdsep)
        Me.FlowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.FlowLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        Me.FlowLayoutPanel1.Size = New System.Drawing.Size(1546, 50)
        Me.FlowLayoutPanel1.TabIndex = 2
        '
        'cmdppe
        '
        Me.cmdppe.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdppe.FlatAppearance.BorderSize = 0
        Me.cmdppe.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cmdppe.Image = CType(resources.GetObject("cmdppe.Image"), System.Drawing.Image)
        Me.cmdppe.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdppe.Location = New System.Drawing.Point(3, 3)
        Me.cmdppe.Name = "cmdppe"
        Me.cmdppe.Size = New System.Drawing.Size(241, 46)
        Me.cmdppe.TabIndex = 18
        Me.cmdppe.Text = "Property, Plant and Equipment Tracker" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        Me.cmdppe.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdppe.UseVisualStyleBackColor = True
        '
        'cmdsep
        '
        Me.cmdsep.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdsep.FlatAppearance.BorderSize = 0
        Me.cmdsep.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cmdsep.Image = CType(resources.GetObject("cmdsep.Image"), System.Drawing.Image)
        Me.cmdsep.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdsep.Location = New System.Drawing.Point(250, 3)
        Me.cmdsep.Name = "cmdsep"
        Me.cmdsep.Size = New System.Drawing.Size(216, 46)
        Me.cmdsep.TabIndex = 19
        Me.cmdsep.Text = "Semi-Expendable Property Tracker" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        Me.cmdsep.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdsep.UseVisualStyleBackColor = True
        '
        'frmissuance
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(1546, 850)
        Me.Controls.Add(Me.Panel2)
        Me.Controls.Add(Me.FlowLayoutPanel1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MinimizeBox = False
        Me.Name = "frmissuance"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Issuance Record and Tracker"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        Me.FlowLayoutPanel1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents Panel2 As Panel
    Friend WithEvents FlowLayoutPanel1 As FlowLayoutPanel
    Friend WithEvents cmdppe As Button
    Friend WithEvents cmdsep As Button
End Class
