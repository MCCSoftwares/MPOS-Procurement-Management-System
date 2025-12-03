Imports System.Data.SqlClient
Imports System.IO
Imports Microsoft.Reporting.WinForms
Imports System.Text.RegularExpressions
Imports System.Globalization

Public Class frmabstractfile

    ' Reuse one bold font (avoid creating many Font objects)
    Private _boldCellFont As Font
    ' --- REPLACE THIS WHOLE SUB ---
    Private Sub Frmabstractfile_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            With DataGridView1
                .DataSource = Nothing
                .Rows.Clear()
                .Columns.Clear()
                .AutoGenerateColumns = False
                .RowHeadersVisible = False
                .AllowUserToAddRows = False
                .AllowUserToOrderColumns = False
                .AllowUserToResizeRows = False
                .MultiSelect = True
                .SelectionMode = DataGridViewSelectionMode.FullRowSelect
                .EditMode = DataGridViewEditMode.EditOnEnter   ' <— important
                .ReadOnly = False                               ' <— grid is editable

                ' Flat, borderless look
                .BorderStyle = BorderStyle.None
                .CellBorderStyle = DataGridViewCellBorderStyle.None
                .ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
                .AdvancedCellBorderStyle.Left = DataGridViewAdvancedCellBorderStyle.None
                .AdvancedCellBorderStyle.Right = DataGridViewAdvancedCellBorderStyle.None
                .AdvancedCellBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.None
                .AdvancedCellBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.None
            End With

            ' ID
            Dim colId As New DataGridViewTextBoxColumn With {
            .Name = "ID", .HeaderText = "ID", .DataPropertyName = "ID", .Visible = False, .ReadOnly = True
        }

            ' COMPANY (read-only)
            Dim colCompany As New DataGridViewTextBoxColumn With {
            .Name = "COMPANY", .HeaderText = "COMPANY",
            .DataPropertyName = "COMPANY",
            .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
            .ReadOnly = True
        }

            ' BIDS (read-only)
            Dim colBids As New DataGridViewTextBoxColumn With {
            .Name = "BIDS", .HeaderText = "BIDS",
            .DataPropertyName = "BIDS", .Width = 140,
            .DefaultCellStyle = New DataGridViewCellStyle With {.Format = "N2", .Alignment = DataGridViewContentAlignment.MiddleCenter},
            .ReadOnly = True
        }
            colBids.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter

            ' REMARKS (editable ComboBox)
            Dim colRemarks As New DataGridViewComboBoxColumn With {
            .Name = "REMARKS",
            .HeaderText = "REMARKS",
            .DataPropertyName = "REMARKS",              ' bound to DB column
            .DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton,
            .DisplayStyleForCurrentCellOnly = True,
            .FlatStyle = FlatStyle.Standard,
            .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
            .ReadOnly = False
        }
            colRemarks.Items.AddRange(
            "Lowest Calculated Responsive Bidder",                                           ' maps to "Empty"
            "Responsive Bidder",
            "Non-Responsive",
            "Above ABC",
            ""
        )
            colRemarks.DefaultCellStyle.NullValue = ""        ' normalize NULL to ""

            DataGridView1.Columns.AddRange({colId, colCompany, colBids, colRemarks})

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

            If lblid.Text <> "" Then LoadAbstract()

            LoadAOBApprovers()

        Catch ex As Exception
            MessageBox.Show("Form setup error: " & ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


    Private Sub LoadAOBApprovers()
        Try
            Using conn As New SqlConnection(frmmain.txtdb.Text)
                conn.Open()

                ' local helper: fill any ComboBox by TYPE without losing current text/selection
                Dim FillCombo As Action(Of ComboBox, String) =
            Sub(cbo As ComboBox, typeVal As String)
                Dim prevText As String = If(cbo.Text, "").Trim()
                Dim seen As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

                cbo.BeginUpdate()
                Dim newItems As New List(Of String)

                Using cmd As New SqlCommand("SELECT CNAMES FROM TBL_APPROVER WHERE TYPE = @t ORDER BY CNAMES", conn)
                    cmd.Parameters.Add("@t", SqlDbType.NVarChar, 100).Value = typeVal
                    Using rdr As SqlDataReader = cmd.ExecuteReader()
                        While rdr.Read()
                            Dim raw As String = If(rdr("CNAMES")?.ToString(), "").Trim()
                            If raw.Length = 0 Then Continue While
                            ' Normalize any newline variant to " | "
                            Dim display As String = raw.Replace(vbCrLf, " | ").Replace(vbLf, " | ").Replace(vbCr, " | ").Replace("  ", " ").Trim()
                            If seen.Add(display) Then newItems.Add(display)
                        End While
                    End Using
                End Using

                cbo.Items.Clear()
                If newItems.Count > 0 Then cbo.Items.AddRange(newItems.ToArray())

                ' If previous text isn't present, keep it by adding and selecting it
                If prevText.Length > 0 Then
                    Dim idx As Integer = cbo.FindStringExact(prevText)
                    If idx < 0 Then
                        cbo.Items.Insert(0, prevText)
                        idx = 0
                    End If
                    cbo.SelectedIndex = idx
                Else
                    ' keep whatever default behavior you prefer; do nothing = no selection change
                End If

                ' Keep style as-is (don’t force DropDownList which would clear non-list text)
                cbo.EndUpdate()
            End Sub

                ' Refresh the three combos without clearing their current text
                FillCombo(comboenduser, "AOB: End User Heads")
                FillCombo(comboshead, "AOB: Section Heads")
                FillCombo(comboapproval, "AOB: Approval")
            End Using

        Catch ex As Exception
            MessageBox.Show("Error loading approvers: " & ex.Message, "Approvers", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub








    ' Open the dropdown as soon as the user clicks/enters the REMARKS cell
    Private Sub DataGridView1_CellEnter(sender As Object, e As DataGridViewCellEventArgs) _
    Handles DataGridView1.CellEnter
        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Exit Sub
        If DataGridView1.Columns(e.ColumnIndex).Name = "REMARKS" Then
            DataGridView1.BeginEdit(True)
            ' Wait until the editing control exists, then drop down
            BeginInvoke(Sub()
                            Dim cb = TryCast(DataGridView1.EditingControl, ComboBox)
                            If cb IsNot Nothing Then cb.DroppedDown = True
                        End Sub)
        End If
    End Sub

    ' Commit immediately when user changes selection in the ComboBox
    Private Sub DataGridView1_CurrentCellDirtyStateChanged(sender As Object, e As EventArgs) _
    Handles DataGridView1.CurrentCellDirtyStateChanged
        If DataGridView1.IsCurrentCellDirty AndAlso
       TypeOf DataGridView1.CurrentCell Is DataGridViewComboBoxCell Then
            DataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit)
        End If
    End Sub

    ' Auto-save to DB when the REMARKS value changes
    Private Sub DataGridView1_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) _
    Handles DataGridView1.CellValueChanged
        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Exit Sub
        If DataGridView1.Columns(e.ColumnIndex).Name <> "REMARKS" Then Exit Sub

        Dim row = DataGridView1.Rows(e.RowIndex)
        Dim idVal = row.Cells("ID").Value
        If idVal Is Nothing OrElse idVal Is DBNull.Value Then Exit Sub

        Dim itemId As Integer = CInt(idVal)
        Dim remark As String = If(row.Cells("REMARKS").Value, "").ToString()

        SaveRemarkForRow(itemId, remark)
    End Sub

    ' Tolerate legacy/null values (prevents blocking the editor)
    Private Sub DataGridView1_DataError(sender As Object, e As DataGridViewDataErrorEventArgs) _
    Handles DataGridView1.DataError
        e.ThrowException = False
    End Sub

    ' Normalize nulls to "" so ComboBox matches an item
    Private Sub DataGridView1_DataBindingComplete(sender As Object, e As DataGridViewBindingCompleteEventArgs) _
    Handles DataGridView1.DataBindingComplete
        For Each r As DataGridViewRow In DataGridView1.Rows
            If r.Cells("REMARKS").Value Is Nothing OrElse r.Cells("REMARKS").Value Is DBNull.Value Then
                r.Cells("REMARKS").Value = ""
            End If
        Next
    End Sub

    ' Tiny helper that performs the DB update
    Private Sub SaveRemarkForRow(itemId As Integer, remark As String)
        Try
            Using conn As New SqlClient.SqlConnection(My.Forms.frmmain.txtdb.Text)
                conn.Open()
                Using cmd As New SqlClient.SqlCommand(
                "UPDATE TBL_ABSTRACTITEMS SET REMARKS = @r WHERE ID = @id;", conn)
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = itemId
                    Dim p = cmd.Parameters.Add("@r", SqlDbType.NVarChar, 500)
                    If String.IsNullOrWhiteSpace(remark) Then
                        p.Value = DBNull.Value
                    Else
                        p.Value = remark
                    End If
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Failed to save remark: " & ex.Message, "Save Remark",
                        MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' Reuse your existing helpers
    Private Sub EnableDoubleBuffering(dgv As DataGridView)
        Dim t = dgv.GetType()
        Dim pi = t.GetProperty("DoubleBuffered", Reflection.BindingFlags.Instance Or Reflection.BindingFlags.NonPublic)
        If pi IsNot Nothing Then pi.SetValue(dgv, True, Nothing)
    End Sub

    '======================== NEW HELPERS (Company headers + row shading) ========================

    ' Extract "Company Name" from a label that may look like "Acme Corp : 123,456.00"
    Private Function CompanyNameOnlyFromLabel(lbl As Label) As String
        Dim raw As String = If(lbl Is Nothing OrElse lbl.Text Is Nothing, "", lbl.Text.Trim())
        If String.IsNullOrWhiteSpace(raw) Then Return "NO COMPANY"
        Dim parts = raw.Split(":"c)
        Dim name = parts(0).Trim()
        If String.IsNullOrWhiteSpace(name) Then name = "NO COMPANY"
        Return name
    End Function

    ' Set the headers for COMPANY A/B/C based on lbltop1/2/3 (with fallback)


    ' Safe decimal try-parse returning Nullable
    Private Function SafeDec(v As Object) As Decimal?
        If v Is Nothing OrElse v Is DBNull.Value Then Return Nothing
        Dim d As Decimal
        If Decimal.TryParse(v.ToString(), d) Then Return d
        Return Nothing
    End Function

    Public Sub LoadAbstract()
        Try
            ' Reset grid binding
            With DataGridView1
                .DataSource = Nothing
                .Refresh()
            End With

            ' Validate Abstract ID
            Dim absId As Integer
            If Not Integer.TryParse(lblid.Text.Trim(), absId) Then
                ' Nothing to load yet
                Return
            End If

            ' ─────────────────────────────────────────────
            ' 1) Load HEADER from TBL_ABSTRACT
            ' ─────────────────────────────────────────────
            Using conn As New SqlClient.SqlConnection(My.Forms.frmmain.txtdb.Text)
                conn.Open()

                Const hdrSql As String = "
