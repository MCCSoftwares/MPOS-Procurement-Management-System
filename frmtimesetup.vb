Imports System.Diagnostics

Public Class frmtimesetup

    Private Sub frmtimesetup_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Start the timer so we refresh date/time every second
        Timer1.Start()
        EnableDoubleBuffering(Me)
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        ' Update the textboxes with the current date and time
        txtdate.Text = DateTime.Now.ToString("dddd MMMM dd, yyyy")
        txttime.Text = DateTime.Now.ToString("hh:mm:ss tt")
    End Sub

    Private Sub cmdChange_Click(sender As Object, e As EventArgs) Handles cmdchange.Click
        ' Open the Windows Date & Time settings
        Process.Start("control.exe", "timedate.cpl")
    End Sub

    Private Sub cmdConfirm_Click(sender As Object, e As EventArgs) Handles cmdconfirm.Click
        Timer1.Stop()
        Me.Dispose()
        frmmain.Show()
        'frmmain.ShowDialog()
    End Sub

    Private Sub txttime_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txttime.KeyPress
        e.Handled = True
    End Sub

    Private Sub txtdate_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtdate.KeyPress
        e.Handled = True
    End Sub
End Class