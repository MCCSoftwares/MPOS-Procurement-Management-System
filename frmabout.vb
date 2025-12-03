Imports System.Reflection
Public Class frmabout
    Private Sub Frmabout_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim ver As Version = Assembly.GetExecutingAssembly().GetName().Version
        lblversion.Text = $"{ver.Major}.{ver.Minor}.{ver.Build}.{ver.Revision}"
    End Sub
End Class