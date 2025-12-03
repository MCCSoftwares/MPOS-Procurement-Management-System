Imports System.Data.SqlClient
Imports System.IO
Imports System.Diagnostics
Imports System.Threading
Imports System.Threading.Tasks
Imports System.Collections.Generic

Public Class frmequipment
    ' ── Paging state ──────────────────────────────────
    Private Const PageSize As Integer = 50
    Private _currentPage As Integer = 1       ' 1-based
    Private _totalRows As Integer = 0
    Private _totalPages As Integer = 1

    ' ── Async infra ───────────────────────────────────
    Private _cts As CancellationTokenSource = New CancellationTokenSource()
    Private WithEvents _searchTimer As System.Windows.Forms.Timer
    Private _suppressSearch As Boolean = False  ' avoid duplicate search on Refresh

    ' ── Count cache (per filter) to avoid COUNT(*) storms ─
    Private Structure CountEntry
        Public Count As Integer
        Public AsOf As DateTime
    End Structure
    Private ReadOnly _countCache As New Dictionary(Of String, CountEntry)(StringComparer.OrdinalIgnoreCase)

    ' ── Key structures for keyset pagination ─
    Private Structure Key
        Public E As String
        Public Id As Integer
    End Structure

    ' Last row key of the current page (for fast NEXT)
    Private _lastKey As Key

    ' Start key (first row) per page number (for fast PREV): page -> first key
    Private ReadOnly _pageStart As New Dictionary(Of Integer, Key)()

    Private Async Sub FrmEdesc_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
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

                .BorderStyle = BorderStyle.None
                .CellBorderStyle = DataGridViewCellBorderStyle.None
                .ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
                .AdvancedCellBorderStyle.Left = DataGridViewAdvancedCellBorderStyle.None
                .AdvancedCellBorderStyle.Right = DataGridViewAdvancedCellBorderStyle.None
                .AdvancedCellBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.None
                .AdvancedCellBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.None

                .ColumnCount = 5
                With .Columns(0) : .Name = "ID" : .HeaderText = "ID" : .DataPropertyName = "ID" : .Visible = False : End With
                With .Columns(1) : .Name = "EDESC" : .HeaderText = "Description" : .DataPropertyName = "EDESC" : .Width = 200 : End With
                With .Columns(2) : .Name = "ACODE" : .HeaderText = "Account Code" : .DataPropertyName = "ACODE" : .Width = 120 : End With
                With .Columns(3) : .Name = "IPNO" : .HeaderText = "Item/Property No" : .DataPropertyName = "IPNO" : .Width = 100 : End With
                With .Columns(4) : .Name = "UNIT" : .HeaderText = "Unit" : .DataPropertyName = "UNIT" : .Width = 80 : End With

                Dim imgBtn As New DataGridViewButtonColumn() With {
                    .Name = "IMAGE", .HeaderText = "Image", .Text = "View Image", .UseColumnTextForButtonValue = True
                }
                .Columns.Add(imgBtn)

                Dim hdrColor As Color = Color.FromArgb(73, 94, 252)
                .EnableHeadersVisualStyles = False
                With .ColumnHeadersDefaultCellStyle
                    .BackColor = hdrColor : .ForeColor = Color.White
                    .SelectionBackColor = hdrColor : .SelectionForeColor = Color.White
                    .Font = New Font("Segoe UI", 10.0!, FontStyle.Bold)
                    .Alignment = DataGridViewContentAlignment.MiddleLeft
                End With
                .ColumnHeadersHeight = 52
                .ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing

                With .DefaultCellStyle
                    .Font = New Font("Segoe UI", 9.0!)
                    .ForeColor = Color.Black
                    .SelectionBackColor = Color.FromArgb(210, 222, 255)
                    .SelectionForeColor = Color.Black
                End With
                .RowsDefaultCellStyle.BackColor = Color.White
                .AlternatingRowsDefaultCellStyle.BackColor = Color.White
                .RowTemplate.Height = 34
                .RowTemplate.DefaultCellStyle.Padding = New Padding(0, 6, 0, 6)
            End With

            _searchTimer = New System.Windows.Forms.Timer() With {.Interval = 350}

            ' Tag your paging buttons with their direction (adjust if your UI uses opposite mapping)
            If cmdnext IsNot Nothing Then cmdnext.Tag = "prev"  ' Back
            If btnNext IsNot Nothing Then btnNext.Tag = "next"  ' Next

            Await ResetAndLoadAsync()
        Catch ex As Exception
            MessageBox.Show("Error initializing grid: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ───────────────────────────────────────────────────
    ' Refresh button: clear search, go to page 1, show all
    ' ───────────────────────────────────────────────────
    Private Async Sub cmdrefresh_Click(sender As Object, e As EventArgs) Handles cmdrefresh.Click
        Try
            If _searchTimer IsNot Nothing Then _searchTimer.Stop()
            _suppressSearch = True
            Try
                txtsearch.Clear()   ' TextChanged will be ignored due to _suppressSearch
            Finally
                _suppressSearch = False
            End Try
            Await ResetAndLoadAsync() ' resets to page 1 and loads unfiltered data
        Catch ex As Exception
            MessageBox.Show("Refresh error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ───────────────────────────────────────────────────
    ' Quick page-only reload (used by delete & paging)
    ' ───────────────────────────────────────────────────
    Private Async Function ReloadCurrentPageQuickAsync() As Task
        Try
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
        Catch ex As OperationCanceledException
        Catch ex As Exception
            MessageBox.Show("Error refreshing page: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Function

    Private Sub UpdateCountCacheForCurrentFilter(newCount As Integer)
        Dim filter = CurrentFilterText()
        _countCache(filter) = New CountEntry With {.Count = newCount, .AsOf = DateTime.UtcNow}
    End Sub

    ' ───────────────────────────────────────────────────
    ' New/Edit dialogs → do nothing on close (manual refresh only)
    ' ───────────────────────────────────────────────────
    Private Sub Cmdnew_Click(sender As Object, e As EventArgs) Handles cmdnew.Click
        Try
            Using frm As New FrmEquipmentAdd()
                frm.lblid.Text = "0"
                frm.ShowDialog(Me)
                ' Intentionally no reload here. Use Refresh to see changes.
            End Using
        Catch
        End Try
    End Sub

    Private Sub cmdEdit_Click(sender As Object, e As EventArgs) Handles cmdedit.Click
        If DataGridView1.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select a record to edit.", "Info",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Dim id As Integer = CInt(DataGridView1.SelectedRows(0).Cells("ID").Value)
        Using frm As New FrmEquipmentAdd()
            frm.lblid.Text = id.ToString()
            frm.ShowDialog(Me)
            ' Intentionally no reload here. Use Refresh to see changes.
        End Using
    End Sub

    ' ───────────────────────────────────────────────────
    ' Legacy sync loader (kept for compatibility)
    ' ───────────────────────────────────────────────────
    Public Sub LoadData()
        Try
            Using conn As SqlConnection = GetTunedConnection(),
                  cmd As New SqlCommand("
                      SELECT TOP 150 ID, EDESC, ATITLE, ACODE, IPNO, UNIT
                      FROM TBL_EDESC
                      ORDER BY EDESC, ID", conn)
                cmd.CommandTimeout = 30
                conn.Open()
                Using rdr = cmd.ExecuteReader()
                    Dim dt As New DataTable()
                    dt.Load(rdr)
                    DataGridView1.DataSource = dt
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error loading data: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ───────────────────────────────────────────────────
    ' Async paging/search pipeline
    ' ───────────────────────────────────────────────────
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
        _lastKey = New Key With {.E = Nothing, .Id = 0}
        _pageStart.Clear()

        Await RefreshTotalRowsAsync(_cts.Token)
        Await LoadPageAsync(_cts.Token) ' initial page via OFFSET
        UpdatePagerUI()
    End Function

    Private Function CurrentFilterText() As String
        Return If(txtsearch.Text, "").Trim()
    End Function

    Private Function NoFilter() As Boolean
        Return String.IsNullOrWhiteSpace(CurrentFilterText())
    End Function

    Private Async Function RefreshTotalRowsAsync(ct As CancellationToken) As Task
        Dim filter As String = CurrentFilterText()

        ' tiny cache: 5s TTL
        Dim entry As CountEntry
        If _countCache.TryGetValue(filter, entry) Then
            If (DateTime.UtcNow - entry.AsOf) < TimeSpan.FromSeconds(5) Then
                _totalRows = entry.Count
                _totalPages = Math.Max(1, CInt(Math.Ceiling(_totalRows / CDbl(PageSize))))
                If _currentPage > _totalPages Then _currentPage = _totalPages
                Exit Function
            End If
        End If

        Dim hasFilter As Boolean = Not String.IsNullOrWhiteSpace(filter)
        Dim whereSql As String = If(hasFilter, " WHERE (EDESC LIKE @q OR ATITLE LIKE @q OR ACODE LIKE @q OR IPNO LIKE @q OR UNIT LIKE @q)", "")
        Dim sqlCount As String = "SELECT COUNT(*) FROM TBL_EDESC" & whereSql

        Using conn As SqlConnection = GetTunedConnection(),
              cmd As New SqlCommand(sqlCount, conn)
            cmd.CommandTimeout = 30
            If hasFilter Then cmd.Parameters.Add("@q", SqlDbType.NVarChar, 200).Value = "%" & filter & "%"

            Await conn.OpenAsync(ct)
            Dim countObj = Await cmd.ExecuteScalarAsync(ct)
            _totalRows = If(countObj Is Nothing OrElse countObj Is DBNull.Value, 0, Convert.ToInt32(countObj))
        End Using

        _totalPages = Math.Max(1, CInt(Math.Ceiling(_totalRows / CDbl(PageSize))))
        If _currentPage > _totalPages Then _currentPage = _totalPages

        _countCache(filter) = New CountEntry With {.Count = _totalRows, .AsOf = DateTime.UtcNow}
    End Function

    ' Standard OFFSET/FETCH page load (also records keys)
    Private Async Function LoadPageAsync(ct As CancellationToken) As Task
        Dim offset As Integer = (Math.Max(1, _currentPage) - 1) * PageSize
        Dim filter As String = CurrentFilterText()
        Dim hasFilter As Boolean = Not String.IsNullOrWhiteSpace(filter)

        Dim finalSql As String =
            "SELECT ID, EDESC, ATITLE, ACODE, IPNO, UNIT " &
            "FROM TBL_EDESC" &
            If(hasFilter, " WHERE (EDESC LIKE @q OR ATITLE LIKE @q OR ACODE LIKE @q OR IPNO LIKE @q OR UNIT LIKE @q)", "") &
            " ORDER BY EDESC, ID " &
            " OFFSET @off ROWS FETCH NEXT @ps ROWS ONLY;"

        Try
            If lblpage IsNot Nothing Then lblpage.Text = "Loading..."
            If btnNext IsNot Nothing Then btnNext.Enabled = False
            If cmdnext IsNot Nothing Then cmdnext.Enabled = False

            Using conn As SqlConnection = GetTunedConnection(),
                  cmd As New SqlCommand(finalSql, conn)
                cmd.CommandTimeout = 30
                If hasFilter Then cmd.Parameters.Add("@q", SqlDbType.NVarChar, 200).Value = "%" & filter & "%"
                cmd.Parameters.Add("@off", SqlDbType.Int).Value = offset
                cmd.Parameters.Add("@ps", SqlDbType.Int).Value = PageSize

                Await conn.OpenAsync(ct)
                Using rdr = Await cmd.ExecuteReaderAsync(ct)
                    Dim dt As New DataTable()
                    dt.Load(rdr)
                    DataGridView1.DataSource = dt

                    If dt.Rows.Count > 0 Then
                        Dim first As DataRow = dt.Rows(0)
                        Dim last As DataRow = dt.Rows(dt.Rows.Count - 1)

                        Dim firstKey As New Key With {.E = Convert.ToString(first("EDESC")), .Id = Convert.ToInt32(first("ID"))}
                        _pageStart(_currentPage) = firstKey

                        _lastKey = New Key With {.E = Convert.ToString(last("EDESC")), .Id = Convert.ToInt32(last("ID"))}
                    End If
                End Using
            End Using
        Catch ex As OperationCanceledException
        Catch ex As Exception
            MessageBox.Show("Error loading page: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            UpdatePagerUI()
        End Try
    End Function

    ' Fast NEXT (no filter): start after current _lastKey
    Private Async Function LoadPageKeysetNextAsync(ct As CancellationToken) As Task
        Try
            If lblpage IsNot Nothing Then lblpage.Text = "Loading..."
            If btnNext IsNot Nothing Then btnNext.Enabled = False
            If cmdnext IsNot Nothing Then cmdnext.Enabled = False

            Dim sql As String =
                "SELECT TOP (@ps) ID, EDESC, ATITLE, ACODE, IPNO, UNIT " &
                "FROM TBL_EDESC " &
                "WHERE (EDESC > @lastE) OR (EDESC = @lastE AND ID > @lastId) " &
                "ORDER BY EDESC, ID;"

            Using conn As SqlConnection = GetTunedConnection(),
                  cmd As New SqlCommand(sql, conn)
                cmd.CommandTimeout = 30
                cmd.Parameters.Add("@ps", SqlDbType.Int).Value = PageSize
                cmd.Parameters.Add("@lastE", SqlDbType.NVarChar, 500).Value = If(_lastKey.E, "")
                cmd.Parameters.Add("@lastId", SqlDbType.Int).Value = _lastKey.Id

                Await conn.OpenAsync(ct)
                Using rdr = Await cmd.ExecuteReaderAsync(ct)
                    Dim dt As New DataTable()
                    dt.Load(rdr)

                    If dt.Rows.Count = 0 Then
                        UpdatePagerUI()
                        Return
                    End If

                    DataGridView1.DataSource = dt

                    Dim first As DataRow = dt.Rows(0)
                    Dim last As DataRow = dt.Rows(dt.Rows.Count - 1)

                    _pageStart(_currentPage) = New Key With {.E = Convert.ToString(first("EDESC")), .Id = Convert.ToInt32(first("ID"))}
                    _lastKey = New Key With {.E = Convert.ToString(last("EDESC")), .Id = Convert.ToInt32(last("ID"))}
                End Using
            End Using
        Catch ex As OperationCanceledException
        Catch ex As Exception
            MessageBox.Show("Error loading page: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            UpdatePagerUI()
        End Try
    End Function

    ' Fast PREV (no filter): rows BEFORE current page's first key, then reverse to ascending
    Private Async Function LoadPageKeysetPrevAsync(currentFirstKey As Key, ct As CancellationToken) As Task
        Try
            If lblpage IsNot Nothing Then lblpage.Text = "Loading..."
            If btnNext IsNot Nothing Then btnNext.Enabled = False
            If cmdnext IsNot Nothing Then cmdnext.Enabled = False

            Dim sql As String =
                "SELECT TOP (@ps) ID, EDESC, ATITLE, ACODE, IPNO, UNIT " &
                "FROM TBL_EDESC " &
                "WHERE (EDESC < @firstE) OR (EDESC = @firstE AND ID < @firstId) " &
                "ORDER BY EDESC DESC, ID DESC;"

            Using conn As SqlConnection = GetTunedConnection(),
                  cmd As New SqlCommand(sql, conn)
                cmd.CommandTimeout = 30
                cmd.Parameters.Add("@ps", SqlDbType.Int).Value = PageSize
                cmd.Parameters.Add("@firstE", SqlDbType.NVarChar, 500).Value = If(currentFirstKey.E, "")
                cmd.Parameters.Add("@firstId", SqlDbType.Int).Value = currentFirstKey.Id

                Await conn.OpenAsync(ct)
                Using rdr = Await cmd.ExecuteReaderAsync(ct)
                    Dim dtDesc As New DataTable()
                    dtDesc.Load(rdr)

                    If dtDesc.Rows.Count = 0 Then
                        UpdatePagerUI()
                        Return
                    End If

                    Dim dt As DataTable = dtDesc.Clone()
                    For i As Integer = dtDesc.Rows.Count - 1 To 0 Step -1
                        dt.ImportRow(dtDesc.Rows(i))
                    Next

                    DataGridView1.DataSource = dt

                    Dim first As DataRow = dt.Rows(0)
                    Dim last As DataRow = dt.Rows(dt.Rows.Count - 1)

                    _pageStart(_currentPage) = New Key With {.E = Convert.ToString(first("EDESC")), .Id = Convert.ToInt32(first("ID"))}
                    _lastKey = New Key With {.E = Convert.ToString(last("EDESC")), .Id = Convert.ToInt32(last("ID"))}
                End Using
            End Using
        Catch ex As OperationCanceledException
        Catch ex As Exception
            MessageBox.Show("Error loading page: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            UpdatePagerUI()
        End Try
    End Function

    Private Async Function MovePageAsync(delta As Integer) As Task
        If _totalPages < 1 Then _totalPages = 1
        Dim target = Math.Min(_totalPages, Math.Max(1, _currentPage + delta))

        If target <> _currentPage Then
            Try
                If _cts IsNot Nothing Then
                    _cts.Cancel()
                    _cts.Dispose()
                End If
            Catch
            End Try
            _cts = New CancellationTokenSource()

            If NoFilter() Then
                If delta > 0 Then
                    _currentPage = target
                    Await LoadPageKeysetNextAsync(_cts.Token)
                Else
                    If _currentPage <= 1 Then
                        UpdatePagerUI()
                        Return
                    End If
                    Dim currentFirstKey As Key
                    If _pageStart.TryGetValue(_currentPage, currentFirstKey) Then
                        _currentPage = target
                        Await LoadPageKeysetPrevAsync(currentFirstKey, _cts.Token)
                    Else
                        _currentPage = target
                        Await LoadPageAsync(_cts.Token)
                    End If
                End If
            Else
                _currentPage = target
                Await LoadPageAsync(_cts.Token)
            End If
        End If

        UpdatePagerUI()
    End Function

    Private Sub UpdatePagerUI()
        If _totalPages < 1 Then _totalPages = 1
        If _currentPage < 1 Then _currentPage = 1
        If _currentPage > _totalPages Then _currentPage = _totalPages

        Dim prevEnabled As Boolean = (_currentPage > 1)
        Dim nextEnabled As Boolean = (_currentPage < _totalPages)

        ' Enable/disable based on Tag ("next" or "prev")
        If cmdnext IsNot Nothing Then
            Dim t As String = TryCast(cmdnext.Tag, String)
            cmdnext.Enabled = If(String.Equals(t, "next", StringComparison.OrdinalIgnoreCase), nextEnabled, prevEnabled)
        End If
        If btnNext IsNot Nothing Then
            Dim t As String = TryCast(btnNext.Tag, String)
            btnNext.Enabled = If(String.Equals(t, "next", StringComparison.OrdinalIgnoreCase), nextEnabled, prevEnabled)
        End If

        If lblpage IsNot Nothing Then
            lblpage.Text = $"Page {_currentPage} / {_totalPages}   •   Rows: {_totalRows}"
        End If
    End Sub

    ' ── Debounced search (WinForms Timer) ─────────────
    Private Sub txtsearch_TextChanged(sender As Object, e As EventArgs) Handles txtsearch.TextChanged
        If _suppressSearch Then Return
        If _searchTimer Is Nothing Then Return
        _searchTimer.Stop()
        _searchTimer.Start()
    End Sub

    Private Async Sub _searchTimer_Tick(sender As Object, e As EventArgs) Handles _searchTimer.Tick
        _searchTimer.Stop()
        Await ResetAndLoadAsync()
    End Sub

    Private Async Sub cmdsearch_Click(sender As Object, e As EventArgs) Handles cmdsearch.Click
        Await ResetAndLoadAsync()
    End Sub

    ' ───────────────────────────────────────────────────
    ' Delete: reflect immediately (optional; move behind Refresh if desired)
    ' ───────────────────────────────────────────────────
    Private Async Sub cmdDelete_Click(sender As Object, e As EventArgs) Handles cmddelete.Click
        If DataGridView1.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select a record to delete.", "Info",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        If MessageBox.Show("Are you sure you want to delete this record?", "Confirm Delete",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then

            Dim id As Integer = CInt(DataGridView1.SelectedRows(0).Cells("ID").Value)
            Try
                Using conn As SqlConnection = GetTunedConnection(),
                      cmd As New SqlCommand("DELETE FROM TBL_EDESC WHERE ID = @ID", conn)
                    cmd.CommandTimeout = 30
                    cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id
                    Await conn.OpenAsync()
                    Await cmd.ExecuteNonQueryAsync()
                End Using
            Catch ex As Exception
                MessageBox.Show("Error deleting record: " & ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try

            If _totalRows > 0 Then _totalRows -= 1
            _totalPages = Math.Max(1, CInt(Math.Ceiling(_totalRows / CDbl(PageSize))))
            UpdateCountCacheForCurrentFilter(_totalRows)
            Await ReloadCurrentPageQuickAsync()
        End If
    End Sub

    ' ───────────────────────────────────────────────────
    ' Image button: view image
    ' ───────────────────────────────────────────────────
    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) _
        Handles DataGridView1.CellContentClick

        If e.RowIndex < 0 OrElse DataGridView1.Columns(e.ColumnIndex).Name <> "IMAGE" Then Return

        Dim id As Integer = CInt(DataGridView1.Rows(e.RowIndex).Cells("ID").Value)

        Using conn As SqlConnection = GetTunedConnection(),
              cmd As New SqlCommand("SELECT IMAGES FROM TBL_EDESC WHERE ID = @ID", conn)
            cmd.CommandTimeout = 30
            cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id
            conn.Open()

            Dim result = cmd.ExecuteScalar()
            If result Is Nothing OrElse IsDBNull(result) Then
                MessageBox.Show("No image for this record.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If

            Dim imgBytes As Byte() = DirectCast(result, Byte())
            Dim temp As String = Path.Combine(Path.GetTempPath(), $"IMG_{id}.jpg")
            File.WriteAllBytes(temp, imgBytes)
            Process.Start(New ProcessStartInfo(temp) With {.UseShellExecute = True})
        End Using
    End Sub

    ' ───────────────────────────────────────────────────
    ' Tuned connection builder (pooling, packet size, timeout)
    ' ───────────────────────────────────────────────────
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

    ' ───────────────────────────────────────────────────
    ' Paging button handlers (honor Tag = "next"/"prev")
    ' ───────────────────────────────────────────────────
    Private Async Sub cmdnext_Click(sender As Object, e As EventArgs) Handles cmdnext.Click
        Dim dir As String = TryCast(cmdnext.Tag, String)
        Dim delta As Integer = If(String.Equals(dir, "next", StringComparison.OrdinalIgnoreCase), +1, -1)
        Await MovePageAsync(delta)
    End Sub

    Private Async Sub btnNext_Click(sender As Object, e As EventArgs) Handles btnNext.Click
        Dim dir As String = TryCast(btnNext.Tag, String)
        Dim delta As Integer = If(String.Equals(dir, "next", StringComparison.OrdinalIgnoreCase), +1, -1)
        Await MovePageAsync(delta)
    End Sub
End Class
