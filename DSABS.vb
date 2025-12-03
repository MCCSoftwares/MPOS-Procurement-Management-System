

Partial Public Class DSABS
    Partial Public Class DTABSDataTable
        Private Sub DTABSDataTable_DTABSRowChanging(sender As Object, e As DTABSRowChangeEvent) Handles Me.DTABSRowChanging

        End Sub

        Private Sub DTABSDataTable_ColumnChanging(sender As Object, e As DataColumnChangeEventArgs) Handles Me.ColumnChanging
            If (e.Column.ColumnName = Me.COMPANYColumn.ColumnName) Then
                'Add user code here
            End If

        End Sub

    End Class
End Class
