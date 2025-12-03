Imports System.Data.SqlClient
Imports Microsoft.Reporting.WinForms
Imports System.IO
Imports System.Text
Public Class frmPOFile

    ' This runs when the Save button is clicked
    Private Sub cmdsave_Click(sender As Object, e As EventArgs) Handles cmdsave.Click
        If cmdsave.Text = "Save" Then
            SavePO()
        ElseIf cmdsave.Text = "Update" Then
            UpdatePO()
        End If
    End Sub

    ' Inserts a new PO, updates related statuses, and captures the new POID
    Public Sub SavePO()
        Try
            ' 1) Parse & validate inputs
            Dim poDate As DateTime
            If Not DateTime.TryParse(dtpodate.Value, poDate) Then
                MessageBox.Show("Invalid PO Date.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            Dim prId As Integer
            If Not Integer.TryParse(lblprid.Text, prId) Then
                MessageBox.Show("Invalid PR ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            Dim prNo = lblprno.Text.Trim()
            Dim fCluster = txtfcluster.Text.Trim()
            Dim purpose = txtpurpose.Text.Trim()
            Dim cname = txtncompany.Text.Trim()
            Dim caddress = txtcaddress.Text.Trim()
            Dim ctin = txtctin.Text.Trim()
            Dim nname = lblsupname.Text.Trim()
            Dim pdelivery = txtpdelivery.Text.Trim()
            Dim ddelivery = dtddelivery.Value.Date
            Dim status = txtstatus.Text.Trim()
            Dim mproc = txtmproc.Text.Trim()


            Const poEntity = "Ministry of Public Order and Safety"      ' as requested
            Const items = 0
            Const tcost = 0.0D


            ' 2) Open DB & start transaction
            Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
                conn.Open()
                Using tran = conn.BeginTransaction()

                    ' 2a) Generate the next PO number: YYYY-MM-####, series resets each new year
                    Dim yearPart = poDate.Year.ToString()
                    Dim monthPart = poDate.Month.ToString("D2")
                    Dim maxSeries As Integer

                    Using cmdSeries As New SqlCommand(
                        "SELECT MAX(CAST(RIGHT(PO_NO,4) AS INT))
                           FROM TBL_PO
                          WHERE LEFT(PO_NO,4) = @year;", conn, tran)
                        cmdSeries.Parameters.AddWithValue("@year", yearPart)
                        Dim result = cmdSeries.ExecuteScalar()
                        maxSeries = If(IsDBNull(result) OrElse result Is Nothing, 0, Convert.ToInt32(result))
                    End Using

                    Dim nextSeries = maxSeries + 1
                    Dim poNoNew = $"{yearPart}-{monthPart}-{nextSeries:0000}"

                    ' 2b) Insert and update statuses, returning the new POID
                    Dim newPoId As Integer
                    Using cmd As New SqlCommand(
                        "DECLARE @NewId INT;
                         INSERT INTO TBL_PO
                            (PO_DATE, PO_NO, PO_PRID, PO_PRNO,
                             PO_ENTITY, PO_FCLUSTER, PO_PURPOSE, PO_TIN, PO_PDELIVERY, PO_DDELIVERY,
                             PO_CNAME, PO_CADDRESS, PO_ITEMS, PO_MPROC, PO_NNAME, PO_TCOST, PO_STATUS)
                          VALUES
                            (@poDate, @poNo, @prId, @prNo,
                             @poEntity, @fCluster, @purpose, @ctin, @pdelivery, @ddelivery,
                             @cname, @caddress, @items, @mproc, @nname, @tcost, @status);
                         SET @NewId = SCOPE_IDENTITY();

                         UPDATE TBL_PR
                            SET PRSTATUS = '[P.O] Processing'
                          WHERE ID = @prId;

                         SELECT @NewId;", conn, tran)

                        cmd.Parameters.AddWithValue("@poDate", poDate)
                        cmd.Parameters.AddWithValue("@poNo", poNoNew)
                        cmd.Parameters.AddWithValue("@prId", prId)
                        cmd.Parameters.AddWithValue("@prNo", prNo)
                        cmd.Parameters.AddWithValue("@poEntity", poEntity)
                        cmd.Parameters.AddWithValue("@fCluster", fCluster)
                        cmd.Parameters.AddWithValue("@cname", cname)
                        cmd.Parameters.AddWithValue("@caddress", caddress)
                        cmd.Parameters.AddWithValue("@ctin", ctin)
                        cmd.Parameters.AddWithValue("@pdelivery", pdelivery)
                        cmd.Parameters.AddWithValue("@ddelivery", ddelivery)
                        cmd.Parameters.AddWithValue("@purpose", purpose)

                        cmd.Parameters.AddWithValue("@nname", nname)
                        cmd.Parameters.AddWithValue("@mproc", mproc)
                        cmd.Parameters.AddWithValue("@items", items)
                        cmd.Parameters.AddWithValue("@tcost", tcost)
                        cmd.Parameters.AddWithValue("@status", status)

                        newPoId = Convert.ToInt32(cmd.ExecuteScalar())
                    End Using

                    tran.Commit()

                    ' 3) Update the form with the new POID and PO number
                    lbltitle.Text = "PURCHASE ORDER | PR: " & txtpono.Text
                    lblpoid.Text = newPoId.ToString()
                    txtpono.Text = poNoNew

                    cmdsave.Text = "Update"
                    addPRItems()

                    '/////////////////////////////////// NOTIFICATION
                    NotificationHelper.AddNotification($" Purchase Order Number {poNoNew } for PR Number {prNo } has been created by {frmmain.lblaname.Text.Trim()}.")
                    '/////////////////////////////////// NOTIFICATION

                    MessageBox.Show("Purchase Order saved successfully.", "Success",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information)
                End Using
            End Using

            cmdsubmit.Enabled = True
            cmdprint.Enabled = True



        Catch ex As Exception
            MessageBox.Show("Error saving Purchase Order: " & ex.Message,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


    Public Sub UpdatePO()
        Try
            ' 1) Validate inputs
            Dim poId As Integer
            If Not Integer.TryParse(lblpoid.Text, poId) Then
                MessageBox.Show("Invalid PO ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            Dim poDate As DateTime
            If Not DateTime.TryParse(dtpodate.Value, poDate) Then
                MessageBox.Show("Invalid PO Date.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            Dim poNo = txtpono.Text.Trim()
            Dim prId As Integer = Convert.ToInt32(lblprid.Text)
            Dim prNo = lblprno.Text.Trim()
            Dim poEntity = "Ministry of Public Order and Safety"  ' if you have a control for this, replace
            Dim fCluster = txtfcluster.Text.Trim()
            Dim purpose = txtpurpose.Text.Trim()
            Dim cname = txtncompany.Text.Trim()
            Dim caddress = txtcaddress.Text.Trim()
            Dim ctin = txtctin.Text.Trim()
            Dim nname = lblsupname.Text.Trim()
            Dim pdelivery = txtpdelivery.Text.Trim()
            Dim ddelivery = dtddelivery.Value.Date
            Dim status = txtstatus.Text.Trim()
            Dim mproc = txtmproc.Text.Trim()



            ' 2) Update in a transaction
            Using conn As New SqlConnection(frmmain.txtdb.Text)
                conn.Open()
                Using tran = conn.BeginTransaction()
                    Using cmd As New SqlCommand(
                        "UPDATE TBL_PO
                            SET PO_DATE     = @poDate,
                                PO_NO       = @poNo,
                                PO_PRID     = @prId,
                                PO_PRNO     = @prNo,
                                PO_ENTITY   = @poEntity,
                                PO_FCLUSTER = @fCluster,
                                PO_PURPOSE  = @purpose,
                                PO_CNAME    = @cname,
                                PO_CADDRESS = @caddress,
                                PO_TIN      = @ctin,
                                PO_NNAME    = @nname,
                                PO_PDELIVERY= @pdelivery,
                                PO_DDELIVERY= @ddelivery,
                                PO_MPROC    = @mproc,
                                PO_STATUS   = @status
                          WHERE ID = @poId;", conn, tran)

                        cmd.Parameters.AddWithValue("@poDate", poDate)
                        cmd.Parameters.AddWithValue("@poNo", poNo)
                        cmd.Parameters.AddWithValue("@prId", prId)
                        cmd.Parameters.AddWithValue("@prNo", prNo)
                        cmd.Parameters.AddWithValue("@poEntity", poEntity)
                        cmd.Parameters.AddWithValue("@fCluster", fCluster)
                        cmd.Parameters.AddWithValue("@purpose", purpose)
                        cmd.Parameters.AddWithValue("@cname", cname)
                        cmd.Parameters.AddWithValue("@caddress", caddress)
                        cmd.Parameters.AddWithValue("@ctin", ctin)
                        cmd.Parameters.AddWithValue("@nname", nname)
                        cmd.Parameters.AddWithValue("@pdelivery", pdelivery)
                        cmd.Parameters.AddWithValue("@ddelivery", ddelivery)
                        cmd.Parameters.AddWithValue("@mproc", mproc)
                        cmd.Parameters.AddWithValue("@status", status)
                        cmd.Parameters.AddWithValue("@poId", poId)

                        cmd.ExecuteNonQuery()
                    End Using

                    tran.Commit()
                End Using
            End Using

            ' frmpo.LoadPO()

            MessageBox.Show("Purchase Order updated successfully.",
                            "Updated", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MessageBox.Show("Error updating Purchase Order: " & ex.Message,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Public Sub LoadPODetails(ByVal poId As Integer)
        Try
            'MsgBox(poId)
            Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
                conn.Open()

                ' 1) Pull the PO header
                Using cmd As New SqlCommand("
                    SELECT 
                      ID,
                      PO_DATE,
                      PO_NO,
                      PO_PRID,
                      PO_PRNO,
                      PO_ENTITY,
                      PO_FCLUSTER,
                      PO_PURPOSE,
                      PO_CNAME,
                      PO_CADDRESS,
                      PO_TIN,
                      PO_NNAME,
                      PO_TIN,
                      PO_PDELIVERY,
                      PO_DDELIVERY,
                      PO_ITEMS,
                      PO_TCOST,
                      PO_MPROC,
                      PO_STATUS
                    FROM TBL_PO
                   WHERE ID = @id;", conn)
                    cmd.Parameters.AddWithValue("@id", poId)
                    Using rdr = cmd.ExecuteReader()
                        If Not rdr.Read() Then
                            MessageBox.Show("Purchase Order not found.", "Error",
                                            MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Return
                        End If

                        ' Fill controls
                        lblpoid.Text = rdr("ID")
                        lblprid.Text = rdr("PO_PRID").ToString()
                        lblprno.Text = rdr("PO_PRNO").ToString()
                        txtpono.Text = rdr("PO_NO").ToString()
                        dtpodate.Value = Convert.ToDateTime(rdr("PO_DATE")).ToString("MM/dd/yyyy")
                        txtfcluster.Text = rdr("PO_FCLUSTER").ToString()
                        txtpurpose.Text = rdr("PO_PURPOSE").ToString()
                        txtncompany.Text = rdr("PO_CNAME").ToString()
                        txtcaddress.Text = rdr("PO_CADDRESS").ToString()
                        txtctin.Text = rdr("PO_TIN").ToString()
                        txtpdelivery.Text = rdr("PO_PDELIVERY").ToString()
                        dtddelivery.Value = Convert.ToDateTime(rdr("PO_DDELIVERY")).ToString("MM/dd/yyyy")
                        txtmproc.Text = rdr("PO_MPROC").ToString()
                        lblsupname.Text = rdr("PO_NNAME").ToString()
                        lbltotal.Text = Convert.ToDecimal(rdr("PO_TCOST")).ToString("N2")
                        txtstatus.Text = rdr("PO_STATUS").ToString()
                        lbltitle.Text = "PURCHASE ORDER | PR: " & rdr("PO_PRNO").ToString()

                        If txtstatus.Text = "Processing" Then
                            cmdsave.Enabled = True
                            cmdsubmit.Enabled = True
                            cmdscancel.Enabled = False
                        Else
                            cmdsave.Enabled = False
                            cmdsubmit.Enabled = False
                            cmdscancel.Enabled = True
                        End If
                    End Using
                End Using

                ' 2) (Optional) If you also want to refresh RFQ-based fields:
                '    e.g. txtmproc, txtcaddress, lblsupname, dtpDelivery, etc.
                '    you can re-query TBL_RFQ and TBL_RFQSUPPLIER here.

            End Using

            ' Switch button into Update mode
            cmdsave.Text = "Update"
            dtpodate.Enabled = False

        Catch ex As Exception
            MessageBox.Show("Error loading PO details: " & ex.Message,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Public Sub LoadPOItems()
        Try
            Dim poId As Integer
            If Not Integer.TryParse(lblpoid.Text, poId) Then Return
            ' 1) Read POID from the form

            If Not Integer.TryParse(lblpoid.Text, poId) Then
                MessageBox.Show("Invalid PO ID.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            ' 2) Query TBL_POITEMS
            Dim dt As New DataTable()
            Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
                conn.Open()
                Const sql As String = "
                SELECT
                    PO_ITEMNO,
                    PO_UNIT,
                    PO_DESC,
                    PO_QTY,
                    PO_UCOST,
                    PO_TCOST
                  FROM TBL_POITEMS
                 WHERE POID = @poid
                 ORDER BY PO_ITEMNO;"

                Using da As New SqlDataAdapter(sql, conn)
                    da.SelectCommand.Parameters.AddWithValue("@poid", poId)
                    da.Fill(dt)
                    DataGridView1.AutoGenerateColumns = False
                    DataGridView1.DataSource = dt
                End Using

            End Using
            'MsgBox("it should have been loaded")
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try

    End Sub


    Public Sub addPRItems()
        Try
            ' 0) Parse keys from the form
            Dim prId As Integer, poId As Integer
            If Not Integer.TryParse(lblprid.Text, prId) OrElse Not Integer.TryParse(lblpoid.Text, poId) Then
                MessageBox.Show("Invalid PRID or POID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
            Dim prNo = lblprno.Text.Trim()
            Dim poNo = txtpono.Text.Trim()

            Dim sumQty As Decimal = 0D
            Dim sumCost As Decimal = 0D

            Using conn As New SqlClient.SqlConnection(My.Forms.frmmain.txtdb.Text)
                conn.Open()
                Using tran = conn.BeginTransaction()

                    ' 1) Delete existing PO items for this PO
                    Using delCmd As New SqlClient.SqlCommand(
                    "DELETE FROM TBL_POITEMS WHERE POID = @poid;", conn, tran)
                        delCmd.Parameters.AddWithValue("@poid", poId)
                        delCmd.ExecuteNonQuery()
                    End Using

                    ' 2) Find the winning supplier-group (lowest total RFQ_TCOST)
                    Dim winningRFQSIDObj As Object
                    Using grpCmd As New SqlClient.SqlCommand(
                    "SELECT TOP 1 RFQSID
                       FROM TBL_RFQITEMS
                      WHERE PRID = @prid
                      GROUP BY RFQSID
                      ORDER BY SUM(RFQ_TCOST) ASC;", conn, tran)
                        grpCmd.Parameters.AddWithValue("@prid", prId)
                        winningRFQSIDObj = grpCmd.ExecuteScalar()
                    End Using

                    If winningRFQSIDObj Is Nothing OrElse winningRFQSIDObj Is DBNull.Value Then
                        tran.Rollback()
                        MessageBox.Show("No RFQ items found for this PR. Cannot generate PO items.", "No Data",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return
                    End If
                    Dim winningRFQSID As Integer = Convert.ToInt32(winningRFQSIDObj)

                    ' 3) Insert that group's items into TBL_POITEMS
                    Using insCmd As New SqlClient.SqlCommand(
                    "INSERT INTO TBL_POITEMS
                        (PRID, PRNO, POID, PONO,
                         PO_ITEMNO, PO_UNIT, PO_DESC,
                         PO_QTY, PO_UCOST, PO_TCOST)
                     SELECT 
                        I.PRID,
                        @prno,
                        @poid,
                        @pono,
                        I.RFQ_ITEMNO,
                        I.RFQ_UNIT,
                        I.RFQ_DESC,
                        I.RFQ_QTY,
                        I.RFQ_UCOST,
                        I.RFQ_TCOST
                       FROM TBL_RFQITEMS AS I
                      WHERE I.PRID   = @prid
                        AND I.RFQSID = @rfqsid;", conn, tran)
                        insCmd.Parameters.AddWithValue("@prid", prId)
                        insCmd.Parameters.AddWithValue("@rfqsid", winningRFQSID)
                        insCmd.Parameters.AddWithValue("@prno", prNo)
                        insCmd.Parameters.AddWithValue("@poid", poId)
                        insCmd.Parameters.AddWithValue("@pono", poNo)
                        insCmd.ExecuteNonQuery()
                    End Using

                    ' 4) Recompute totals from the newly inserted POITEMS
                    Using sumCmd As New SqlClient.SqlCommand(
                    "SELECT SUM(PO_QTY) AS TQty, SUM(PO_TCOST) AS TCost
                       FROM TBL_POITEMS 
                      WHERE POID = @poid;", conn, tran)
                        sumCmd.Parameters.AddWithValue("@poid", poId)
                        Using rdr = sumCmd.ExecuteReader()
                            If rdr.Read() Then
                                sumQty = If(rdr.IsDBNull(0), 0D, Convert.ToDecimal(rdr(0)))
                                sumCost = If(rdr.IsDBNull(1), 0D, Convert.ToDecimal(rdr(1)))
                            End If
                        End Using
                    End Using

                    ' 5) Update the PO totals in TBL_PO
                    Using updCmd As New SqlClient.SqlCommand(
                    "UPDATE TBL_PO
                        SET PO_ITEMS = @items,
                            PO_TCOST = @tcost
                      WHERE ID       = @poid;", conn, tran)
                        updCmd.Parameters.AddWithValue("@items", sumQty)
                        updCmd.Parameters.AddWithValue("@tcost", sumCost)
                        updCmd.Parameters.AddWithValue("@poid", poId)
                        updCmd.ExecuteNonQuery()
                    End Using

                    tran.Commit()
                End Using
            End Using

            ' 6) Update the UI
            lbltotal.Text = sumCost.ToString("N2")   ' ← put summed PO_TCOST on the label
            LoadPOItems()

            MessageBox.Show("PO items generated from lowest-quote supplier.", "Done",
                        MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MessageBox.Show("Error adding PR items: " & ex.ToString, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub



    Private Sub FrmPOFile_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            ' ───────────────────────────────────────────────
            ' Reset / base config
            ' ───────────────────────────────────────────────
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

                ' 🚫 Flat, borderless look (same as other grids)
                .BorderStyle = BorderStyle.None
                .CellBorderStyle = DataGridViewCellBorderStyle.None
                .ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
                .AdvancedCellBorderStyle.Left = DataGridViewAdvancedCellBorderStyle.None
                .AdvancedCellBorderStyle.Right = DataGridViewAdvancedCellBorderStyle.None
                .AdvancedCellBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.None
                .AdvancedCellBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.None
            End With

            ' ───────────────────────────────────────────────
            ' Define columns for PO items
            ' ───────────────────────────────────────────────
            DataGridView1.ColumnCount = 6
            With DataGridView1
                .Columns(0).Name = "PO_ITEMNO"
                .Columns(0).HeaderText = "Item No"
                .Columns(0).DataPropertyName = "PO_ITEMNO"

                .Columns(1).Name = "PO_UNIT"
                .Columns(1).HeaderText = "Unit"
                .Columns(1).DataPropertyName = "PO_UNIT"

                .Columns(2).Name = "PO_DESC"
                .Columns(2).HeaderText = "Description"
                .Columns(2).DataPropertyName = "PO_DESC"
                .Columns(2).DefaultCellStyle.WrapMode = DataGridViewTriState.True

                .Columns(3).Name = "PO_QTY"
                .Columns(3).HeaderText = "Quantity"
                .Columns(3).DataPropertyName = "PO_QTY"
                .Columns(3).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

                .Columns(4).Name = "PO_UCOST"
                .Columns(4).HeaderText = "Unit Cost"
                .Columns(4).DataPropertyName = "PO_UCOST"
                .Columns(4).DefaultCellStyle.Format = "N2"
                .Columns(4).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight

                .Columns(5).Name = "PO_TCOST"
                .Columns(5).HeaderText = "AMOUNT"
                .Columns(5).DataPropertyName = "PO_TCOST"
                .Columns(5).DefaultCellStyle.Format = "N2"
                .Columns(5).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            End With

            ' ───────────────────────────────────────────────
            ' Styling (headers, rows, fonts, colors)
            ' ───────────────────────────────────────────────
            ' ───────────────────────────────────────────────
            ' Styling  (match your new design)
            ' ───────────────────────────────────────────────
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

            ' ───────────────────────────────────────────────
            ' Finally, load and bind data
            ' ───────────────────────────────────────────────
            'LoadPOItems()
            If lblpoid.Text <> "" Then
                LoadPOItems()
            End If

        Catch ex As Exception
            MessageBox.Show("Error initializing items grid: " & ex.Message,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub cmdprint_Click(sender As Object, e As EventArgs) Handles cmdprint.Click
        Try
            ' ─────────────────────────────────────────────────────────────
            ' 1) Read header fields from your controls
            ' ─────────────────────────────────────────────────────────────
            Dim companyName = txtncompany.Text.Trim()
            Dim companyAddr = txtcaddress.Text.Trim()
            Dim companyTIN = txtctin.Text.Trim()
            Dim poNo = txtpono.Text.Trim()
            Dim poDate = dtpodate.Value.ToString("MM/dd/yyyy")                             ' or dtdpo.Value.ToString("MM/dd/yyyy")
            Dim modeProc = txtmproc.Text.Trim()
            Dim pDelivery = txtpdelivery.Text.Trim()
            Dim dDelivery = dtddelivery.Value.ToString("MM/dd/yyyy")
            Dim purposeText = txtpurpose.Text.Trim()
            Dim supplierName = lblsupname.Text.Trim()
            Dim fundCluster = txtfcluster.Text.Trim()

            Dim poId As Integer
            If Not Integer.TryParse(lblpoid.Text, poId) Then
                MessageBox.Show("Invalid PO ID.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            ' ─────────────────────────────────────────────────────────────
            ' 2) Fetch MINISTER & ACCOUNTANT from TBL_CONFIG
            '    and split name/position on newline or last space
            ' ─────────────────────────────────────────────────────────────
            Dim ministerHtml As String = ""
            Dim accountantHtml As String = ""
            Using cfgConn As New SqlConnection(frmmain.txtdb.Text)
                cfgConn.Open()
                Using cmd As New SqlCommand("
SELECT TOP 1 MINISTER, ACCOUNTANT
  FROM TBL_CONFIG;", cfgConn)
                    Using rdr = cmd.ExecuteReader()
                        If rdr.Read() Then
                            ' MINISTER
                            Dim rawMin = rdr("MINISTER").ToString().Trim()
                            Dim minName As String, minPos As String
                            If rawMin.Contains(vbCrLf) Then
                                Dim parts = rawMin.Split(New String() {vbCrLf}, StringSplitOptions.RemoveEmptyEntries)
                                minName = parts(0).Trim()
                                minPos = If(parts.Length > 1, parts(1).Trim(), "")
                            ElseIf rawMin.Contains(" ") Then
                                Dim idx = rawMin.LastIndexOf(" "c)
                                minName = rawMin.Substring(0, idx).Trim()
                                minPos = rawMin.Substring(idx + 1).Trim()
                            Else
                                minName = rawMin
                                minPos = ""
                            End If
                            ministerHtml = $"<b>{minName}</b><br/>{minPos}"

                            ' ACCOUNTANT
                            Dim rawAcc = rdr("ACCOUNTANT").ToString().Trim()
                            Dim accName As String, accPos As String
                            If rawAcc.Contains(vbCrLf) Then
                                Dim parts = rawAcc.Split(New String() {vbCrLf}, StringSplitOptions.RemoveEmptyEntries)
                                accName = parts(0).Trim()
                                accPos = If(parts.Length > 1, parts(1).Trim(), "")
                            ElseIf rawAcc.Contains(" ") Then
                                Dim idx = rawAcc.LastIndexOf(" "c)
                                accName = rawAcc.Substring(0, idx).Trim()
                                accPos = rawAcc.Substring(idx + 1).Trim()
                            Else
                                accName = rawAcc
                                accPos = ""
                            End If
                            accountantHtml = $"<b>{accName}</b><br/>{accPos}"
                        End If
                    End Using
                End Using
            End Using

            ' ─────────────────────────────────────────────────────────────
            ' 3) Load PO items into a DataTable
            ' ─────────────────────────────────────────────────────────────
            Dim DTPO As New DataTable()
            Using conn As New SqlConnection(frmmain.txtdb.Text)
                conn.Open()
                Using da As New SqlDataAdapter("
SELECT 
    PO_ITEMNO,
    PO_UNIT,
    PO_DESC,
    PO_QTY,
    PO_UCOST,
    PO_TCOST
  FROM TBL_POITEMS
 WHERE POID = @poid
 ORDER BY PO_ITEMNO;", conn)
                    da.SelectCommand.Parameters.AddWithValue("@poid", poId)
                    da.Fill(DTPO)
                End Using
            End Using


            ' ─────────────────────────────────────────────────────────────
            ' 4) Build RDLC parameters (names must match your rptPO.rdlc)
            ' ─────────────────────────────────────────────────────────────
            Dim rptParams As New List(Of ReportParameter) From {
                New ReportParameter("cname", companyName),
                New ReportParameter("caddress", companyAddr),
                New ReportParameter("ctin", companyTIN),
                New ReportParameter("pono", poNo),
                New ReportParameter("podate", poDate),
                New ReportParameter("mproc", modeProc),
                New ReportParameter("pdelivery", pDelivery),
                New ReportParameter("ddelivery", dDelivery),
                New ReportParameter("purpose", purposeText),
                New ReportParameter("supname", supplierName),
                New ReportParameter("awords", "*** " & NumberToWords(lbltotal.Text) & " Pesos ***"),
                New ReportParameter("fcluster", fundCluster),
                New ReportParameter("minister", ministerHtml),
                New ReportParameter("accountant", accountantHtml)
            }

            ' ─────────────────────────────────────────────────────────────
            ' 5) Show preview in frmpprev
            ' ─────────────────────────────────────────────────────────────
            Dim rdlcPath = Path.Combine(Application.StartupPath, "Report", "rptPO.rdlc")
            If Not File.Exists(rdlcPath) Then
                MessageBox.Show($"RDLC file not found: {rdlcPath}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            Using preview As New frmpprev()
                With preview.ReportViewer1
                    .ProcessingMode = ProcessingMode.Local
                    .LocalReport.ReportPath = rdlcPath
                    .LocalReport.DataSources.Clear()
                    .LocalReport.DataSources.Add(
                        New ReportDataSource("DSPO", DTPO))
                    .LocalReport.SetParameters(rptParams)
                    .SetDisplayMode(DisplayMode.PrintLayout)
                    .ZoomMode = ZoomMode.Percent
                    .ZoomPercent = 100
                    .RefreshReport()
                End With

                preview.panelsubmit.Visible = False
                preview.ShowDialog()
            End Using

        Catch ex As Exception
            MessageBox.Show("Error printing PO preview: " & ex.Message,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub txtpono_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtpono.KeyPress
        e.Handled = True
    End Sub

    Private Sub txtdpo_KeyPress(sender As Object, e As KeyPressEventArgs)
        e.Handled = True
    End Sub

    Private Sub txtmproc_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtmproc.KeyPress
        e.Handled = True
    End Sub

    Private Sub txtstatus_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtstatus.KeyPress
        e.Handled = True
    End Sub

    Private Sub txtncompany_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtncompany.KeyPress
        e.Handled = True
    End Sub

    Private Sub txtcaddress_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtcaddress.KeyPress
        e.Handled = True
    End Sub

    Private Sub txtctin_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtctin.KeyPress

    End Sub

    Private Sub txtpurpose_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtpurpose.KeyPress
        e.Handled = True
    End Sub

    Private Sub txtfcluster_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtfcluster.KeyPress
        e.Handled = True
    End Sub

    Private Sub cmdsubmit_Click(sender As Object, e As EventArgs) Handles cmdsubmit.Click
        ' Create a fresh instance; DO NOT Dispose() the form first.
        Using f As New frmdeliveryadd()
            ' Pass values before showing (Load will see these)
            Dim ddate As String = dtddelivery.Value.ToString("MMMM dd, yyyy")
            Dim podate As String = dtpodate.Value.ToString("MMMM dd, yyyy")
            f.lblpoid.Text = lblpoid.Text
            f.lblprid.Text = lblprid.Text
            f.txtpono.Text = txtpono.Text
            f.txtprno.Text = lblprno.Text
            f.txtpdelivery.Text = txtpdelivery.Text
            f.lblpodate.Text = podate
            f.txtcname.Text = txtncompany.Text
            f.txtddelivery.Text = ddate
            f.txtttype.Text = "Direct Payment"


            ' If frmdeliveryadd has a DateTimePicker, set it directly.
            ' If it only has a textbox for date, pass the string you'd like to show.

            f.StartPosition = FormStartPosition.CenterParent
            f.ShowDialog(Me)  ' modal, parented
        End Using
    End Sub

    Public Sub submitpo()
        Try
            ' 1) Validate PR ID
            Dim prIdValue As Integer
            If Not Integer.TryParse(lblprid.Text, prIdValue) Then
                MessageBox.Show("Invalid PR ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            ' 2) Update PO + PR statuses in one transaction
            Using conn As New SqlClient.SqlConnection(My.Forms.frmmain.txtdb.Text)
                conn.Open()
                Using tran = conn.BeginTransaction()
                    ' a) Mark all POs for this PR as Completed
                    Using cmdPO As New SqlClient.SqlCommand(
                        "UPDATE TBL_PO
                        SET PO_STATUS = 'Completed'
                      WHERE PO_PRID   = @prid;", conn, tran)

                        cmdPO.Parameters.Add("@prid", SqlDbType.Int).Value = prIdValue
                        cmdPO.ExecuteNonQuery()
                    End Using

                    ' b) Update the PR's status
                    Using cmdPR As New SqlClient.SqlCommand(
                        "UPDATE TBL_PR
                        SET PRSTATUS = '[P.O] Completed'
                      WHERE ID        = @prid;", conn, tran)

                        cmdPR.Parameters.Add("@prid", SqlDbType.Int).Value = prIdValue
                        cmdPR.ExecuteNonQuery()
                    End Using

                    tran.Commit()
                End Using
            End Using

            ' 3) Disable controls now that submission is done

            cmdsave.Enabled = False
            cmdsubmit.Enabled = False
            cmdscancel.Enabled = True
            txtstatus.Text = "Completed"

            ' 4) Refresh the grids / parent form
            ' frmpo.LoadPO()     ' or whatever your PO‐list reload method is
            LoadPOItems()          ' if you have a separate method to reload the items grid

            MessageBox.Show("Purchase Order has been submitted successfully and is now completed.",
                            "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MessageBox.Show("Error submitting Purchase Order: " & ex.Message,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


    Private Sub cmdscancel_Click(sender As Object, e As EventArgs) Handles cmdscancel.Click
        ' Prevent double-click re-entry
        If Not cmdscancel.Enabled Then Return
        cmdscancel.Enabled = False

        Try
            ' 1) Validate PR ID and PO ID
            Dim prIdValue As Integer
            If Not Integer.TryParse(lblprid.Text, prIdValue) Then
                MessageBox.Show("Invalid PR ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                cmdscancel.Enabled = True
                Return
            End If

            Dim poIdValue As Integer
            If Not Integer.TryParse(lblpoid.Text, poIdValue) Then
                MessageBox.Show("Invalid PO ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                cmdscancel.Enabled = True
                Return
            End If

            ' 2) Delete IAR (+ items) only if IAR is Pending, and revert statuses, in one transaction
            Using conn As New SqlClient.SqlConnection(My.Forms.frmmain.txtdb.Text)
                conn.Open()
                Using tran = conn.BeginTransaction(IsolationLevel.ReadCommitted)
                    Try
                        ' Safety: rollback automatically if the connection drops mid-transaction
                        Using cmdOn As New SqlClient.SqlCommand("SET XACT_ABORT ON; SET NOCOUNT ON;", conn, tran)
                            cmdOn.ExecuteNonQuery()
                        End Using

                        ' 2a) Check for any Pending IAR for this POID (lock row to avoid race)
                        Dim pendingCount As Integer
                        Using cmdChk As New SqlClient.SqlCommand("
                        SELECT COUNT(*)
                        FROM TBL_IAR WITH (UPDLOCK, HOLDLOCK)
                        WHERE POID = @poid AND STATUS = 'Pending';", conn, tran)
                            cmdChk.Parameters.Add("@poid", SqlDbType.Int).Value = poIdValue
                            pendingCount = CInt(cmdChk.ExecuteScalar())
                        End Using

                        If pendingCount = 0 Then
                            ' Nothing to delete; do not revert statuses. Exit gracefully.
                            tran.Rollback()
                            MessageBox.Show("Cancel blocked: No Pending IAR found for this PO. Only Pending IAR can be deleted.",
                                            "Cannot Cancel", MessageBoxButtons.OK, MessageBoxIcon.Information)
                            cmdscancel.Enabled = True
                            Return
                        End If

                        ' 2b) Delete IAR Items whose parent IAR (same POID) is Pending
                        Using cmdDelItems As New SqlClient.SqlCommand("
                        DELETE I
                        FROM TBL_IARITEMS AS I
                        WHERE EXISTS (
                            SELECT 1
                            FROM TBL_IAR AS H
                            WHERE H.ID = I.IARID
                              AND H.POID = @poid
                              AND H.STATUS = 'Pending'
                        );", conn, tran)
                            cmdDelItems.Parameters.Add("@poid", SqlDbType.Int).Value = poIdValue
                            cmdDelItems.ExecuteNonQuery()
                        End Using

                        ' 2c) Delete IAR headers for this POID that are Pending
                        Using cmdDelIAR As New SqlClient.SqlCommand("
                        DELETE FROM TBL_IAR
                        WHERE POID = @poid
                          AND STATUS = 'Pending';", conn, tran)
                            cmdDelIAR.Parameters.Add("@poid", SqlDbType.Int).Value = poIdValue
                            cmdDelIAR.ExecuteNonQuery()
                        End Using

                        ' 2d) Revert all POs for this PR back to Processing
                        Using cmdPO As New SqlClient.SqlCommand("
                        UPDATE TBL_PO
                        SET PO_STATUS = 'Processing'
                        WHERE PO_PRID = @prid;", conn, tran)
                            cmdPO.Parameters.Add("@prid", SqlDbType.Int).Value = prIdValue
                            cmdPO.ExecuteNonQuery()
                        End Using

                        ' 2e) Revert PR status back to [PO] Processing
                        Using cmdPR As New SqlClient.SqlCommand("
                        UPDATE TBL_PR
                        SET PRSTATUS = '[P.O] Processing'
                        WHERE ID = @prid;", conn, tran)
                            cmdPR.Parameters.Add("@prid", SqlDbType.Int).Value = prIdValue
                            cmdPR.ExecuteNonQuery()
                        End Using

                        tran.Commit()

                    Catch exTx As Exception
                        Try : tran.Rollback() : Catch : End Try
                        Throw
                    End Try
                End Using
            End Using

            ' 3) Re-enable / update UI
            cmdsave.Enabled = True
            cmdsubmit.Enabled = True
            cmdscancel.Enabled = False
            txtstatus.Text = "Processing"

            ' 4) Refresh UI
            'frmpo.LoadPO()      ' reload your PO list
            LoadPOItems()       ' reload the items grid if you have one

            '/////////////////////////////////// NOTIFICATION
            NotificationHelper.AddNotification($"Delivery information for Purchase Order {txtpono.Text} has been Canceled and Deleted by {frmmain.lblaname.Text.Trim()}.")
            '/////////////////////////////////// NOTIFICATION

            MessageBox.Show(
                "Purchase Order submission has been canceled. Pending IAR records were deleted, and status reverted to Processing.",
                "Canceled", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MessageBox.Show("Error canceling PO submission: " & ex.Message,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            ' Let user try again
            cmdscancel.Enabled = True
        End Try
    End Sub
    ' ───────────────────────────────
    ' Convert Number to Words (Philippine Peso Format)
    ' ───────────────────────────────
    Private Function NumberToWords(ByVal number As Decimal) As String
        Dim wholePart As Long = Math.Truncate(number)
        Dim fractionPart As Integer = CInt((number - wholePart) * 100)

        Dim words As New StringBuilder()

        ' Convert whole part
        words.Append(ConvertWholeNumber(wholePart) & " Pesos")

        ' If there are centavos
        If fractionPart > 0 Then
            words.Append(" and " & fractionPart.ToString("00") & "/100 Centavos")
        End If

        Return words.ToString().Trim()
    End Function

    Private Function ConvertWholeNumber(ByVal number As Long) As String
        Dim unitsMap() As String = {"Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen"}
        Dim tensMap() As String = {"Zero", "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety"}

        If number = 0 Then
            Return "Zero"
        End If

        If number < 0 Then
            Return "Minus " & ConvertWholeNumber(Math.Abs(number))
        End If

        Dim words As String = ""

        If (number \ 1000000000) > 0 Then
            words &= ConvertWholeNumber(number \ 1000000000) & " Billion "
            number = number Mod 1000000000
        End If

        If (number \ 1000000) > 0 Then
            words &= ConvertWholeNumber(number \ 1000000) & " Million "
            number = number Mod 1000000
        End If

        If (number \ 1000) > 0 Then
            words &= ConvertWholeNumber(number \ 1000) & " Thousand "
            number = number Mod 1000
        End If

        If (number \ 100) > 0 Then
            words &= ConvertWholeNumber(number \ 100) & " Hundred "
            number = number Mod 100
        End If

        If number > 0 Then
            If words <> "" Then words &= " "

            If number < 20 Then
                words &= unitsMap(number)
            Else
                words &= tensMap(number \ 10)
                If (number Mod 10) > 0 Then
                    words &= " " & unitsMap(number Mod 10)
                End If
            End If
        End If

        Return words.Trim()
    End Function


    Private Sub Txtctin_TextChanged(sender As Object, e As EventArgs) Handles txtctin.TextChanged

    End Sub
End Class