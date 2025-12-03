Imports System.Reflection
Imports System.Windows.Forms

Public Module UiHelpers

    Public Sub EnableDoubleBuffering(ctrl As Control, Optional recurse As Boolean = True)
        If ctrl Is Nothing Then Return

        ' Skip controls that don't like UserPaint (TextBox, RichTextBox, ComboBox, MaskedTextBox)
        Dim isNativeEdit As Boolean =
            TypeOf ctrl Is TextBoxBase OrElse
            TypeOf ctrl Is ComboBox OrElse
            TypeOf ctrl Is NumericUpDown OrElse
            TypeOf ctrl Is DateTimePicker

        ' DoubleBuffered (safe)
        Dim prop = ctrl.GetType().GetProperty("DoubleBuffered",
                     BindingFlags.Instance Or BindingFlags.NonPublic)
        If prop IsNot Nothing Then prop.SetValue(ctrl, True, Nothing)

        ' Only touch SetStyle/UserPaint on safe controls (Form, Panel, DataGridView, custom panels, etc.)
        If Not isNativeEdit Then
            Dim setStyle = ctrl.GetType().GetMethod("SetStyle",
                         BindingFlags.Instance Or BindingFlags.NonPublic)

            If setStyle IsNot Nothing Then
                Dim flags = ControlStyles.AllPaintingInWmPaint Or
                            ControlStyles.OptimizedDoubleBuffer
                ' add UserPaint only when safe
                flags = flags Or ControlStyles.UserPaint

                setStyle.Invoke(ctrl, New Object() {flags, True})
            End If

            Dim upd = ctrl.GetType().GetMethod("UpdateStyles",
                         BindingFlags.Instance Or BindingFlags.NonPublic)
            If upd IsNot Nothing Then upd.Invoke(ctrl, Nothing)
        End If

        If recurse Then
            For Each c As Control In ctrl.Controls
                EnableDoubleBuffering(c, True)
            Next
        End If
    End Sub

End Module
