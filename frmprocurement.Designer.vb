<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmprocurement
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmprocurement))
        Me.cmdpr = New System.Windows.Forms.Button()
        Me.cmdrfq = New System.Windows.Forms.Button()
        Me.cmdabstract = New System.Windows.Forms.Button()
        Me.cmdpo = New System.Windows.Forms.Button()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.FlowLayoutPanel2 = New System.Windows.Forms.FlowLayoutPanel()
        Me.Panel1.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.FlowLayoutPanel2.SuspendLayout()
        Me.SuspendLayout()
        '
        'cmdpr
        '
        Me.cmdpr.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdpr.FlatAppearance.BorderSize = 0
        Me.cmdpr.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cmdpr.ForeColor = System.Drawing.Color.White
        Me.cmdpr.Image = CType(resources.GetObject("cmdpr.Image"), System.Drawing.Image)
        Me.cmdpr.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdpr.Location = New System.Drawing.Point(3, 3)
        Me.cmdpr.Name = "cmdpr"
        Me.cmdpr.Size = New System.Drawing.Size(148, 48)
        Me.cmdpr.TabIndex = 2
        Me.cmdpr.Text = "Purchase Request"
        Me.cmdpr.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdpr.UseVisualStyleBackColor = True
        '
        'cmdrfq
        '
        Me.cmdrfq.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdrfq.FlatAppearance.BorderSize = 0
        Me.cmdrfq.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cmdrfq.ForeColor = System.Drawing.Color.White
        Me.cmdrfq.Image = CType(resources.GetObject("cmdrfq.Image"), System.Drawing.Image)
        Me.cmdrfq.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdrfq.Location = New System.Drawing.Point(157, 3)
        Me.cmdrfq.Name = "cmdrfq"
        Me.cmdrfq.Size = New System.Drawing.Size(168, 48)
        Me.cmdrfq.TabIndex = 3
        Me.cmdrfq.Text = "Request for Quotation"
        Me.cmdrfq.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdrfq.UseVisualStyleBackColor = True
        '
        'cmdabstract
        '
        Me.cmdabstract.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdabstract.FlatAppearance.BorderSize = 0
        Me.cmdabstract.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cmdabstract.ForeColor = System.Drawing.Color.White
        Me.cmdabstract.Image = CType(resources.GetObject("cmdabstract.Image"), System.Drawing.Image)
        Me.cmdabstract.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdabstract.Location = New System.Drawing.Point(331, 3)
        Me.cmdabstract.Name = "cmdabstract"
        Me.cmdabstract.Size = New System.Drawing.Size(137, 48)
        Me.cmdabstract.TabIndex = 4
        Me.cmdabstract.Text = "Abstract of Bids"
        Me.cmdabstract.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdabstract.UseVisualStyleBackColor = True
        '
        'cmdpo
        '
        Me.cmdpo.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdpo.FlatAppearance.BorderSize = 0
        Me.cmdpo.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cmdpo.ForeColor = System.Drawing.Color.White
        Me.cmdpo.Image = CType(resources.GetObject("cmdpo.Image"), System.Drawing.Image)
        Me.cmdpo.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdpo.Location = New System.Drawing.Point(474, 3)
        Me.cmdpo.Name = "cmdpo"
        Me.cmdpo.Size = New System.Drawing.Size(133, 48)
        Me.cmdpo.TabIndex = 5
        Me.cmdpo.Text = "Purchase Order"
        Me.cmdpo.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdpo.UseVisualStyleBackColor = True
        '
        'Panel1
        '
        Me.Panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.Panel1.BackColor = System.Drawing.Color.White
        Me.Panel1.Controls.Add(Me.PictureBox1)
        Me.Panel1.Controls.Add(Me.Label1)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel1.Location = New System.Drawing.Point(0, 52)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(1359, 745)
        Me.Panel1.TabIndex = 1
        '
        'PictureBox1
        '
        Me.PictureBox1.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.PictureBox1.BackgroundImage = CType(resources.GetObject("PictureBox1.BackgroundImage"), System.Drawing.Image)
        Me.PictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.PictureBox1.Location = New System.Drawing.Point(625, 347)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(43, 40)
        Me.PictureBox1.TabIndex = 1
        Me.PictureBox1.TabStop = False
        '
        'Label1
        '
        Me.Label1.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(558, 390)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(169, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Please select a module to manage"
        '
        'FlowLayoutPanel2
        '
        Me.FlowLayoutPanel2.BackColor = System.Drawing.Color.FromArgb(CType(CType(35, Byte), Integer), CType(CType(40, Byte), Integer), CType(CType(44, Byte), Integer))
        Me.FlowLayoutPanel2.Controls.Add(Me.cmdpr)
        Me.FlowLayoutPanel2.Controls.Add(Me.cmdrfq)
        Me.FlowLayoutPanel2.Controls.Add(Me.cmdabstract)
        Me.FlowLayoutPanel2.Controls.Add(Me.cmdpo)
        Me.FlowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Top
        Me.FlowLayoutPanel2.Location = New System.Drawing.Point(0, 0)
        Me.FlowLayoutPanel2.Name = "FlowLayoutPanel2"
        Me.FlowLayoutPanel2.Size = New System.Drawing.Size(1359, 52)
        Me.FlowLayoutPanel2.TabIndex = 6
        '
        'frmprocurement
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1359, 797)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.FlowLayoutPanel2)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MinimizeBox = False
        Me.Name = "frmprocurement"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Procurement Management"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.FlowLayoutPanel2.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Panel1 As Panel
    Friend WithEvents cmdpr As Button
    Friend WithEvents cmdrfq As Button
    Friend WithEvents cmdabstract As Button
    Friend WithEvents cmdpo As Button
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents Label1 As Label
    Friend WithEvents FlowLayoutPanel2 As FlowLayoutPanel
End Class
