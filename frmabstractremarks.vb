Imports System.Data.SqlClient

Public Class frmabstractremarks

    ''' <summary>
    ''' The ID of the TBL_ABSTRACTITEMS row we're editing.
    ''' </summary>
    Public Property ItemID As Integer

    Private Sub frmabstractremarks_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Load existing remark (if any)
        If ItemID <= 0 Then Return

        Try
            Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
                conn.Open()
                Const sql As String = "
SELECT REMARKS
  FROM TBL_ABSTRACTITEMS
 WHERE ID = @id;"
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@id", ItemID)
                    Dim existing = TryCast(cmd.ExecuteScalar(), String)
                    txtremarks.Text = If(existing, "")
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error loading remarks: " & ex.Message,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub cmdsave_Click(sender As Object, e As EventArgs) Handles cmdsave.Click
        Try
            Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
                conn.Open()
                Const sql As String = "
UPDATE TBL_ABSTRACTITEMS
   SET REMARKS = @rm
 WHERE ID      = @id;"
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@rm", txtremarks.Text.Trim())
                    cmd.Parameters.AddWithValue("@id", ItemID)
                    cmd.ExecuteNonQuery()
                End Using
            End Using

            ' Notify owner form to refresh its list
            If TypeOf Me.Owner Is frmabstractfile Then
                DirectCast(Me.Owner, frmabstractfile).LoadAbstract()
            End If

            Me.Close()
        Catch ex As Exception
            MessageBox.Show("Error saving remarks: " & ex.Message,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub Cmdclose_Click(sender As Object, e As EventArgs) Handles cmdclose.Click
        Me.Dispose()

    End Sub
End Class