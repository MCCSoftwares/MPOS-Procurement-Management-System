Imports System.IO
Imports System.Data.SqlClient
Public Class frmconsetup
    Private Sub Frmconsetup_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim configPath As String = Application.StartupPath & "\server.config"

        If File.Exists(configPath) Then
            Dim lines() As String = File.ReadAllLines(configPath)

            For Each line In lines
                If line.StartsWith("ServerIP=") Then
                    txtipadd.Text = line.Replace("ServerIP=", "")
                ElseIf line.StartsWith("Port=") Then
                    txtipport.Text = line.Replace("Port=", "")
                ElseIf line.StartsWith("Username=") Then
                    txtsqluser.Text = line.Replace("Username=", "")
                ElseIf line.StartsWith("Password=") Then
                    txtsqlpassword.Text = line.Replace("Password=", "")
                End If
            Next
        End If
    End Sub


    Private Sub SaveConfig()
        Dim configPath As String = Application.StartupPath & "\server.config"

        Using sw As New StreamWriter(configPath, False)
            sw.WriteLine("ServerIP=" & txtipadd.Text)
            sw.WriteLine("Port=" & txtipport.Text)
            sw.WriteLine("Username=" & txtsqluser.Text)
            sw.WriteLine("Password=" & txtsqlpassword.Text)

        End Using

        MessageBox.Show("Configuration saved.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub Cmdclose_Click(sender As Object, e As EventArgs) Handles cmdclose.Click
        Me.Dispose()
    End Sub

    Private Sub Cmdsave_Click(sender As Object, e As EventArgs) Handles cmdsave.Click
        SaveConfig()
        MsgBox("System will restart to update the network changes.", vbInformation, "Message")
        Application.Exit()
    End Sub

    Private Sub Cmdcheck_Click(sender As Object, e As EventArgs) Handles cmdcheck.Click
        Dim connectionString As String = "Server=" & txtipadd.Text & "," & txtipport.Text & ";Database=MPOSDB;User Id=" & txtsqluser.Text & ";Password=" & txtsqlpassword.Text & ";"
        Dim conn As New SqlConnection(connectionString)

        cmdcheck.Text = "Connecting..."
        cmdcheck.ForeColor = Color.Black

        Try
            conn.Open()
            MessageBox.Show("Connection successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            cmdcheck.Text = "Connected"
            cmdcheck.ForeColor = Color.Green
            conn.Close()
            Catch ex As Exception
            MessageBox.Show("Connection failed: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            cmdcheck.Text = "Disconnected"
            cmdcheck.ForeColor = Color.Red

        End Try
    End Sub
End Class