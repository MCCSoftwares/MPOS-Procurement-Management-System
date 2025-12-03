Imports System.Windows.Forms

Public Class frmprocurement

    ' Keep one live instance per hosted form
    Private ReadOnly _hosted As New Dictionary(Of Type, Form)()

    Private Sub frmprocurement_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Make panel resize the hosted form automatically
        Panel1.Dock = DockStyle.Fill
        Panel1.AutoScroll = False
        Panel1.BorderStyle = BorderStyle.None
        ' Optionally show a default screen here:
        ' ShowForm(Of frmpr)()
    End Sub

    ' ---------- Core hoster ----------
    Private Sub ShowForm(Of T As {Form, New})(Optional canAccess As Boolean = True)
        If Not canAccess Then
            MessageBox.Show("Unauthorized: your access level does not allow this action.", "Access denied",
                        MessageBoxButtons.OK, MessageBoxIcon.[Error])
            Return
        End If

        Me.SuspendLayout()
        Panel1.SuspendLayout()

        Dim formType As Type = GetType(T)   ' <-- renamed from t
        Dim f As Form = Nothing

        If _hosted.TryGetValue(formType, f) Then
            If f Is Nothing OrElse f.IsDisposed Then
                _hosted.Remove(formType)
                f = Nothing
            End If
        End If

        If f Is Nothing Then
            f = New T() With {
            .TopLevel = False,
            .FormBorderStyle = FormBorderStyle.None,
            .Dock = DockStyle.Fill
        }
            AddHandler f.FormClosed, Sub() If _hosted.ContainsKey(formType) Then _hosted.Remove(formType)
            _hosted(formType) = f
            Panel1.Controls.Add(f)
        End If

        For Each ctl As Control In Panel1.Controls
            ctl.Visible = False
        Next

        f.Visible = True
        f.BringToFront()
        f.Show()

        Panel1.ResumeLayout()
        Me.ResumeLayout()
    End Sub


    ' ---------- Button handlers (fixed & clear) ----------
    Private Sub cmdpr_Click(sender As Object, e As EventArgs) Handles cmdpr.Click
        ShowForm(Of frmpr)(canAccess:=True)
    End Sub

    Private Sub cmdrfq_Click(sender As Object, e As EventArgs) Handles cmdrfq.Click
        ShowForm(Of frmrfq)(canAccess:=True)
    End Sub

    Private Sub cmdabstract_Click(sender As Object, e As EventArgs) Handles cmdabstract.Click
        Dim position As String = frmmain.lblaposition.Text
        Dim allowed As Boolean = Not String.Equals(position, "End User", StringComparison.OrdinalIgnoreCase)
        ShowForm(Of frmabstract)(canAccess:=allowed)
    End Sub

    Private Sub cmdpo_Click(sender As Object, e As EventArgs) Handles cmdpo.Click
        Dim position As String = frmmain.lblaposition.Text
        Dim allowed As Boolean = Not String.Equals(position, "End User", StringComparison.OrdinalIgnoreCase)
        ShowForm(Of frmpo)(canAccess:=allowed)
    End Sub

End Class
