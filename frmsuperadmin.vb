Imports System.Data
Imports System.Data.SqlClient
Imports System.Text
Public Class frmsuperadmin
    '──────────────────────────────────────────────────────────────────────────────
    ' frmsuperadmin – table manager (load, search, delete)
    '──────────────────────────────────────────────────────────────────────────────
    Private ReadOnly AllowedTables As String() = {
        "TBL_ABSTRACT",
        "TBL_ABSTRACTITEMS",
        "TBL_APPROVAL",
        "TBL_EDESC",
        "TBL_IAR",
        "TBL_IARITEMS",
        "TBL_INVENTORY",
        "TBL_LOGIN",
        "TBL_LOGS_IAR",      ' (Fixed name: remove leading TBL_TBL_)
        "TBL_PO",
        "TBL_POITEMS",
        "TBL_RFQ",
        "TBL_RFQITEMS",
        "TBL_RFQSUPPLIER"
    }

    Private Class TableSchema
        Public Property AllColumns As List(Of String) = New List(Of String)
        Public Property SearchableColumns As List(Of String) = New List(Of String)
        Public Property PrimaryKeyColumns As List(Of String) = New List(Of String)
    End Class

    Private _schemaCache As New Dictionary(Of String, TableSchema)(StringComparer.OrdinalIgnoreCase)
    Private _currentTable As String = Nothing
    Private _searchDebounce As New Timer With {.Interval = 300} ' ms debounce

    '──────────────────────────────────────────────────────────────────────────────
    ' Form Load
    '──────────────────────────────────────────────────────────────────────────────
    Private Sub frmsuperadmin_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            ' Populate tables
            combotable.Items.Clear()
            combotable.Items.AddRange(AllowedTables)
            If combotable.Items.Count > 0 Then combotable.SelectedIndex = 0

            ' DataGridView base style (aligned with your design, but auto-generate columns for flexibility)
            With DataGridView1
                .DataSource = Nothing
                .Rows.Clear()
                .Columns.Clear()
                .AutoGenerateColumns = True
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

                .EnableHeadersVisualStyles = False
                .ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(73, 94, 252)
                .ColumnHeadersDefaultCellStyle.ForeColor = Color.White
                .ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(73, 94, 252)
                .ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White
                .ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 11.0!, FontStyle.Bold)
                .ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
                .ColumnHeadersHeight = 52
                .ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing

                .DefaultCellStyle.Font = New Font("Segoe UI", 10.0!)
                .DefaultCellStyle.ForeColor = Color.Black
                .RowsDefaultCellStyle.BackColor = Color.White
                .AlternatingRowsDefaultCellStyle.BackColor = Color.White
                .DefaultCellStyle.SelectionBackColor = Color.FromArgb(210, 222, 255)
                .DefaultCellStyle.SelectionForeColor = Color.Black

                .RowTemplate.Height = 34
                .RowTemplate.DefaultCellStyle.Padding = New Padding(0, 6, 0, 6)
                .ReadOnly = True
            End With

            ' Reduce flicker (double-buffer via reflection)
            Try
                Dim dgvType = GetType(DataGridView)
                Dim pi = dgvType.GetProperty("DoubleBuffered", Reflection.BindingFlags.Instance Or Reflection.BindingFlags.NonPublic)
                If pi IsNot Nothing Then pi.SetValue(DataGridView1, True, Nothing)
            Catch
            End Try

            ' Debounced search
            AddHandler _searchDebounce.Tick, Sub()
                                                 _searchDebounce.Stop()
                                                 LoadCurrentTable(txtsearch.Text.Trim())
                                             End Sub

        Catch ex As Exception
            MessageBox.Show("Load error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    '──────────────────────────────────────────────────────────────────────────────
    ' Events
    '──────────────────────────────────────────────────────────────────────────────
    Private Sub combotable_SelectedIndexChanged(sender As Object, e As EventArgs) Handles combotable.SelectedIndexChanged
        _currentTable = TryCast(combotable.SelectedItem, String)
        LoadCurrentTable(txtsearch.Text.Trim())
    End Sub

    Private Sub txtsearch_TextChanged(sender As Object, e As EventArgs) Handles txtsearch.TextChanged
        _searchDebounce.Stop()
        _searchDebounce.Start()
    End Sub

    Private Sub cmddelete_Click(sender As Object, e As EventArgs) Handles cmddelete.Click
        Try
            If String.IsNullOrWhiteSpace(_currentTable) Then
                MessageBox.Show("Please select a table first.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If

            If DataGridView1.SelectedRows Is Nothing OrElse DataGridView1.SelectedRows.Count = 0 Then
                MessageBox.Show("Select at least one row to delete.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If

            Dim schema = GetTableSchema(_currentTable)
            If schema.PrimaryKeyColumns.Count = 0 Then
                ' Fallback: try a single "ID" column if exists
                If schema.AllColumns.Any(Function(c) c.Equals("ID", StringComparison.OrdinalIgnoreCase)) Then
                    schema.PrimaryKeyColumns = New List(Of String) From {"ID"}
                Else
                    MessageBox.Show($"Table '{_currentTable}' has no primary key; cannot safely delete.", "Cannot Delete", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If
            End If

            Dim ask = MessageBox.Show($"Are you sure you want to delete {DataGridView1.SelectedRows.Count} record(s) from {_currentTable}?",
                                      "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2)
            If ask <> DialogResult.Yes Then Return

            Dim deleted As Integer = 0

            Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
                conn.Open()
                Using tran = conn.BeginTransaction()
                    Try
                        For Each r As DataGridViewRow In DataGridView1.SelectedRows
                            Using cmd As New SqlCommand(BuildDeleteSql(_currentTable, schema.PrimaryKeyColumns), conn, tran)
                                For i = 0 To schema.PrimaryKeyColumns.Count - 1
                                    Dim col = schema.PrimaryKeyColumns(i)
                                    Dim val As Object = If(r.Cells(col) IsNot Nothing, r.Cells(col).Value, DBNull.Value)
                                    cmd.Parameters.AddWithValue($"@pk{i}", If(val IsNot Nothing, val, DBNull.Value))
                                Next
                                deleted += cmd.ExecuteNonQuery()
                            End Using
                        Next
                        tran.Commit()
                    Catch
                        tran.Rollback()
                        Throw
                    End Try
                End Using
            End Using

            LoadCurrentTable(txtsearch.Text.Trim())
            MessageBox.Show($"Deleted {deleted} record(s) from {_currentTable}.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MessageBox.Show("Delete error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    '──────────────────────────────────────────────────────────────────────────────
    ' Core loaders
    '──────────────────────────────────────────────────────────────────────────────
    Private Sub LoadCurrentTable(Optional searchText As String = Nothing)
        If String.IsNullOrWhiteSpace(_currentTable) Then Return
        Try
            Dim schema = GetTableSchema(_currentTable)

            Dim sql As New StringBuilder()
            sql.Append("SELECT * FROM ").Append(QuoteName(_currentTable))

            Dim parms As New List(Of SqlParameter)

            If Not String.IsNullOrWhiteSpace(searchText) AndAlso schema.SearchableColumns.Count > 0 Then
                sql.Append(" WHERE ")
                For i = 0 To schema.SearchableColumns.Count - 1
                    If i > 0 Then sql.Append(" OR ")
                    sql.Append(QuoteName(schema.SearchableColumns(i))).Append(" LIKE @p").Append(i)
                    parms.Add(New SqlParameter("@p" & i, "%" & searchText & "%"))
                Next
            End If

            ' NOTE: Avoid SELECT * for huge tables? If needed, add TOP(N) here.
            ' sql.Insert(7, " TOP (1000) ") ' optional cap

            Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
                Using da As New SqlDataAdapter(sql.ToString(), conn)
                    If parms.Count > 0 Then da.SelectCommand.Parameters.AddRange(parms.ToArray())
                    Dim dt As New DataTable()
                    da.Fill(dt)
                    DataGridView1.DataSource = dt
                End Using
            End Using

            AutoSizeColumns()

        Catch ex As Exception
            MessageBox.Show("Load table error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub AutoSizeColumns()
        If DataGridView1 Is Nothing OrElse DataGridView1.Columns.Count = 0 Then Return
        For Each c As DataGridViewColumn In DataGridView1.Columns
            c.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
        Next
    End Sub

    '──────────────────────────────────────────────────────────────────────────────
    ' Schema helpers (cached)
    '──────────────────────────────────────────────────────────────────────────────
    Private Function GetTableSchema(tableName As String) As TableSchema
        If _schemaCache.ContainsKey(tableName) Then Return _schemaCache(tableName)

        Dim ts As New TableSchema()

        Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
            conn.Open()

            ' 1) All columns
            Using cmd As New SqlCommand("
            SELECT COLUMN_NAME, DATA_TYPE
            FROM INFORMATION_SCHEMA.COLUMNS
            WHERE TABLE_NAME = @t
            ORDER BY ORDINAL_POSITION;", conn)
                cmd.Parameters.Add("@t", SqlDbType.NVarChar, 128).Value = tableName
                Using rdr = cmd.ExecuteReader()
                    While rdr.Read()
                        Dim col = rdr.GetString(0)
                        Dim dtype = rdr.GetString(1).ToLowerInvariant()
                        ts.AllColumns.Add(col)
                        ' Searchable columns: text-like
                        If dtype.Contains("char") OrElse dtype = "text" OrElse dtype = "ntext" OrElse dtype = "varchar" OrElse dtype = "nvarchar" Then
                            ts.SearchableColumns.Add(col)
                        End If
                    End While
                End Using
            End Using

            ' If nothing text-like, allow search on all columns as string cast fallback (kept simple: we won’t auto-cast; just leave empty)
            ' 2) Primary key columns
            Using cmdPk As New SqlCommand("
            SELECT c.COLUMN_NAME
            FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
            JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE c
                 ON c.CONSTRAINT_NAME = tc.CONSTRAINT_NAME
                AND c.TABLE_NAME = tc.TABLE_NAME
            WHERE tc.TABLE_NAME = @t AND tc.CONSTRAINT_TYPE = 'PRIMARY KEY'
            ORDER BY c.ORDINAL_POSITION;", conn)
                cmdPk.Parameters.Add("@t", SqlDbType.NVarChar, 128).Value = tableName
                Using rdr = cmdPk.ExecuteReader()
                    While rdr.Read()
                        ts.PrimaryKeyColumns.Add(rdr.GetString(0))
                    End While
                End Using
            End Using
        End Using

        _schemaCache(tableName) = ts
        Return ts
    End Function

    Private Function QuoteName(identifier As String) As String
        ' SQL Server QUOTENAME behavior with []
        If identifier Is Nothing Then Return ""
        Return "[" & identifier.Replace("]", "]]") & "]"
    End Function

    Private Function BuildDeleteSql(tableName As String, pkCols As List(Of String)) As String
        Dim sb As New StringBuilder()
        sb.Append("DELETE FROM ").Append(QuoteName(tableName)).Append(" WHERE ")
        For i = 0 To pkCols.Count - 1
            If i > 0 Then sb.Append(" AND ")
            sb.Append(QuoteName(pkCols(i))).Append(" = @pk").Append(i)
        Next
        Return sb.ToString()
    End Function

End Class