Imports System.Data.SqlClient
Imports System.Globalization
Imports System.Linq

Public Class frmissuanceSEPFile

    ' ─────────────────────────────────────────────────────────────────────────────
    ' Flags / backing lists
    ' ─────────────────────────────────────────────────────────────────────────────
    Private _employeeNames As New List(Of String)()
    Private _isManageMode As Boolean = False        ' <— prevents Load() from rebinding combos
    Private _combosLoaded As Boolean = False        ' <— remember if combos already bound

    ' ─────────────────────────────────────────────────────────────────────────────
    ' Expose header values
    ' ─────────────────────────────────────────────────────────────────────────────
    Public ReadOnly Property HeaderICSNo As String
        Get
            Try
                Return If(txticsno IsNot Nothing, txticsno.Text.Trim(), "")
            Catch
                Return ""
            End Try
        End Get
    End Property

    Public ReadOnly Property HeaderICSDate As Date
        Get
            Try
                Return If(dticsdate IsNot Nothing, dticsdate.Value.Date, Date.Today)
            Catch
                Return Date.Today
            End Try
        End Get
    End Property

    Public ReadOnly Property HeaderFCluster As String
        Get
            Try
                Return If(combofcluster IsNot Nothing, combofcluster.Text.Trim(), "")
            Catch
                Return ""
            End Try
        End Get
    End Property

    Public ReadOnly Property HeaderParMRToName As String
        Get
            Dim t = ExtractNameAndPositionSafe(If(If(comborby Is Nothing, "", comborby.Text), ""))
            Return t.Item1
        End Get
    End Property

    Public ReadOnly Property HeaderParMRToPosition As String
        Get
            Dim t = ExtractNameAndPositionSafe(If(If(comborby Is Nothing, "", comborby.Text), ""))
            Return t.Item2
        End Get
    End Property

    Public ReadOnly Property HeaderPreparedByName As String
        Get
            Dim t = ExtractNameAndPositionSafe(If(If(comboiby Is Nothing, "", comboiby.Text), ""))
            Return t.Item1
        End Get
    End Property

    Private Function ExtractNameAndPositionSafe(s As String) As Tuple(Of String, String)
        If String.IsNullOrWhiteSpace(s) Then Return Tuple.Create("", "")
        Dim parts = s.Split(New Char() {"|"c}, 2, StringSplitOptions.None)
        Dim name As String = parts(0).Trim()
        Dim pos As String = If(parts.Length > 1, parts(1).Trim(), "")
        Return Tuple.Create(name, pos)
    End Function

    ' ─────────────────────────────────────────────────────────────────────────────
    ' Load
    ' ─────────────────────────────────────────────────────────────────────────────
    Private Async Sub frmissuanceSEPFile_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            EnsureGridReady()

            With DataGridView1
                .ReadOnly = True
                .AllowUserToAddRows = False
                .AllowUserToDeleteRows = False
                .MultiSelect = True
                .SelectionMode = DataGridViewSelectionMode.FullRowSelect
            End With

            ' If not in Manage mode, use default caption
            If Not _isManageMode Then
                cmdsave.Text = If(String.IsNullOrWhiteSpace(txticsno.Text), "Save", "Update")
            End If

            If dticsdate.Value = Date.MinValue Then
                dticsdate.Value = Date.Today
            End If

            ' Only bind combos during Load when NOT in manage mode and not already loaded
            If Not _isManageMode AndAlso Not _combosLoaded Then
                PrepEmployeeCombos()
                Dim names = Await LoadEmployeeNamesAsync().ConfigureAwait(True)
                BindEmployeeCombos(names)
            End If

        Catch ex As Exception
            MessageBox.Show("Load error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ─────────────────────────────────────────────────────────────────────────────
    ' Connection helper
    ' ─────────────────────────────────────────────────────────────────────────────
    Private ReadOnly Property ConnStr As String
        Get
            Return My.Forms.frmmain.txtdb.Text
        End Get
    End Property

    ' ─────────────────────────────────────────────────────────────────────────────
    ' Employee lists / combos
    ' ─────────────────────────────────────────────────────────────────────────────
    Private Sub BindEmployeeCombos(names As List(Of String))
        If names Is Nothing Then names = New List(Of String)

        Dim rList As List(Of String) = names.ToList() ' comborby
        Dim iList As List(Of String) = names.ToList() ' comboiby

        rList.Insert(0, "-- Select --")
        iList.Insert(0, "-- Select --")

        BindOneCombo(comborby, rList)
        BindOneCombo(comboiby, iList)

        _combosLoaded = True
    End Sub

    Private Sub BindOneCombo(cb As ComboBox, data As List(Of String))
        If cb Is Nothing Then Return
        cb.DataSource = Nothing
        cb.Items.Clear()
        cb.DataSource = If(data Is Nothing, New List(Of String), data.ToList()) ' clone again
        cb.DropDownStyle = ComboBoxStyle.DropDownList
        cb.AutoCompleteMode = AutoCompleteMode.None
        cb.AutoCompleteSource = AutoCompleteSource.ListItems
        cb.SelectedIndex = 0
    End Sub

    Private Async Function LoadEmployeeNamesAsync() As Task(Of List(Of String))
        Dim result As New List(Of String)()
        Using conn As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("
            SELECT CNAMES
            FROM TBL_APPROVER WITH (NOLOCK)
            WHERE [TYPE] = 'Employee'
            ORDER BY CNAMES;", conn)

                Await conn.OpenAsync().ConfigureAwait(False)
                Using rdr = Await cmd.ExecuteReaderAsync().ConfigureAwait(False)
                    While Await rdr.ReadAsync().ConfigureAwait(False)
                        Dim raw As String = If(TryCast(rdr("CNAMES"), String), "").Trim()
                        If raw <> "" Then
                            Dim formatted = FormatNamePosition(raw)
                            If formatted <> "" Then result.Add(formatted)
                        End If
                    End While
                End Using
            End Using
        End Using

        result = result.
            Distinct(StringComparer.OrdinalIgnoreCase).
            OrderBy(Function(s) s, StringComparer.OrdinalIgnoreCase).
            ToList()

        _employeeNames = result
        Return result
    End Function

    Private Function NormalizeText(s As String) As String
        If s Is Nothing Then Return ""
        s = s.Replace(vbCrLf, " ").Replace(vbCr, " ").Replace(vbLf, " ")
        s = System.Text.RegularExpressions.Regex.Replace(s, "\s+", " ")
        Return s.Trim().ToUpperInvariant()
    End Function

    Private Function NameOnly(display As String) As String
        If String.IsNullOrWhiteSpace(display) Then Return ""
        Dim s = display.Replace(vbCrLf, vbLf).Replace(vbCr, vbLf)
        Dim first = s.Split(New String() {vbLf}, StringSplitOptions.None)(0)
        Dim p = first.IndexOf("|"c)
        If p >= 0 Then first = first.Substring(0, p)
        Return first.Trim()
    End Function

    Private Function FormatNamePosition(input As String) As String
        If String.IsNullOrWhiteSpace(input) Then Return ""
        Dim normalized = input.Replace(vbCrLf, vbLf).Replace(vbCr, vbLf)
        Dim parts = normalized.Split({vbLf}, StringSplitOptions.RemoveEmptyEntries)
        Dim name As String = parts(0).Trim()
        Dim pos As String = If(parts.Length > 1, parts(1).Trim(), "")
        If pos <> "" Then
            Return $"{name} | {pos}"
        Else
            Return name
        End If
    End Function

    Private Function ExtractFirstLine(input As String) As String
        If String.IsNullOrWhiteSpace(input) Then Return ""
        Dim normalized = input.Replace(vbCrLf, vbLf).Replace(vbCr, vbLf)
        Dim idx = normalized.IndexOf(vbLf, StringComparison.Ordinal)
        Dim first = If(idx >= 0, normalized.Substring(0, idx), normalized)
        Return first.Trim()
    End Function

    Private Sub PrepEmployeeCombos()
        PrepOneCombo(comborby)
        PrepOneCombo(comboiby)
    End Sub

    Private Sub PrepOneCombo(cb As ComboBox)
        If cb Is Nothing Then Return
        cb.DropDownStyle = ComboBoxStyle.DropDownList
        cb.AutoCompleteMode = AutoCompleteMode.None
        cb.AutoCompleteSource = AutoCompleteSource.ListItems
        cb.DataSource = Nothing
        cb.Items.Clear()
    End Sub

    ' ─────────────────────────────────────────────────────────────────────────────
    ' Grid
    ' ─────────────────────────────────────────────────────────────────────────────
    Private Sub EnsureGridReady()
        If DataGridView1.Columns.Count > 0 Then Exit Sub
        SetupGrid()
    End Sub

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

            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            .AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells
            .DefaultCellStyle.Padding = New Padding(0, 6, 0, 6)

            .BorderStyle = BorderStyle.None
            .CellBorderStyle = DataGridViewCellBorderStyle.None
            .ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
            .AdvancedCellBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None

            Dim colID As New DataGridViewTextBoxColumn With {.Name = "colID", .HeaderText = "ID", .Visible = False}

            Dim colQty As New DataGridViewTextBoxColumn With {
                .Name = "colQty", .HeaderText = "Quantity",
                .DefaultCellStyle = New DataGridViewCellStyle With {.Alignment = DataGridViewContentAlignment.MiddleCenter, .Format = "#,##0.##"},
                .FillWeight = 55
            }

            Dim colUnit As New DataGridViewTextBoxColumn With {.Name = "colUnit", .HeaderText = "Unit", .FillWeight = 65}
            Dim colUCost As New DataGridViewTextBoxColumn With {
                .Name = "colUCost", .HeaderText = "Unit Cost",
                .DefaultCellStyle = New DataGridViewCellStyle With {.Alignment = DataGridViewContentAlignment.MiddleCenter, .Format = "##,###.##"},
                .FillWeight = 70
            }
            Dim colTCost As New DataGridViewTextBoxColumn With {
                .Name = "colTCost", .HeaderText = "Total Cost",
                .DefaultCellStyle = New DataGridViewCellStyle With {.Alignment = DataGridViewContentAlignment.MiddleCenter, .Format = "##,###.##"},
                .FillWeight = 80
            }
            Dim colDesc As New DataGridViewTextBoxColumn With {
                .Name = "colDesc", .HeaderText = "Description",
                .DefaultCellStyle = New DataGridViewCellStyle With {.WrapMode = DataGridViewTriState.True},
                .FillWeight = 300
            }
            Dim colPNo As New DataGridViewTextBoxColumn With {.Name = "colPNo", .HeaderText = "Inventory Item No", .FillWeight = 80}
            Dim colLife As New DataGridViewTextBoxColumn With {
                .Name = "colLife", .HeaderText = "Estimated Useful Life", .FillWeight = 90, .ValueType = GetType(String)
            }

            .Columns.AddRange({colID, colQty, colUnit, colUCost, colTCost, colDesc, colPNo, colLife})

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

            .RowTemplate.Height = 0
        End With

        DataGridView1.AutoResizeRows(DataGridViewAutoSizeRowsMode.DisplayedCells)
    End Sub

    ' Public helper (variant 1)
    Public Sub AddItemRow(qty As Decimal, unit As String, descText As String,
                          inventoryNo As String, usefulLife As String,
                          unitCost As Decimal, totalCost As Decimal)
        EnsureGridReady()
        Dim i = DataGridView1.Rows.Add()
        With DataGridView1.Rows(i)
            .Cells("colID").Value = DBNull.Value
            .Cells("colQty").Value = qty
            .Cells("colUnit").Value = unit
            .Cells("colDesc").Value = descText
            .Cells("colPNo").Value = inventoryNo
            .Cells("colLife").Value = usefulLife
            .Cells("colUCost").Value = unitCost
            .Cells("colTCost").Value = totalCost
        End With
    End Sub

    ' ICS number generator (year-wide sequence, formatted with month)
    Private Async Function GenerateIcsNoAsync(d As Date) As Task(Of String)
        Dim yyyy As String = d.ToString("yyyy", Globalization.CultureInfo.InvariantCulture)
        Dim prefixForDisplay As String = d.ToString("yyyy-MM", Globalization.CultureInfo.InvariantCulture)
        Dim nextSeries As Integer = 1

        Using conn As New SqlConnection(ConnStr)
            Await conn.OpenAsync()
            Using tx = conn.BeginTransaction(IsolationLevel.Serializable)
                Try
                    Using cmd As New SqlCommand("
                        SELECT MAX(TRY_CAST(RIGHT(ics_no, 4) AS int)) AS MaxSeries
                        FROM TBL_SEP WITH (UPDLOCK, HOLDLOCK)
                        WHERE LEFT(ics_no, 4) = @yyyy;", conn, tx)
                        cmd.Parameters.Add("@yyyy", SqlDbType.VarChar, 4).Value = yyyy
                        Dim obj = Await cmd.ExecuteScalarAsync()
                        If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                            nextSeries = CInt(obj) + 1
                        End If
                    End Using
                    tx.Commit()
                Catch
                    tx.Rollback()
                    Throw
                End Try
            End Using
        End Using
        Return $"{prefixForDisplay}-{nextSeries:0000}"
    End Function

    Private Async Function EnsureEmployeeCombosLoadedAsync() As Task
        If comborby Is Nothing OrElse comboiby Is Nothing Then Return
        If _combosLoaded Then Return

        PrepEmployeeCombos()
        Dim names As List(Of String) = Await LoadEmployeeNamesAsync().ConfigureAwait(True)

        Dim rList = names.ToList()
        Dim iList = names.ToList()

        rList.Insert(0, "-- Select --")
        iList.Insert(0, "-- Select --")

        BindOneCombo(comborby, rList)
        BindOneCombo(comboiby, iList)

        _combosLoaded = True
    End Function

    ' Legacy unique check (kept)
    Private Async Function EnsureSepNoUniqueAsync(sepNo As String) As Task
        Using conn As New SqlConnection(ConnStr)
            Await conn.OpenAsync()
            Using cmd As New SqlCommand("SELECT COUNT(1) FROM TBL_SEP WITH (NOLOCK) WHERE sep_card = @p;", conn)
                cmd.Parameters.AddWithValue("@p", sepNo.Trim())
                Dim cnt = CInt(Await cmd.ExecuteScalarAsync())
                If cnt > 0 Then
                    Throw New InvalidOperationException($"SEP No. '{sepNo}' already exists.")
                End If
            End Using
        End Using
    End Function

    ' ─────────────────────────────────────────────────────────────────────────────
    ' SAVE / UPDATE
    ' ─────────────────────────────────────────────────────────────────────────────
    Private Async Sub cmdsave_Click(sender As Object, e As EventArgs) Handles cmdsave.Click
        Dim sepDate As Date = dticsdate.Value.Date
        Dim sepNo As String = txticsno.Text.Trim()

        Try
            If combofcluster.Text = "" Or comboiby.Text = "" Or comborby.Text = "" Then
                MsgBox("Fund Cluster, Received by and Issued by is required to create an ICS Entry.", vbInformation, "Message")
            Else
                If cmdsave.Text = "Save" Then
                    If DataGridView1.Rows.Count = 0 Then
                        ' must add item first
                    Else
                        If String.IsNullOrEmpty(sepNo) Then
                            sepNo = Await GenerateIcsNoAsync(sepDate)
                            Await EnsureSepNoUniqueAsync(sepNo)
                            txticsno.Text = sepNo
                        Else
                            Await EnsureSepNoUniqueAsync(sepNo)
                        End If

                        Await InsertNewSEPAsync(sepNo, sepDate)
                        cmdsave.Text = "Update"
                        cmdadd.Enabled = True
                        cmddelete.Enabled = True
                        cmdprint.Enabled = True
                        MessageBox.Show("Saved successfully.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    End If
                Else
                    Await UpdateSEPAsync(sepNo, sepDate)
                    MessageBox.Show("Updated successfully.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            End If
        Catch ex As Exception
            MessageBox.Show("Save/Update error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Async Function InsertNewSEPAsync(sepNo As String, sepDate As Date) As Task
        Using conn As New SqlConnection(ConnStr)
            Await conn.OpenAsync()
            Using tx = conn.BeginTransaction(IsolationLevel.ReadCommitted)
                Try
                    Using cmdInit As New SqlCommand("SET XACT_ABORT ON; SET NOCOUNT ON;", conn, tx)
                        cmdInit.ExecuteNonQuery()
                    End Using

                    Dim fcl As Object = DBNull.Value
                    If combofcluster IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(combofcluster.Text) Then
                        fcl = combofcluster.Text.Trim()
                    End If

                    Dim rName As String = "", rPos As String = ""
                    SplitNamePosition(If(comborby Is Nothing, "", comborby.Text), rName, rPos)

                    Dim iName As String = "", dummy As String = ""
                    SplitNamePosition(If(comboiby Is Nothing, "", comboiby.Text), iName, dummy)

                    For Each row As DataGridViewRow In DataGridView1.Rows
                        If row.IsNewRow Then Continue For

                        Dim qty As Decimal = ToDecimalSafe(row.Cells("colQty").Value)
                        Dim unit As String = CStr(If(row.Cells("colUnit").Value, "")).Trim()
                        Dim descText As String = CStr(If(row.Cells("colDesc").Value, "")).Trim()
                        Dim pno As String = CStr(If(row.Cells("colPNo").Value, "")).Trim()
                        Dim life As String = CStr(If(row.Cells("colLife").Value, "")).Trim()
                        Dim ucost As Decimal = ToDecimalSafe(row.Cells("colUCost").Value)
                        Dim tcost As Decimal = ToDecimalSafe(row.Cells("colTCost").Value)
                        If tcost = 0D Then tcost = qty * ucost

                        Dim newId As Integer
                        Using cmd As New SqlCommand("
                            INSERT INTO TBL_SEP
                                (ics_no, ics_date,
                                 descriptions, property_number, life_span,
                                 qty, unit, unit_cost, total_cost,
                                 fcluster, par_mr_to, position, prepared_by)
                            VALUES
                                (@icsno, @icsdate,
                                 @desc, @pno, @life,
                                 @qty, @unit, @ucost, @tcost,
                                 @fcl, @parmrto, @position, @prepared_by);
                            SELECT CAST(SCOPE_IDENTITY() AS int);", conn, tx)

                            cmd.Parameters.Add("@icsno", SqlDbType.VarChar, 50).Value = sepNo
                            cmd.Parameters.Add("@icsdate", SqlDbType.Date).Value = sepDate

                            cmd.Parameters.Add("@desc", SqlDbType.VarChar, 3500).Value =
                                If(String.IsNullOrWhiteSpace(descText), DBNull.Value, DirectCast(descText, Object))
                            cmd.Parameters.Add("@pno", SqlDbType.VarChar, 100).Value =
                                If(String.IsNullOrWhiteSpace(pno), DBNull.Value, DirectCast(pno, Object))
                            cmd.Parameters.Add("@life", SqlDbType.VarChar, 100).Value =
                                If(String.IsNullOrWhiteSpace(life), DBNull.Value, DirectCast(life, Object))

                            cmd.Parameters.Add("@qty", SqlDbType.Decimal).Value = qty
                            cmd.Parameters("@qty").Precision = 18 : cmd.Parameters("@qty").Scale = 2

                            cmd.Parameters.Add("@unit", SqlDbType.VarChar, 50).Value =
                                If(String.IsNullOrWhiteSpace(unit), DBNull.Value, DirectCast(unit, Object))

                            cmd.Parameters.Add("@ucost", SqlDbType.Decimal).Value = ucost
                            cmd.Parameters("@ucost").Precision = 18 : cmd.Parameters("@ucost").Scale = 2

                            cmd.Parameters.Add("@tcost", SqlDbType.Decimal).Value = tcost
                            cmd.Parameters("@tcost").Precision = 18 : cmd.Parameters("@tcost").Scale = 2

                            cmd.Parameters.Add("@fcl", SqlDbType.NVarChar, 150).Value =
                                If(TypeOf fcl Is DBNull, DBNull.Value, DirectCast(fcl, Object))

                            cmd.Parameters.Add("@parmrto", SqlDbType.VarChar, 150).Value =
                                If(String.IsNullOrWhiteSpace(rName), DBNull.Value, DirectCast(rName, Object))
                            cmd.Parameters.Add("@position", SqlDbType.VarChar, 100).Value =
                                If(String.IsNullOrWhiteSpace(rPos), DBNull.Value, DirectCast(rPos, Object))
                            cmd.Parameters.Add("@prepared_by", SqlDbType.VarChar, 150).Value =
                                If(String.IsNullOrWhiteSpace(iName), DBNull.Value, DirectCast(iName, Object))

                            newId = CInt(Await cmd.ExecuteScalarAsync())
                        End Using

                        row.Cells("colID").Value = newId

                        Using cmdInv As New SqlCommand("
                            UPDATE TBL_INVENTORY
                               SET QTY = QTY - @qty
                             WHERE (IPNO = @pno) OR (IPNO IS NULL AND DESCRIPTIONS = @desc);", conn, tx)
                            cmdInv.Parameters.Add("@qty", SqlDbType.Decimal).Value = qty
                            cmdInv.Parameters("@qty").Precision = 18 : cmdInv.Parameters("@qty").Scale = 2

                            cmdInv.Parameters.Add("@pno", SqlDbType.VarChar, 100).Value =
                                If(String.IsNullOrWhiteSpace(pno), DBNull.Value, DirectCast(pno, Object))
                            cmdInv.Parameters.Add("@desc", SqlDbType.VarChar, 3500).Value =
                                If(String.IsNullOrWhiteSpace(descText), DBNull.Value, DirectCast(descText, Object))

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

    Private Sub SplitNamePosition(input As String, ByRef name As String, ByRef pos As String)
        name = "" : pos = ""
        If String.IsNullOrWhiteSpace(input) Then Exit Sub
        Dim s = input.Trim()
        Dim p = s.IndexOf("|"c)
        If p >= 0 Then
            name = s.Substring(0, p).Trim()
            pos = s.Substring(p + 1).Trim()
        Else
            name = s
        End If
    End Sub

    Private Async Function UpdateSEPAsync(sepNo As String, sepDate As Date) As Task
        Using conn As New SqlConnection(ConnStr)
            Await conn.OpenAsync()
            Using tx = conn.BeginTransaction(IsolationLevel.ReadCommitted)
                Try
                    Using cmdInit As New SqlCommand("SET XACT_ABORT ON; SET NOCOUNT ON;", conn, tx)
                        cmdInit.ExecuteNonQuery()
                    End Using

                    Dim fcl As Object = DBNull.Value
                    If TryCast(combofcluster, ComboBox) IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(combofcluster.Text) Then
                        fcl = combofcluster.Text.Trim()
                    End If

                    Dim rTuple = ExtractNameAndPositionSafe(If(If(comborby Is Nothing, "", comborby.Text), ""))
                    Dim rName As String = If(rTuple Is Nothing, "", rTuple.Item1)
                    Dim rPos As String = If(rTuple Is Nothing, "", rTuple.Item2)

                    Dim iTuple = ExtractNameAndPositionSafe(If(If(comboiby Is Nothing, "", comboiby.Text), ""))
                    Dim iName As String = If(iTuple Is Nothing, "", iTuple.Item1)

                    For Each row As DataGridViewRow In DataGridView1.Rows
                        If row.IsNewRow Then Continue For

                        Dim idObj = row.Cells("colID").Value
                        Dim qtyNew As Decimal = ToDecimalSafe(row.Cells("colQty").Value)
                        Dim unit As String = CStr(If(row.Cells("colUnit").Value, "")).Trim()
                        Dim descText As String = CStr(If(row.Cells("colDesc").Value, "")).Trim()
                        Dim pno As String = CStr(If(row.Cells("colPNo").Value, "")).Trim()
                        Dim life As String = CStr(If(row.Cells("colLife").Value, "")).Trim()
                        Dim ucost As Decimal = ToDecimalSafe(row.Cells("colUCost").Value)
                        Dim tcost As Decimal = ToDecimalSafe(row.Cells("colTCost").Value)
                        If tcost = 0D Then tcost = qtyNew * ucost

                        If idObj Is Nothing OrElse idObj Is DBNull.Value OrElse String.IsNullOrWhiteSpace(CStr(idObj)) Then
                            Dim newId As Integer
                            Using cmdIns As New SqlCommand("
                                INSERT INTO TBL_SEP
                                    (ics_no, ics_date,
                                     descriptions, property_number, life_span,
                                     qty, unit, unit_cost, total_cost,
                                     fcluster, par_mr_to, position, prepared_by)
                                VALUES
                                    (@icsno, @icsdate,
                                     @desc, @pno, @life,
                                     @qty, @unit, @ucost, @tcost,
                                     @fcl, @parmrto, @position, @prepared_by);
                                SELECT CAST(SCOPE_IDENTITY() AS int);", conn, tx)

                                cmdIns.Parameters.AddWithValue("@icsno", sepNo)
                                cmdIns.Parameters.AddWithValue("@icsdate", sepDate)
                                cmdIns.Parameters.AddWithValue("@desc", descText)
                                cmdIns.Parameters.AddWithValue("@pno", If(String.IsNullOrWhiteSpace(pno), CType(DBNull.Value, Object), pno))
                                cmdIns.Parameters.AddWithValue("@life", If(String.IsNullOrWhiteSpace(life), CType(DBNull.Value, Object), life))
                                cmdIns.Parameters.AddWithValue("@qty", qtyNew)
                                cmdIns.Parameters.AddWithValue("@unit", unit)
                                cmdIns.Parameters.AddWithValue("@ucost", ucost)
                                cmdIns.Parameters.AddWithValue("@tcost", tcost)
                                cmdIns.Parameters.AddWithValue("@fcl", fcl)
                                cmdIns.Parameters.AddWithValue("@parmrto", If(String.IsNullOrWhiteSpace(rName), CType(DBNull.Value, Object), rName))
                                cmdIns.Parameters.AddWithValue("@position", If(String.IsNullOrWhiteSpace(rPos), CType(DBNull.Value, Object), rPos))
                                cmdIns.Parameters.AddWithValue("@prepared_by", If(String.IsNullOrWhiteSpace(iName), CType(DBNull.Value, Object), iName))

                                newId = CInt(Await cmdIns.ExecuteScalarAsync())
                            End Using
                            row.Cells("colID").Value = newId

                            Using cmdInv As New SqlCommand("
                                UPDATE TBL_INVENTORY
                                   SET QTY = QTY - @qty
                                 WHERE (IPNO = @pno) OR (IPNO IS NULL AND DESCRIPTIONS = @desc);", conn, tx)
                                cmdInv.Parameters.AddWithValue("@qty", qtyNew)
                                cmdInv.Parameters.AddWithValue("@pno", pno)
                                cmdInv.Parameters.AddWithValue("@desc", descText)
                                Await cmdInv.ExecuteNonQueryAsync()
                            End Using

                        Else
                            Dim id As Integer = CInt(idObj)

                            Dim qtyOld As Decimal = 0D
                            Using cmdGet As New SqlCommand("SELECT qty FROM TBL_SEP WITH (NOLOCK) WHERE ID = @id;", conn, tx)
                                cmdGet.Parameters.AddWithValue("@id", id)
                                Dim obj = Await cmdGet.ExecuteScalarAsync()
                                If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then qtyOld = Convert.ToDecimal(obj)
                            End Using

                            Using cmdUp As New SqlCommand("
                                UPDATE TBL_SEP
                                   SET ics_no=@icsno, ics_date=@icsdate,
                                       descriptions=@desc, property_number=@pno, life_span=@life,
                                       qty=@qty, unit=@unit, unit_cost=@ucost, total_cost=@tcost,
                                       fcluster=@fcl, par_mr_to=@parmrto, position=@position, prepared_by=@prepared_by
                                 WHERE ID=@id;", conn, tx)

                                cmdUp.Parameters.AddWithValue("@icsno", sepNo)
                                cmdUp.Parameters.AddWithValue("@icsdate", sepDate)
                                cmdUp.Parameters.AddWithValue("@desc", descText)
                                cmdUp.Parameters.AddWithValue("@pno", If(String.IsNullOrWhiteSpace(pno), CType(DBNull.Value, Object), pno))
                                cmdUp.Parameters.AddWithValue("@life", If(String.IsNullOrWhiteSpace(life), CType(DBNull.Value, Object), life))
                                cmdUp.Parameters.AddWithValue("@qty", qtyNew)
                                cmdUp.Parameters.AddWithValue("@unit", unit)
                                cmdUp.Parameters.AddWithValue("@ucost", ucost)
                                cmdUp.Parameters.AddWithValue("@tcost", tcost)
                                cmdUp.Parameters.AddWithValue("@fcl", fcl)
                                cmdUp.Parameters.AddWithValue("@parmrto", If(String.IsNullOrWhiteSpace(rName), CType(DBNull.Value, Object), rName))
                                cmdUp.Parameters.AddWithValue("@position", If(String.IsNullOrWhiteSpace(rPos), CType(DBNull.Value, Object), rPos))
                                cmdUp.Parameters.AddWithValue("@prepared_by", If(String.IsNullOrWhiteSpace(iName), CType(DBNull.Value, Object), iName))
                                cmdUp.Parameters.AddWithValue("@id", id)

                                Await cmdUp.ExecuteNonQueryAsync()
                            End Using

                            Dim delta As Decimal = qtyNew - qtyOld
                            If delta <> 0D Then
                                Using cmdInv As New SqlCommand("
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

    ' ─────────────────────────────────────────────────────────────────────────────
    ' Delete selected rows (with inventory return)
    ' ─────────────────────────────────────────────────────────────────────────────
    Private Async Sub cmddelete_Click(sender As Object, e As EventArgs) Handles cmddelete.Click
        If DataGridView1.SelectedRows.Count = 0 Then Return
        If MessageBox.Show("Delete selected item(s)?", "Confirm Delete",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then Return

        Using conn As New SqlConnection(ConnStr)
            Await conn.OpenAsync()
            Using tx = conn.BeginTransaction(IsolationLevel.ReadCommitted)
                Try
                    Using cmdInit As New SqlCommand("SET XACT_ABORT ON; SET NOCOUNT ON;", conn, tx)
                        cmdInit.ExecuteNonQuery()
                    End Using

                    For Each r As DataGridViewRow In DataGridView1.SelectedRows
                        If r.IsNewRow Then Continue For

                        Dim idObj = r.Cells("colID").Value
                        Dim qty As Decimal = 0D
                        Dim pno As String = CStr(If(r.Cells("colPNo").Value, "")).Trim()
                        Dim descText As String = CStr(If(r.Cells("colDesc").Value, "")).Trim()

                        If idObj Is Nothing OrElse idObj Is DBNull.Value OrElse CStr(idObj).Trim() = "" Then
                            DataGridView1.Rows.Remove(r)
                        Else
                            Dim id As Integer = CInt(idObj)

                            Using cmdGet As New SqlCommand("SELECT qty FROM TBL_SEP WITH (NOLOCK) WHERE ID=@id;", conn, tx)
                                cmdGet.Parameters.AddWithValue("@id", id)
                                Dim obj = Await cmdGet.ExecuteScalarAsync()
                                If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then qty = Convert.ToDecimal(obj)
                            End Using

                            Using cmdDel As New SqlCommand("DELETE FROM TBL_SEP WHERE ID=@id;", conn, tx)
                                cmdDel.Parameters.AddWithValue("@id", id)
                                Await cmdDel.ExecuteNonQueryAsync()
                            End Using

                            Using cmdInv As New SqlCommand("
                                UPDATE TBL_INVENTORY
                                   SET QTY = QTY + @qty
                                 WHERE (IPNO = @pno) OR (IPNO IS NULL AND DESCRIPTIONS = @desc);", conn, tx)
                                cmdInv.Parameters.AddWithValue("@qty", qty)
                                cmdInv.Parameters.AddWithValue("@pno", pno)
                                cmdInv.Parameters.AddWithValue("@desc", descText)
                                Await cmdInv.ExecuteNonQueryAsync()
                            End Using

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

    ' ─────────────────────────────────────────────────────────────────────────────
    ' Utils
    ' ─────────────────────────────────────────────────────────────────────────────
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

    Private Sub cmdclose_Click(sender As Object, e As EventArgs) Handles cmdclose.Click
        Me.Dispose()
    End Sub

    ' ─────────────────────────────────────────────────────────────────────────────
    ' Reload items for existing ICS
    ' ─────────────────────────────────────────────────────────────────────────────
    Private Async Function ReloadSEPItemsAsync(icsNo As String) As Task
        EnsureGridReady()

        Dim dt As New DataTable()
        Using conn As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("
                SELECT ID, qty, unit, unit_cost, total_cost,
                       descriptions, property_number, life_span
                  FROM TBL_SEP WITH (NOLOCK)
                 WHERE ics_no = @p
                 ORDER BY ID;", conn)
                cmd.Parameters.Add("@p", SqlDbType.VarChar, 50).Value = icsNo
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
                    .Cells("colUCost").Value = CDec(r("unit_cost"))
                    .Cells("colTCost").Value = CDec(r("total_cost"))
                    .Cells("colDesc").Value = r("descriptions").ToString()
                    .Cells("colPNo").Value = r("property_number").ToString()
                    .Cells("colLife").Value = r("life_span").ToString()
                End With
            Next
        Finally
            DataGridView1.ResumeLayout()
        End Try
    End Function

    ' ─────────────────────────────────────────────────────────────────────────────
    ' MANAGE open
    ' ─────────────────────────────────────────────────────────────────────────────
    Public Async Function OpenForManageAsync(icsNo As String) As Task
        _isManageMode = True                    ' <— tell Load() not to rebind combos
        txticsno.Text = icsNo
        txticsno.Enabled = False : txticsno.ReadOnly = True
        dticsdate.Enabled = False
        cmdsave.Text = "Update"
        cmdprint.Enabled = True

        ' Make sure combos are loaded NOW (before ShowDialog triggers Load)
        Await EnsureEmployeeCombosLoadedAsync()

        ' Load header from DB
        Dim icsDate As Date = Date.Today
        Dim fcluster As String = ""
        Dim parTo As String = ""
        Dim pos As String = ""
        Dim preparedBy As String = ""

        Using conn As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("
                SELECT TOP 1 ics_date, fcluster, par_mr_to, position, prepared_by
                  FROM TBL_SEP WITH (NOLOCK)
                 WHERE ics_no = @p
                 ORDER BY ID;", conn)
                cmd.Parameters.Add("@p", SqlDbType.VarChar, 50).Value = icsNo
                Await conn.OpenAsync()
                Using rdr = Await cmd.ExecuteReaderAsync()
                    If Await rdr.ReadAsync() Then
                        If Not Convert.IsDBNull(rdr("ics_date")) Then icsDate = CDate(rdr("ics_date"))
                        fcluster = If(Convert.IsDBNull(rdr("fcluster")), "", rdr("fcluster").ToString())
                        parTo = If(Convert.IsDBNull(rdr("par_mr_to")), "", rdr("par_mr_to").ToString())
                        pos = If(Convert.IsDBNull(rdr("position")), "", rdr("position").ToString())
                        preparedBy = If(Convert.IsDBNull(rdr("prepared_by")), "", rdr("prepared_by").ToString())
                    End If
                End Using
            End Using
        End Using

        dticsdate.Value = icsDate
        If combofcluster IsNot Nothing Then combofcluster.Text = fcluster

        ' Select independently into each combo
        SelectReceivedBy(comborby, parTo, pos)   ' targets "Name | Position"
        SelectPreparedBy(comboiby, preparedBy)   ' targets Name-only

        ' Items
        Await ReloadSEPItemsAsync(icsNo)
        UpdateSaveButtonState()
    End Function

    ' Update Save button availability
    Private Sub UpdateSaveButtonState()
        Dim hasItems As Boolean = DataGridView1 IsNot Nothing AndAlso DataGridView1.Rows.Cast(Of DataGridViewRow)().
                              Any(Function(r) Not r.IsNewRow)

        If hasItems Then
            cmdsave.Text = "Update"
            cmdsave.Enabled = True
            cmddelete.Enabled = True
            cmdprint.Enabled = True
        Else
            cmdsave.Text = "Save"
            cmdsave.Enabled = False
            cmddelete.Enabled = False
            cmdprint.Enabled = False
        End If
    End Sub

    Private Sub GridRowsChanged(sender As Object, e As EventArgs)
        UpdateSaveButtonState()
    End Sub

    Private Sub EnsureComboHasValue(cb As ComboBox, value As String)
        If cb Is Nothing Then Return
        Dim v As String = If(value, "").Trim()
        If v = "" Then Return
        Dim exists = cb.Items.Cast(Of Object)().
            Any(Function(x) String.Equals(CStr(x), v, StringComparison.OrdinalIgnoreCase))
        If Not exists Then cb.Items.Add(v)
        cb.SelectedItem = v
    End Sub

    ' ─────────────────────────────────────────────────────────────────────────────
    ' Print / Preview (unchanged)
    ' ─────────────────────────────────────────────────────────────────────────────
    ' ─────────────────────────────────────────────────────────────────────────────
    ' Print / Preview  (ICS)  — ename comes from txtpentity
    ' ─────────────────────────────────────────────────────────────────────────────
    Private Sub cmdprint_Click(sender As Object, e As EventArgs) Handles cmdprint.Click
        If DataGridView1.Rows.Count = 0 Then
            MessageBox.Show("No items to print.", "ICS", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        ' Build the DataTable to match your RDLC dataset fields (DSICS)
        Dim dt As New DataTable("DTICS")
        dt.Columns.Add("qty", GetType(Decimal))
        dt.Columns.Add("unit", GetType(String))
        dt.Columns.Add("unit_cost", GetType(Decimal))
        dt.Columns.Add("total_cost", GetType(Decimal))
        dt.Columns.Add("descriptions", GetType(String))
        dt.Columns.Add("property_number", GetType(String))
        dt.Columns.Add("life_span", GetType(String))

        For Each r As DataGridViewRow In DataGridView1.Rows
            If r.IsNewRow Then Continue For

            Dim qty As Decimal = ToDecimalSafe(r.Cells("colQty").Value)
            Dim unit As String = If(r.Cells("colUnit").Value, "").ToString()
            Dim ucost As Decimal = ToDecimalSafe(r.Cells("colUCost").Value)
            Dim tcost As Decimal = ToDecimalSafe(r.Cells("colTCost").Value)
            If tcost = 0D Then tcost = qty * ucost
            Dim descText As String = If(r.Cells("colDesc").Value, "").ToString()
            Dim pno As String = If(r.Cells("colPNo").Value, "").ToString()
            Dim life As String = If(r.Cells("colLife").Value, "").ToString()

            dt.Rows.Add(qty, unit, ucost, tcost, descText, pno, life)
        Next

        ' Split combobox texts → parameters
        Dim rT = ExtractNameAndPositionSafe(If(If(comborby Is Nothing OrElse comborby.SelectedItem Is Nothing, "", comborby.Text), ""))
        Dim iT = ExtractNameAndPositionSafe(If(If(comboiby Is Nothing OrElse comboiby.SelectedItem Is Nothing, "", comboiby.Text), ""))

        Dim paramList As New List(Of Microsoft.Reporting.WinForms.ReportParameter) From {
        New Microsoft.Reporting.WinForms.ReportParameter("ename", If(txtpentity Is Nothing, "", txtpentity.Text.Trim())),
        New Microsoft.Reporting.WinForms.ReportParameter("fcluster", If(combofcluster Is Nothing, "", combofcluster.Text.Trim())),
        New Microsoft.Reporting.WinForms.ReportParameter("ICSNo", txticsno.Text.Trim()),
        New Microsoft.Reporting.WinForms.ReportParameter("RName", rT.Item1),
        New Microsoft.Reporting.WinForms.ReportParameter("RPosition", rT.Item2),
        New Microsoft.Reporting.WinForms.ReportParameter("IName", iT.Item1),
        New Microsoft.Reporting.WinForms.ReportParameter("IPosition", iT.Item2)
    }

        frmpprev.Dispose()

        Dim panelSubmitCtrl As Control = Nothing
        If Me.Controls.ContainsKey("panelsubmit") Then
            panelSubmitCtrl = Me.Controls("panelsubmit")
            panelSubmitCtrl.Enabled = False
        End If

        Try
            Dim rdlcPath As String = IO.Path.Combine(Application.StartupPath, "report", "rptics.rdlc")
            If Not IO.File.Exists(rdlcPath) Then
                MessageBox.Show("Report file not found: " & rdlcPath, "Report", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            With frmpprev.ReportViewer1
                .Reset()
                .ProcessingMode = Microsoft.Reporting.WinForms.ProcessingMode.Local
                .LocalReport.ReportPath = rdlcPath
                .LocalReport.DataSources.Clear()

                ' RDLC dataset name must be DSICS
                Dim rds As New Microsoft.Reporting.WinForms.ReportDataSource("DSICS", dt)
                .LocalReport.DataSources.Add(rds)

                .LocalReport.SetParameters(paramList)
                .SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout)
                .ZoomMode = Microsoft.Reporting.WinForms.ZoomMode.Percent
                .RefreshReport()
            End With

            frmpprev.panelsubmit.Visible = False
            frmpprev.ShowDialog(Me)
        Finally
            If panelSubmitCtrl IsNot Nothing Then panelSubmitCtrl.Enabled = True
        End Try
    End Sub


    Private Sub cmdadd_Click(sender As Object, e As EventArgs) Handles cmdadd.Click
        Try
            Using dlg As New frmissuanceSEPAdd(Me)
                dlg.StartPosition = FormStartPosition.CenterParent
                If dlg.ShowDialog(Me) = DialogResult.OK Then
                    ' AddItemRow is called from the child
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show("Add Item error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' Public helper (variant 2)
    Public Sub AddItemRow(qty As Decimal,
                          unit As String,
                          descText As String,
                          propNo As String,
                          unitAmount As Decimal,
                          totalCost As Decimal,
                          euLife As String)

        EnsureGridReady()

        If DataGridView1.Columns("colLife").ValueType IsNot GetType(String) Then
            DataGridView1.Columns("colLife").ValueType = GetType(String)
        End If

        Dim i = DataGridView1.Rows.Add()
        With DataGridView1.Rows(i)
            .Cells("colID").Value = DBNull.Value
            .Cells("colQty").Value = qty
            .Cells("colUnit").Value = unit
            .Cells("colDesc").Value = descText
            .Cells("colPNo").Value = propNo
            .Cells("colUCost").Value = unitAmount
            .Cells("colTCost").Value = totalCost
            .Cells("colLife").Value = euLife
        End With
    End Sub

    Private Function FindIndex(cb As ComboBox, pred As Func(Of String, Boolean)) As Integer
        For i As Integer = 0 To cb.Items.Count - 1
            Dim txt = cb.GetItemText(cb.Items(i)).Trim()
            If pred(txt) Then Return i
        Next
        Return -1
    End Function

    Private Sub InjectIntoComboSingle(cb As ComboBox, value As String)
        Dim list As New List(Of String)

        If cb.DataSource IsNot Nothing Then
            For Each obj In CType(cb.DataSource, System.Collections.IEnumerable)
                list.Add(cb.GetItemText(obj))
            Next
        Else
            For Each obj In cb.Items
                list.Add(cb.GetItemText(obj))
            Next
        End If

        Dim insertAt As Integer = If(list.Count > 0 AndAlso list(0).StartsWith("--", StringComparison.Ordinal), 1, 0)

        If Not list.Any(Function(s) String.Equals(s.Trim(), value.Trim(), StringComparison.OrdinalIgnoreCase)) Then
            list.Insert(insertAt, value)
        End If

        cb.DataSource = Nothing
        cb.Items.Clear()
        cb.DataSource = list
    End Sub

    ' RECEIVED BY: prefer exact "Name | Position"; fallback to same Name; else inject
    Private Sub SelectReceivedBy(cb As ComboBox, name As String, pos As String)
        If cb Is Nothing Then Exit Sub
        cb.SelectedIndex = 0 ' reset to placeholder

        Dim full As String = If(name, "").Trim()
        If Not String.IsNullOrWhiteSpace(pos) Then full &= " | " & pos.Trim()

        If full <> "" Then
            Dim exact = FindIndex(cb, Function(t) NormalizeText(t) = NormalizeText(full))
            If exact >= 0 Then
                cb.SelectedIndex = exact
                Exit Sub
            End If
        End If

        Dim targetName = NameOnly(name)
        If targetName <> "" Then
            Dim byName = FindIndex(cb, Function(t) NormalizeText(NameOnly(t)) = NormalizeText(targetName))
            If byName >= 0 Then
                cb.SelectedIndex = byName
                Exit Sub
            End If
        End If

        Dim injectText As String = If(String.IsNullOrWhiteSpace(pos), NameOnly(name), $"{NameOnly(name)} | {pos.Trim()}")
        If String.IsNullOrWhiteSpace(injectText) Then Exit Sub
        InjectIntoComboSingle(cb, injectText)
        cb.SelectedIndex = If(cb.Items.Count > 1 AndAlso cb.GetItemText(cb.Items(0)).StartsWith("--"), 1, 0)
    End Sub

    ' ISSUED BY: match Name-only against items that may be "Name | Position"; else inject Name-only
    Private Sub SelectPreparedBy(cb As ComboBox, preparedBy As String)
        If cb Is Nothing Then Exit Sub
        cb.SelectedIndex = 0 ' reset to placeholder

        Dim targetName As String = NameOnly(preparedBy)
        If targetName = "" Then Exit Sub

        Dim normTarget = NormalizeText(targetName)

        Dim idx = FindIndex(cb, Function(t) NormalizeText(NameOnly(t)) = normTarget)
        If idx >= 0 Then
            cb.SelectedIndex = idx
            Exit Sub
        End If

        idx = FindIndex(cb, Function(t) NormalizeText(NameOnly(t)).StartsWith(normTarget, StringComparison.Ordinal))
        If idx >= 0 Then
            cb.SelectedIndex = idx
            Exit Sub
        End If

        idx = FindIndex(cb, Function(t) NormalizeText(NameOnly(t)).Contains(normTarget))
        If idx >= 0 Then
            cb.SelectedIndex = idx
            Exit Sub
        End If

        InjectIntoComboSingle(cb, targetName)
        cb.SelectedIndex = If(cb.Items.Count > 1 AndAlso cb.GetItemText(cb.Items(0)).StartsWith("--"), 1, 0)
    End Sub

    Private Sub EnsureComboHasValueExact(cb As ComboBox, display As String)
        If cb Is Nothing Then Return
        Dim target = If(display, "").Trim()
        If target = "" Then
            If cb.Items.Count > 0 Then cb.SelectedIndex = 0
            Return
        End If

        For i As Integer = 0 To cb.Items.Count - 1
            Dim txt = cb.GetItemText(cb.Items(i)).Trim()
            If String.Equals(txt, target, StringComparison.OrdinalIgnoreCase) Then
                cb.SelectedIndex = i
                Return
            End If
        Next

        InjectIntoCombo(cb, target)
        cb.SelectedIndex = 0
    End Sub

    Private Sub InjectIntoCombo(cb As ComboBox, value As String)
        If cb.DataSource Is Nothing Then
            cb.Items.Insert(0, value)
        Else
            Dim list As New List(Of String)
            For Each obj In CType(cb.DataSource, System.Collections.IEnumerable)
                list.Add(cb.GetItemText(obj))
            Next
            list.Insert(0, value)
            cb.DataSource = Nothing
            cb.Items.Clear()
            cb.DataSource = list
        End If
    End Sub

End Class
