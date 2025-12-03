Imports System.Data
Imports System.Data.SqlClient
Imports System.Threading
Imports System.Threading.Tasks

Public Class frmpo
    Private WithEvents ctxMenu As ContextMenuStrip

    ' ── Filter state ─────────────────────────────────────────────────────────────
    Private _lastSearch As String = ""
    Private _lastStatus As String = "All"
    Private _lastFCluster As String = "All"
    Private _lastOffice As String = "All"   ' PO_ENTITY (Procuring Entity / Office)
    Private _lastYear As String = "All"
    Private _suppressFilterEvents As Boolean = False

    ' ── Paging ──────────────────────────────────────────────────────────────────
    Private Const PageSize As Integer = 100
    Private _pageIndex As Integer = 1
    Private _totalRows As Integer = 0
    Private _totalPages As Integer = 1

    ' ── Async search (debounce + cancellation) ──────────────────────────────────
    Private _searchCts As CancellationTokenSource
    Private Const SearchDebounceMs As Integer = 250

    Private Sub Frmpo_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            ' Grid setup
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
            DataGridView1.ColumnCount = 12
            With DataGridView1
                .Columns(0).Name = "ID" : .Columns(0).HeaderText = "ID" : .Columns(0).DataPropertyName = "ID" : .Columns(0).Visible = False

                .Columns(1).Name = "PO_DATE" : .Columns(1).HeaderText = "PO DATE" : .Columns(1).DataPropertyName = "PO_DATE"
                .Columns(1).DefaultCellStyle.Format = "MM/dd/yyyy"
                .Columns(1).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft

                .Columns(2).Name = "PO_NO" : .Columns(2).HeaderText = "PO NUMBER" : .Columns(2).DataPropertyName = "PO_NO"

                .Columns(3).Name = "PO_PRNO" : .Columns(3).HeaderText = "PR NUMBER" : .Columns(3).DataPropertyName = "PO_PRNO"

                .Columns(4).Name = "PO_ENTITY" : .Columns(4).HeaderText = "PROCURING ENTITY / OFFICE" : .Columns(4).DataPropertyName = "PO_ENTITY"

                .Columns(5).Name = "PO_FCLUSTER" : .Columns(5).HeaderText = "FUND CLUSTER" : .Columns(5).DataPropertyName = "PO_FCLUSTER"
                .Columns(5).DefaultCellStyle.WrapMode = DataGridViewTriState.True

                .Columns(6).Name = "PO_PURPOSE" : .Columns(6).HeaderText = "PURPOSE" : .Columns(6).DataPropertyName = "PO_PURPOSE"

                .Columns(7).Name = "PO_CNAME" : .Columns(7).HeaderText = "SUPPLIER" : .Columns(7).DataPropertyName = "PO_CNAME"

                .Columns(8).Name = "PO_ITEMS" : .Columns(8).HeaderText = "NO. OF ITEMS" : .Columns(8).DataPropertyName = "PO_ITEMS"

                .Columns(9).Name = "PO_TCOST" : .Columns(9).HeaderText = "TOTAL COST" : .Columns(9).DataPropertyName = "PO_TCOST"
                .Columns(9).DefaultCellStyle.Format = "N2"

                .Columns(10).Name = "PO_STATUS" : .Columns(10).HeaderText = "STATUS" : .Columns(10).DataPropertyName = "PO_STATUS"

                .Columns(11).Name = "PO_PRID" : .Columns(11).HeaderText = "PRID" : .Columns(11).DataPropertyName = "PO_PRID" : .Columns(11).Visible = False
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
            ctxMenu.Items.Add("Open").Name = "ctxOpen"
            ctxMenu.Items.Add("Delete").Name = "ctxDelete"
            AddHandler ctxMenu.ItemClicked, AddressOf ContextMenu_ItemClicked
            DataGridView1.ContextMenuStrip = ctxMenu
            AddHandler DataGridView1.MouseDown, AddressOf DataGridView1_MouseDown

            ' Filters & first page
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

            _lastStatus = "All"
            _lastFCluster = "All"
            _lastOffice = "All"
            _lastYear = curYear
            _suppressFilterEvents = False

            _pageIndex = 1
            RefreshPage(resetPage:=True)

        Catch ex As Exception
            MessageBox.Show("Form Load Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
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
                    SELECT DISTINCT PO_STATUS
                    FROM TBL_PO
                    WHERE NULLIF(LTRIM(RTRIM(PO_STATUS)), '') IS NOT NULL
                    ORDER BY PO_STATUS;", conn)
                    Using rdr = cmd.ExecuteReader()
                        While rdr.Read()
                            combostatus.Items.Add(rdr("PO_STATUS").ToString())
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
                    SELECT DISTINCT PO_FCLUSTER
                    FROM TBL_PO
                    WHERE NULLIF(LTRIM(RTRIM(PO_FCLUSTER)), '') IS NOT NULL
                    ORDER BY PO_FCLUSTER;", conn)
                    Using rdr = cmd.ExecuteReader()
                        While rdr.Read()
                            Combofcluster.Items.Add(rdr("PO_FCLUSTER").ToString())
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
                    SELECT DISTINCT PO_ENTITY
                    FROM TBL_PO
                    WHERE NULLIF(LTRIM(RTRIM(PO_ENTITY)), '') IS NOT NULL
                    ORDER BY PO_ENTITY;", conn)
                    Using rdr = cmd.ExecuteReader()
                        While rdr.Read()
                            Combooffice.Items.Add(rdr("PO_ENTITY").ToString())
                        End While
                    End Using
                End Using
            End Using
            Combooffice.SelectedIndex = 0
        Catch ex As Exception
            MessageBox.Show("Failed to load Procuring Entities/Offices: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub LoadYears()
        Try
            Comboyear.Items.Clear()
            Comboyear.Items.Add("All")
            Using conn As New SqlConnection(frmmain.txtdb.Text)
                conn.Open()
                Using cmd As New SqlCommand("
                    SELECT DISTINCT YEAR(PO_DATE) AS YR
                    FROM TBL_PO
                    WHERE PO_DATE IS NOT NULL
                    ORDER BY YR DESC;", conn)
                    Using rdr = cmd.ExecuteReader()
                        While rdr.Read()
                            Comboyear.Items.Add(rdr("YR").ToString())
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
            MessageBox.Show("Failed to load Years: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ── Helpers ────────────────────────────────────────────────────────────────
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

    ' ── Paging core ────────────────────────────────────────────────────────────
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
            Dim sql As String = "SELECT COUNT(1) FROM TBL_PO WITH (NOLOCK) WHERE 1=1 "
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
                "SELECT ID, PO_DATE, PO_NO, PO_PRNO, PO_ENTITY, PO_FCLUSTER, " &
                "       PO_PURPOSE, PO_CNAME, PO_ITEMS, PO_TCOST, PO_STATUS, PO_PRID " &
                "FROM TBL_PO WITH (NOLOCK) WHERE 1=1 "
            Using cmd As New SqlCommand() With {.Connection = conn}
                AddFilterWhere(sql, cmd)
                sql &= " ORDER BY PO_DATE DESC, ID DESC " &
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
            sql &= " AND PO_STATUS = @status "
            cmd.Parameters.Add("@status", SqlDbType.NVarChar, 150).Value = _lastStatus
        End If
        If Not String.Equals(_lastFCluster, "All", StringComparison.OrdinalIgnoreCase) Then
            sql &= " AND PO_FCLUSTER = @fcl "
            cmd.Parameters.Add("@fcl", SqlDbType.NVarChar, 500).Value = _lastFCluster
        End If
        If Not String.Equals(_lastOffice, "All", StringComparison.OrdinalIgnoreCase) Then
            sql &= " AND PO_ENTITY = @office "
            cmd.Parameters.Add("@office", SqlDbType.NVarChar, 250).Value = _lastOffice
        End If
        Dim yr As Integer
        If Not String.Equals(_lastYear, "All", StringComparison.OrdinalIgnoreCase) AndAlso Integer.TryParse(_lastYear, yr) Then
            sql &= " AND YEAR(PO_DATE) = @yr "
            cmd.Parameters.Add("@yr", SqlDbType.Int).Value = yr
        End If

        ' LIKE search across: PO_NO, PO_PRNO, PO_ENTITY, PO_FCLUSTER, PO_PURPOSE, PO_CNAME
        If Not String.IsNullOrWhiteSpace(_lastSearch) Then
            sql &= " AND (PO_NO LIKE @q OR PO_PRNO LIKE @q OR PO_ENTITY LIKE @q " &
                   "      OR PO_FCLUSTER LIKE @q OR PO_PURPOSE LIKE @q OR PO_CNAME LIKE @q) "
            cmd.Parameters.Add("@q", SqlDbType.NVarChar, 200).Value = "%" & _lastSearch.Trim() & "%"
        End If
    End Sub

    ' ── Async refresh used by search box ────────────────────────────────────────
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
            Dim sql As String = "SELECT COUNT(1) FROM TBL_PO WITH (NOLOCK) WHERE 1=1 "
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
                "SELECT ID, PO_DATE, PO_NO, PO_PRNO, PO_ENTITY, PO_FCLUSTER, " &
                "       PO_PURPOSE, PO_CNAME, PO_ITEMS, PO_TCOST, PO_STATUS, PO_PRID " &
                "FROM TBL_PO WITH (NOLOCK) WHERE 1=1 "
            Using cmd As New SqlCommand() With {.Connection = conn}
                AddFilterWhere(sql, cmd)
                sql &= " ORDER BY PO_DATE DESC, ID DESC " &
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

    ' ── Search & filter events ────────────────────────────────────────────────
    Private Async Sub cmdsearch_Click(sender As Object, e As EventArgs) Handles cmdsearch.Click
        If _searchCts IsNot Nothing Then _searchCts.Cancel()
        _searchCts = New CancellationTokenSource()
        _lastSearch = txtsearch.Text.Trim()
        _lastStatus = If(combostatus.SelectedItem IsNot Nothing, combostatus.SelectedItem.ToString(), "All")
        _lastFCluster = If(Combofcluster.SelectedItem IsNot Nothing, Combofcluster.SelectedItem.ToString(), "All")
        _lastOffice = If(Combooffice.SelectedItem IsNot Nothing, Combooffice.SelectedItem.ToString(), "All")
        _lastYear = If(Comboyear.SelectedItem IsNot Nothing, Comboyear.SelectedItem.ToString(), "All")
        Try
            Await RefreshPageAsync(resetPage:=True, ct:=_searchCts.Token)
        Catch
        End Try
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
            _lastFCluster = If(Combofcluster.SelectedItem IsNot Nothing, Combofcluster.SelectedItem.ToString(), "All")
            _lastOffice = If(Combooffice.SelectedItem IsNot Nothing, Combooffice.SelectedItem.ToString(), "All")
            _lastYear = If(Comboyear.SelectedItem IsNot Nothing, Comboyear.SelectedItem.ToString(), "All")
            Await RefreshPageAsync(resetPage:=True, ct:=ct)
        Catch
        End Try
    End Sub

    Private Sub txtsearch_KeyDown(sender As Object, e As KeyEventArgs) Handles txtsearch.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            cmdsearch_Click(Nothing, EventArgs.Empty)
        End If
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

    ' ── Pager controls ─────────────────────────────────────────────────────────
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

    ' ── Context menu plumbing ──────────────────────────────────────────────────
    Private Sub ContextMenu_ItemClicked(sender As Object, e As ToolStripItemClickedEventArgs)
        Select Case e.ClickedItem.Name
            Case "ctxOpen" : OpenSelectedPO()
            Case "ctxDelete" : DeletePO()
        End Select
    End Sub

    Private Sub DataGridView1_MouseDown(sender As Object, e As MouseEventArgs)
        If e.Button = MouseButtons.Right Then
            Dim hit = DataGridView1.HitTest(e.X, e.Y)
            If hit.RowIndex >= 0 Then
                DataGridView1.ClearSelection()
                DataGridView1.Rows(hit.RowIndex).Selected = True
                DataGridView1.CurrentCell = DataGridView1.Rows(hit.RowIndex).Cells(Math.Max(0, hit.ColumnIndex))
            End If
        End If
    End Sub

    ' ── Buttons / wrappers ────────────────────────────────────────────────────
    Private Sub Cmdnew_Click(sender As Object, e As EventArgs) Handles cmdnew.Click
        Try
            frmpoadd.Dispose()
            frmpoadd.ShowDialog()
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub

    Private Sub Cmdopen_Click(sender As Object, e As EventArgs) Handles cmdopen.Click
        OpenSelectedPO()
    End Sub

    Private Sub cmddelete_Click(sender As Object, e As EventArgs) Handles cmddelete.Click
        DeletePO()
    End Sub

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

    ' ── Open / Delete implementations (your existing logic preserved) ─────────
    Private Sub OpenSelectedPO()
        Try
            frmPOFile.Dispose()
            If DataGridView1.SelectedRows.Count = 0 Then
                MessageBox.Show("Please select a Purchase Order to open.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If
            Dim rawId = DataGridView1.SelectedRows(0).Cells("ID").Value
            Dim poId As Integer
            If rawId Is Nothing OrElse Not Integer.TryParse(rawId.ToString(), poId) Then
                MessageBox.Show("Invalid PO ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
            Using poForm As New frmPOFile()
                poForm.LoadPODetails(poId)
                poForm.LoadPOItems()
                poForm.cmdsave.Text = "Update"
                poForm.ShowDialog()
            End Using
        Catch ex As Exception
            MessageBox.Show("Error opening PO: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Public Sub DeletePO()
        ' (unchanged – your existing delete logic)
        ' — paste your DeletePO method here unchanged —
        ' I left your original DeletePO below exactly as you provided
        ' so nothing in deletion flow is altered.
        ' 1) Ensure at least one row is selected
        If DataGridView1.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select one or more Purchase Orders to delete.",
                        "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim poIds As New List(Of Integer)()
        Dim poNosPicked As New List(Of String)()

        For Each row As DataGridViewRow In DataGridView1.SelectedRows
            Dim rawId = row.Cells("ID").Value
            Dim idVal As Integer
            If rawId IsNot Nothing AndAlso Integer.TryParse(rawId.ToString(), idVal) Then
                poIds.Add(idVal)
                poNosPicked.Add(row.Cells("PO_NO").Value?.ToString())
            End If
        Next

        If poIds.Count = 0 Then
            MessageBox.Show("No valid Purchase Orders selected.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        Try
            Using conn As New SqlClient.SqlConnection(My.Forms.frmmain.txtdb.Text)
                conn.Open()

                Using tran = conn.BeginTransaction(IsolationLevel.ReadCommitted)
                    Dim inParams As New List(Of String)()
                    Dim cmdCheck As New SqlClient.SqlCommand()
                    cmdCheck.Connection = conn
                    cmdCheck.Transaction = tran

                    For i As Integer = 0 To poIds.Count - 1
                        Dim pName = "@id" & i.ToString()
                        inParams.Add(pName)
                        cmdCheck.Parameters.Add(pName, SqlDbType.Int).Value = poIds(i)
                    Next

                    cmdCheck.CommandText =
                    "SELECT ID, PO_NO, PO_STATUS " &
                    "FROM TBL_PO WITH (UPDLOCK, HOLDLOCK) " &
                    $"WHERE ID IN ({String.Join(",", inParams)})"

                    Dim blocked As New List(Of String)()
                    Dim livePoMap As New Dictionary(Of Integer, (PoNo As String, Status As String))()

                    Using rdr = cmdCheck.ExecuteReader()
                        While rdr.Read()
                            Dim rid As Integer = rdr.GetInt32(0)
                            Dim pno As String = If(rdr.IsDBNull(1), "", rdr.GetString(1))
                            Dim st As String = If(rdr.IsDBNull(2), "", rdr.GetString(2))
                            livePoMap(rid) = (pno, st)

                            If String.Equals(st.Trim(), "Completed", StringComparison.OrdinalIgnoreCase) Then
                                blocked.Add(If(String.IsNullOrWhiteSpace(pno), $"ID:{rid}", pno))
                            End If
                        End While
                    End Using

                    Dim missingIds = poIds.Where(Function(x) Not livePoMap.ContainsKey(x)).ToList()
                    If missingIds.Count > 0 Then
                        tran.Rollback()
                        MessageBox.Show("Some selected Purchase Orders were not found or already deleted." & vbCrLf &
                                    "Missing IDs: " & String.Join(", ", missingIds),
                                    "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        RefreshPage(resetPage:=True)
                        Return
                    End If

                    If blocked.Count > 0 Then
                        tran.Rollback()
                        MessageBox.Show("Delete blocked. The following Purchase Order(s) are already 'Completed':" & vbCrLf &
                                    String.Join(", ", blocked),
                                    "Blocked", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        Return
                    End If

                    Dim confList = livePoMap.Values.Select(Function(t) If(String.IsNullOrWhiteSpace(t.PoNo), "(no number)", t.PoNo)).ToList()
                    Dim msg = $"Are you sure you want to DELETE these Purchase Orders?{vbCrLf}{String.Join(", ", confList)}"
                    If MessageBox.Show(msg, "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                        tran.Rollback()
                        Return
                    End If

                    For Each poId In poIds
                        Dim prId As Integer = 0
                        Using cmdGet As New SqlClient.SqlCommand("SELECT PO_PRID FROM TBL_PO WHERE ID = @poid;", conn, tran)
                            cmdGet.Parameters.Add("@poid", SqlDbType.Int).Value = poId
                            Dim obj = cmdGet.ExecuteScalar()
                            If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                                prId = Convert.ToInt32(obj)
                            End If
                        End Using

                        Using cmdDelItems As New SqlClient.SqlCommand(
                        "DELETE FROM TBL_POITEMS WHERE POID = @poid;", conn, tran)
                            cmdDelItems.Parameters.Add("@poid", SqlDbType.Int).Value = poId
                            cmdDelItems.ExecuteNonQuery()
                        End Using

                        Using cmdDelHdr As New SqlClient.SqlCommand(
                        "DELETE FROM TBL_PO WHERE ID = @poid AND UPPER(LTRIM(RTRIM(PO_STATUS))) <> 'COMPLETED';", conn, tran)
                            cmdDelHdr.Parameters.Add("@poid", SqlDbType.Int).Value = poId
                            Dim affected = cmdDelHdr.ExecuteNonQuery()
                            If affected = 0 Then
                                Throw New InvalidOperationException("Delete aborted: a selected PO became 'Completed' during the operation.")
                            End If
                        End Using

                        Using cmdUpdPr As New SqlClient.SqlCommand(
                        "UPDATE TBL_PR SET PRSTATUS = '[AOB] Completed' WHERE ID = @prid;", conn, tran)
                            cmdUpdPr.Parameters.Add("@prid", SqlDbType.Int).Value = prId
                            cmdUpdPr.ExecuteNonQuery()
                        End Using
                    Next

                    tran.Commit()
                End Using
            End Using

            RefreshPage(resetPage:=False)
            NotificationHelper.AddNotification(
                $"Purchase Order(s): {String.Join(", ", poNosPicked)} has been deleted by {My.Forms.frmmain.lblaname.Text.Trim()}.")

            MessageBox.Show("Selected Purchase Order(s) deleted successfully.", "Deleted",
                        MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MessageBox.Show("Error deleting Purchase Orders: " & ex.Message,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
End Class
