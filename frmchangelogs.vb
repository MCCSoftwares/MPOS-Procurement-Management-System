Public Class frmchangelogs
    Public Property CurrentVersion As String
    Public Property BodyText As String
    Public Property DontShow As Boolean = False
    Private Sub Frmchangelogs_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Text = "What's New"
        TXTDETAILS.Text = BodyText
    End Sub

    Private Sub TXTDETAILS_TextChanged(sender As Object, e As EventArgs) Handles TXTDETAILS.TextChanged
        DontShow = chkDontShow.Checked
        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub
End Class