<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmtimesetup
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
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmtimesetup))
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.txtdate = New System.Windows.Forms.TextBox()
        Me.txttime = New System.Windows.Forms.TextBox()
        Me.cmdconfirm = New System.Windows.Forms.Button()
        Me.cmdchange = New System.Windows.Forms.Button()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.SuspendLayout()
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.BackColor = System.Drawing.Color.Transparent
        Me.Label2.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.Color.White
        Me.Label2.Location = New System.Drawing.Point(545, 196)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(52, 15)
        Me.Label2.TabIndex = 7
        Me.Label2.Text = "Today is"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.BackColor = System.Drawing.Color.Transparent
        Me.Label1.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.White
        Me.Label1.Location = New System.Drawing.Point(545, 135)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(79, 15)
        Me.Label1.TabIndex = 6
        Me.Label1.Text = "Current Time"
        '
        'txtdate
        '
        Me.txtdate.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtdate.Location = New System.Drawing.Point(548, 212)
        Me.txtdate.Name = "txtdate"
        Me.txtdate.Size = New System.Drawing.Size(317, 26)
        Me.txtdate.TabIndex = 5
        Me.txtdate.TabStop = False
        Me.txtdate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'txttime
        '
        Me.txttime.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.txttime.Font = New System.Drawing.Font("Microsoft Sans Serif", 27.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txttime.Location = New System.Drawing.Point(548, 151)
        Me.txttime.Name = "txttime"
        Me.txttime.Size = New System.Drawing.Size(317, 42)
        Me.txttime.TabIndex = 4
        Me.txttime.TabStop = False
        Me.txttime.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'cmdconfirm
        '
        Me.cmdconfirm.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdconfirm.Image = CType(resources.GetObject("cmdconfirm.Image"), System.Drawing.Image)
        Me.cmdconfirm.Location = New System.Drawing.Point(700, 251)
        Me.cmdconfirm.Name = "cmdconfirm"
        Me.cmdconfirm.Size = New System.Drawing.Size(165, 38)
        Me.cmdconfirm.TabIndex = 1
        Me.cmdconfirm.UseVisualStyleBackColor = True
        '
        'cmdchange
        '
        Me.cmdchange.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdchange.Location = New System.Drawing.Point(548, 251)
        Me.cmdchange.Name = "cmdchange"
        Me.cmdchange.Size = New System.Drawing.Size(146, 38)
        Me.cmdchange.TabIndex = 0
        Me.cmdchange.Text = "Change Time/Date"
        Me.cmdchange.UseVisualStyleBackColor = True
        '
        'Timer1
        '
        '
        'frmtimesetup
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackgroundImage = CType(resources.GetObject("$this.BackgroundImage"), System.Drawing.Image)
        Me.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.ClientSize = New System.Drawing.Size(891, 446)
        Me.Controls.Add(Me.cmdchange)
        Me.Controls.Add(Me.cmdconfirm)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.txtdate)
        Me.Controls.Add(Me.txttime)
        Me.DoubleBuffered = True
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmtimesetup"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Verify Time and Date Setup"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Label2 As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents txtdate As TextBox
    Friend WithEvents txttime As TextBox
    Friend WithEvents cmdconfirm As Button
    Friend WithEvents cmdchange As Button
    Friend WithEvents Timer1 As Timer
End Class