SELECT
    ABS_ABSNO,      -- AOBRNumber
    ABS_PRNO,
    ABS_PRID,
    ABS_DABSTRACT,
    ABS_RFQID,
    ABS_STATUS,
    ABS_PURPOSE,
    ABS_MPROC,
    ABS_EUSER,
    ABS_BACRESO,
    ABS_SIGEUSER,
    ABS_SIGSHEAD,
    ABS_SIGAPP,
    ABS_APPBUDGET,
    ABS_ACOMPANY, ABS_ATOTAL,
    ABS_BCOMPANY, ABS_BTOTAL,
    ABS_CCOMPANY, ABS_CTOTAL,
    ABS_AWARDEE, ABS_TTYPE
    -- Optional deadline columns (one may exist)
    -- ABS_DEADLINE or ABS_DDEADLINE
FROM TBL_ABSTRACT
WHERE ID = @absid;"

                Using cmd As New SqlClient.SqlCommand(hdrSql, conn)
                    cmd.Parameters.Add("@absid", SqlDbType.Int).Value = absId
                    Using rdr = cmd.ExecuteReader()
                        If rdr.Read() Then
                            ' Safe helpers
                            Dim GetStr = Function(name As String) As String
                                             Dim ord As Integer = -1
                                             Try : ord = rdr.GetOrdinal(name) : Catch : ord = -1 : End Try
                                             If ord < 0 OrElse rdr.IsDBNull(ord) Then Return ""
                                             Return rdr.GetString(ord).Trim()
                                         End Function
                            Dim GetDec = Function(name As String) As Decimal?
                                             Dim ord As Integer = -1
                                             Try : ord = rdr.GetOrdinal(name) : Catch : ord = -1 : End Try
                                             If ord < 0 OrElse rdr.IsDBNull(ord) Then Return Nothing
                                             Return rdr.GetDecimal(ord)
                                         End Function
                            Dim GetDateVal = Function(name As String) As Date?
                                                 Dim ord As Integer = -1
                                                 Try : ord = rdr.GetOrdinal(name) : Catch : ord = -1 : End Try
                                                 If ord < 0 OrElse rdr.IsDBNull(ord) Then Return Nothing
                                                 Return CDate(rdr.GetValue(ord))
                                             End Function
                            Dim HasCol = Function(name As String) As Boolean
                                             For i = 0 To rdr.FieldCount - 1
                                                 If String.Equals(rdr.GetName(i), name, StringComparison.OrdinalIgnoreCase) Then Return True
                                             Next
                                             Return False
                                         End Function

                            ' Map to controls (all your newly added)
                            txtaobrno.Text = GetStr("ABS_ABSNO")                    ' AOBRNumber
                            txtprno.Text = GetStr("ABS_PRNO")
                            lblprid.Text = rdr("ABS_PRID")
                            lblrfqid.Text = rdr("ABS_RFQID")
                            txtstatus.Text = GetStr("ABS_STATUS")
                            txtpurpose.Text = GetStr("ABS_PURPOSE")
                            txtmproc.Text = GetStr("ABS_MPROC")
                            txteuser.Text = GetStr("ABS_EUSER")
                            txtbacreso.Text = GetStr("ABS_BACRESO")
                            LBLTTYPE.Text = GetStr("ABS_TTYPE")
                            ' Signatories
                            comboenduser.Text = GetStr("ABS_SIGEUSER")
                            comboshead.Text = GetStr("ABS_SIGSHEAD")
                            comboapproval.Text = GetStr("ABS_SIGAPP")

                            ' Budget
                            Dim bud = GetDec("ABS_APPBUDGET")
                            If bud.HasValue Then
                                lblappbudget.Text = bud.Value.ToString("N2")
                            Else
                                lblappbudget.Text = ""
                            End If

                            ' Dates
                            Dim dabs = GetDateVal("ABS_DABSTRACT")
                            If dabs.HasValue Then
                                Dim ctrlDabs = Me.Controls.Find("DTDAbs", True).FirstOrDefault()
                                If ctrlDabs IsNot Nothing AndAlso TypeOf ctrlDabs Is DateTimePicker Then
                                    DirectCast(ctrlDabs, DateTimePicker).Value = dabs.Value
                                End If
                            End If

                            ' Optional deadline column (ABS_DEADLINE or ABS_DDEADLINE)
                            Dim dline As Date? = Nothing
                            If HasCol("ABS_DEADLINE") Then dline = GetDateVal("ABS_DEADLINE")
                            If Not dline.HasValue AndAlso HasCol("ABS_DDEADLINE") Then dline = GetDateVal("ABS_DDEADLINE")
                            If dline.HasValue Then
                                Dim ctrlDeadline = Me.Controls.Find("dtdeadline", True).FirstOrDefault()
                                If ctrlDeadline IsNot Nothing AndAlso TypeOf ctrlDeadline Is DateTimePicker Then
                                    DirectCast(ctrlDeadline, DateTimePicker).Value = dline.Value
                                End If
                            End If
                        Else
                            MessageBox.Show("Abstract header not found.", "Load", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            Return
                        End If
                    End Using
                End Using

                ' ─────────────────────────────────────────────
                ' 2) Load ITEMS from TBL_ABSTRACTITEMS
                ' ─────────────────────────────────────────────
                Dim dt As New DataTable()
                Const itemsSql As String = "
                    SELECT 
                        ID,
                        COMPANY,
                        BIDS,
                        REMARKS
                    FROM TBL_ABSTRACTITEMS WITH (NOLOCK)
                    WHERE ABSID = @absid
                    ORDER BY BIDS ASC, COMPANY ASC;"

                Using da As New SqlClient.SqlDataAdapter(itemsSql, conn)
                    da.SelectCommand.Parameters.Add("@absid", SqlDbType.Int).Value = absId
                    da.Fill(dt)
                End Using

                ' Bind
                DataGridView1.AutoGenerateColumns = False
                DataGridView1.DataSource = dt
            End Using

            ' Title (show AOBRNumber + PR)
            lbltitle.Text = $"ABSTRACT OF BIDS | AOBR #: {txtaobrno.Text.Trim()} | PR #: {txtprno.Text.Trim()}"

            ' Buttons state
            Dim st As String = If(txtstatus.Text, String.Empty).Trim()
            Dim isCompleted As Boolean = st.Equals("Completed", StringComparison.OrdinalIgnoreCase)
            Dim isActive As Boolean = st.Equals("Pending", StringComparison.OrdinalIgnoreCase) OrElse
                                  st.Equals("Processing", StringComparison.OrdinalIgnoreCase) OrElse
                                  st = String.Empty

            cmdrefresh.Enabled = isActive
            cmdsave.Enabled = isActive
            cmdsubmit.Enabled = isActive
            cmdscancel.Enabled = isCompleted
            DataGridView1.Enabled = isActive
            DTDAbs.Enabled = False

        Catch ex As Exception
            MessageBox.Show("Error loading abstract: " & ex.ToString,
                        "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub



    Private Sub txtprno_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtprno.KeyPress
        e.Handled = True
    End Sub

    Private Sub txtdabstract_KeyPress(sender As Object, e As KeyPressEventArgs)
        e.Handled = True
    End Sub

    Private Sub txtstatus_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtstatus.KeyPress
        e.Handled = True
    End Sub

    Private Sub txtpurpose_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtpurpose.KeyPress
        e.Handled = True
    End Sub

    Private Sub LockCombo(combo As ComboBox)
        ' Prevent typing
        AddHandler combo.KeyPress, Sub(s, e) e.Handled = True
        AddHandler combo.KeyDown, Sub(s, e) e.SuppressKeyPress = True

        ' Prevent mouse opening of the drop‑down
        AddHandler combo.MouseDown,
            Sub(s, e)
                Dim cb = DirectCast(s, ComboBox)
                cb.DroppedDown = False
            End Sub

        ' If something (Alt+Down, code, etc.) tries to open it:
        AddHandler combo.DropDown,
            Sub(s, e)
                DirectCast(s, ComboBox).DroppedDown = False
            End Sub
    End Sub

    Public Sub SaveAbstract()
        Try
            ' ─────────────────────────────────────────────────────────
            ' 0) Gather + validate inputs
            ' ─────────────────────────────────────────────────────────
            Dim prno As String = txtprno.Text.Trim()
            Dim purposeText As String = txtpurpose.Text.Trim()
            Dim statusText As String = If(String.IsNullOrWhiteSpace(txtstatus.Text), "Pending", txtstatus.Text.Trim())

            If String.IsNullOrWhiteSpace(prno) _
        OrElse String.IsNullOrWhiteSpace(lblprid.Text) _
        OrElse String.IsNullOrWhiteSpace(lblrfqid.Text) _
        OrElse String.IsNullOrWhiteSpace(purposeText) Then
                MessageBox.Show("Please complete all required fields (PR No, PR ID, RFQ ID, Purpose).",
                            "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Dim prId As Integer, rfqId As Integer
            If Not Integer.TryParse(lblprid.Text.Trim(), prId) Then
                MessageBox.Show("Invalid PR ID.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning) : Return
            End If
            If Not Integer.TryParse(lblrfqid.Text.Trim(), rfqId) Then
                MessageBox.Show("Invalid RFQ ID.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning) : Return
            End If

            ' mproc: support either txtmproc or combomproc
            Dim mprocValue As String = ""
            Dim ctrlMprocTxt = Me.Controls.Find("txtmproc", True).FirstOrDefault()
            If TypeOf ctrlMprocTxt Is TextBox Then
                mprocValue = DirectCast(ctrlMprocTxt, TextBox).Text.Trim()
            Else
                Dim ctrlMprocCombo = Me.Controls.Find("combomproc", True).FirstOrDefault()
                If TypeOf ctrlMprocCombo Is ComboBox Then mprocValue = DirectCast(ctrlMprocCombo, ComboBox).Text.Trim()
            End If

            Dim euserValue As String = ""
            Dim ctrlEuser = Me.Controls.Find("txteuser", True).FirstOrDefault()
            If TypeOf ctrlEuser Is TextBox Then euserValue = DirectCast(ctrlEuser, TextBox).Text.Trim()

            Dim bacReso As String = ""
            Dim ctrlBacReso = Me.Controls.Find("txtbacreso", True).FirstOrDefault()
            If TypeOf ctrlBacReso Is TextBox Then bacReso = DirectCast(ctrlBacReso, TextBox).Text.Trim()

            Dim sigEndUser As String = ""
            Dim ctrlSigEU = Me.Controls.Find("comboenduser", True).FirstOrDefault()
            If TypeOf ctrlSigEU Is ComboBox Then sigEndUser = DirectCast(ctrlSigEU, ComboBox).Text.Trim()

            Dim sigSHead As String = ""
            Dim ctrlSigSH = Me.Controls.Find("comboshead", True).FirstOrDefault()
            If TypeOf ctrlSigSH Is ComboBox Then sigSHead = DirectCast(ctrlSigSH, ComboBox).Text.Trim()

            Dim sigApproval As String = ""
            Dim ctrlSigApp = Me.Controls.Find("comboapproval", True).FirstOrDefault()
            If TypeOf ctrlSigApp Is ComboBox Then sigApproval = DirectCast(ctrlSigApp, ComboBox).Text.Trim()

            ' ABS_APPBUDGET from lblappbudget
            Dim appBudget As Decimal? = Nothing
            Dim ctrlBudget = Me.Controls.Find("lblappbudget", True).FirstOrDefault()
            If TypeOf ctrlBudget Is Label Then
                Dim raw = DirectCast(ctrlBudget, Label).Text
                Dim cleaned = New String(raw.Where(Function(ch) Char.IsDigit(ch) OrElse ch = "."c OrElse ch = "-"c).ToArray())
                Dim tempDec As Decimal
                If Decimal.TryParse(cleaned, Globalization.NumberStyles.Any, Globalization.CultureInfo.InvariantCulture, tempDec) Then
                    appBudget = tempDec
                End If
            End If

            ' Dates
            Dim dabsDate As Date = Date.Today
            Dim ctrlDabs = Me.Controls.Find("DTDAbs", True).FirstOrDefault()
            If TypeOf ctrlDabs Is DateTimePicker Then dabsDate = DirectCast(ctrlDabs, DateTimePicker).Value.Date

            Dim deadlineDate As Date? = Nothing
            Dim ctrlDeadline = Me.Controls.Find("dtdeadline", True).FirstOrDefault()
            If TypeOf ctrlDeadline Is DateTimePicker Then deadlineDate = DirectCast(ctrlDeadline, DateTimePicker).Value.Date

            ' ─────────────────────────────────────────────────────────
            ' 1) DB: generate ABS_ABSNO (AOBR-YYYY-<running number for YEAR>)
            '    + compute Top 3 suppliers
            ' ─────────────────────────────────────────────────────────
            Dim absAbsNo As String = ""
            Dim newID As Integer
            Dim comp1 As String = Nothing, comp2 As String = Nothing, comp3 As String = Nothing
            Dim tot1 As Decimal? = Nothing, tot2 As Decimal? = Nothing, tot3 As Decimal? = Nothing

            Using conn As New SqlClient.SqlConnection(My.Forms.frmmain.txtdb.Text)
                conn.Open()
                Using tran = conn.BeginTransaction()

                    ' ---- (1a) Concurrency-safe yearly sequence generation ----
                    ' Lock scope per year to prevent duplicates with many users saving at once
                    Using lockCmd As New SqlClient.SqlCommand("EXEC sp_getapplock @Resource=@res, @LockMode='Exclusive', @LockTimeout=5000, @LockOwner='Transaction';", conn, tran)
                        lockCmd.Parameters.Add("@res", SqlDbType.NVarChar, 100).Value = "AOBR-" & dabsDate.ToString("yyyy")
                        lockCmd.ExecuteNonQuery()
                    End Using

                    Dim prefix As String = $"AOBR-{dabsDate:yyyy}-"  ' correct prefix
                    Dim padWidth As Integer = 4                     ' 0001, 0002, ...
                    Dim yearMax As Integer = 0

                    ' Find the highest series for this year (rows that start with AOBR-YYYY-)
                    Using seqCmd As New SqlClient.SqlCommand("
                    SELECT ISNULL(MAX(TRY_CONVERT(int, PARSENAME(REPLACE(ABS_ABSNO,'-','.'),1))),0)
                    FROM TBL_ABSTRACT WITH (UPDLOCK, HOLDLOCK)
                    WHERE ABS_ABSNO LIKE @pfx + '%';", conn, tran)
                        seqCmd.Parameters.Add("@pfx", SqlDbType.NVarChar, 32).Value = prefix
                        yearMax = CInt(seqCmd.ExecuteScalar())
                    End Using

                    Dim nextSeq As Integer = yearMax + 1
                    absAbsNo = prefix & nextSeq.ToString("D" & padWidth)   ' AOBR-YYYY-0001

                    ' push to UI immediately
                    Dim aobCtrl = Me.Controls.Find("txtaobrno", True).FirstOrDefault()
                    If TypeOf aobCtrl Is TextBox Then DirectCast(aobCtrl, TextBox).Text = absAbsNo

                    ' ---- (1b) Compute Top 3 suppliers by SUM(RFQ_TCOST) for this PR ----
                    Using topCmd As New SqlClient.SqlCommand("
                        ;WITH Totals AS(
                            SELECT RFQSID, SUM(RFQ_TCOST) AS TotalCost
                              FROM TBL_RFQITEMS
                             WHERE PRID = @prid
                             GROUP BY RFQSID
                        ),
                        Ranked AS(
                            SELECT RFQSID, TotalCost,
                                   ROW_NUMBER() OVER(ORDER BY TotalCost ASC, RFQSID ASC) AS rn
                              FROM Totals
                        )
                        SELECT r.RFQSID, r.TotalCost, s.RFQ_NCOMPANY
                          FROM Ranked r
                          LEFT JOIN TBL_RFQSUPPLIER s ON s.ID = r.RFQSID
                         WHERE r.rn <= 3
                         ORDER BY r.rn;", conn, tran)
                        topCmd.Parameters.Add("@prid", SqlDbType.Int).Value = prId
                        Using rdr = topCmd.ExecuteReader()
                            Dim i As Integer = 0
                            While rdr.Read()
                                i += 1
                                Dim nm As String = If(rdr.IsDBNull(2), "", rdr.GetString(2))
                                Dim tot As Decimal = If(rdr.IsDBNull(1), 0D, rdr.GetDecimal(1))
                                If i = 1 Then comp1 = nm : tot1 = tot
                                If i = 2 Then comp2 = nm : tot2 = tot
                                If i = 3 Then comp3 = nm : tot3 = tot
                            End While
                        End Using
                    End Using

                    ' ---- (1c) Optional deadline column detection ----
                    Dim deadlineCol As String = Nothing
                    Using colCmd As New SqlClient.SqlCommand("
                    SELECT COLUMN_NAME
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_NAME = 'TBL_ABSTRACT'
                      AND COLUMN_NAME IN ('ABS_DEADLINE','ABS_DDEADLINE');", conn, tran)
                        Dim obj = colCmd.ExecuteScalar()
                        If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then deadlineCol = obj.ToString()
                    End Using

                    Dim ttype As String = LBLTTYPE.Text

                    ' ---- (1d) INSERT header ----
                    Dim cols As New List(Of String) From {
                    "ABS_ABSNO", "ABS_PRNO", "ABS_PRID", "ABS_DABSTRACT", "ABS_RFQID",
                    "ABS_STATUS", "ABS_PURPOSE", "ABS_MPROC", "ABS_EUSER", "ABS_BACRESO",
                    "ABS_SIGEUSER", "ABS_SIGSHEAD", "ABS_SIGAPP", "ABS_APPBUDGET",
                    "ABS_ACOMPANY", "ABS_ATOTAL", "ABS_BCOMPANY", "ABS_BTOTAL",
                    "ABS_CCOMPANY", "ABS_CTOTAL", "ABS_AWARDEE", "ABS_TTYPE"
                }
                    Dim vals As New List(Of String) From {
                    "@absno", "@prno", "@prid", "@dabs", "@rfqid",
                    "@status", "@purpose", "@mproc", "@euser", "@bacreso",
                    "@sigeu", "@sigshead", "@sigapp", "@appbudget",
                    "@comp1", "@tot1", "@comp2", "@tot2", "@comp3", "@tot3", "@awardee", "@ttype"
                }
                    If deadlineCol IsNot Nothing AndAlso deadlineDate.HasValue Then
                        cols.Add(deadlineCol) : vals.Add("@deadline")
                    End If

                    Dim insertSql As String =
                    "INSERT INTO TBL_ABSTRACT (" & String.Join(",", cols) & ") " &
                    "VALUES (" & String.Join(",", vals) & "); SELECT SCOPE_IDENTITY();"

                    Using cmd As New SqlClient.SqlCommand(insertSql, conn, tran)
                        cmd.Parameters.Add("@absno", SqlDbType.NVarChar, 50).Value = absAbsNo
                        cmd.Parameters.Add("@prno", SqlDbType.NVarChar, 50).Value = prno
                        cmd.Parameters.Add("@prid", SqlDbType.Int).Value = prId
                        cmd.Parameters.Add("@dabs", SqlDbType.Date).Value = dabsDate
                        cmd.Parameters.Add("@rfqid", SqlDbType.Int).Value = rfqId
                        cmd.Parameters.Add("@status", SqlDbType.NVarChar, 50).Value = statusText
                        cmd.Parameters.Add("@purpose", SqlDbType.NVarChar, 500).Value = purposeText
                        cmd.Parameters.Add("@mproc", SqlDbType.NVarChar, 150).Value = mprocValue
                        cmd.Parameters.Add("@euser", SqlDbType.NVarChar, 150).Value = euserValue
                        cmd.Parameters.Add("@bacreso", SqlDbType.NVarChar, 500).Value = bacReso
                        cmd.Parameters.Add("@sigeu", SqlDbType.NVarChar, 200).Value = sigEndUser
                        cmd.Parameters.Add("@sigshead", SqlDbType.NVarChar, 200).Value = sigSHead
                        cmd.Parameters.Add("@sigapp", SqlDbType.NVarChar, 200).Value = sigApproval
                        cmd.Parameters.Add("@ttype", SqlDbType.NVarChar, 200).Value = ttype

                        Dim pBudget = cmd.Parameters.Add("@appbudget", SqlDbType.Decimal) : pBudget.Precision = 18 : pBudget.Scale = 2
                        pBudget.Value = If(appBudget.HasValue, CType(appBudget.Value, Object), DBNull.Value)

                        ' Top 3 + awardee
                        cmd.Parameters.Add("@comp1", SqlDbType.NVarChar, 255).Value = If(String.IsNullOrWhiteSpace(comp1), CType(DBNull.Value, Object), comp1)
                        Dim pTot1 = cmd.Parameters.Add("@tot1", SqlDbType.Decimal) : pTot1.Precision = 18 : pTot1.Scale = 2
                        pTot1.Value = If(tot1.HasValue, CType(tot1.Value, Object), DBNull.Value)

                        cmd.Parameters.Add("@comp2", SqlDbType.NVarChar, 255).Value = If(String.IsNullOrWhiteSpace(comp2), CType(DBNull.Value, Object), comp2)
                        Dim pTot2 = cmd.Parameters.Add("@tot2", SqlDbType.Decimal) : pTot2.Precision = 18 : pTot2.Scale = 2
                        pTot2.Value = If(tot2.HasValue, CType(tot2.Value, Object), DBNull.Value)

                        cmd.Parameters.Add("@comp3", SqlDbType.NVarChar, 255).Value = If(String.IsNullOrWhiteSpace(comp3), CType(DBNull.Value, Object), comp3)
                        Dim pTot3 = cmd.Parameters.Add("@tot3", SqlDbType.Decimal) : pTot3.Precision = 18 : pTot3.Scale = 2
                        pTot3.Value = If(tot3.HasValue, CType(tot3.Value, Object), DBNull.Value)

                        cmd.Parameters.Add("@awardee", SqlDbType.NVarChar, 255).Value = If(String.IsNullOrWhiteSpace(comp1), CType(DBNull.Value, Object), comp1)

                        If deadlineCol IsNot Nothing AndAlso deadlineDate.HasValue Then
                            cmd.Parameters.Add("@deadline", SqlDbType.Date).Value = deadlineDate.Value
                        End If

                        newID = Convert.ToInt32(cmd.ExecuteScalar())
                    End Using

                    Using cmdPr As New SqlClient.SqlCommand(
                    "UPDATE TBL_PR
                       SET PRSTATUS = '[AOB] Processing'
                     WHERE ID = @prid;", conn, tran)
                        cmdPr.Parameters.Add("@prid", SqlDbType.Int).Value = prId
                        cmdPr.ExecuteNonQuery()
                    End Using

                    tran.Commit()
                End Using
            End Using

            ' ─────────────────────────────────────────────────────────
            ' 2) UI & notify
            ' ─────────────────────────────────────────────────────────
            lblid.Text = newID.ToString()
            txtstatus.Text = statusText
            cmdrefresh.Enabled = True
            cmdprint.Enabled = True
            cmdsubmit.Enabled = True
            cmdscancel.Enabled = False
            cmdsave.Text = "Update"
            cmdrefresh.PerformClick()

            NotificationHelper.AddNotification(
            $"Abstract of Bids No. {absAbsNo} for PR Number {prno} has been created by {frmmain.lblaname.Text.Trim()} and is now in process."
        )
            MessageBox.Show("Abstract saved successfully.", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information)

            If Application.OpenForms().OfType(Of frmabstract).Any() Then
                My.Forms.frmabstract.LoadAbstract()
            End If

        Catch ex As Exception
            MessageBox.Show("Save error: " & ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub





    ' Example button handler
    Private Sub cmdsave_Click(sender As Object, e As EventArgs) Handles cmdsave.Click
        If cmdsave.Text = "Save" Then
            SaveAbstract()
        Else
            UpdateAbstract()
        End If
    End Sub

    Public Sub UpdateAbstract()
        Try
            ' ✅ Basic validation (same spirit as your original)
            If String.IsNullOrWhiteSpace(lblid.Text) _
           OrElse String.IsNullOrWhiteSpace(txtprno.Text) _
           OrElse String.IsNullOrWhiteSpace(lblprid.Text) _
           OrElse String.IsNullOrWhiteSpace(lblrfqid.Text) _
           OrElse String.IsNullOrWhiteSpace(txtpurpose.Text) Then

                MessageBox.Show("Please complete all required fields.", "Validation",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            ' Pull values
            Dim absId As Integer = CInt(lblid.Text)
            Dim prId As Integer = CInt(lblprid.Text)
            Dim rfqId As Integer = CInt(lblrfqid.Text)
            Dim mproc As String = If(Not String.IsNullOrWhiteSpace(txtmproc.Text), txtmproc.Text.Trim(),
                                 If(txtmproc IsNot Nothing, txtmproc.Text.Trim(), ""))
            Dim euser As String = If(txteuser IsNot Nothing, txteuser.Text.Trim(), "")
            Dim bacReso As String = If(txtbacreso IsNot Nothing, txtbacreso.Text.Trim(), "")
            Dim sigEU As String = If(comboenduser IsNot Nothing, comboenduser.Text.Trim(), "")
            Dim sigSHead As String = If(comboshead IsNot Nothing, comboshead.Text.Trim(), "")
            Dim sigApp As String = If(comboapproval IsNot Nothing, comboapproval.Text.Trim(), "")
            Dim absNo As String = If(txtaobrno IsNot Nothing, txtaobrno.Text.Trim(), "")
            Dim statusText As String = If(String.IsNullOrWhiteSpace(txtstatus.Text), "Pending", txtstatus.Text.Trim())
            Dim dabs As Date = If(DTDAbs IsNot Nothing, DTDAbs.Value.Date, Date.Today)

            ' Parse ABS_APPBUDGET from label (e.g., "1,234.56")
            Dim appBudget As Decimal = 0D
            If lblappbudget IsNot Nothing Then
                Decimal.TryParse(lblappbudget.Text.Replace(",", "").Trim(),
                             Globalization.NumberStyles.Any,
                             Globalization.CultureInfo.InvariantCulture,
                             appBudget)
            End If

            Using conn As New SqlClient.SqlConnection(My.Forms.frmmain.txtdb.Text)
                conn.Open()

                Const sql As String = "
UPDATE TBL_ABSTRACT
   SET ABS_ABSNO     = @absno,
       ABS_PRNO      = @prno,
       ABS_PRID      = @prid,
       ABS_RFQID     = @rfqid,
       ABS_DABSTRACT = @dabs,
       ABS_STATUS    = @status,
       ABS_PURPOSE   = @purpose,
       ABS_MPROC     = @mproc,
       ABS_EUSER     = @euser,
       ABS_BACRESO   = @bacreso,
       ABS_SIGEUSER  = @sigeu,
       ABS_SIGSHEAD  = @sigshead,
       ABS_SIGAPP    = @sigapp,
       ABS_APPBUDGET = @appbudget
 WHERE ID            = @id;"

                Using cmd As New SqlClient.SqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@absno", absNo)
                    cmd.Parameters.AddWithValue("@prno", txtprno.Text.Trim())
                    cmd.Parameters.AddWithValue("@prid", prId)
                    cmd.Parameters.AddWithValue("@rfqid", rfqId)
                    cmd.Parameters.AddWithValue("@dabs", dabs)
                    cmd.Parameters.AddWithValue("@status", statusText)
                    cmd.Parameters.AddWithValue("@purpose", txtpurpose.Text.Trim())
                    cmd.Parameters.AddWithValue("@mproc", mproc)
                    cmd.Parameters.AddWithValue("@euser", euser)
                    cmd.Parameters.AddWithValue("@bacreso", bacReso)
                    cmd.Parameters.AddWithValue("@sigeu", sigEU)
                    cmd.Parameters.AddWithValue("@sigshead", sigSHead)
                    cmd.Parameters.AddWithValue("@sigapp", sigApp)
                    cmd.Parameters.AddWithValue("@appbudget", appBudget)
                    cmd.Parameters.AddWithValue("@id", absId)
                    cmd.ExecuteNonQuery()
                End Using
            End Using

            MessageBox.Show("Abstract updated successfully.", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information)

            NotificationHelper.AddNotification(
            $"Abstract of Bids for PR Number: {txtprno.Text.Trim()} has been updated by {frmmain.lblaname.Text.Trim()}."
        )

            If Application.OpenForms().OfType(Of frmabstract).Any() Then
                My.Forms.frmabstract.LoadAbstract()
            End If

        Catch ex As Exception
            MessageBox.Show("Update error: " & ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub




    ' =======================
    ' RefreshAbstract  (REPLACED / NEW LOGIC)
    ' Deletes existing rows for this ABSID,
    ' recomputes Top 3 suppliers by total cost,
    ' inserts 1 row per supplier into TBL_ABSTRACTITEMS,
    ' then updates TBL_ABSTRACT header summary.
    ' =======================
    Public Sub RefreshAbstract()
        Try
            ' 0) Validate keys
            If String.IsNullOrWhiteSpace(lblid.Text) _
       OrElse String.IsNullOrWhiteSpace(lblprid.Text) _
       OrElse String.IsNullOrWhiteSpace(lblrfqid.Text) Then

                MessageBox.Show("Please save/select the Abstract header first.", "Refresh Abstract",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Dim absId As Integer, prId As Integer, rfqId As Integer
            If Not Integer.TryParse(lblid.Text.Trim(), absId) Then
                MessageBox.Show("Invalid Abstract ID.", "Refresh Abstract",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning) : Return
            End If
            If Not Integer.TryParse(lblprid.Text.Trim(), prId) Then
                MessageBox.Show("Invalid PR ID.", "Refresh Abstract",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning) : Return
            End If
            If Not Integer.TryParse(lblrfqid.Text.Trim(), rfqId) Then
                MessageBox.Show("Invalid RFQ ID.", "Refresh Abstract",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning) : Return
            End If

            ' Get MPROC string (support combo or textbox, whichever you now use)
            Dim mprocValue As String = ""
            Dim c1() As Control = Me.Controls.Find("combomproc", True)
            If c1 IsNot Nothing AndAlso c1.Length > 0 AndAlso TypeOf c1(0) Is ComboBox Then
                mprocValue = DirectCast(c1(0), ComboBox).Text.Trim()
            End If
            If String.IsNullOrWhiteSpace(mprocValue) Then
                Dim c2() As Control = Me.Controls.Find("txtmproc", True)
                If c2 IsNot Nothing AndAlso c2.Length > 0 AndAlso TypeOf c2(0) Is TextBox Then
                    mprocValue = DirectCast(c2(0), TextBox).Text.Trim()
                End If
            End If

            Using conn As New SqlClient.SqlConnection(My.Forms.frmmain.txtdb.Text)
                conn.Open()
                Using tran = conn.BeginTransaction()

                    ' 1) Clear existing item rows for this abstract
                    Using delCmd As New SqlClient.SqlCommand(
                "DELETE FROM TBL_ABSTRACTITEMS WHERE ABSID = @absid;", conn, tran)
                        delCmd.Parameters.Add("@absid", SqlDbType.Int).Value = absId
                        delCmd.ExecuteNonQuery()
                    End Using

                    ' 2) Compute Top 3 suppliers by total RFQ cost for this PR
                    Dim tops As New List(Of (Sid As Integer, Tot As Decimal, Name As String))()

                    Using topCmd As New SqlClient.SqlCommand("
                    ;WITH Totals AS(
                        SELECT RFQSID, SUM(RFQ_TCOST) AS TotalCost
                          FROM TBL_RFQITEMS
                         WHERE PRID = @prid
                         GROUP BY RFQSID
                    ),
                    Ranked AS(
                        SELECT RFQSID, TotalCost,
                               ROW_NUMBER() OVER(ORDER BY TotalCost ASC, RFQSID ASC) AS rn
                          FROM Totals
                    )
                    SELECT r.RFQSID, r.TotalCost, s.RFQ_NCOMPANY
                      FROM Ranked r
                      LEFT JOIN TBL_RFQSUPPLIER s ON s.ID = r.RFQSID
                     WHERE r.rn <= 3
                     ORDER BY r.rn;", conn, tran)

                        topCmd.Parameters.Add("@prid", SqlDbType.Int).Value = prId

                        Using rdr = topCmd.ExecuteReader()
                            While rdr.Read()
                                Dim sid = If(rdr.IsDBNull(0), 0, rdr.GetInt32(0))
                                Dim tot = If(rdr.IsDBNull(1), 0D, rdr.GetDecimal(1))
                                Dim nm = If(rdr.IsDBNull(2), "", rdr.GetString(2))
                                If sid <> 0 Then tops.Add((sid, tot, nm))
                            End While
                        End Using
                    End Using

                    ' Map them to slots A/B/C
                    Dim s1Name As String = "", s2Name As String = "", s3Name As String = ""
                    Dim s1Tot As Decimal = 0D, s2Tot As Decimal = 0D, s3Tot As Decimal = 0D
                    If tops.Count > 0 Then s1Name = tops(0).Name : s1Tot = tops(0).Tot
                    If tops.Count > 1 Then s2Name = tops(1).Name : s2Tot = tops(1).Tot
                    If tops.Count > 2 Then s3Name = tops(2).Name : s3Tot = tops(2).Tot

                    ' 3) Insert rows into TBL_ABSTRACTITEMS
                    Const insertSql As String = "
                    INSERT INTO TBL_ABSTRACTITEMS
                     (PRID, ABSID, ABSNO, RFQID, PRNO, DABS, MPROC, PURPOSES, COMPANY, BIDS, REMARKS)
                    VALUES
                     (@prid, @absid, @absno, @rfqid, @prno, @dabs, @mproc, @purpose, @company, @bids, @remarks);"

                    Using insCmd As New SqlClient.SqlCommand(insertSql, conn, tran)
                        insCmd.Parameters.Add("@prid", SqlDbType.Int).Value = prId
                        insCmd.Parameters.Add("@absid", SqlDbType.Int).Value = absId
                        insCmd.Parameters.Add("@rfqid", SqlDbType.Int).Value = rfqId
                        insCmd.Parameters.Add("@prno", SqlDbType.NVarChar, 50).Value = txtprno.Text.Trim()
                        insCmd.Parameters.Add("@dabs", SqlDbType.Date).Value = Date.Today
                        insCmd.Parameters.Add("@mproc", SqlDbType.NVarChar, 150).Value = mprocValue
                        insCmd.Parameters.Add("@purpose", SqlDbType.NVarChar, 500).Value = txtpurpose.Text.Trim()
                        insCmd.Parameters.Add("@company", SqlDbType.NVarChar, 500)
                        Dim bidsParam = insCmd.Parameters.Add("@bids", SqlDbType.Decimal)
                        bidsParam.Precision = 18 : bidsParam.Scale = 2
                        insCmd.Parameters.Add("@remarks", SqlDbType.NVarChar, 250).Value = ""

                        Dim ExecRow As Action(Of Integer, String, Decimal) =
                    Sub(absno As Integer, comp As String, tot As Decimal)
                        insCmd.Parameters("@company").Value = comp
                        bidsParam.Value = tot
                        insCmd.Parameters.Add("@absno", SqlDbType.NVarChar, 50).Value = absno.ToString()
                        insCmd.ExecuteNonQuery()
                        insCmd.Parameters.RemoveAt("@absno")
                    End Sub

                        If s1Name <> "" Then ExecRow(1, s1Name, s1Tot)
                        If s2Name <> "" Then ExecRow(2, s2Name, s2Tot)
                        If s3Name <> "" Then ExecRow(3, s3Name, s3Tot)
                    End Using

                    ' 4) Compute PR total cost (budget)
                    Dim totalBudget As Decimal = 0D
                    Using budgetCmd As New SqlClient.SqlCommand("
                    SELECT ISNULL(SUM(PR_TCOST),0) 
                      FROM TBL_PRITEMS 
                     WHERE PRID = @prid;", conn, tran)
                        budgetCmd.Parameters.Add("@prid", SqlDbType.Int).Value = prId
                        totalBudget = CDec(budgetCmd.ExecuteScalar())
                    End Using

                    ' 5) Update header summary on TBL_ABSTRACT including ABS_APPBUDGET
                    Using updCmd As New SqlClient.SqlCommand("
                    UPDATE TBL_ABSTRACT
                       SET 
                         ABS_ACOMPANY  = @comp1,
                         ABS_ATOTAL    = @tot1,
                         ABS_BCOMPANY  = @comp2,
                         ABS_BTOTAL    = @tot2,
                         ABS_CCOMPANY  = @comp3,
                         ABS_CTOTAL    = @tot3,
                         ABS_AWARDEE   = @comp1,
                         ABS_APPBUDGET = @appbudget
                     WHERE ID = @absid;", conn, tran)

                        updCmd.Parameters.Add("@comp1", SqlDbType.NVarChar, 255).Value = If(s1Name <> "", CType(s1Name, Object), DBNull.Value)
                        Dim p1 = updCmd.Parameters.Add("@tot1", SqlDbType.Decimal) : p1.Precision = 18 : p1.Scale = 2 : p1.Value = If(s1Name <> "", CType(s1Tot, Object), DBNull.Value)

                        updCmd.Parameters.Add("@comp2", SqlDbType.NVarChar, 255).Value = If(s2Name <> "", CType(s2Name, Object), DBNull.Value)
                        Dim p2 = updCmd.Parameters.Add("@tot2", SqlDbType.Decimal) : p2.Precision = 18 : p2.Scale = 2 : p2.Value = If(s2Name <> "", CType(s2Tot, Object), DBNull.Value)

                        updCmd.Parameters.Add("@comp3", SqlDbType.NVarChar, 255).Value = If(s3Name <> "", CType(s3Name, Object), DBNull.Value)
                        Dim p3 = updCmd.Parameters.Add("@tot3", SqlDbType.Decimal) : p3.Precision = 18 : p3.Scale = 2 : p3.Value = If(s3Name <> "", CType(s3Tot, Object), DBNull.Value)

                        Dim pb = updCmd.Parameters.Add("@appbudget", SqlDbType.Decimal)
                        pb.Precision = 18 : pb.Scale = 2 : pb.Value = totalBudget

                        updCmd.Parameters.Add("@absid", SqlDbType.Int).Value = absId
                        updCmd.ExecuteNonQuery()
                    End Using

                    tran.Commit()

                    ' Set to label
                    lblappbudget.Text = totalBudget.ToString("N2")
                End Using
            End Using

            ' 6) UI refresh
            MessageBox.Show("Abstract refreshed. Top 3 bids re-imported, summary updated, and budget computed.",
                    "Refresh Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)

            LoadAbstract()

            If Application.OpenForms().OfType(Of frmabstract).Any() Then
                My.Forms.frmabstract.LoadAbstract()
            End If

        Catch ex As Exception
            MessageBox.Show("Error refreshing abstract: " & ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub




    Private Sub Cmdrefresh_Click(sender As Object, e As EventArgs) Handles cmdrefresh.Click
        RefreshAbstract()
    End Sub

    Private Sub Cmdprint_Click(sender As Object, e As EventArgs) Handles cmdprint.Click
        frmpprev.Dispose()

        Try
            ' 0) Validate keys
            Dim absID As Integer
            If Not Integer.TryParse(lblid.Text, absID) Then
                MessageBox.Show("Invalid Abstract ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            ' 1) Load company-level rows for this ABSID → DTABS (COMPANY, BIDS, REMARKS)
            Dim dtAbs As New DataTable("DTABS")
            Using conn As New SqlConnection(frmmain.txtdb.Text)
                conn.Open()

                Const sql As String =
                "SELECT COMPANY, BIDS, ISNULL(REMARKS,'') AS REMARKS " &
                "FROM TBL_ABSTRACTITEMS WITH (NOLOCK) " &
                "WHERE ABSID = @ABSID AND COMPANY IS NOT NULL " &
                "ORDER BY BIDS ASC;"

                Using da As New SqlDataAdapter(sql, conn)
                    da.SelectCommand.Parameters.Add("@ABSID", SqlDbType.Int).Value = absID
                    da.Fill(dtAbs)
                End Using
            End Using

            If dtAbs.Rows.Count = 0 Then
                MessageBox.Show("No company bids found for this abstract.", "Print", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            ' 2) Determine TOP1 (lowest bid)
            Dim topComp As String = dtAbs.Rows(0).Field(Of String)("COMPANY")?.Trim()
            Dim topBid As Decimal = SafeDecimal(dtAbs.Rows(0)("BIDS"))

            ' 3) Load MINISTER/MEM1/... for the >200k template
            Dim ministerHtml As String = "", mem1Html As String = "", mem2Html As String = ""
            Dim mem3Html As String = "", vChairHtml As String = "", bacHtml As String = ""

            Using cfgConn As New SqlConnection(frmmain.txtdb.Text)
                cfgConn.Open()
                Const cfgSql As String = "
                SELECT TOP 1 MINISTER, MEM1, MEM2, MEM3, CHAIRPERSON, BACCHAIR
                FROM TBL_CONFIG WITH (NOLOCK);"
                Using cmdCfg As New SqlCommand(cfgSql, cfgConn)
                    Using rdr = cmdCfg.ExecuteReader()
                        If rdr.Read() Then
                            ministerHtml = FormatSignatoryHtml(rdr("MINISTER")?.ToString())
                            mem1Html = FormatSignatoryHtml(rdr("MEM1")?.ToString())
                            mem2Html = FormatSignatoryHtml(rdr("MEM2")?.ToString())
                            mem3Html = FormatSignatoryHtml(rdr("MEM3")?.ToString())
                            vChairHtml = FormatSignatoryHtml(rdr("CHAIRPERSON")?.ToString())
                            bacHtml = FormatSignatoryHtml(rdr("BACCHAIR")?.ToString())
                        End If
                    End Using
                End Using
            End Using

            ' 4) Build recapp HTML
            Dim topBidWords As String = NumberToPesoWordsOnly(topBid) ' ends with "Pesos only"
            Dim topBidPhp As String = topBid.ToString("#,##0.00")

            Dim recAppHtml As String =
            "<b>WHEREAS</b>, after careful review and evaluation of their respective offers together with their " &
            "respective eligibility documents, the proposal of <b><u>" & HtmlEncode(topComp) & "</u></b> in the " &
            "amount of <b><u>" & HtmlEncode(topBidWords) & " (PhP " & HtmlEncode(topBidPhp) & ")</u></b> " &
            "turned out to be the most advantageous offer to the government.<br/><br/>" &
            "By virtue of BAC Resolution No. " & HtmlEncode(txtbacreso.Text.Trim()) & " s. 2025"

            ' 5) Common parameters
            Dim appBudgetVal As Decimal = SafeDecimal(lblappbudget.Text)

            Dim baseParams As New List(Of ReportParameter) From {
            New ReportParameter("aobno", txtaobrno.Text.Trim()),
            New ReportParameter("prno", txtprno.Text.Trim()),
            New ReportParameter("dabstract", DTDAbs.Value.ToString("MMMM dd yyyy")),
            New ReportParameter("purpose", txtpurpose.Text.Trim()),
            New ReportParameter("enduser", txteuser.Text.Trim()),
            New ReportParameter("mproc", txtmproc.Text.Trim()),
            New ReportParameter("appbudget", appBudgetVal.ToString("#,##0.00")),
            New ReportParameter("appbudgetword", NumberToPesoWordsOnly(appBudgetVal)),
            New ReportParameter("deadline", dtdeadline.Value.ToString("MMMM dd yyyy")),
            New ReportParameter("recapp", recAppHtml)
        }

            ' 6) Choose template + add signatories depending on threshold
            Dim rdlcPath As String
            Dim signatoryParams As New List(Of ReportParameter)

            If topBid > 200000D Then
                ' >= or > ? — currently strictly greater than 200,000 goes here
                rdlcPath = IO.Path.Combine(Application.StartupPath, "Report", "rptabsbids.rdlc")
                signatoryParams.AddRange(New ReportParameter() {
                New ReportParameter("minister", ministerHtml),
                New ReportParameter("mem1", mem1Html),
                New ReportParameter("mem2", mem2Html),
                New ReportParameter("mem3", mem3Html),
                New ReportParameter("vchair", vChairHtml),
                New ReportParameter("bac", bacHtml)
            })
            Else
                rdlcPath = IO.Path.Combine(Application.StartupPath, "Report", "rptabsbidsless.rdlc")

                ' Parse "Name|Position" → "Name{vbCrLf}Position"
                Dim euserHead As String = ComboNamePosHtml(comboenduser)
                Dim secHead As String = ComboNamePosHtml(comboshead)
                Dim approBal As String = ComboNamePosHtml(comboapproval)

                signatoryParams.AddRange(New ReportParameter() {
                New ReportParameter("euserhead", euserHead),
                New ReportParameter("sechead", secHead),
                New ReportParameter("approval", approBal)
            })
            End If

            ' 7) Ensure RDLC exists
            If Not IO.File.Exists(rdlcPath) Then
                MessageBox.Show($"RDLC file not found: {rdlcPath}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            ' 8) Preview
            frmpprev.ReportViewer1.ProcessingMode = ProcessingMode.Local
            frmpprev.ReportViewer1.LocalReport.ReportPath = rdlcPath
            frmpprev.ReportViewer1.LocalReport.EnableExternalImages = True
            frmpprev.ReportViewer1.LocalReport.DataSources.Clear()
            frmpprev.ReportViewer1.LocalReport.DataSources.Add(New ReportDataSource("DSABS", dtAbs))
            frmpprev.ReportViewer1.LocalReport.SetParameters(baseParams.Concat(signatoryParams))
            frmpprev.ReportViewer1.SetDisplayMode(DisplayMode.PrintLayout)
            frmpprev.ReportViewer1.ZoomMode = ZoomMode.Percent
            frmpprev.ReportViewer1.ZoomPercent = 100
            frmpprev.ReportViewer1.RefreshReport()

            frmpprev.lblprid.Text = Me.lblprid.Text
            frmpprev.lblprno.Text = txtprno.Text
            frmpprev.lbltransaction.Text = "AOB"
            frmpprev.panelsubmit.Visible = False

            frmpprev.ShowDialog()

        Catch ex As Exception
            MessageBox.Show("Error printing abstract report: " & ex.ToString, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Function ComboNamePosHtml(cb As ComboBox) As String
        If cb Is Nothing OrElse String.IsNullOrWhiteSpace(cb.Text) Then Return ""
        Dim parts = cb.Text.Split("|"c)
        Dim namePart As String = parts(0).Trim()
        Dim posPart As String = If(parts.Length > 1, parts(1).Trim(), "")
        If String.IsNullOrEmpty(posPart) Then
            Return "<b>" & HtmlEncode(namePart) & "</b>"
        Else
            Return "<b>" & HtmlEncode(namePart) & "</b><br/>" & HtmlEncode(posPart)
        End If
    End Function

    Private Function SafeDecimal(v As Object) As Decimal
        If v Is Nothing OrElse v Is DBNull.Value Then Return 0D
        Dim s = v.ToString().Replace(",", "").Replace("₱", "").Trim()
        Dim d As Decimal
        If Decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, d) Then Return d
        Return 0D
    End Function

    Private Function FormatSignatoryHtml(raw As String) As String
        If String.IsNullOrWhiteSpace(raw) Then Return String.Empty

        Dim parts = raw.Replace(vbCrLf, vbLf).
                    Split(New String() {vbLf}, StringSplitOptions.RemoveEmptyEntries).
                    Select(Function(s) s.Trim()).
                    ToArray()

        Dim nm As String = If(parts.Length > 0, parts(0), String.Empty)
        Dim pos As String = If(parts.Length > 1, parts(1), String.Empty)

        If String.IsNullOrEmpty(pos) Then
            Return "<b>" & HtmlEncode(nm) & "</b>"
        Else
            Return "<b>" & HtmlEncode(nm) & "</b><br/>" & HtmlEncode(pos)
        End If
    End Function

    Private Function HtmlEncode(s As String) As String
        If String.IsNullOrEmpty(s) Then Return ""
        Return s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;")
    End Function

    Private Function NumberToPesoWordsOnly(amount As Decimal) As String
        Dim pesos As Long = CLng(Math.Truncate(amount))
        Dim centavos As Integer = CInt((amount - pesos) * 100)

        Dim pesosPart As String = If(pesos = 0, "Zero", ToWords(pesos)) & " Pesos"

        If centavos > 0 Then
            Return $"{pesosPart} and {centavos:00}/100 Centavos"
        Else
            Return $"{pesosPart} Only"
        End If
    End Function

    Private Function ToWords(n As Long) As String
        Dim ones() As String = {"", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine",
                        "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen",
                        "Sixteen", "Seventeen", "Eighteen", "Nineteen"}
        Dim tens() As String = {"", "", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety"}

        If n = 0 Then Return "Zero"
        If n < 0 Then Return "Minus " & ToWords(Math.Abs(n))

        Dim parts As New List(Of String)

        Dim billions = n \ 1000000000 : n = n Mod 1000000000
        Dim millions = n \ 1000000 : n = n Mod 1000000
        Dim thousands = n \ 1000 : n = n Mod 1000
        Dim hundreds = n \ 100 : n = n Mod 100
        Dim remainder = n

        If billions > 0 Then parts.Add(ToWords(billions) & " Billion")
        If millions > 0 Then parts.Add(ToWords(millions) & " Million")
        If thousands > 0 Then parts.Add(ToWords(thousands) & " Thousand")
        If hundreds > 0 Then parts.Add(ones(hundreds) & " Hundred")

        If remainder > 0 Then
            If remainder < 20 Then
                parts.Add(ones(remainder))
            Else
                Dim t = remainder \ 10
                Dim o = remainder Mod 10
                If o > 0 Then
                    parts.Add(tens(t) & " " & ones(o))
                Else
                    parts.Add(tens(t))
                End If
            End If
        End If

        Return String.Join(" ", parts).Replace("  ", " ").Trim()
    End Function



    Private Sub cmdaddremarks_Click(sender As Object, e As EventArgs)
        ' Ensure an item row is selected
        If DataGridView1.CurrentRow Is Nothing Then
            MessageBox.Show("Please select an item to add remarks.",
                        "Add Remarks", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' Grab the AbstractItem ID from column 0
        Dim itemId As Integer = Convert.ToInt32(DataGridView1.CurrentRow.Cells("ID").Value)

        ' Open the remarks dialog, passing this form as owner
        Dim frm As New frmabstractremarks()
        frm.Owner = Me
        frm.ItemID = itemId
        frm.ShowDialog()

        ' After closing, refresh your items grid
        LoadAbstract()
    End Sub

    Private Sub combomproc_KeyPress(sender As Object, e As KeyPressEventArgs)
        e.Handled = True
    End Sub

    Private Sub cmdsubmit_Click(sender As Object, e As EventArgs)

        If lblid.Text = "" Then
            MessageBox.Show("No PR selected.", "Submit", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        Dim ask = MessageBox.Show(
        "This Abstract of Bids will be removed from Approval list and changes Status to Pending." &
        vbCrLf & vbCrLf & "Do you want to continue?",
        "Cancel Approval",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Question)

        If ask <> DialogResult.Yes Then Exit Sub

        Try
            Using conn As New SqlClient.SqlConnection(frmmain.txtdb.Text)
                conn.Open()

                ' ────────── DELETE matching approval row ──────────
                Const deleteSql As String = "
                DELETE FROM TBL_APPROVAL
                 WHERE PRID = @prid
                   AND APPROVAL_TYPE = @type;"
                Using delCmd As New SqlClient.SqlCommand(deleteSql, conn)
                    delCmd.Parameters.AddWithValue("@prid", Convert.ToInt32(lblprid.Text))
                    delCmd.Parameters.AddWithValue("@type", "Abstract Of Bids")
                    delCmd.ExecuteNonQuery()
                End Using

                ' ────────── UPDATE Purchase Request status ──────────
                Const updateSql As String = "
                UPDATE TBL_PR
                   SET PRSTATUS = '[AOB] Pending'
                 WHERE ID = @id;"
                Using updCmd As New SqlClient.SqlCommand(updateSql, conn)
                    updCmd.Parameters.AddWithValue("@id", Convert.ToInt32(lblprid.Text))
                    updCmd.ExecuteNonQuery()
                End Using

                ' ────────── UPDATE Request for Quotation status ──────────
                Const updateRFQ As String = "
                UPDATE TBL_RFQ
                   SET RFQ_STATUS = 'Pending'
                 WHERE RFQ_PRID = @id;"
                Using updCmd As New SqlClient.SqlCommand(updateRFQ, conn)
                    updCmd.Parameters.AddWithValue("@id", Convert.ToInt32(lblprid.Text))
                    updCmd.ExecuteNonQuery()
                End Using

                ' ────────── UPDATE Purchase Request status ──────────
                Const updateabs As String = "
                UPDATE TBL_ABSTRACT
                   SET ABS_STATUS = 'Pending'
                 WHERE ABS_PRID = @id;"
                Using updCmd As New SqlClient.SqlCommand(updateabs, conn)
                    updCmd.Parameters.AddWithValue("@id", Convert.ToInt32(lblprid.Text))
                    updCmd.ExecuteNonQuery()
                End Using
            End Using

            MessageBox.Show("Approval request has been cancelled.", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information)

            ' Optional: refresh UI
            Me.cmdrefresh.Enabled = True
            Me.cmdsave.Enabled = True
            Me.txtstatus.Text = "Pending"
            Me.LoadAbstract()
            frmpr.loadrecords()

            NotificationHelper.AddNotification(
                $"Abstract of Bids for PR Number {Me.txtprno.Text.Trim()} has cancelled the approval request by {frmmain.lblaname.Text.Trim()}."
            )

        Catch ex As Exception
            MessageBox.Show("Error cancelling approval: " & ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Sub Cmdsubmit_Click_1(sender As Object, e As EventArgs) Handles cmdsubmit.Click
        Try
            If LBLTTYPE.Text = "Direct Payment" Then
                ' 1) Make sure we have a PR ID to work with
                Dim prIdValue As Integer
                If Not Integer.TryParse(lblprid.Text, prIdValue) Then
                    MessageBox.Show("Invalid PR ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End If

                ' 2) Run both updates in one transaction
                Using conn As New SqlClient.SqlConnection(My.Forms.frmmain.txtdb.Text)
                    conn.Open()
                    Using tran = conn.BeginTransaction()
                        ' 2a) Mark RFQ as Ready
                        Using cmdRfQ As New SqlClient.SqlCommand(
                            "UPDATE TBL_ABSTRACT
                        SET ABS_STATUS = 'Completed'
                      WHERE ABS_PRID   = @prid;", conn, tran)
                            cmdRfQ.Parameters.Add("@prid", SqlDbType.Int).Value = prIdValue
                            cmdRfQ.ExecuteNonQuery()
                        End Using

                        ' 2b) Mark PR as RFQ Completed
                        Using cmdPr As New SqlClient.SqlCommand(
                            "UPDATE TBL_PR
                        SET PRSTATUS = '[AOB] Completed'
                      WHERE ID        = @prid;", conn, tran)
                            cmdPr.Parameters.Add("@prid", SqlDbType.Int).Value = prIdValue
                            cmdPr.ExecuteNonQuery()
                        End Using

                        tran.Commit()
                    End Using
                End Using

                ' 3) Disable the controls now that submission is done
                Me.cmdrefresh.Enabled = False
                Me.cmdsave.Enabled = False
                Me.cmdsubmit.Enabled = False
                Me.cmdscancel.Enabled = True
                Me.DataGridView1.Enabled = False
                Me.txtstatus.Text = "Completed"

                NotificationHelper.AddNotification($"Abstract of Bids for PR Number {txtprno.Text} has been Marked as Complete by {frmmain.lblaname.Text.Trim()}.")

                frmrfq.LoadRecords()

                MessageBox.Show("Request for Quotation status has been updated and is now ready for the Abstract of Canvass.",
                                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

                ' Refresh UI
                frmabstract.LoadAbstract()
            Else
                '  ' Create a fresh instance; DO NOT Dispose() the form first.
                '   Using f As New frmdeliveryadd()
                ' Pass values before showing (Load will see these)
                ' Dim ddate As String = dtddelivery.Value.ToString("MMMM dd, yyyy")
                ' Dim podate As String = dtpodate.Value.ToString("MMMM dd, yyyy")
                '   f.lblpoid.Text = Nothing
                '   f.lblprid.Text = lblprid.Text
                '  f.txtpono.Text = Nothing
                '  f.txtprno.Text = txtprno.Text
                '  f.txtpdelivery.Text = "N/A"
                '  f.lblpodate.Text = "N/A"
                '   f.txtcname.Text = DataGridView1.SelectedCells(1).Value
                '       f.txtddelivery.Text = DTDAbs.Value
                '  f.txtttype.Text = LBLTTYPE.Text


                ' If frmdeliveryadd has a DateTimePicker, set it directly.
                ' If it only has a textbox for date, pass the string you'd like to show.

                '  f.StartPosition = FormStartPosition.CenterParent
                '      f.ShowDialog(Me)  ' modal, parented
                '    End Using
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

            Using conn As New SqlClient.SqlConnection(My.Forms.frmmain.txtdb.Text)
                conn.Open()

                ' 2) Block if there is already a PO for this PR (same rule across TTYPEs)
                Using cmdCheckAbs As New SqlClient.SqlCommand(
                "SELECT COUNT(*) FROM TBL_PO WHERE PO_PRID = @prid;", conn)
                    cmdCheckAbs.Parameters.Add("@prid", SqlDbType.Int).Value = prIdValue
                    Dim poCount As Integer = CInt(cmdCheckAbs.ExecuteScalar())
                    If poCount > 0 Then
                        MessageBox.Show(
                        "Cannot cancel Abstract of Bids because there's an existing Purchase Order linked to this PR.",
                        "Cannot Cancel", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return
                    End If
                End Using

                ' 3) Branch by TTYPE
                Dim ttype As String = LBLTTYPE.Text.Trim()

                If String.Equals(ttype, "Direct Payment", StringComparison.OrdinalIgnoreCase) Then
                    ' ─────────────────────────────────────────────────────────────────────
                    ' Direct Payment: revert statuses only (your original behavior)
                    ' ─────────────────────────────────────────────────────────────────────
                    Using tran = conn.BeginTransaction()
                        Using cmdAbs As New SqlClient.SqlCommand(
                        "UPDATE TBL_ABSTRACT
                         SET ABS_STATUS = 'Processing'
                         WHERE ABS_PRID = @prid;", conn, tran)
                            cmdAbs.Parameters.Add("@prid", SqlDbType.Int).Value = prIdValue
                            cmdAbs.ExecuteNonQuery()
                        End Using

                        Using cmdPr As New SqlClient.SqlCommand(
                        "UPDATE TBL_PR
                         SET PRSTATUS = '[AOB] Processing'
                         WHERE ID = @prid;", conn, tran)
                            cmdPr.Parameters.Add("@prid", SqlDbType.Int).Value = prIdValue
                            cmdPr.ExecuteNonQuery()
                        End Using

                        tran.Commit()
                    End Using

                ElseIf ttype = "Cash Advance" OrElse ttype = "Reimbursement" Then
                    ' ─────────────────────────────────────────────────────────────────────
                    ' Cash Advance / Reimbursement:
                    '  - Delete related IAR items and IAR headers for this PR
                    '  - Then revert statuses (ABS + PR) to Processing
                    ' Notes:
                    '   * This assumes TBL_IAR has PRID (nullable) and/or POID (nullable).
                    '   * If your column is named differently (e.g., IAR_PRID), update the predicates below.
                    ' ─────────────────────────────────────────────────────────────────────
                    Using tran = conn.BeginTransaction(IsolationLevel.Serializable)
                        ' Harden the transaction against connection aborts; keep batch quiet
                        Using cmdOn As New SqlClient.SqlCommand("SET XACT_ABORT ON; SET NOCOUNT ON;", conn, tran)
                            cmdOn.ExecuteNonQuery()
                        End Using

                        ' 3a) Delete IAR items tied to any IAR rows for this PR
                        Using cmdDelItems As New SqlClient.SqlCommand("
                        DELETE IARITEMS
                        FROM TBL_IARITEMS AS IARITEMS WITH (UPDLOCK, HOLDLOCK)
                        WHERE IARITEMS.IARID IN (
                            SELECT IAR.ID
                            FROM TBL_IAR AS IAR WITH (UPDLOCK, HOLDLOCK)
                            WHERE
                                IAR.PRID = @prid
                                OR IAR.POID IN (SELECT ID FROM TBL_PO WHERE PO_PRID = @prid)
                        );", conn, tran)
                            cmdDelItems.Parameters.Add("@prid", SqlDbType.Int).Value = prIdValue
                            cmdDelItems.ExecuteNonQuery()
                        End Using

                        ' 3b) Delete IAR header(s) for this PR
                        Using cmdDelIar As New SqlClient.SqlCommand("
                        DELETE FROM TBL_IAR
                        WHERE
                            PRID = @prid
                            OR POID IN (SELECT ID FROM TBL_PO WHERE PO_PRID = @prid);", conn, tran)
                            cmdDelIar.Parameters.Add("@prid", SqlDbType.Int).Value = prIdValue
                            cmdDelIar.ExecuteNonQuery()
                        End Using

                        ' 3c) Revert statuses
                        Using cmdAbs As New SqlClient.SqlCommand("
                        UPDATE TBL_ABSTRACT
                        SET ABS_STATUS = 'Processing'
                        WHERE ABS_PRID = @prid;", conn, tran)
                            cmdAbs.Parameters.Add("@prid", SqlDbType.Int).Value = prIdValue
                            cmdAbs.ExecuteNonQuery()
                        End Using

                        Using cmdPr As New SqlClient.SqlCommand("
                        UPDATE TBL_PR
                        SET PRSTATUS = '[AOB] Processing'
                        WHERE ID = @prid;", conn, tran)
                            cmdPr.Parameters.Add("@prid", SqlDbType.Int).Value = prIdValue
                            cmdPr.ExecuteNonQuery()
                        End Using

                        tran.Commit()
                    End Using

                Else
                    ' For any unrecognized TTYPE, fall back to revert statuses (safe default)
                    Using tran = conn.BeginTransaction()
                        Using cmdAbs As New SqlClient.SqlCommand("
                        UPDATE TBL_ABSTRACT
                        SET ABS_STATUS = 'Processing'
                        WHERE ABS_PRID = @prid;", conn, tran)
                            cmdAbs.Parameters.Add("@prid", SqlDbType.Int).Value = prIdValue
                            cmdAbs.ExecuteNonQuery()
                        End Using
                        Using cmdPr As New SqlClient.SqlCommand("
                        UPDATE TBL_PR
                        SET PRSTATUS = '[AOB] Processing'
                        WHERE ID = @prid;", conn, tran)
                            cmdPr.Parameters.Add("@prid", SqlDbType.Int).Value = prIdValue
                            cmdPr.ExecuteNonQuery()
                        End Using
                        tran.Commit()
                    End Using
                End If
            End Using

            ' 4) Re-enable controls / UI state
            cmdrefresh.Enabled = True
            cmdsave.Enabled = True
            cmdsubmit.Enabled = True
            cmdscancel.Enabled = False
            txtstatus.Text = "Processing"
            Me.DataGridView1.Enabled = True

            NotificationHelper.AddNotification($"Abstract of Bids for PR Number {txtprno.Text} status has been marked as Processing by {frmmain.lblaname.Text.Trim()}.")

            ' 5) Refresh UI
            frmabstract.LoadAbstract()
            LoadAbstract()

        Catch ex As Exception
            MessageBox.Show("Error canceling AOB submission: " & ex.Message,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub lblappbudget_TextChanged(sender As Object, e As EventArgs) Handles lblappbudget.TextChanged
        Dim val As Decimal
        If Decimal.TryParse(lblappbudget.Text, val) Then
            If val < 200000D Then
                panelless.Enabled = True
            Else
                panelless.Enabled = False
            End If
        Else
            ' If parsing fails, keep panel disabled for safety
            panelless.Enabled = False
        End If
    End Sub

    Private Sub txteuser_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txteuser.KeyPress
        e.Handled = True
    End Sub


    Private Sub txtmproc_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtmproc.KeyPress
        e.Handled = True
    End Sub

    Private Sub txtaobrno_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtaobrno.KeyPress
        e.Handled = True
    End Sub

    Private Sub comboenduser_KeyPress(sender As Object, e As KeyPressEventArgs) Handles comboenduser.KeyPress
        e.Handled = True
    End Sub

    Private Sub comboshead_KeyPress(sender As Object, e As KeyPressEventArgs) Handles comboshead.KeyPress
        e.Handled = True
    End Sub

    Private Sub comboapproval_KeyPress(sender As Object, e As KeyPressEventArgs) Handles comboapproval.KeyPress
        e.Handled = True
    End Sub
End Class
