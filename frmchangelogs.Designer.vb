<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmchangelogs
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmchangelogs))
        Me.TXTDETAILS = New System.Windows.Forms.TextBox()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.chkDontShow = New System.Windows.Forms.CheckBox()
        Me.cmdclose = New System.Windows.Forms.Button()
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'TXTDETAILS
        '
        Me.TXTDETAILS.BackColor = System.Drawing.Color.White
        Me.TXTDETAILS.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TXTDETAILS.Location = New System.Drawing.Point(0, 0)
        Me.TXTDETAILS.Multiline = True
        Me.TXTDETAILS.Name = "TXTDETAILS"
        Me.TXTDETAILS.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.TXTDETAILS.Size = New System.Drawing.Size(774, 545)
        Me.TXTDETAILS.TabIndex = 0
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.White
        Me.Panel1.Controls.Add(Me.cmdclose)
        Me.Panel1.Controls.Add(Me.chkDontShow)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel1.Location = New System.Drawing.Point(0, 545)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(774, 68)
        Me.Panel1.TabIndex = 1
        '
        'chkDontShow
        '
        Me.chkDontShow.AutoSize = True
        Me.chkDontShow.Location = New System.Drawing.Point(12, 27)
        Me.chkDontShow.Name = "chkDontShow"
        Me.chkDontShow.Size = New System.Drawing.Size(180, 17)
        Me.chkDontShow.TabIndex = 0
        Me.chkDontShow.Text = "Don’t show again for this version"
        Me.chkDontShow.UseVisualStyleBackColor = True
        '
        'cmdclose
        '
        Me.cmdclose.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdclose.Image = CType(resources.GetObject("cmdclose.Image"), System.Drawing.Image)
        Me.cmdclose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdclose.Location = New System.Drawing.Point(630, 9)
        Me.cmdclose.Name = "cmdclose"
        Me.cmdclose.Size = New System.Drawing.Size(135, 51)
        Me.cmdclose.TabIndex = 1
        Me.cmdclose.Text = "Close"
        Me.cmdclose.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdclose.UseVisualStyleBackColor = True
        '
        'frmchangelogs
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(774, 613)
        Me.Controls.Add(Me.TXTDETAILS)
        Me.Controls.Add(Me.Panel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmchangelogs"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "New version: Change Logs"
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents TXTDETAILS As TextBox
    Friend WithEvents Panel1 As Panel
    Friend WithEvents chkDontShow As CheckBox
    Friend WithEvents cmdclose As Button
End Class
