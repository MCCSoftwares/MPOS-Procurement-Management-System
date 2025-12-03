Imports System.IO
Imports System.Data
Imports System.Data.SqlClient
Imports Microsoft.Reporting.WinForms
Imports System.Globalization

Public Class frmrfqfile

    Private Sub frmrfqfile_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            ' ─────────────────────────────────────────────────────────────
            ' Reset / Base config
            ' ─────────────────────────────────────────────────────────────
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

                ' 🔲 No borders (match modern flat look)
                .BorderStyle = BorderStyle.None
                .CellBorderStyle = DataGridViewCellBorderStyle.None
                .ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
                .AdvancedCellBorderStyle.Left = DataGridViewAdvancedCellBorderStyle.None
                .AdvancedCellBorderStyle.Right = DataGridViewAdvancedCellBorderStyle.None
                .AdvancedCellBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.None
                .AdvancedCellBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.None
            End With

            ' ─────────────────────────────────────────────────────────────
            ' Columns
            ' ─────────────────────────────────────────────────────────────
            DataGridView1.ColumnCount = 7

            With DataGridView1
                .Columns(0).Name = "ID"
                .Columns(0).HeaderText = "ID"
                .Columns(0).DataPropertyName = "ID"
                .Columns(0).Visible = False

                .Columns(1).Name = "RFQ_NCOMPANY"
                .Columns(1).HeaderText = "COMPANY NAME"
                .Columns(1).DataPropertyName = "RFQ_NCOMPANY"
                .Columns(1).Width = 220

                .Columns(2).Name = "RFQ_CADDRESS"
                .Columns(2).HeaderText = "COMPLETE ADDRESS"
                .Columns(2).DataPropertyName = "RFQ_CADDRESS"
                .Columns(2).Width = 350
                .Columns(2).DefaultCellStyle.WrapMode = DataGridViewTriState.True

                .Columns(3).Name = "RFQ_PNAME"
                .Columns(3).HeaderText = "CERTIFIED BY"
                .Columns(3).DataPropertyName = "RFQ_PNAME"
                .Columns(3).Width = 160

                .Columns(4).Name = "RFQ_DATES"
                .Columns(4).HeaderText = "DATE CERTIFIED"
                .Columns(4).DataPropertyName = "RFQ_DATES"
                .Columns(4).DefaultCellStyle.Format = "MM/dd/yyyy"
                .Columns(4).Width = 130
                .Columns(4).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                .Columns(4).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter

                .Columns(5).Name = "RFQ_TELNO"
                .Columns(5).HeaderText = "TEL NO./FAX NO."
                .Columns(5).DataPropertyName = "RFQ_TELNO"
                .Columns(5).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                .Columns(5).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
                .Columns(5).Width = 180

                .Columns(6).Name = "RFQ_SUM"
                .Columns(6).HeaderText = "AMOUNT"
                .Columns(6).DataPropertyName = "RFQ_SUM"
                .Columns(6).DefaultCellStyle.Format = "N2"
                .Columns(6).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                .Columns(6).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight
                .Columns(6).Width = 140
            End With

            ' ─────────────────────────────────────────────────────────────
            ' Styling
            ' ─────────────────────────────────────────────────────────────
            Dim hdrColor As Color = Color.FromArgb(73, 94, 252)

            With DataGridView1
                .EnableHeadersVisualStyles = False

                ' Header
                .ColumnHeadersDefaultCellStyle.BackColor = hdrColor
                .ColumnHeadersDefaultCellStyle.ForeColor = Color.White
                .ColumnHeadersDefaultCellStyle.SelectionBackColor = hdrColor
                .ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White
                .ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 10.0!, FontStyle.Bold)
                .ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
                .ColumnHeadersHeight = 52
                .ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing

                ' Rows
                .DefaultCellStyle.Font = New Font("Segoe UI", 9.0!)
                .DefaultCellStyle.ForeColor = Color.Black
                .RowsDefaultCellStyle.BackColor = Color.White
                .AlternatingRowsDefaultCellStyle.BackColor = Color.White
                .DefaultCellStyle.SelectionBackColor = Color.FromArgb(210, 222, 255)
                .DefaultCellStyle.SelectionForeColor = Color.Black

                ' Row spacing
                .RowTemplate.Height = 34
                .RowTemplate.DefaultCellStyle.Padding = New Padding(0, 6, 0, 6)
            End With

            ' Smooth paint
            EnableDoubleBuffering(DataGridView1)

            ' ─────────────────────────────────────────────────────────────
            ' Load data
            ' ─────────────────────────────────────────────────────────────
            If lblprid.Text <> "" Then
                LoadSupplierRecords()
            End If

        Catch ex As Exception
            MessageBox.Show("Error initializing supplier grid: " & ex.Message,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' Reuse helper
    Private Sub EnableDoubleBuffering(dgv As DataGridView)
        Dim t = dgv.GetType()
        Dim pi = t.GetProperty("DoubleBuffered", Reflection.BindingFlags.Instance Or Reflection.BindingFlags.NonPublic)
        If pi IsNot Nothing Then pi.SetValue(dgv, True, Nothing)
    End Sub

    Public Sub LoadRFQData(ByVal RFQID As Integer)
        Try
            Using conn As New SqlConnection(frmmain.txtdb.Text)
                conn.Open()

                Const sql As String = "
                SELECT
                  ID,
                  RFQ_PRNO,
                  RFQ_PRID,
                  RFQ_PRDATE,
                  RFQ_PRRECEIVED,
                  RFQ_DCANVASS,
                  RFQ_PENTITY,
                  RFQ_STATUS,
                  RFQ_PURPOSE,
                  RFQ_OEUSER,
                  RFQ_FUNDS,
                  RFQ_MPROC,
                  RFQ_QSDATE,
                  RFQ_TTYPE,
                  RFQ_CANBY
                FROM TBL_RFQ
                WHERE ID = @ID;
                "
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@ID", RFQID)
                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        If reader.Read() Then
                            ' — RFQ’s own PK —
                            lblid.Text = reader("ID").ToString()
                            ' — Original PR’s ID —
                            lblprid.Text = reader("RFQ_PRID").ToString()
                            ' — Header fields —
                            txtprno.Text = reader("RFQ_PRNO").ToString()

                            ' Dates from DB (already DateTime) → format for UI textbox
                            Dim dPR = AppDate.SafeGetDate(reader, "RFQ_PRDATE")
                            Dim dRec = AppDate.SafeGetDate(reader, "RFQ_PRRECEIVED")
                            Dim dCan = AppDate.SafeGetDate(reader, "RFQ_DCANVASS")
                            Dim dQS = AppDate.SafeGetDate(reader, "RFQ_QSDATE")

                            txtprdate.Text = AppDate.ToUiDate(dPR)
                            dtrecieved.Value = AppDate.ToUiDate(dRec)
                            dtdcanvass.Value = If(dCan.HasValue, dCan.Value, Date.Today)
                            dtsdate.Value = If(dQS.HasValue, dQS.Value, Date.Today)

                            txtpentity.Text = reader("RFQ_PENTITY").ToString()
                            txtstatus.Text = reader("RFQ_STATUS").ToString()
                            txtpurpose.Text = reader("RFQ_PURPOSE").ToString()
                            txtoeuser.Text = reader("RFQ_OEUSER").ToString()
                            txtfund.Text = reader("RFQ_FUNDS").ToString()
                            combomproc.Text = reader("RFQ_MPROC").ToString()
                            txtcanby.Text = reader("RFQ_CANBY").ToString()
                            LBLTTYPE.Text = reader("RFQ_TTYPE").ToString()
                            lbltitle.Text = "REQUEST FOR QUOTATION | PR #: " & txtprno.Text
                        Else
                            MessageBox.Show("RFQ record not found.", "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        End If
                    End Using
                End Using
            End Using

            If txtstatus.Text = "Processing" Then
                Me.cmdaddsup.Enabled = True
                Me.cmddelete.Enabled = True
                Me.cmdsave.Enabled = True
                Me.cmdsubmit.Enabled = True
                Me.cmdscancel.Enabled = False
                Me.dtrecieved.Enabled = False
            ElseIf txtstatus.Text = "Completed" Then
                Me.cmdaddsup.Enabled = False
                Me.cmddelete.Enabled = False
                Me.cmdsave.Enabled = False
                Me.cmdsubmit.Enabled = False
                Me.cmdscancel.Enabled = True
                Me.dtrecieved.Enabled = False
            Else
                Me.cmdaddsup.Enabled = False
                Me.cmddelete.Enabled = False
                Me.cmdsave.Enabled = False
                Me.cmdsubmit.Visible = False
                Me.cmdscancel.Visible = False
                Me.dtrecieved.Enabled = False
            End If
        Catch ex As Exception
            MessageBox.Show("Failed to load RFQ data: " & ex.Message,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Public Sub LoadSupplierRecords()
        ' 1) Parse the RFQ header ID
        Dim rfqId As Integer
        If Not Integer.TryParse(lblprid.Text, rfqId) Then
            Return
        End If

        Try
            Using conn As New SqlConnection(frmmain.txtdb.Text)
                conn.Open()

                ' 2) Include ID in the query
                Const sql As String = "
                SELECT
                  ID,
                  RFQ_NCOMPANY   AS [RFQ_NCOMPANY],
                  RFQ_CADDRESS   AS [RFQ_CADDRESS],
                  RFQ_PNAME      AS [RFQ_PNAME],
                  RFQ_DATES      AS [RFQ_DATES],
                  RFQ_TELNO      AS [RFQ_TELNO],
                  RFQ_SUM        AS [RFQ_SUM]
                FROM TBL_RFQSUPPLIER
                WHERE RFQ_PRID = @RFQID
                ORDER BY ID;
                "

                Using da As New SqlDataAdapter(sql, conn)
                    da.SelectCommand.Parameters.AddWithValue("@RFQID", rfqId)
                    Dim dt As New DataTable()
                    da.Fill(dt)

                    ' 3) Bind to your pre-formatted grid
                    DataGridView1.AutoGenerateColumns = False
                    DataGridView1.DataSource = dt
                End Using
            End Using

        Catch ex As Exception
            MessageBox.Show("Failed to load suppliers: " & ex.Message,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub cmdaddsup_Click(sender As Object, e As EventArgs) Handles cmdaddsup.Click
        Try
            Dim position As String = My.Forms.frmmain.lblaposition.Text
            If position = "End User" Then
                MsgBox("Unauthorized: your access level does not allow this action.", vbCritical, "Message")
            Else
                frmrfqsupplier.Dispose()
                frmrfqsupplier.cmdedit.Enabled = False
                frmrfqsupplier.cmdprint.Enabled = False
                frmrfqsupplier.cmdsave.Text = "Save"
                frmrfqsupplier.ShowDialog()
            End If
        Catch ex As Exception
            MessageBox.Show("Error opening supplier form: " & ex.Message,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub txtprno_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtprno.KeyPress
        e.Handled = True
    End Sub

    Private Sub txtprdate_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtprdate.KeyPress
        e.Handled = True
    End Sub

    Private Sub txtstatus_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtstatus.KeyPress
        e.Handled = True
    End Sub

    Private Sub txtfund_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtfund.KeyPress
        e.Handled = True
    End Sub

    Public Sub SaveRFQ()
        Try
            ' 1) Validation
            If String.IsNullOrWhiteSpace(txtprno.Text) Then
                MessageBox.Show("PR No. is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtprno.Focus() : Return
            End If
            If String.IsNullOrWhiteSpace(lblprid.Text) Then
                MessageBox.Show("PR ID is missing.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            ' 2) Gather values
            Dim rfqDate As DateTime = dtrecieved.Value
            Dim prNo As String = txtprno.Text.Trim()
            Dim prId As Integer = Convert.ToInt32(lblprid.Text)

            ' Parse user-entered dates with flexible parser (handles MM/dd and dd/MM)
            Dim prDate As DateTime = AppDate.ParseDateFlexible(txtprdate.Text)
            Dim prReceived As DateTime = AppDate.ParseDateFlexible(dtrecieved.Value)

            Dim dCanvass As DateTime = dtdcanvass.Value
            Dim pEntity As String = txtpentity.Text.Trim()
            Dim status As String = txtstatus.Text.Trim()
            Dim purpose As String = txtpurpose.Text.Trim()
            Dim oeUser As String = txtoeuser.Text.Trim()
            Dim funds As String = txtfund.Text.Trim()
            Dim mProc As String = combomproc.Text.Trim()
            Dim qsDate As DateTime = dtsdate.Value
            Dim ttype As String = LBLTTYPE.Text.Trim()
            Dim canBy As String

            If txtcanby.Text = "" Then
                canBy = frmmain.lblaname.Text.Trim()
            Else
                canBy = txtcanby.Text
            End If


            ' 3) Insert into TBL_RFQ, then update PRSTATUS in TBL_PR
            Try
                Using conn As New SqlConnection(frmmain.txtdb.Text)
                    conn.Open()

                    Const insertSql As String = "
INSERT INTO TBL_RFQ
  (RFQ_DATE, RFQ_PRNO, RFQ_PRID, RFQ_PRDATE,
   RFQ_PRRECEIVED, RFQ_DCANVASS, RFQ_PENTITY, RFQ_STATUS,
   RFQ_PURPOSE, RFQ_OEUSER, RFQ_FUNDS, RFQ_MPROC,
   RFQ_QSDATE, RFQ_CANBY, RFQ_TTYPE)
VALUES
  (@Date, @PRNO, @PRID, @PRDATE,
   @RECEIVED, @DCANVASS, @PENTITY, @STATUS,
   @PURPOSE, @OEUSER, @FUNDS, @MPROC,
   @QSDATE, @CANBY, @TTYPE);
SELECT CAST(SCOPE_IDENTITY() AS INT);"

                    Dim newId As Integer
                    Using cmd As New SqlCommand(insertSql, conn)
                        With cmd.Parameters
                            .AddWithValue("@Date", rfqDate)
                            .AddWithValue("@PRNO", prNo)
                            .AddWithValue("@PRID", prId)
                            .AddWithValue("@PRDATE", prDate)
                            .AddWithValue("@RECEIVED", prReceived)
                            .AddWithValue("@DCANVASS", dCanvass)
                            .AddWithValue("@PENTITY", pEntity)
                            .AddWithValue("@STATUS", status)
                            .AddWithValue("@PURPOSE", purpose)
                            .AddWithValue("@OEUSER", oeUser)
                            .AddWithValue("@FUNDS", funds)
                            .AddWithValue("@MPROC", mProc)
                            .AddWithValue("@QSDATE", qsDate)
                            .AddWithValue("@CANBY", canBy)
                            .AddWithValue("@TTYPE", ttype)
                        End With

                        newId = Convert.ToInt32(cmd.ExecuteScalar())
                        lblid.Text = newId.ToString()
                    End Using

                    Const updatePrSql As String = "
                        UPDATE TBL_PR
                           SET PRSTATUS = '[RFQ] Processing'
                         WHERE ID = @prid;"
                    Using cmd2 As New SqlCommand(updatePrSql, conn)
                        cmd2.Parameters.AddWithValue("@prid", prId)
                        cmd2.ExecuteNonQuery()
                    End Using
                End Using

                ' 4) Enable commands now that we're saved
                cmdaddsup.Enabled = True
                cmddelete.Enabled = True
                cmdedit.Enabled = True
                cmdsave.Text = "Update"
                cmdsubmit.Enabled = True
                cmdscancel.Enabled = True
                cmdprint.Enabled = True

                ' 5) Refresh list in the RFQ form
                frmrfq.LoadRecords()

                NotificationHelper.AddNotification(
                    $"Request for Quotation for PR Number: {txtprno.Text.Trim()} has been created by {frmmain.lblaname.Text.Trim()} and is now in process.")

                MessageBox.Show("RFQ saved and PR status updated to [RFQ] Processing.",
                                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

            Catch ex As Exception
                MessageBox.Show("Error saving RFQ: " & ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try

        Catch ex As Exception
            MessageBox.Show("Error saving RFQ: " & ex.Message,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Public Sub UpdateRFQ()
        ' 1) Validation
        If String.IsNullOrWhiteSpace(lblid.Text) OrElse Not Integer.TryParse(lblid.Text, Nothing) Then
            MessageBox.Show("RFQ ID is missing or invalid.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        If String.IsNullOrWhiteSpace(txtprno.Text) Then
            MessageBox.Show("PR No. is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtprno.Focus() : Return
        End If

        ' 2) Gather values
        Dim recordId As Integer = Convert.ToInt32(lblid.Text)
        Dim received As DateTime = AppDate.ParseDateFlexible(dtrecieved.Value)
        Dim dCanvass As DateTime = dtdcanvass.Value
        Dim entity As String = txtpentity.Text.Trim()
        Dim status As String = txtstatus.Text.Trim()
        Dim purpose As String = txtpurpose.Text.Trim()
        Dim oeUser As String = txtoeuser.Text.Trim()
        Dim funds As String = txtfund.Text.Trim()
        Dim mProc As String = combomproc.Text.Trim()
        Dim qsDate As DateTime = dtsdate.Value
        Dim canBy As String = txtcanby.Text

        ' 3) Perform UPDATE
        Try
            Using conn As New SqlConnection(frmmain.txtdb.Text)
                conn.Open()
                Const sql As String = "
UPDATE TBL_RFQ
   SET RFQ_PRRECEIVED  = @RECEIVED,
       RFQ_DCANVASS    = @DCANVASS,
       RFQ_PENTITY     = @PENTITY,
       RFQ_STATUS      = @STATUS,
       RFQ_PURPOSE     = @PURPOSE,
       RFQ_OEUSER      = @OEUSER,
       RFQ_FUNDS       = @FUNDS,
       RFQ_MPROC       = @MPROC,
       RFQ_QSDATE      = @QSDATE,
       RFQ_CANBY       = @CANBY
 WHERE ID = @ID;"
                Using cmd As New SqlCommand(sql, conn)
                    With cmd.Parameters
                        .AddWithValue("@RECEIVED", received)
                        .AddWithValue("@DCANVASS", dCanvass)
                        .AddWithValue("@PENTITY", entity)
                        .AddWithValue("@STATUS", status)
                        .AddWithValue("@PURPOSE", purpose)
                        .AddWithValue("@OEUSER", oeUser)
                        .AddWithValue("@FUNDS", funds)
                        .AddWithValue("@MPROC", mProc)
                        .AddWithValue("@QSDATE", qsDate)
                        .AddWithValue("@CANBY", canBy)
                        .AddWithValue("@ID", recordId)
                    End With
                    Dim affected = cmd.ExecuteNonQuery()
                    If affected = 0 Then
                        MessageBox.Show("No record was updated. Please check the RFQ ID.", "Update Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return
                    End If
                End Using
            End Using

            frmrfq.LoadRecords()
            MessageBox.Show("RFQ updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show("Error updating RFQ: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub cmdsave_Click(sender As Object, e As EventArgs) Handles cmdsave.Click
        Try
            If String.Equals(cmdsave.Text, "Save", StringComparison.OrdinalIgnoreCase) Then
                SaveRFQ()
            ElseIf String.Equals(cmdsave.Text, "Update", StringComparison.OrdinalIgnoreCase) Then
                UpdateRFQ()
            End If
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message, "Save/Update", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub cmdedit_Click(sender As Object, e As EventArgs) Handles cmdedit.Click
        Try
            Dim position As String = My.Forms.frmmain.lblaposition.Text
            If position = "End User" Then
                MsgBox("Unauthorized: your access level does not allow this action.", vbCritical, "Message")
            Else
                Try
                    frmrfqsupplier.Dispose()

                    ' 1) Ensure a supplier row is selected
                    If DataGridView1.SelectedRows.Count = 0 Then
                        MessageBox.Show("Please select a supplier to edit.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return
                    End If

                    ' 2) Read the hidden supplier‐PK
                    Dim rawId = DataGridView1.SelectedRows(0).Cells("ID").Value
                    Dim supplierId As Integer
                    If rawId Is Nothing OrElse Not Integer.TryParse(rawId.ToString(), supplierId) Then
                        MessageBox.Show("Invalid Supplier ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return
                    End If

                    ' 3) Launch the supplier form in edit mode
                    Using supForm As New frmrfqsupplier()
                        If Me.txtstatus.Text = "Processing" Then
                            supForm.cmdedit.Enabled = True
                            supForm.cmdsave.Enabled = True
                        Else
                            supForm.cmdedit.Enabled = False
                            supForm.cmdsave.Enabled = False
                        End If

                        supForm.LoadSupplier(supplierId)
                        supForm.LoadRFQItems()
                        supForm.ShowDialog()
                    End Using

                    ' 4) Refresh your grid after the dialog closes
                    LoadSupplierRecords()
                    frmrfqsupplier.LoadRFQItems()
                Catch ex As Exception
                    MsgBox(ex.ToString())
                End Try
            End If
        Catch ex As Exception
            MsgBox(ex.ToString())
        End Try
    End Sub

    Private Sub cmddelete_Click(sender As Object, e As EventArgs) Handles cmddelete.Click
        Dim position As String = My.Forms.frmmain.lblaposition.Text
        If position = "End User" Then
            MsgBox("Unauthorized: your access level does not allow this action.", vbCritical, "Message")
            Return
        End If

        ' 1) Ensure at least one row
        If DataGridView1.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select one or more supplier(s) to delete.", "No Selection",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' 2) Collect supplier IDs & names
        Dim ids As New List(Of Integer)()
        Dim names As New List(Of String)()
        For Each row As DataGridViewRow In DataGridView1.SelectedRows
            Dim rawId = row.Cells("ID").Value
            Dim rawName = row.Cells("RFQ_NCOMPANY").Value
            Dim idVal As Integer
            If rawId IsNot Nothing AndAlso Integer.TryParse(rawId.ToString(), idVal) Then
                ids.Add(idVal)
                names.Add(rawName?.ToString())
            End If
        Next

        If ids.Count = 0 Then
            MessageBox.Show("No valid supplier rows selected.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        ' 3) Confirm
        Dim msg = $"Are you sure you want to delete the following supplier(s) and all their quotes?{vbCrLf}" &
                  String.Join(vbCrLf, names)
        If MessageBox.Show(msg, "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
            Return
        End If

        ' 4) Delete supplier + items in a transaction
        Try
            Using conn As New SqlConnection(frmmain.txtdb.Text)
                conn.Open()
                Using tran = conn.BeginTransaction()
                    Using cmd As New SqlCommand("", conn, tran)
                        cmd.Parameters.Add("@ID", SqlDbType.Int)

                        For Each idVal In ids
                            cmd.CommandText = "DELETE FROM TBL_RFQITEMS WHERE RFQSID = @ID"
                            cmd.Parameters("@ID").Value = idVal
                            cmd.ExecuteNonQuery()

                            cmd.CommandText = "DELETE FROM TBL_RFQSUPPLIER WHERE ID = @ID"
                            cmd.ExecuteNonQuery()
                        Next
                    End Using
                    tran.Commit()
                End Using
            End Using

            LoadSupplierRecords()
            MessageBox.Show("Selected supplier(s) and their quote‐items deleted.", "Deleted",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show("Error deleting supplier(s): " & ex.Message, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub cmdprint_Click(sender As Object, e As EventArgs) Handles cmdprint.Click
        Try
            ' 1) Validate RFQ header ID
            Dim prId As Integer
            If Not Integer.TryParse(lblprid.Text, prId) Then
                MessageBox.Show("Invalid RFQ header ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            ' 2) Load table data for RDLC
            Dim dt As New DataTable()
            Using conn As New SqlConnection(frmmain.txtdb.Text)
                conn.Open()
                Const sql As String = "
                SELECT
                  PR_ITEMNO,
                  PR_UNIT,
                  PR_QTY,
                  PR_DESC
                FROM TBL_PRITEMS
                WHERE PRID = @PRID
                ORDER BY PR_ITEMNO;
                "
                Using da As New SqlDataAdapter(sql, conn)
                    da.SelectCommand.Parameters.AddWithValue("@PRID", prId)
                    da.Fill(dt)
                End Using
            End Using

            ' 3) Get BACSEC
            Dim BACSECNAME As String = ""
            Using cfgConn As New SqlConnection(frmmain.txtdb.Text)
                cfgConn.Open()
                Const cfgSql As String = "SELECT TOP 1 BACSEC FROM TBL_CONFIG;"
                Using cmdCfg As New SqlCommand(cfgSql, cfgConn)
                    Using rdr = cmdCfg.ExecuteReader()
                        If rdr.Read() Then
                            BACSECNAME = rdr("BACSEC")?.ToString().Trim()
                        End If
                    End Using
                End Using
            End Using

            ' 4) Build HTML for BACSEC (Name bold + position under it)
            Dim parts = BACSECNAME _
                .Split(New String() {vbCrLf, vbLf}, StringSplitOptions.RemoveEmptyEntries) _
                .Select(Function(s) s.Trim()) _
                .ToArray()

            Dim namePart As String = If(parts.Length > 0, parts(0), String.Empty)
            Dim positionPart As String = If(parts.Length > 1, parts(1), String.Empty)

            Dim BACSECHtml As String = If(String.IsNullOrEmpty(positionPart),
                                          $"<b>{namePart}</b>",
                                          $"<b>{namePart}</b><br/>{positionPart}")

            ' Normalize date parameters for report (robust for both locales)
            Dim prDateStr As String = ""
            Dim prRecStr As String = ""
            If Not String.IsNullOrWhiteSpace(txtprdate.Text) Then
                prDateStr = AppDate.ParseDateFlexible(txtprdate.Text).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)
            End If
            If Not String.IsNullOrWhiteSpace(dtrecieved.Value) Then
                prRecStr = AppDate.ParseDateFlexible(dtrecieved.Value).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)
            End If

            ' 5) Report parameters
            Dim rptParams = New List(Of ReportParameter) From {
                New ReportParameter("PRNO", txtprno.Text),
                New ReportParameter("PRDATE", prDateStr),
                New ReportParameter("PRRECEIVED", prRecStr),
                New ReportParameter("DCANVASS", dtdcanvass.Value.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)),
                New ReportParameter("OEUSER", txtoeuser.Text),
                New ReportParameter("MPROC", combomproc.Text),
                New ReportParameter("SUBDATE", dtsdate.Value.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)),
                New ReportParameter("REQDOCS", "-PhilGEPS Certificate" & vbNewLine & "-Omnibus Sworn Statement " & Year(Now) & vbNewLine & "-Mayor's Permit " & Year(Now) & vbNewLine & "-ITR/Tax Clearance"),
                New ReportParameter("HEADSIGN", String.Empty),
                New ReportParameter("PURPOSE", txtpurpose.Text),
                New ReportParameter("FUNDS", txtfund.Text),
                New ReportParameter("BACSEC", BACSECHtml),
                New ReportParameter("CANBY", txtcanby.Text)
            }

            ' 6) RDLC path
            Dim rdlcPath = Path.Combine(Application.StartupPath, "report", "rptrfq.rdlc")
            If Not File.Exists(rdlcPath) Then
                MessageBox.Show($"RDLC file not found: {rdlcPath}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            ' 7) Show print preview
            Using preview As New frmpprev()
                With preview.ReportViewer1
                    .ProcessingMode = ProcessingMode.Local
                    .LocalReport.ReportPath = rdlcPath
                    .LocalReport.DataSources.Clear()
                    .LocalReport.DataSources.Add(New ReportDataSource("DSPR", dt))
                    .LocalReport.SetParameters(rptParams)
                    .SetDisplayMode(DisplayMode.PrintLayout)
                    .ZoomMode = ZoomMode.Percent
                    .RefreshReport()
                End With

                preview.panelsubmit.Visible = False
                preview.ShowDialog()
            End Using

        Catch ex As Exception
            MessageBox.Show("Error printing RFQ: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub txtpurpose_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtpurpose.KeyPress
        e.Handled = True
    End Sub

    Private Sub cmdsubmit_Click(sender As Object, e As EventArgs) Handles cmdsubmit.Click
        Try
            ' 0) Normalize TTYPE
            Dim ttype As String = If(LBLTTYPE?.Text, "").Trim().ToUpperInvariant()

            ' 1) Validate PR ID
            Dim prIdValue As Integer
            If Not Integer.TryParse(lblprid.Text, prIdValue) Then
                MessageBox.Show("Invalid PR ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            If ttype = "DIRECT PAYMENT" Then
                ' ─────────────────────────────────────────────
                ' DIRECT PAYMENT: original behavior
                ' ─────────────────────────────────────────────
                Using conn As New SqlClient.SqlConnection(frmmain.txtdb.Text)
                    conn.Open()
                    Using tran = conn.BeginTransaction()
                        ' RFQ -> Completed
                        Using cmdRfQ As New SqlClient.SqlCommand("
                        UPDATE TBL_RFQ
                           SET RFQ_STATUS = 'Completed'
                         WHERE RFQ_PRID   = @prid;", conn, tran)
                            cmdRfQ.Parameters.Add("@prid", SqlDbType.Int).Value = prIdValue
                            cmdRfQ.ExecuteNonQuery()
                        End Using

                        ' PR -> [RFQ] Completed
                        Using cmdPr As New SqlClient.SqlCommand("
                        UPDATE TBL_PR
                           SET PRSTATUS = '[RFQ] Completed'
                         WHERE ID        = @prid;", conn, tran)
                            cmdPr.Parameters.Add("@prid", SqlDbType.Int).Value = prIdValue
                            cmdPr.ExecuteNonQuery()
                        End Using

                        tran.Commit()
                    End Using
                End Using

                ' Update UI
                Me.cmdaddsup.Enabled = False
                Me.cmddelete.Enabled = False
                Me.cmdsave.Enabled = False
                Me.cmdsubmit.Enabled = False
                Me.cmdscancel.Enabled = True
                Me.txtstatus.Text = "Completed"
                frmrfq.LoadRecords()

                MessageBox.Show(
                "Request for Quotation status has been updated and is now ready for the Abstract of Canvass.",
                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

                NotificationHelper.AddNotification(
                $"Request for Quotation for PR Number {txtprno.Text} has been Marked as Complete by {frmmain.lblaname.Text.Trim()}.")

                LoadSupplierRecords()

            ElseIf ttype = "CASH ADVANCE" OrElse ttype = "REIMBURSEMENT" Then
                ' ---- Find RFQ_SUM column (prefer Name, fallback to HeaderText) ----
                Dim rfqSumColIndex As Integer = -1
                For i As Integer = 0 To DataGridView1.Columns.Count - 1
                    Dim c = DataGridView1.Columns(i)
                    If String.Equals(c.Name, "RFQ_SUM", StringComparison.OrdinalIgnoreCase) _
                       OrElse String.Equals(c.HeaderText, "AMOUNT", StringComparison.OrdinalIgnoreCase) Then
                        rfqSumColIndex = i
                        Exit For
                    End If
                Next
                If rfqSumColIndex = -1 Then
                    MessageBox.Show("The grid does not contain an RFQ_SUM/AMOUNT column.", "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End If

                ' ---- Try to locate a supplier column by common names ----
                Dim suppColIndex As Integer = -1
                Dim suppNames() As String = {"SUPPLIER", "RFQ_SUPPLIER", "PO_CNAME", "RFQ_CNAME"}
                For i As Integer = 0 To DataGridView1.Columns.Count - 1
                    Dim c = DataGridView1.Columns(i)
                    For Each nm In suppNames
                        If String.Equals(c.Name, nm, StringComparison.OrdinalIgnoreCase) _
                           OrElse String.Equals(c.HeaderText, "SUPPLIER", StringComparison.OrdinalIgnoreCase) Then
                            suppColIndex = i
                            Exit For
                        End If
                    Next
                    If suppColIndex <> -1 Then Exit For
                Next

                ' ---- Scan rows to find the lowest RFQ_SUM ----
                Dim lowestRow As DataGridViewRow = Nothing
                Dim lowestVal As Decimal = Decimal.MaxValue

                For Each row As DataGridViewRow In DataGridView1.Rows
                    If row.IsNewRow Then Continue For

                    Dim raw = row.Cells(rfqSumColIndex).Value
                    If raw Is Nothing OrElse raw Is DBNull.Value Then Continue For

                    Dim val As Decimal
                    If TypeOf raw Is Decimal Then
                        val = DirectCast(raw, Decimal)
                    ElseIf Decimal.TryParse(raw.ToString(), Globalization.NumberStyles.Any, Globalization.CultureInfo.CurrentCulture, val) _
                       OrElse Decimal.TryParse(raw.ToString(), Globalization.NumberStyles.Any, Globalization.CultureInfo.InvariantCulture, val) Then
                        ' parsed OK
                    Else
                        Continue For
                    End If

                    If val < lowestVal Then
                        lowestVal = val
                        lowestRow = row
                    End If
                Next

                If lowestRow Is Nothing Then
                    MessageBox.Show("No valid numeric RFQ_SUM values found.", "Warning",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If

                ' ---- Resolve supplier text from the chosen row (fallbacks included) ----
                Dim supplierText As String = ""
                If suppColIndex >= 0 Then
                    supplierText = Convert.ToString(lowestRow.Cells(suppColIndex).FormattedValue)?.Trim()
                End If
                If String.IsNullOrEmpty(supplierText) Then
                    ' Fallback: try a visible non-RFQ_SUM text column
                    For i As Integer = 0 To DataGridView1.Columns.Count - 1
                        If i = rfqSumColIndex Then Continue For
                        Dim col = DataGridView1.Columns(i)
                        If col.Visible Then
                            supplierText = Convert.ToString(lowestRow.Cells(i).FormattedValue)?.Trim()
                            If Not String.IsNullOrEmpty(supplierText) Then Exit For
                        End If
                    Next
                End If

                ' ---- Open frmdeliveryadd prefilled with the lowest supplier ----
                Using f As New frmdeliveryadd()
                    f.lblpoid.Text = Nothing
                    f.lblprid.Text = lblprid.Text
                    f.txtpono.Text = Nothing
                    f.txtprno.Text = txtprno.Text
                    f.txtpdelivery.Text = "N/A"
                    f.lblpodate.Text = "N/A"
                    f.txtcname.Text = supplierText
                    f.txtddelivery.Text = ""
                    f.txtttype.Text = LBLTTYPE.Text
                    f.StartPosition = FormStartPosition.CenterParent
                    f.ShowDialog(Me)
                End Using

            End If


        Catch ex As Exception
            MessageBox.Show("Error submitting RFQ: " & ex.Message,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub



    Private Sub cmdscancel_Click(sender As Object, e As EventArgs) Handles cmdscancel.Click
        Try
            ' 1) Validate PR ID
            Dim prIdValue As Integer
            If Not Integer.TryParse(lblprid.Text, prIdValue) Then
                MessageBox.Show("Invalid PR ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            ' 2) Check for related abstracts
            Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
                conn.Open()

                Using cmdCheck As New SqlCommand("
SELECT COUNT(*) FROM TBL_ABSTRACT WHERE ABS_PRID = @prid;", conn)
                    cmdCheck.Parameters.Add("@prid", SqlDbType.Int).Value = prIdValue
                    Dim cnt As Integer = CInt(cmdCheck.ExecuteScalar())
                    If cnt > 0 Then
                        MessageBox.Show(
                            "Cannot cancel RFQ submission because there's an existing abstract record linked to this PR.",
                            "Cannot Cancel", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return
                    End If
                End Using

                ' 3) Revert both statuses in one transaction
                Using tran = conn.BeginTransaction()
                    Using cmdRfQ As New SqlCommand("
UPDATE TBL_RFQ
   SET RFQ_STATUS = 'Processing'
 WHERE RFQ_PRID   = @prid;", conn, tran)
                        cmdRfQ.Parameters.Add("@prid", SqlDbType.Int).Value = prIdValue
                        cmdRfQ.ExecuteNonQuery()
                    End Using

                    Using cmdPr As New SqlCommand("
UPDATE TBL_PR
   SET PRSTATUS = '[RFQ] Processing'
 WHERE ID        = @prid;", conn, tran)
                        cmdPr.Parameters.Add("@prid", SqlDbType.Int).Value = prIdValue
                        cmdPr.ExecuteNonQuery()
                    End Using

                    tran.Commit()
                End Using
            End Using

            ' 4) Re-enable controls
            cmdaddsup.Enabled = True
            cmddelete.Enabled = True
            cmdsave.Enabled = True
            cmdsubmit.Enabled = True
            cmdscancel.Enabled = False
            txtstatus.Text = "Processing"

            NotificationHelper.AddNotification(
                $"Request for Quotation for PR Number {txtprno.Text} status has been marked as Processing by {frmmain.lblaname.Text.Trim()}.")

            ' 5) Refresh UI
            frmrfq.LoadRecords()
            LoadSupplierRecords()

            MessageBox.Show("RFQ submission has been canceled. Status reverted to Pending/Processing.",
                            "Canceled", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MessageBox.Show("Error canceling RFQ submission: " & ex.Message,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub Dtdcanvass_ValueChanged(sender As Object, e As EventArgs) Handles dtdcanvass.ValueChanged
        Try
            dtsdate.Value = dtdcanvass.Value.AddDays(3)
        Catch ex As Exception

        End Try

    End Sub

    Private Sub txtoeuser_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtoeuser.KeyPress
        e.Handled = True
    End Sub

    Private Sub combomproc_KeyPress(sender As Object, e As KeyPressEventArgs) Handles combomproc.KeyPress
        e.Handled = True
    End Sub
End Class
