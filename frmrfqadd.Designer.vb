<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmrfqadd
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmrfqadd))
        Me.comboprno = New System.Windows.Forms.ComboBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.cmdcontinue = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'comboprno
        '
        Me.comboprno.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.comboprno.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.comboprno.FormattingEnabled = True
        Me.comboprno.Location = New System.Drawing.Point(12, 41)
        Me.comboprno.Name = "comboprno"
        Me.comboprno.Size = New System.Drawing.Size(440, 26)
        Me.comboprno.TabIndex = 0
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(9, 25)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(217, 13)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Search or Select Purchase Request Number"
        '
        'cmdcontinue
        '
        Me.cmdcontinue.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cmdcontinue.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdcontinue.FlatAppearance.BorderSize = 0
        Me.cmdcontinue.Image = CType(resources.GetObject("cmdcontinue.Image"), System.Drawing.Image)
        Me.cmdcontinue.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdcontinue.Location = New System.Drawing.Point(12, 75)
        Me.cmdcontinue.Name = "cmdcontinue"
        Me.cmdcontinue.Size = New System.Drawing.Size(440, 46)
        Me.cmdcontinue.TabIndex = 92
        Me.cmdcontinue.Text = "Continue"
        Me.cmdcontinue.UseVisualStyleBackColor = True
        '
        'frmrfqadd
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(464, 143)
        Me.Controls.Add(Me.cmdcontinue)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.comboprno)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmrfqadd"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Select Purchase Request Number"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents comboprno As ComboBox
    Friend WithEvents Label1 As Label
    Friend WithEvents cmdcontinue As Button
End Class
