Imports System.Data
Imports System.Data.SqlClient
Imports System.Threading
Imports System.Threading.Tasks
Imports System.Drawing

Public Class frmdelivery
    ' ── Filter state ─────────────────────────────────────────────────────────────
    Private _lastSearch As String = ""
    Private _lastStatus As String = "All"     ' TBL_IAR.STATUS
    Private _lastFCluster As String = "All"   ' TBL_IAR.FCLUSTER
    Private _lastOffice As String = "All"     ' TBL_IAR.REQOFFICE
    Private _lastYear As String = "All"       ' YEAR(TBL_IAR.IARDATE)
    Private _lastTType As String = "All"      ' NEW: TBL_IAR.TTYPE
    Private _suppressFilterEvents As Boolean = False

    ' ── Paging ──────────────────────────────────────────────────────────────────
    Private Const PageSize As Integer = 100
    Private _pageIndex As Integer = 1
    Private _totalRows As Integer = 0
    Private _totalPages As Integer = 1

    ' ── Async debounce for textbox search ───────────────────────────────────────
    Private _searchCts As CancellationTokenSource
    Private Const SearchDebounceMs As Integer = 250

    ' ── UI: context menu ────────────────────────────────────────────────────────
    Private ctxMenu As ContextMenuStrip

    Private Sub frmdelivery_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            ConfigureIARGrid()
            BuildContextMenu()

            ' Load filter lists
            _suppressFilterEvents = True
            LoadStatuses()
            LoadFClusters()
            LoadOffices()
            LoadYears()
            LoadTTypes() ' NEW

            ' Defaults
            SafeSelect(combostatus, "All")
            SafeSelect(Combofcluster, "All")
            SafeSelect(Combooffice, "All")
            SafeSelect(Comboyear, "All")
            SafeSelect(combottype, "All") ' NEW

            _lastStatus = "All"
            _lastFCluster = "All"
            _lastOffice = "All"
            _lastYear = "All"
            _lastTType = "All" ' NEW
            _suppressFilterEvents = False

            _pageIndex = 1
            RefreshPage(resetPage:=True)
        Catch ex As Exception
            MessageBox.Show("Form Load Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ── GRID ────────────────────────────────────────────────────────────────────
    Private Sub ConfigureIARGrid()
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

        ' ID + TTYPE (inserted right after ID) + existing columns
        DataGridView1.ColumnCount = 15
        With DataGridView1
            ' 0) ID (hidden)
            .Columns(0).Name = "ID" : .Columns(0).HeaderText = "ID" : .Columns(0).DataPropertyName = "ID" : .Columns(0).Visible = False

            ' 1) TTYPE (Transaction Type)
            .Columns(1).Name = "TTYPE" : .Columns(1).HeaderText = "TRANSACTION TYPE" : .Columns(1).DataPropertyName = "TTYPE"

            ' 2) IARDATE
            .Columns(2).Name = "IARDATE" : .Columns(2).HeaderText = "IAR DATE" : .Columns(2).DataPropertyName = "IARDATE"
            .Columns(2).DefaultCellStyle.Format = "MM/dd/yyyy"
            .Columns(2).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft

            ' 3) IARNO
            .Columns(3).Name = "IARNO" : .Columns(3).HeaderText = "IAR NO." : .Columns(3).DataPropertyName = "IARNO"

            ' 4) PONO
            .Columns(4).Name = "PONO" : .Columns(4).HeaderText = "PO NO." : .Columns(4).DataPropertyName = "PONO"

            ' 5) REQOFFICE
            .Columns(5).Name = "REQOFFICE" : .Columns(5).HeaderText = "REQUESTING OFFICE" : .Columns(5).DataPropertyName = "REQOFFICE"

            ' 6) INVOICENO
            .Columns(6).Name = "INVOICENO" : .Columns(6).HeaderText = "INVOICE NO." : .Columns(6).DataPropertyName = "INVOICENO"

            ' 7) DINSPECTED
            .Columns(7).Name = "DINSPECTED" : .Columns(7).HeaderText = "DATE INSPECTED" : .Columns(7).DataPropertyName = "DINSPECTED"
            .Columns(7).DefaultCellStyle.Format = "MM/dd/yyyy"

            ' 8) DACCEPT
            .Columns(8).Name = "DACCEPT" : .Columns(8).HeaderText = "DATE ACCEPTED" : .Columns(8).DataPropertyName = "DACCEPT"
            .Columns(8).DefaultCellStyle.Format = "MM/dd/yyyy"

            ' 9) ASTATUS
            .Columns(9).Name = "ASTATUS" : .Columns(9).HeaderText = "ACCEPTANCE STATUS" : .Columns(9).DataPropertyName = "ASTATUS"

            ' 10) AOFFICER
            .Columns(10).Name = "AOFFICER" : .Columns(10).HeaderText = "ACCEPTANCE OFFICER" : .Columns(10).DataPropertyName = "AOFFICER"

            ' 11) DDATE
            .Columns(11).Name = "DDATE" : .Columns(11).HeaderText = "DELIVERY DATE" : .Columns(11).DataPropertyName = "DDATE"
            .Columns(11).DefaultCellStyle.Format = "MM/dd/yyyy"

            ' 12) STATUS
            .Columns(12).Name = "STATUS" : .Columns(12).HeaderText = "STATUS" : .Columns(12).DataPropertyName = "STATUS"

            ' 13) POID (hidden)
            .Columns(13).Name = "POID" : .Columns(13).HeaderText = "POID" : .Columns(13).DataPropertyName = "POID" : .Columns(13).Visible = False

            ' 14) PRID (hidden)
            .Columns(14).Name = "PRID" : .Columns(14).HeaderText = "PRID" : .Columns(14).DataPropertyName = "PRID" : .Columns(14).Visible = False
        End With

        ' Styling (matches your look)
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
    End Sub

    ' ── Context menu ────────────────────────────────────────────────────────────
    Private Sub BuildContextMenu()
        ctxMenu = New ContextMenuStrip()
        Dim mOpen = New ToolStripMenuItem("Open")
        Dim mDelete = New ToolStripMenuItem("Delete")
        AddHandler mOpen.Click, Sub() Cmdopen_Click(Nothing, EventArgs.Empty)
        AddHandler mDelete.Click, Sub() DeleteSelectedIARs()
        ctxMenu.Items.AddRange(New ToolStripItem() {mOpen, mDelete})
        DataGridView1.ContextMenuStrip = ctxMenu
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

    Private Sub DataGridView1_KeyDown(sender As Object, e As KeyEventArgs) Handles DataGridView1.KeyDown
        If e.KeyCode = Keys.Delete Then
            DeleteSelectedIARs()
            e.Handled = True
        End If
    End Sub

    ' ── Combo loaders ───────────────────────────────────────────────────────────
    Private Sub LoadStatuses()
        Try
            combostatus.Items.Clear()
            combostatus.Items.Add("All")
            Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
                conn.Open()
                Using cmd As New SqlCommand("
                    SELECT DISTINCT STATUS
                      FROM TBL_IAR WITH (NOLOCK)
                     WHERE NULLIF(LTRIM(RTRIM(STATUS)), '') IS NOT NULL
                     ORDER BY STATUS;", conn)
                    Using rdr = cmd.ExecuteReader()
                        While rdr.Read()
                            combostatus.Items.Add(rdr("STATUS").ToString())
                        End While
                    End Using
                End Using
            End Using
            If combostatus.Items.Count > 0 Then combostatus.SelectedIndex = 0
        Catch ex As Exception
            MessageBox.Show("Failed to load IAR statuses: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub LoadFClusters()
        Try
            Combofcluster.Items.Clear()
            Combofcluster.Items.Add("All")
            Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
                conn.Open()
                Using cmd As New SqlCommand("
                    SELECT DISTINCT FCLUSTER
                      FROM TBL_IAR WITH (NOLOCK)
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
            Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
                conn.Open()
                Using cmd As New SqlCommand("
                    SELECT DISTINCT REQOFFICE
                      FROM TBL_IAR WITH (NOLOCK)
                     WHERE NULLIF(LTRIM(RTRIM(REQOFFICE)), '') IS NOT NULL
                     ORDER BY REQOFFICE;", conn)
                    Using r = cmd.ExecuteReader()
                        While r.Read()
                            Combooffice.Items.Add(r("REQOFFICE").ToString())
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
            Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
                conn.Open()
                Using cmd As New SqlCommand("
                    SELECT DISTINCT YEAR(IARDATE) AS YR
                      FROM TBL_IAR WITH (NOLOCK)
                     WHERE IARDATE IS NOT NULL
                     ORDER BY YR DESC;", conn)
                    Using r = cmd.ExecuteReader()
                        While r.Read()
                            Comboyear.Items.Add(r("YR").ToString())
                        End While
                    End Using
                End Using
            End Using
            Comboyear.SelectedIndex = 0
        Catch ex As Exception
            MessageBox.Show("Failed to load Years: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' NEW: Load TTYPE values
    Private Sub LoadTTypes()
        Try
            combottype.Items.Clear()
            combottype.Items.Add("All")
            Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
                conn.Open()
                Using cmd As New SqlCommand("
                    SELECT DISTINCT TTYPE
                      FROM TBL_IAR WITH (NOLOCK)
                     WHERE NULLIF(LTRIM(RTRIM(TTYPE)), '') IS NOT NULL
                     ORDER BY TTYPE;", conn)
                    Using r = cmd.ExecuteReader()
                        While r.Read()
                            combottype.Items.Add(r("TTYPE").ToString())
                        End While
                    End Using
                End Using
            End Using
            If combottype.Items.Count > 0 Then combottype.SelectedIndex = 0
        Catch ex As Exception
            MessageBox.Show("Failed to load Transaction Types: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
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

    ' ── Paging core ─────────────────────────────────────────────────────────────
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
        Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
            conn.Open()
            Dim sql As String = "SELECT COUNT(1) FROM TBL_IAR WITH (NOLOCK) WHERE (1=1) "
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
        Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
            conn.Open()
            Dim sql As String =
                "SELECT ID, TTYPE, IARDATE, IARNO, PONO, REQOFFICE, INVOICENO, " &
                "       DINSPECTED, DACCEPT, ASTATUS, AOFFICER, DDATE, STATUS, PRID, POID " &
                "FROM TBL_IAR WITH (NOLOCK) WHERE (1=1) "

            Using cmd As New SqlCommand() With {.Connection = conn}
                AddFilterWhere(sql, cmd)
                sql &= " ORDER BY IARDATE DESC, ID DESC " &
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
        ' Status
        If Not String.Equals(_lastStatus, "All", StringComparison.OrdinalIgnoreCase) Then
            sql &= " AND STATUS = @status "
            cmd.Parameters.Add("@status", SqlDbType.NVarChar, 150).Value = _lastStatus
        End If

        ' Fund Cluster
        If Not String.Equals(_lastFCluster, "All", StringComparison.OrdinalIgnoreCase) Then
            sql &= " AND FCLUSTER = @fcluster "
            cmd.Parameters.Add("@fcluster", SqlDbType.NVarChar, 350).Value = _lastFCluster
        End If

        ' Office
        If Not String.Equals(_lastOffice, "All", StringComparison.OrdinalIgnoreCase) Then
            sql &= " AND REQOFFICE = @office "
            cmd.Parameters.Add("@office", SqlDbType.NVarChar, 350).Value = _lastOffice
        End If

        ' Year — include rows with NULL IARDATE (e.g., Pending without dates)
        Dim yr As Integer
        If Not String.Equals(_lastYear, "All", StringComparison.OrdinalIgnoreCase) AndAlso Integer.TryParse(_lastYear, yr) Then
            sql &= " AND (YEAR(IARDATE) = @yr OR IARDATE IS NULL) "
            cmd.Parameters.Add("@yr", SqlDbType.Int).Value = yr
        End If

        ' NEW: TTYPE exact filter
        If Not String.Equals(_lastTType, "All", StringComparison.OrdinalIgnoreCase) Then
            sql &= " AND TTYPE = @ttype "
            cmd.Parameters.Add("@ttype", SqlDbType.NVarChar, 150).Value = _lastTType
        End If

        ' LIKE search across fields; treat NULL as empty so they don't get excluded
        If Not String.IsNullOrWhiteSpace(_lastSearch) Then
            sql &= " AND (ISNULL(PONO,'') LIKE @q " &
               "  OR ISNULL(IARNO,'') LIKE @q " &
               "  OR ISNULL(REQOFFICE,'') LIKE @q " &
               "  OR ISNULL(INVOICENO,'') LIKE @q " &
               "  OR ISNULL(AOFFICER,'') LIKE @q " &
               "  OR ISNULL(ASTATUS,'') LIKE @q " &
               "  OR ISNULL(TTYPE,'') LIKE @q " &
               "  OR ISNULL(CNAME,'') LIKE @q) "
            cmd.Parameters.Add("@q", SqlDbType.NVarChar, 200).Value = "%" & _lastSearch.Trim() & "%"
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

    ' ── Search & filters (events) ───────────────────────────────────────────────
    Private Sub cmdsearch_Click(sender As Object, e As EventArgs) Handles cmdsearch.Click
        _lastSearch = txtsearch.Text.Trim()
        _lastStatus = If(combostatus.SelectedItem IsNot Nothing, combostatus.SelectedItem.ToString(), "All")
        _lastFCluster = If(Combofcluster.SelectedItem IsNot Nothing, Combofcluster.SelectedItem.ToString(), "All")
        _lastOffice = If(Combooffice.SelectedItem IsNot Nothing, Combooffice.SelectedItem.ToString(), "All")
        _lastYear = If(Comboyear.SelectedItem IsNot Nothing, Comboyear.SelectedItem.ToString(), "All")
        _lastTType = If(combottype.SelectedItem IsNot Nothing, combottype.SelectedItem.ToString(), "All") ' NEW
        RefreshPage(resetPage:=True)
    End Sub

    Private Async Sub txtsearch_TextChanged(sender As Object, e As EventArgs) Handles txtsearch.TextChanged
        If _searchCts IsNot Nothing Then _searchCts.Cancel()
        _searchCts = New CancellationTokenSource()
        Dim token = _searchCts.Token
        Try
            Await Task.Delay(SearchDebounceMs, token)
        Catch ex As TaskCanceledException
            Return
        End Try
        If token.IsCancellationRequested Then Return
        _lastSearch = txtsearch.Text.Trim()
        RefreshPage(resetPage:=True)
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

    ' NEW: TTYPE combo change
    Private Sub combottype_SelectedIndexChanged(sender As Object, e As EventArgs) Handles combottype.SelectedIndexChanged
        If _suppressFilterEvents Then Return
        _lastTType = If(combottype.SelectedItem IsNot Nothing, combottype.SelectedItem.ToString(), "All")
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
    ' NEW: lock typing on TTYPE combo
    Private Sub combottype_KeyPress(sender As Object, e As KeyPressEventArgs) Handles combottype.KeyPress
        e.Handled = True
    End Sub

    ' ── Refresh button ──────────────────────────────────────────────────────────
    Private Sub cmdrefresh_Click(sender As Object, e As EventArgs) Handles cmdrefresh.Click
        Try
            _suppressFilterEvents = True

            LoadStatuses()
            LoadFClusters()
            LoadOffices()
            LoadYears()
            LoadTTypes() ' NEW

            SafeSelect(combostatus, "All")
            SafeSelect(Combofcluster, "All")
            SafeSelect(Combooffice, "All")
            SafeSelect(Comboyear, "All")
            SafeSelect(combottype, "All") ' NEW
            txtsearch.Clear()

            _suppressFilterEvents = False

            _lastSearch = ""
            _lastStatus = "All"
            _lastFCluster = "All"
            _lastOffice = "All"
            _lastYear = "All"
            _lastTType = "All" ' NEW

            RefreshPage(resetPage:=True)
        Catch ex As Exception
            _suppressFilterEvents = False
            MessageBox.Show("Refresh error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
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

    ' ── Open ───────────────────────────────────────────────────────────────────
    Private Sub Cmdopen_Click(sender As Object, e As EventArgs) Handles cmdopen.Click
        Try
            If DataGridView1.SelectedRows.Count = 0 Then
                MessageBox.Show("Please select a row first.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            If Not DataGridView1.Columns.Contains("PRID") Then
                MessageBox.Show("PRID column not found.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            Dim row As DataGridViewRow = DataGridView1.SelectedRows(0) ' first selected row
            Dim prIdValue As Object = row.Cells("PRID").Value

            If prIdValue Is Nothing OrElse IsDBNull(prIdValue) Then
                MessageBox.Show("Selected record has no PR ID.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            Dim prId As Integer
            If Not Integer.TryParse(prIdValue.ToString(), prId) Then
                MessageBox.Show("Invalid PR ID value.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            Dim frm As New frmdeliveryfile()
            frm.lblprid.Text = prId.ToString()
            frm.LoadIAR()
            frm.ShowDialog(Me)
        Catch ex As Exception
            MessageBox.Show("Error opening IAR record: " & ex.ToString, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ── Delete (button + context menu + Delete key) ─────────────────────────────
    Private Sub Cmddelete_Click(sender As Object, e As EventArgs) Handles cmddelete.Click
        DeleteSelectedIARs()
    End Sub

    Private Sub DeleteSelectedIARs()
        If DataGridView1.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select one or more IAR records to delete.", "No Selection",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        ' Collect selected IAR IDs
        Dim iarIds As New List(Of Integer)
        For Each row As DataGridViewRow In DataGridView1.SelectedRows
            Dim val = row.Cells("ID").Value
            Dim id As Integer
            If val IsNot Nothing AndAlso Integer.TryParse(val.ToString(), id) Then
                iarIds.Add(id)
            End If
        Next
        If iarIds.Count = 0 Then Exit Sub

        If MessageBox.Show($"Delete {iarIds.Count} selected IAR record(s)? This will also delete its items and revert related statuses.",
                           "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
            Exit Sub
        End If

        Dim deleted As Integer = 0
        Dim skippedCompleted As Integer = 0

        Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
            conn.Open()
            Using tran = conn.BeginTransaction()
                Try
                    ' Ensure consistent behavior on errors
                    Using cmdOn As New SqlCommand("SET XACT_ABORT ON; SET NOCOUNT ON;", conn, tran)
                        cmdOn.ExecuteNonQuery()
                    End Using

                    For Each id In iarIds
                        ' Lock row and fetch fresh values from DB (don’t rely on grid)
                        Dim prid As Object = Nothing, poid As Object = Nothing
                        Dim status As String = "", ttype As String = ""

                        Using cmd As New SqlCommand("
                        SELECT PRID, POID, STATUS, TTYPE
                        FROM TBL_IAR WITH (UPDLOCK, ROWLOCK)
                        WHERE ID = @id;", conn, tran)
                            cmd.Parameters.Add("@id", SqlDbType.Int).Value = id
                            Using r = cmd.ExecuteReader()
                                If r.Read() Then
                                    prid = r("PRID")
                                    poid = r("POID")
                                    status = If(IsDBNull(r("STATUS")), "", r("STATUS").ToString())
                                    ttype = If(IsDBNull(r("TTYPE")), "", r("TTYPE").ToString())
                                Else
                                    ' Already gone; skip
                                    Continue For
                                End If
                            End Using
                        End Using

                        ' Respect completed records
                        If String.Equals(status, "Completed", StringComparison.OrdinalIgnoreCase) Then
                            skippedCompleted += 1
                            Continue For
                        End If

                        ' 1) Delete IAR items by IARID (safer than PRID)
                        Using cmd As New SqlCommand("
                        DELETE FROM TBL_IARITEMS WHERE IARID = @iarid;", conn, tran)
                            cmd.Parameters.Add("@iarid", SqlDbType.Int).Value = id
                            cmd.ExecuteNonQuery()
                        End Using

                        ' 2) Delete IAR header
                        Using cmd As New SqlCommand("
                        DELETE FROM TBL_IAR WHERE ID = @id;", conn, tran)
                            cmd.Parameters.Add("@id", SqlDbType.Int).Value = id
                            cmd.ExecuteNonQuery()
                        End Using

                        ' 3) Revert statuses depending on TTYPE
                        If prid IsNot Nothing AndAlso Not IsDBNull(prid) Then
                            Dim iPrId As Integer = Convert.ToInt32(prid)

                            If String.Equals(ttype, "Direct Payment", StringComparison.OrdinalIgnoreCase) Then
                                ' Original behavior for DP
                                Using cmd As New SqlCommand("
                                UPDATE TBL_PR
                                SET PRSTATUS = N'[P.O] Processing'
                                WHERE ID = @prid;", conn, tran)
                                    cmd.Parameters.Add("@prid", SqlDbType.Int).Value = iPrId
                                    cmd.ExecuteNonQuery()
                                End Using

                                ' Update PO status only for DP (if present)
                                If poid IsNot Nothing AndAlso Not IsDBNull(poid) Then
                                    Using cmd As New SqlCommand("
                                    UPDATE TBL_PO
                                    SET PO_STATUS = N'Processing'
                                    WHERE ID = @poid;", conn, tran)
                                        cmd.Parameters.Add("@poid", SqlDbType.Int).Value = Convert.ToInt32(poid)
                                        cmd.ExecuteNonQuery()
                                    End Using
                                End If

                            ElseIf String.Equals(ttype, "Cash Advance", StringComparison.OrdinalIgnoreCase) _
                                OrElse String.Equals(ttype, "Reimbursement", StringComparison.OrdinalIgnoreCase) Then

                                ' New behavior for CA/Reimb
                                Using cmd As New SqlCommand("
                                UPDATE TBL_PR
                                SET PRSTATUS = N'[AOB] Processing'
                                WHERE ID = @prid;", conn, tran)
                                    cmd.Parameters.Add("@prid", SqlDbType.Int).Value = iPrId
                                    cmd.ExecuteNonQuery()
                                End Using

                                Using cmd As New SqlCommand("
                                UPDATE TBL_ABSTRACT
                                SET ABS_STATUS = N'Pending'
                                WHERE ABS_PRID = @prid;", conn, tran)
                                    cmd.Parameters.Add("@prid", SqlDbType.Int).Value = iPrId
                                    cmd.ExecuteNonQuery()
                                End Using

                                ' No PO update for CA/Reimb

                            Else
                                ' Fallback: treat like DP (optional). You can remove this else to do nothing.
                                Using cmd As New SqlCommand("
                                UPDATE TBL_PR
                                SET PRSTATUS = N'[P.O] Processing]'
                                WHERE ID = @prid;", conn, tran)
                                    cmd.Parameters.Add("@prid", SqlDbType.Int).Value = iPrId
                                    cmd.ExecuteNonQuery()
                                End Using
                                If poid IsNot Nothing AndAlso Not IsDBNull(poid) Then
                                    Using cmd As New SqlCommand("
                                    UPDATE TBL_PO
                                    SET PO_STATUS = N'Processing'
                                    WHERE ID = @poid;", conn, tran)
                                        cmd.Parameters.Add("@poid", SqlDbType.Int).Value = Convert.ToInt32(poid)
                                        cmd.ExecuteNonQuery()
                                    End Using
                                End If
                            End If
                        End If

                        deleted += 1
                    Next

                    tran.Commit()

                Catch ex As Exception
                    Try : tran.Rollback() : Catch : End Try
                    MessageBox.Show("Delete error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Exit Sub
                End Try
            End Using
        End Using

        Dim msg As String = $"{deleted} record(s) deleted."
        If skippedCompleted > 0 Then msg &= $"  {skippedCompleted} skipped (STATUS = Completed)."
        MessageBox.Show(msg, "Delete", MessageBoxButtons.OK, MessageBoxIcon.Information)

        RefreshPage(resetPage:=True)
    End Sub

    Private Sub Cmdnew_Click(sender As Object, e As EventArgs) Handles cmdnew.Click
        Try
            frmdeliveryfile.Dispose()
            frmdeliveryfile.ShowDialog()
        Catch ex As Exception

        End Try
    End Sub
End Class
