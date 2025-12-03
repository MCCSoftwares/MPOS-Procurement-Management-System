Imports System.Data.SqlClient
Imports System.Text.RegularExpressions
Imports Microsoft.Reporting.WinForms
Imports System.Globalization
Imports System.IO

Public Class frmissuancePPEFile
    Private _isManageMode As Boolean = False
    Private _managedParNo As String = Nothing

    Private Async Sub frmissuancePPEFile_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            ' Make sure columns exist before anything tries to add rows
            EnsureGridReady()

            ' Only set the Save/Update label automatically when NOT in Manage mode
            If Not _isManageMode Then
                cmdsave.Text = If(String.IsNullOrWhiteSpace(lblid.Text), "Save", "Update")
            End If

            ' Grid behavior (read-only view, selection)
            With DataGridView1
                .ReadOnly = True
                .AllowUserToAddRows = False
                .AllowUserToDeleteRows = False
                .MultiSelect = True
            End With

            ' In manage mode we may need free-text in combos (to show values not in list).
            If Not _isManageMode Then
                comborby.DropDownStyle = ComboBoxStyle.DropDownList
                comboiby.DropDownStyle = ComboBoxStyle.DropDownList
            End If

            ' Load employee lists if not loaded yet
            If comborby.DataSource Is Nothing OrElse comboiby.DataSource Is Nothing Then
                Await LoadEmployeeCombosAsync()
            End If

            ' If this form was opened in Manage mode *before* Load fired,
            ' make sure the UI reflects that state.
            If _isManageMode AndAlso Not String.IsNullOrWhiteSpace(_managedParNo) Then
                ' If rows weren't added yet (depending on open timing), reload as fallback
                If DataGridView1.Rows.Count = 0 Then
                    Await ReloadPPEItemsAsync(_managedParNo)
                End If
                cmdprint.Enabled = True
                cmdsave.Text = "Update"
                txtparno.Enabled = False : txtparno.ReadOnly = True
                dtpardate.Enabled = False
            End If

        Catch ex As Exception
            MessageBox.Show("Load error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub



    Private Sub SetupGrid()
        With DataGridView1
            ' base
            .DataSource = Nothing
            .Rows.Clear()
            .Columns.Clear()
            .AutoGenerateColumns = False
            .RowHeadersVisible = False
            .AllowUserToAddRows = False
            .AllowUserToResizeRows = False
            .MultiSelect = True
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect

            ' ---- key fixes ----
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            .AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells   ' grow rows for wrapped cells
            ' (optional) a bit of vertical padding so it breathes
            .DefaultCellStyle.Padding = New Padding(0, 6, 0, 6)

            ' flat look
            .BorderStyle = BorderStyle.None
            .CellBorderStyle = DataGridViewCellBorderStyle.None
            .ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
            .AdvancedCellBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None

            ' columns
            Dim colID As New DataGridViewTextBoxColumn With {.Name = "colID", .HeaderText = "ID", .Visible = False}

            Dim colQty As New DataGridViewTextBoxColumn With {
            .Name = "colQty", .HeaderText = "Quantity",
            .DefaultCellStyle = New DataGridViewCellStyle With {.Alignment = DataGridViewContentAlignment.MiddleCenter, .Format = "#,##0.##"},
            .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, .FillWeight = 55
        }

            Dim colUnit As New DataGridViewTextBoxColumn With {
            .Name = "colUnit", .HeaderText = "Unit",
            .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, .FillWeight = 65
        }

            Dim colDesc As New DataGridViewTextBoxColumn With {
            .Name = "colDesc", .HeaderText = "Description",
            .DefaultCellStyle = New DataGridViewCellStyle With {.WrapMode = DataGridViewTriState.True},
            .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
            .FillWeight = 300                ' give it the most space
        }

            Dim colPNo As New DataGridViewTextBoxColumn With {
            .Name = "colPNo", .HeaderText = "Property Number",
            .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, .FillWeight = 50
        }

            Dim colDAcq As New DataGridViewTextBoxColumn With {
            .Name = "colDAcq", .HeaderText = "Date Acquired",
            .DefaultCellStyle = New DataGridViewCellStyle With {.Format = "MM/dd/yyyy", .Alignment = DataGridViewContentAlignment.MiddleCenter},
            .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, .FillWeight = 80
        }

            Dim colAmt As New DataGridViewTextBoxColumn With {
            .Name = "colAmt", .HeaderText = "Amount",
            .DefaultCellStyle = New DataGridViewCellStyle With {.Alignment = DataGridViewContentAlignment.MiddleCenter, .Format = "##,###.##"},
            .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, .FillWeight = 65
        }

            .Columns.AddRange({colID, colQty, colUnit, colDesc, colPNo, colDAcq, colAmt})

            ' reinforce (in case anything overrides)
            .Columns("colDesc").DefaultCellStyle.WrapMode = DataGridViewTriState.True
            .Columns("colAmt").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .Columns("colQty").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

            ' header + rows styling (unchanged)
            Dim hdrColor As Color = Color.FromArgb(73, 94, 252)
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

            ' IMPORTANT: don’t force a fixed row height when using autosize
            .RowTemplate.Height = 0
        End With

        ' After you add rows (or set DataSource), you can force a re-measure:
        DataGridView1.AutoResizeRows(DataGridViewAutoSizeRowsMode.DisplayedCells)
    End Sub





    Private Sub cmdadd_Click(sender As Object, e As EventArgs) Handles cmdadd.Click
        ' Open add-item dialog; pass current form as parent so it can push a row back here
        Using dlg As New frmissuancePPEAdd(Me)
            dlg.StartPosition = FormStartPosition.CenterParent
            dlg.ShowDialog(Me)
        End Using
    End Sub

    ' cmdsave/cmdprint/cmdclose will be implemented in the next step of your flow
    ' (cmdsave: if "Save" insert header+items; if "Update" update header+items)
    ' For now we only manage buffered grid items per your instruction.

    Private Class EmpItem
        Public Property Display As String      ' Name | Position
        Public Property Value As String        ' original CNAMES (Name{newline}Position)
        Public Overrides Function ToString() As String
            Return Display
        End Function
    End Class

    Private Async Function LoadEmployeeCombosAsync() As Task
        Dim list As New List(Of EmpItem)

        ' Optional placeholder at index 0
        list.Add(New EmpItem With {.Display = "", .Value = ""})

        Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
            Using cmd As New SqlCommand("
            SELECT DISTINCT CNAMES
            FROM TBL_APPROVER WITH (NOLOCK)
            WHERE TYPE = @t AND CNAMES IS NOT NULL AND LTRIM(RTRIM(CNAMES)) <> ''
            ORDER BY CNAMES;", conn)
                cmd.Parameters.AddWithValue("@t", "Employee")
                cmd.CommandTimeout = 15

                Await conn.OpenAsync()
                Using rdr = Await cmd.ExecuteReaderAsync()
                    While Await rdr.ReadAsync()
                        Dim raw As String = rdr("CNAMES").ToString()
                        Dim display As String = FormatCNames(raw)   ' Name | Position
                        If Not String.IsNullOrEmpty(display) Then
                            list.Add(New EmpItem With {.Display = display, .Value = raw})
                        End If
                    End While
                End Using
            End Using
        End Using

        ' Bind
        comborby.DataSource = New List(Of EmpItem)(list)
        comborby.DisplayMember = "Display"
        comborby.ValueMember = "Value"
        comborby.SelectedIndex = 0  ' ← forces to placeholder

        comboiby.DataSource = New List(Of EmpItem)(list)
        comboiby.DisplayMember = "Display"
        comboiby.ValueMember = "Value"
        comboiby.SelectedIndex = 0
    End Function


    ' Converts "Name{newline}Position" or "Name\nPosition" (any newline variant) → "Name | Position"
    Private Function FormatCNames(raw As String) As String
        If String.IsNullOrWhiteSpace(raw) Then Return ""
        ' Normalize all CR/LF variants to a single \n then split
        Dim norm = Regex.Replace(raw, "\r\n|\r|\n", vbLf).Trim()
        Dim parts = norm.Split({vbLf}, StringSplitOptions.RemoveEmptyEntries).
                        Select(Function(p) p.Trim()).
                        ToArray()

        If parts.Length = 0 Then Return ""
        Dim name As String = parts(0)
        Dim pos As String = If(parts.Length > 1, parts(1), "")
        If pos = "" Then
            Return name                    ' fallback: no position stored
        Else
            Return $"{name} | {pos}"
        End If
    End Function

    ' Handy accessors
    Private ReadOnly Property ConnStr As String
        Get
            Return My.Forms.frmmain.txtdb.Text
        End Get
    End Property

    ' Build PAR No. = YYYY-1-MM-SSSS (series resets each year)
    Private Async Function GenerateParNoAsync(d As Date) As Task(Of String)
        Dim yearStr = d.ToString("yyyy")
        Dim monthStr = d.ToString("MM")
        Dim nextSeries As Integer = 1

        Using conn As New SqlClient.SqlConnection(ConnStr)
            Await conn.OpenAsync()
            Using cmd As New SqlClient.SqlCommand("
            ;WITH S AS (
              SELECT MAX(CAST(RIGHT(PAR_NO, 4) AS int)) AS MaxSeries
              FROM TBL_PPE WITH (NOLOCK)
              WHERE LEFT(PAR_NO,4) = @yyyy
            )
            SELECT ISNULL(MaxSeries, 0) FROM S;", conn)
                cmd.Parameters.AddWithValue("@yyyy", yearStr)
                Dim obj = Await cmd.ExecuteScalarAsync()
                If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                    nextSeries = CInt(obj) + 1
                End If
            End Using
        End Using

        Dim seriesStr = nextSeries.ToString("0000")
        Return $"{yearStr}-1-{monthStr}-{seriesStr}"
    End Function

    ' Ensure PAR No. is not already used
    Private Async Function EnsureParNoUniqueAsync(parNo As String) As Task
        Using conn As New SqlClient.SqlConnection(ConnStr)
            Await conn.OpenAsync()
            Using cmd As New SqlClient.SqlCommand("
            SELECT COUNT(1) FROM TBL_PPE WITH (NOLOCK) WHERE PAR_NO = @p;", conn)
                cmd.Parameters.AddWithValue("@p", parNo.Trim())
                Dim cnt = CInt(Await cmd.ExecuteScalarAsync())
                If cnt > 0 Then
                    Throw New InvalidOperationException($"PAR No. '{parNo}' already exists.")
                End If
            End Using
        End Using
    End Function

    Private Async Function ReloadPPEItemsAsync(parNo As String) As Task
        EnsureGridReady()
        Dim dt As New DataTable()

        Using conn As New SqlClient.SqlConnection(My.Forms.frmmain.txtdb.Text)
            Using cmd As New SqlClient.SqlCommand("
            SELECT ID, qty, unit, description, property_number, date_acquired, unit_cost
            FROM TBL_PPE WITH (NOLOCK)
            WHERE par_no = @p
            ORDER BY ID;", conn)
                cmd.Parameters.AddWithValue("@p", parNo)
                Await conn.OpenAsync()
                Using rdr = Await cmd.ExecuteReaderAsync()
                    dt.Load(rdr)
                End Using
            End Using
        End Using

        DataGridView1.SuspendLayout()
        Try
            DataGridView1.Rows.Clear()
            For Each r As DataRow In dt.Rows
                Dim idx = DataGridView1.Rows.Add()
                With DataGridView1.Rows(idx)
                    .Cells("colID").Value = CInt(r("ID"))
                    .Cells("colQty").Value = CDec(r("qty"))
                    .Cells("colUnit").Value = r("unit").ToString()
                    .Cells("colDesc").Value = r("description").ToString()
                    .Cells("colPNo").Value = r("property_number").ToString()
                    .Cells("colDAcq").Value = CDate(r("date_acquired"))
                    .Cells("colAmt").Value = CDec(r("unit_cost"))
                End With
            Next
        Finally
            DataGridView1.ResumeLayout()
        End Try
    End Function



    Private Async Sub cmdsave_Click(sender As Object, e As EventArgs) Handles cmdsave.Click

        ' Build/validate PAR No.
        Dim parDate As Date = dtpardate.Value.Date
        Dim parNo As String = txtparno.Text.Trim()

        Try
            If cmdsave.Text = "Save" Then
                ' 1) Generate if empty; else validate uniqueness
                If String.IsNullOrEmpty(parNo) Then
                    parNo = Await GenerateParNoAsync(parDate)
                    ' Double-check uniqueness before use (race-safety)
                    Await EnsureParNoUniqueAsync(parNo)
                    txtparno.Text = parNo
                Else
                    Await EnsureParNoUniqueAsync(parNo)
                End If

                ' 2) Insert all buffered rows + deduct inventory in one transaction
                Await InsertNewPPEAsync(parNo, parDate)

                ' 3) Reload and flip state
                Await ReloadPPEItemsAsync(parNo)
                cmdprint.Enabled = True
                cmdadd.Enabled = True
                cmddelete.Enabled = True
                cmdsave.Text = "Update"
                MessageBox.Show("Saved successfully.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information)

            ElseIf cmdsave.Text = "Update" Then
                ' Update existing rows (and insert any new ones) + adjust inventory deltas
                Await UpdatePPEAsync(parNo, parDate)
                Await ReloadPPEItemsAsync(parNo)
                cmdprint.Enabled = True
                cmdadd.Enabled = True
                cmddelete.Enabled = True
                MessageBox.Show("Updated successfully.", "Updated", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If

        Catch ex As Exception
            MessageBox.Show("Save/Update error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Async Function InsertNewPPEAsync(parNo As String, parDate As Date) As Task
        Dim parToName As String = "", parToPos As String = ""
        SplitNamePosition(comborby.Text, parToName, parToPos)

        Dim preparedByName As String = "", tmpPos As String = ""
        SplitNamePosition(comboiby.Text, preparedByName, tmpPos) ' ignore position for prepared_by

        Using conn As New SqlClient.SqlConnection(ConnStr)
            Await conn.OpenAsync()
            Using tx = conn.BeginTransaction(IsolationLevel.ReadCommitted)
                Try
                    Using cmdInit As New SqlClient.SqlCommand("SET XACT_ABORT ON; SET NOCOUNT ON;", conn, tx)
                        cmdInit.ExecuteNonQuery()
                    End Using

                    For Each row As DataGridViewRow In DataGridView1.Rows
                        If row.IsNewRow Then Continue For

                        Dim qty As Decimal = Convert.ToDecimal(row.Cells("colQty").Value)
                        Dim unit As String = Convert.ToString(row.Cells("colUnit").Value)
                        Dim descText As String = Convert.ToString(row.Cells("colDesc").Value)
                        Dim pno As String = Convert.ToString(row.Cells("colPNo").Value)
                        Dim dAcq As Date = Convert.ToDateTime(row.Cells("colDAcq").Value)
                        Dim ucost As Decimal = Convert.ToDecimal(row.Cells("colAmt").Value)
                        Dim tcost As Decimal = ucost * qty

                        Dim newId As Integer
                        Using cmd As New SqlClient.SqlCommand("
                        INSERT INTO TBL_PPE
                        (par_no, par_date,
                         description, property_number, date_acquired,
                         qty, unit, unit_cost, total_cost,
                         par_to, position, prepared_by, fcluster)
                        VALUES
                        (@par, @pdate,
                         @desc, @pno, @dacq,
                         @qty, @unit, @ucost, @tcost,
                         @pto, @pos, @pby, @fcl);
                        SELECT CAST(SCOPE_IDENTITY() AS int);", conn, tx)

                            cmd.Parameters.AddWithValue("@par", parNo)
                            cmd.Parameters.AddWithValue("@pdate", parDate)
                            cmd.Parameters.AddWithValue("@desc", descText)
                            cmd.Parameters.AddWithValue("@pno", pno)
                            cmd.Parameters.AddWithValue("@dacq", dAcq)
                            cmd.Parameters.AddWithValue("@qty", qty)
                            cmd.Parameters.AddWithValue("@unit", unit)
                            cmd.Parameters.AddWithValue("@ucost", ucost)
                            cmd.Parameters.AddWithValue("@tcost", tcost)

                            cmd.Parameters.AddWithValue("@pto", If(String.IsNullOrWhiteSpace(parToName), CType(DBNull.Value, Object), parToName))
                            cmd.Parameters.AddWithValue("@pos", If(String.IsNullOrWhiteSpace(parToPos), CType(DBNull.Value, Object), parToPos))
                            cmd.Parameters.AddWithValue("@pby", If(String.IsNullOrWhiteSpace(preparedByName), CType(DBNull.Value, Object), preparedByName))
                            cmd.Parameters.AddWithValue("@fcl", If(String.IsNullOrWhiteSpace(combofcluster.Text), CType(DBNull.Value, Object), combofcluster.Text.Trim()))

                            newId = CInt(Await cmd.ExecuteScalarAsync())
                        End Using

                        row.Cells("colID").Value = newId

                        Using cmdInv As New SqlClient.SqlCommand("
                        UPDATE TBL_INVENTORY
                        SET QTY = QTY - @qty
                        WHERE (IPNO = @pno) OR (IPNO IS NULL AND DESCRIPTIONS = @desc);", conn, tx)
                            cmdInv.Parameters.AddWithValue("@qty", qty)
                            cmdInv.Parameters.AddWithValue("@pno", pno)
                            cmdInv.Parameters.AddWithValue("@desc", descText)
                            Await cmdInv.ExecuteNonQueryAsync()
                        End Using
                    Next

                    tx.Commit()
                Catch
                    tx.Rollback()
                    Throw
                End Try
            End Using
        End Using
    End Function





    Private Function ComboValueOrText(cb As ComboBox) As String
        Dim v As Object = cb.SelectedValue
        If v IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(v.ToString()) Then
            Return v.ToString().Trim()
        End If
        Return cb.Text.Trim()
    End Function

    Private Async Function UpdatePPEAsync(parNo As String, parDate As Date) As Task
        Dim parToName As String = "", parToPos As String = ""
        SplitNamePosition(comborby.Text, parToName, parToPos)

        Dim preparedByName As String = "", tmpPos As String = ""
        SplitNamePosition(comboiby.Text, preparedByName, tmpPos)

        Using conn As New SqlClient.SqlConnection(ConnStr)
            Await conn.OpenAsync()
            Using tx = conn.BeginTransaction(IsolationLevel.ReadCommitted)
                Try
                    Using cmdInit As New SqlClient.SqlCommand("SET XACT_ABORT ON; SET NOCOUNT ON;", conn, tx)
                        cmdInit.ExecuteNonQuery()
                    End Using

                    For Each row As DataGridViewRow In DataGridView1.Rows
                        If row.IsNewRow Then Continue For

                        Dim idObj As Object = row.Cells("colID").Value
                        Dim qtyNew As Decimal = Convert.ToDecimal(row.Cells("colQty").Value)
                        Dim unit As String = Convert.ToString(row.Cells("colUnit").Value)
                        Dim descText As String = Convert.ToString(row.Cells("colDesc").Value)
                        Dim pno As String = Convert.ToString(row.Cells("colPNo").Value)
                        Dim dAcq As Date = Convert.ToDateTime(row.Cells("colDAcq").Value)
                        Dim ucost As Decimal = Convert.ToDecimal(row.Cells("colAmt").Value)
                        Dim tcost As Decimal = ucost * qtyNew

                        If idObj Is Nothing OrElse idObj Is DBNull.Value OrElse String.IsNullOrWhiteSpace(idObj.ToString()) Then
                            ' New line in Update mode → INSERT + deduct stock
                            Dim newId As Integer
                            Using cmdIns As New SqlClient.SqlCommand("
                            INSERT INTO TBL_PPE
                            (par_no, par_date,
                             description, property_number, date_acquired,
                             qty, unit, unit_cost, total_cost,
                             par_to, position, prepared_by, fcluster)
                            VALUES
                            (@par, @pdate,
                             @desc, @pno, @dacq,
                             @qty, @unit, @ucost, @tcost,
                             @pto, @pos, @pby, @fcl);
                            SELECT CAST(SCOPE_IDENTITY() AS int);", conn, tx)

                                cmdIns.Parameters.AddWithValue("@par", parNo)
                                cmdIns.Parameters.AddWithValue("@pdate", parDate)
                                cmdIns.Parameters.AddWithValue("@desc", descText)
                                cmdIns.Parameters.AddWithValue("@pno", pno)
                                cmdIns.Parameters.AddWithValue("@dacq", dAcq)
                                cmdIns.Parameters.AddWithValue("@qty", qtyNew)
                                cmdIns.Parameters.AddWithValue("@unit", unit)
                                cmdIns.Parameters.AddWithValue("@ucost", ucost)
                                cmdIns.Parameters.AddWithValue("@tcost", tcost)

                                cmdIns.Parameters.AddWithValue("@pto", If(String.IsNullOrWhiteSpace(parToName), CType(DBNull.Value, Object), parToName))
                                cmdIns.Parameters.AddWithValue("@pos", If(String.IsNullOrWhiteSpace(parToPos), CType(DBNull.Value, Object), parToPos))
                                cmdIns.Parameters.AddWithValue("@pby", If(String.IsNullOrWhiteSpace(preparedByName), CType(DBNull.Value, Object), preparedByName))
                                cmdIns.Parameters.AddWithValue("@fcl", If(String.IsNullOrWhiteSpace(combofcluster.Text), CType(DBNull.Value, Object), combofcluster.Text.Trim()))

                                newId = CInt(Await cmdIns.ExecuteScalarAsync())
                            End Using
                            row.Cells("colID").Value = newId

                            Using cmdInv As New SqlClient.SqlCommand("
                            UPDATE TBL_INVENTORY
                            SET QTY = QTY - @qty
                            WHERE (IPNO = @pno) OR (IPNO IS NULL AND DESCRIPTIONS = @desc);", conn, tx)
                                cmdInv.Parameters.AddWithValue("@qty", qtyNew)
                                cmdInv.Parameters.AddWithValue("@pno", pno)
                                cmdInv.Parameters.AddWithValue("@desc", descText)
                                Await cmdInv.ExecuteNonQueryAsync()
                            End Using

                        Else
                            ' Existing line → UPDATE + adjust inventory by delta
                            Dim id As Integer = CInt(idObj)

                            Dim qtyOld As Decimal = 0D
                            Using cmdGet As New SqlClient.SqlCommand("
                            SELECT qty FROM TBL_PPE WITH (NOLOCK) WHERE ID = @id;", conn, tx)
                                cmdGet.Parameters.AddWithValue("@id", id)
                                Dim objQty = Await cmdGet.ExecuteScalarAsync()
                                If objQty IsNot Nothing AndAlso objQty IsNot DBNull.Value Then
                                    qtyOld = Convert.ToDecimal(objQty)
                                End If
                            End Using

                            Using cmdUp As New SqlClient.SqlCommand("
                            UPDATE TBL_PPE
                            SET par_no=@par, par_date=@pdate,
                                description=@desc, property_number=@pno, date_acquired=@dacq,
                                qty=@qty, unit=@unit, unit_cost=@ucost, total_cost=@tcost,
                                par_to=@pto, position=@pos, prepared_by=@pby, fcluster=@fcl
                            WHERE ID=@id;", conn, tx)

                                cmdUp.Parameters.AddWithValue("@par", parNo)
                                cmdUp.Parameters.AddWithValue("@pdate", parDate)
                                cmdUp.Parameters.AddWithValue("@desc", descText)
                                cmdUp.Parameters.AddWithValue("@pno", pno)
                                cmdUp.Parameters.AddWithValue("@dacq", dAcq)
                                cmdUp.Parameters.AddWithValue("@qty", qtyNew)
                                cmdUp.Parameters.AddWithValue("@unit", unit)
                                cmdUp.Parameters.AddWithValue("@ucost", ucost)
                                cmdUp.Parameters.AddWithValue("@tcost", tcost)

                                cmdUp.Parameters.AddWithValue("@pto", If(String.IsNullOrWhiteSpace(parToName), CType(DBNull.Value, Object), parToName))
                                cmdUp.Parameters.AddWithValue("@pos", If(String.IsNullOrWhiteSpace(parToPos), CType(DBNull.Value, Object), parToPos))
                                cmdUp.Parameters.AddWithValue("@pby", If(String.IsNullOrWhiteSpace(preparedByName), CType(DBNull.Value, Object), preparedByName))
                                cmdUp.Parameters.AddWithValue("@fcl", If(String.IsNullOrWhiteSpace(combofcluster.Text), CType(DBNull.Value, Object), combofcluster.Text.Trim()))

                                cmdUp.Parameters.AddWithValue("@id", id)
                                Await cmdUp.ExecuteNonQueryAsync()
                            End Using

                            Dim delta As Decimal = qtyNew - qtyOld
                            If delta <> 0D Then
                                Using cmdInv As New SqlClient.SqlCommand("
                                UPDATE TBL_INVENTORY
                                SET QTY = QTY - @delta
                                WHERE (IPNO = @pno) OR (IPNO IS NULL AND DESCRIPTIONS = @desc);", conn, tx)
                                    cmdInv.Parameters.AddWithValue("@delta", delta)
                                    cmdInv.Parameters.AddWithValue("@pno", pno)
                                    cmdInv.Parameters.AddWithValue("@desc", descText)
                                    Await cmdInv.ExecuteNonQueryAsync()
                                End Using
                            End If
                        End If
                    Next

                    tx.Commit()
                Catch
                    tx.Rollback()
                    Throw
                End Try
            End Using
        End Using
    End Function





    Private Async Sub cmddelete_Click(sender As Object, e As EventArgs) Handles cmddelete.Click
        If DataGridView1.SelectedRows.Count = 0 Then Return

        ' Confirm
        If MessageBox.Show("Delete selected item(s)?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
            Return
        End If

        Using conn As New SqlClient.SqlConnection(ConnStr)
            Await conn.OpenAsync()
            Using tx = conn.BeginTransaction(IsolationLevel.ReadCommitted)
                Try
                    Using cmdInit As New SqlClient.SqlCommand("SET XACT_ABORT ON; SET NOCOUNT ON;", conn, tx)
                        cmdInit.ExecuteNonQuery()
                    End Using

                    For Each r As DataGridViewRow In DataGridView1.SelectedRows
                        If r.IsNewRow Then Continue For

                        Dim idObj = r.Cells("colID").Value
                        Dim qty As Decimal = 0D
                        Dim pno As String = Convert.ToString(r.Cells("colPNo").Value)
                        Dim descText As String = Convert.ToString(r.Cells("colDesc").Value)

                        If idObj Is Nothing OrElse idObj Is DBNull.Value OrElse CStr(idObj).Trim() = "" Then
                            ' Not yet in DB → just remove row
                            DataGridView1.Rows.Remove(r)
                        Else
                            Dim id As Integer = Convert.ToInt32(idObj)

                            ' Get qty to return to inventory
                            Using cmdGet As New SqlClient.SqlCommand("
                            SELECT QTY FROM TBL_PPE WITH (NOLOCK) WHERE ID=@id;", conn, tx)
                                cmdGet.Parameters.AddWithValue("@id", id)
                                Dim obj = Await cmdGet.ExecuteScalarAsync()
                                If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then qty = Convert.ToDecimal(obj)
                            End Using

                            ' Delete item
                            Using cmdDel As New SqlClient.SqlCommand("DELETE FROM TBL_PPE WHERE ID=@id;", conn, tx)
                                cmdDel.Parameters.AddWithValue("@id", id)
                                Await cmdDel.ExecuteNonQueryAsync()
                            End Using

                            ' Return stock to inventory
                            Using cmdInv As New SqlClient.SqlCommand("
                            UPDATE TBL_INVENTORY
                            SET QTY = QTY + @qty
                            WHERE (IPNO = @pno) OR (IPNO IS NULL AND DESCRIPTIONS = @desc);", conn, tx)
                                cmdInv.Parameters.AddWithValue("@qty", qty)
                                cmdInv.Parameters.AddWithValue("@pno", pno)
                                cmdInv.Parameters.AddWithValue("@desc", descText)
                                Await cmdInv.ExecuteNonQueryAsync()
                            End Using

                            ' Remove from grid
                            DataGridView1.Rows.Remove(r)
                        End If
                    Next

                    tx.Commit()
                Catch ex As Exception
                    tx.Rollback()
                    MessageBox.Show("Delete error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End Using
        End Using
    End Sub

    Private Sub SplitNamePosition(inputText As String, ByRef outName As String, ByRef outPos As String)
        outName = "" : outPos = ""
        If String.IsNullOrWhiteSpace(inputText) Then Exit Sub
        Dim s = inputText.Trim()
        Dim p = s.IndexOf("|"c)
        If p >= 0 Then
            outName = s.Substring(0, p).Trim()
            outPos = s.Substring(p + 1).Trim()
            Exit Sub
        End If
        s = s.Replace(vbCrLf, vbLf).Replace(vbCr, vbLf)
        Dim lf = s.IndexOf(vbLf)
        If lf >= 0 Then
            outName = s.Substring(0, lf).Trim()
            outPos = s.Substring(lf + 1).Trim()
        Else
            outName = s
        End If
    End Sub


    ' Returns the text the user sees (display text)
    Private Function ComboDisplay(cb As ComboBox) As String
        If cb.SelectedItem IsNot Nothing Then
            Return cb.Text.Trim() ' in DropDownList this is Display; in DropDown it's current text
        End If
        Return cb.Text.Trim()
    End Function

    ' Convenience: get Name & Position from comborby
    Private Sub GetParToAndPosition(ByRef parTo As String, ByRef pos As String)
        SplitNamePosition(ComboDisplay(comborby), parTo, pos)
    End Sub

    ' Convenience: get Name only from comboiby (Prepared by)
    Private Function GetPreparedByName() As String
        Dim nm As String = "", ps As String = ""
        SplitNamePosition(ComboDisplay(comboiby), nm, ps)
        Return nm
    End Function

    Private Sub SelectOrInjectByDisplay(cb As ComboBox, display As String)
        If String.IsNullOrWhiteSpace(display) Then
            If cb.Items.Count > 0 Then cb.SelectedIndex = 0
            Return
        End If

        Dim data = TryCast(cb.DataSource, List(Of EmpItem))
        If data Is Nothing Then
            ' not bound? just add the display string
            cb.Items.Clear()
            cb.Items.Add(display)
            cb.SelectedIndex = 0
            Return
        End If

        Dim idx As Integer = data.FindIndex(Function(x) String.Equals(x.Display, display, StringComparison.OrdinalIgnoreCase))
        If idx >= 0 Then
            cb.SelectedIndex = idx
        Else
            data.Insert(0, New EmpItem With {.Display = display, .Value = display})
            cb.DataSource = Nothing
            cb.DataSource = data
            cb.DisplayMember = "Display"
            cb.ValueMember = "Value"
            cb.SelectedIndex = 0
        End If
    End Sub

    ' Find by name only (prepared_by). Matches any "Name | Position" that starts with the name.
    Private Sub SelectByNamePrefix(cb As ComboBox, empName As String)
        If String.IsNullOrWhiteSpace(empName) Then
            If cb.Items.Count > 0 Then cb.SelectedIndex = 0
            Return
        End If
        Dim data = TryCast(cb.DataSource, List(Of EmpItem))
        If data IsNot Nothing Then
            Dim idx As Integer = data.FindIndex(Function(x) x.Display.StartsWith(empName.Trim() & " |", StringComparison.OrdinalIgnoreCase) _
                                                         OrElse String.Equals(x.Display.Trim(), empName.Trim(), StringComparison.OrdinalIgnoreCase))
            If idx >= 0 Then
                cb.SelectedIndex = idx
                Return
            End If
        End If
        ' inject fallback when not present
        SelectOrInjectByDisplay(cb, empName.Trim())
    End Sub

    ' Public method: prepare the file form for managing an existing PAR
    Public Async Function OpenForManageAsync(parNo As String) As Task
        ' Lock header fields and set mode
        txtparno.Enabled = False
        dtpardate.Enabled = False
        cmdsave.Text = "Update"
        cmdprint.Enabled = True

        txtparno.Text = parNo

        ' Ensure employee combos are loaded
        Await LoadEmployeeCombosAsync()

        ' Load header fields from DB (par_date, par_to, position, prepared_by)
        Dim parDate As Date = Date.Today
        Dim parTo As String = "", pos As String = "", preparedBy As String = "", fcluster As String = ""

        Using conn As New SqlClient.SqlConnection(My.Forms.frmmain.txtdb.Text)
            Using cmd As New SqlClient.SqlCommand("
            SELECT TOP 1 par_date, par_to, position, prepared_by, fcluster
            FROM TBL_PPE WITH (NOLOCK)
            WHERE par_no = @p
            ORDER BY ID;", conn)
                cmd.Parameters.AddWithValue("@p", parNo)
                Await conn.OpenAsync()
                Using rdr = Await cmd.ExecuteReaderAsync()
                    If Await rdr.ReadAsync() Then
                        If Not Convert.IsDBNull(rdr("par_date")) Then parDate = Convert.ToDateTime(rdr("par_date"))
                        parTo = If(Convert.IsDBNull(rdr("par_to")), "", rdr("par_to").ToString())
                        pos = If(Convert.IsDBNull(rdr("position")), "", rdr("position").ToString())
                        preparedBy = If(Convert.IsDBNull(rdr("prepared_by")), "", rdr("prepared_by").ToString())
                        fcluster = If(Convert.IsDBNull(rdr("fcluster")), "", rdr("fcluster").ToString())
                    End If
                End Using
            End Using
        End Using

        dtpardate.Value = parDate

        ' Received by → comborby: "Name | Position"
        Dim rcvDisplay As String = If(String.IsNullOrWhiteSpace(parTo), "", (parTo & If(String.IsNullOrWhiteSpace(pos), "", " | " & pos)).Trim())
        SelectOrInjectByDisplay(comborby, rcvDisplay)

        ' Issued by → comboiby: by Name only (no position)
        SelectByNamePrefix(comboiby, preparedBy)

        ' Section (if you have a section control)
        If combofcluster IsNot Nothing Then
            If Not String.IsNullOrWhiteSpace(fcluster) Then
                combofcluster.Text = fcluster
            End If
        End If

        ' Load grid rows for this PAR
        Await ReloadPPEItemsAsync(parNo)
    End Function

    Private Function FindComboIndexByDisplay(cb As ComboBox, display As String) As Integer
        If cb Is Nothing OrElse cb.Items Is Nothing Then Return -1
        Dim target As String = display.Trim()
        For i As Integer = 0 To cb.Items.Count - 1
            Dim txt As String = cb.GetItemText(cb.Items(i)).Trim()
            If String.Equals(txt, target, StringComparison.OrdinalIgnoreCase) Then
                Return i
            End If
        Next
        Return -1
    End Function

    Public Async Function InitializeForManage(parNo As String) As Task
        _isManageMode = True
        _managedParNo = parNo

        EnsureGridReady()

        ' Lock header fields
        txtparno.Text = parNo
        txtparno.Enabled = False : txtparno.ReadOnly = True
        dtpardate.Enabled = False

        ' Load employee lists (for comborby/comboiby)
        If comborby.DataSource Is Nothing OrElse comboiby.DataSource Is Nothing Then
            Await LoadEmployeeCombosAsync()
        End If

        ' --- Load header (date, par_to, position, prepared_by, fcluster) ---
        Dim parDate As Date = Date.Today
        Dim parTo As String = "", pos As String = "", preparedBy As String = "", fcluster As String = ""

        Using conn As New SqlClient.SqlConnection(My.Forms.frmmain.txtdb.Text)
            Using cmd As New SqlClient.SqlCommand("
            SELECT TOP 1 par_date, par_to, position, prepared_by, fcluster
            FROM TBL_PPE WITH (NOLOCK)
            WHERE par_no = @p
            ORDER BY ID DESC;", conn)
                cmd.Parameters.AddWithValue("@p", parNo)
                Await conn.OpenAsync()
                Using rdr = Await cmd.ExecuteReaderAsync()
                    If Await rdr.ReadAsync() Then
                        If Not Convert.IsDBNull(rdr("par_date")) Then parDate = CDate(rdr("par_date"))
                        parTo = If(Convert.IsDBNull(rdr("par_to")), "", rdr("par_to").ToString())
                        pos = If(Convert.IsDBNull(rdr("position")), "", rdr("position").ToString())
                        preparedBy = If(Convert.IsDBNull(rdr("prepared_by")), "", rdr("prepared_by").ToString())
                        fcluster = If(Convert.IsDBNull(rdr("fcluster")), "", rdr("fcluster").ToString())
                    End If
                End Using
            End Using
        End Using

        dtpardate.Value = parDate

        Dim rcvDisplay As String = If(String.IsNullOrWhiteSpace(pos), parTo, parTo & " | " & pos)
        comborby.DropDownStyle = ComboBoxStyle.DropDownList
        EnsureComboShows(comborby, rcvDisplay)

        ' Issued by (Name only)
        comboiby.DropDownStyle = ComboBoxStyle.DropDownList
        EnsureComboShows(comboiby, preparedBy, preferPrefixMatch:=True)

        ' Fund Cluster
        combofcluster.DropDownStyle = ComboBoxStyle.DropDown
        combofcluster.Text = fcluster

        ' Load items
        Await ReloadPPEItemsAsync(parNo)

        cmdprint.Enabled = True
        cmdsave.Text = "Update"
    End Function



    ' Build the grid if not yet built (safe to call many times)
    Private Sub EnsureGridReady()
        If DataGridView1.Columns.Count = 0 Then
            SetupGrid()
        End If
    End Sub


    Public Sub AddItemRow(qty As Decimal, unit As String, descText As String, pNo As String, dAcquired As Date, amount As Decimal)
        EnsureGridReady()                    ' <-- add
        Dim rowIdx = DataGridView1.Rows.Add()
        With DataGridView1.Rows(rowIdx)
            .Cells("colID").Value = DBNull.Value
            .Cells("colQty").Value = qty
            .Cells("colUnit").Value = unit
            .Cells("colDesc").Value = descText
            .Cells("colPNo").Value = pNo
            .Cells("colDAcq").Value = dAcquired
            .Cells("colAmt").Value = amount
        End With
    End Sub

    ' Ensure a combo shows a specific display text, even if it is not in the current DataSource.
    ' Works with data-bound or non data-bound combos. If not found, it inserts a temporary item.
    Private Sub EnsureComboShows(cb As ComboBox, desiredDisplay As String, Optional preferPrefixMatch As Boolean = False)
        If cb Is Nothing Then Exit Sub
        Dim target As String = If(desiredDisplay, "").Trim()

        ' Find existing item
        Dim foundIdx As Integer = -1
        For i As Integer = 0 To cb.Items.Count - 1
            Dim txt As String = cb.GetItemText(cb.Items(i)).Trim()
            If String.Equals(txt, target, StringComparison.OrdinalIgnoreCase) Then
                foundIdx = i : Exit For
            End If
            If preferPrefixMatch AndAlso
           txt.StartsWith(target & " |", StringComparison.OrdinalIgnoreCase) Then
                foundIdx = i : Exit For
            End If
        Next
        If foundIdx >= 0 Then
            cb.SelectedIndex = foundIdx
            Return
        End If

        ' Not found: rebuild a bindable list and inject the value
        Dim entries As New List(Of Object)
        If cb.DataSource IsNot Nothing Then
            For Each obj In CType(cb.DataSource, System.Collections.IEnumerable)
                entries.Add(obj)
            Next
        Else
            For Each obj In cb.Items
                entries.Add(obj)
            Next
        End If

        ' If your items are EmpItem { Display, Value }, create one; otherwise add plain string
        Dim usesEmpItem As Boolean = (entries.Count > 0 AndAlso entries(0).[GetType]().GetProperty("Display") IsNot Nothing)
        Dim newItem As Object
        If usesEmpItem Then
            ' EmpItem defined earlier in your code
            newItem = New EmpItem With {.Display = target, .Value = target}
        Else
            newItem = target
        End If

        ' Insert after placeholder "-- Select --" if present
        Dim insertIdx As Integer = 0
        If cb.Items.Count > 0 AndAlso cb.GetItemText(cb.Items(0)).Contains("Select") Then insertIdx = 1 Else insertIdx = cb.Items.Count

        If cb.DataSource IsNot Nothing Then
            entries.Insert(insertIdx, newItem)
            cb.DataSource = Nothing
            cb.Items.Clear()
            cb.DataSource = entries
            If usesEmpItem Then
                cb.DisplayMember = "Display"
                cb.ValueMember = "Value"
            End If
        Else
            cb.Items.Insert(insertIdx, newItem)
        End If

        cb.SelectedIndex = insertIdx
    End Sub

    Private Sub Cmdclose_Click(sender As Object, e As EventArgs) Handles cmdclose.Click
        Me.Dispose()
    End Sub

    Private Sub cmdprint_Click(sender As Object, e As EventArgs) Handles cmdprint.Click
        If DataGridView1 Is Nothing OrElse DataGridView1.Rows.Count = 0 Then
            MessageBox.Show("No items to print.", "PAR", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        frmpprev.Dispose()

        ' Disable submit panel during preview
        Dim panelSubmitCtrl As Control = Nothing
        If Me.Controls.ContainsKey("panelsubmit") Then
            panelSubmitCtrl = Me.Controls("panelsubmit")
            panelSubmitCtrl.Enabled = False
        End If

        Try
            ' ───────── Build DSPAR.DTPAR table ─────────
            Dim dt As New DataTable("DTPAR")
            dt.Columns.Add("qty", GetType(Decimal))
            dt.Columns.Add("unit", GetType(String))
            dt.Columns.Add("description", GetType(String))
            dt.Columns.Add("property_number", GetType(String))
            dt.Columns.Add("date_aquired", GetType(Date))     ' (spelling per your schema)
            dt.Columns.Add("total_cost", GetType(Decimal))

            For Each r As DataGridViewRow In DataGridView1.Rows
                If r.IsNewRow Then Continue For

                Dim qty As Decimal = ToDecimalSafe(r.Cells("colQty").Value)
                Dim unit As String = If(r.Cells("colUnit").Value, "").ToString()
                Dim descText As String = If(r.Cells("colDesc").Value, "").ToString()
                Dim pno As String = If(r.Cells("colPNo").Value, "").ToString()

                Dim dAcq As Date = Date.Today
                If r.Cells("colDAcq").Value IsNot Nothing AndAlso r.Cells("colDAcq").Value IsNot DBNull.Value Then
                    dAcq = CDate(r.Cells("colDAcq").Value)
                End If

                ' colAmt is UNIT COST; RDLC wants total_cost
                Dim ucost As Decimal = ToDecimalSafe(r.Cells("colAmt").Value)
                Dim tcost As Decimal = qty * ucost

                dt.Rows.Add(qty, unit, descText, pno, dAcq, tcost)
            Next

            ' ───────── Report parameters ─────────
            Dim rcvName As String = "", rcvPos As String = ""
            SplitNamePosition(comborby.Text, rcvName, rcvPos)    ' Received by

            Dim issName As String = "", issPos As String = ""
            SplitNamePosition(comboiby.Text, issName, issPos)    ' Issued by

            Dim parms As New List(Of ReportParameter) From {
            New ReportParameter("ename", txtpentity.Text.Trim()),
            New ReportParameter("fcluster", combofcluster.Text.Trim()),
            New ReportParameter("PARNo", txtparno.Text.Trim()),
            New ReportParameter("RName", rcvName),
            New ReportParameter("RPosition", rcvPos),    ' (param name per your spec)
            New ReportParameter("IName", issName),
            New ReportParameter("IPosition", issPos)
        }

            ' ───────── Bind to frmpprev.ReportViewer1 ─────────
            Dim rdlcPath As String = Path.Combine(Application.StartupPath, "report", "rptpar.rdlc")
            If Not File.Exists(rdlcPath) Then
                MessageBox.Show("Report file not found: " & rdlcPath, "Report", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            With frmpprev.ReportViewer1
                .Reset()
                .ProcessingMode = ProcessingMode.Local
                .LocalReport.ReportPath = rdlcPath
                .LocalReport.DataSources.Clear()

                ' IMPORTANT: Name must match the RDLC dataset name.
                ' If you get “data source not defined”, try "DTPAR" or check RDLC’s DataSet name.
                Dim rds As New ReportDataSource("DSPAR", dt)
                .LocalReport.DataSources.Add(rds)
                .SetDisplayMode(DisplayMode.PrintLayout)
                .ZoomMode = ZoomMode.Percent
                .RefreshReport()
                .LocalReport.SetParameters(parms)
                .RefreshReport()
            End With
            frmpprev.panelsubmit.Visible = False
            frmpprev.ShowDialog(Me)

        Finally
            ' Re-enable submit panel after closing preview
            If panelSubmitCtrl IsNot Nothing Then panelSubmitCtrl.Enabled = True
        End Try
    End Sub

    ' Utility: robust decimal parsing from grid cells (handles commas)
    Private Function ToDecimalSafe(val As Object) As Decimal
        If val Is Nothing OrElse val Is DBNull.Value Then Return 0D
        Dim s As String = val.ToString().Trim()
        Dim d As Decimal
        If Decimal.TryParse(s, NumberStyles.Any, CultureInfo.CurrentCulture, d) Then Return d
        If Decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, d) Then Return d
        s = s.Replace(",", "")
        If Decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, d) Then Return d
        Return 0D
    End Function

    Private Sub combofcluster_KeyPress(sender As Object, e As KeyPressEventArgs) Handles combofcluster.KeyPress
        e.Handled = True
    End Sub
End Class
