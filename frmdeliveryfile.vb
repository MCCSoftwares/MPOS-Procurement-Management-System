Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports Microsoft.Reporting.WinForms
Imports System.Net

Public Class frmdeliveryfile

    ' ─────────────────────────────────────────────────────────────────────────────
    ' Call this in Form_Load (after InitializeComponent)

    ' ─────────────────────────────────────────────────────────────────────────────
    ' Public: Load header from TBL_IAR by POID, populate controls, then load items
    ' ─────────────────────────────────────────────────────────────────────────────
    Public Sub LoadIAR()
        Try
            Dim pRid As Integer
            If Not Integer.TryParse(lblprid.Text, pRid) Then
                MessageBox.Show("Invalid PR ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                ClearIARForm()
                Return
            End If

            ' 1) Pull IAR header (assuming one IAR per POID; if multiple, takes latest by ID)
            Dim iarFound As Boolean = False
            Dim iarId As Integer = 0

            Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
                conn.Open()

                Using cmd As New SqlCommand("
                    SELECT TOP(1)
                        ID,
                        IARNO,
                        IARDATE,
                        PONO,
                        POID,
                        PODATE,
                        PRNO,
                        PRID,
                        STATUS,
                        REQOFFICE,
                        RESCODE,
                        FCLUSTER,
                        CNAME,
                        PDELIVERY,
                        DDATE,
                        INVOICENO,
                        INVOICEDATE,
                        ISTATUS,
                        ASTATUS,
                        DINSPECTED,
                        SCUSTODIAN,
                        DACCEPT,
                        IOFFICER,
                        TTYPE
                    FROM TBL_IAR
                    WHERE PRID = @pRid
                    ORDER BY ID DESC;", conn)

                    cmd.Parameters.Add("@pRid", SqlDbType.Int).Value = pRid

                    Using rdr = cmd.ExecuteReader(CommandBehavior.SingleRow)
                        If rdr.Read() Then
                            iarFound = True

                            ' Keep ID for items
                            iarId = SafeGetInt(rdr, "ID")
                            lbliarid.Text = iarId.ToString()

                            ' Map to controls
                            txtiarno.Text = SafeGetString(rdr, "IARNO")
                            Dim hasIARDate As Boolean = Not IsDBNull(rdr("IARDATE")) : dtiardate.Checked = hasIARDate : If hasIARDate Then dtiardate.Value = CDate(rdr("IARDATE")) : dtiardate.Enabled = False Else dtiardate.Enabled = True

                            txtpono.Text = SafeGetString(rdr, "PONO")
                            lblpoid.Text = SafeGetInt(rdr, "POID").ToString()
                            txtprno.Text = SafeGetString(rdr, "PRNO")
                            lblprid.Text = SafeGetInt(rdr, "PRID").ToString()
                            txtstatus.Text = SafeGetString(rdr, "STATUS")
                            txtreqoff.Text = SafeGetString(rdr, "REQOFFICE")
                            txtrcc.Text = SafeGetString(rdr, "RESCODE")
                            txtfcluster.Text = SafeGetString(rdr, "FCLUSTER")
                            txtcname.Text = SafeGetString(rdr, "CNAME")
                            txtpdelivery.Text = SafeGetString(rdr, "PDELIVERY")
                            lblpodate.Text = If(IsDBNull(rdr("PODATE")), "", CDate(rdr("PODATE")).ToString("MM/dd/yyyy"))

                            ' DateTimePickers (try-parse to avoid exceptions)
                            SetDatePickerValue(dtddelivery, SafeGetDateNullable(rdr, "DDATE"))
                            txtino.Text = SafeGetString(rdr, "INVOICENO")
                            SetDatePickerValue(dtidate, SafeGetDateNullable(rdr, "INVOICEDATE"))

                            ' Checkboxes
                            Dim iStatus As String = SafeGetString(rdr, "ISTATUS").Trim()
                            cbinspected.Checked = String.Equals(iStatus, "Yes", StringComparison.OrdinalIgnoreCase)

                            Dim aStatus As String = SafeGetString(rdr, "ASTATUS").Trim()
                            Select Case aStatus
                                Case "Complete"
                                    cbcomplete.Checked = True
                                    cbpartial.Checked = False
                                Case "Partial"
                                    cbcomplete.Checked = False
                                    cbpartial.Checked = True
                                Case Else
                                    cbcomplete.Checked = False
                                    cbpartial.Checked = False
                            End Select

                            ' Labels for dates / inspector
                            lblidate.Text = SafeGetDateString(rdr, "DINSPECTED")
                            lbladate.Text = SafeGetDateString(rdr, "DACCEPT")
                            comboinspector.Text = SafeGetString(rdr, "IOFFICER")
                            comboscustodian.Text = SafeGetString(rdr, "SCUSTODIAN")
                            txtttype.Text = SafeGetString(rdr, "TTYPE")


                            If txtstatus.Text = "Pending" Then
                                cmdsubmit.Enabled = False
                                cmdprint.Enabled = False
                            ElseIf txtstatus.Text = "Processing" Then
                                cmdsubmit.Enabled = True
                            Else
                                cmdsubmit.Enabled = False
                                cmdupdate.Enabled = False
                            End If
                        Else
                            iarFound = False
                        End If
                    End Using
                End Using

                LoadIARItems()
                ' 2) Load items bound to grid if header was found
            End Using

        Catch ex As Exception
            MessageBox.Show("LoadIAR Error: " & ex.ToString, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ─────────────────────────────────────────────────────────────────────────────
    ' Private helpers
    ' ─────────────────────────────────────────────────────────────────────────────
    Private Sub LoadIARItems()
        Try
            ' IARID must come from lbliarid.Text
            Dim iarId As Integer
            If Not Integer.TryParse(lbliarid.Text.Trim(), iarId) OrElse iarId <= 0 Then
                MessageBox.Show("IAR ID is missing/invalid. Cannot load items.", "IAR Items", MessageBoxButtons.OK, MessageBoxIcon.Information)
                DataGridView1.DataSource = Nothing
                Return
            End If

            ' Columns already configured in Form_Load
            '   DataGridView1.AutoGenerateColumns = False
            ' DataGridView1.SuspendLayout()

            Using conn As New SqlClient.SqlConnection(My.Forms.frmmain.txtdb.Text)
                conn.Open()

                Using cmd As New SqlClient.SqlCommand("
                SELECT
                    ITEMNO       AS [ITEMNO],
                    DESCRIPTIONS AS [DESCRIPTIONS],
                    UNITS        AS [UNITS],
                    QUANTITY     AS [QUANTITY]
                FROM TBL_IARITEMS
                WHERE IARID = @iarid
                ORDER BY ITEMNO;", conn)

                    cmd.Parameters.Add("@iarid", SqlDbType.Int).Value = iarId

                    Using da As New SqlClient.SqlDataAdapter(cmd)
                        Dim dt As New DataTable()

                        ' ✅ This was missing
                        da.Fill(dt)

                        If dt.Rows.Count = 0 Then
                            DataGridView1.DataSource = Nothing
                            DataGridView1.Rows.Clear()
                            MessageBox.Show($"No IAR items found for IARID={iarId}.", "IAR Items", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        Else
                            DataGridView1.DataSource = dt
                            If DataGridView1.Columns.Contains("QUANTITY") Then
                                DataGridView1.Columns("QUANTITY").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                                DataGridView1.Columns("QUANTITY").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
                                DataGridView1.Columns("QUANTITY").DefaultCellStyle.Format = "N0"
                            End If
                            DataGridView1.ClearSelection()
                        End If
                    End Using
                End Using
            End Using

        Catch ex As Exception
            MessageBox.Show("LoadIARItems Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub



    Private Sub ClearIARForm()
        ' Clear header fields
        txtiarno.Clear()
        lbliarid.Text = ""
        txtpono.Clear()
        lblpoid.Text = ""
        txtprno.Clear()
        lblprid.Text = ""
        txtstatus.Clear()
        txtreqoff.Clear()
        txtrcc.Clear()
        txtfcluster.Clear()
        txtcname.Clear()
        txtpdelivery.Clear()
        txtino.Clear()
        lblidate.Text = ""
        lbladate.Text = ""
        cbinspected.Checked = False
        cbcomplete.Checked = False
        cbpartial.Checked = False

        ' Reset DTPs safely to today (or keep last value if you prefer)
        dtddelivery.Value = Date.Today
        dtidate.Value = Date.Today

        ' Clear grid
        If DataGridView1.DataSource IsNot Nothing Then
            DataGridView1.DataSource = Nothing
        End If
        DataGridView1.Rows.Clear()
    End Sub

    Private Shared Function SafeGetString(rdr As SqlDataReader, col As String) As String
        Dim idx = rdr.GetOrdinal(col)
        If rdr.IsDBNull(idx) Then Return String.Empty
        Return Convert.ToString(rdr.GetValue(idx)).Trim()
    End Function

    Private Shared Function SafeGetInt(rdr As SqlDataReader, col As String) As Integer
        Dim idx = rdr.GetOrdinal(col)
        If rdr.IsDBNull(idx) Then Return 0
        Return Convert.ToInt32(rdr.GetValue(idx))
    End Function

    Private Shared Function SafeGetDateNullable(rdr As SqlDataReader, col As String) As Date?
        Dim idx = rdr.GetOrdinal(col)
        If rdr.IsDBNull(idx) Then Return Nothing
        Dim v = rdr.GetValue(idx)
        Dim d As DateTime
        If DateTime.TryParse(Convert.ToString(v), d) Then Return d
        Return Nothing
    End Function

    Private Shared Function SafeGetDateString(rdr As SqlDataReader, col As String) As String
        Dim d = SafeGetDateNullable(rdr, col)
        If d.HasValue Then
            ' Use your preferred display format
            Return d.Value.ToString("MM/dd/yyyy")
        End If
        Return String.Empty
    End Function

    Private Sub SetDatePickerValue(dtp As DateTimePicker, d As Date?)
        Try
            If d.HasValue Then
                dtp.Value = d.Value
            Else
                ' Keep current value if null, or set to today; choose your preference
                dtp.Value = Date.Today
            End If
        Catch
            dtp.Value = Date.Today
        End Try
    End Sub

    Private Sub Frmdeliveryfile_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
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
                .ReadOnly = True

                ' Flat, borderless look
                .BorderStyle = BorderStyle.None
                .CellBorderStyle = DataGridViewCellBorderStyle.None
                .ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
                .AdvancedCellBorderStyle.Left = DataGridViewAdvancedCellBorderStyle.None
                .AdvancedCellBorderStyle.Right = DataGridViewAdvancedCellBorderStyle.None
                .AdvancedCellBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.None
                .AdvancedCellBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.None
            End With

            ' Define columns for: ITEMNO, DESCRIPTIONS, UNITS, QUANTITY
            DataGridView1.ColumnCount = 4

            With DataGridView1
                .Columns(0).Name = "ITEMNO"
                .Columns(0).HeaderText = "ITEM/PROPERTY NUMBER"
                .Columns(0).DataPropertyName = "ITEMNO"
                .Columns(0).Width = 90
                .Columns(0).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
                .Columns(0).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft

                .Columns(1).Name = "DESCRIPTIONS"
                .Columns(1).HeaderText = "DESCRIPTION"
                .Columns(1).DataPropertyName = "DESCRIPTIONS"
                .Columns(1).AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                .Columns(1).DefaultCellStyle.WrapMode = DataGridViewTriState.True

                .Columns(2).Name = "UNITS"
                .Columns(2).HeaderText = "UNIT"
                .Columns(2).DataPropertyName = "UNITS"
                .Columns(2).Width = 100
                .Columns(2).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                .Columns(2).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter

                .Columns(3).Name = "QUANTITY"
                .Columns(3).HeaderText = "QTY"
                .Columns(3).DataPropertyName = "QUANTITY"
                .Columns(3).Width = 100
                .Columns(3).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
                .Columns(3).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft
                .Columns(3).DefaultCellStyle.Format = "N0"
            End With

            ' Styling (match your theme)
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

            LoadIARItems()
            LoadCustodians()
            LoadInspectors()

        Catch ex As Exception
            MessageBox.Show("Grid Setup Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub Cmdupdate_Click(sender As Object, e As EventArgs) Handles cmdupdate.Click
        Try
            ' Validate target IAR row
            Dim iarId As Integer
            If Not Integer.TryParse(lbliarid.Text.Trim(), iarId) OrElse iarId <= 0 Then
                MessageBox.Show("Invalid IAR ID.", "Update IAR", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            ' Map UI -> values
            Dim todayOnly As Date = Date.Today

            ' Use dtiardate.Value for year/month and for saving to IARDATE
            Dim chosenDate As DateTime = dtiardate.Value
            Dim yr As Integer = chosenDate.Year
            Dim mo As Integer = chosenDate.Month

            ' ── NEW: determine prefix by TTYPE ───────────────────────────────────────
            Dim ttype As String = txtttype.Text.Trim()
            Dim prefix As String = ""
            Select Case ttype
                Case "Cash Advance" : prefix = "C-"
                Case "Reimbursement" : prefix = "R-"
                Case Else : prefix = ""   ' Direct Payment (or any others) => no prefix
            End Select

            ' For series lookup (resets per YEAR, per TTYPE), filter by prefix + YEAR only
            Dim yearPrefix As String = prefix & yr.ToString("0000") & "-"               ' e.g., "C-2025-" or "2025-"
            ' For the final IARNO display, include prefix + YEAR + MONTH
            Dim displayPrefix As String = prefix & yr.ToString("0000") & "-" & mo.ToString("00") & "-"   ' e.g., "C-2025-09-"

            Dim currentIarNo As String = txtiarno.Text.Trim()
            Dim newIarNo As String = currentIarNo

            ' Status mapping
            If cbcomplete.Checked AndAlso cbpartial.Checked Then
                MessageBox.Show("Please select either 'Complete' or 'Partial', not both.", "Update IAR", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Dim iStatus As String = If(cbinspected.Checked, "Yes", "No")
            Dim aStatus As String =
            If(cbcomplete.Checked, "Complete",
            If(cbpartial.Checked, "Partial", String.Empty))

            ' Dates to set depending on checkboxes
            Dim setDIns As Boolean = cbinspected.Checked
            Dim setDAccept As Boolean = (cbcomplete.Checked OrElse cbpartial.Checked)

            Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
                conn.Open()
                Using tran = conn.BeginTransaction(IsolationLevel.Serializable)

                    ' If UI IARNO is blank, check DB first; if DB has it, reuse; else generate next series
                    If String.IsNullOrWhiteSpace(newIarNo) Then
                        Using cmdChk As New SqlCommand("SELECT IARNO FROM TBL_IAR WITH (UPDLOCK, HOLDLOCK) WHERE ID=@id;", conn, tran)
                            cmdChk.Parameters.Add("@id", SqlDbType.Int).Value = iarId
                            Dim existing As Object = cmdChk.ExecuteScalar()
                            If existing IsNot Nothing AndAlso existing IsNot DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(CStr(existing)) Then
                                newIarNo = CStr(existing)
                            Else
                                ' ── NEW: Generate next series for prefix + YEAR (ignores month), series per TTYPE and resets each year
                                Using cmdNext As New SqlCommand("
                                SELECT ISNULL(
                                    MAX(TRY_CONVERT(int,
                                        PARSENAME(REPLACE(LTRIM(RTRIM(IARNO)),'-','.'), 1)  -- take last token after final '-'
                                    )), 0)
                                FROM TBL_IAR WITH (UPDLOCK, HOLDLOCK)
                                WHERE IARNO LIKE @yearPrefix + '%';", conn, tran)

                                    cmdNext.Parameters.Add("@yearPrefix", SqlDbType.VarChar, 32).Value = yearPrefix   ' e.g., "C-2025-"
                                    Dim maxSeries As Integer = Convert.ToInt32(cmdNext.ExecuteScalar())
                                    newIarNo = displayPrefix & (maxSeries + 1).ToString("0000")  ' e.g., "C-2025-09-0004"
                                End Using
                            End If
                        End Using
                    End If

                    ' Update only the fields you edit + required system fields
                    Using cmdUpd As New SqlCommand("
                    UPDATE TBL_IAR
                    SET
                        IARNO       = @iarno,
                        IARDATE     = @iardate,         -- always from dtiardate.Value
                        STATUS      = 'Processing',
                        INVOICENO   = @invoiceno,
                        INVOICEDATE = @invoicedate,
                        IOFFICER    = @iofficer,
                        SCUSTODIAN  = @scustodian,
                        ISTATUS     = @istatus,
                        ASTATUS     = @astatus,
                        DINSPECTED  = CASE WHEN @setDIns = 1 THEN @dinspected ELSE DINSPECTED END,
                        DACCEPT     = CASE WHEN @setDAccept = 1 THEN @daccept ELSE DACCEPT END
                    WHERE ID = @id;", conn, tran)

                        cmdUpd.Parameters.Add("@iarno", SqlDbType.VarChar, 50).Value = newIarNo
                        cmdUpd.Parameters.Add("@invoiceno", SqlDbType.VarChar, 50).Value = txtino.Text.Trim()
                        cmdUpd.Parameters.Add("@invoicedate", SqlDbType.DateTime).Value = dtidate.Value
                        cmdUpd.Parameters.Add("@iofficer", SqlDbType.VarChar, 100).Value = comboinspector.Text.Trim()
                        cmdUpd.Parameters.Add("@scustodian", SqlDbType.VarChar, 100).Value = comboscustodian.Text.Trim()
                        cmdUpd.Parameters.Add("@istatus", SqlDbType.VarChar, 10).Value = iStatus
                        If String.IsNullOrEmpty(aStatus) Then
                            cmdUpd.Parameters.Add("@astatus", SqlDbType.VarChar, 20).Value = DBNull.Value
                        Else
                            cmdUpd.Parameters.Add("@astatus", SqlDbType.VarChar, 20).Value = aStatus
                        End If

                        ' Checkbox-driven date stamping
                        cmdUpd.Parameters.Add("@setDIns", SqlDbType.Bit).Value = If(setDIns, 1, 0)
                        cmdUpd.Parameters.Add("@setDAccept", SqlDbType.Bit).Value = If(setDAccept, 1, 0)
                        cmdUpd.Parameters.Add("@dinspected", SqlDbType.DateTime).Value = If(setDIns, CType(todayOnly, Object), DBNull.Value)
                        cmdUpd.Parameters.Add("@daccept", SqlDbType.DateTime).Value = If(setDAccept, CType(todayOnly, Object), DBNull.Value)

                        ' Always persist IARDATE from the picker selection
                        cmdUpd.Parameters.Add("@iardate", SqlDbType.DateTime).Value = chosenDate

                        cmdUpd.Parameters.Add("@id", SqlDbType.Int).Value = iarId

                        Dim n = cmdUpd.ExecuteNonQuery()
                        If n <> 1 Then
                            Throw New ApplicationException("Update failed. The IAR record may not exist or was modified by another user.")
                        End If

                        cmdprint.Enabled = True
                        cmdsubmit.Enabled = True
                        cmdupdate.Enabled = True
                    End Using

                    tran.Commit()
                End Using
            End Using

            ' Reflect to UI
            txtiarno.Text = newIarNo
            txtstatus.Text = "Processing"

            ' Set the labels (datevalue(now)) only when corresponding boxes were checked
            If setDIns Then lblidate.Text = todayOnly.ToString("MM/dd/yyyy")
            If setDAccept Then lbladate.Text = todayOnly.ToString("MM/dd/yyyy")

            '/////////////////////////////////// NOTIFICATION
            NotificationHelper.AddNotification($"Inspection and Acceptance Report Number {txtiarno.Text} for Purchase Order {txtpono.Text} has been created by {frmmain.lblaname.Text.Trim()}.")
            '/////////////////////////////////// NOTIFICATION

            MessageBox.Show("IAR updated successfully.", "Update IAR", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MessageBox.Show("Update IAR failed: " & ex.Message, "Update IAR", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub



    Private Sub txtiarno_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtiarno.KeyPress
        e.Handled = True
    End Sub

    Private Sub txtiardate_KeyPress(sender As Object, e As KeyPressEventArgs)
        e.Handled = True
    End Sub

    Private Sub txtpono_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtpono.KeyPress
        e.Handled = True
    End Sub

    Private Sub txtprno_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtprno.KeyPress
        e.Handled = True
    End Sub

    Private Sub txtstatus_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtstatus.KeyPress
        e.Handled = True
    End Sub

    Private Sub txtreqoff_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtreqoff.KeyPress
        e.Handled = True
    End Sub

    Private Sub txtrcc_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtrcc.KeyPress
        e.Handled = True
    End Sub

    Private Sub txtfcluster_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtfcluster.KeyPress
        e.Handled = True
    End Sub

    Private Sub txtcname_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtcname.KeyPress
        e.Handled = True
    End Sub


    Private Sub Cmdclose_Click(sender As Object, e As EventArgs) Handles cmdclose.Click
        Me.Dispose()
    End Sub

    Private Sub Cmdprint_Click(sender As Object, e As EventArgs) Handles cmdprint.Click
        Try
            frmpprev.Dispose()

            Dim iarId As Integer
            If Not Integer.TryParse(lbliarid.Text.Trim(), iarId) OrElse iarId <= 0 Then
                MessageBox.Show("Invalid IAR ID for printing.", "IAR Print", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            ' 1) Load items
            Dim dt As New DataTable("DTIAR")
            Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
                conn.Open()
                Using cmd As New SqlCommand("
                SELECT ITEMNO, DESCRIPTIONS, UNITS, QUANTITY
                FROM TBL_IARITEMS
                WHERE IARID = @iarid
                ORDER BY ITEMNO;", conn)
                    cmd.Parameters.Add("@iarid", SqlDbType.Int).Value = iarId
                    Using da As New SqlDataAdapter(cmd)
                        da.Fill(dt)
                    End Using
                End Using
            End Using

            ' ⚠️ 2) Format signatories as HTML (bold name + line for position)
            Dim insHtml As String = FormatSignatureForHtml(comboinspector.Text)
            Dim accHtml As String = FormatSignatureForHtml(comboscustodian.Text)
            Dim accCompValue As String = ""
            Dim accPartValue As String = ""

            If cbcomplete.Checked Then
                accCompValue = "✓"
                accPartValue = "_"
            ElseIf cbpartial.Checked Then
                accCompValue = "_"
                accPartValue = "✓"
            End If

            ' 3) Parameters
            Dim rp As New List(Of ReportParameter) From {
            New ReportParameter("cname", txtcname.Text.Trim()),
            New ReportParameter("PONO", txtpono.Text.Trim()),
            New ReportParameter("PODATE", lblpodate.Text.Trim()),
            New ReportParameter("REQOFF", txtreqoff.Text.Trim()),
            New ReportParameter("FCLUSTER", txtfcluster.Text.Trim()),
            New ReportParameter("RCC", txtrcc.Text.Trim()),
            New ReportParameter("IARNO", txtiarno.Text.Trim()),
            New ReportParameter("IARDATE", dtiardate.Value),
            New ReportParameter("INO", txtino.Text.Trim()),
            New ReportParameter("IDATE", dtidate.Value.ToString("MM/dd/yyyy")),
            New ReportParameter("INSDATE", lblidate.Text.Trim()),
            New ReportParameter("INSCONFIRM", If(cbinspected.Checked, "✓", "")),
            New ReportParameter("INSOFFICER", insHtml), ' ⚠️ HTML
            New ReportParameter("ACCOFFICER", accHtml), ' ⚠️ HTML
            New ReportParameter("ACCCOMP", accCompValue),
            New ReportParameter("ACCPART", accPartValue),
            New ReportParameter("ACCDATE", lbladate.Text.Trim())
        }

            ' 4) RDLC path
            Dim rdlcPath As String = Path.Combine(Application.StartupPath, "report", "rptiar.rdlc")
            If Not File.Exists(rdlcPath) Then
                MessageBox.Show("Report file not found: " & rdlcPath, "IAR Print", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            ' 5) Preview
            Dim p = My.Forms.frmpprev
            p.panelsubmit.Visible = False

            With p.ReportViewer1
                .Reset()
                .ProcessingMode = ProcessingMode.Local
                .LocalReport.ReportPath = rdlcPath
                .LocalReport.DataSources.Clear()
                Const RdlcDataSourceName As String = "DSIAR"
                .LocalReport.DataSources.Add(New ReportDataSource(RdlcDataSourceName, dt))
                .LocalReport.SetParameters(rp)
                .SetDisplayMode(DisplayMode.PrintLayout)
                .ZoomMode = ZoomMode.Percent
                .ZoomPercent = 100
                .RefreshReport()
            End With

            p.ShowDialog()

        Catch ex As Exception
            MessageBox.Show("Print preview failed: " & ex.Message, "IAR Print", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ─────────────────────────────────────────────────────────
    ' Helper: bold Name + newline Position for HTML RDLC
    ' Accepts "Name - Position" or multiline "Name{LF}Position"
    ' ─────────────────────────────────────────────────────────
    Private Function FormatSignatureForHtml(input As String) As String
        If String.IsNullOrWhiteSpace(input) Then Return ""

        Dim t As String = input.Trim()

        ' Normalize any HTML <br> back to LF to avoid double breaks
        t = t.Replace("<br />", vbLf).Replace("<br/>", vbLf).Replace("<br>", vbLf)
        t = t.Replace(vbCrLf, vbLf).Replace(vbCr, vbLf)

        Dim namePart As String = t
        Dim posPart As String = ""

        If t.Contains(" - ") Then
            Dim parts = t.Split(New String() {" - "}, 2, StringSplitOptions.None)
            namePart = parts(0).Trim()
            If parts.Length > 1 Then posPart = parts(1).Trim()
        Else
            Dim parts = t.Split({vbLf}, 2, StringSplitOptions.None)
            namePart = parts(0).Trim()
            If parts.Length > 1 Then posPart = parts(1).Trim()
        End If

        Dim nameHtml = WebUtility.HtmlEncode(namePart)
        Dim posHtml = WebUtility.HtmlEncode(posPart)

        If String.IsNullOrEmpty(posHtml) Then
            Return $"<b>{nameHtml}</b>"
        Else
            Return $"<b>{nameHtml}</b><br/>{posHtml}"
        End If
    End Function

    Private Sub cmdsubmit_Click(sender As Object, e As EventArgs) Handles cmdsubmit.Click
        Try
            ' We need the IARID in both branches, so parse once up front.
            Dim iarId As Integer
            If Not Integer.TryParse(lbliarid.Text, iarId) Then
                MessageBox.Show("Invalid IAR ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            If txtttype.Text = "Direct Payment" Then

                Dim hadExisting As Boolean = False

                Using conn As New SqlClient.SqlConnection(My.Forms.frmmain.txtdb.Text)
                    conn.Open()
                    Using tran = conn.BeginTransaction()

                        ' 1) Check if there are already logs for this IARID
                        Using cmdCheck As New SqlClient.SqlCommand(
                        "SELECT TOP 1 1 FROM TBL_LOGS_IAR WHERE IARID = @IARID;", conn, tran)
                            cmdCheck.Parameters.Add("@IARID", SqlDbType.Int).Value = iarId
                            hadExisting = (cmdCheck.ExecuteScalar() IsNot Nothing)
                        End Using

                        ' 2) If none, insert from TBL_IARITEMS → TBL_LOGS_IAR
                        If Not hadExisting Then
                            Using cmdIns As New SqlClient.SqlCommand("
                            INSERT INTO TBL_LOGS_IAR
                            ( IARID, POID, PRID, ITEMNO, DESCRIPTIONS, UNITS, QUANTITY, ACODE, ATITLE, DATES, PTYPE, STATUS )
                            SELECT 
                              I.IARID, I.POID, I.PRID, I.ITEMNO, I.DESCRIPTIONS, I.UNITS, I.QUANTITY, I.ACODE, I.ATITLE,
                              CONVERT(date, GETDATE()) AS DATES,
                              NULL AS PTYPE,
                              'Pending' AS STATUS
                            FROM TBL_IARITEMS I
                            WHERE I.IARID = @IARID
                              AND NOT EXISTS (
                                    SELECT 1
                                    FROM TBL_LOGS_IAR L
                                    WHERE L.IARID = I.IARID
                                      AND L.ITEMNO = I.ITEMNO
                              );", conn, tran)
                                cmdIns.Parameters.Add("@IARID", SqlDbType.Int).Value = iarId
                                cmdIns.ExecuteNonQuery()
                            End Using
                        End If

                        tran.Commit()
                    End Using
                End Using

                ' 3) Open the import form and filter by IARID
                Dim f As New frminventoryimport()
                Try
                    f.FilterIarId = iarId   ' if you've added this property
                Catch
                    ' ignore if not yet added
                End Try
                f.StartPosition = FormStartPosition.CenterParent
                f.lblprid.Text = lblprid.Text
                f.lbliarno.Text = txtiarno.Text
                f.ShowDialog(Me)

                'If hadExisting Then
                '    MessageBox.Show("Existing log found for this IAR. Opened the import for review.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
                'End If

            Else
                ' Cash Advance / Reimbursement path
                If MsgBox("Items from Cash Advance and Reimbursement are not allowed to import to the Inventory. This IAR will be closed instead. Do you want to continue?",
                      vbCritical + vbYesNo, "Message") = vbYes Then

                    Using conn As New SqlClient.SqlConnection(My.Forms.frmmain.txtdb.Text)
                        conn.Open()
                        Using cmd As New SqlClient.SqlCommand("
                        UPDATE TBL_IAR
                           SET STATUS = 'Completed'
                         WHERE ID = @IARID;", conn)
                            cmd.Parameters.Add("@IARID", SqlDbType.Int).Value = iarId
                            Dim affected As Integer = cmd.ExecuteNonQuery()
                            If affected > 0 Then
                                MessageBox.Show("IAR has been marked as Completed.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

                                ' Optional: reflect the change in UI if you show status on this form
                                ' txtstatus.Text = "Completed"

                                ' Optional: refresh your listing/grid if needed
                                ' RefreshIarList()
                            Else
                                MessageBox.Show("No IAR record was updated. Please verify the selected IAR.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                            End If
                        End Using
                    End Using

                End If
            End If

        Catch ex As Exception
            MessageBox.Show("Import / open failed: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub



    Private Sub LoadInspectors()
        Dim connStr As String = My.Forms.frmmain.txtdb.Text ' Or your connection string
        Using conn As New SqlConnection(connStr)
            conn.Open()
            Dim sql As String = "SELECT CNAMES FROM TBL_APPROVER WHERE TYPE = 'Inspection Committee'"
            Using cmd As New SqlCommand(sql, conn)
                Using rdr As SqlDataReader = cmd.ExecuteReader()
                    comboinspector.Items.Clear()
                    While rdr.Read()
                        Dim raw As String = rdr("CNAMES").ToString()
                        Dim display As String = raw.Replace(vbCrLf, " - ") ' or " | "
                        comboinspector.Items.Add(display)
                    End While
                End Using
            End Using
        End Using
    End Sub


    Private Sub LoadCustodians()
        Dim connStr As String = My.Forms.frmmain.txtdb.Text ' Or your connection string
        Using conn As New SqlConnection(connStr)
            conn.Open()
            Dim sql As String = "SELECT CNAMES FROM TBL_APPROVER WHERE TYPE = 'Inventory Committee'"
            Using cmd As New SqlCommand(sql, conn)
                Using rdr As SqlDataReader = cmd.ExecuteReader()
                    comboscustodian.Items.Clear()
                    While rdr.Read()
                        Dim raw As String = rdr("CNAMES").ToString()
                        Dim display As String = raw.Replace(vbCrLf, " - ") ' or " | "
                        comboscustodian.Items.Add(display)
                    End While
                End Using
            End Using
        End Using
    End Sub

    Private Sub Cbpartial_CheckedChanged(sender As Object, e As EventArgs) Handles cbpartial.CheckedChanged
        If cbpartial.Checked = True Then cbcomplete.Checked = False
    End Sub

    Private Sub Cbcomplete_CheckedChanged(sender As Object, e As EventArgs) Handles cbcomplete.CheckedChanged
        If cbcomplete.Checked = True Then cbpartial.Checked = False
    End Sub

    Private Sub comboinspector_KeyPress(sender As Object, e As KeyPressEventArgs) Handles comboinspector.KeyPress
        e.Handled = True
    End Sub

    Private Sub comboscustodian_KeyPress(sender As Object, e As KeyPressEventArgs) Handles comboscustodian.KeyPress
        e.Handled = True
    End Sub

    Private Sub Comboinspector_SelectedIndexChanged(sender As Object, e As EventArgs) Handles comboinspector.SelectedIndexChanged

    End Sub

    Private Sub Txtttype_TextChanged_1(sender As Object, e As EventArgs) Handles txtttype.TextChanged

    End Sub

    Private Sub txtttype_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtttype.KeyPress
        e.Handled = True
    End Sub
End Class
