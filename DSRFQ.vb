

Partial Public Class DSRFQ
    Partial Public Class DTRFQDataTable
        Private Sub DTRFQDataTable_ColumnChanging(sender As Object, e As DataColumnChangeEventArgs) Handles Me.ColumnChanging
            If (e.Column.ColumnName = Me.RFQ_ITEMNOColumn.ColumnName) Then
                'Add user code here
            End If

        End Sub

    End Class
End Class
