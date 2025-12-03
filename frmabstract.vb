Imports System.Data
Imports System.Data.SqlClient
Imports System.Threading
Imports System.Threading.Tasks

Public Class frmabstract
    Private WithEvents ctxMenu As ContextMenuStrip
    Private WithEvents mnuOpen As ToolStripMenuItem
    Private WithEvents mnuDelete As ToolStripMenuItem

    ' ── Filters ─────────────────────────────────────────────────────────────
    Private _lastSearch As String = ""
    Private _lastStatus As String = "All"
    Private _lastMProc As String = "All"          ' ABS_MPROC
    Private _lastOffice As String = "All"         ' ABS_EUSER
    Private _lastYear As String = "All"           ' YEAR(ABS_DABSTRACT)
    Private _suppressFilterEvents As Boolean = False

    ' ── Paging ─────────────────────────────────────────────────────────────
    Private Const PageSize As Integer = 100
    Private _pageIndex As Integer = 1
    Private _totalRows As Integer = 0
    Private _totalPages As Integer = 1

    ' ── Async search (debounce + cancellation) ─────────────────────────────
    Private _searchCts As CancellationTokenSource
    Private Const SearchDebounceMs As Integer = 250

    Private Sub Frmabstract_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            ' Grid base config
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
            DataGridView1.ColumnCount = 11
            With DataGridView1
                .Columns(0).Name = "ID" : .Columns(0).HeaderText = "ID" : .Columns(0).DataPropertyName = "ID" : .Columns(0).Visible = False
                .Columns(1).Name = "ABS_PRID" : .Columns(1).HeaderText = "PRID" : .Columns(1).DataPropertyName = "ABS_PRID" : .Columns(1).Visible = False

                .Columns(2).Name = "ABS_ABSNO" : .Columns(2).HeaderText = "AOBR NUMBER" : .Columns(2).DataPropertyName = "ABS_ABSNO" : .Columns(2).Width = 160
                .Columns(3).Name = "ABS_PRNO" : .Columns(3).HeaderText = "PR NUMBER" : .Columns(3).DataPropertyName = "ABS_PRNO" : .Columns(3).Width = 160
                .Columns(4).Name = "ABS_EUSER" : .Columns(4).HeaderText = "OFFICE/SECTION (END USER)" : .Columns(4).DataPropertyName = "ABS_EUSER" : .Columns(4).Width = 220
                .Columns(5).Name = "ABS_MPROC" : .Columns(5).HeaderText = "MODE OF PROCUREMENT" : .Columns(5).DataPropertyName = "ABS_MPROC" : .Columns(5).Width = 220
                .Columns(6).Name = "ABS_PURPOSE" : .Columns(6).HeaderText = "PURPOSE" : .Columns(6).DataPropertyName = "ABS_PURPOSE" : .Columns(6).Width = 380 : .Columns(6).DefaultCellStyle.WrapMode = DataGridViewTriState.True
                .Columns(7).Name = "ABS_APPBUDGET" : .Columns(7).HeaderText = "APPROVED BUDGET" : .Columns(7).DataPropertyName = "ABS_APPBUDGET" : .Columns(7).Width = 220 : .Columns(7).DefaultCellStyle.Format = "N2"
                .Columns(8).Name = "ABS_AWARDEE" : .Columns(8).HeaderText = "AWARDEE" : .Columns(8).DataPropertyName = "ABS_AWARDEE" : .Columns(8).Width = 220
                .Columns(9).Name = "ABS_ATOTAL" : .Columns(9).HeaderText = "BID AMOUNT" : .Columns(9).DataPropertyName = "ABS_ATOTAL" : .Columns(9).Width = 160 : .Columns(9).DefaultCellStyle.Format = "N2"
                .Columns(10).Name = "ABS_STATUS" : .Columns(10).HeaderText = "STATUS" : .Columns(10).DataPropertyName = "ABS_STATUS" : .Columns(10).Width = 200
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

            ' Context menu
            ctxMenu = New ContextMenuStrip()
            mnuOpen = New ToolStripMenuItem("Open") With {.Name = "mnuOpen"}
            mnuDelete = New ToolStripMenuItem("Delete") With {.Name = "mnuDelete"}
            ctxMenu.Items.AddRange(New ToolStripItem() {mnuOpen, mnuDelete})
            DataGridView1.ContextMenuStrip = ctxMenu
            AddHandler DataGridView1.CellMouseDown, AddressOf DataGridView1_CellMouseDown
            AddHandler ctxMenu.Opening, AddressOf ctxMenu_Opening

            ' Load filters + first page
            _suppressFilterEvents = True
            LoadStatuses()
            LoadMProc()
            LoadOffices()
            LoadYears()

            Dim curYear As String = DateTime.Now.Year.ToString()
            SafeSelect(combostatus, "All")
            SafeSelect(Combomproc, "All")
            SafeSelect(Combooffice, "All")
            SafeSelect(Comboyear, curYear)

            _lastStatus = "All"
            _lastMProc = "All"
            _lastOffice = "All"
            _lastYear = curYear
            _suppressFilterEvents = False

            _pageIndex = 1
            RefreshPage(resetPage:=True)

        Catch ex As Exception
            MessageBox.Show("Form Load Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ── Combo loaders ───────────────────────────────────────────────────────
    Private Sub LoadStatuses()
        Try
            combostatus.Items.Clear()
            combostatus.Items.Add("All")
            Using conn As New SqlConnection(frmmain.txtdb.Text)
                conn.Open()
                Using cmd As New SqlCommand("
                    SELECT DISTINCT ABS_STATUS
                    FROM TBL_ABSTRACT
                    WHERE NULLIF(LTRIM(RTRIM(ABS_STATUS)), '') IS NOT NULL
                    ORDER BY ABS_STATUS;", conn)
                    Using rdr = cmd.ExecuteReader()
                        While rdr.Read()
                            combostatus.Items.Add(rdr("ABS_STATUS").ToString())
                        End While
                    End Using
                End Using
            End Using
            If combostatus.Items.Count > 0 Then combostatus.SelectedIndex = 0
        Catch ex As Exception
            MessageBox.Show("Failed to load statuses: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub LoadMProc()
        Try
            Combomproc.Items.Clear()
            Combomproc.Items.Add("All")
            Using conn As New SqlConnection(frmmain.txtdb.Text)
                conn.Open()
                Using cmd As New SqlCommand("
                    SELECT DISTINCT ABS_MPROC
                    FROM TBL_ABSTRACT
                    WHERE NULLIF(LTRIM(RTRIM(ABS_MPROC)), '') IS NOT NULL
                    ORDER BY ABS_MPROC;", conn)
                    Using rdr = cmd.ExecuteReader()
                        While rdr.Read()
                            Combomproc.Items.Add(rdr("ABS_MPROC").ToString())
                        End While
                    End Using
                End Using
            End Using
            Combomproc.SelectedIndex = 0
        Catch ex As Exception
            MessageBox.Show("Failed to load Modes of Procurement: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub LoadOffices()
        Try
            Combooffice.Items.Clear()
            Combooffice.Items.Add("All")
            Using conn As New SqlConnection(frmmain.txtdb.Text)
                conn.Open()
                Using cmd As New SqlCommand("
                    SELECT DISTINCT ABS_EUSER
                    FROM TBL_ABSTRACT
                    WHERE NULLIF(LTRIM(RTRIM(ABS_EUSER)), '') IS NOT NULL
                    ORDER BY ABS_EUSER;", conn)
                    Using rdr = cmd.ExecuteReader()
                        While rdr.Read()
                            Combooffice.Items.Add(rdr("ABS_EUSER").ToString())
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
                    SELECT DISTINCT YEAR(ABS_DABSTRACT) AS YR
                    FROM TBL_ABSTRACT
                    WHERE ABS_DABSTRACT IS NOT NULL
                    ORDER BY YR DESC;", conn)
                    Using rdr = cmd.ExecuteReader()
                        While rdr.Read()
                            Comboyear.Items.Add(rdr("YR").ToString())
                        End While
                    End Using
                End Using
            End Using

            ' Ensure current year exists
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
            MessageBox.Show("Failed to load Years: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ── Helpers ─────────────────────────────────────────────────────────────
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

    ' ── Filter SQL helper ───────────────────────────────────────────────────
    Private Sub AddFilterWhere(ByRef sql As String, ByRef cmd As SqlCommand)
        If Not String.Equals(_lastStatus, "All", StringComparison.OrdinalIgnoreCase) Then
            sql &= " AND ABS_STATUS = @status "
            cmd.Parameters.Add("@status", SqlDbType.NVarChar, 100).Value = _lastStatus
        End If
        If Not String.Equals(_lastMProc, "All", StringComparison.OrdinalIgnoreCase) Then
            sql &= " AND ABS_MPROC = @mproc "
            cmd.Parameters.Add("@mproc", SqlDbType.NVarChar, 150).Value = _lastMProc
        End If
        If Not String.Equals(_lastOffice, "All", StringComparison.OrdinalIgnoreCase) Then
            sql &= " AND ABS_EUSER = @office "
            cmd.Parameters.Add("@office", SqlDbType.NVarChar, 250).Value = _lastOffice
        End If
        Dim yr As Integer
        If Not String.Equals(_lastYear, "All", StringComparison.OrdinalIgnoreCase) AndAlso Integer.TryParse(_lastYear, yr) Then
            sql &= " AND YEAR(ABS_DABSTRACT) = @yr "
            cmd.Parameters.Add("@yr", SqlDbType.Int).Value = yr
        End If

        ' LIKE search across requested columns
        If Not String.IsNullOrWhiteSpace(_lastSearch) Then
            sql &= " AND (ABS_ABSNO LIKE @q OR ABS_EUSER LIKE @q OR ABS_MPROC LIKE @q " &
                   "      OR ABS_PURPOSE LIKE @q OR ABS_AWARDEE LIKE @q) "
            cmd.Parameters.Add("@q", SqlDbType.NVarChar, 200).Value = "%" & _lastSearch.Trim() & "%"
        End If
    End Sub

    ' ── Paging (sync) ───────────────────────────────────────────────────────
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
            Dim sql As String = "SELECT COUNT(1) FROM TBL_ABSTRACT WITH (NOLOCK) WHERE 1=1 "
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
                "SELECT ID, ABS_PRID, ABS_ABSNO, ABS_PRNO, ABS_EUSER, ABS_MPROC, " &
                "       ABS_PURPOSE, ABS_APPBUDGET, ABS_AWARDEE, ABS_ATOTAL, ABS_STATUS " &
                "FROM TBL_ABSTRACT WITH (NOLOCK) WHERE 1=1 "
            Using cmd As New SqlCommand() With {.Connection = conn}
                AddFilterWhere(sql, cmd)
                sql &= " ORDER BY ID DESC " &
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

    ' ── Async search pipeline (used by txtsearch & cmdsearch) ──────────────
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
        Catch ex As SqlException
            Dim msg = ex.Message.ToLowerInvariant()
            If ct.IsCancellationRequested OrElse msg.Contains("operation cancelled by user") Then Return
            MessageBox.Show("Paging error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Catch ex As Exception
            If ct.IsCancellationRequested Then Return
            MessageBox.Show("Paging error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Function

    Private Async Function GetTotalCountAsync(Optional ct As CancellationToken = Nothing) As Task(Of Integer)
        Using conn As New SqlConnection(frmmain.txtdb.Text)
            Await conn.OpenAsync(ct)
            Dim sql As String = "SELECT COUNT(1) FROM TBL_ABSTRACT WITH (NOLOCK) WHERE 1=1 "
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
                "SELECT ID, ABS_PRID, ABS_ABSNO, ABS_PRNO, ABS_EUSER, ABS_MPROC, " &
                "       ABS_PURPOSE, ABS_APPBUDGET, ABS_AWARDEE, ABS_ATOTAL, ABS_STATUS " &
                "FROM TBL_ABSTRACT WITH (NOLOCK) WHERE 1=1 "
            Using cmd As New SqlCommand() With {.Connection = conn}
                AddFilterWhere(sql, cmd)
                sql &= " ORDER BY ID DESC " &
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

    ' ── Search & filters ───────────────────────────────────────────────────
    Private Async Sub cmdsearch_Click(sender As Object, e As EventArgs) Handles cmdsearch.Click
        If _searchCts IsNot Nothing Then _searchCts.Cancel()
        _searchCts = New CancellationTokenSource()
        _lastSearch = txtsearch.Text.Trim()
        _lastStatus = If(combostatus.SelectedItem IsNot Nothing, combostatus.SelectedItem.ToString(), "All")
        _lastMProc = If(Combomproc.SelectedItem IsNot Nothing, Combomproc.SelectedItem.ToString(), "All")
        _lastOffice = If(Combooffice.SelectedItem IsNot Nothing, Combooffice.SelectedItem.ToString(), "All")
        _lastYear = If(Comboyear.SelectedItem IsNot Nothing, Comboyear.SelectedItem.ToString(), "All")
        Try
            Await RefreshPageAsync(resetPage:=True, ct:=_searchCts.Token)
        Catch
        End Try
    End Sub

    Private Async Sub txtsearch_KeyDown(sender As Object, e As KeyEventArgs) Handles txtsearch.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            cmdsearch_Click(Nothing, EventArgs.Empty)
        End If
    End Sub

    Private Async Sub txtsearch_TextChanged(sender As Object, e As EventArgs) Handles txtsearch.TextChanged
        If _suppressFilterEvents Then Return
        If _searchCts IsNot Nothing Then _searchCts.Cancel()
        _searchCts = New CancellationTokenSource()
        Dim ct = _searchCts.Token
        Try
            Await Task.Delay(SearchDebounceMs, ct)
            If ct.IsCancellationRequested Then Return
            _lastSearch = txtsearch.Text.Trim()
            _lastStatus = If(combostatus.SelectedItem IsNot Nothing, combostatus.SelectedItem.ToString(), "All")
            _lastMProc = If(Combomproc.SelectedItem IsNot Nothing, Combomproc.SelectedItem.ToString(), "All")
            _lastOffice = If(Combooffice.SelectedItem IsNot Nothing, Combooffice.SelectedItem.ToString(), "All")
            _lastYear = If(Comboyear.SelectedItem IsNot Nothing, Comboyear.SelectedItem.ToString(), "All")
            Await RefreshPageAsync(resetPage:=True, ct:=ct)
        Catch
        End Try
    End Sub

    Private Sub combostatus_SelectedIndexChanged(sender As Object, e As EventArgs) Handles combostatus.SelectedIndexChanged
        If _suppressFilterEvents Then Return
        _lastStatus = If(combostatus.SelectedItem IsNot Nothing, combostatus.SelectedItem.ToString(), "All")
        RefreshPage(resetPage:=True)
    End Sub

    Private Sub Combomproc_SelectedIndexChanged(sender As Object, e As EventArgs) Handles Combomproc.SelectedIndexChanged
        If _suppressFilterEvents Then Return
        _lastMProc = If(Combomproc.SelectedItem IsNot Nothing, Combomproc.SelectedItem.ToString(), "All")
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
    Private Sub Combomproc_KeyPress(sender As Object, e As KeyPressEventArgs) Handles Combomproc.KeyPress
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

    ' ── Pager controls ─────────────────────────────────────────────────────
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

    ' ── Right-click helpers ─────────────────────────────────────────────────
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

    ' ── Buttons / wrappers ─────────────────────────────────────────────────
    Private Sub cmdrefresh_Click(sender As Object, e As EventArgs) Handles cmdrefresh.Click
        Try
            _suppressFilterEvents = True
            LoadStatuses()
            LoadMProc()
            LoadOffices()
            LoadYears()

            Dim curYear As String = DateTime.Now.Year.ToString()
            SafeSelect(combostatus, "All")
            SafeSelect(Combomproc, "All")
            SafeSelect(Combooffice, "All")
            SafeSelect(Comboyear, curYear)
            txtsearch.Clear()
            _suppressFilterEvents = False

            _lastSearch = ""
            _lastStatus = "All"
            _lastMProc = "All"
            _lastOffice = "All"
            _lastYear = curYear
            RefreshPage(resetPage:=True)
        Catch ex As Exception
            _suppressFilterEvents = False
            MessageBox.Show("Refresh error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub Cmdopen_Click(sender As Object, e As EventArgs) Handles cmdopen.Click
        OpenAbstract()
    End Sub

    Private Sub Cmddelete_Click(sender As Object, e As EventArgs) Handles cmddelete.Click
        DeleteAbstract()
    End Sub

    Private Sub cmdnew_Click(sender As Object, e As EventArgs) Handles cmdnew.Click
        Try
            Dim addForm As New frmabstractadd()
            addForm.ShowDialog()
        Catch ex As Exception
            MessageBox.Show("Error opening Add Abstract dialog: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ——— Open / Delete logic (unchanged from your version) ———
    Public Sub OpenAbstract()
        Try
            If DataGridView1.CurrentRow Is Nothing Then
                MessageBox.Show("Please select a record to open.", "Open Abstract",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Dim selID = CInt(DataGridView1.CurrentRow.Cells("ID").Value)

            Dim absPRID As Integer
            Dim absRFQID As Integer
            Dim absPRNO As String = ""
            Dim absDAbs As DateTime
            Dim absStatus As String = ""
            Dim absMproc As String = ""
            Dim absPurpose As String = ""
            Dim absAcompany As String = ""
            Dim absATotal As Decimal = 0D
            Dim absBcompany As String = ""
            Dim absBTotal As Decimal = 0D
            Dim absCcompany As String = ""
            Dim absCTotal As Decimal = 0D

            Using conn = New SqlConnection(frmmain.txtdb.Text)
                conn.Open()
                Const sql = "
SELECT
    ID,
    ABS_PRID,
    ABS_RFQID,
    ABS_PRNO,
    ABS_DABSTRACT,
    ABS_STATUS,
    ABS_MPROC,
    ABS_PURPOSE,
    ABS_ACOMPANY,
    ABS_ATOTAL,
    ABS_BCOMPANY,
    ABS_BTOTAL,
    ABS_CCOMPANY,
    ABS_CTOTAL
FROM TBL_ABSTRACT
WHERE ID = @id;"
                Using cmd = New SqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@id", selID)
                    Using rdr = cmd.ExecuteReader()
                        If Not rdr.Read() Then
                            MessageBox.Show("Record not found.", "Open Abstract",
                                            MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Return
                        End If

                        absPRID = If(rdr.IsDBNull(rdr.GetOrdinal("ABS_PRID")), 0, rdr.GetInt32(rdr.GetOrdinal("ABS_PRID")))
                        absRFQID = If(rdr.IsDBNull(rdr.GetOrdinal("ABS_RFQID")), 0, rdr.GetInt32(rdr.GetOrdinal("ABS_RFQID")))
                        absPRNO = rdr("ABS_PRNO").ToString()
                        absDAbs = If(rdr.IsDBNull(rdr.GetOrdinal("ABS_DABSTRACT")), Date.Now, rdr.GetDateTime(rdr.GetOrdinal("ABS_DABSTRACT")))
                        absStatus = rdr("ABS_STATUS").ToString()
                        absMproc = rdr("ABS_MPROC").ToString()
                        absPurpose = rdr("ABS_PURPOSE").ToString()

                        absAcompany = If(rdr.IsDBNull(rdr.GetOrdinal("ABS_ACOMPANY")), "", rdr("ABS_ACOMPANY").ToString())
                        absATotal = If(rdr.IsDBNull(rdr.GetOrdinal("ABS_ATOTAL")), 0D, rdr.GetDecimal(rdr.GetOrdinal("ABS_ATOTAL")))
                        absBcompany = If(rdr.IsDBNull(rdr.GetOrdinal("ABS_BCOMPANY")), "", rdr("ABS_BCOMPANY").ToString())
                        absBTotal = If(rdr.IsDBNull(rdr.GetOrdinal("ABS_BTOTAL")), 0D, rdr.GetDecimal(rdr.GetOrdinal("ABS_BTOTAL")))
                        absCcompany = If(rdr.IsDBNull(rdr.GetOrdinal("ABS_CCOMPANY")), "", rdr("ABS_CCOMPANY").ToString())
                        absCTotal = If(rdr.IsDBNull(rdr.GetOrdinal("ABS_CTOTAL")), 0D, rdr.GetDecimal(rdr.GetOrdinal("ABS_CTOTAL")))
                    End Using
                End Using
            End Using

            frmabstractfile.lblid.Text = selID.ToString()
            frmabstractfile.lblprid.Text = absPRID.ToString()
            frmabstractfile.lblrfqid.Text = absRFQID.ToString()
            frmabstractfile.txtprno.Text = absPRNO
            frmabstractfile.DTDAbs.Value = absDAbs
            frmabstractfile.txtstatus.Text = absStatus
            frmabstractfile.txtmproc.Text = absMproc
            frmabstractfile.txtpurpose.Text = absPurpose

            If frmabstractfile.cmdsave IsNot Nothing Then frmabstractfile.cmdsave.Text = "Update"
            frmabstractfile.cmdrefresh.Enabled = True
            frmabstractfile.cmdprint.Enabled = True
            If frmabstractfile.GetType().GetMethod("LoadAbstract") IsNot Nothing Then
                frmabstractfile.LoadAbstract()
            End If

            frmabstractfile.ShowDialog()

        Catch ex As Exception
            MessageBox.Show("Error opening abstract: " & ex.Message,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Public Sub DeleteAbstract()
        If DataGridView1.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select one or more abstracts to delete.",
                            "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim absIds As New List(Of Integer)()
        Dim prNos As New List(Of String)()
        Dim aobrNos As New List(Of String)()

        For Each row As DataGridViewRow In DataGridView1.SelectedRows
            Dim rawId = row.Cells("ID").Value
            Dim idVal As Integer
            If rawId IsNot Nothing AndAlso Integer.TryParse(rawId.ToString(), idVal) Then
                absIds.Add(idVal)
                prNos.Add(If(row.Cells("ABS_PRNO").Value, "").ToString())
                aobrNos.Add(If(row.Cells("ABS_ABSNO").Value, "").ToString())
            End If
        Next

        If absIds.Count = 0 Then
            MessageBox.Show("No valid abstracts selected.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        Dim blocked As New List(Of (AbsNo As String, PrNo As String))()
        Dim absToPr As New Dictionary(Of Integer, Integer)()

        Try
            Using conn As New SqlConnection(frmmain.txtdb.Text)
                conn.Open()

                For i As Integer = 0 To absIds.Count - 1
                    Dim absId = absIds(i)

                    Dim prId As Integer = 0
                    Using cmdGet As New SqlCommand("SELECT ABS_PRID FROM TBL_ABSTRACT WHERE ID = @absid;", conn)
                        cmdGet.Parameters.Add("@absid", SqlDbType.Int).Value = absId
                        Dim obj = cmdGet.ExecuteScalar()
                        If obj Is Nothing OrElse obj Is DBNull.Value Then Continue For
                        prId = Convert.ToInt32(obj)
                        If Not absToPr.ContainsKey(absId) Then absToPr.Add(absId, prId)
                    End Using

                    Dim existsPo As Boolean = False
                    Using cmdChk As New SqlCommand("IF EXISTS(SELECT 1 FROM TBL_PO WHERE PO_PRID = @prid) SELECT 1 ELSE SELECT 0;", conn)
                        cmdChk.Parameters.Add("@prid", SqlDbType.Int).Value = prId
                        existsPo = (Convert.ToInt32(cmdChk.ExecuteScalar()) = 1)
                    End Using

                    If existsPo Then
                        Dim thisAobr As String = If(aobrNos.ElementAtOrDefault(i), "")
                        Dim thisPrNo As String = If(prNos.ElementAtOrDefault(i), "")
                        blocked.Add((thisAobr, thisPrNo))
                    End If
                Next
            End Using
        Catch ex As Exception
            MessageBox.Show("Pre-check failed: " & ex.Message, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End Try

        If blocked.Count > 0 Then
            Dim lines = blocked.Select(Function(b) $"• AOBR: {b.AbsNo}  |  PR No: {b.PrNo}")
            Dim msgBlocked As String =
                "Cannot delete the selected Abstract(s) because a Purchase Order already exists for the linked PR:" &
                vbCrLf & vbCrLf & String.Join(vbCrLf, lines) &
                vbCrLf & vbCrLf & "Delete aborted. Please void the related PO(s) first if appropriate."
            MessageBox.Show(msgBlocked, "Delete Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim msg = $"Are you sure you want to DELETE this Abstract of Bids Number(s)?{vbCrLf}{String.Join(", ", aobrNos)}"
        If MessageBox.Show(msg, "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
            Return
        End If

        Try
            Using conn As New SqlConnection(frmmain.txtdb.Text)
                conn.Open()
                Using tran = conn.BeginTransaction()

                    For Each absId In absIds
                        Dim prId As Integer
                        If Not absToPr.TryGetValue(absId, prId) Then
                            Using cmdGet As New SqlCommand("SELECT ABS_PRID FROM TBL_ABSTRACT WHERE ID = @absid;", conn, tran)
                                cmdGet.Parameters.Add("@absid", SqlDbType.Int).Value = absId
                                prId = Convert.ToInt32(cmdGet.ExecuteScalar())
                            End Using
                        End If

                        Using cmdDelItems As New SqlCommand(
                            "DELETE FROM TBL_ABSTRACTITEMS WHERE ABSID = @absid;", conn, tran)
                            cmdDelItems.Parameters.Add("@absid", SqlDbType.Int).Value = absId
                            cmdDelItems.ExecuteNonQuery()
                        End Using

                        Using cmdDelHdr As New SqlCommand(
                            "DELETE FROM TBL_ABSTRACT WHERE ID = @absid;", conn, tran)
                            cmdDelHdr.Parameters.Add("@absid", SqlDbType.Int).Value = absId
                            cmdDelHdr.ExecuteNonQuery()
                        End Using

                        Using cmdUpdPr As New SqlCommand(
                            "UPDATE TBL_PR SET PRSTATUS = '[RFQ] Completed' WHERE ID = @prid;", conn, tran)
                            cmdUpdPr.Parameters.Add("@prid", SqlDbType.Int).Value = prId
                            cmdUpdPr.ExecuteNonQuery()
                        End Using

                        Using cmdUpdrfq As New SqlCommand(
                            "UPDATE TBL_RFQ SET RFQ_STATUS = 'Completed' WHERE RFQ_PRID = @prid;", conn, tran)
                            cmdUpdrfq.Parameters.Add("@prid", SqlDbType.Int).Value = prId
                            cmdUpdrfq.ExecuteNonQuery()
                        End Using
                    Next

                    tran.Commit()
                End Using
            End Using

            RefreshPage(resetPage:=False)
            NotificationHelper.AddNotification(
                $"Abstract of Bids No. {String.Join(", ", aobrNos)} for PR Number(s): {String.Join(", ", prNos)} " &
                $"has been deleted by {frmmain.lblaname.Text.Trim()}.")

            MessageBox.Show("Selected abstract(s) deleted successfully.", "Deleted",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MessageBox.Show("Error deleting abstracts: " & ex.Message,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' — Compatibility wrappers —
    Public Sub LoadAbstract()
        RefreshPage(resetPage:=True)
    End Sub
    Public Sub LoadAbstract(searchTerm As String, statusFilter As String)
        _lastSearch = If(searchTerm, "").Trim()
        _lastStatus = If(String.IsNullOrWhiteSpace(statusFilter), "All", statusFilter.Trim())
        RefreshPage(resetPage:=True)
    End Sub
End Class
