Imports System.Data.SqlClient
Imports System.Globalization
Public Class frminventoryfile
    Private Sub Frminventoryfile_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            If String.Equals(lbltrans.Text.Trim(), "Edit", StringComparison.OrdinalIgnoreCase) Then
                combotype.Enabled = False
                txtdesc.ReadOnly = True
                txtatitle.ReadOnly = True
                txtacode.ReadOnly = True
                txtipno.ReadOnly = True
                txtunit.ReadOnly = True
                txtstatus.ReadOnly = True

                txtqty.ReadOnly = False
                txtremarks.ReadOnly = False
            End If

            UpdateStockStatus()
        Catch ex As Exception
            MessageBox.Show("Load Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' --- replace your UpdateStockStatus + Load handlers with these ---

    Private Function ParseQty(s As String) As Decimal
        Dim q As Decimal = 0D
        If Not Decimal.TryParse(If(s, "").Trim(), NumberStyles.Number, CultureInfo.CurrentCulture, q) Then
            ' Fallback: try invariant (covers "1,000.00" vs locale)
            Decimal.TryParse(If(s, "").Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, q)
        End If
        Return q
    End Function

    Private Sub UpdateStockStatus()
        Dim q As Decimal = ParseQty(txtqty.Text)
        txtstatus.Text = If(q > 0D, "In Stock", "Out of Stock")
    End Sub

    Private Sub cmdsave_Click(sender As Object, e As EventArgs) Handles cmdsave.Click
        Try
            ' ------------------------------
            ' SAVE NEW RECORD
            ' ------------------------------

            If String.Equals(cmdsave.Text, "Save", StringComparison.OrdinalIgnoreCase) Then
                If combotype.Text = "" Then
                    MsgBox("Please select a type to save an item or property.", vbExclamation, "Message")
                Else

                    ' Basic validation
                    If String.IsNullOrWhiteSpace(txtdesc.Text) Then
                        MessageBox.Show("Description is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txtdesc.Focus()
                        Exit Sub
                    End If

                    Dim qty As Integer = 0
                    Integer.TryParse(txtqty.Text.Trim(), qty)
                    Dim remarks As String = txtremarks.Text.Trim()
                    Dim status As String = If(qty > 0, "In Stock", "Out of Stock")

                    Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
                        conn.Open()

                        ' Check for duplicates
                        Using cmdChk As New SqlCommand("SELECT COUNT(1) FROM TBL_INVENTORY WHERE DESCRIPTIONS = @desc;", conn)
                            cmdChk.Parameters.Add("@desc", SqlDbType.NVarChar, 500).Value = txtdesc.Text.Trim()
                            If CInt(cmdChk.ExecuteScalar()) > 0 Then
                                MessageBox.Show("This description already exists in inventory.", "Duplicate Found", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                Exit Sub
                            End If
                        End Using

                        ' Insert new record
                        Using cmd As New SqlCommand("
                    INSERT INTO TBL_INVENTORY
                        (PTYPE, DESCRIPTIONS, ATITLE, ACODE, IPNO, UNITS, QTY, STATUS, REMARKS)
                    VALUES
                        (@ptype, @desc, @atitle, @acode, @ipno, @units, @qty, @status, @remarks);", conn)

                            cmd.Parameters.Add("@ptype", SqlDbType.NVarChar, 100).Value = combotype.Text.Trim()
                            cmd.Parameters.Add("@desc", SqlDbType.NVarChar, 500).Value = txtdesc.Text.Trim()
                            cmd.Parameters.Add("@atitle", SqlDbType.NVarChar, 500).Value = txtatitle.Text.Trim()
                            cmd.Parameters.Add("@acode", SqlDbType.NVarChar, 100).Value = txtacode.Text.Trim()
                            cmd.Parameters.Add("@ipno", SqlDbType.NVarChar, 100).Value = txtipno.Text.Trim()
                            cmd.Parameters.Add("@units", SqlDbType.NVarChar, 100).Value = txtunit.Text.Trim()
                            cmd.Parameters.Add("@qty", SqlDbType.Int).Value = qty
                            cmd.Parameters.Add("@status", SqlDbType.NVarChar, 50).Value = status
                            cmd.Parameters.Add("@remarks", SqlDbType.NVarChar, 1000).Value = remarks

                            cmd.ExecuteNonQuery()
                        End Using
                    End Using

                    ' Refresh parent list then close
                    'If Me.Owner IsNot Nothing AndAlso TypeOf Me.Owner Is frminventory Then
                    '  DirectCast(Me.Owner, frminventory).LoadInventory()
                    'End If
                    '  frminventory.LoadInventory()
                    MsgBox("Item/Property has been added to the inventory.", vbInformation, "Message")
                    Me.Dispose()
                    Exit Sub
                End If
            End If

            ' ------------------------------
            ' UPDATE EXISTING RECORD
            ' ------------------------------
            If String.Equals(cmdsave.Text, "Update", StringComparison.OrdinalIgnoreCase) Then
                Dim id As Integer
                If Not Integer.TryParse(lblid.Text, id) Then
                    MessageBox.Show("Invalid inventory ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End If

                Dim qty As Integer = 0
                Integer.TryParse(txtqty.Text.Trim(), qty)
                Dim remarks As String = txtremarks.Text.Trim()
                Dim status As String = If(qty > 0, "In Stock", "Out of Stock")

                Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
                    conn.Open()
                    Using cmd As New SqlCommand("
                    UPDATE TBL_INVENTORY
                       SET QTY = @qty,
                           STATUS = @status,
                           REMARKS = @remarks
                     WHERE ID = @id;", conn)

                        cmd.Parameters.Add("@qty", SqlDbType.Int).Value = qty
                        cmd.Parameters.Add("@status", SqlDbType.NVarChar, 50).Value = status
                        cmd.Parameters.Add("@remarks", SqlDbType.NVarChar, 1000).Value = remarks
                        cmd.Parameters.Add("@id", SqlDbType.Int).Value = id

                        Dim affected As Integer = cmd.ExecuteNonQuery()
                        If affected = 0 Then
                            MessageBox.Show("Nothing was updated. The record may have been changed or removed by another user.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            Return
                        End If
                    End Using
                End Using

                ' Refresh parent list then close
                ' If Me.Owner IsNot Nothing AndAlso TypeOf Me.Owner Is frminventory Then
                ' DirectCast(Me.Owner, frminventory).LoadInventory()
                'End If
                ' frminventory.LoadInventory()
                MsgBox("Item/Property has been updated to the inventory.", vbInformation, "Message")
                Me.Dispose()
            End If

        Catch ex As Exception
            MessageBox.Show(cmdsave.Text & " failed: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


    ' Compute once controls are fully shown (more reliable than Load for prefilled fields)
    Private Sub frminventoryfile_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        UpdateStockStatus()
    End Sub

    ' Keep status in sync while typing
    Private Sub txtqty_TextChanged(sender As Object, e As EventArgs) Handles txtqty.TextChanged
        UpdateStockStatus()
    End Sub

    ' (optional) keep digits-only typing; allow control keys
    Private Sub txtqty_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtqty.KeyPress
        If Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsDigit(e.KeyChar) Then
            e.Handled = True
        End If
    End Sub

    Private Sub Cmdclose_Click(sender As Object, e As EventArgs) Handles cmdclose.Click
        Me.Dispose()
    End Sub
End Class