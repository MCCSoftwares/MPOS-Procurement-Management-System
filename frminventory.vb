Imports System.Windows.Forms
Imports System.Data.SqlClient
Imports System.Threading
Imports System.Threading.Tasks
Imports System.Collections.Generic
Imports System.Linq

Public Class frminventory
    Private _isDeleting As Boolean = False
    Private _confirmingDelete As Boolean = False
    ' ───────────────────────────────────────────────
    ' Paging & state
    ' ───────────────────────────────────────────────
    Private Const PageSize As Integer = 50
    Private _currentPage As Integer = 1
    Private _totalRows As Integer = 0
    Private _totalPages As Integer = 1

    ' Search / filter state
    Private _searchTerm As String = ""
    Private _ptypeFilter As String = ""     ' empty = all
    Private _statusFilter As String = ""    ' empty = all

    ' Async infra
    Private _cts As CancellationTokenSource = New CancellationTokenSource()
    Private WithEvents _searchTimer As System.Windows.Forms.Timer
    Private _suppressSearch As Boolean = False

    ' COUNT(*) tiny cache (per filter key) to reduce chatter
    Private Structure CountEntry
        Public Count As Integer
        Public AsOf As DateTime
    End Structure
    Private ReadOnly _countCache As New Dictionary(Of String, CountEntry)(StringComparer.OrdinalIgnoreCase)

    ' ───────────────────────────────────────────────
    ' ATITLE/ACODE cache & flags (ADDED)
    ' ───────────────────────────────────────────────
    Private _accountTitles As New List(Of String)()
    Private _titleToCode As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
    Private _suppressGridEvents As Boolean = False

    ' Column name constants
    Private Const COL_ID As String = "ID"
    Private Const COL_PTYPE As String = "PTYPE"
    Private Const COL_DESC As String = "DESCRIPTIONS"
    Private Const COL_ATITLE As String = "ATITLE"
    Private Const COL_ACODE As String = "ACODE"
    Private Const COL_IPNO As String = "IPNO"
    Private Const COL_UNITS As String = "UNITS"
    Private Const COL_QTY As String = "QTY"
    Private Const COL_STATUS As String = "STATUS"
    Private Const COL_REMARKS As String = "REMARKS"

    ' ───────────────────────────────────────────────
    ' Utility: escape LIKE wildcards
    ' ───────────────────────────────────────────────
    Private Function EscapeLikeValue(value As String) As String
        If String.IsNullOrEmpty(value) Then Return ""
        Return value.Replace("\", "\\").Replace("%", "\%").Replace("_", "\_")
    End Function

    ' ───────────────────────────────────────────────
    ' Setup grid
    ' ───────────────────────────────────────────────
    Private Sub SetupInventoryGrid()
        With DataGridView1
            .DataSource = Nothing
            .Rows.Clear()
            .Columns.Clear()
            .AutoGenerateColumns = False
            .RowHeadersVisible = False
            .AllowUserToAddRows = False
            .AllowUserToResizeRows = False
            .MultiSelect = True
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
            .ReadOnly = False
            .EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2

            .BorderStyle = BorderStyle.None
            .CellBorderStyle = DataGridViewCellBorderStyle.None
            .ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
            .AdvancedCellBorderStyle.Left = DataGridViewAdvancedCellBorderStyle.None
            .AdvancedCellBorderStyle.Right = DataGridViewAdvancedCellBorderStyle.None
            .AdvancedCellBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.None
            .AdvancedCellBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.None

            .EnableHeadersVisualStyles = False

            Dim hdrColor As Color = Color.FromArgb(73, 94, 252)
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

            .RowTemplate.Height = 34
            .RowTemplate.DefaultCellStyle.Padding = New Padding(0, 6, 0, 6)
        End With

        ' Your original index-based column setup
        DataGridView1.ColumnCount = 10
        With DataGridView1
            .Columns(0).Name = COL_ID : .Columns(0).HeaderText = "ID" : .Columns(0).DataPropertyName = COL_ID : .Columns(0).Visible = False : .Columns(0).ReadOnly = True
            .Columns(1).Name = COL_PTYPE : .Columns(1).HeaderText = "TYPE" : .Columns(1).DataPropertyName = COL_PTYPE
            .Columns(2).Name = COL_DESC : .Columns(2).HeaderText = "DESCRIPTIONS" : .Columns(2).DataPropertyName = COL_DESC : .Columns(2).DefaultCellStyle.WrapMode = DataGridViewTriState.True
            .Columns(3).Name = COL_ATITLE : .Columns(3).HeaderText = "ACCOUNT TITLE" : .Columns(3).DataPropertyName = COL_ATITLE
            .Columns(4).Name = COL_ACODE : .Columns(4).HeaderText = "ACCOUNT CODE" : .Columns(4).DataPropertyName = COL_ACODE
            .Columns(5).Name = COL_IPNO : .Columns(5).HeaderText = "ITEM/PROPERTY NO." : .Columns(5).DataPropertyName = COL_IPNO
            .Columns(6).Name = COL_UNITS : .Columns(6).HeaderText = "UNIT" : .Columns(6).DataPropertyName = COL_UNITS
            .Columns(7).Name = COL_QTY : .Columns(7).HeaderText = "QTY" : .Columns(7).DataPropertyName = COL_QTY : .Columns(7).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            ' IMPORTANT: no .Format = "N2" here; we format dynamically in CellFormatting
            .Columns(8).Name = COL_STATUS : .Columns(8).HeaderText = "STOCK STATUS" : .Columns(8).DataPropertyName = COL_STATUS
            .Columns(9).Name = COL_REMARKS : .Columns(9).HeaderText = "REMARKS" : .Columns(9).DataPropertyName = COL_REMARKS : .Columns(9).DefaultCellStyle.WrapMode = DataGridViewTriState.True
        End With

        ' Replace ATITLE text column with a ComboBox column (same position)
        If DataGridView1.Columns.Contains(COL_ATITLE) Then
            Dim idx As Integer = DataGridView1.Columns(COL_ATITLE).Index
            DataGridView1.Columns.RemoveAt(idx)
            Dim atitleCol As New DataGridViewComboBoxColumn With {
                .Name = COL_ATITLE,
                .HeaderText = "ACCOUNT TITLE",
                .DataPropertyName = COL_ATITLE,
                .FlatStyle = FlatStyle.Flat,
                .DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton
            }
            DataGridView1.Columns.Insert(idx, atitleCol)
        End If

        ' Ensure editable columns are not read-only
        DataGridView1.Columns(COL_ATITLE).ReadOnly = False
        DataGridView1.Columns(COL_ACODE).ReadOnly = False
        DataGridView1.Columns(COL_REMARKS).ReadOnly = False
    End Sub

    ' ───────────────────────────────────────────────
    ' Form load
    ' ───────────────────────────────────────────────
    Private Async Sub frminventory_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            SetupInventoryGrid()
            _searchTimer = New System.Windows.Forms.Timer() With {.Interval = 350}

            ' Update statuses based on QTY (Out of Stock / In Stock)
            Await EnsureInventoryStatusesAsync()

            ' Load ATITLE list and bind to Combo
            Await LoadAccountTitlesAsync()
            BindATitleComboItems()

            ' Load combo filters (DISTINCT values)
            Await LoadFilterCombosAsync()

            ' First page
            Await ResetAndLoadAsync()
        Catch ex As Exception
            MessageBox.Show("Inventory Load Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ───────────────────────────────────────────────
    ' Build WHERE + parameters for current filters
    ' ───────────────────────────────────────────────
    Private Function BuildWhereClauseAndParams(cmd As SqlCommand, Optional forCount As Boolean = False) As String
        Dim clauses As New List(Of String)

        If Not String.IsNullOrWhiteSpace(_searchTerm) Then
            Dim q As String = "%" & EscapeLikeValue(_searchTerm.Trim()) & "%"
            cmd.Parameters.Add("@q", SqlDbType.NVarChar, 4000).Value = q
            clauses.Add(
                "(PTYPE LIKE @q ESCAPE '\' OR " &
                " DESCRIPTIONS LIKE @q ESCAPE '\' OR " &
                " ATITLE LIKE @q ESCAPE '\' OR " &
                " ACODE LIKE @q ESCAPE '\' OR " &
                " IPNO LIKE @q ESCAPE '\' OR " &
                " UNITS LIKE @q ESCAPE '\' OR " &
                " STATUS LIKE @q ESCAPE '\' OR " &
                " REMARKS LIKE @q ESCAPE '\')"
            )
        End If

        If Not String.IsNullOrWhiteSpace(_ptypeFilter) Then
            cmd.Parameters.Add("@ptype", SqlDbType.NVarChar, 200).Value = _ptypeFilter
            clauses.Add("PTYPE = @ptype")
        End If

        If Not String.IsNullOrWhiteSpace(_statusFilter) Then
            cmd.Parameters.Add("@status", SqlDbType.NVarChar, 200).Value = _statusFilter
            clauses.Add("STATUS = @status")
        End If

        If clauses.Count = 0 Then Return String.Empty
        Return " WHERE " & String.Join(" AND ", clauses)
    End Function

    ' Make a key for the count cache
    Private Function CurrentFilterKey() As String
        Return $"{_searchTerm}||PTYPE={_ptypeFilter}||STATUS={_statusFilter}"
    End Function

    ' ───────────────────────────────────────────────
    ' Main reset (page 1 + reload)
    ' ───────────────────────────────────────────────
    Private Async Function ResetAndLoadAsync() As Task
        Try
            If _cts IsNot Nothing Then
                _cts.Cancel()
                _cts.Dispose()
            End If
        Catch
        End Try
        _cts = New CancellationTokenSource()

        _currentPage = 1
        Await RefreshTotalRowsAsync(_cts.Token)
        Await LoadPageAsync(_cts.Token)
        UpdatePagerUI()
    End Function

    Private Async Function RefreshTotalRowsAsync(ct As CancellationToken) As Task
        Dim cacheKey As String = CurrentFilterKey()
        Dim entry As CountEntry

        ' 5s TTL
        If _countCache.TryGetValue(cacheKey, entry) Then
            If (DateTime.UtcNow - entry.AsOf) < TimeSpan.FromSeconds(5) Then
                _totalRows = entry.Count
                _totalPages = Math.Max(1, CInt(Math.Ceiling(_totalRows / CDbl(PageSize))))
                Exit Function
            End If
        End If

        Using conn As SqlConnection = GetTunedConnection(),
              cmd As New SqlCommand("", conn)
            cmd.CommandTimeout = 30

            Dim whereSql As String = BuildWhereClauseAndParams(cmd, True)
            cmd.CommandText = "SELECT COUNT(*) FROM TBL_INVENTORY" & whereSql & ";"

            Await conn.OpenAsync(ct)
            Dim obj = Await cmd.ExecuteScalarAsync(ct)
            _totalRows = If(obj Is Nothing OrElse obj Is DBNull.Value, 0, Convert.ToInt32(obj))
        End Using

        _totalPages = Math.Max(1, CInt(Math.Ceiling(_totalRows / CDbl(PageSize))))
        _countCache(cacheKey) = New CountEntry With {.Count = _totalRows, .AsOf = DateTime.UtcNow}
    End Function

    Public Async Function LoadPageAsync(ct As CancellationToken) As Task
        Dim offset As Integer = (Math.Max(1, _currentPage) - 1) * PageSize

        Try
            If lblPageInfo IsNot Nothing Then lblPageInfo.Text = "Loading..."
            If btnNext IsNot Nothing Then btnNext.Enabled = False
            If btnPrev IsNot Nothing Then btnPrev.Enabled = False

            Using conn As SqlConnection = GetTunedConnection(),
                  cmd As New SqlCommand("", conn)
                cmd.CommandTimeout = 30

                Dim whereSql As String = BuildWhereClauseAndParams(cmd, False)
                cmd.CommandText =
                    "SELECT ID, PTYPE, DESCRIPTIONS, ATITLE, ACODE, IPNO, UNITS, QTY, STATUS, REMARKS " &
                    "FROM TBL_INVENTORY " & whereSql & " " &
                    "ORDER BY ID DESC " &
                    "OFFSET @off ROWS FETCH NEXT @ps ROWS ONLY;"

                cmd.Parameters.Add("@off", SqlDbType.Int).Value = offset
                cmd.Parameters.Add("@ps", SqlDbType.Int).Value = PageSize

                Await conn.OpenAsync(ct)
                Using rdr = Await cmd.ExecuteReaderAsync(ct)
                    Dim dt As New DataTable()
                    dt.Load(rdr)
                    _suppressGridEvents = True
                    DataGridView1.DataSource = dt
                    _suppressGridEvents = False
                End Using
            End Using

            ' Ensure ATITLE Combo has list
            BindATitleComboItems()

            ' Reinforce editability after bind
            DataGridView1.ReadOnly = False
            If DataGridView1.Columns.Contains(COL_ID) Then DataGridView1.Columns(COL_ID).ReadOnly = True
            If DataGridView1.Columns.Contains(COL_ATITLE) Then DataGridView1.Columns(COL_ATITLE).ReadOnly = False
            If DataGridView1.Columns.Contains(COL_ACODE) Then DataGridView1.Columns(COL_ACODE).ReadOnly = False
            If DataGridView1.Columns.Contains(COL_REMARKS) Then DataGridView1.Columns(COL_REMARKS).ReadOnly = False

            ' Apply row colors based on STATUS
            ApplyRowColors()

        Catch ex As OperationCanceledException
        Catch ex As Exception
            MessageBox.Show("LoadInventory Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            UpdatePagerUI()
        End Try
    End Function

    ' ───────────────────────────────────────────────
    ' Pager UI
    ' ───────────────────────────────────────────────
    Public Sub UpdatePagerUI()
        If _totalPages < 1 Then _totalPages = 1
        If _currentPage < 1 Then _currentPage = 1
        If _currentPage > _totalPages Then _currentPage = _totalPages

        If lblPageInfo IsNot Nothing Then
            lblPageInfo.Text = $"Page {_currentPage} of {_totalPages}  •  Rows: {_totalRows}  •  PageSize: {PageSize}"
        End If

        If btnPrev IsNot Nothing Then btnPrev.Enabled = (_currentPage > 1)
        If btnNext IsNot Nothing Then btnNext.Enabled = (_currentPage < _totalPages)
    End Sub

    Private Async Sub btnPrev_Click(sender As Object, e As EventArgs) Handles btnPrev.Click
        If _currentPage > 1 Then
            _currentPage -= 1
            Try
                If _cts IsNot Nothing Then
                    _cts.Cancel()
                    _cts.Dispose()
                End If
            Catch
            End Try
            _cts = New CancellationTokenSource()
            Await LoadPageAsync(_cts.Token)
        End If
    End Sub

    Private Async Sub btnNext_Click(sender As Object, e As EventArgs) Handles btnNext.Click
        If _currentPage < _totalPages Then
            _currentPage += 1
            Try
                If _cts IsNot Nothing Then
                    _cts.Cancel()
                    _cts.Dispose()
                End If
            Catch
            End Try
            _cts = New CancellationTokenSource()
            Await LoadPageAsync(_cts.Token)
        End If
    End Sub

    ' ───────────────────────────────────────────────
    ' Debounced search (typing in txtsearch)
    ' ───────────────────────────────────────────────
    Private Sub txtsearch_TextChanged(sender As Object, e As EventArgs) Handles txtsearch.TextChanged
        If _suppressSearch Then Return
        If _searchTimer Is Nothing Then Return
        _searchTimer.Stop()
        _searchTimer.Start()
    End Sub

    Private Async Sub _searchTimer_Tick(sender As Object, e As EventArgs) Handles _searchTimer.Tick
        _searchTimer.Stop()
        _searchTerm = txtsearch.Text.Trim()
        Await ResetAndLoadAsync()
    End Sub

    Private Async Sub txtsearch_KeyDown(sender As Object, e As KeyEventArgs) Handles txtsearch.KeyDown
        Try
            If e.KeyCode = Keys.Enter Then
                e.SuppressKeyPress = True
                _searchTimer.Stop()
                _searchTerm = txtsearch.Text.Trim()
                Await ResetAndLoadAsync()
            ElseIf e.KeyCode = Keys.Escape Then
                e.SuppressKeyPress = True
                If _searchTimer IsNot Nothing Then _searchTimer.Stop()
                _suppressSearch = True
                Try
                    txtsearch.Clear()
                Finally
                    _suppressSearch = False
                End Try
                _searchTerm = ""
                Await ResetAndLoadAsync()
            End If
        Catch ex As Exception
            MessageBox.Show("Search KeyDown Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ───────────────────────────────────────────────
    ' NEW: cmdsearch & cmdrefresh
    ' ───────────────────────────────────────────────
    Private Async Sub cmdsearch_Click(sender As Object, e As EventArgs) Handles cmdsearch.Click
        _searchTerm = txtsearch.Text.Trim()
        _ptypeFilter = SafeComboText(comboptype)
        _statusFilter = SafeComboText(combostatus)
        Await ResetAndLoadAsync()
    End Sub

    Private Async Sub cmdrefresh_Click(sender As Object, e As EventArgs) Handles cmdrefresh.Click
        Try
            If _searchTimer IsNot Nothing Then _searchTimer.Stop()

            _suppressSearch = True
            Try
                txtsearch.Clear()
                If comboptype IsNot Nothing AndAlso comboptype.Items.Count > 0 Then comboptype.SelectedIndex = 0
                If combostatus IsNot Nothing AndAlso combostatus.Items.Count > 0 Then combostatus.SelectedIndex = 0
            Finally
                _suppressSearch = False
            End Try

            _searchTerm = ""
            _ptypeFilter = ""
            _statusFilter = ""

            ' Update statuses before reloading the grid
            Await EnsureInventoryStatusesAsync()

            Await ResetAndLoadAsync()
        Catch ex As Exception
            MessageBox.Show("Refresh error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' Optional: react immediately when user changes a filter dropdown
    Private Async Sub comboptype_SelectionChangeCommitted(sender As Object, e As EventArgs) Handles comboptype.SelectionChangeCommitted
        _ptypeFilter = SafeComboText(comboptype)
        Await ResetAndLoadAsync()
    End Sub

    Private Async Sub combostatus_SelectionChangeCommitted(sender As Object, e As EventArgs) Handles combostatus.SelectionChangeCommitted
        _statusFilter = SafeComboText(combostatus)
        Await ResetAndLoadAsync()
    End Sub

    Private Function SafeComboText(cb As ComboBox) As String
        If cb Is Nothing Then Return ""
        Dim t As String = If(cb.SelectedItem, "").ToString().Trim()
        If t.Equals("(All)", StringComparison.OrdinalIgnoreCase) Then Return ""
        Return t
    End Function

    ' ───────────────────────────────────────────────
    ' Load distinct values into filters
    ' ───────────────────────────────────────────────
    Private Async Function LoadFilterCombosAsync() As Task
        Try
            Using conn As SqlConnection = GetTunedConnection()
                Await conn.OpenAsync()

                ' PTYPE
                Using cmd As New SqlCommand("
                    SELECT DISTINCT LTRIM(RTRIM(PTYPE)) AS PTYPE
                    FROM TBL_INVENTORY
                    WHERE PTYPE IS NOT NULL AND LTRIM(RTRIM(PTYPE)) <> ''
                    ORDER BY PTYPE;", conn)
                    Using rdr = Await cmd.ExecuteReaderAsync()
                        Dim items As New List(Of String) From {"(All)"}
                        While Await rdr.ReadAsync()
                            items.Add(rdr("PTYPE").ToString())
                        End While
                        comboptype.Items.Clear()
                        comboptype.Items.AddRange(items.ToArray())
                        If comboptype.Items.Count > 0 Then comboptype.SelectedIndex = 0
                    End Using
                End Using

                ' STATUS
                Using cmd As New SqlCommand("
                    SELECT DISTINCT LTRIM(RTRIM(STATUS)) AS STATUS
                    FROM TBL_INVENTORY
                    WHERE STATUS IS NOT NULL AND LTRIM(RTRIM(STATUS)) <> ''
                    ORDER BY STATUS;", conn)
                    Using rdr = Await cmd.ExecuteReaderAsync()
                        Dim items As New List(Of String) From {"(All)"}
                        While Await rdr.ReadAsync()
                            items.Add(rdr("STATUS").ToString())
                        End While
                        combostatus.Items.Clear()
                        combostatus.Items.AddRange(items.ToArray())
                        If combostatus.Items.Count > 0 Then combostatus.SelectedIndex = 0
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Loading filter values failed: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Function

    ' ───────────────────────────────────────────────
    ' Delete (async, quick page reload)
    ' ───────────────────────────────────────────────
    Private Async Sub cmddelete_Click(sender As Object, e As EventArgs) Handles cmddelete.Click
        ' Re-entrancy guards: block while confirming or deleting
        If _confirmingDelete OrElse _isDeleting Then Return

        ' Collect unique selected IDs once
        Dim ids As New List(Of Integer)()
        For Each r As DataGridViewRow In DataGridView1.SelectedRows
            If r IsNot Nothing AndAlso Not r.IsNewRow Then
                Dim cell = r.Cells(COL_ID)?.Value
                Dim id As Integer
                If cell IsNot Nothing AndAlso Integer.TryParse(cell.ToString(), id) Then
                    If Not ids.Contains(id) Then ids.Add(id)
                End If
            End If
        Next

        If ids.Count = 0 Then
            MessageBox.Show("Please select at least one record to delete.", "Delete", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        ' Show confirmation ONCE
        Dim proceed As Boolean = False
        _confirmingDelete = True
        Try
            Dim prompt As String = If(ids.Count = 1,
                                  "Are you sure you want to delete this record?",
                                  $"Are you sure you want to delete {ids.Count} selected records?")
            Dim res = MessageBox.Show(prompt, "Confirm Delete",
                                  MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                                  MessageBoxDefaultButton.Button2)
            proceed = (res = DialogResult.Yes)
        Finally
            _confirmingDelete = False
        End Try

        If Not proceed Then Return

        ' Do the delete once
        _isDeleting = True
        Try
            Using conn As SqlConnection = GetTunedConnection()
                Await conn.OpenAsync()
                Using tran = conn.BeginTransaction()
                    Using cmd As SqlCommand = conn.CreateCommand()
                        cmd.Transaction = tran
                        cmd.CommandTimeout = 30

                        ' Parameterized IN list
                        Dim pnames As New List(Of String)
                        For i As Integer = 0 To ids.Count - 1
                            Dim p As String = "@p" & i.ToString()
                            pnames.Add(p)
                            cmd.Parameters.Add(p, SqlDbType.Int).Value = ids(i)
                        Next
                        cmd.CommandText = "DELETE FROM TBL_INVENTORY WHERE ID IN (" & String.Join(",", pnames) & ");"
                        Await cmd.ExecuteNonQueryAsync()
                    End Using
                    tran.Commit()
                End Using
            End Using

            ' Reload current page quickly (no full recount)
            Try
                If _cts IsNot Nothing Then
                    _cts.Cancel()
                    _cts.Dispose()
                End If
            Catch
            End Try
            _cts = New CancellationTokenSource()
            Await LoadPageAsync(_cts.Token)
            UpdatePagerUI()

        Catch ex As Exception
            MessageBox.Show("Delete failed: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            _isDeleting = False
        End Try
    End Sub

    ' ───────────────────────────────────────────────
    ' Edit flow (unchanged UX, no auto-reload)
    ' ───────────────────────────────────────────────
    Private Sub cmdedit_Click(sender As Object, e As EventArgs)

    End Sub

    Private Function SafeCell(row As DataGridViewRow, colName As String) As String
        Dim v = If(row.Cells(colName)?.Value, Nothing)
        Return If(v Is Nothing OrElse v Is DBNull.Value, "", v.ToString())
    End Function

    Private Function ToInt(s As String) As Integer
        Dim n As Integer
        Integer.TryParse(If(s, "0"), n)
        Return n
    End Function

    ' ───────────────────────────────────────────────
    ' Tuned connection (pooling, packet size, timeout)
    ' ───────────────────────────────────────────────
    Private Function GetTunedConnection() As SqlConnection
        Dim cs As String = My.Forms.frmmain.txtdb.Text
        Dim b As New SqlConnectionStringBuilder(cs)
        b.Pooling = True
        If b.MinPoolSize < 10 Then b.MinPoolSize = 10
        If b.MaxPoolSize < 100 Then b.MaxPoolSize = 100
        If b.ConnectTimeout > 5 Then b.ConnectTimeout = 5
        If b.PacketSize < 32767 Then b.PacketSize = 32767
        Return New SqlConnection(b.ConnectionString)
    End Function

    ' ───────────────────────────────────────────────
    ' Your menu hider (unchanged)
    ' ───────────────────────────────────────────────
    Public Class MenuHideMessageFilter
        Implements IMessageFilter

        Private ReadOnly _menu As Control
        Private ReadOnly _toggle As Control

        Public Sub New(menu As Control, toggle As Control)
            _menu = menu
            _toggle = toggle
        End Sub

        Public Function PreFilterMessage(ByRef m As Message) As Boolean Implements IMessageFilter.PreFilterMessage
            Try
                Const WM_LBUTTONDOWN As Integer = &H201
                Const WM_RBUTTONDOWN As Integer = &H204

                If m.Msg = WM_LBUTTONDOWN OrElse m.Msg = WM_RBUTTONDOWN Then
                    Dim target = Control.FromHandle(m.HWnd)
                    Dim lp = m.LParam.ToInt32()
                    Dim x As Integer = CShort(lp And &HFFFF)
                    Dim y As Integer = CShort((lp >> 16) And &HFFFF)
                    Dim localPt As New Point(x, y)
                    Dim screenPt As Point = If(target IsNot Nothing, target.PointToScreen(localPt), Cursor.Position)

                    If _menu.Visible Then
                        Dim menuRect = _menu.RectangleToScreen(_menu.ClientRectangle)
                        Dim toggleRect = _toggle.RectangleToScreen(_toggle.ClientRectangle)

                        If Not menuRect.Contains(screenPt) AndAlso Not toggleRect.Contains(screenPt) Then
                            _menu.Visible = False
                        End If
                    End If
                End If

                Return False
            Catch ex As Exception

            End Try

        End Function
    End Class

    Private _hideFilter As IMessageFilter

    Private Sub frminventory_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        If _hideFilter IsNot Nothing Then
            Application.RemoveMessageFilter(_hideFilter)
            _hideFilter = Nothing
        End If
        Try
            If _cts IsNot Nothing Then
                _cts.Cancel()
                _cts.Dispose()
            End If
        Catch
        End Try
    End Sub

    ' ───────────────────────────────────────────────
    ' Other buttons
    ' ───────────────────────────────────────────────
    Private Sub Cmdnew_Click(sender As Object, e As EventArgs) Handles cmdnew.Click
        Try
            frminventoryfind.Dispose()
            frminventoryfind.ShowDialog(Me)
            ' No auto reload—use Search/Refresh instead
        Catch
        End Try
    End Sub


    ' ======================================================================
    '               NEW: Auto Out-of-Stock & QTY Formatting
    ' ======================================================================

    ' Update STATUS based on QTY (called on Load and Refresh)
    Private Async Function EnsureInventoryStatusesAsync() As Task
        Try
            Using conn As SqlConnection = GetTunedConnection(),
                  cmd As New SqlCommand("
                -- Set Out of Stock for zero/negative quantities
                UPDATE TBL_INVENTORY
                   SET STATUS = 'Out of Stock'
                 WHERE ISNULL(QTY, 0) <= 0
                   AND (STATUS IS NULL OR STATUS <> 'Out of Stock');

                -- Normalize In Stock where quantity is positive
                UPDATE TBL_INVENTORY
                   SET STATUS = 'In Stock'
                 WHERE ISNULL(QTY, 0) > 0
                   AND (STATUS IS NULL OR STATUS <> 'In Stock');", conn)
                cmd.CommandTimeout = 30
                Await conn.OpenAsync()
                Await cmd.ExecuteNonQueryAsync()
            End Using
        Catch ex As Exception
            MessageBox.Show("Auto status update failed: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Function

    ' Format QTY: show integer with no decimals if whole, else keep decimals (up to 2)
    Private Sub DataGridView1_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles DataGridView1.CellFormatting
        Try
            If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Return
            Dim colName = DataGridView1.Columns(e.ColumnIndex).Name
            If colName = COL_QTY AndAlso e.Value IsNot Nothing AndAlso e.Value IsNot DBNull.Value Then
                Dim decVal As Decimal
                If Decimal.TryParse(e.Value.ToString(), decVal) Then
                    If decVal = Math.Truncate(decVal) Then
                        ' Whole number: no decimals
                        e.Value = decVal.ToString("0")
                    Else
                        ' Fractional: keep up to 2 decimals as-is
                        e.Value = decVal.ToString("0.##")
                    End If
                    e.FormattingApplied = True
                End If
            End If
        Catch
            ' Ignore format errors gracefully
        End Try
    End Sub

    ' Apply colors after binding completes too
    Private Sub DataGridView1_DataBindingComplete(sender As Object, e As DataGridViewBindingCompleteEventArgs) Handles DataGridView1.DataBindingComplete
        ApplyRowColors()
    End Sub

    ' Gracefully ignore combo data errors when typed value isn't in list
    Private Sub DataGridView1_DataError(sender As Object, e As DataGridViewDataErrorEventArgs) Handles DataGridView1.DataError
        e.ThrowException = False
    End Sub

    ' ── Row color helpers ───────────────────────────────────────────────────────
    Private Sub ApplyRowColors()
        For Each r As DataGridViewRow In DataGridView1.Rows
            ApplyRowColorForRow(r)
        Next
    End Sub

    Private Sub ApplyRowColorForRow(r As DataGridViewRow)
        If r Is Nothing OrElse r.IsNewRow Then Return
        Dim statusText As String = If(r.Cells(COL_STATUS)?.Value, "").ToString().Trim()
        If statusText.Equals("Out of Stock", StringComparison.OrdinalIgnoreCase) Then
            r.DefaultCellStyle.BackColor = Color.Salmon
            r.DefaultCellStyle.SelectionBackColor = Color.Salmon
            r.DefaultCellStyle.SelectionForeColor = Color.Black
        Else
            r.DefaultCellStyle.BackColor = Color.White
            r.DefaultCellStyle.SelectionBackColor = Color.FromArgb(210, 222, 255)
            r.DefaultCellStyle.SelectionForeColor = Color.Black
        End If
    End Sub

    ' ======================================================================
    '                     ATITLE Combo + Autosave + Row Colors
    ' ======================================================================

    ' Load distinct ATITLE → ACODE from TBL_EDESC
    Private Async Function LoadAccountTitlesAsync() As Task
        _accountTitles.Clear()
        _titleToCode.Clear()

        Using conn As SqlConnection = GetTunedConnection(),
              cmd As New SqlCommand("
                SELECT LTRIM(RTRIM(ATITLE)) AS ATITLE, LTRIM(RTRIM(ACODE)) AS ACODE
                FROM TBL_EDESC
                WHERE ATITLE IS NOT NULL AND LTRIM(RTRIM(ATITLE)) <> ''
                ORDER BY ATITLE, ACODE;", conn)
            Await conn.OpenAsync()
            Using rdr = Await cmd.ExecuteReaderAsync()
                While Await rdr.ReadAsync()
                    Dim t As String = rdr("ATITLE").ToString()
                    Dim c As String = rdr("ACODE").ToString()
                    If Not _titleToCode.ContainsKey(t) Then
                        _titleToCode(t) = c
                        _accountTitles.Add(t)
                    End If
                End While
            End Using
        End Using

        _accountTitles = _accountTitles.Distinct(StringComparer.OrdinalIgnoreCase).
                                         OrderBy(Function(s) s, StringComparer.OrdinalIgnoreCase).ToList()
    End Function

    ' Ensure a typed title appears in the dropdown (prevents blank display)
    Private Sub EnsureTitleInList(atitle As String)
        If String.IsNullOrWhiteSpace(atitle) Then Return
        If Not _titleToCode.ContainsKey(atitle) Then
            _titleToCode(atitle) = "" ' unknown code
        End If
        If Not _accountTitles.Any(Function(s) s.Equals(atitle, StringComparison.OrdinalIgnoreCase)) Then
            _accountTitles.Add(atitle)
            _accountTitles = _accountTitles.Distinct(StringComparer.OrdinalIgnoreCase).
                                            OrderBy(Function(s) s, StringComparer.OrdinalIgnoreCase).ToList()
            BindATitleComboItems()
        End If
    End Sub

    ' Bind the ATITLE list to the ComboBox column
    Private Sub BindATitleComboItems()
        Dim col = TryCast(DataGridView1.Columns(COL_ATITLE), DataGridViewComboBoxColumn)
        If col Is Nothing Then Return
        col.DataSource = Nothing
        col.DataSource = _accountTitles
        col.DisplayMember = Nothing
        col.ValueMember = Nothing
    End Sub

    Private Function LookupAcode(atitle As String) As String
        If String.IsNullOrWhiteSpace(atitle) Then Return ""
        Dim code As String = ""
        If _titleToCode.TryGetValue(atitle.Trim(), code) Then
            Return If(code, "")
        End If
        Return ""
    End Function

    ' Unified save for ATITLE/ACODE/REMARKS
    Private Async Function SaveInventoryFieldsAsync(id As Integer, atitle As String, acode As String, remarks As String) As Task
        Try
            Using conn As SqlConnection = GetTunedConnection(),
                  cmd As New SqlCommand("
                    UPDATE TBL_INVENTORY
                    SET ATITLE = @ATITLE, ACODE = @ACODE, REMARKS = @REMARKS
                    WHERE ID = @ID;", conn)
                cmd.Parameters.Add("@ATITLE", SqlDbType.NVarChar, 400).Value = If(atitle, "").Trim()
                cmd.Parameters.Add("@ACODE", SqlDbType.NVarChar, 200).Value = If(acode, "").Trim()
                cmd.Parameters.Add("@REMARKS", SqlDbType.NVarChar, -1).Value = If(remarks, "").Trim()
                cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id

                Await conn.OpenAsync()
                Await cmd.ExecuteNonQueryAsync()
            End Using
        Catch ex As Exception
            MessageBox.Show("Auto-save failed: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Function

    ' Let users type in ATITLE Combo + autocomplete
    Private Sub DataGridView1_EditingControlShowing(sender As Object, e As DataGridViewEditingControlShowingEventArgs) Handles DataGridView1.EditingControlShowing
        Try
            If DataGridView1.CurrentCell Is Nothing Then Return
            If DataGridView1.Columns(DataGridView1.CurrentCell.ColumnIndex).Name = COL_ATITLE Then
                Dim cb = TryCast(e.Control, ComboBox)
                If cb IsNot Nothing Then
                    cb.DropDownStyle = ComboBoxStyle.DropDown ' allow typing
                    cb.AutoCompleteMode = AutoCompleteMode.SuggestAppend
                    cb.AutoCompleteSource = AutoCompleteSource.ListItems
                    RemoveHandler cb.SelectionChangeCommitted, AddressOf ATITLE_SelectionChangeCommitted
                    AddHandler cb.SelectionChangeCommitted, AddressOf ATITLE_SelectionChangeCommitted
                End If
            End If
        Catch
        End Try
    End Sub

    ' Commit combo edit immediately so CellValueChanged fires
    Private Sub DataGridView1_CurrentCellDirtyStateChanged(sender As Object, e As EventArgs) Handles DataGridView1.CurrentCellDirtyStateChanged
        If _suppressGridEvents Then Return
        If DataGridView1.IsCurrentCellDirty Then
            If TypeOf DataGridView1.CurrentCell Is DataGridViewComboBoxCell Then
                DataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit)
            End If
        End If
    End Sub

    ' Dropdown selection for ATITLE
    Private Async Sub ATITLE_SelectionChangeCommitted(sender As Object, e As EventArgs)
        If _suppressGridEvents Then Return
        Dim row = DataGridView1.CurrentRow
        If row Is Nothing OrElse row.IsNewRow Then Return

        Dim id As Integer
        If Not Integer.TryParse(If(row.Cells(COL_ID)?.Value, "").ToString(), id) Then Return

        Dim atitle As String = If(row.Cells(COL_ATITLE)?.Value, "").ToString()
        Dim cb = TryCast(sender, ComboBox)
        If cb IsNot Nothing Then atitle = cb.Text

        ' Ensure value exists in dropdown to avoid blank display
        EnsureTitleInList(atitle)

        Dim acode As String = LookupAcode(atitle)
        Dim remarks As String = If(row.Cells(COL_REMARKS)?.Value, "").ToString()

        _suppressGridEvents = True
        Try
            row.Cells(COL_ATITLE).Value = atitle
            row.Cells(COL_ACODE).Value = acode
        Finally
            _suppressGridEvents = False
        End Try

        Await SaveInventoryFieldsAsync(id, atitle, acode, remarks)
    End Sub

    ' Manual typing in ATITLE / edits in ACODE / REMARKS / STATUS color refresh
    Private Async Sub DataGridView1_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellValueChanged
        If _suppressGridEvents Then Return
        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Return

        Dim colName = DataGridView1.Columns(e.ColumnIndex).Name
        Dim row = DataGridView1.Rows(e.RowIndex)
        If row Is Nothing OrElse row.IsNewRow Then Return

        ' Recolor immediately if STATUS changed
        If colName = COL_STATUS Then
            ApplyRowColorForRow(row)
            Return
        End If

        If colName <> COL_ATITLE AndAlso colName <> COL_ACODE AndAlso colName <> COL_REMARKS Then Return

        Dim idStr = If(row.Cells(COL_ID)?.Value, "").ToString()
        Dim id As Integer
        If Not Integer.TryParse(idStr, id) Then Return

        Dim atitle As String = If(row.Cells(COL_ATITLE)?.Value, "").ToString()
        Dim acode As String = If(row.Cells(COL_ACODE)?.Value, "").ToString()
        Dim remarks As String = If(row.Cells(COL_REMARKS)?.Value, "").ToString()

        If colName = COL_ATITLE Then
            EnsureTitleInList(atitle)
            Dim looked = LookupAcode(atitle)
            If Not String.IsNullOrEmpty(looked) AndAlso Not String.Equals(looked, acode, StringComparison.OrdinalIgnoreCase) Then
                _suppressGridEvents = True
                Try
                    row.Cells(COL_ACODE).Value = looked
                    acode = looked
                Finally
                    _suppressGridEvents = False
                End Try
            End If
        End If

        Await SaveInventoryFieldsAsync(id, atitle, acode, remarks)
    End Sub

    ' Also fire save when user finishes editing a text cell (helps for REMARKS)
    Private Async Sub DataGridView1_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellEndEdit
        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Return
        Dim colName = DataGridView1.Columns(e.ColumnIndex).Name
        If colName <> COL_ATITLE AndAlso colName <> COL_ACODE AndAlso colName <> COL_REMARKS Then Return

        Dim row = DataGridView1.Rows(e.RowIndex)
        If row Is Nothing OrElse row.IsNewRow Then Return

        Dim idStr = If(row.Cells(COL_ID)?.Value, "").ToString()
        Dim id As Integer
        If Not Integer.TryParse(idStr, id) Then Return

        Dim atitle As String = If(row.Cells(COL_ATITLE)?.Value, "").ToString()
        Dim acode As String = If(row.Cells(COL_ACODE)?.Value, "").ToString()
        Dim remarks As String = If(row.Cells(COL_REMARKS)?.Value, "").ToString()

        Await SaveInventoryFieldsAsync(id, atitle, acode, remarks)
    End Sub

    Private Sub Cmdedit_Click_1(sender As Object, e As EventArgs) Handles cmdedit.Click
        Try
            Dim gv = DataGridView1
            If gv Is Nothing OrElse gv.CurrentRow Is Nothing OrElse gv.CurrentRow.Index < 0 Then
                MessageBox.Show("Please select an item to edit.", "Edit", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If

            Dim r As DataGridViewRow = gv.CurrentRow

            Using f As New frminventoryfile()
                f.lbltrans.Text = "Edit"
                f.cmdsave.Text = "Update"
                f.lblid.Text = SafeCell(r, COL_ID)
                f.combotype.Text = SafeCell(r, COL_PTYPE)
                f.txtdesc.Text = SafeCell(r, COL_DESC)
                f.txtatitle.Text = SafeCell(r, COL_ATITLE)
                f.txtacode.Text = SafeCell(r, COL_ACODE)
                f.txtipno.Text = SafeCell(r, COL_IPNO)
                f.txtunit.Text = SafeCell(r, COL_UNITS)
                f.txtqty.Text = SafeCell(r, COL_QTY)
                f.txtremarks.Text = SafeCell(r, COL_REMARKS)
                Dim q As Integer = ToInt(f.txtqty.Text)
                f.txtstatus.Text = If(q > 0, "In Stock", "Out of Stock")
                f.StartPosition = FormStartPosition.CenterParent
                f.ShowDialog(Me)
            End Using
        Catch ex As Exception
            MessageBox.Show("Edit Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
End Class
