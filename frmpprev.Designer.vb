<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmpprev
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmpprev))
        Me.ReportViewer1 = New Microsoft.Reporting.WinForms.ReportViewer()
        Me.panelsubmit = New System.Windows.Forms.Panel()
        Me.lblprno = New System.Windows.Forms.Label()
        Me.lblprid = New System.Windows.Forms.Label()
        Me.lblapprovers = New System.Windows.Forms.Label()
        Me.lbltransaction = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.cmdsubmit = New System.Windows.Forms.Button()
        Me.panelsubmit.SuspendLayout()
        Me.SuspendLayout()
        '
        'ReportViewer1
        '
        Me.ReportViewer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ReportViewer1.Location = New System.Drawing.Point(0, 0)
        Me.ReportViewer1.Name = "ReportViewer1"
        Me.ReportViewer1.ServerReport.BearerToken = Nothing
        Me.ReportViewer1.Size = New System.Drawing.Size(1045, 669)
        Me.ReportViewer1.TabIndex = 0
        '
        'panelsubmit
        '
        Me.panelsubmit.BackColor = System.Drawing.Color.DarkGreen
        Me.panelsubmit.Controls.Add(Me.lblprno)
        Me.panelsubmit.Controls.Add(Me.lblprid)
        Me.panelsubmit.Controls.Add(Me.lblapprovers)
        Me.panelsubmit.Controls.Add(Me.lbltransaction)
        Me.panelsubmit.Controls.Add(Me.Label2)
        Me.panelsubmit.Controls.Add(Me.Label1)
        Me.panelsubmit.Controls.Add(Me.cmdsubmit)
        Me.panelsubmit.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.panelsubmit.Location = New System.Drawing.Point(0, 669)
        Me.panelsubmit.Name = "panelsubmit"
        Me.panelsubmit.Size = New System.Drawing.Size(1045, 51)
        Me.panelsubmit.TabIndex = 1
        '
        'lblprno
        '
        Me.lblprno.AutoSize = True
        Me.lblprno.Location = New System.Drawing.Point(519, 20)
        Me.lblprno.Name = "lblprno"
        Me.lblprno.Size = New System.Drawing.Size(0, 13)
        Me.lblprno.TabIndex = 74
        Me.lblprno.Visible = False
        '
        'lblprid
        '
        Me.lblprid.AutoSize = True
        Me.lblprid.Location = New System.Drawing.Point(472, 16)
        Me.lblprid.Name = "lblprid"
        Me.lblprid.Size = New System.Drawing.Size(0, 13)
        Me.lblprid.TabIndex = 73
        Me.lblprid.Visible = False
        '
        'lblapprovers
        '
        Me.lblapprovers.AutoSize = True
        Me.lblapprovers.Location = New System.Drawing.Point(612, 25)
        Me.lblapprovers.Name = "lblapprovers"
        Me.lblapprovers.Size = New System.Drawing.Size(0, 13)
        Me.lblapprovers.TabIndex = 72
        Me.lblapprovers.Visible = False
        '
        'lbltransaction
        '
        Me.lbltransaction.AutoSize = True
        Me.lbltransaction.Location = New System.Drawing.Point(667, 8)
        Me.lbltransaction.Name = "lbltransaction"
        Me.lbltransaction.Size = New System.Drawing.Size(69, 13)
        Me.lbltransaction.TabIndex = 71
        Me.lbltransaction.Text = "lbltransaction"
        Me.lbltransaction.Visible = False
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.ForeColor = System.Drawing.Color.White
        Me.Label2.Location = New System.Drawing.Point(9, 29)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(204, 13)
        Me.Label2.TabIndex = 70
        Me.Label2.Text = "Click on the button to submit for Approval."
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Calibri", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.White
        Me.Label1.Location = New System.Drawing.Point(7, 5)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(149, 23)
        Me.Label1.TabIndex = 69
        Me.Label1.Text = "Ready to Submit?"
        '
        'cmdsubmit
        '
        Me.cmdsubmit.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdsubmit.BackColor = System.Drawing.Color.White
        Me.cmdsubmit.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdsubmit.FlatAppearance.BorderSize = 0
        Me.cmdsubmit.Image = CType(resources.GetObject("cmdsubmit.Image"), System.Drawing.Image)
        Me.cmdsubmit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdsubmit.Location = New System.Drawing.Point(893, 2)
        Me.cmdsubmit.Name = "cmdsubmit"
        Me.cmdsubmit.Size = New System.Drawing.Size(149, 46)
        Me.cmdsubmit.TabIndex = 68
        Me.cmdsubmit.Text = "Submit for Approval"
        Me.cmdsubmit.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdsubmit.UseVisualStyleBackColor = False
        '
        'frmpprev
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1045, 720)
        Me.Controls.Add(Me.ReportViewer1)
        Me.Controls.Add(Me.panelsubmit)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmpprev"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Print Preview"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        Me.panelsubmit.ResumeLayout(False)
        Me.panelsubmit.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents ReportViewer1 As Microsoft.Reporting.WinForms.ReportViewer
    Friend WithEvents panelsubmit As Panel
    Friend WithEvents cmdsubmit As Button
    Friend WithEvents Label2 As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents lbltransaction As Label
    Friend WithEvents lblapprovers As Label
    Friend WithEvents lblprid As Label
    Friend WithEvents lblprno As Label
End Class
