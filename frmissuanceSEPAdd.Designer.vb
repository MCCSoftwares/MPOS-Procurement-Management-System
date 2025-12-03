<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmissuanceSEPAdd
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmissuanceSEPAdd))
        Me.comboamount = New System.Windows.Forms.ComboBox()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.combodesc = New System.Windows.Forms.ComboBox()
        Me.txtunit = New System.Windows.Forms.TextBox()
        Me.lbliid = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.cmdsave = New System.Windows.Forms.Button()
        Me.cmdclose = New System.Windows.Forms.Button()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.txtpnumber = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.txtqty = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.txteulife = New System.Windows.Forms.TextBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.txttcost = New System.Windows.Forms.TextBox()
        Me.Panel1.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'comboamount
        '
        Me.comboamount.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.comboamount.FormattingEnabled = True
        Me.comboamount.Location = New System.Drawing.Point(11, 271)
        Me.comboamount.Name = "comboamount"
        Me.comboamount.Size = New System.Drawing.Size(392, 28)
        Me.comboamount.TabIndex = 104
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.FromArgb(CType(CType(35, Byte), Integer), CType(CType(40, Byte), Integer), CType(CType(44, Byte), Integer))
        Me.Panel1.Controls.Add(Me.Label8)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(693, 51)
        Me.Panel1.TabIndex = 114
        '
        'Label8
        '
        Me.Label8.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("Calibri", 20.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label8.ForeColor = System.Drawing.Color.White
        Me.Label8.Location = New System.Drawing.Point(229, 9)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(227, 33)
        Me.Label8.TabIndex = 0
        Me.Label8.Text = "ITEM DESCRIPTION"
        '
        'PictureBox1
        '
        Me.PictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.PictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.PictureBox1.Location = New System.Drawing.Point(417, 81)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(264, 208)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.PictureBox1.TabIndex = 113
        Me.PictureBox1.TabStop = False
        '
        'combodesc
        '
        Me.combodesc.FormattingEnabled = True
        Me.combodesc.Location = New System.Drawing.Point(11, 92)
        Me.combodesc.Name = "combodesc"
        Me.combodesc.Size = New System.Drawing.Size(392, 21)
        Me.combodesc.TabIndex = 99
        '
        'txtunit
        '
        Me.txtunit.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtunit.Location = New System.Drawing.Point(11, 134)
        Me.txtunit.Name = "txtunit"
        Me.txtunit.Size = New System.Drawing.Size(392, 20)
        Me.txtunit.TabIndex = 100
        '
        'lbliid
        '
        Me.lbliid.AutoSize = True
        Me.lbliid.Location = New System.Drawing.Point(495, 65)
        Me.lbliid.Name = "lbliid"
        Me.lbliid.Size = New System.Drawing.Size(0, 13)
        Me.lbliid.TabIndex = 112
        Me.lbliid.Visible = False
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(8, 255)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(283, 13)
        Me.Label5.TabIndex = 111
        Me.Label5.Text = "Amount (Select from filtered amount from Purchased Order)"
        '
        'cmdsave
        '
        Me.cmdsave.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cmdsave.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdsave.FlatAppearance.BorderSize = 0
        Me.cmdsave.Image = CType(resources.GetObject("cmdsave.Image"), System.Drawing.Image)
        Me.cmdsave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdsave.Location = New System.Drawing.Point(10, 413)
        Me.cmdsave.Name = "cmdsave"
        Me.cmdsave.Size = New System.Drawing.Size(195, 46)
        Me.cmdsave.TabIndex = 105
        Me.cmdsave.Text = "Save"
        Me.cmdsave.UseVisualStyleBackColor = True
        '
        'cmdclose
        '
        Me.cmdclose.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cmdclose.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdclose.FlatAppearance.BorderSize = 0
        Me.cmdclose.Image = CType(resources.GetObject("cmdclose.Image"), System.Drawing.Image)
        Me.cmdclose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdclose.Location = New System.Drawing.Point(209, 413)
        Me.cmdclose.Name = "cmdclose"
        Me.cmdclose.Size = New System.Drawing.Size(195, 46)
        Me.cmdclose.TabIndex = 106
        Me.cmdclose.Text = "Close"
        Me.cmdclose.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(8, 209)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(86, 13)
        Me.Label2.TabIndex = 110
        Me.Label2.Text = "Property Number"
        '
        'txtpnumber
        '
        Me.txtpnumber.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtpnumber.Location = New System.Drawing.Point(11, 225)
        Me.txtpnumber.Name = "txtpnumber"
        Me.txtpnumber.Size = New System.Drawing.Size(392, 20)
        Me.txtpnumber.TabIndex = 102
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(8, 157)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(46, 13)
        Me.Label1.TabIndex = 109
        Me.Label1.Text = "Quantity"
        '
        'txtqty
        '
        Me.txtqty.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtqty.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtqty.Location = New System.Drawing.Point(11, 173)
        Me.txtqty.Name = "txtqty"
        Me.txtqty.Size = New System.Drawing.Size(392, 29)
        Me.txtqty.TabIndex = 101
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(8, 117)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(26, 13)
        Me.Label4.TabIndex = 108
        Me.Label4.Text = "Unit"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(8, 76)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(83, 13)
        Me.Label3.TabIndex = 107
        Me.Label3.Text = "Item Description"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(9, 357)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(107, 13)
        Me.Label7.TabIndex = 117
        Me.Label7.Text = "Estimated Usage Life"
        '
        'txteulife
        '
        Me.txteulife.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txteulife.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txteulife.Location = New System.Drawing.Point(12, 373)
        Me.txteulife.Name = "txteulife"
        Me.txteulife.Size = New System.Drawing.Size(392, 29)
        Me.txteulife.TabIndex = 116
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(9, 306)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(55, 13)
        Me.Label6.TabIndex = 119
        Me.Label6.Text = "Total Cost"
        '
        'txttcost
        '
        Me.txttcost.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txttcost.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txttcost.Location = New System.Drawing.Point(12, 322)
        Me.txttcost.Name = "txttcost"
        Me.txttcost.Size = New System.Drawing.Size(392, 29)
        Me.txttcost.TabIndex = 118
        '
        'frmissuanceSEPAdd
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(693, 479)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.txttcost)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.txteulife)
        Me.Controls.Add(Me.comboamount)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.combodesc)
        Me.Controls.Add(Me.txtunit)
        Me.Controls.Add(Me.lbliid)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.cmdsave)
        Me.Controls.Add(Me.cmdclose)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.txtpnumber)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.txtqty)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label3)
        Me.Name = "frmissuanceSEPAdd"
        Me.Text = "Item Description"
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents comboamount As ComboBox
    Friend WithEvents Panel1 As Panel
    Friend WithEvents Label8 As Label
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents combodesc As ComboBox
    Friend WithEvents txtunit As TextBox
    Friend WithEvents lbliid As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents cmdsave As Button
    Friend WithEvents cmdclose As Button
    Friend WithEvents Label2 As Label
    Friend WithEvents txtpnumber As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents txtqty As TextBox
    Friend WithEvents Label4 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents Label7 As Label
    Friend WithEvents txteulife As TextBox
    Friend WithEvents Label6 As Label
    Friend WithEvents txttcost As TextBox
End Class
