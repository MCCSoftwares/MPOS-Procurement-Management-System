' frmsupplier.vb
Imports System.Data
Imports System.Data.SqlClient
Imports System.Drawing
Imports System.IO
Imports System.IO.Compression
Imports System.Linq
Imports System.Windows.Forms
Imports System.Windows.Forms.VisualStyles

Public Class frmsupplier

    '──────────────────────────────────────────────────────────────────────────────
    ' Paging & search state
    '──────────────────────────────────────────────────────────────────────────────
    Private _pageSize As Integer = 100
    Private _pageIndex As Integer = 0
    Private _totalRows As Integer = 0
    Private _searchTerm As String = String.Empty
    Private _searchTimer As Timer

    ' Debounce for painted-button hit-tests
    Private _nextHitAllowedAt As Date = Date.MinValue
    Private Const HIT_DEBOUNCE_MS As Integer = 250

    '──────────────────────────────────────────────────────────────────────────────
    ' Column keys (match TBL_SUPPLIERS)
    '──────────────────────────────────────────────────────────────────────────────
    Private Const COL_DB_ID As String = "ID"
    Private Const COL_DB_SUPPLIER As String = "SUPPLIER"
    Private Const COL_DB_ADDRESS As String = "ADDRESS"
    Private Const COL_DB_TIN As String = "TIN_NO"
    Private Const COL_DB_CONTACT As String = "CONTACT_NO"
    Private Const COL_DB_BPERMIT As String = "BPERMIT"
    Private Const COL_DB_PHILGEPS As String = "PHILGEPS"
    Private Const COL_DB_ITR As String = "ITR"
    Private Const COL_DB_UPLOADS As String = "UPLOADS" ' varbinary(MAX)

    ' Grid indices
    Private Enum GridCol
        ID = 0
        Supplier = 1
        Address = 2
        TIN = 3
        Contact = 4
        BPermit = 5
        Philgeps = 6
        ITR = 7
        Link = 8
        HiddenBlob = 9
    End Enum

    Private _pageTable As DataTable

    '──────────────────────────────────────────────────────────────────────────────
    ' Form events
    '──────────────────────────────────────────────────────────────────────────────
    Private Sub frmsupplier_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        SetupGrid()

        For Each b In New Control() {cmdprev, cmdnext, cmddelete, cmdadd, cmdedit}
            If b IsNot Nothing Then b.Cursor = Cursors.Hand
        Next

        _searchTimer = New Timer() With {.Interval = 300}
        AddHandler _searchTimer.Tick, AddressOf SearchTimer_Tick

        LoadPage(0)
    End Sub

    Private Sub cmdprev_Click(sender As Object, e As EventArgs) Handles cmdprev.Click
        If _pageIndex > 0 Then
            _pageIndex -= 1
            LoadPage(_pageIndex)
        End If
    End Sub

    Private Sub cmdnext_Click(sender As Object, e As EventArgs) Handles cmdnext.Click
        Dim maxPage As Integer = Math.Max(CInt(Math.Ceiling(_totalRows / CDbl(_pageSize))) - 1, 0)
        If _pageIndex < maxPage Then
            _pageIndex += 1
            LoadPage(_pageIndex)
        End If
    End Sub

    Private Sub cmddelete_Click(sender As Object, e As EventArgs) Handles cmddelete.Click
        If DataGridView1.SelectedRows.Count = 0 Then
            MessageBox.Show("Select one or more rows to delete.", "Delete", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If
        If MessageBox.Show("Delete the selected supplier record(s)?", "Confirm Delete",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Warning) <> DialogResult.Yes Then Return

        Dim ids As New List(Of Integer)
        For Each r As DataGridViewRow In DataGridView1.SelectedRows
            If r.Cells(CInt(GridCol.ID)).Value IsNot Nothing Then ids.Add(CInt(r.Cells(CInt(GridCol.ID)).Value))
        Next
        If ids.Count = 0 Then Return

        Try
            Using conn As New SqlConnection(frmmain.txtdb.Text)
                conn.Open()
                Using cmd As New SqlCommand("DELETE FROM dbo.TBL_SUPPLIERS WHERE ID IN (" &
                                            String.Join(",", ids.Select(Function(i) "@p" & i)) & ")", conn)
                    For Each i In ids
                        cmd.Parameters.AddWithValue("@p" & i, i)
                    Next
                    cmd.ExecuteNonQuery()
                End Using
            End Using
            RefreshCounts()
            Dim maxPage As Integer = Math.Max(CInt(Math.Ceiling(_totalRows / CDbl(_pageSize))) - 1, 0)
            If _pageIndex > maxPage Then _pageIndex = maxPage
            LoadPage(_pageIndex)
        Catch ex As Exception
            MessageBox.Show("Delete failed: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub cmdadd_Click(sender As Object, e As EventArgs) Handles cmdadd.Click
        Using f As New frmsupplierfile()
            f.ModeEdit = False
            If f.ShowDialog(Me) = DialogResult.OK Then
                _pageIndex = 0
                LoadPage(_pageIndex)
            End If
        End Using
    End Sub

    Private Sub cmdedit_Click(sender As Object, e As EventArgs) Handles cmdedit.Click
        If DataGridView1.SelectedRows.Count = 0 Then
            MessageBox.Show("Select a supplier to edit.", "Edit", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If
        Dim r As DataGridViewRow = DataGridView1.SelectedRows(0)
        Dim id As Integer = CInt(r.Cells(CInt(GridCol.ID)).Value)

        Using f As New frmsupplierfile()
            f.ModeEdit = True
            f.SupplierID = id
            f.txtsupplier.Text = SafeStr(r.Cells(CInt(GridCol.Supplier)).Value)
            f.txtaddress.Text = SafeStr(r.Cells(CInt(GridCol.Address)).Value)
            f.txttin.Text = SafeStr(r.Cells(CInt(GridCol.TIN)).Value)
            f.txtcontact.Text = SafeStr(r.Cells(CInt(GridCol.Contact)).Value)
            If f.ShowDialog(Me) = DialogResult.OK Then
                LoadPage(_pageIndex)
            End If
        End Using
    End Sub

    '──────────────────────────────────────────────────────────────────────────────
    ' Search (txtsearch LIKE)
    '──────────────────────────────────────────────────────────────────────────────
    Private Sub txtsearch_TextChanged(sender As Object, e As EventArgs) Handles txtsearch.TextChanged
        _searchTimer.Stop()
        _searchTimer.Start()
    End Sub

    Private Sub SearchTimer_Tick(sender As Object, e As EventArgs)
        _searchTimer.Stop()
        _searchTerm = If(txtsearch IsNot Nothing, txtsearch.Text.Trim(), String.Empty)
        _pageIndex = 0
        LoadPage(_pageIndex)
    End Sub

    '──────────────────────────────────────────────────────────────────────────────
    ' Grid setup & styling  (alignment + header text)
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
            .MultiSelect = True
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
            .ReadOnly = False
            .EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2

            ' Flat style
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
            .ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter ' CENTER headers
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

            .DoubleBuffered(True)
        End With

        ' Columns (lock manual typing on text columns)
        Dim colID = New DataGridViewTextBoxColumn() With {
            .Name = "ID", .HeaderText = "ID", .DataPropertyName = COL_DB_ID, .Visible = False, .ReadOnly = True
        }

        Dim colSupplier = New DataGridViewTextBoxColumn() With {
            .Name = "Supplier", .HeaderText = "Supplier", .DataPropertyName = COL_DB_SUPPLIER,
            .Width = 260, .ReadOnly = True
        }
        colSupplier.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft

        Dim colAddress = New DataGridViewTextBoxColumn() With {
            .Name = "Address", .HeaderText = "Address", .DataPropertyName = COL_DB_ADDRESS,
            .Width = 320, .ReadOnly = True
        }
        colAddress.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft

        Dim colTIN = New DataGridViewTextBoxColumn() With {
            .Name = "TIN", .HeaderText = "TIN No.", .DataPropertyName = COL_DB_TIN,
            .Width = 140, .ReadOnly = True
        }
        colTIN.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

        Dim colContact = New DataGridViewTextBoxColumn() With {
            .Name = "Contact", .HeaderText = "Contact No.", .DataPropertyName = COL_DB_CONTACT,
            .Width = 140, .ReadOnly = True
        }
        colContact.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

        Dim colBPermit = New DataGridViewCheckBoxColumn() With {
            .Name = "BPERMIT", .HeaderText = "Business Permit", .Width = 150,
            .DataPropertyName = "_BPERMIT_BOOL", .ReadOnly = False
        }
        colBPermit.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

        Dim colPhil = New DataGridViewCheckBoxColumn() With {
            .Name = "PHILGEPS", .HeaderText = "PhilGEPS Registration No./Certificate", .Width = 230,
            .DataPropertyName = "_PHILGEPS_BOOL", .ReadOnly = False
        }
        colPhil.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

        Dim colITR = New DataGridViewCheckBoxColumn() With {
            .Name = "ITR", .HeaderText = "Updated Tax Clearance/ITR", .Width = 200,
            .DataPropertyName = "_ITR_BOOL", .ReadOnly = False
        }
        colITR.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

        Dim colLink As New DataGridViewTextBoxColumn() With {
            .Name = "Link", .HeaderText = "Uploaded File", ' renamed header
            .ReadOnly = True, .Width = 160, .SortMode = DataGridViewColumnSortMode.NotSortable
        }

        Dim colBlob = New DataGridViewTextBoxColumn() With {
            .Name = "BLOB", .HeaderText = "BLOB", .DataPropertyName = COL_DB_UPLOADS, .Visible = False, .ReadOnly = True
        }

        DataGridView1.Columns.AddRange(New DataGridViewColumn() {
            colID, colSupplier, colAddress, colTIN, colContact, colBPermit, colPhil, colITR, colLink, colBlob
        })

        ' Event wiring
        AddHandler DataGridView1.CellPainting, AddressOf DataGridView1_CellPainting
        ' IMPORTANT: trigger actions only on MouseUp (avoid double-fire)
        AddHandler DataGridView1.CellMouseUp, AddressOf DataGridView1_CellMouseUp
        AddHandler DataGridView1.CellMouseMove, AddressOf DataGridView1_CellMouseMove
        AddHandler DataGridView1.MouseLeave, AddressOf DataGridView1_MouseLeave
        AddHandler DataGridView1.CellFormatting, AddressOf DataGridView1_CellFormatting
        AddHandler DataGridView1.CellValueChanged, AddressOf DataGridView1_CellValueChanged
        AddHandler DataGridView1.CurrentCellDirtyStateChanged, AddressOf DataGridView1_CurrentCellDirtyStateChanged
    End Sub

    ' TRUE/FALSE -> checkbox
    Private Sub DataGridView1_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs)
        If _pageTable Is Nothing OrElse e.RowIndex < 0 OrElse e.RowIndex >= _pageTable.Rows.Count Then Return
        Select Case DataGridView1.Columns(e.ColumnIndex).Name
            Case "BPERMIT"
                e.Value = ToBool(_pageTable.Rows(e.RowIndex)(COL_DB_BPERMIT)) : e.FormattingApplied = True
            Case "PHILGEPS"
                e.Value = ToBool(_pageTable.Rows(e.RowIndex)(COL_DB_PHILGEPS)) : e.FormattingApplied = True
            Case "ITR"
                e.Value = ToBool(_pageTable.Rows(e.RowIndex)(COL_DB_ITR)) : e.FormattingApplied = True
        End Select
    End Sub

    Private Sub DataGridView1_CurrentCellDirtyStateChanged(sender As Object, e As EventArgs)
        If TypeOf DataGridView1.CurrentCell Is DataGridViewCheckBoxCell AndAlso DataGridView1.IsCurrentCellDirty Then
            DataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit)
        End If
    End Sub

    Private Sub DataGridView1_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs)
        If e.RowIndex < 0 Then Return
        Dim colName = DataGridView1.Columns(e.ColumnIndex).Name
        If colName <> "BPERMIT" AndAlso colName <> "PHILGEPS" AndAlso colName <> "ITR" Then Return

        Dim id As Integer = CInt(DataGridView1.Rows(e.RowIndex).Cells(CInt(GridCol.ID)).Value)
        Dim newBool As Boolean = CBool(DataGridView1.Rows(e.RowIndex).Cells(e.ColumnIndex).Value)
        Dim asText As String = If(newBool, "TRUE", "FALSE")
        Dim dbCol As String = If(colName = "BPERMIT", COL_DB_BPERMIT, If(colName = "PHILGEPS", COL_DB_PHILGEPS, COL_DB_ITR))

        Try
            Using conn As New SqlConnection(frmmain.txtdb.Text)
                conn.Open()
                Using cmd As New SqlCommand($"UPDATE dbo.TBL_SUPPLIERS SET {dbCol}=@v WHERE ID=@id", conn)
                    cmd.Parameters.AddWithValue("@v", asText)
                    cmd.Parameters.AddWithValue("@id", id)
                    cmd.ExecuteNonQuery()
                End Using
            End Using
            Dim dr = _pageTable.Select("ID=" & id).FirstOrDefault()
            If dr IsNot Nothing Then dr(dbCol) = asText
        Catch ex As Exception
            MessageBox.Show("Save failed: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    '──────────────────────────────────────────────────────────────────────────────
    ' Paging loader (LIKE filter)
    '──────────────────────────────────────────────────────────────────────────────
    Private Sub RefreshCounts()
        Using conn As New SqlConnection(frmmain.txtdb.Text)
            conn.Open()
            Dim sqlCnt As String =
                "SELECT COUNT(*) FROM dbo.TBL_SUPPLIERS " &
                "WHERE (@q = '' OR SUPPLIER LIKE @pat OR ADDRESS LIKE @pat OR TIN_NO LIKE @pat OR CONTACT_NO LIKE @pat);"
            Using cmd As New SqlCommand(sqlCnt, conn)
                cmd.Parameters.AddWithValue("@q", _searchTerm)
                cmd.Parameters.AddWithValue("@pat", "%" & _searchTerm & "%")
                _totalRows = CInt(cmd.ExecuteScalar())
            End Using
        End Using
    End Sub

    Private Sub LoadPage(pageIndex As Integer)
        Try
            RefreshCounts()
            Dim skip As Integer = pageIndex * _pageSize

            Using conn As New SqlConnection(frmmain.txtdb.Text)
                conn.Open()
                Dim sql As String =
                    "WITH P AS (" &
                    "  SELECT ID, SUPPLIER, ADDRESS, TIN_NO, CONTACT_NO, BPERMIT, PHILGEPS, ITR, UPLOADS " &
                    "  FROM dbo.TBL_SUPPLIERS " &
                    "  WHERE (@q = '' OR SUPPLIER LIKE @pat OR ADDRESS LIKE @pat OR TIN_NO LIKE @pat OR CONTACT_NO LIKE @pat)" &
                    ") " &
                    "SELECT * FROM P ORDER BY SUPPLIER " &
                    "OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;"

                Using da As New SqlDataAdapter(sql, conn)
                    da.SelectCommand.Parameters.AddWithValue("@q", _searchTerm)
                    da.SelectCommand.Parameters.AddWithValue("@pat", "%" & _searchTerm & "%")
                    da.SelectCommand.Parameters.AddWithValue("@skip", skip)
                    da.SelectCommand.Parameters.AddWithValue("@take", _pageSize)
                    Dim dt As New DataTable()
                    da.Fill(dt)

                    If Not dt.Columns.Contains("_BPERMIT_BOOL") Then dt.Columns.Add("_BPERMIT_BOOL", GetType(Boolean))
                    If Not dt.Columns.Contains("_PHILGEPS_BOOL") Then dt.Columns.Add("_PHILGEPS_BOOL", GetType(Boolean))
                    If Not dt.Columns.Contains("_ITR_BOOL") Then dt.Columns.Add("_ITR_BOOL", GetType(Boolean))

                    For Each r As DataRow In dt.Rows
                        r("_BPERMIT_BOOL") = ToBool(r(COL_DB_BPERMIT))
                        r("_PHILGEPS_BOOL") = ToBool(r(COL_DB_PHILGEPS))
                        r("_ITR_BOOL") = ToBool(r(COL_DB_ITR))
                    Next

                    _pageTable = dt
                    DataGridView1.DataSource = _pageTable
                End Using
            End Using

            Dim totalPages As Integer = Math.Max(CInt(Math.Ceiling(_totalRows / CDbl(_pageSize))), 1)
            lblPageInfo.Text = $"Page {(_pageIndex + 1)} of {totalPages}   •   Rows {_totalRows}"
        Catch ex As Exception
            MessageBox.Show("Load failed: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    '──────────────────────────────────────────────────────────────────────────────
    ' Painted buttons (Uploaded File column)
    '──────────────────────────────────────────────────────────────────────────────
    Private Function HasBlob(rowIndex As Integer) As Boolean
        If _pageTable Is Nothing OrElse rowIndex < 0 OrElse rowIndex >= _pageTable.Rows.Count Then Return False
        Dim v = _pageTable.Rows(rowIndex)(COL_DB_UPLOADS)
        If v Is DBNull.Value OrElse v Is Nothing Then Return False
        Dim b As Byte() = TryCast(v, Byte())
        Return (b IsNot Nothing AndAlso b.Length > 0)
    End Function

    Private Function UploadButtonRect_Rel(cellSize As Size) As Rectangle
        Dim w As Integer = Math.Min(72, Math.Max(48, CInt(cellSize.Width * 0.45)))
        Dim h As Integer = 26
        Dim x As Integer = (cellSize.Width - w) \ 2
        Dim y As Integer = (cellSize.Height - h) \ 2
        Return New Rectangle(x, y, w, h)
    End Function

    Private Sub ViewDeleteRects_Rel(cellSize As Size, ByRef rView As Rectangle, ByRef rDel As Rectangle)
        Dim w As Integer = 66, h As Integer = 26, spacing As Integer = 8
        Dim totalW As Integer = w * 2 + spacing
        Dim x0 As Integer = (cellSize.Width - totalW) \ 2
        Dim y As Integer = (cellSize.Height - h) \ 2
        rView = New Rectangle(x0, y, w, h)
        rDel = New Rectangle(x0 + w + spacing, y, w, h)
    End Sub

    Private Sub DataGridView1_CellPainting(sender As Object, e As DataGridViewCellPaintingEventArgs)
        If e.RowIndex < 0 Then Return
        If DataGridView1.Columns(e.ColumnIndex).Name <> "Link" Then Return

        e.Handled = True
        e.PaintBackground(e.CellBounds, True)

        If Not HasBlob(e.RowIndex) Then
            Dim rRel As Rectangle = UploadButtonRect_Rel(e.CellBounds.Size)
            Dim rAbs As New Rectangle(e.CellBounds.X + rRel.X, e.CellBounds.Y + rRel.Y, rRel.Width, rRel.Height)
            ButtonRenderer.DrawButton(e.Graphics, rAbs, VisualStyles.PushButtonState.Default)
            TextRenderer.DrawText(e.Graphics, "Upload", DataGridView1.Font, rAbs, Color.Black,
                                  TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)
        Else
            Dim rvRel As Rectangle, rdRel As Rectangle
            ViewDeleteRects_Rel(e.CellBounds.Size, rvRel, rdRel)
            Dim rvAbs As New Rectangle(e.CellBounds.X + rvRel.X, e.CellBounds.Y + rvRel.Y, rvRel.Width, rvRel.Height)
            Dim rdAbs As New Rectangle(e.CellBounds.X + rdRel.X, e.CellBounds.Y + rdRel.Y, rdRel.Width, rdRel.Height)

            ButtonRenderer.DrawButton(e.Graphics, rvAbs, VisualStyles.PushButtonState.Default)
            TextRenderer.DrawText(e.Graphics, "View", DataGridView1.Font, rvAbs, Color.Black,
                                  TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)
            ButtonRenderer.DrawButton(e.Graphics, rdAbs, VisualStyles.PushButtonState.Default)
            TextRenderer.DrawText(e.Graphics, "Delete", DataGridView1.Font, rdAbs, Color.Black,
                                  TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)
        End If
    End Sub

    ' Trigger only on MouseUp (debounced)
    Private Sub DataGridView1_CellMouseUp(sender As Object, e As DataGridViewCellMouseEventArgs)
        HandleButtonHitTest(e)
    End Sub

    Private Sub HandleButtonHitTest(e As DataGridViewCellMouseEventArgs)
        If e.RowIndex < 0 Then Return
        If DataGridView1.Columns(e.ColumnIndex).Name <> "Link" Then Return

        ' Debounce
        If Date.Now < _nextHitAllowedAt Then Return
        _nextHitAllowedAt = Date.Now.AddMilliseconds(HIT_DEBOUNCE_MS)

        Dim cellSize As Size = DataGridView1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, False).Size
        Dim pRel As Point = e.Location

        If Not HasBlob(e.RowIndex) Then
            Dim rRel As Rectangle = UploadButtonRect_Rel(cellSize)
            If rRel.Contains(pRel) Then
                HandleUpload(e.RowIndex)
            End If
        Else
            Dim rvRel As Rectangle, rdRel As Rectangle
            ViewDeleteRects_Rel(cellSize, rvRel, rdRel)
            If rvRel.Contains(pRel) Then
                HandleView(e.RowIndex)
            ElseIf rdRel.Contains(pRel) Then
                HandleDeleteBlob(e.RowIndex)
            End If
        End If
    End Sub

    Private Sub DataGridView1_CellMouseMove(sender As Object, e As DataGridViewCellMouseEventArgs)
        If e.RowIndex >= 0 AndAlso e.ColumnIndex >= 0 AndAlso DataGridView1.Columns(e.ColumnIndex).Name = "Link" Then
            Dim cellSize As Size = DataGridView1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, False).Size
            Dim pRel As Point = e.Location
            If Not HasBlob(e.RowIndex) Then
                DataGridView1.Cursor = If(UploadButtonRect_Rel(cellSize).Contains(pRel), Cursors.Hand, Cursors.Default)
            Else
                Dim rvRel As Rectangle, rdRel As Rectangle
                ViewDeleteRects_Rel(cellSize, rvRel, rdRel)
                Dim onBtn As Boolean = rvRel.Contains(pRel) OrElse rdRel.Contains(pRel)
                DataGridView1.Cursor = If(onBtn, Cursors.Hand, Cursors.Default)
            End If
        Else
            DataGridView1.Cursor = Cursors.Default
        End If
    End Sub

    Private Sub DataGridView1_MouseLeave(sender As Object, e As EventArgs)
        DataGridView1.Cursor = Cursors.Default
    End Sub

    ' Upload / View / Delete
    Private Sub HandleUpload(rowIndex As Integer)
        Dim id As Integer = CInt(DataGridView1.Rows(rowIndex).Cells(CInt(GridCol.ID)).Value)
        Using ofd As New OpenFileDialog()
            ofd.Title = "Select PDF to upload"
            ofd.Filter = "PDF files (*.pdf)|*.pdf"
            If ofd.ShowDialog() <> DialogResult.OK Then Return

            Dim original As Byte() = File.ReadAllBytes(ofd.FileName)
            Dim gz As Byte() = GzipBytes(original)
            Dim toStore As Byte() = If(gz.Length < original.Length, gz, original)

            Try
                Using conn As New SqlConnection(frmmain.txtdb.Text)
                    conn.Open()
                    Using cmd As New SqlCommand("UPDATE dbo.TBL_SUPPLIERS SET UPLOADS=@b WHERE ID=@id", conn)
                        cmd.Parameters.Add("@b", SqlDbType.VarBinary, toStore.Length).Value = toStore
                        cmd.Parameters.AddWithValue("@id", id)
                        cmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim dr = _pageTable.Select("ID=" & id).FirstOrDefault()
                If dr IsNot Nothing Then dr(COL_DB_UPLOADS) = toStore
                DataGridView1.InvalidateCell(CInt(GridCol.Link), rowIndex)
            Catch ex As Exception
                MessageBox.Show("Upload failed: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Using
    End Sub

    Private Sub HandleView(rowIndex As Integer)
        Dim id As Integer = CInt(DataGridView1.Rows(rowIndex).Cells(CInt(GridCol.ID)).Value)
        Dim data As Byte() = TryCast(_pageTable.Select("ID=" & id).First()(COL_DB_UPLOADS), Byte())
        If data Is Nothing OrElse data.Length = 0 Then Return
        Try
            Dim bytes As Byte() = If(IsGzip(data), GunzipBytes(data), data)
            Dim tempPath As String = Path.Combine(Path.GetTempPath(), $"SUPP_{id}_{DateTime.Now.Ticks}.pdf")
            File.WriteAllBytes(tempPath, bytes)
            Process.Start(New ProcessStartInfo(tempPath) With {.UseShellExecute = True})
        Catch ex As Exception
            MessageBox.Show("Unable to open PDF: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub HandleDeleteBlob(rowIndex As Integer)
        Dim id As Integer = CInt(DataGridView1.Rows(rowIndex).Cells(CInt(GridCol.ID)).Value)
        If MessageBox.Show("Remove the stored PDF for this supplier?", "Delete File",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then Return
        Try
            Using conn As New SqlConnection(frmmain.txtdb.Text)
                conn.Open()
                Using cmd As New SqlCommand("UPDATE dbo.TBL_SUPPLIERS SET UPLOADS=NULL WHERE ID=@id", conn)
                    cmd.Parameters.AddWithValue("@id", id)
                    cmd.ExecuteNonQuery()
                End Using
            End Using
            Dim dr = _pageTable.Select("ID=" & id).FirstOrDefault()
            If dr IsNot Nothing Then dr(COL_DB_UPLOADS) = DBNull.Value
            DataGridView1.InvalidateCell(CInt(GridCol.Link), rowIndex)
        Catch ex As Exception
            MessageBox.Show("Delete failed: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' Helpers
    Private Function ToBool(o As Object) As Boolean
        If o Is Nothing OrElse o Is DBNull.Value Then Return False
        Dim s As String = o.ToString().Trim().ToUpperInvariant()
        Return (s = "TRUE" OrElse s = "1" OrElse s = "YES" OrElse s = "Y")
    End Function

    Private Shared Function SafeStr(o As Object) As String
        If o Is Nothing OrElse o Is DBNull.Value Then Return String.Empty
        Return o.ToString()
    End Function

    Private Function GzipBytes(input As Byte()) As Byte()
        Using msOut As New MemoryStream()
            Using gz As New GZipStream(msOut, CompressionMode.Compress, True)
                gz.Write(input, 0, input.Length)
            End Using
            Return msOut.ToArray()
        End Using
    End Function

    Private Function GunzipBytes(input As Byte()) As Byte()
        Using msIn As New MemoryStream(input)
            Using gz As New GZipStream(msIn, CompressionMode.Decompress)
                Using msOut As New MemoryStream()
                    gz.CopyTo(msOut)
                    Return msOut.ToArray()
                End Using
            End Using
        End Using
    End Function

    Private Function IsGzip(buf As Byte()) As Boolean
        Return buf IsNot Nothing AndAlso buf.Length > 2 AndAlso buf(0) = &H1F AndAlso buf(1) = &H8B
    End Function

End Class

'==================== DataGridView DoubleBuffer helper ====================
Module DataGridViewExtensions
    <System.Runtime.CompilerServices.Extension()>
    Public Sub DoubleBuffered(dgv As DataGridView, enable As Boolean)
        Dim dgvType As Type = dgv.GetType()
        Dim pi = dgvType.GetProperty("DoubleBuffered", Reflection.BindingFlags.Instance Or Reflection.BindingFlags.NonPublic)
        If pi IsNot Nothing Then
            pi.SetValue(dgv, enable, Nothing)
        End If
    End Sub
End Module
