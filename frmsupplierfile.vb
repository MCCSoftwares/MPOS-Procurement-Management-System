' frmsupplierfile.vb  (Add/Edit supplier record)
Imports System.Data
Imports System.Data.SqlClient
Imports System.Windows.Forms

Public Class frmsupplierfile

    Public Property ModeEdit As Boolean = False
    Public Property SupplierID As Integer = 0

    Private Sub frmsupplierfile_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Styling touch (optional)
        cmdsave.Cursor = Cursors.Hand
        cmdclose.Cursor = Cursors.Hand
        Me.AcceptButton = cmdsave
        Me.CancelButton = cmdclose

        If ModeEdit AndAlso SupplierID > 0 Then
            ' If you want to re-load from DB (more authoritative than values passed-in),
            ' uncomment this block. Otherwise we rely on the values prefilled by the parent.
            '
            'Try
            '    Using conn As New SqlConnection(frmmain.txtdb.Text)
            '        conn.Open()
            '        Using cmd As New SqlCommand("SELECT SUPPLIER, ADDRESS, TIN_NO, CONTACT_NO FROM dbo.TBL_SUPPLIERS WHERE ID=@id", conn)
            '            cmd.Parameters.AddWithValue("@id", SupplierID)
            '            Using rdr = cmd.ExecuteReader()
            '                If rdr.Read() Then
            '                    txtsupplier.Text = rdr("SUPPLIER").ToString()
            '                    txtaddress.Text = rdr("ADDRESS").ToString()
            '                    txttin.Text = rdr("TIN_NO").ToString()
            '                    txtcontact.Text = rdr("CONTACT_NO").ToString()
            '                End If
            '            End Using
            '        End Using
            '    End Using
            'Catch ex As Exception
            '    MessageBox.Show("Load failed: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            'End Try
        End If
    End Sub

    Private Sub cmdsave_Click(sender As Object, e As EventArgs) Handles cmdsave.Click
        ' Basic validation
        Dim sName As String = txtsupplier.Text.Trim()
        Dim sAddr As String = txtaddress.Text.Trim()
        Dim sTin As String = txttin.Text.Trim()
        Dim sContact As String = txtcontact.Text.Trim()

        If sName = "" Then
            MessageBox.Show("Supplier name is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtsupplier.Focus()
            Return
        End If

        Try
            Using conn As New SqlConnection(frmmain.txtdb.Text)
                conn.Open()

                If ModeEdit AndAlso SupplierID > 0 Then
                    ' UPDATE
                    Using cmd As New SqlCommand("
UPDATE dbo.TBL_SUPPLIERS
SET SUPPLIER=@s, ADDRESS=@a, TIN_NO=@t, CONTACT_NO=@c
WHERE ID=@id;", conn)
                        cmd.Parameters.AddWithValue("@s", sName)
                        cmd.Parameters.AddWithValue("@a", sAddr)
                        cmd.Parameters.AddWithValue("@t", sTin)
                        cmd.Parameters.AddWithValue("@c", sContact)
                        cmd.Parameters.AddWithValue("@id", SupplierID)
                        cmd.ExecuteNonQuery()
                    End Using
                Else
                    ' INSERT (defaults for boolean text fields)
                    Using cmd As New SqlCommand("
INSERT INTO dbo.TBL_SUPPLIERS (SUPPLIER, ADDRESS, TIN_NO, CONTACT_NO, BPERMIT, PHILGEPS, ITR, UPLOADS)
VALUES (@s, @a, @t, @c, 'FALSE', 'FALSE', 'FALSE', NULL);
SELECT CAST(SCOPE_IDENTITY() AS INT);", conn)
                        cmd.Parameters.AddWithValue("@s", sName)
                        cmd.Parameters.AddWithValue("@a", sAddr)
                        cmd.Parameters.AddWithValue("@t", sTin)
                        cmd.Parameters.AddWithValue("@c", sContact)
                        Dim newId As Integer = CInt(cmd.ExecuteScalar())
                        SupplierID = newId
                    End Using
                End If
            End Using

            Me.DialogResult = DialogResult.OK
            Me.Close()

        Catch ex As Exception
            MessageBox.Show("Save failed: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub cmdclose_Click(sender As Object, e As EventArgs) Handles cmdclose.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub

End Class
