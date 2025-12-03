Imports System.Data
Imports System.Data.SqlClient
Imports System.Threading
Imports System.Threading.Tasks
Imports System.Drawing
Imports System.Linq

Public Class frmRIS

    '──────────────────────────────────────────────────────────────────────────────
    ' Paging / filters / async state
    '──────────────────────────────────────────────────────────────────────────────
    Private Const PageSize As Integer = 100
    Private _pageIndex As Integer = 1
    Private _totalRows As Integer = 0
    Private _totalPages As Integer = 1

    Private _lastSearch As String = ""
    Private _lastDiv As String = "All"
    Private _lastOffice As String = "All"
    Private _lastYear As String = "All"

    Private _suppressComboEvents As Boolean = False
    Private _cts As CancellationTokenSource

    '──────────────────────────────────────────────────────────────────────────────
    ' Retry policy for SQL (deadlocks/timeouts)
    '──────────────────────────────────────────────────────────────────────────────
    Private Const RETRIES As Integer = 3
    Private Shared ReadOnly RETRY_DELAYS_MS As Integer() = {120, 280, 600}
    Private Shared Function IsRetryableSql(ex As SqlException) As Boolean
        ' 1205 = deadlock victim, 1222 = lock timeout
        Return ex.Number = 1205 OrElse ex.Number = 1222
    End Function

    '──────────────────────────────────────────────────────────────────────────────
    ' Form load
    '──────────────────────────────────────────────────────────────────────────────
    Private Async Sub frmRIS_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            SetupGrid()
            Await LoadFiltersAsync()
            _pageIndex = 1
            Await RefreshGridAsync()
        Catch ex As Exception
            MessageBox.Show("Form Load Error: " & ex.Message, "RIS", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    '──────────────────────────────────────────────────────────────────────────────
    ' Event handlers (no lambdas → VB friendly)
    '──────────────────────────────────────────────────────────────────────────────
    Private Async Sub cmdsearch_Click(sender As Object, e As EventArgs) Handles cmdsearch.Click
        Await DoSearchAsync()
    End Sub

    Private Async Sub txtsearch_KeyDown(sender As Object, e As KeyEventArgs) Handles txtsearch.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            Await DoSearchAsync()
        End If
    End Sub

    Private Async Sub cmdrefresh_Click(sender As Object, e As EventArgs) Handles cmdrefresh.Click
        ResetFiltersToAll()
        _pageIndex = 1
        Await RefreshGridAsync()
    End Sub

    Private Async Sub combodiv_SelectedIndexChanged(sender As Object, e As EventArgs) Handles combodiv.SelectedIndexChanged
        If _suppressComboEvents Then Return
        _lastDiv = SafeSelected(combodiv)
        _pageIndex = 1
        Await RefreshGridAsync()
    End Sub

    Private Async Sub combooffsec_SelectedIndexChanged(sender As Object, e As EventArgs) Handles combooffsec.SelectedIndexChanged
        If _suppressComboEvents Then Return
        _lastOffice = SafeSelected(combooffsec)
        _pageIndex = 1
        Await RefreshGridAsync()
    End Sub

    Private Async Sub comboyear_SelectedIndexChanged(sender As Object, e As EventArgs) Handles Comboyear.SelectedIndexChanged
        If _suppressComboEvents Then Return
        _lastYear = SafeSelected(Comboyear)
        _pageIndex = 1
        Await RefreshGridAsync()
    End Sub

    Private Async Sub cmdprev_Click(sender As Object, e As EventArgs) Handles cmdprev.Click
        If _pageIndex > 1 Then
            _pageIndex -= 1
            Await RefreshGridAsync()
        End If
    End Sub

    Private Async Sub cmdnext_Click(sender As Object, e As EventArgs) Handles cmdnext.Click
        If _pageIndex < _totalPages Then
            _pageIndex += 1
            Await RefreshGridAsync()
        End If
    End Sub

    Private Async Function DoSearchAsync() As Task
        Dim q As String = If(txtsearch.Text, "").Trim()
        _lastSearch = q
        _pageIndex = 1
        Await RefreshGridAsync()
    End Function

    Private Function SafeSelected(cbo As ComboBox) As String
        If cbo Is Nothing OrElse cbo.SelectedItem Is Nothing Then Return "All"
        Dim s As String = cbo.SelectedItem.ToString().Trim()
        If s.Length = 0 Then Return "All"
        Return s
    End Function

    Private Sub ResetFiltersToAll()
        _suppressComboEvents = True
        Try
            If combodiv.Items.Count > 0 Then combodiv.SelectedIndex = 0
            If combooffsec.Items.Count > 0 Then combooffsec.SelectedIndex = 0
            If Comboyear.Items.Count > 0 Then Comboyear.SelectedIndex = 0
            txtsearch.Clear()
            _lastSearch = ""
            _lastDiv = "All"
            _lastOffice = "All"
            _lastYear = "All"
        Finally
            _suppressComboEvents = False
        End Try
    End Sub

    '──────────────────────────────────────────────────────────────────────────────
    ' Grid visual setup (matches PPE/SEP)
    '──────────────────────────────────────────────────────────────────────────────
    Private Sub SetupGrid()
        With DataGridView1
            .DataSource = Nothing
            .Rows.Clear()
            .Columns.Clear()
            .AutoGenerateColumns = False
            .RowHeadersVisible = False
            .AllowUserToAddRows = False
            .AllowUserToResizeRows = False

            ' ✔ allow multi-select and full-row select
            .MultiSelect = True
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect

            ' ✔ disable typing / editing in any cell
            .ReadOnly = True
            .EditMode = DataGridViewEditMode.EditProgrammatically

            ' Clean borders
            .BorderStyle = BorderStyle.None
            .CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
            .ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
            .AdvancedCellBorderStyle.Left = DataGridViewAdvancedCellBorderStyle.Single
            .AdvancedCellBorderStyle.Right = DataGridViewAdvancedCellBorderStyle.Single
            .AdvancedCellBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.Single
            .AdvancedCellBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.Single

            ' Fill mode (auto stretch across form)
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            .AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None
        End With

        ' Columns
        With DataGridView1
            .Columns.Add(New DataGridViewTextBoxColumn() With {
                .Name = "ID", .HeaderText = "ID",
                .DataPropertyName = "ID", .Visible = False
            })

            Dim colNo As New DataGridViewTextBoxColumn()
            colNo.Name = "RIS_NO"
            colNo.HeaderText = "RIS NO"
            colNo.DataPropertyName = "RIS_NO"
            colNo.FillWeight = 90
            .Columns.Add(colNo)

            Dim colDate As New DataGridViewTextBoxColumn()
            colDate.Name = "RIS_DATE"
            colDate.HeaderText = "RIS DATE"
            colDate.DataPropertyName = "RIS_DATE"
            colDate.DefaultCellStyle.Format = "MM/dd/yyyy"
            colDate.FillWeight = 80
            .Columns.Add(colDate)

            Dim colDiv As New DataGridViewTextBoxColumn()
            colDiv.Name = "RIS_DIVISION"
            colDiv.HeaderText = "DIVISION"
            colDiv.DataPropertyName = "RIS_DIVISION"
            colDiv.FillWeight = 130
            .Columns.Add(colDiv)

            Dim colOff As New DataGridViewTextBoxColumn()
            colOff.Name = "RIS_OFFICE"
            colOff.HeaderText = "OFFICE/SECTION"
            colOff.DataPropertyName = "RIS_OFFICE"
            colOff.FillWeight = 130
            .Columns.Add(colOff)

            Dim colF As New DataGridViewTextBoxColumn()
            colF.Name = "FCLUSTER"
            colF.HeaderText = "FUND CLUSTER"
            colF.DataPropertyName = "FCLUSTER"
            colF.FillWeight = 100
            .Columns.Add(colF)

            Dim colPurpose As New DataGridViewTextBoxColumn()
            colPurpose.Name = "PURPOSES"
            colPurpose.HeaderText = "PURPOSES"
            colPurpose.DataPropertyName = "PURPOSES"
            colPurpose.DefaultCellStyle.WrapMode = DataGridViewTriState.True
            colPurpose.FillWeight = 250
            .Columns.Add(colPurpose)
        End With

        ' Styling
        Dim hdrColor As Color = Color.FromArgb(73, 94, 252)
        With DataGridView1
            .EnableHeadersVisualStyles = False
            .ColumnHeadersDefaultCellStyle.BackColor = hdrColor
            .ColumnHeadersDefaultCellStyle.ForeColor = Color.White
            .ColumnHeadersDefaultCellStyle.SelectionBackColor = hdrColor
            .ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White
            .ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 10.0!, FontStyle.Bold)
            .ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            .ColumnHeadersHeight = 52
            .ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing

            .DefaultCellStyle.Font = New Font("Segoe UI", 9.0!)
            .DefaultCellStyle.ForeColor = Color.Black
            .RowsDefaultCellStyle.BackColor = Color.White
            .AlternatingRowsDefaultCellStyle.BackColor = Color.White
            .DefaultCellStyle.SelectionBackColor = Color.FromArgb(210, 222, 255)
            .DefaultCellStyle.SelectionForeColor = Color.Black
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft

            .RowTemplate.Height = 34
            .RowTemplate.DefaultCellStyle.Padding = New Padding(0, 6, 0, 6)
        End With
    End Sub

    '──────────────────────────────────────────────────────────────────────────────
    ' Filters
    '──────────────────────────────────────────────────────────────────────────────
    Private Async Function LoadFiltersAsync() As Task
        _suppressComboEvents = True
        Try
            Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
                Await conn.OpenAsync()

                ' DIVISION
                Using cmd As New SqlCommand("SELECT DISTINCT RIS_DIVISION FROM TBL_RIS WHERE ISNULL(RIS_DIVISION,'')<>'' ORDER BY RIS_DIVISION;", conn)
                    Using rd As SqlDataReader = Await cmd.ExecuteReaderAsync()
                        combodiv.Items.Clear()
                        combodiv.Items.Add("All")
                        While Await rd.ReadAsync()
                            combodiv.Items.Add(rd.GetString(0))
                        End While
                    End Using
                End Using

                ' OFFICE/SECTION
                Using cmd As New SqlCommand("SELECT DISTINCT RIS_OFFICE FROM TBL_RIS WHERE ISNULL(RIS_OFFICE,'')<>'' ORDER BY RIS_OFFICE;", conn)
                    Using rd As SqlDataReader = Await cmd.ExecuteReaderAsync()
                        combooffsec.Items.Clear()
                        combooffsec.Items.Add("All")
                        While Await rd.ReadAsync()
                            combooffsec.Items.Add(rd.GetString(0))
                        End While
                    End Using
                End Using

                ' YEAR
                Using cmd As New SqlCommand("SELECT DISTINCT YEAR(RIS_DATE) AS YY FROM TBL_RIS WHERE RIS_DATE IS NOT NULL ORDER BY YY DESC;", conn)
                    Using rd As SqlDataReader = Await cmd.ExecuteReaderAsync()
                        Comboyear.Items.Clear()
                        Comboyear.Items.Add("All")
                        While Await rd.ReadAsync()
                            Comboyear.Items.Add(rd.GetInt32(0).ToString())
                        End While
                    End Using
                End Using
            End Using

            If combodiv.Items.Count > 0 Then combodiv.SelectedIndex = 0
            If combooffsec.Items.Count > 0 Then combooffsec.SelectedIndex = 0
            If Comboyear.Items.Count > 0 Then Comboyear.SelectedIndex = 0
        Finally
            _suppressComboEvents = False
        End Try
    End Function

    '──────────────────────────────────────────────────────────────────────────────
    ' Data loading (async, paged, parameterized)
    '──────────────────────────────────────────────────────────────────────────────
    Private Sub CancelPending()
        Try
            If _cts IsNot Nothing Then _cts.Cancel()
        Catch
        End Try
        _cts = New CancellationTokenSource()
    End Sub

    Private Async Function RefreshGridAsync() As Task
        CancelPending()
        Dim tk As CancellationToken = _cts.Token

        ToggleLoadingUI(True)

        Try
            Dim whereSql As String = ""
            Dim paramList As List(Of SqlParameter) = Nothing
            BuildWhereClause(whereSql, paramList)

            Dim cntSql As String = "SELECT COUNT(1) FROM TBL_RIS WITH (NOLOCK) " & whereSql & ";"

            Dim dataSql As String =
                "SELECT ID, RIS_NO, RIS_DATE, RIS_DIVISION, RIS_OFFICE, FCLUSTER, PURPOSES " &
                "FROM TBL_RIS WITH (NOLOCK) " & whereSql & " " &
                "ORDER BY RIS_DATE DESC, ID DESC " &
                "OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;"

            Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
                Await conn.OpenAsync()

                ' Count
                Using cmdCnt As New SqlCommand(cntSql, conn)
                    AddParams(cmdCnt, paramList)
                    Dim o As Object = Await cmdCnt.ExecuteScalarAsync()
                    _totalRows = If(o Is Nothing OrElse o Is DBNull.Value, 0, Convert.ToInt32(o))
                End Using

                _totalPages = Math.Max(1, CInt(Math.Ceiling(_totalRows / CDbl(PageSize))))
                _pageIndex = Math.Max(1, Math.Min(_pageIndex, _totalPages))

                ' Data
                Using cmd As New SqlCommand(dataSql, conn)
                    AddParams(cmd, paramList)
                    cmd.Parameters.Add("@Offset", SqlDbType.Int).Value = (Math.Max(1, _pageIndex) - 1) * PageSize
                    cmd.Parameters.Add("@PageSize", SqlDbType.Int).Value = PageSize

                    Dim dt As New DataTable()
                    Using rd As SqlDataReader = Await cmd.ExecuteReaderAsync()
                        dt.Load(rd)
                    End Using

                    If tk.IsCancellationRequested Then Return
                    DataGridView1.DataSource = dt
                End Using
            End Using

            lblpage.Text = "Page " & _pageIndex & " / " & _totalPages & "  •  Rows: " & _totalRows.ToString("N0")
            cmdprev.Enabled = (_pageIndex > 1)
            cmdnext.Enabled = (_pageIndex < _totalPages)

        Catch ex As Exception
            If Not (TypeOf ex Is OperationCanceledException) Then
                MessageBox.Show("Load Error: " & ex.Message, "RIS", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        Finally
            ToggleLoadingUI(False)
        End Try
    End Function

    Private Sub BuildWhereClause(ByRef whereSql As String, ByRef parameters As List(Of SqlParameter))
        Dim parts As New List(Of String)()
        parameters = New List(Of SqlParameter)()

        If Not String.IsNullOrWhiteSpace(_lastSearch) Then
            parts.Add("(RIS_NO LIKE @q OR RIS_DIVISION LIKE @q OR RIS_OFFICE LIKE @q OR FCLUSTER LIKE @q OR PURPOSES LIKE @q)")
            Dim pQ As New SqlParameter("@q", SqlDbType.NVarChar, 4000)
            pQ.Value = "%" & _lastSearch & "%"
            parameters.Add(pQ)
        End If

        If Not String.Equals(_lastDiv, "All", StringComparison.OrdinalIgnoreCase) Then
            parts.Add("RIS_DIVISION = @div")
            Dim pDiv As New SqlParameter("@div", SqlDbType.NVarChar, 250)
            pDiv.Value = _lastDiv
            parameters.Add(pDiv)
        End If

        If Not String.Equals(_lastOffice, "All", StringComparison.OrdinalIgnoreCase) Then
            parts.Add("RIS_OFFICE = @off")
            Dim pOff As New SqlParameter("@off", SqlDbType.NVarChar, 250)
            pOff.Value = _lastOffice
            parameters.Add(pOff)
        End If

        If Not String.Equals(_lastYear, "All", StringComparison.OrdinalIgnoreCase) Then
            parts.Add("YEAR(RIS_DATE) = @yy")
            Dim pY As New SqlParameter("@yy", SqlDbType.Int)
            pY.Value = Integer.Parse(_lastYear)
            parameters.Add(pY)
        End If

        whereSql = If(parts.Count > 0, "WHERE " & String.Join(" AND ", parts), "")
    End Sub

    Private Sub AddParams(cmd As SqlCommand, src As List(Of SqlParameter))
        If src Is Nothing Then Exit Sub
        For Each p As SqlParameter In src
            Dim cp As New SqlParameter()
            cp.ParameterName = p.ParameterName
            cp.SqlDbType = p.SqlDbType
            cp.Size = p.Size
            cp.Direction = p.Direction
            cp.Value = If(p.Value, DBNull.Value)
            cmd.Parameters.Add(cp)
        Next
    End Sub

    Private Sub ToggleLoadingUI(isLoading As Boolean)
        If Me.IsDisposed Then Return
        cmdsearch.Enabled = Not isLoading
        cmdrefresh.Enabled = Not isLoading
        combodiv.Enabled = Not isLoading
        combooffsec.Enabled = Not isLoading
        Comboyear.Enabled = Not isLoading
        cmdprev.Enabled = Not isLoading AndAlso _pageIndex > 1
        cmdnext.Enabled = Not isLoading AndAlso _pageIndex < _totalPages
        Cursor = If(isLoading, Cursors.AppStarting, Cursors.Default)
    End Sub

    Private Sub cmdnew_Click(sender As Object, e As EventArgs) Handles cmdnew.Click
        ' Prevent double-click while creating the form
        cmdnew.Enabled = False

        Try
            ' If a frmrisfile is already open, just focus it (avoid duplicates)
            Dim opened = Application.OpenForms.OfType(Of frmrisfile)().FirstOrDefault()
            If opened IsNot Nothing Then
                opened.Activate()
                Return
            End If

            ' Open the RIS File form in "new" mode (frmrisfile handles init on Load)
            Using f As New frmrisfile()
                f.StartPosition = FormStartPosition.CenterParent
                f.ShowInTaskbar = False
                Me.Enabled = False
                f.ShowDialog(Me)
            End Using

        Catch ex As Exception
            MessageBox.Show("Unable to open RIS file: " & ex.Message, "New RIS", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Me.Enabled = True
            cmdnew.Enabled = True
        End Try
    End Sub

    Private Async Sub cmdedit_Click(sender As Object, e As EventArgs) Handles cmdedit.Click
        Try
            If DataGridView1.CurrentRow Is Nothing Then
                MessageBox.Show("Please select a RIS to edit.", "Edit RIS", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If

            Dim risNo As String = GetRisNoFromRow(DataGridView1.CurrentRow)
            If String.IsNullOrWhiteSpace(risNo) Then
                MessageBox.Show("Selected row has no RIS No.", "Edit RIS", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If

            Dim f As New frmrisfile()
            f.Show(Me)
            Await f.LoadExistingAsync(risNo)

        Catch ex As Exception
            MessageBox.Show("Open error: " & ex.Message, "Edit RIS", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    '──────────────────────────────────────────────────────────────────────────────
    ' DELETE RIS: Restock inventory, delete items, delete parent (transactional)
    '  -> Updated for multi-select, multi-delete
    '──────────────────────────────────────────────────────────────────────────────
    Private Async Sub cmddelete_Click(sender As Object, e As EventArgs) Handles cmddelete.Click
        Try
            If DataGridView1.SelectedRows Is Nothing OrElse DataGridView1.SelectedRows.Count = 0 Then
                MessageBox.Show("Please select at least one RIS to delete.", "Delete RIS", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If

            ' Collect distinct RIS numbers from selected rows
            Dim risList As New List(Of String)()
            For Each r As DataGridViewRow In DataGridView1.SelectedRows
                If r Is Nothing OrElse r.IsNewRow Then Continue For
                Dim risNo As String = GetRisNoFromRow(r)
                If Not String.IsNullOrWhiteSpace(risNo) AndAlso Not risList.Contains(risNo, StringComparer.OrdinalIgnoreCase) Then
                    risList.Add(risNo)
                End If
            Next

            If risList.Count = 0 Then
                MessageBox.Show("No valid RIS No found in the selected rows.", "Delete RIS", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If

            Dim msg As String =
                $"Delete {risList.Count} RIS record(s)?{Environment.NewLine}" &
                $"All their items will be removed and quantities returned to inventory."
            If MessageBox.Show(msg, "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                Return
            End If

            ' Perform deletions sequentially to reduce lock contention
            Dim ok As Integer = 0
            Dim fail As Integer = 0
            For Each ris In risList
                Try
                    Await DeleteRisAsync(ris)
                    ok += 1
                Catch ex As Exception
                    fail += 1
                End Try
            Next

            ' Refresh list after delete
            Await RefreshGridAsync()

            MessageBox.Show($"Delete completed.{Environment.NewLine}Success: {ok}{Environment.NewLine}Failed: {fail}",
                            "Delete RIS", MessageBoxButtons.OK, If(fail = 0, MessageBoxIcon.Information, MessageBoxIcon.Warning))

        Catch ex As Exception
            MessageBox.Show("Delete error: " & ex.Message, "Delete RIS", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Function GetRisNoFromRow(r As DataGridViewRow) As String
        If r Is Nothing Then Return ""
        If DataGridView1.Columns.Contains("RIS_NO") Then
            Return Convert.ToString(r.Cells("RIS_NO").Value)
        End If
        ' Fallback to index if needed
        If r.Cells.Count > 2 Then
            Return Convert.ToString(r.Cells(2).Value)
        End If
        Return ""
    End Function

    Private Async Function DeleteRisAsync(risNo As String) As Task
        Dim attempts As Integer = 0
        Do
            Dim shouldRetry As Boolean = False
            Try
                Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
                    Await conn.OpenAsync()

                    Using tran = conn.BeginTransaction(IsolationLevel.ReadCommitted)
                        Try
                            ' Lower deadlock priority; set lock timeout
                            Using setPrio As New SqlCommand("SET DEADLOCK_PRIORITY LOW; SET LOCK_TIMEOUT 5000;", conn, tran)
                                setPrio.CommandTimeout = 10
                                Await setPrio.ExecuteNonQueryAsync()
                            End Using

                            ' 1) Gather items and quantities for this RIS (lock rows)
                            Dim byIpno As New Dictionary(Of String, Decimal)(StringComparer.OrdinalIgnoreCase)
                            Using cmdItems As New SqlCommand("
                                SELECT STOCK_NO, QUANTITY
                                FROM TBL_RISITEMS WITH (UPDLOCK, ROWLOCK)
                                WHERE RIS_NO=@RIS_NO;", conn, tran)
                                cmdItems.Parameters.Add("@RIS_NO", SqlDbType.NVarChar, 50).Value = risNo
                                Using rd = Await cmdItems.ExecuteReaderAsync()
                                    While Await rd.ReadAsync()
                                        Dim ipno As String = If(rd.IsDBNull(0), "", rd.GetString(0))
                                        Dim qty As Decimal = If(rd.IsDBNull(1), 0D, Convert.ToDecimal(rd(1)))
                                        If String.IsNullOrWhiteSpace(ipno) OrElse qty = 0D Then Continue While
                                        If byIpno.ContainsKey(ipno) Then
                                            byIpno(ipno) += qty
                                        Else
                                            byIpno(ipno) = qty
                                        End If
                                    End While
                                End Using
                            End Using

                            ' 2) Restock inventory
                            For Each kv In byIpno
                                Using cmdInv As New SqlCommand("
                                    UPDATE TBL_INVENTORY WITH (ROWLOCK, UPDLOCK)
                                    SET QTY = QTY + @QTY
                                    WHERE IPNO=@IPNO;", conn, tran)
                                    cmdInv.CommandTimeout = 30
                                    cmdInv.Parameters.Add("@QTY", SqlDbType.Decimal).Value = kv.Value
                                    cmdInv.Parameters.Add("@IPNO", SqlDbType.NVarChar, 250).Value = kv.Key
                                    Await cmdInv.ExecuteNonQueryAsync()
                                End Using
                            Next

                            ' 3) Delete child items
                            Using cmdDelItems As New SqlCommand("
                                DELETE FROM TBL_RISITEMS WHERE RIS_NO=@RIS_NO;", conn, tran)
                                cmdDelItems.CommandTimeout = 30
                                cmdDelItems.Parameters.Add("@RIS_NO", SqlDbType.NVarChar, 50).Value = risNo
                                Await cmdDelItems.ExecuteNonQueryAsync()
                            End Using

                            ' 4) Delete parent
                            Using cmdDelParent As New SqlCommand("
                                DELETE FROM TBL_RIS WHERE RIS_NO=@RIS_NO;", conn, tran)
                                cmdDelParent.CommandTimeout = 30
                                cmdDelParent.Parameters.Add("@RIS_NO", SqlDbType.NVarChar, 50).Value = risNo
                                Dim n = Await cmdDelParent.ExecuteNonQueryAsync()
                                If n = 0 Then
                                    Throw New ApplicationException("RIS not found or already deleted.")
                                End If
                            End Using

                            tran.Commit()
                        Catch
                            Try : tran.Rollback() : Catch : End Try
                            Throw
                        End Try
                    End Using
                End Using

                Return ' success

            Catch ex As SqlException
                If IsRetryableSql(ex) AndAlso attempts < RETRIES Then
                    shouldRetry = True
                Else
                    Throw
                End If
            End Try

            If shouldRetry Then
                Await Task.Delay(RETRY_DELAYS_MS(attempts))
                attempts += 1
            End If
        Loop While attempts <= RETRIES
    End Function

End Class
