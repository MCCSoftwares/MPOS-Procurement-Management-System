Imports System.Data.SqlClient
Public Class frmrfqupdate

    Private Sub combounit_KeyPress(sender As Object, e As KeyPressEventArgs) Handles combounit.KeyPress
        e.Handled = True
    End Sub


    Private Sub txtdesc_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtdesc.KeyPress
        e.Handled = True
    End Sub

    Private Sub txtqty_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtqty.KeyPress
        e.Handled = True
    End Sub

    Private Sub Frmrfqupdate_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub txtucost_TextChanged(sender As Object, e As EventArgs) Handles txtucost.TextChanged
        RecomputeTotal()
    End Sub

    Private Sub RecomputeTotal()
        Dim q As Decimal, u As Decimal
        Decimal.TryParse(txtqty.Text, q)
        Decimal.TryParse(txtucost.Text, u)
        txttcost.Text = (q * u).ToString("N2")
    End Sub

    Private Sub txttcost_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txttcost.KeyPress
        e.Handled = True
    End Sub

    Private Sub txtucost_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtucost.KeyPress
        ' Allow control keys (backspace, delete), digits, and one dot only
        If Not Char.IsControl(e.KeyChar) AndAlso
       Not Char.IsDigit(e.KeyChar) AndAlso
       e.KeyChar <> "."c Then
            e.Handled = True
        End If

        ' If it's a dot, make sure there's not already one in the text
        If e.KeyChar = "."c Then
            Dim tb = DirectCast(sender, TextBox)
            If tb.Text.Contains("."c) Then
                e.Handled = True
            End If
        End If
    End Sub

    Private Sub Cmdclose_Click(sender As Object, e As EventArgs) Handles cmdclose.Click
        Me.Dispose()
    End Sub

    Public Sub UpdateRFQItem()
        ' 1) Validate we have a valid item ID
        Dim itemId As Integer
        If Not Integer.TryParse(lbliid.Text, itemId) OrElse itemId <= 0 Then
            MessageBox.Show("Invalid item ID.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' 2) Validate required fields
        If String.IsNullOrWhiteSpace(combounit.Text) Then
            MessageBox.Show("Unit is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            combounit.Focus() : Return
        End If
        If String.IsNullOrWhiteSpace(txtdesc.Text) Then
            MessageBox.Show("Description is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtdesc.Focus() : Return
        End If

        ' 3) Parse numeric values
        Dim qty As Decimal
        Dim ucost As Decimal
        Decimal.TryParse(txtqty.Text, qty)
        Decimal.TryParse(txtucost.Text, ucost)
        Dim tcost = qty * ucost
        Dim sum = tcost

        Try
            Using conn As New SqlConnection(frmmain.txtdb.Text)
                conn.Open()
                Const sql As String = "
                UPDATE TBL_RFQITEMS
                SET
                  RFQ_UNIT  = @Unit,
                  RFQ_DESC  = @Desc,
                  RFQ_QTY   = @Qty,
                  RFQ_UCOST = @UCost,
                  RFQ_TCOST = @TCost,
                  RFQ_SUM   = @Sum
                WHERE ID = @ID
            "
                Using cmd As New SqlCommand(sql, conn)
                    With cmd.Parameters
                        .AddWithValue("@Unit", combounit.Text.Trim())
                        .AddWithValue("@Desc", txtdesc.Text.Trim())
                        .AddWithValue("@Qty", qty)
                        .AddWithValue("@UCost", ucost)
                        .AddWithValue("@TCost", tcost)
                        .AddWithValue("@Sum", sum)
                        .AddWithValue("@ID", itemId)
                    End With
                    If cmd.ExecuteNonQuery() = 0 Then
                        MessageBox.Show("No record was updated. Please check the item ID.",
                                        "Update Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return
                    End If
                End Using
            End Using

            ' 4) Refresh the items grid on the supplier form
            Dim parent = TryCast(Application.OpenForms("frmrfqsupplier"), frmrfqsupplier)
            If parent IsNot Nothing Then
                parent.LoadRFQItems()    ' or whatever your method name is to reload the grid
            End If
            UpdateSupplierSum()
            MessageBox.Show("Item updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show("Error updating item: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            ' 5) Close and dispose this update form
            Me.Close()
            Me.Dispose()
        End Try
    End Sub

    Private Sub Cmdsave_Click(sender As Object, e As EventArgs) Handles cmdsave.Click
        UpdateRFQItem()
    End Sub

    ''' Call this from frmrfqupdate (after you update an item) to re‐sum the supplier’s
    ''' total and refresh both the supplier list and the RFQ header form. '''
    Public Sub UpdateSupplierSum()
        ' 1) Find the supplier form
        Dim supForm = TryCast(Application.OpenForms("frmrfqsupplier"), frmrfqsupplier)
        If supForm Is Nothing Then
            MessageBox.Show("Cannot find supplier form.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        ' 2) Parse the Supplier ID from frmrfqsupplier.lblid
        Dim supId As Integer
        If Not Integer.TryParse(supForm.lblid.Text, supId) OrElse supId <= 0 Then
            MessageBox.Show("Invalid Supplier ID.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Try
            ' 3) Update TBL_RFQSUPPLIER.RFQ_SUM = SUM(RFQ_SUM) from TBL_RFQITEMS
            Using conn As New SqlConnection(frmmain.txtdb.Text)
                conn.Open()
                Const sql As String = "
                UPDATE TBL_RFQSUPPLIER
                SET RFQ_SUM = (
                    SELECT ISNULL(SUM(RFQ_SUM), 0)
                      FROM TBL_RFQITEMS
                     WHERE RFQSID = @SupID
                )
                WHERE ID = @SupID
            "
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@SupID", supId)
                    cmd.ExecuteNonQuery()
                End Using
            End Using

            ' 4) Refresh the supplier’s item grid on frmrfqsupplier
            supForm.LoadRFQItems()        ' <-- your method to reload the RFQ items grid

            ' 5) Refresh the supplier list on the RFQ header form
            Dim hdrForm = TryCast(Application.OpenForms("frmrfqfile"), frmrfqfile)
            If hdrForm IsNot Nothing Then
                hdrForm.LoadSupplierRecords()  ' reload the supplier list grid
                ' and also refresh the header’s own data if desired:
                Dim hdrId As Integer
                If Integer.TryParse(hdrForm.lblid.Text, hdrId) Then
                    hdrForm.LoadRFQData(hdrId)
                End If
            End If

        Catch ex As Exception
            MessageBox.Show("Error updating supplier total: " & ex.Message,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

End Class