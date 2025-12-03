Imports System.ComponentModel
Imports System.Data.SqlClient
Imports System.IO
Imports System.Threading
Imports System.Threading.Tasks

Public Class frmpradd
    Private _editingItemId As Integer = -1
    Private suppressTextChanged As Boolean = False

    ' Async infra for suggestions
    Private WithEvents _suggestTimer As System.Windows.Forms.Timer
    Private _suggestCts As CancellationTokenSource = New CancellationTokenSource()

    ' Prevent double-save/update
    Private _isSaving As Boolean = False

    Private Sub Cmdaddunit_Click(sender As Object, e As EventArgs)
        Try
            frmpraddunit.Dispose()
            frmpraddunit.ShowDialog()
        Catch
        End Try
    End Sub

    Private Sub Cmdclose_Click(sender As Object, e As EventArgs) Handles cmdclose.Click
        Me.Dispose()
    End Sub

    Private Sub Frmpradd_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Combo behavior: user types freely; we show a drop-down list of suggestions only
        With txtdesc
            .DropDownStyle = ComboBoxStyle.DropDown   ' allow typing
            .AutoCompleteMode = AutoCompleteMode.None ' do NOT auto-append/replace text
            .AutoCompleteSource = AutoCompleteSource.None
        End With

        ' Debounce for suggestions
        _suggestTimer = New System.Windows.Forms.Timer() With {.Interval = 250}

        ' Optional: initial focus
        combounit.Focus()
    End Sub

    ' ───────────────────────────────────────────────
    ' Numeric inputs & totals
    ' ───────────────────────────────────────────────
    Private Sub NumericTextBox_KeyPress(sender As Object, e As KeyPressEventArgs) _
        Handles txtqty.KeyPress, txtucost.KeyPress, txttcost.KeyPress

        Dim tb As TextBox = DirectCast(sender, TextBox)
        If Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsDigit(e.KeyChar) AndAlso e.KeyChar <> "."c Then
            e.Handled = True
        End If
        If e.KeyChar = "."c AndAlso tb.Text.Contains(".") Then
            e.Handled = True
        End If
    End Sub

    Private Sub NumericTextBox_Leave(sender As Object, e As EventArgs) _
        Handles txtqty.Leave, txtucost.Leave, txttcost.Leave

        Dim tb As TextBox = DirectCast(sender, TextBox)
        Dim val As Double
        If Double.TryParse(tb.Text, val) Then
            tb.Text = val.ToString("#,##0.00")
        Else
            tb.Text = "0.00"
        End If
        If tb Is txtqty OrElse tb Is txtucost Then
            ComputeTotalCost()
        End If
    End Sub

    Private Sub NumericTextBox_TextChanged(sender As Object, e As EventArgs) _
        Handles txtqty.TextChanged, txtucost.TextChanged
        ComputeTotalCost()
    End Sub

    Private Sub ComputeTotalCost()
        Dim qty As Double = 0, ucost As Double = 0
        Double.TryParse(txtqty.Text, qty)
        Double.TryParse(txtucost.Text, ucost)
        Dim total As Double = qty * ucost
        txttcost.Text = total.ToString("#,##0.00")
    End Sub

    ' ───────────────────────────────────────────────
    ' SAVE / UPDATE (async, non-blocking)
    ' ───────────────────────────────────────────────
    Private Async Function SavePRItemAsync() As Task
        If _isSaving Then Return
        _isSaving = True

        Try
            ' Required
            If String.IsNullOrWhiteSpace(combounit.Text) Then
                MessageBox.Show("Unit is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                combounit.Focus() : Exit Function
            End If
            If String.IsNullOrWhiteSpace(txtdesc.Text) Then
                MessageBox.Show("Description is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtdesc.Focus() : Exit Function
            End If
            If String.IsNullOrWhiteSpace(txtqty.Text) Then
                MessageBox.Show("Quantity is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtqty.Focus() : Exit Function
            End If
            If String.IsNullOrWhiteSpace(txtucost.Text) Then
                MessageBox.Show("Unit cost is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtucost.Focus() : Exit Function
            End If
            If String.IsNullOrWhiteSpace(txttcost.Text) Then
                MessageBox.Show("Total cost is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txttcost.Focus() : Exit Function
            End If

            ' Parent PR
            Dim parent = TryCast(Application.OpenForms("frmprfile"), frmprfile)
            If parent Is Nothing Then
                MessageBox.Show("Cannot find PR form.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) : Exit Function
            End If
            Dim prId As Integer
            If Not Integer.TryParse(parent.lblid.Text, prId) Then
                MessageBox.Show("Invalid PR ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) : Exit Function
            End If
            Dim prNo As String = parent.txtprno.Text.Trim()

            ' Numbers
            Dim qty As Decimal = Convert.ToDecimal(txtqty.Text)
            Dim ucost As Decimal = Convert.ToDecimal(txtucost.Text)
            Dim tcost As Decimal = Convert.ToDecimal(txttcost.Text)

            Using conn As SqlConnection = GetTunedConnection()
                Await conn.OpenAsync()

                ' Next line #
                Dim nextLine As Integer
                Using cmdMax As New SqlCommand("SELECT ISNULL(MAX(PR_ITEMNO),0) + 1 FROM TBL_PRITEMS WHERE PRID=@PRID", conn)
                    cmdMax.Parameters.Add("@PRID", SqlDbType.Int).Value = prId
                    nextLine = Convert.ToInt32(Await cmdMax.ExecuteScalarAsync())
                End Using

                Using cmdIns As New SqlCommand("
                    INSERT INTO TBL_PRITEMS
                      (PRID, PRNO, PR_ITEMNO, PR_UNIT, PR_DESC, PR_QTY, PR_UCOST, PR_TCOST, PR_SUM)
                    VALUES
                      (@PRID,@PRNO,@Line,@Unit,@Desc,@Qty,@UCost,@TCost,@Sum);", conn)
                    With cmdIns.Parameters
                        .Add("@PRID", SqlDbType.Int).Value = prId
                        .Add("@PRNO", SqlDbType.NVarChar, 100).Value = prNo
                        .Add("@Line", SqlDbType.Int).Value = nextLine
                        .Add("@Unit", SqlDbType.NVarChar, 100).Value = combounit.Text.Trim()
                        .Add("@Desc", SqlDbType.NVarChar, 4000).Value = txtdesc.Text.Trim()
                        .Add("@Qty", SqlDbType.Decimal).Value = qty
                        .Add("@UCost", SqlDbType.Decimal).Value = ucost
                        .Add("@TCost", SqlDbType.Decimal).Value = tcost
                        .Add("@Sum", SqlDbType.Decimal).Value = qty * ucost
                    End With
                    Await cmdIns.ExecuteNonQueryAsync()
                End Using
            End Using

            ' Refresh parent grid without blocking this form's UI
            If TypeOf Application.OpenForms("frmprfile") Is frmprfile Then
                Dim p = DirectCast(Application.OpenForms("frmprfile"), frmprfile)
                p.BeginInvoke(Sub() p.LoadPRItems())
                p.BeginInvoke(Sub() UpdatePRTotalsAsync(prId)) ' async update of header totals
            End If

            MessageBox.Show("Item saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

            ' Clear fields (keep unit if you prefer)
            txtdesc.Text = ""
            txtqty.Text = "0.00"
            txtucost.Text = "0.00"
            txttcost.Text = "0.00"
            combounit.Focus()

        Catch ex As Exception
            MessageBox.Show("Error saving item: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            _isSaving = False
        End Try
    End Function

    Private Async Function UpdatePRItemAsync() As Task
        If _isSaving Then Return
        _isSaving = True

        Try
            ' Required
            If String.IsNullOrWhiteSpace(combounit.Text) Then
                MessageBox.Show("Unit is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                combounit.Focus() : Exit Function
            End If
            If String.IsNullOrWhiteSpace(txtdesc.Text) Then
                MessageBox.Show("Description is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtdesc.Focus() : Exit Function
            End If
            If String.IsNullOrWhiteSpace(txtqty.Text) Then
                MessageBox.Show("Quantity is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtqty.Focus() : Exit Function
            End If
            If String.IsNullOrWhiteSpace(txtucost.Text) Then
                MessageBox.Show("Unit cost is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtucost.Focus() : Exit Function
            End If
            If String.IsNullOrWhiteSpace(txttcost.Text) Then
                MessageBox.Show("Total cost is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txttcost.Focus() : Exit Function
            End If

            Dim parent = TryCast(Application.OpenForms("frmprfile"), frmprfile)
            If parent Is Nothing Then
                MessageBox.Show("Cannot find PR form.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Function
            End If

            Dim prId As Integer
            If Not Integer.TryParse(parent.lblid.Text, prId) Then
                MessageBox.Show("Invalid PR ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Function
            End If

            Dim itemId As Integer
            If Not Integer.TryParse(lbliid.Text, itemId) OrElse itemId <= 0 Then
                MessageBox.Show("No item selected for update.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Function
            End If

            Dim qty As Decimal = Convert.ToDecimal(txtqty.Text)
            Dim ucost As Decimal = Convert.ToDecimal(txtucost.Text)
            Dim tcost As Decimal = Convert.ToDecimal(txttcost.Text)
            Dim sum As Decimal = qty * ucost

            Using conn As SqlConnection = GetTunedConnection()
                Await conn.OpenAsync()
                Using cmd As New SqlCommand("
                    UPDATE TBL_PRITEMS
                    SET 
                      PR_UNIT  = @Unit,
                      PR_DESC  = @Desc,
                      PR_QTY   = @Qty,
                      PR_UCOST = @UCost,
                      PR_TCOST = @TCost,
                      PR_SUM   = @Sum
                    WHERE ID = @ItemId;", conn)
                    cmd.Parameters.Add("@Unit", SqlDbType.NVarChar, 100).Value = combounit.Text.Trim()
                    cmd.Parameters.Add("@Desc", SqlDbType.NVarChar, 4000).Value = txtdesc.Text.Trim()
                    cmd.Parameters.Add("@Qty", SqlDbType.Decimal).Value = qty
                    cmd.Parameters.Add("@UCost", SqlDbType.Decimal).Value = ucost
                    cmd.Parameters.Add("@TCost", SqlDbType.Decimal).Value = tcost
                    cmd.Parameters.Add("@Sum", SqlDbType.Decimal).Value = sum
                    cmd.Parameters.Add("@ItemId", SqlDbType.Int).Value = itemId
                    Await cmd.ExecuteNonQueryAsync()
                End Using
            End Using

            ' Refresh parent (non-blocking)
            parent.BeginInvoke(Sub() parent.LoadPRItems())
            parent.BeginInvoke(Sub() UpdatePRTotalsAsync(prId))

            MessageBox.Show("Item updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MessageBox.Show("Error updating item: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            _isSaving = False
        End Try
    End Function

    Private Async Sub Cmdsave_Click(sender As Object, e As EventArgs) Handles cmdsave.Click
        Try
            If String.Equals(cmdsave.Text, "Save", StringComparison.OrdinalIgnoreCase) Then
                Await SavePRItemAsync()
            ElseIf String.Equals(cmdsave.Text, "Update Item", StringComparison.OrdinalIgnoreCase) Then
                Await UpdatePRItemAsync()
            Else
                ' Fallback: decide by presence of lbliid
                If String.IsNullOrWhiteSpace(lbliid.Text) Then
                    Await SavePRItemAsync()
                Else
                    Await UpdatePRItemAsync()
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ───────────────────────────────────────────────
    ' Load an existing detail for editing
    ' ───────────────────────────────────────────────
    Public Async Sub LoadItemData(ByVal itemId As Integer)
        _editingItemId = itemId
        Try
            Using conn As SqlConnection = GetTunedConnection()
                Await conn.OpenAsync()
                Const sql As String = "
                    SELECT ID, PR_UNIT, PR_DESC, PR_QTY, PR_UCOST, PR_TCOST
                    FROM TBL_PRITEMS
                    WHERE ID = @ID;"
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.Add("@ID", SqlDbType.Int).Value = itemId
                    Using reader As SqlDataReader = Await cmd.ExecuteReaderAsync()
                        If Await reader.ReadAsync() Then
                            lbliid.Text = reader.GetInt32(reader.GetOrdinal("ID")).ToString()
                            combounit.Text = reader.GetString(reader.GetOrdinal("PR_UNIT"))
                            txtdesc.Text = reader.GetString(reader.GetOrdinal("PR_DESC"))
                            txtqty.Text = reader.GetDecimal(reader.GetOrdinal("PR_QTY")).ToString("#,##0.00")
                            txtucost.Text = reader.GetDecimal(reader.GetOrdinal("PR_UCOST")).ToString("#,##0.00")
                            txttcost.Text = reader.GetDecimal(reader.GetOrdinal("PR_TCOST")).ToString("#,##0.00")
                        Else
                            MessageBox.Show("Item not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                            _editingItemId = -1
                        End If
                    End Using
                End Using
            End Using
            cmdsave.Text = "Update Item"
        Catch ex As Exception
            MessageBox.Show("Failed to load item: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ───────────────────────────────────────────────
    ' Update header totals (async)
    ' ───────────────────────────────────────────────
    Public Async Function UpdatePRTotalsAsync(ByVal prId As Integer) As Task
        Try
            Using conn As SqlConnection = GetTunedConnection()
                Await conn.OpenAsync()
                Const sql As String = "
                    UPDATE TBL_PR
                    SET
                      I_ITOTALS = (SELECT COUNT(*)              FROM TBL_PRITEMS WHERE PRID = @PRID),
                      I_STOTALS = (SELECT ISNULL(SUM(PR_SUM),0) FROM TBL_PRITEMS WHERE PRID = @PRID)
                    WHERE ID = @PRID;"
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.Add("@PRID", SqlDbType.Int).Value = prId
                    Await cmd.ExecuteNonQueryAsync()
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error updating header totals: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Function

    ' ───────────────────────────────────────────────
    ' Suggestions for txtdesc (debounced + async)
    ' ───────────────────────────────────────────────
    Private Sub txtdesc_TextChanged(sender As Object, e As EventArgs) Handles txtdesc.TextChanged
        If suppressTextChanged Then Return
        If _suggestTimer Is Nothing Then Return
        _suggestTimer.Stop()
        _suggestTimer.Start()
    End Sub

    Private Async Sub _suggestTimer_Tick(sender As Object, e As EventArgs) Handles _suggestTimer.Tick
        _suggestTimer.Stop()
        Await LoadSuggestionsAsync(txtdesc.Text)
    End Sub

    Private Shared Function EscapeLikeValue(value As String) As String
        If String.IsNullOrEmpty(value) Then Return ""
        Return value.Replace("\", "\\").Replace("%", "\%").Replace("_", "\_")
    End Function

    Private Async Function LoadSuggestionsAsync(term As String) As Task
        term = If(term, "").Trim()
        If term.Length < 2 Then
            ' Hide suggestions if too short
            suppressTextChanged = True
            txtdesc.DroppedDown = False
            txtdesc.SelectedIndex = -1
            suppressTextChanged = False
            Return
        End If

        ' cancel previous fetch
        Try
            If _suggestCts IsNot Nothing Then
                _suggestCts.Cancel()
                _suggestCts.Dispose()
            End If
        Catch
        End Try
        _suggestCts = New CancellationTokenSource()
        Dim ct = _suggestCts.Token

        Dim suggestions As New List(Of String)
        Try
            Using conn As SqlConnection = GetTunedConnection()
                Await conn.OpenAsync(ct)
                Using cmd As New SqlCommand("
                    SELECT TOP 15 EDESC
                      FROM TBL_EDESC
                     WHERE EDESC LIKE @q ESCAPE '\'
                     ORDER BY EDESC;", conn)
                    cmd.Parameters.Add("@q", SqlDbType.NVarChar, 500).Value = "%" & EscapeLikeValue(term) & "%"
                    Using rdr = Await cmd.ExecuteReaderAsync(ct)
                        While Await rdr.ReadAsync(ct)
                            suggestions.Add(rdr.GetString(0))
                        End While
                    End Using
                End Using
            End Using
        Catch ex As OperationCanceledException
            Return
        Catch ex As Exception
            ' optional: log or show once
            Return
        End Try

        ' Update the dropdown list only; DO NOT overwrite user-typed text
        suppressTextChanged = True
        Dim caret As Integer = txtdesc.SelectionStart
        txtdesc.BeginUpdate()
        Try
            txtdesc.Items.Clear()
            If suggestions.Count > 0 Then
                txtdesc.Items.AddRange(suggestions.ToArray())
                txtdesc.SelectedIndex = -1 ' keep pure text, no auto-select
                ' restore the typed text BEFORE dropping down
                txtdesc.Text = term
                txtdesc.SelectionStart = Math.Min(caret, txtdesc.Text.Length)
                txtdesc.SelectionLength = 0

                ' Show the list
                txtdesc.DroppedDown = True

                ' Ensure the caret/cursor shows in the edit box:
                txtdesc.Focus()
                txtdesc.Cursor = Cursors.IBeam

                ' Re-assert selection AFTER the dropdown is shown (WinForms quirk)
                txtdesc.BeginInvoke(New Action(Sub()
                                                   Try
                                                       txtdesc.SelectionStart = Math.Min(caret, txtdesc.Text.Length)
                                                       txtdesc.SelectionLength = 0
                                                   Catch
                                                   End Try
                                               End Sub))
            Else
                ' No suggestions → hide list, keep caret in edit box
                txtdesc.Text = term
                txtdesc.SelectionStart = Math.Min(caret, txtdesc.Text.Length)
                txtdesc.SelectionLength = 0
                txtdesc.DroppedDown = False
                txtdesc.Focus()
            End If
        Finally
            txtdesc.EndUpdate()
            suppressTextChanged = False
        End Try
    End Function

    ' User picked a suggestion → set text and load details
    Private Async Sub txtdesc_SelectionChangeCommitted(sender As Object, e As EventArgs) Handles txtdesc.SelectionChangeCommitted
        If suppressTextChanged Then Return
        Dim chosen = TryCast(txtdesc.SelectedItem, String)
        If String.IsNullOrEmpty(chosen) Then Return

        suppressTextChanged = True
        txtdesc.Text = chosen
        txtdesc.DroppedDown = False
        suppressTextChanged = False

        Await LoadDescDetailsAsync(chosen)
    End Sub

    Private Async Function LoadDescDetailsAsync(desc As String) As Task
        If String.IsNullOrWhiteSpace(desc) Then Return
        Try
            Using conn As SqlConnection = GetTunedConnection()
                Await conn.OpenAsync()
                Using cmd As New SqlCommand("
                    SELECT UNIT, IMAGES 
                      FROM TBL_EDESC 
                     WHERE EDESC = @EDESC;", conn)
                    cmd.Parameters.Add("@EDESC", SqlDbType.NVarChar, 4000).Value = desc
                    Using rdr = Await cmd.ExecuteReaderAsync()
                        If Await rdr.ReadAsync() Then
                            combounit.Text = rdr("UNIT").ToString()
                            If Not IsDBNull(rdr("IMAGES")) Then
                                Dim imgBytes As Byte() = DirectCast(rdr("IMAGES"), Byte())
                                Using ms As New MemoryStream(imgBytes)
                                    PictureBox1.Image = Image.FromStream(ms)
                                End Using
                            Else
                                PictureBox1.Image = Nothing
                            End If
                        End If
                    End Using
                End Using
            End Using
        Catch
            ' soft-fail
        End Try
    End Function

    Private Sub txtdesc_Validating(sender As Object, e As CancelEventArgs) Handles txtdesc.Validating
        ' If you truly require a value selected from suggestions, keep this.
        ' Otherwise, comment out to allow free text.
        If txtdesc.FindStringExact(txtdesc.Text) = -1 Then
            e.Cancel = True
            MessageBox.Show("Please select a valid Description from the list.",
                            "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    Private Sub combounit_KeyPress(sender As Object, e As KeyPressEventArgs) Handles combounit.KeyPress
        ' lock typing if you want it strictly list-based
        e.Handled = True
    End Sub

    ' ───────────────────────────────────────────────
    ' Header totals (sync wrapper kept for old calls)
    ' ───────────────────────────────────────────────
    Public Sub UpdatePRTotals(ByVal prId As Integer)
        ' Keep back-compat: run async without blocking UI
        Me.BeginInvoke(Async Sub() Await UpdatePRTotalsAsync(prId))
    End Sub

    Private Sub frmpradd_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        Try
            If _suggestCts IsNot Nothing Then
                _suggestCts.Cancel()
                _suggestCts.Dispose()
            End If
        Catch
        End Try
        Me.Dispose()
    End Sub

    Private Sub Txtdesc_SelectedIndexChanged(sender As Object, e As EventArgs) Handles txtdesc.SelectedIndexChanged
        ' Intentionally unused — SelectionChangeCommitted is the one we want.
    End Sub

    ' ───────────────────────────────────────────────
    ' Tuned connection for speed/stability
    ' ───────────────────────────────────────────────
    Private Function GetTunedConnection() As SqlConnection
        Dim cs As String = frmmain.txtdb.Text
        Dim b As New SqlConnectionStringBuilder(cs)
        b.Pooling = True
        If b.MinPoolSize < 10 Then b.MinPoolSize = 10
        If b.MaxPoolSize < 100 Then b.MaxPoolSize = 100
        If b.ConnectTimeout > 5 Then b.ConnectTimeout = 5
        If b.PacketSize < 32767 Then b.PacketSize = 32767
        Return New SqlConnection(b.ConnectionString)
    End Function
End Class
