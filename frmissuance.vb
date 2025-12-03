Public Class frmissuance
    Private _loadingChild As Boolean = False
    Private _currentChild As Form = Nothing
    Private _sep As frmissuancesep = Nothing
    Private _ppe As frmissuancePPE = Nothing

    Private Sub ShowChild(child As Form)
        If _loadingChild Then Exit Sub
        _loadingChild = True
        Try
            ' Avoid re-adding the same instance
            If _currentChild Is child Then Exit Sub

            Panel2.SuspendLayout()

            Panel2.Controls.Clear()
            If _currentChild IsNot Nothing Then
                Try : _currentChild.Dispose() : Catch : End Try
            End If

            child.TopLevel = False
            child.FormBorderStyle = FormBorderStyle.None
            child.Dock = DockStyle.Fill

            Panel2.Controls.Add(child)
            _currentChild = child
            child.Show()

            Panel2.ResumeLayout()
        Finally
            _loadingChild = False
        End Try
    End Sub

    Private Sub cmdppe_Click(sender As Object, e As EventArgs) Handles cmdppe.Click
        If _ppe Is Nothing OrElse _ppe.IsDisposed Then _ppe = New frmissuancePPE()
        ShowChild(_ppe)
    End Sub

    Private Sub Cmdsep_Click(sender As Object, e As EventArgs) Handles cmdsep.Click
        If _sep Is Nothing OrElse _sep.IsDisposed Then _sep = New frmissuancesep()
        ShowChild(_sep)
    End Sub
End Class
