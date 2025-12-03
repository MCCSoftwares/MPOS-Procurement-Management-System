Imports System.Data
Imports System.Data.SqlClient

Public Class frmpr
    Private WithEvents ctxMenu As ContextMenuStrip
    Private WithEvents mnuOpen As ToolStripMenuItem
    Private WithEvents mnuDelete As ToolStripMenuItem

    ' ── Filter state ─────────────────────────────────────────────────────────────
    Private _lastSearch As String = ""
    Private _lastStatus As String = "All"
    Private _lastFCluster As String = "All"
    Private _lastOffice As String = "All"
    Private _lastYear As String = "All"

    ' Suppress SelectedIndexChanged while (re)filling combos
    Private _suppressFilterEvents As Boolean = False

    ' ── Paging state ─────────────────────────────────────────────────────────────
    Private Const PageSize As Integer = 100
    Private _pageIndex As Integer = 1
    Private _totalRows As Integer = 0
    Private _totalPages As Integer = 1

    ' ── Async search infra ──────────────────────────────────────────────────────
    Private _searchCts As Threading.CancellationTokenSource
    Private Const SearchDebounceMs As Integer = 250

    Private Sub cmdnew_Click(sender As Object, e As EventArgs) Handles cmdnew.Click
        Try
            frmprfile.Dispose()
            frmprfile.cmdadd.Enabled = False
            frmprfile.cmdedit.Enabled = False
            frmprfile.cmddelete.Enabled = False
            frmprfile.cmdprint.Enabled = False
            frmprfile.cmdaddapp.Enabled = False
            frmprfile.lbltitle.Text = "CREATE PURCHASE REQUEST"
            frmprfile.Text = "CREATE PURCHASE REQUEST"
            frmprfile.ShowDialog()
        Catch
        End Try
    End Sub

    Private Sub Frmpr_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            ' 🧹 Grid setup
            With DataGridView1
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
            End With

            ' Columns
            DataGridView1.ColumnCount = 13
            With DataGridView1
                .Columns(0).Name = "ID" : .Columns(0).HeaderText = "ID" : .Columns(0).DataPropertyName = "ID" : .Columns(0).Visible = False
                .Columns(1).Name = "PRDATE" : .Columns(1).HeaderText = "Date" : .Columns(1).DataPropertyName = "PRDATE" : .Columns(1).Width = 110
                .Columns(2).Name = "PTTYPE" : .Columns(2).HeaderText = "Transaction Type" : .Columns(2).DataPropertyName = "PTTYPE" : .Columns(2).Width = 135
                .Columns(3).Name = "CATEGORY" : .Columns(3).HeaderText = "Transaction Category" : .Columns(3).DataPropertyName = "CATEGORY" : .Columns(3).Width = 135
                .Columns(4).Name = "PRNO" : .Columns(4).HeaderText = "PR Number" : .Columns(4).DataPropertyName = "PRNO" : .Columns(4).Width = 135
                .Columns(5).Name = "ENAMES" : .Columns(5).HeaderText = "Entity Name" : .Columns(5).DataPropertyName = "ENAMES" : .Columns(5).Width = 170
                .Columns(6).Name = "FCLUSTER" : .Columns(6).HeaderText = "Fund Cluster" : .Columns(6).DataPropertyName = "FCLUSTER" : .Columns(6).Width = 120
                .Columns(7).Name = "OFFSEC" : .Columns(7).HeaderText = "Office/Section" : .Columns(7).DataPropertyName = "OFFSEC" : .Columns(7).Width = 110
                .Columns(8).Name = "PRPURPOSE" : .Columns(8).HeaderText = "Description" : .Columns(8).DataPropertyName = "PRPURPOSE" : .Columns(8).Width = 420 : .Columns(8).DefaultCellStyle.WrapMode = DataGridViewTriState.True
                .Columns(9).Name = "I_ITOTALS" : .Columns(9).HeaderText = "Items" : .Columns(9).DataPropertyName = "I_ITOTALS" : .Columns(9).Width = 65
                .Columns(10).Name = "I_STOTALS" : .Columns(10).HeaderText = "Total Cost" : .Columns(10).DataPropertyName = "I_STOTALS" : .Columns(10).DefaultCellStyle.Format = "N2" : .Columns(10).Width = 95
                .Columns(11).Name = "REQBY" : .Columns(11).HeaderText = "Requested by" : .Columns(11).DataPropertyName = "REQBY" : .Columns(11).Width = 160
                .Columns(12).Name = "PRSTATUS" : .Columns(12).HeaderText = "Status" : .Columns(12).DataPropertyName = "PRSTATUS" : .Columns(12).Width = 180
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

            ' ── Context menu (right-click) ───────────────────────────────────────────────
            ctxMenu = New ContextMenuStrip()
            mnuOpen = New ToolStripMenuItem("Open") With {.Name = "mnuOpen"}
            mnuDelete = New ToolStripMenuItem("Delete") With {.Name = "mnuDelete"}
            ctxMenu.Items.AddRange(New ToolStripItem() {mnuOpen, mnuDelete})
            DataGridView1.ContextMenuStrip = ctxMenu
            AddHandler DataGridView1.CellMouseDown, AddressOf DataGridView1_CellMouseDown
            AddHandler ctxMenu.Opening, AddressOf ctxMenu_Opening

            ' Load filters (suppress change events during fill)
            _suppressFilterEvents = True
            LoadStatuses()
            LoadFClusters()
            LoadOffices()
            LoadYears()

            ' Default to current year BEFORE first data load
            _lastYear = DateTime.Now.Year.ToString()
            If Comboyear.Items.Contains(_lastYear) Then Comboyear.SelectedItem = _lastYear
            _suppressFilterEvents = False

            ' Sync state from controls & first page
            _lastStatus = If(combostatus.SelectedItem IsNot Nothing, combostatus.SelectedItem.ToString(), "All")
            _lastFCluster = If(Combofcluster.SelectedItem IsNot Nothing, Combofcluster.SelectedItem.ToString(), "All")
            _lastOffice = If(Combooffice.SelectedItem IsNot Nothing, Combooffice.SelectedItem.ToString(), "All")
            _lastYear = If(Comboyear.SelectedItem IsNot Nothing, Comboyear.SelectedItem.ToString(), _lastYear)

            _pageIndex = 1
            RefreshPage(resetPage:=True)

        Catch ex As Exception
            MessageBox.Show("Form Load Error: " & ex.Message)
        End Try
    End Sub

    ' ── Combo loaders ───────────────────────────────────────────────────────────
    Private Sub LoadStatuses()
        Try
            combostatus.Items.Clear()
            combostatus.Items.Add("All")
            Using conn As New SqlConnection(frmmain.txtdb.Text)
                conn.Open()
                Using cmd As New SqlCommand("
                    SELECT DISTINCT PRSTATUS
                    FROM TBL_PR WITH (NOLOCK)
                    WHERE NULLIF(LTRIM(RTRIM(PRSTATUS)), '') IS NOT NULL
                    ORDER BY PRSTATUS;", conn)
                    Using rdr = cmd.ExecuteReader()
                        While rdr.Read()
                            combostatus.Items.Add(rdr("PRSTATUS").ToString())
                        End While
                    End Using
                End Using
            End Using
            If combostatus.Items.Count > 0 Then combostatus.SelectedIndex = 0
        Catch ex As Exception
            MessageBox.Show("Failed to load statuses: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub LoadFClusters()
        Try
            Combofcluster.Items.Clear()
            Combofcluster.Items.Add("All")
            Using conn As New SqlConnection(frmmain.txtdb.Text)
                conn.Open()
                Using cmd As New SqlCommand("
                    SELECT DISTINCT FCLUSTER
                    FROM TBL_PR WITH (NOLOCK)
                    WHERE NULLIF(LTRIM(RTRIM(FCLUSTER)), '') IS NOT NULL
                    ORDER BY FCLUSTER;", conn)
                    Using r = cmd.ExecuteReader()
                        While r.Read()
                            Combofcluster.Items.Add(r("FCLUSTER").ToString())
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
                    SELECT DISTINCT OFFSEC
                    FROM TBL_PR WITH (NOLOCK)
                    WHERE NULLIF(LTRIM(RTRIM(OFFSEC)), '') IS NOT NULL
                    ORDER BY OFFSEC;", conn)
                    Using r = cmd.ExecuteReader()
                        While r.Read()
                            Combooffice.Items.Add(r("OFFSEC").ToString())
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
                    SELECT DISTINCT YEAR(PRDATE) AS YR
                    FROM TBL_PR WITH (NOLOCK)
                    WHERE PRDATE IS NOT NULL
                    ORDER BY YR DESC;", conn)
                    Using r = cmd.ExecuteReader()
                        While r.Read()
                            Comboyear.Items.Add(r("YR").ToString())
                        End While
                    End Using
                End Using
            End Using

            ' Ensure the current year exists even if no records yet
            Dim curYear As String = DateTime.Now.Year.ToString()
            If Comboyear.Items.IndexOf(curYear) < 0 Then
                If Comboyear.Items.Count > 0 AndAlso Comboyear.Items(0).ToString() = "All" Then
                    Comboyear.Items.Insert(1, curYear)
                Else
                    Comboyear.Items.Insert(0, curYear)
                End If
            End If

            Comboyear.SelectedIndex = 0 ' "All" by default during load; we select current year later when needed
        Catch ex As Exception
            MessageBox.Show("Failed to load Years: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ── Rendering helpers ───────────────────────────────────────────────────────
    Private Sub EnableDoubleBuffering(dgv As DataGridView)
        Dim dgvType = dgv.GetType()
        Dim pi = dgvType.GetProperty("DoubleBuffered", Reflection.BindingFlags.Instance Or Reflection.BindingFlags.NonPublic)
        If pi IsNot Nothing Then pi.SetValue(dgv, True, Nothing)
    End Sub

    Private Sub DataGridView1_MouseDown(sender As Object, e As MouseEventArgs) Handles DataGridView1.MouseDown
        If e.Button = MouseButtons.Right Then
            Dim hit = DataGridView1.HitTest(e.X, e.Y)
            If hit.Type = DataGridViewHitTestType.Cell AndAlso hit.RowIndex >= 0 Then
                DataGridView1.ClearSelection()
                DataGridView1.Rows(hit.RowIndex).Selected = True
                DataGridView1.CurrentCell = DataGridView1.Rows(hit.RowIndex).Cells(hit.ColumnIndex)
            End If
        End If
    End Sub

    Private Sub ContextMenu_ItemClicked(sender As Object, e As ToolStripItemClickedEventArgs)
        Select Case e.ClickedItem.Name
            Case "ctxOpen" : cmdopen_Click(Nothing, Nothing)
            Case "ctxDelete" : Cmddelete_Click(Nothing, Nothing)
        End Select
    End Sub

    ' ── Public entrypoints (compat) ─────────────────────────────────────────────
    Public Sub loadrecords()
        RefreshPage(resetPage:=True)
    End Sub

    Public Sub loadrecords(searchTerm As String, statusFilter As String, fcluster As String, office As String, yearText As String)
        _lastSearch = If(searchTerm, "").Trim()
        _lastStatus = If(String.IsNullOrWhiteSpace(statusFilter), "All", statusFilter.Trim())
        _lastFCluster = If(String.IsNullOrWhiteSpace(fcluster), "All", fcluster.Trim())
        _lastOffice = If(String.IsNullOrWhiteSpace(office), "All", office.Trim())
        _lastYear = If(String.IsNullOrWhiteSpace(yearText), "All", yearText.Trim())
        RefreshPage(resetPage:=True)
    End Sub

    ' ── Paging core (sync) ──────────────────────────────────────────────────────
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
            Dim sql As String =
                "SELECT COUNT(1) FROM TBL_PR WITH (NOLOCK) WHERE (1=1) "
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
                "SELECT ID, PRDATE, PTTYPE, PRNO, ENAMES, FCLUSTER, OFFSEC, " &
                "       PRPURPOSE, I_ITOTALS, I_STOTALS, REQBY, PRSTATUS, CATEGORY " &
                "FROM TBL_PR WITH (NOLOCK) " &
                "WHERE (1=1) "

            Using cmd As New SqlCommand() With {.Connection = conn}
                AddFilterWhere(sql, cmd)
                sql &= " ORDER BY YEAR(PRDATE) DESC, RIGHT(PRNO, 4) ASC " &
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
        If Not String.Equals(_lastStatus, "All", StringComparison.OrdinalIgnoreCase) Then
            sql &= " AND PRSTATUS = @status "
            cmd.Parameters.Add("@status", SqlDbType.NVarChar, 100).Value = _lastStatus
        End If
        If Not String.Equals(_lastFCluster, "All", StringComparison.OrdinalIgnoreCase) Then
            sql &= " AND FCLUSTER = @fcl "
            cmd.Parameters.Add("@fcl", SqlDbType.NVarChar, 150).Value = _lastFCluster
        End If
        If Not String.Equals(_lastOffice, "All", StringComparison.OrdinalIgnoreCase) Then
            sql &= " AND OFFSEC = @office "
            cmd.Parameters.Add("@office", SqlDbType.NVarChar, 150).Value = _lastOffice
        End If
        Dim yr As Integer
        If Not String.Equals(_lastYear, "All", StringComparison.OrdinalIgnoreCase) AndAlso Integer.TryParse(_lastYear, yr) Then
            sql &= " AND YEAR(PRDATE) = @yr "
            cmd.Parameters.Add("@yr", SqlDbType.Int).Value = yr
        End If
        If Not String.IsNullOrWhiteSpace(_lastSearch) Then
            sql &= " AND (PRNO LIKE @q OR ENAMES LIKE @q OR FCLUSTER LIKE @q OR OFFSEC LIKE @q " &
           "      OR PRPURPOSE LIKE @q OR REQBY LIKE @q) "
            cmd.Parameters.Add("@q", SqlDbType.NVarChar, 200).Value = "%" & _lastSearch & "%"
        End If
    End Sub

    ' ── Legacy (non-paged) overload kept for compatibility ──────────────────────
    Public Sub loadrecords(searchTerm As String, statusFilter As String)
        _lastSearch = If(searchTerm, "").Trim()
        _lastStatus = If(String.IsNullOrWhiteSpace(statusFilter), "All", statusFilter.Trim())
        LoadRecordsCore(_lastSearch, _lastStatus)
    End Sub

    Private Sub LoadRecordsCore(searchTerm As String, statusFilter As String)
        Try
            Dim dt As New DataTable()
            Using conn As New SqlConnection(frmmain.txtdb.Text)
                conn.Open()

                Dim roleRaw As String = If(My.Forms.frmmain.lblaposition.Text, "")
                Dim isEndUser As Boolean = roleRaw.Trim().Replace("-", "").Replace(" ", "").Equals("enduser", StringComparison.OrdinalIgnoreCase)

                Dim sql As String =
                    "SELECT ID, PRDATE, PTTYPE, PRNO, ENAMES, FCLUSTER, OFFSEC, " &
                    "       PRPURPOSE, I_ITOTALS, I_STOTALS, REQBY, PRSTATUS, CATEGORY " &
                    "FROM TBL_PR WITH (NOLOCK) WHERE 1=1 "

                Using cmd As New SqlCommand() With {.Connection = conn}
                    If isEndUser Then
                        sql &= "AND OFFSEC = @offsec "
                        cmd.Parameters.Add("@offsec", SqlDbType.NVarChar, 255).Value = If(My.Forms.frmmain.lbloffice.Text, "").Trim()
                    End If
                    If Not String.Equals(statusFilter, "All", StringComparison.OrdinalIgnoreCase) Then
                        sql &= "AND PRSTATUS = @status "
                        cmd.Parameters.Add("@status", SqlDbType.NVarChar, 100).Value = statusFilter
                    End If
                    If Not String.IsNullOrWhiteSpace(searchTerm) Then
                        sql &= "AND (PRNO LIKE @q OR ENAMES LIKE @q OR FCLUSTER LIKE @q OR OFFSEC LIKE @q " &
                        "     OR PRPURPOSE LIKE @q OR REQBY LIKE @q) "
                        cmd.Parameters.Add("@q", SqlDbType.NVarChar, 200).Value = "%" & searchTerm.Trim() & "%"
                    End If
                    sql &= "ORDER BY ID DESC;"
                    cmd.CommandText = sql
                    Using da As New SqlDataAdapter(cmd)
                        da.Fill(dt)
                    End Using
                End Using
            End Using
            DataGridView1.DataSource = dt
        Catch ex As Exception
            MessageBox.Show("Failed to load data: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ── Search & Filters (ASYNC search) ─────────────────────────────────────────
    Private Async Sub cmdsearch_Click(sender As Object, e As EventArgs) Handles cmdsearch.Click
        If _searchCts IsNot Nothing Then _searchCts.Cancel()
        _searchCts = New Threading.CancellationTokenSource()
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
            _searchCts = New Threading.CancellationTokenSource()
            Try
                Await ApplySearchAsync(txtsearch.Text.Trim(), _searchCts.Token)
            Catch ex As OperationCanceledException
            Catch ex As Exception
                MessageBox.Show("Search error: " & ex.Message, "Search", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    ' Debounced live search as you type
    Private Async Sub Txtsearch_TextChanged(sender As Object, e As EventArgs) Handles txtsearch.TextChanged
        If _searchCts IsNot Nothing Then _searchCts.Cancel()
        _searchCts = New Threading.CancellationTokenSource()
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

    ' ── Refresh button: reset combos to defaults + reload data ──────────────────
    Private Sub cmdrefresh_Click(sender As Object, e As EventArgs) Handles cmdrefresh.Click
        Try
            _suppressFilterEvents = True

            ' Rebuild lists
            LoadStatuses()
            LoadFClusters()
            LoadOffices()
            LoadYears()

            ' Force defaults: All/All/All + current year
            SafeSelect(combostatus, "All")
            SafeSelect(Combofcluster, "All")
            SafeSelect(Combooffice, "All")
            Dim curYear As String = DateTime.Now.Year.ToString()
            SafeSelect(Comboyear, curYear)

            _suppressFilterEvents = False

            ' Sync backing state (keep search as-is)
            _lastStatus = "All"
            _lastFCluster = "All"
            _lastOffice = "All"
            _lastYear = curYear
            _lastSearch = txtsearch.Text.Trim()

            RefreshPage(resetPage:=True)
        Catch ex As Exception
            _suppressFilterEvents = False
            MessageBox.Show("Refresh error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
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

    ' ── Pager buttons ───────────────────────────────────────────────────────────
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

    ' ── Open/Delete ─────────────────────────────────────────────────────────────
    Private Sub cmdopen_Click(sender As Object, e As EventArgs) Handles cmdopen.Click
        openpr()
    End Sub

    Private Sub Cmddelete_Click(sender As Object, e As EventArgs) Handles cmddelete.Click
        DeletePR()
    End Sub

    Public Sub DeletePR()
        If DataGridView1.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select one or more PRs to delete.", "No Selection",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim pendingIds As New List(Of Integer)
        Dim pendingNos As New List(Of String)
        Dim blockedNos As New List(Of String)

        For Each row As DataGridViewRow In DataGridView1.SelectedRows
            Dim idVal = Convert.ToInt32(row.Cells("ID").Value)
            Dim prNo = row.Cells("PRNO").Value.ToString()
            Dim status = row.Cells("PRSTATUS").Value?.ToString().Trim()

            If String.Equals(status, "Pending", StringComparison.OrdinalIgnoreCase) Then
                pendingIds.Add(idVal)
                pendingNos.Add(prNo)
            Else
                blockedNos.Add(prNo)
            End If
        Next

        If blockedNos.Count > 0 Then
            MessageBox.Show("The following PR(s) cannot be deleted because their status is not Pending:" &
                            vbCrLf & String.Join(", ", blockedNos),
                            "Cannot Delete", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
        If pendingIds.Count = 0 Then Return

        Dim msg = $"Are you sure you want to delete these Pending PR(s)?{vbCrLf}{String.Join(", ", pendingNos)}"
        If MessageBox.Show(msg, "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
            Return
        End If

        Try
            Using conn As New SqlConnection(frmmain.txtdb.Text)
                conn.Open()
                Using tran = conn.BeginTransaction()
                    Using cmdDel As New SqlCommand("DELETE FROM TBL_PR WHERE ID = @ID", conn, tran)
                        cmdDel.Parameters.Add("@ID", SqlDbType.Int)
                        For Each idVal In pendingIds
                            cmdDel.Parameters("@ID").Value = idVal
                            cmdDel.ExecuteNonQuery()
                        Next
                    End Using
                    tran.Commit()
                End Using
            End Using

            NotificationHelper.AddNotification(
                $"Purchase Request Number {vbCrLf}{String.Join(", ", pendingNos)} has been deleted by {frmmain.lblaname.Text.Trim()}.")

            loadrecords()
            MessageBox.Show("Selected Pending PR(s) deleted.", "Deleted",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show("Error deleting PR(s): " & ex.Message, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Public Sub openpr()
        Try
            frmprfile.Dispose()
            If DataGridView1.CurrentRow Is Nothing Then
                MessageBox.Show("Please select a PR first.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Dim selectedID As Integer
            If Not Integer.TryParse(DataGridView1.CurrentRow.Cells("ID").Value?.ToString(), selectedID) Then
                MessageBox.Show("Invalid record ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            With frmprfile
                .LoadPRData(selectedID)
                .LoadPRItems()
                .lbltitle.Text = "PURCHASE REQUEST | PR NO.: " & DataGridView1.SelectedCells(2).Value
                .Text = "PURCHASE REQUEST | PR NO.: " & DataGridView1.SelectedCells(2).Value
                .ShowDialog()
            End With

            frmprfile.LoadPRItems()
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub

    ' Select row on right-click before the menu opens
    Private Sub DataGridView1_CellMouseDown(sender As Object, e As DataGridViewCellMouseEventArgs)
        If e.Button = MouseButtons.Right AndAlso e.RowIndex >= 0 AndAlso e.ColumnIndex >= 0 Then
            DataGridView1.ClearSelection()
            DataGridView1.Rows(e.RowIndex).Selected = True
            DataGridView1.CurrentCell = DataGridView1.Rows(e.RowIndex).Cells(e.ColumnIndex)
        End If
    End Sub

    ' Enable/disable items and cancel menu if no valid selection
    Private Sub ctxMenu_Opening(sender As Object, e As System.ComponentModel.CancelEventArgs)
        Dim hasRow As Boolean = (DataGridView1.SelectedRows.Count > 0)
        mnuOpen.Enabled = hasRow
        mnuDelete.Enabled = hasRow
        If Not hasRow Then e.Cancel = True
    End Sub

    Private Sub mnuOpen_Click(sender As Object, e As EventArgs) Handles mnuOpen.Click
        cmdopen_Click(Nothing, EventArgs.Empty)
    End Sub

    Private Sub mnuDelete_Click(sender As Object, e As EventArgs) Handles mnuDelete.Click
        Cmddelete_Click(Nothing, EventArgs.Empty)
    End Sub

    ' ── Async helpers for search (reuses your WHERE builder) ────────────────────
    Private Async Function ApplySearchAsync(value As String, Optional ct As Threading.CancellationToken = Nothing) As Task
        _lastSearch = If(value, "").Trim()
        _lastStatus = If(combostatus.SelectedItem IsNot Nothing, combostatus.SelectedItem.ToString(), "All")
        _lastFCluster = If(Combofcluster.SelectedItem IsNot Nothing, Combofcluster.SelectedItem.ToString(), "All")
        _lastOffice = If(Combooffice.SelectedItem IsNot Nothing, Combooffice.SelectedItem.ToString(), "All")
        _lastYear = If(Comboyear.SelectedItem IsNot Nothing, Comboyear.SelectedItem.ToString(), "All")
        Await RefreshPageAsync(resetPage:=True, ct:=ct)
    End Function

    Private Async Function RefreshPageAsync(Optional resetPage As Boolean = False, Optional ct As Threading.CancellationToken = Nothing) As Task
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
            ' user typed again → prior query cancelled; ignore
            Return

        Catch ex As SqlClient.SqlException
            ' SQL Server throws these messages when a command is cancelled mid-flight
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


    Private Async Function GetTotalCountAsync(Optional ct As Threading.CancellationToken = Nothing) As Task(Of Integer)
        Using conn As New SqlConnection(frmmain.txtdb.Text)
            Await conn.OpenAsync(ct)
            Dim sql As String = "SELECT COUNT(1) FROM TBL_PR WITH (NOLOCK) WHERE (1=1) "
            Using cmd As New SqlCommand() With {.Connection = conn}
                AddFilterWhere(sql, cmd)
                cmd.CommandText = sql
                Dim o = Await cmd.ExecuteScalarAsync(ct)
                Return If(o Is Nothing OrElse IsDBNull(o), 0, Convert.ToInt32(o))
            End Using
        End Using
    End Function

    Private Async Function GetPageDataAsync(pageIndex As Integer, pageSize As Integer, Optional ct As Threading.CancellationToken = Nothing) As Task(Of DataTable)
        Dim dt As New DataTable()
        Using conn As New SqlConnection(frmmain.txtdb.Text)
            Await conn.OpenAsync(ct)

            Dim sql As String =
                "SELECT ID, PRDATE, PTTYPE, PRNO, ENAMES, FCLUSTER, OFFSEC, " &
                "       PRPURPOSE, I_ITOTALS, I_STOTALS, REQBY, PRSTATUS, CATEGORY " &
                "FROM TBL_PR WITH (NOLOCK) WHERE (1=1) "

            Using cmd As New SqlCommand() With {.Connection = conn}
                AddFilterWhere(sql, cmd)
                sql &= " ORDER BY YEAR(PRDATE) DESC, RIGHT(PRNO, 4) ASC " &
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
End Class
