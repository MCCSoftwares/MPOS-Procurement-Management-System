'─────────────────────────────────────────────
' Notifications.vb (in your project)
'─────────────────────────────────────────────
Imports System.Data.SqlClient

Module NotificationHelper

    ''' <summary>
    ''' Inserts a new row into TBL_NOTIF with:
    '''  • NOT_DT = now
    '''  • NOT_MESSAGE = your custom text
    '''  • NOT_EXPIREDATE = now + 7 days
    ''' </summary>
    ''' <param name="message">The text to store in NOT_MESSAGE</param>
    Public Sub AddNotification(message As String)
        Try
            Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
                conn.Open()
                Const sql As String = "
INSERT INTO TBL_NOTIF
   (NOT_DT, NOT_MESSAGE, NOT_EXPIREDATE)
VALUES
   (@dt,   @msg,         @exp);"

                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@dt", DateTime.Now)
                    cmd.Parameters.AddWithValue("@msg", message.Trim())
                    cmd.Parameters.AddWithValue("@exp", DateTime.Now.AddDays(7))
                    cmd.ExecuteNonQuery()
                End Using
            End Using

            My.Forms.frmmain.LoadNotifications()

        Catch ex As Exception
            ' You can log this or show a message, but don't raise unhandled
            Debug.WriteLine($"Notification error: {ex.Message}")
        End Try
    End Sub

End Module
