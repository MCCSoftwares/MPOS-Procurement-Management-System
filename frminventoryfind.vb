Imports System.Data
Imports System.Data.SqlClient

Public Class frminventoryfind
    ' Guards to avoid races between selection and TextChanged
    Private isCommittingSelection As Boolean = False
    Private suppressTextChanged As Boolean = False

    ' ─────────────────────────────────────────────────────────
    ' Fields
    ' ─────────────────────────────────────────────────────────
    Private ReadOnly suggestionTimer As New System.Windows.Forms.Timer() With {.Interval = 250}
    Private currentRows As New List(Of DataRow)()   ' mirrors ComboBox.Items order
    Private selectedRow As DataRow = Nothing
    Private updatingList As Boolean = False         ' guard while repopulating Items

    ' Central caret restore: focus + IBeam + selection
    Private Sub RestoreCaret(Optional keepText As String = Nothing, Optional keepPos As Integer? = Nothing)
        Try
            If keepText IsNot Nothing Then
                suppressTextChanged = True
                combodesc.Text = keepText
                suppressTextChanged = False
            End If

            If Not combodesc.Focused Then combodesc.Focus()
            combodesc.Cursor = Cursors.IBeam

            Dim pos As Integer = If(keepPos.HasValue, Math.Min(keepPos.Value, combodesc.Text.Length), combodesc.Text.Length)
            combodesc.SelectionStart = pos
            combodesc.SelectionLength = 0
        Catch
            ' swallow focus/selection errors
        End Try
    End Sub

    ' ─────────────────────────────────────────────────────────
    ' Form Load
    ' ─────────────────────────────────────────────────────────
    Private Sub frminventoryfind_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            ' Combo: allow typing, no built-in autocomplete (we manage suggestions)
            combodesc.DropDownStyle = ComboBoxStyle.DropDown
            combodesc.AutoCompleteMode = AutoCompleteMode.None
            combodesc.AutoCompleteSource = AutoCompleteSource.None
            combodesc.MaxDropDownItems = 15
            combodesc.IntegralHeight = True

            AddHandler suggestionTimer.Tick, AddressOf SuggestionTimer_Tick
            AddHandler combodesc.TextChanged, AddressOf Combodesc_TextChanged
            AddHandler combodesc.SelectionChangeCommitted, AddressOf Combodesc_SelectionChangeCommitted
            AddHandler combodesc.KeyDown, AddressOf Combodesc_KeyDown
            AddHandler combodesc.DropDown, Sub() BeginInvoke(CType(Sub() RestoreCaret(), MethodInvoker))
            AddHandler combodesc.DropDownClosed, Sub() BeginInvoke(CType(Sub() RestoreCaret(), MethodInvoker))

            cmdadd.Enabled = False
        Catch ex As Exception
            MessageBox.Show("Load error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ─────────────────────────────────────────────────────────
    ' Typing → debounce → query LIKE suggestions
    ' ─────────────────────────────────────────────────────────
    Private Sub Combodesc_TextChanged(sender As Object, e As EventArgs)
        If suppressTextChanged OrElse isCommittingSelection Then Return

        selectedRow = Nothing
        If combodesc.SelectedIndex <> -1 Then combodesc.SelectedIndex = -1
        cmdadd.Enabled = False

        suggestionTimer.Stop()
        suggestionTimer.Start()
    End Sub

    ' ─────────────────────────────────────────────────────────
    ' Typing debounce callback
    ' ─────────────────────────────────────────────────────────
    Private Sub SuggestionTimer_Tick(sender As Object, e As EventArgs)
        suggestionTimer.Stop()
        Try
            Dim q As String = combodesc.Text.Trim()
            Dim caret As Integer = combodesc.SelectionStart

            If q.Length = 0 Then
                RepopulateItems(New DataTable(), retainText:=True, caret:=caret)
                selectedRow = Nothing
                cmdadd.Enabled = False
                combodesc.DroppedDown = False
                BeginInvoke(CType(Sub() RestoreCaret(keepText:=q, keepPos:=caret), MethodInvoker))
                Exit Sub
            End If

            Dim dt As DataTable = GetSuggestions(q) ' LIKE %q%
            RepopulateItems(dt, retainText:=True, caret:=caret)

            If combodesc.Items.Count > 0 Then
                ' Show list AFTER restoring text/caret, then re-assert caret via BeginInvoke
                combodesc.DroppedDown = True
                BeginInvoke(CType(Sub() RestoreCaret(keepText:=q, keepPos:=caret), MethodInvoker))
            Else
                selectedRow = Nothing
                cmdadd.Enabled = False
                combodesc.DroppedDown = False
                BeginInvoke(CType(Sub() RestoreCaret(keepText:=q, keepPos:=caret), MethodInvoker))
            End If

        Catch
            ' silent
        End Try
    End Sub

    ' ─────────────────────────────────────────────────────────
    ' Refill ComboBox.Items from DataTable safely
    ' ─────────────────────────────────────────────────────────
    Private Sub RepopulateItems(dt As DataTable, Optional retainText As Boolean = True, Optional caret As Integer = -1)
        updatingList = True
        Dim typed As String = combodesc.Text   ' capture BEFORE updates

        Try
            combodesc.BeginUpdate()
            combodesc.Items.Clear()
            currentRows.Clear()

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                For Each r As DataRow In dt.Rows
                    combodesc.Items.Add(r("EDESC").ToString())
                    currentRows.Add(r)
                Next
            End If

            ' Keep in free-text mode
            combodesc.SelectedIndex = -1
        Finally
            combodesc.EndUpdate()
            updatingList = False
        End Try

        ' Restore typed text and caret after repopulating, without triggering TextChanged
        If retainText Then
            Try
                suppressTextChanged = True
                combodesc.Text = typed
                combodesc.SelectionStart = If(caret >= 0, Math.Min(caret, combodesc.Text.Length), combodesc.Text.Length)
                combodesc.SelectionLength = 0
            Finally
                suppressTextChanged = False
            End Try
        End If
    End Sub

    ' ─────────────────────────────────────────────────────────
    ' User picks an item (mouse click or Enter on list)
    ' ─────────────────────────────────────────────────────────
    Private Sub Combodesc_SelectionChangeCommitted(sender As Object, e As EventArgs)
        Try
            isCommittingSelection = True
            suggestionTimer.Stop()

            selectedRow = Nothing
            If combodesc.SelectedIndex >= 0 AndAlso combodesc.SelectedIndex < currentRows.Count Then
                selectedRow = currentRows(combodesc.SelectedIndex)
                cmdadd.Enabled = True

                ' Show chosen text without re-triggering TextChanged
                suppressTextChanged = True
                combodesc.Text = selectedRow("EDESC").ToString()
                suppressTextChanged = False

                combodesc.DroppedDown = False
                BeginInvoke(CType(Sub() RestoreCaret(), MethodInvoker))
            Else
                cmdadd.Enabled = False
            End If
        Catch ex As Exception
            MessageBox.Show("Select error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            isCommittingSelection = False
        End Try
    End Sub

    Private Sub Combodesc_KeyDown(sender As Object, e As KeyEventArgs)
        Try
            ' Enter confirms selection from list (if open)
            If e.KeyCode = Keys.Enter AndAlso combodesc.DroppedDown AndAlso combodesc.SelectedIndex >= 0 Then
                isCommittingSelection = True
                suggestionTimer.Stop()

                selectedRow = currentRows(combodesc.SelectedIndex)
                cmdadd.Enabled = True

                suppressTextChanged = True
                combodesc.Text = selectedRow("EDESC").ToString()
                suppressTextChanged = False

                combodesc.DroppedDown = False
                BeginInvoke(CType(Sub() RestoreCaret(), MethodInvoker))

                e.Handled = True
                e.SuppressKeyPress = True
            ElseIf e.KeyCode = Keys.Escape Then
                ' ESC closes list and restores caret to edit box
                combodesc.DroppedDown = False
                BeginInvoke(CType(Sub() RestoreCaret(), MethodInvoker))
                e.Handled = True
                e.SuppressKeyPress = True
            End If
        Catch ex As Exception
            MessageBox.Show("Key handling error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            isCommittingSelection = False
        End Try
    End Sub

    ' ─────────────────────────────────────────────────────────
    ' SQL: fetch TOP 15 distinct EDESC + other columns
    ' ─────────────────────────────────────────────────────────
    Private Function GetSuggestions(userText As String) As DataTable
        Dim dt As New DataTable()
        Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
            conn.Open()
            Dim sql As String =
                "SELECT TOP (15)
                         EDESC,
                         MAX(ATITLE) AS ATITLE,
                         MAX(ACODE)  AS ACODE,
                         MAX(IPNO)   AS IPNO,
                         MAX(UNIT)   AS UNIT
                   FROM TBL_EDESC WITH (NOLOCK)
                  WHERE EDESC LIKE @q
                  GROUP BY EDESC
                  ORDER BY EDESC;"
            Using cmd As New SqlCommand(sql, conn)
                cmd.Parameters.Add("@q", SqlDbType.NVarChar, 500).Value = "%" & userText & "%"
                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using
            End Using
        End Using
        Return dt
    End Function

    ' ─────────────────────────────────────────────────────────
    ' cmdadd: check duplicates in TBL_INVENTORY then open form
    ' ─────────────────────────────────────────────────────────
    Private Sub cmdadd_Click(sender As Object, e As EventArgs) Handles cmdadd.Click
        Try
            If selectedRow Is Nothing Then
                MessageBox.Show("Please select a description from the list first.", "Select Description", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Exit Sub
            End If

            Dim selDesc As String = SafeRowValue(selectedRow, "EDESC")
            If String.IsNullOrWhiteSpace(selDesc) Then
                MessageBox.Show("Invalid selection.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            ' 1) Duplicate check
            Dim exists As Boolean
            Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
                conn.Open()
                Using cmd As New SqlCommand("SELECT COUNT(1) FROM TBL_INVENTORY WITH (NOLOCK) WHERE DESCRIPTIONS = @d;", conn)
                    cmd.Parameters.Add("@d", SqlDbType.NVarChar, 500).Value = selDesc
                    exists = (CInt(cmd.ExecuteScalar()) > 0)
                End Using
            End Using

            If exists Then
                MessageBox.Show("This description already exists in inventory.", "Duplicate Found", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            ' 2) Prefill and open frminventoryfile
            Using f As New frminventoryfile()
                f.combotype.Text = ""                       ' not specified by source
                f.txtdesc.Text = selDesc                    ' TBL_EDESC.EDESC
                f.txtatitle.Text = SafeRowValue(selectedRow, "ATITLE")
                f.txtacode.Text = SafeRowValue(selectedRow, "ACODE")
                f.txtipno.Text = SafeRowValue(selectedRow, "IPNO")
                f.txtunit.Text = SafeRowValue(selectedRow, "UNIT")
                f.txtqty.Text = ""                          ' empty
                f.txtstatus.Text = "oUT OF sTOCK"           ' per spec
                f.txtremarks.Text = ""                      ' empty
                f.lbltrans.Text = "New"
                f.txtdesc.Enabled = False
                f.txtacode.Enabled = False
                f.txtatitle.Enabled = False
                f.txtunit.Enabled = False
                f.txtstatus.Enabled = False
                f.txtipno.Enabled = False
                f.StartPosition = FormStartPosition.CenterParent
                f.ShowDialog(Me)
            End Using

            Me.Dispose()

        Catch ex As Exception
            MessageBox.Show("Add error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ─────────────────────────────────────────────────────────
    ' Helpers
    ' ─────────────────────────────────────────────────────────
    Private Function SafeRowValue(r As DataRow, col As String) As String
        If r Is Nothing OrElse Not r.Table.Columns.Contains(col) OrElse r.IsNull(col) Then Return ""
        Return r(col).ToString().Trim()
    End Function

End Class
