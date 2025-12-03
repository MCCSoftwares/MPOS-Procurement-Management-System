Imports System.Data.SqlClient
Public Class frmpraddunit
    Private Sub cmdsave_Click(sender As Object, e As EventArgs) Handles cmdsave.Click
        Dim newUnit As String = txtunit.Text.Trim()
        If String.IsNullOrEmpty(newUnit) Then
            MessageBox.Show("Please enter a unit name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtunit.Focus()
            Return
        End If

        Try
            Using conn As New SqlConnection(frmmain.txtdb.Text)
                conn.Open()

                ' 1) Check for existing unit
                Using checkCmd As New SqlCommand("SELECT COUNT(*) FROM TBL_UNITS WHERE UNITS = @Unit", conn)
                    checkCmd.Parameters.AddWithValue("@Unit", newUnit)
                    Dim existsCount As Integer = Convert.ToInt32(checkCmd.ExecuteScalar())
                    If existsCount > 0 Then
                        MessageBox.Show($"The unit '{newUnit}' already exists.", "Duplicate", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        Return
                    End If
                End Using

                ' 2) Insert new unit
                Using insertCmd As New SqlCommand("INSERT INTO TBL_UNITS (UNITS) VALUES (@Unit)", conn)
                    insertCmd.Parameters.AddWithValue("@Unit", newUnit)
                    insertCmd.ExecuteNonQuery()
                End Using

                conn.Close()
            End Using

            MessageBox.Show($"Unit '{newUnit}' added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            txtunit.Clear()
            txtunit.Focus()

            ' 3) Refresh the unit list in frmpradd
            If Application.OpenForms().OfType(Of frmpradd)().Any() Then
                Dim addForm = CType(Application.OpenForms("frmpradd"), frmpradd)
                ' addForm.LoadUnits()
            End If

        Catch ex As Exception
            MessageBox.Show("Error adding unit: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

End Class