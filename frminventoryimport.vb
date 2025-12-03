Imports System.Data
Imports System.Data.SqlClient

Public Class frminventoryimport
    ' Passed by caller (frmdeliveryfile)
    Public Property FilterIarId As Integer = 0

    Private _dt As New DataTable()

    Private Sub frminventoryimport_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            SetupGrid()
            If FilterIarId > 0 Then
                LoadByFilter(FilterIarId)
            End If
        Catch ex As Exception
            MessageBox.Show("Form Load Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' -------- Grid setup --------
    Private Sub SetupGrid()
        With DataGridView1
            .DataSource = Nothing
            .Rows.Clear()
            .Columns.Clear()

            .AutoGenerateColumns = False
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None   ' key: let us control widths
            .AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None
            .RowHeadersVisible = False
            .AllowUserToAddRows = False
            .AllowUserToResizeRows = False
            .MultiSelect = False
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
            .ReadOnly = False                      ' we will mark specific columns as read-only
            .EditMode = DataGridViewEditMode.EditOnEnter

            .BorderStyle = BorderStyle.None
            .CellBorderStyle = DataGridViewCellBorderStyle.None
            .ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
            .AdvancedCellBorderStyle.Left = DataGridViewAdvancedCellBorderStyle.None
            .AdvancedCellBorderStyle.Right = DataGridViewAdvancedCellBorderStyle.None
            .AdvancedCellBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.None
            .AdvancedCellBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.None
        End With

        ' ----- Columns -----
        ' ID (hidden, read-only)
        Dim colID As New DataGridViewTextBoxColumn With {
            .Name = "ID", .HeaderText = "ID", .DataPropertyName = "ID",
            .Visible = False, .ReadOnly = True
        }

        ' PTYPE (editable combobox) - wide
        Dim colPTYPE As New DataGridViewComboBoxColumn With {
            .Name = "PTYPE",
            .HeaderText = "TYPE",
            .DataPropertyName = "PTYPE",
            .FlatStyle = FlatStyle.Flat,
            .DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton
        }
        colPTYPE.Items.AddRange("Semi-Expendable", "Property Plan and Equipment", "Consumables")
        colPTYPE.AutoSizeMode = DataGridViewAutoSizeColumnMode.None
        colPTYPE.Width = 220
        colPTYPE.MinimumWidth = 200
        colPTYPE.ReadOnly = False

        ' DESCRIPTIONS (very wide, fills remaining width)
        Dim colDESC As New DataGridViewTextBoxColumn With {
            .Name = "DESCRIPTIONS",
            .HeaderText = "Descriptions",
            .DataPropertyName = "DESCRIPTIONS",
            .ReadOnly = True
        }
        colDESC.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        colDESC.MinimumWidth = 300
        colDESC.FillWeight = 350

        ' ACODE (fixed)
        Dim colACODE As New DataGridViewTextBoxColumn With {
            .Name = "ACODE", .HeaderText = "Account Code", .DataPropertyName = "ACODE",
            .ReadOnly = True
        }
        colACODE.AutoSizeMode = DataGridViewAutoSizeColumnMode.None
        colACODE.Width = 110

        ' ATITLE (fixed)
        Dim colATITLE As New DataGridViewTextBoxColumn With {
            .Name = "ATITLE", .HeaderText = "Account Title", .DataPropertyName = "ATITLE",
            .ReadOnly = True
        }
        colATITLE.AutoSizeMode = DataGridViewAutoSizeColumnMode.None
        colATITLE.Width = 160

        ' ITEMNO (fixed)
        Dim colITEMNO As New DataGridViewTextBoxColumn With {
            .Name = "ITEMNO", .HeaderText = "Item/Property No.", .DataPropertyName = "ITEMNO",
            .ReadOnly = True
        }
        colITEMNO.AutoSizeMode = DataGridViewAutoSizeColumnMode.None
        colITEMNO.Width = 130

        ' UNITS (fixed)
        Dim colUNITS As New DataGridViewTextBoxColumn With {
            .Name = "UNITS", .HeaderText = "Units", .DataPropertyName = "UNITS",
            .ReadOnly = True
        }
        colUNITS.AutoSizeMode = DataGridViewAutoSizeColumnMode.None
        colUNITS.Width = 80

        ' QUANTITY (narrow)
        Dim colQTY As New DataGridViewTextBoxColumn With {
            .Name = "QUANTITY",
            .HeaderText = "Qty",
            .DataPropertyName = "QUANTITY",
            .ReadOnly = True,
            .DefaultCellStyle = New DataGridViewCellStyle With {.Alignment = DataGridViewContentAlignment.MiddleRight}
        }
        colQTY.AutoSizeMode = DataGridViewAutoSizeColumnMode.None
        colQTY.Width = 60
        colQTY.MinimumWidth = 50

        DataGridView1.Columns.AddRange(New DataGridViewColumn() {
            colID, colPTYPE, colDESC, colACODE, colATITLE, colITEMNO, colUNITS, colQTY
        })

        ' ----- Styling -----
        Dim hdrColor As Color = Color.FromArgb(73, 94, 252)
        With DataGridView1
            .EnableHeadersVisualStyles = False
            .ColumnHeadersDefaultCellStyle.BackColor = hdrColor
            .ColumnHeadersDefaultCellStyle.ForeColor = Color.White
            .ColumnHeadersDefaultCellStyle.SelectionBackColor = hdrColor
            .ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White
            .ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 11.0!, FontStyle.Bold)
            .ColumnHeadersHeight = 48
            .ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing

            .DefaultCellStyle.Font = New Font("Segoe UI", 10.0!)
            .DefaultCellStyle.ForeColor = Color.Black
            .RowsDefaultCellStyle.BackColor = Color.White
            .DefaultCellStyle.SelectionBackColor = Color.FromArgb(210, 222, 255)
            .DefaultCellStyle.SelectionForeColor = Color.Black

            .RowTemplate.Height = 34
            .RowTemplate.DefaultCellStyle.Padding = New Padding(0, 6, 0, 6)
        End With

        ' Safety: ignore combobox data mismatches; commit edits immediately
        AddHandler DataGridView1.DataError, Sub(s, e) e.ThrowException = False
        AddHandler DataGridView1.CurrentCellDirtyStateChanged, AddressOf DataGridView1_CurrentCellDirtyStateChanged
        AddHandler DataGridView1.CellValueChanged, AddressOf DataGridView1_CellValueChanged
        AddHandler DataGridView1.DataBindingComplete, AddressOf DataGridView1_DataBindingComplete
    End Sub

    ' -------- Data load w/ filter --------
    Public Sub ApplyIarFilter(iarId As Integer)
        Me.FilterIarId = iarId
        LoadByFilter(iarId)
    End Sub

    Private Sub LoadByFilter(iarId As Integer)
        Try
            _dt.Clear()
            Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
                Using da As New SqlDataAdapter("
                    SELECT ID, PTYPE, DESCRIPTIONS, ACODE, ATITLE, ITEMNO, UNITS, QUANTITY
                    FROM TBL_LOGS_IAR
                    WHERE IARID = @IARID
                    ORDER BY ID;", conn)

                    da.SelectCommand.Parameters.Add("@IARID", SqlDbType.Int).Value = iarId
                    da.Fill(_dt)
                End Using
            End Using

            ' Set data source
            DataGridView1.DataSource = _dt

            ' Reapply widths (binding can override)
            ApplyColumnSizing()

        Catch ex As Exception
            MessageBox.Show("Failed to load inventory logs: " & ex.Message, "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' -------- Ensure widths stick --------
    Private Sub ApplyColumnSizing()
        Try
            DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None
            With DataGridView1.Columns
                If .Contains("PTYPE") Then
                    .Item("PTYPE").AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                    .Item("PTYPE").Width = 220
                    .Item("PTYPE").MinimumWidth = 200
                End If

                If .Contains("DESCRIPTIONS") Then
                    .Item("DESCRIPTIONS").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                    .Item("DESCRIPTIONS").MinimumWidth = 350
                    Try
                        CType(.Item("DESCRIPTIONS"), DataGridViewTextBoxColumn).FillWeight = 350
                    Catch
                        ' ignore if cast fails due to designer overrides
                    End Try
                End If

                If .Contains("QUANTITY") Then
                    .Item("QUANTITY").AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                    .Item("QUANTITY").Width = 60
                    .Item("QUANTITY").MinimumWidth = 50
                End If
            End With
        Catch
            ' no-op
        End Try
    End Sub

    Private Sub DataGridView1_DataBindingComplete(sender As Object, e As DataGridViewBindingCompleteEventArgs)
        ' When binding finishes (or rebinding), enforce the sizing
        ApplyColumnSizing()
    End Sub

    ' -------- Instant save for PTYPE --------
    Private Sub DataGridView1_CurrentCellDirtyStateChanged(sender As Object, e As EventArgs)
        ' Commit ComboBox change immediately so CellValueChanged fires at once
        If DataGridView1.IsCurrentCellDirty Then
            DataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit)
        End If
    End Sub

    Private Sub DataGridView1_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs)
        If e.RowIndex < 0 Then Return

        Dim colName = DataGridView1.Columns(e.ColumnIndex).Name
        If colName <> "PTYPE" Then Return

        Dim idObj = DataGridView1.Rows(e.RowIndex).Cells("ID").Value
        If idObj Is Nothing OrElse idObj Is DBNull.Value Then Return

        Dim idValue As Integer
        If Not Integer.TryParse(idObj.ToString(), idValue) Then Return

        Dim newPTYPE As String = If(DataGridView1.Rows(e.RowIndex).Cells("PTYPE").Value, "").ToString()

        Try
            Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
                conn.Open()
                Using cmd As New SqlCommand("UPDATE TBL_LOGS_IAR SET PTYPE = @PTYPE WHERE ID = @ID;", conn)
                    cmd.Parameters.Add("@PTYPE", SqlDbType.NVarChar, 100).Value = If(String.IsNullOrWhiteSpace(newPTYPE), CType(DBNull.Value, Object), newPTYPE)
                    cmd.Parameters.Add("@ID", SqlDbType.Int).Value = idValue
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Failed to update PTYPE: " & ex.Message, "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub cmdImport_Click(sender As Object, e As EventArgs) Handles cmdimport.Click
        If FilterIarId <= 0 Then
            MessageBox.Show("No IAR selected to import.", "Import", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Try
            Using conn As New SqlClient.SqlConnection(My.Forms.frmmain.txtdb.Text)
                conn.Open()
                Using tran = conn.BeginTransaction(IsolationLevel.ReadCommitted)

                    ' 0) Make sure there is something to import (STATUS not already Imported)
                    Dim pendingCnt As Integer
                    Using cmdCheck As New SqlClient.SqlCommand("
                    SELECT COUNT(1)
                    FROM TBL_LOGS_IAR
                    WHERE IARID = @IARID
                      AND ([STATUS] IS NULL OR LTRIM(RTRIM([STATUS])) <> 'Imported');", conn, tran)
                        cmdCheck.Parameters.Add("@IARID", SqlDbType.Int).Value = FilterIarId
                        pendingCnt = CInt(cmdCheck.ExecuteScalar())
                    End Using
                    If pendingCnt = 0 Then
                        tran.Rollback()
                        MessageBox.Show("Nothing to import. All items are already imported.", "Import", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        Return
                    End If

                    ' 1) Close related PR(s) (TBL_PR.ID joins to PRID in logs)
                    Using cmdClosePR As New SqlClient.SqlCommand("
                    UPDATE P
                    SET P.PRSTATUS = 'Closed'
                    FROM TBL_PR AS P
                    WHERE P.ID IN (
                        SELECT DISTINCT L.PRID
                        FROM TBL_LOGS_IAR AS L
                        WHERE L.IARID = @IARID
                          AND (L.[STATUS] IS NULL OR LTRIM(RTRIM(L.[STATUS])) <> 'Imported')
                    );", conn, tran)
                        cmdClosePR.Parameters.Add("@IARID", SqlDbType.Int).Value = FilterIarId
                        cmdClosePR.ExecuteNonQuery()
                    End Using

                    ' 2) Complete IAR (TBL_IAR.ID)
                    Using cmdIAR As New SqlClient.SqlCommand("
                    UPDATE TBL_IAR
                    SET STATUS = 'Completed'
                    WHERE ID = @IARID;", conn, tran)
                        cmdIAR.Parameters.Add("@IARID", SqlDbType.Int).Value = FilterIarId
                        cmdIAR.ExecuteNonQuery()
                    End Using

                    ' 3) Set-based UPSERT into TBL_INVENTORY (decimal-safe, grouped, case/space-insensitive match on DESCRIPTIONS)
                    Using cmdUpsert As New SqlClient.SqlCommand("
                            DECLARE @agg TABLE (
                                NORMDESC     nvarchar(500) NOT NULL,
                                DESCRIPTIONS nvarchar(500) NOT NULL,
                                PTYPE        nvarchar(100) NULL,
                                ATITLE       nvarchar(200) NULL,
                                ACODE        nvarchar(100) NULL,
                                ITEMNO       nvarchar(100) NULL,
                                UNITS        nvarchar(50)  NULL,
                                QTY          decimal(18,2) NOT NULL
                            );

                            INSERT INTO @agg (NORMDESC, DESCRIPTIONS, PTYPE, ATITLE, ACODE, ITEMNO, UNITS, QTY)
                            SELECT
                                UPPER(LTRIM(RTRIM(L.DESCRIPTIONS)))                                     AS NORMDESC,
                                MAX(LTRIM(RTRIM(L.DESCRIPTIONS)))                                       AS DESCRIPTIONS,
                                MAX(L.PTYPE)                                                            AS PTYPE,
                                MAX(L.ATITLE)                                                           AS ATITLE,
                                MAX(L.ACODE)                                                            AS ACODE,
                                MAX(L.ITEMNO)                                                           AS ITEMNO,
                                MAX(L.UNITS)                                                            AS UNITS,
                                CAST(ROUND(SUM(ISNULL(TRY_CONVERT(decimal(18,2), L.QUANTITY), 0)), 2) AS decimal(18,2)) AS QTY
                            FROM TBL_LOGS_IAR AS L
                            WHERE L.IARID = @IARID
                              AND (L.[STATUS] IS NULL OR LTRIM(RTRIM(L.[STATUS])) <> 'Imported')
                            GROUP BY UPPER(LTRIM(RTRIM(L.DESCRIPTIONS)));

                            -- 3a) UPDATE existing inventory rows
                            UPDATE inv
                            SET inv.QTY = CAST(ROUND(inv.QTY + a.QTY, 2) AS decimal(18,2)),
                                inv.STATUS = CASE WHEN inv.QTY + a.QTY > 0 THEN 'In Stock' ELSE inv.STATUS END
                            FROM TBL_INVENTORY AS inv
                            JOIN @agg AS a
                              ON UPPER(LTRIM(RTRIM(inv.DESCRIPTIONS))) = a.NORMDESC;

                            -- 3b) INSERT missing inventory rows
                            INSERT INTO TBL_INVENTORY
                            (PTYPE, DESCRIPTIONS, ATITLE, ACODE, IPNO, UNITS, QTY, STATUS, REMARKS)
                            SELECT
                                a.PTYPE,
                                a.DESCRIPTIONS,
                                a.ATITLE,
                                a.ACODE,
                                a.ITEMNO,
                                a.UNITS,
                                a.QTY,
                                'In Stock',
                                ''
                            FROM @agg AS a
                            WHERE NOT EXISTS (
                                SELECT 1
                                FROM TBL_INVENTORY inv
                                WHERE UPPER(LTRIM(RTRIM(inv.DESCRIPTIONS))) = a.NORMDESC
                            );", conn, tran)
                        cmdUpsert.Parameters.Add("@IARID", SqlDbType.Int).Value = FilterIarId
                        cmdUpsert.ExecuteNonQuery()
                    End Using

                    ' 4) Mark logs as Imported (bracket + trim to be safe)
                    Using cmdMark As New SqlClient.SqlCommand("
                    UPDATE TBL_LOGS_IAR
                    SET [STATUS] = 'Imported'
                    WHERE IARID = @IARID
                      AND ([STATUS] IS NULL OR LTRIM(RTRIM([STATUS])) <> 'Imported');", conn, tran)
                        cmdMark.Parameters.Add("@IARID", SqlDbType.Int).Value = FilterIarId
                        cmdMark.ExecuteNonQuery()
                    End Using

                    ' 5) Commit all
                    tran.Commit()
                End Using
            End Using

            '/////////////////////////////////// NOTIFICATION
            NotificationHelper.AddNotification($"Supply and Equipment for IAR Number {lbliarno.Text} has been successfully imported to the Inventory by {frmmain.lblaname.Text.Trim()}, Purchase has been completed.")
            '/////////////////////////////////// NOTIFICATION

            ' Refresh grid and notify
            LoadByFilter(FilterIarId)
            cmdimport.Enabled = False
            frmdeliveryfile.cmdsubmit.Enabled = False
            frmdeliveryfile.txtstatus.Text = "Completed"
            frmdeliveryfile.cmdupdate.Enabled = False
            MessageBox.Show("Import completed successfully.", "Import", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MessageBox.Show("Import failed. No changes were saved." & Environment.NewLine & ex.Message,
                            "Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            ' Transaction rolls back automatically if not committed
        End Try
    End Sub





End Class
