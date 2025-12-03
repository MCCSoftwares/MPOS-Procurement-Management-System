<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frminventoryfind
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frminventoryfind))
        Me.combodesc = New System.Windows.Forms.ComboBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.cmdadd = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'combodesc
        '
        Me.combodesc.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.combodesc.FormattingEnabled = True
        Me.combodesc.Location = New System.Drawing.Point(29, 43)
        Me.combodesc.MaxLength = 3500
        Me.combodesc.Name = "combodesc"
        Me.combodesc.Size = New System.Drawing.Size(408, 28)
        Me.combodesc.TabIndex = 0
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(26, 27)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(60, 13)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Description"
        '
        'cmdadd
        '
        Me.cmdadd.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cmdadd.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdadd.FlatAppearance.BorderSize = 0
        Me.cmdadd.Image = CType(resources.GetObject("cmdadd.Image"), System.Drawing.Image)
        Me.cmdadd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdadd.Location = New System.Drawing.Point(29, 80)
        Me.cmdadd.Name = "cmdadd"
        Me.cmdadd.Size = New System.Drawing.Size(408, 46)
        Me.cmdadd.TabIndex = 80
        Me.cmdadd.Text = "Add to Inventory"
        Me.cmdadd.UseVisualStyleBackColor = True
        '
        'frminventoryfind
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(482, 158)
        Me.Controls.Add(Me.cmdadd)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.combodesc)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frminventoryfind"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Find Item/Property"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents combodesc As ComboBox
    Friend WithEvents Label1 As Label
    Friend WithEvents cmdadd As Button
End Class
