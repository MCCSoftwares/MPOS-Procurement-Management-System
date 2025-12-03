Imports System.Data
Imports System.Data.SqlClient
Imports System.Threading

Public Class frmrfq
    Private WithEvents ctxMenu As ContextMenuStrip
    Private WithEvents mnuOpen As ToolStripMenuItem
    Private WithEvents mnuDelete As ToolStripMenuItem

    ' ── Filters ─────────────────────────────────────────────────────────────
    Private _lastSearch As String = ""
    Private _lastStatus As String = "All"
    Private _lastFCluster As String = "All"     ' RFQ_FUNDS
    Private _lastOffice As String = "All"       ' RFQ_OEUSER
    Private _lastYear As String = "All"         ' YEAR(RFQ_DATE)
    Private _suppressFilterEvents As Boolean = False

    ' ── Paging ─────────────────────────────────────────────────────────────
    Private Const PageSize As Integer = 100
    Private _pageIndex As Integer = 1
    Private _totalRows As Integer = 0
    Private _totalPages As Integer = 1

    ' ── Async search infra ─────────────────────────────────────────────────
    Private _searchCts As CancellationTokenSource
    Private Const SearchDebounceMs As Integer = 250

    Private Sub Frmrfq_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            ' Base grid config
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
            End With

            ' Columns
            DataGridView1.ColumnCount = 10
            With DataGridView1
                .Columns(0).Name = "ID" : .Columns(0).HeaderText = "ID" : .Columns(0).DataPropertyName = "ID" : .Columns(0).Visible = False

                .Columns(1).Name = "RFQ_DATE" : .Columns(1).HeaderText = "DATE" : .Columns(1).DataPropertyName = "RFQ_DATE"
                .Columns(1).DefaultCellStyle.Format = "MM/dd/yyyy" : .Columns(1).Width = 120
                .Columns(1).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft

                .Columns(2).Name = "RFQ_PRNO" : .Columns(2).HeaderText = "PR NUMBER" : .Columns(2).DataPropertyName = "RFQ_PRNO" : .Columns(2).Width = 160

                .Columns(3).Name = "RFQ_PENTITY" : .Columns(3).HeaderText = "PROCURING ENTITY" : .Columns(3).DataPropertyName = "RFQ_PENTITY" : .Columns(3).Width = 260

                .Columns(4).Name = "RFQ_PURPOSE" : .Columns(4).HeaderText = "PURPOSE" : .Columns(4).DataPropertyName = "RFQ_PURPOSE" : .Columns(4).Width = 360
                .Columns(4).DefaultCellStyle.WrapMode = DataGridViewTriState.True

                .Columns(5).Name = "RFQ_OEUSER" : .Columns(5).HeaderText = "OFFICE/END USER" : .Columns(5).DataPropertyName = "RFQ_OEUSER" : .Columns(5).Width = 220

                .Columns(6).Name = "RFQ_FUNDS" : .Columns(6).HeaderText = "FUND CLUSTER" : .Columns(6).DataPropertyName = "RFQ_FUNDS" : .Columns(6).Width = 160

                .Columns(7).Name = "RFQ_MPROC" : .Columns(7).HeaderText = "MODE OF PROCUREMENT" : .Columns(7).DataPropertyName = "RFQ_MPROC" : .Columns(7).Width = 240

                .Columns(8).Name = "RFQ_QSDATE" : .Columns(8).HeaderText = "QUOTE SUBMISSION DATE" : .Columns(8).DataPropertyName = "RFQ_QSDATE"
                .Columns(8).DefaultCellStyle.Format = "MM/dd/yyyy" : .Columns(8).Width = 170
                .Columns(8).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft

                .Columns(9).Name = "RFQ_STATUS" : .Columns(9).HeaderText = "STATUS" : .Columns(9).DataPropertyName = "RFQ_STATUS" : .Columns(9).Width = 220
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
                .RowTemplate.Height = 34
                .RowTemplate.DefaultCellStyle.Padding = New Padding(0, 6, 0, 6)
            End With
            EnableDoubleBuffering(DataGridView1)

            ' Context menu (right-click)
            ctxMenu = New ContextMenuStrip()
            mnuOpen = New ToolStripMenuItem("Open") With {.Name = "mnuOpen"}
            mnuDelete = New ToolStripMenuItem("Delete") With {.Name = "mnuDelete"}
            ctxMenu.Items.AddRange(New ToolStripItem() {mnuOpen, mnuDelete})
            DataGridView1.ContextMenuStrip = ctxMenu
            AddHandler DataGridView1.CellMouseDown, AddressOf DataGridView1_CellMouseDown
            AddHandler ctxMenu.Opening, AddressOf ctxMenu_Opening

            ' Load filters + data
            _suppressFilterEvents = True
            LoadStatuses()
            LoadFClusters()
            LoadOffices()
            LoadYears()

            Dim curYear As String = DateTime.Now.Year.ToString()
            SafeSelect(combostatus, "All")
            SafeSelect(Combofcluster, "All")
            SafeSelect(Combooffice, "All")
            SafeSelect(Comboyear, curYear)
            _lastStatus = "All" : _lastFCluster = "All" : _lastOffice = "All" : _lastYear = curYear
            _suppressFilterEvents = False

            _pageIndex = 1
            RefreshPage(resetPage:=True)

        Catch ex As Exception
            MessageBox.Show("Form Load Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ── Combobox loaders ─────────────────────────────────────────────────────
    Private Sub LoadStatuses()
        Try
            combostatus.Items.Clear()
            combostatus.Items.Add("All")
            Using conn As New SqlConnection(frmmain.txtdb.Text)
                conn.Open()
                Using cmd As New SqlCommand("
                    SELECT DISTINCT RFQ_STATUS
                    FROM TBL_RFQ
                    WHERE NULLIF(LTRIM(RTRIM(RFQ_STATUS)), '') IS NOT NULL
                    ORDER BY RFQ_STATUS;", conn)
                    Using rdr = cmd.ExecuteReader()
                        While rdr.Read()
                            combostatus.Items.Add(rdr("RFQ_STATUS").ToString())
                        End While
                    End Using
                End Using
            End Using
            If combostatus.Items.Count > 0 Then combostatus.SelectedIndex = 0
        Catch ex As Exception
            MessageBox.Show("Failed to load RFQ statuses: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub LoadFClusters()
        Try
            Combofcluster.Items.Clear()
            Combofcluster.Items.Add("All")
            Using conn As New SqlConnection(frmmain.txtdb.Text)
                conn.Open()
                Using cmd As New SqlCommand("
                    SELECT DISTINCT RFQ_FUNDS
                    FROM TBL_RFQ
                    WHERE NULLIF(LTRIM(RTRIM(RFQ_FUNDS)), '') IS NOT NULL
                    ORDER BY RFQ_FUNDS;", conn)
                    Using r = cmd.ExecuteReader()
                        While r.Read()
                            Combofcluster.Items.Add(r("RFQ_FUNDS").ToString())
                        End While
                    End Using
                End Using
            End Using
            Combofcluster.SelectedIndex = 0
        Catch ex As Exception
            MessageBox.Show("Failed to load Fund Clusters: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub LoadOffices()
        Try
            Combooffice.Items.Clear()
            Combooffice.Items.Add("All")
            Using conn As New SqlConnection(frmmain.txtdb.Text)
                conn.Open()
                Using cmd As New SqlCommand("
                    SELECT DISTINCT RFQ_OEUSER
                    FROM TBL_RFQ
                    WHERE NULLIF(LTRIM(RTRIM(RFQ_OEUSER)), '') IS NOT NULL
                    ORDER BY RFQ_OEUSER;", conn)
                    Using r = cmd.ExecuteReader()
                        While r.Read()
                            Combooffice.Items.Add(r("RFQ_OEUSER").ToString())
                        End While
                    End Using
                End Using
            End Using
            Combooffice.SelectedIndex = 0
        Catch ex As Exception
            MessageBox.Show("Failed to load Offices: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub LoadYears()
        Try
            Comboyear.Items.Clear()
            Comboyear.Items.Add("All")
            Using conn As New SqlConnection(frmmain.txtdb.Text)
                conn.Open()
                Using cmd As New SqlCommand("
                    SELECT DISTINCT YEAR(RFQ_DATE) AS YR
                    FROM TBL_RFQ
                    WHERE RFQ_DATE IS NOT NULL
                    ORDER BY YR DESC;", conn)
                    Using r = cmd.ExecuteReader()
                        While r.Read()
                            Comboyear.Items.Add(r("YR").ToString())
                        End While
                    End Using
                End Using
            End Using
            Dim curYear As String = DateTime.Now.Year.ToString()
            If Comboyear.Items.IndexOf(curYear) < 0 Then
                If Comboyear.Items.Count > 0 AndAlso Comboyear.Items(0).ToString() = "All" Then
                    Comboyear.Items.Insert(1, curYear)
                Else
                    Comboyear.Items.Insert(0, curYear)
                End If
            End If
            Comboyear.SelectedIndex = Math.Max(Comboyear.Items.IndexOf(curYear), 0)
        Catch ex As Exception
            MsgBox("Failed to load Years: " & ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ===== Helpers =============================================================
    Private Sub EnableDoubleBuffering(dgv As DataGridView)
        Dim t = dgv.GetType()
        Dim pi = t.GetProperty("DoubleBuffered", Reflection.BindingFlags.Instance Or Reflection.BindingFlags.NonPublic)
        If pi IsNot Nothing Then pi.SetValue(dgv, True, Nothing)
    End Sub

    Private Sub SafeSelect(cbo As ComboBox, desired As String)
        If String.IsNullOrWhiteSpace(desired) Then desired = "All"
        Dim idx As Integer = cbo.FindStringExact(desired)
        If idx >= 0 Then
            cbo.SelectedIndex = idx
        Else
            Dim allIdx = cbo.FindStringExact("All")
            If allIdx >= 0 Then cbo.SelectedIndex = allIdx
        End If
    End Sub

    ' ── Public compat API ──────────────────────────────────────────────────────
    Public Sub LoadRecords()
        RefreshPage(resetPage:=True)
    End Sub

    Public Sub LoadRecords(searchTerm As String, statusFilter As String)
        _lastSearch = If(searchTerm, "").Trim()
        _lastStatus = If(String.IsNullOrWhiteSpace(statusFilter), "All", statusFilter.Trim())
        RefreshPage(resetPage:=True)
    End Sub

    ' ── Paging (sync) ─────────────────────────────────────────────────────────
    Private Sub RefreshPage(Optional resetPage As Boolean = False)
        Try
            If resetPage Then _pageIndex = 1
            _totalRows = GetTotalCount()
            _totalPages = Math.Max(1, CInt(Math.Ceiling(_totalRows / CDbl(PageSize))))
            If _pageIndex > _totalPages Then _pageIndex = _totalPages
            If _pageIndex < 1 Then _pageIndex = 1

            Dim dt As DataTable = GetPageData(_pageIndex, PageSize)
            DataGridView1.DataSource = dt
            UpdatePagerUI()
        Catch ex As Exception
            MessageBox.Show("Paging error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Function GetTotalCount() As Integer
        Using conn As New SqlConnection(frmmain.txtdb.Text)
            conn.Open()
            Dim sql As String = "SELECT COUNT(1) FROM TBL_RFQ WITH (NOLOCK) WHERE 1=1 "
            Using cmd As New SqlCommand() With {.Connection = conn}
                AddFilterWhere(sql, cmd)
                cmd.CommandText = sql
                Dim o = cmd.ExecuteScalar()
                Return If(o Is Nothing OrElse IsDBNull(o), 0, Convert.ToInt32(o))
            End Using
        End Using
    End Function

    Private Function GetPageData(pageIndex As Integer, pageSize As Integer) As DataTable
        Dim dt As New DataTable()
        Using conn As New SqlConnection(frmmain.txtdb.Text)
            conn.Open()
            Dim sql As String =
                "SELECT ID, RFQ_DATE, RFQ_PRNO, RFQ_PENTITY, RFQ_PURPOSE, " &
                "       RFQ_OEUSER, RFQ_FUNDS, RFQ_MPROC, RFQ_QSDATE, RFQ_STATUS " &
                "FROM TBL_RFQ WITH (NOLOCK) WHERE 1=1 "
            Using cmd As New SqlCommand() With {.Connection = conn}
                AddFilterWhere(sql, cmd)
                sql &= " ORDER BY RFQ_DATE DESC, ID DESC " &
                       " OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;"
                Dim skip As Integer = (pageIndex - 1) * pageSize
                cmd.Parameters.Add("@skip", SqlDbType.Int).Value = skip
                cmd.Parameters.Add("@take", SqlDbType.Int).Value = pageSize
                cmd.CommandText = sql
                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using
            End Using
        End Using
        Return dt
    End Function

    Private Sub AddFilterWhere(ByRef sql As String, ByRef cmd As SqlCommand)
        ' Role restriction
        Dim roleRaw As String = If(My.Forms.frmmain.lblaposition.Text, "")
        Dim isEndUser As Boolean = roleRaw.Trim().Replace("-", "").Replace(" ", "").Equals("enduser", StringComparison.OrdinalIgnoreCase)
        If isEndUser Then
            sql &= " AND RFQ_OEUSER = @officeUser "
            cmd.Parameters.Add("@officeUser", SqlDbType.NVarChar, 255).Value = If(My.Forms.frmmain.lbloffice.Text, "").Trim()
        End If

        If Not String.Equals(_lastStatus, "All", StringComparison.OrdinalIgnoreCase) Then
            sql &= " AND RFQ_STATUS = @status "
            cmd.Parameters.Add("@status", SqlDbType.NVarChar, 100).Value = _lastStatus
        End If
        If Not String.Equals(_lastFCluster, "All", StringComparison.OrdinalIgnoreCase) Then
            sql &= " AND RFQ_FUNDS = @funds "
            cmd.Parameters.Add("@funds", SqlDbType.NVarChar, 150).Value = _lastFCluster
        End If
        If Not String.Equals(_lastOffice, "All", StringComparison.OrdinalIgnoreCase) Then
            sql &= " AND RFQ_OEUSER = @oe "
            cmd.Parameters.Add("@oe", SqlDbType.NVarChar, 150).Value = _lastOffice
        End If
        Dim yr As Integer
        If Not String.Equals(_lastYear, "All", StringComparison.OrdinalIgnoreCase) AndAlso Integer.TryParse(_lastYear, yr) Then
            sql &= " AND YEAR(RFQ_DATE) = @yr "
            cmd.Parameters.Add("@yr", SqlDbType.Int).Value = yr
        End If

        ' LIKE search across key cols
        If Not String.IsNullOrWhiteSpace(_lastSearch) Then
            sql &= " AND (RFQ_PRNO LIKE @q OR RFQ_PENTITY LIKE @q OR RFQ_PURPOSE LIKE @q " &
                   "      OR RFQ_OEUSER LIKE @q OR RFQ_FUNDS LIKE @q OR RFQ_MPROC LIKE @q) "
            cmd.Parameters.Add("@q", SqlDbType.NVarChar, 200).Value = "%" & _lastSearch.Trim() & "%"
        End If
    End Sub

    ' ── Search & Filters (ASYNC search) ─────────────────────────────────────
    Private Async Sub cmdsearch_Click(sender As Object, e As EventArgs) Handles cmdsearch.Click
        If _searchCts IsNot Nothing Then _searchCts.Cancel()
        _searchCts = New CancellationTokenSource()
        Try
            Await ApplySearchAsync(txtsearch.Text.Trim(), _searchCts.Token)
        Catch ex As OperationCanceledException
        Catch ex As Exception
            MessageBox.Show("Search error: " & ex.Message, "Search", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Async Sub txtsearch_KeyDown(sender As Object, e As KeyEventArgs) Handles txtsearch.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            If _searchCts IsNot Nothing Then _searchCts.Cancel()
            _searchCts = New CancellationTokenSource()
            Try
                Await ApplySearchAsync(txtsearch.Text.Trim(), _searchCts.Token)
            Catch ex As OperationCanceledException
            Catch ex As Exception
                MessageBox.Show("Search error: " & ex.Message, "Search", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Async Sub Txtsearch_TextChanged(sender As Object, e As EventArgs) Handles txtsearch.TextChanged
        If _searchCts IsNot Nothing Then _searchCts.Cancel()
        _searchCts = New CancellationTokenSource()
        Dim ct = _searchCts.Token
        Try
            Await Task.Delay(SearchDebounceMs, ct)
            If ct.IsCancellationRequested Then Return
            Await ApplySearchAsync(txtsearch.Text.Trim(), ct)
        Catch ex As TaskCanceledException
        Catch ex As OperationCanceledException
        Catch ex As Exception
            MessageBox.Show("Search error: " & ex.Message, "Search", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Async Function ApplySearchAsync(value As String, Optional ct As CancellationToken = Nothing) As Task
        _lastSearch = If(value, "").Trim()
        _lastStatus = If(combostatus.SelectedItem IsNot Nothing, combostatus.SelectedItem.ToString(), "All")
        _lastFCluster = If(Combofcluster.SelectedItem IsNot Nothing, Combofcluster.SelectedItem.ToString(), "All")
        _lastOffice = If(Combooffice.SelectedItem IsNot Nothing, Combooffice.SelectedItem.ToString(), "All")
        _lastYear = If(Comboyear.SelectedItem IsNot Nothing, Comboyear.SelectedItem.ToString(), "All")
        Await RefreshPageAsync(resetPage:=True, ct:=ct)
    End Function

    Private Async Function RefreshPageAsync(Optional resetPage As Boolean = False, Optional ct As CancellationToken = Nothing) As Task
        Try
            If resetPage Then _pageIndex = 1

            _totalRows = Await GetTotalCountAsync(ct)
            _totalPages = Math.Max(1, CInt(Math.Ceiling(_totalRows / CDbl(PageSize))))
            If _pageIndex > _totalPages Then _pageIndex = _totalPages
            If _pageIndex < 1 Then _pageIndex = 1

            Dim dt As DataTable = Await GetPageDataAsync(_pageIndex, PageSize, ct)
            If ct.IsCancellationRequested Then Return

            DataGridView1.DataSource = dt
            UpdatePagerUI()

        Catch ex As OperationCanceledException
            Return
        Catch ex As SqlException
            Dim msg = ex.Message.ToLowerInvariant()
            If ct.IsCancellationRequested _
               OrElse msg.Contains("operation cancelled by user") _
               OrElse msg.Contains("a severe error occurred on the current command") Then
                Return
            End If
            MessageBox.Show("Paging error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Catch ex As Exception
            If ct.IsCancellationRequested Then Return
            MessageBox.Show("Paging error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Function

    Private Async Function GetTotalCountAsync(Optional ct As CancellationToken = Nothing) As Task(Of Integer)
        Using conn As New SqlConnection(frmmain.txtdb.Text)
            Await conn.OpenAsync(ct)
            Dim sql As String = "SELECT COUNT(1) FROM TBL_RFQ WITH (NOLOCK) WHERE 1=1 "
            Using cmd As New SqlCommand() With {.Connection = conn}
                AddFilterWhere(sql, cmd)
                cmd.CommandText = sql
                Dim o = Await cmd.ExecuteScalarAsync(ct)
                Return If(o Is Nothing OrElse IsDBNull(o), 0, Convert.ToInt32(o))
            End Using
        End Using
    End Function

    Private Async Function GetPageDataAsync(pageIndex As Integer, pageSize As Integer, Optional ct As CancellationToken = Nothing) As Task(Of DataTable)
        Dim dt As New DataTable()
        Using conn As New SqlConnection(frmmain.txtdb.Text)
            Await conn.OpenAsync(ct)
            Dim sql As String =
                "SELECT ID, RFQ_DATE, RFQ_PRNO, RFQ_PENTITY, RFQ_PURPOSE, " &
                "       RFQ_OEUSER, RFQ_FUNDS, RFQ_MPROC, RFQ_QSDATE, RFQ_STATUS " &
                "FROM TBL_RFQ WITH (NOLOCK) WHERE 1=1 "
            Using cmd As New SqlCommand() With {.Connection = conn}
                AddFilterWhere(sql, cmd)
                sql &= " ORDER BY RFQ_DATE DESC, ID DESC " &
                       " OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;"
                Dim skip As Integer = (pageIndex - 1) * pageSize
                cmd.Parameters.Add("@skip", SqlDbType.Int).Value = skip
                cmd.Parameters.Add("@take", SqlDbType.Int).Value = pageSize
                cmd.CommandText = sql
                Using rdr As SqlDataReader = Await cmd.ExecuteReaderAsync(ct)
                    dt.Load(rdr)
                End Using
            End Using
        End Using
        Return dt
    End Function

    ' ── Filter events ─────────────────────────────────────────────────────────
    Private Sub combostatus_SelectedIndexChanged(sender As Object, e As EventArgs) Handles combostatus.SelectedIndexChanged
        If _suppressFilterEvents Then Return
        _lastStatus = If(combostatus.SelectedItem IsNot Nothing, combostatus.SelectedItem.ToString(), "All")
        RefreshPage(resetPage:=True)
    End Sub

    Private Sub Combofcluster_SelectedIndexChanged(sender As Object, e As EventArgs) Handles Combofcluster.SelectedIndexChanged
        If _suppressFilterEvents Then Return
        _lastFCluster = If(Combofcluster.SelectedItem IsNot Nothing, Combofcluster.SelectedItem.ToString(), "All")
        RefreshPage(resetPage:=True)
    End Sub

    Private Sub Combooffice_SelectedIndexChanged(sender As Object, e As EventArgs) Handles Combooffice.SelectedIndexChanged
        If _suppressFilterEvents Then Return
        _lastOffice = If(Combooffice.SelectedItem IsNot Nothing, Combooffice.SelectedItem.ToString(), "All")
        RefreshPage(resetPage:=True)
    End Sub

    Private Sub Comboyear_SelectedIndexChanged(sender As Object, e As EventArgs) Handles Comboyear.SelectedIndexChanged
        If _suppressFilterEvents Then Return
        _lastYear = If(Comboyear.SelectedItem IsNot Nothing, Comboyear.SelectedItem.ToString(), "All")
        RefreshPage(resetPage:=True)
    End Sub

    ' Lock typing in combos
    Private Sub Combofcluster_KeyPress(sender As Object, e As KeyPressEventArgs) Handles Combofcluster.KeyPress
        e.Handled = True
    End Sub
    Private Sub Combooffice_KeyPress(sender As Object, e As KeyPressEventArgs) Handles Combooffice.KeyPress
        e.Handled = True
    End Sub
    Private Sub Comboyear_KeyPress(sender As Object, e As KeyPressEventArgs) Handles Comboyear.KeyPress
        e.Handled = True
    End Sub
    Private Sub combostatus_KeyPress(sender As Object, e As KeyPressEventArgs) Handles combostatus.KeyPress
        e.Handled = True
    End Sub

    ' ── Pager buttons & label ─────────────────────────────────────────────────
    Private Sub cmdprev_Click(sender As Object, e As EventArgs) Handles cmdprev.Click
        If _pageIndex > 1 Then
            _pageIndex -= 1
            RefreshPage()
        End If
    End Sub

    Private Sub cmdnext_Click(sender As Object, e As EventArgs) Handles cmdnext.Click
        If _pageIndex < _totalPages Then
            _pageIndex += 1
            RefreshPage()
        End If
    End Sub

    Private Sub UpdatePagerUI()
        If _totalPages < 1 Then _totalPages = 1
        If _pageIndex < 1 Then _pageIndex = 1
        If _pageIndex > _totalPages Then _pageIndex = _totalPages
        cmdprev.Enabled = (_pageIndex > 1 AndAlso _totalPages > 1)
        cmdnext.Enabled = (_pageIndex < _totalPages)
        lblpage.Text = $"Page {_pageIndex} of {_totalPages} • {_totalRows} row(s)"
    End Sub

    ' ── Right-click helpers ───────────────────────────────────────────────────
    Private Sub DataGridView1_CellMouseDown(sender As Object, e As DataGridViewCellMouseEventArgs)
        If e.Button = MouseButtons.Right AndAlso e.RowIndex >= 0 AndAlso e.ColumnIndex >= 0 Then
            DataGridView1.ClearSelection()
            DataGridView1.Rows(e.RowIndex).Selected = True
            DataGridView1.CurrentCell = DataGridView1.Rows(e.RowIndex).Cells(e.ColumnIndex)
        End If
    End Sub

    Private Sub ctxMenu_Opening(sender As Object, e As System.ComponentModel.CancelEventArgs)
        Dim hasRow As Boolean = (DataGridView1.SelectedRows.Count > 0)
        mnuOpen.Enabled = hasRow
        mnuDelete.Enabled = hasRow
        If Not hasRow Then e.Cancel = True
    End Sub

    Private Sub mnuOpen_Click(sender As Object, e As EventArgs) Handles mnuOpen.Click
        Cmdopen_Click(Nothing, EventArgs.Empty)
    End Sub

    Private Sub mnuDelete_Click(sender As Object, e As EventArgs) Handles mnuDelete.Click
        Cmddelete_Click(Nothing, EventArgs.Empty)
    End Sub

    ' ——— DeleteRFQ / OpenRFQ (your existing logic, kept) ————————————————
    Public Sub DeleteRFQ()
        Try
            If DataGridView1.SelectedRows.Count = 0 Then
                MessageBox.Show("Please select one or more RFQs to delete.", "No Selection",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Dim nonPending As New List(Of String)
            For Each row As DataGridViewRow In DataGridView1.SelectedRows
                Dim status = row.Cells("RFQ_STATUS").Value?.ToString().Trim()
                Dim prno = row.Cells("RFQ_PRNO").Value?.ToString()
                If status <> "Processing" Then
                    nonPending.Add(prno)
                End If
            Next
            If nonPending.Count > 0 Then
                MessageBox.Show("Cannot delete RFQ(s) for these PR Number(s) because their status is not Pending:" &
                                vbCrLf & String.Join(", ", nonPending),
                                "Delete Blocked", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Dim rfqIds As New List(Of Integer)
            Dim prNos As New List(Of String)
            For Each row As DataGridViewRow In DataGridView1.SelectedRows
                Dim rawId = row.Cells("ID").Value
                Dim rawPr = row.Cells("RFQ_PRNO").Value
                Dim idVal As Integer
                If rawId IsNot Nothing AndAlso Integer.TryParse(rawId.ToString(), idVal) Then
                    rfqIds.Add(idVal)
                    prNos.Add(rawPr?.ToString())
                End If
            Next
            If rfqIds.Count = 0 Then
                MessageBox.Show("No valid RFQ rows selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            Using conn As New SqlConnection(frmmain.txtdb.Text)
                conn.Open()

                Dim blocked As New List(Of String)
                Dim keepIds As New List(Of Integer)
                Dim keepPrs As New List(Of String)
                For i As Integer = 0 To rfqIds.Count - 1
                    Dim prNo = prNos(i)
                    Using cmdCheck As New SqlCommand(
                        "SELECT COUNT(1) FROM TBL_ABSTRACT WHERE ABS_PRNO = @prno;", conn)
                        cmdCheck.Parameters.AddWithValue("@prno", prNo)
                        If CInt(cmdCheck.ExecuteScalar()) > 0 Then
                            blocked.Add(prNo)
                        Else
                            keepIds.Add(rfqIds(i))
                            keepPrs.Add(prNo)
                        End If
                    End Using
                Next

                If blocked.Count > 0 Then
                    MessageBox.Show("Cannot delete RFQ(s) for these PR Number(s) because they have an existing Abstract of Canvass:" &
                                    vbCrLf & String.Join(", ", blocked),
                                    "Delete Blocked", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If
                If keepIds.Count = 0 Then Return

                Dim msg = $"Are you sure you want to delete these RFQ(s)?{vbCrLf}{String.Join(", ", keepPrs)}"
                If MessageBox.Show(msg, "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                    Return
                End If

                Using tran = conn.BeginTransaction()
                    For idx As Integer = 0 To keepIds.Count - 1
                        Dim rfqId = keepIds(idx)
                        Dim prNo = keepPrs(idx)

                        Dim prId As Integer
                        Using cmdGet As New SqlCommand(
                            "SELECT RFQ_PRID FROM TBL_RFQ WHERE ID = @id;", conn, tran)
                            cmdGet.Parameters.AddWithValue("@id", rfqId)
                            prId = Convert.ToInt32(cmdGet.ExecuteScalar())
                        End Using

                        Using cmdDelItems As New SqlCommand(
                            "DELETE FROM TBL_RFQITEMS WHERE PRID = @prid;", conn, tran)
                            cmdDelItems.Parameters.AddWithValue("@prid", prId)
                            cmdDelItems.ExecuteNonQuery()
                        End Using

                        Using cmdDelSup As New SqlCommand(
                            "DELETE FROM TBL_RFQSUPPLIER WHERE RFQ_PRID = @prid;", conn, tran)
                            cmdDelSup.Parameters.AddWithValue("@prid", prId)
                            cmdDelSup.ExecuteNonQuery()
                        End Using

                        Using cmdDelHdr As New SqlCommand(
                            "DELETE FROM TBL_RFQ WHERE ID = @id;", conn, tran)
                            cmdDelHdr.Parameters.AddWithValue("@id", rfqId)
                            cmdDelHdr.ExecuteNonQuery()
                        End Using

                        Using cmdUpdPr As New SqlCommand(
                            "UPDATE TBL_PR SET PRSTATUS = '[P.R] Approved' WHERE ID = @prid;", conn, tran)
                            cmdUpdPr.Parameters.AddWithValue("@prid", prId)
                            cmdUpdPr.ExecuteNonQuery()
                        End Using
                    Next
                    tran.Commit()
                End Using
            End Using

            NotificationHelper.AddNotification(
                $"Request for Quotation for PR Number(s): {String.Join(", ", prNos)} " &
                $"has been deleted by {frmmain.lblaname.Text.Trim()}.")

            RefreshPage(resetPage:=False)
            MessageBox.Show("Selected RFQ(s) deleted.", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MessageBox.Show("Error deleting RFQ(s): " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Public Sub OpenRFQ()
        Try
            frmrfqfile.Dispose()
            If DataGridView1.SelectedRows.Count = 0 Then
                MessageBox.Show("Please select an RFQ to open.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Dim rawId = DataGridView1.SelectedRows(0).Cells("ID").Value
            Dim rfqId As Integer
            If rawId Is Nothing OrElse Not Integer.TryParse(rawId.ToString(), rfqId) Then
                MessageBox.Show("Invalid RFQ ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            Using detail As New frmrfqfile()
                detail.cmdsave.Text = "Update"
                detail.LoadRFQData(rfqId)
                detail.ShowDialog()
            End Using

        Catch ex As Exception
            MessageBox.Show("An error occurred while opening the RFQ: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ── Buttons wired to existing methods ─────────────────────────────────────
    Private Sub Cmdnew_Click(sender As Object, e As EventArgs) Handles cmdnew.Click
        Try
            frmrfqadd.Dispose()
            frmrfqadd.ShowDialog()
        Catch
        End Try
    End Sub

    Private Sub Cmddelete_Click(sender As Object, e As EventArgs) Handles cmddelete.Click
        DeleteRFQ()
    End Sub

    Private Sub Cmdopen_Click(sender As Object, e As EventArgs) Handles cmdopen.Click
        OpenRFQ()
    End Sub

    ' REFRESH → reset to All/All/All + current year, clear search, reload
    Private Sub cmdrefresh_Click(sender As Object, e As EventArgs) Handles cmdrefresh.Click
        Try
            _suppressFilterEvents = True
            LoadStatuses()
            LoadFClusters()
            LoadOffices()
            LoadYears()

            Dim curYear As String = DateTime.Now.Year.ToString()
            SafeSelect(combostatus, "All")
            SafeSelect(Combofcluster, "All")
            SafeSelect(Combooffice, "All")
            SafeSelect(Comboyear, curYear)
            txtsearch.Clear()
            _suppressFilterEvents = False

            _lastSearch = ""
            _lastStatus = "All"
            _lastFCluster = "All"
            _lastOffice = "All"
            _lastYear = curYear

            RefreshPage(resetPage:=True)
        Catch ex As Exception
            _suppressFilterEvents = False
            MessageBox.Show("Refresh error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
End Class
