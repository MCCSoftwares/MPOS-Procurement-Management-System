Imports Microsoft.Win32

Public Class DateFormatHelper

    ''' <summary>
    ''' Updates the system short date format to MM/dd/yyyy
    ''' </summary>
    Public Shared Sub SetDateFormat()
        Try
            ' Registry path for user international settings
            Using regKey As RegistryKey = Registry.CurrentUser.OpenSubKey("Control Panel\International", True)
                If regKey IsNot Nothing Then
                    regKey.SetValue("sShortDate", "MM/dd/yyyy")
                    regKey.SetValue("sDate", "/")
                End If
            End Using

            ' Notify system that settings changed
            SendMessageTimeout(New IntPtr(&HFFFF), &H1A, IntPtr.Zero, "International", &H2, 1000, IntPtr.Zero)

        Catch ex As Exception
            MessageBox.Show("Error updating date format: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' API to broadcast change
    <Runtime.InteropServices.DllImport("user32.dll", CharSet:=Runtime.InteropServices.CharSet.Auto)>
    Private Shared Function SendMessageTimeout(ByVal hWnd As IntPtr, ByVal Msg As Integer,
                                               ByVal wParam As IntPtr, ByVal lParam As String,
                                               ByVal fuFlags As Integer, ByVal uTimeout As Integer,
                                               ByRef lpdwResult As IntPtr) As IntPtr
    End Function

End Class

