Imports System.IO
Imports System.Data.SqlClient
Imports Microsoft.Reporting.WinForms

Public Class frmrfqsupplier

    ' ─────────────────────────────────────────────────────────────────────────────
    ' State
    ' ─────────────────────────────────────────────────────────────────────────────
    Private _suppliers As DataTable   ' cache for supplier list (ID, SUPPLIER, ADDRESS, CONTACT_NO)

    Public Property RFQStatus As String
        Get
            Return frmrfqfile.txtstatus.Text
        End Get
        Set(value As String)
            frmrfqfile.txtstatus.Text = value
        End Set
    End Property

    ' ─────────────────────────────────────────────────────────────────────────────
    ' Form Load
    ' ─────────────────────────────────────────────────────────────────────────────
    Private Sub frmrfqsupplier_Load(sender As Object, e As EventArgs) Handles MyBase.Load
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

                ' Flat, borderless look
                .BorderStyle = BorderStyle.None
                .CellBorderStyle = DataGridViewCellBorderStyle.None
                .ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
                .AdvancedCellBorderStyle.Left = DataGridViewAdvancedCellBorderStyle.None
                .AdvancedCellBorderStyle.Right = DataGridViewAdvancedCellBorderStyle.None
                .AdvancedCellBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.None
                .AdvancedCellBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.None
            End With

            ' ───────────────────────────────────────────────
            ' Columns  (same data, tweaked widths & wrap)
            ' ───────────────────────────────────────────────
            DataGridView1.ColumnCount = 7

            With DataGridView1
                .Columns(0).Name = "ID"
                .Columns(0).HeaderText = "ID"
                .Columns(0).DataPropertyName = "ID"
                .Columns(0).Visible = False

                .Columns(1).Name = "RFQ_ITEMNO"
                .Columns(1).HeaderText = "ITEM NO."
                .Columns(1).DataPropertyName = "RFQ_ITEMNO"
                .Columns(1).Width = 80
                .Columns(1).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                .Columns(1).SortMode = DataGridViewColumnSortMode.NotSortable

                .Columns(2).Name = "RFQ_UNIT"
                .Columns(2).HeaderText = "UNIT"
                .Columns(2).DataPropertyName = "RFQ_UNIT"
                .Columns(2).Width = 90
                .Columns(2).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                .Columns(2).SortMode = DataGridViewColumnSortMode.NotSortable

                .Columns(3).Name = "RFQ_DESC"
                .Columns(3).HeaderText = "DESCRIPTION"
                .Columns(3).DataPropertyName = "RFQ_DESC"
                .Columns(3).Width = 420
                .Columns(3).DefaultCellStyle.WrapMode = DataGridViewTriState.True
                .Columns(3).SortMode = DataGridViewColumnSortMode.NotSortable

                .Columns(4).Name = "RFQ_QTY"
                .Columns(4).HeaderText = "QUANTITY"
                .Columns(4).DataPropertyName = "RFQ_QTY"
                .Columns(4).Width = 110
                .Columns(4).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                .Columns(4).SortMode = DataGridViewColumnSortMode.NotSortable

                .Columns(5).Name = "RFQ_UCOST"
                .Columns(5).HeaderText = "UNIT COST"
                .Columns(5).DataPropertyName = "RFQ_UCOST"
                .Columns(5).DefaultCellStyle.Format = "N2"
                .Columns(5).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                .Columns(5).Width = 140
                .Columns(5).SortMode = DataGridViewColumnSortMode.NotSortable

                .Columns(6).Name = "RFQ_TCOST"
                .Columns(6).HeaderText = "TOTAL COST"
                .Columns(6).DataPropertyName = "RFQ_TCOST"
                .Columns(6).DefaultCellStyle.Format = "N2"
                .Columns(6).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                .Columns(6).Width = 160
                .Columns(6).SortMode = DataGridViewColumnSortMode.NotSortable
            End With

            ' ───────────────────────────────────────────────
            ' Styling (consistent with your theme)
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

            ' Reduce flicker
            EnableDoubleBuffering(DataGridView1)

            ' Load detail table for the current supplier (if any)
            LoadRFQItems()

            ' ───────────────────────────────────────────────
            ' Supplier ComboBox behavior (A–Z, no typing, auto-fill)
            ' ───────────────────────────────────────────────
            SetupSupplierCombo()
            ' Hook selection change
            AddHandler txtcname.SelectedIndexChanged, AddressOf txtcname_SelectedIndexChanged

        Catch ex As Exception
            MessageBox.Show("Error initializing RFQ-Items grid: " & ex.Message,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ─────────────────────────────────────────────────────────────────────────────
    ' Double buffering for smoother grid
    ' ─────────────────────────────────────────────────────────────────────────────
    Private Sub EnableDoubleBuffering(dgv As DataGridView)
        Dim t = dgv.GetType()
        Dim pi = t.GetProperty("DoubleBuffered", Reflection.BindingFlags.Instance Or Reflection.BindingFlags.NonPublic)
        If pi IsNot Nothing Then pi.SetValue(dgv, True, Nothing)
    End Sub

    ' ─────────────────────────────────────────────────────────────────────────────
    ' Supplier ComboBox: bind list from TBL_SUPPLIERS (A–Z), no manual typing
    ' ─────────────────────────────────────────────────────────────────────────────
    Private Sub SetupSupplierCombo()
        Try
            _suppliers = New DataTable()
            Using conn As New SqlConnection(frmmain.txtdb.Text)
                conn.Open()
                ' Load everything we need in one shot; order A–Z by SUPPLIER
                Const sql As String = "
SELECT ID, SUPPLIER, ADDRESS, CONTACT_NO
FROM TBL_SUPPLIERS
ORDER BY SUPPLIER ASC;"
                Using da As New SqlDataAdapter(sql, conn)
                    da.Fill(_suppliers)
                End Using
            End Using

            ' Bind to ComboBox (fixed list, no typing)
            With txtcname
                .DataSource = _suppliers
                .DisplayMember = "SUPPLIER"
                .ValueMember = "ID"
                .SelectedIndex = -1
                .DropDownStyle = ComboBoxStyle.DropDownList  ' 🚫 No manual input
                .AutoCompleteMode = AutoCompleteMode.None
                .AutoCompleteSource = AutoCompleteSource.None
            End With

            ' Clear dependent fields initially
            ClearSupplierDetails()

        Catch ex As Exception
            MessageBox.Show("Failed to load suppliers list: " & ex.Message,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' When supplier changes, copy ADDRESS and CONTACT_NO to the textboxes
    Private Sub txtcname_SelectedIndexChanged(sender As Object, e As EventArgs)
        Try
            If txtcname.SelectedIndex < 0 OrElse _suppliers Is Nothing OrElse _suppliers.Rows.Count = 0 Then
                ClearSupplierDetails()
                Return
            End If

            Dim drv As DataRowView = TryCast(txtcname.SelectedItem, DataRowView)
            If drv Is Nothing Then
                ClearSupplierDetails()
                Return
            End If

            Dim addr As String = If(TryCast(drv.Row("ADDRESS"), String), String.Empty)
            Dim tel As String = If(TryCast(drv.Row("CONTACT_NO"), String), String.Empty)

            txtcaddress.Text = addr
            txttelno.Text = tel

            ' NOTE: txtqcertby is not supplied by TBL_SUPPLIERS; leave user-managed.

        Catch ex As Exception
            MessageBox.Show("Error applying supplier details: " & ex.Message,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub ClearSupplierDetails()
        txtcaddress.Clear()
        txttelno.Clear()
        ' txtqcertby stays user-managed
    End Sub

    ' Helper: select an item in the bound ComboBox by supplier name (case-insensitive)
    Private Sub SelectComboBySupplierName(nameText As String)
        If txtcname.DataSource Is Nothing OrElse String.IsNullOrWhiteSpace(nameText) Then
            txtcname.SelectedIndex = -1 : Exit Sub
        End If
        For i As Integer = 0 To txtcname.Items.Count - 1
            Dim drv = TryCast(txtcname.Items(i), DataRowView)
            If drv IsNot Nothing AndAlso
               String.Equals(Convert.ToString(drv("SUPPLIER")).Trim(), nameText.Trim(), StringComparison.OrdinalIgnoreCase) Then
                txtcname.SelectedIndex = i
                Exit Sub
            End If
        Next
        ' If not found in TBL_SUPPLIERS, leave unselected (no manual input allowed)
        txtcname.SelectedIndex = -1
    End Sub

    ' ─────────────────────────────────────────────────────────────────────────────
    ' Load a supplier record (existing data) — keeps your previous logic
    ' ─────────────────────────────────────────────────────────────────────────────
    Public Sub LoadSupplier(ByVal supplierId As Integer)
        Try
            Using conn As New SqlConnection(frmmain.txtdb.Text)
                conn.Open()
                Const sql As String = "
                SELECT
                  ID,
                  RFQ_PRID,
                  RFQ_PRNO,
                  RFQ_NCOMPANY,
                  RFQ_CADDRESS,
                  RFQ_PNAME,
                  RFQ_DATES,
                  RFQ_TELNO,
                  RFQ_SUM
                FROM TBL_RFQSUPPLIER
                WHERE ID = @ID
            "
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@ID", supplierId)
                    Using rdr = cmd.ExecuteReader()
                        If rdr.Read() Then
                            ' — stash the PK so Update knows what to hit —
                            lblid.Text = rdr.GetInt32(rdr.GetOrdinal("ID")).ToString()

                            ' Map saved company to the fixed ComboBox list (TBL_SUPPLIERS)
                            Dim comp As String = rdr("RFQ_NCOMPANY").ToString()
                            SelectComboBySupplierName(comp)

                            ' If not found in TBL_SUPPLIERS, or address/tel are blank, fallback to RFQ_SUP values
                            If txtcaddress.TextLength = 0 AndAlso Not rdr.IsDBNull(rdr.GetOrdinal("RFQ_CADDRESS")) Then
                                txtcaddress.Text = rdr("RFQ_CADDRESS").ToString()
                            End If
                            If txttelno.TextLength = 0 AndAlso Not rdr.IsDBNull(rdr.GetOrdinal("RFQ_TELNO")) Then
                                txttelno.Text = rdr("RFQ_TELNO").ToString()
                            End If

                            txtqcertby.Text = rdr("RFQ_PNAME").ToString()
                            dtqdate.Value = Convert.ToDateTime(rdr("RFQ_DATES"))
                        Else
                            MessageBox.Show("Supplier record not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        End If
                    End Using
                End Using
            End Using

            ' Switch Save button into Update mode
            cmdsave.Text = "Update"

        Catch ex As Exception
            MessageBox.Show("Error loading supplier: " & ex.Message,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ─────────────────────────────────────────────────────────────────────────────
    ' Load RFQ Items grid (unchanged)
    ' ─────────────────────────────────────────────────────────────────────────────
    Public Sub LoadRFQItems()
        Try
            ' 1) Parse the RFQ header ID (supplier record id acts as RFQSID)
            Dim rfqId As Integer
            If Not Integer.TryParse(lblid.Text, rfqId) Then Return

            ' 2) Query the items
            Dim dt As New DataTable()
            Using conn As New SqlConnection(frmmain.txtdb.Text)
                conn.Open()
                Const sql As String = "
                SELECT ID,
                       RFQ_ITEMNO,
                       RFQ_UNIT,
                       RFQ_DESC,
                       RFQ_QTY,
                       RFQ_UCOST,
                       RFQ_TCOST,
                       RFQ_SUM
                FROM TBL_RFQITEMS
                WHERE RFQSID = @RFQID
                ORDER BY RFQ_ITEMNO;"
                Using da As New SqlDataAdapter(sql, conn)
                    da.SelectCommand.Parameters.AddWithValue("@RFQID", rfqId)
                    da.Fill(dt)
                End Using
            End Using

            ' 3) Bind
            DataGridView1.AutoGenerateColumns = False
            DataGridView1.DataSource = dt

            ' 4) Total RFQ_TCOST
            Dim total As Decimal = 0D
            If dt.Rows.Count > 0 Then
                total = Convert.ToDecimal(dt.Compute("SUM(RFQ_TCOST)", "RFQ_TCOST IS NOT NULL"))
            End If

            ' 5) Show in label (##,###.##)
            lbltotal.Text = "SUB TOTAL: " & total.ToString("#,##0.00")
            lbltotal.Tag = total   ' keep raw decimal if needed

        Catch ex As Exception
            MessageBox.Show("Failed to load RFQ-Items: " & ex.Message,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ─────────────────────────────────────────────────────────────────────────────
    ' Save supplier (unchanged logic, validated to use current Combo selection)
    ' ─────────────────────────────────────────────────────────────────────────────
    Public Sub SaveRFQSupplier()
        ' 1) Find the RFQ header form
        Dim parent = TryCast(Application.OpenForms("frmrfqfile"), frmrfqfile)
        If parent Is Nothing Then
            MessageBox.Show("Cannot find RFQ header form.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        ' 2) Validate we have a RFQ context
        Dim rfqId As Integer
        If Not Integer.TryParse(parent.lblprid.Text, rfqId) OrElse rfqId <= 0 Then
            MessageBox.Show("Invalid RFQ ID.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        Dim rfqNo = parent.txtprno.Text.Trim()

        ' 3) Validate supplier fields
        If txtcname.SelectedIndex < 0 Then
            MessageBox.Show("Please select a supplier.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtcname.DroppedDown = True : Return
        End If
        If String.IsNullOrWhiteSpace(txtcaddress.Text) Then
            MessageBox.Show("Complete address is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtcaddress.Focus() : Return
        End If
        ' If String.IsNullOrWhiteSpace(txtqcertby.Text) Then
        'MessageBox.Show("Certified by is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        'txtqcertby.Focus() : Return
        '  End If

        ' 4) Gather values
        Dim company As String = txtcname.Text.Trim()
        Dim address As String = txtcaddress.Text.Trim()
        Dim certBy As String = txtqcertby.Text.Trim()
        Dim certDate As DateTime = dtqdate.Value.Date
        Dim telNo As String = txttelno.Text.Trim()
        Dim amount As Decimal = 0D

        ' 5) Insert into TBL_RFQSUPPLIER and get new ID
        Try
            Using conn As New SqlConnection(frmmain.txtdb.Text)
                conn.Open()
                Const sql As String = "
                INSERT INTO TBL_RFQSUPPLIER
                  (RFQ_PRID,
                   RFQ_PRNO,
                   RFQ_NCOMPANY,
                   RFQ_CADDRESS,
                   RFQ_PNAME,
                   RFQ_DATES,
                   RFQ_TELNO,
                   RFQ_SUM)
                VALUES
                  (@PRID,
                   @PRNO,
                   @Company,
                   @Address,
                   @CertBy,
                   @CertDate,
                   @Tel,
                   @Sum);
                SELECT CAST(SCOPE_IDENTITY() AS INT);
            "
                Using cmd As New SqlCommand(sql, conn)
                    With cmd.Parameters
                        .AddWithValue("@PRID", rfqId)
                        .AddWithValue("@PRNO", rfqNo)
                        .AddWithValue("@Company", company)
                        .AddWithValue("@Address", address)
                        .AddWithValue("@CertBy", certBy)
                        .AddWithValue("@CertDate", certDate)
                        .AddWithValue("@Tel", telNo)
                        .AddWithValue("@Sum", amount)
                    End With

                    ' ExecuteScalar returns the new supplier ID
                    Dim newSupplierId As Integer = Convert.ToInt32(cmd.ExecuteScalar())
                    lblid.Text = newSupplierId.ToString()
                End Using
            End Using

            ' 6) Refresh the supplier list on the parent form + import items
            parent.LoadSupplierRecords()
            ImportPRItemsToRFQItems()
            MessageBox.Show("Supplier information has been saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

            cmdedit.Enabled = True
            cmdprint.Enabled = True
            cmdsave.Text = "Update"

        Catch ex As Exception
            MessageBox.Show("Error saving supplier: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ─────────────────────────────────────────────────────────────────────────────
    ' Update supplier (unchanged logic, uses current Combo selection)
    ' ─────────────────────────────────────────────────────────────────────────────
    Public Sub UpdateRFQSupplier()
        ' 1) Find the RFQ header form
        Dim parent = TryCast(Application.OpenForms("frmrfqfile"), frmrfqfile)
        If parent Is Nothing Then
            MessageBox.Show("Cannot find RFQ header form.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        ' 2) Parse the supplier ID from lblid
        Dim supplierId As Integer
        If Not Integer.TryParse(lblid.Text, supplierId) OrElse supplierId <= 0 Then
            MessageBox.Show("Invalid Supplier ID.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' 3) Validate supplier fields
        If txtcname.SelectedIndex < 0 Then
            MessageBox.Show("Please select a supplier.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtcname.DroppedDown = True : Return
        End If
        If String.IsNullOrWhiteSpace(txtcaddress.Text) Then
            MessageBox.Show("Complete address is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtcaddress.Focus() : Return
        End If
        If String.IsNullOrWhiteSpace(txtqcertby.Text) Then
            MessageBox.Show("Certified by is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtqcertby.Focus() : Return
        End If

        ' 4) Gather values
        Dim company As String = txtcname.Text.Trim()
        Dim address As String = txtcaddress.Text.Trim()
        Dim certBy As String = txtqcertby.Text.Trim()
        Dim certDate As DateTime = dtqdate.Value.Date
        Dim telNo As String = txttelno.Text.Trim()

        ' 5) Perform the UPDATE
        Try
            Using conn As New SqlConnection(frmmain.txtdb.Text)
                conn.Open()
                Const sql As String = "
                UPDATE TBL_RFQSUPPLIER
                SET
                  RFQ_NCOMPANY = @Company,
                  RFQ_CADDRESS = @Address,
                  RFQ_PNAME    = @CertBy,
                  RFQ_DATES    = @CertDate,
                  RFQ_TELNO    = @Tel
                WHERE ID = @ID
            "
                Using cmd As New SqlCommand(sql, conn)
                    With cmd.Parameters
                        .AddWithValue("@Company", company)
                        .AddWithValue("@Address", address)
                        .AddWithValue("@CertBy", certBy)
                        .AddWithValue("@CertDate", certDate)
                        .AddWithValue("@Tel", telNo)
                        .AddWithValue("@ID", supplierId)
                    End With

                    Dim affected = cmd.ExecuteNonQuery()
                    If affected = 0 Then
                        MessageBox.Show("No supplier was updated. Please check the Supplier ID.",
                                    "Update Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return
                    End If
                End Using
            End Using

            ' 6) Refresh and notify
            parent.LoadSupplierRecords()
            MessageBox.Show("Supplier information updated successfully.",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show("Error updating supplier: " & ex.Message,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ─────────────────────────────────────────────────────────────────────────────
    ' Import PR Items into RFQ Items (unchanged)
    ' ─────────────────────────────────────────────────────────────────────────────
    Public Sub ImportPRItemsToRFQItems()
        ' 1) Get parent RFQ form
        Dim parent = TryCast(Application.OpenForms("frmrfqfile"), frmrfqfile)
        If parent Is Nothing Then
            MessageBox.Show("RFQ form not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        ' 2) Parse IDs & PRNO
        Dim rfqId As Integer = 0
        Dim supplierId As Integer = 0
        Dim prId As Integer = 0
        Dim prNo As String = parent.txtprno.Text.Trim()

        If Integer.TryParse(parent.lblid.Text, rfqId) _
        AndAlso Integer.TryParse(Me.lblid.Text, supplierId) _
        AndAlso Integer.TryParse(parent.lblprid.Text, prId) Then

            Try
                Using conn As New SqlConnection(frmmain.txtdb.Text)
                    conn.Open()

                    Const sql As String = "
                    INSERT INTO TBL_RFQITEMS
                      (RFQID,
                       RFQSID,
                       PRID,
                       PRNO,
                       RFQ_ITEMNO,
                       RFQ_UNIT,
                       RFQ_DESC,
                       RFQ_QTY,
                       RFQ_UCOST,
                       RFQ_TCOST,
                       RFQ_SUM)
                    SELECT
                      @RFQID,
                      @RFQSID,
                      @PRID,
                      @PRNO,
                      PR_ITEMNO,
                      PR_UNIT,
                      PR_DESC,
                      PR_QTY,
                      0.00,
                      0.00,
                      0.00
                    FROM TBL_PRITEMS
                    WHERE PRID = @PRID
                    ORDER BY PR_ITEMNO
                "

                    Using cmd As New SqlCommand(sql, conn)
                        With cmd.Parameters
                            .AddWithValue("@RFQID", rfqId)
                            .AddWithValue("@RFQSID", supplierId)
                            .AddWithValue("@PRID", prId)
                            .AddWithValue("@PRNO", prNo)
                        End With

                        Dim insertedCount = cmd.ExecuteNonQuery()
                        MessageBox.Show($"{insertedCount} item(s) imported into RFQ.", "Import Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    End Using
                End Using

                ' 3) Refresh RFQ‐Items grid on the current form
                LoadRFQItems()

            Catch ex As Exception
                MessageBox.Show("Error importing PR items: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        Else
            MessageBox.Show("Invalid RFQ, Supplier or PR ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    ' ─────────────────────────────────────────────────────────────────────────────
    ' Save / Edit / Delete / Print hooks (unchanged)
    ' ─────────────────────────────────────────────────────────────────────────────
    Private Sub Cmdsave_Click(sender As Object, e As EventArgs) Handles cmdsave.Click
        Try
            If cmdsave.Text = "Save" Then
                SaveRFQSupplier()
            ElseIf cmdsave.Text = "Update" Then
                UpdateRFQSupplier()
            End If
        Catch ex As Exception
            MessageBox.Show("Save/Update error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs)
        ' (kept for compatibility; no code)
    End Sub

    Private Sub Cmdprint_Click(sender As Object, e As EventArgs) Handles cmdprint.Click
        Try
            ' — 1) Pull the RFQ header info for parameters —
            Dim rfqForm = TryCast(Application.OpenForms("frmrfqfile"), frmrfqfile)
            If rfqForm Is Nothing Then
                MessageBox.Show("RFQ header form not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            Dim headerId As Integer
            If Not Integer.TryParse(rfqForm.lblprid.Text, headerId) Then
                MessageBox.Show("Invalid RFQ header ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            Dim BACSECNAME As String = ""
            Using cfgConn As New SqlClient.SqlConnection(frmmain.txtdb.Text)
                cfgConn.Open()
                Const cfgSql As String = "SELECT TOP 1 BACSEC FROM TBL_CONFIG;"
                Using cmdCfg As New SqlClient.SqlCommand(cfgSql, cfgConn)
                    Using rdr = cmdCfg.ExecuteReader()
                        If rdr.Read() Then
                            BACSECNAME = rdr("BACSEC")?.ToString().Trim()
                        End If
                    End Using
                End Using
            End Using

            ' 2) Split on newline (handle both vbCrLf and vbLf just in case)
            Dim parts = BACSECNAME _
            .Split(New String() {vbCrLf, vbLf}, StringSplitOptions.RemoveEmptyEntries) _
            .Select(Function(s) s.Trim()) _
            .ToArray()

            Dim namePart As String = If(parts.Length > 0, parts(0), String.Empty)
            Dim positionPart As String = If(parts.Length > 1, parts(1), String.Empty)

            ' 3) Build HTML: bold name, normal position, with a <br/>
            Dim BACSECHtml As String
            If String.IsNullOrEmpty(positionPart) Then
                BACSECHtml = $"<b>{namePart}</b>"
            Else
                BACSECHtml = $"<b>{namePart}</b><br/>{positionPart}"
            End If

            Dim headerParams As New Dictionary(Of String, String) From {
                {"PRNO", rfqForm.txtprno.Text},
                {"PRDATE", rfqForm.txtprdate.Text},
                {"PRRECEIVED", rfqForm.dtrecieved.Value},
                {"DCANVASS", rfqForm.dtdcanvass.Value.ToString("MM/dd/yyyy")},
                {"OEUSER", rfqForm.txtoeuser.Text},
                {"SUBDATE", rfqForm.dtsdate.Value.ToString("MM/dd/yyyy")},
                {"MPROC", rfqForm.combomproc.Text},
                {"REQDOCS", "-PhilGEPS Certificate" & vbNewLine & "-Omnibus Sworn Statement " & Year(Now) & vbNewLine & "-Mayor's Permit " & Year(Now) & vbNewLine & "-ITR/Tax Clearance"},
                {"PURPOSE", rfqForm.txtpurpose.Text},
                {"FUNDS", rfqForm.txtfund.Text},
                {"BACSEC", BACSECHtml},
                {"CANBY", rfqForm.txtcanby.Text}
            }

            ' — 2) Pull the supplier‐specific parameters —
            Dim supId As Integer
            If Not Integer.TryParse(lblid.Text, supId) Then
                MessageBox.Show("Invalid Supplier ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            headerParams("CNAME") = txtcname.Text.Trim()
            headerParams("CADDRESS") = txtcaddress.Text.Trim()
            headerParams("QCBY") = txtqcertby.Text.Trim()
            headerParams("QCDATE") = dtqdate.Value.ToString("MM/dd/yyyy")
            headerParams("QCTELNO") = txttelno.Text.Trim()

            ' — 3) Load the detail table from TBL_RFQITEMS —
            Dim dt As New DataTable()
            Using conn As New SqlConnection(frmmain.txtdb.Text)
                conn.Open()
                Const sqlItems As String = "
                SELECT
                  RFQ_ITEMNO,
                  RFQ_UNIT,
                  RFQ_DESC,
                  RFQ_QTY,
                  RFQ_UCOST,
                  RFQ_TCOST
                FROM TBL_RFQITEMS
                WHERE RFQSID = @SupID
                ORDER BY RFQ_ITEMNO
            "
                Using da As New SqlDataAdapter(sqlItems, conn)
                    da.SelectCommand.Parameters.AddWithValue("@SupID", supId)
                    da.Fill(dt)
                End Using
            End Using

            ' — 4) Prepare ReportParameters for the RDLC —
            Dim rptParams = New List(Of ReportParameter)
            For Each kvp In headerParams
                rptParams.Add(New ReportParameter(kvp.Key, kvp.Value))
            Next

            ' — 5) Find and verify your RDLC file —
            Dim rdlcPath = Path.Combine(Application.StartupPath, "report", "rptsuppquote.rdlc")
            If Not File.Exists(rdlcPath) Then
                MessageBox.Show($"RDLC file not found: {rdlcPath}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            ' — 6) Show the preview —
            Using preview As New frmpprev()
                With preview.ReportViewer1
                    .ProcessingMode = ProcessingMode.Local
                    .LocalReport.ReportPath = rdlcPath

                    ' Dataset name must match what you defined in the RDLC (here "DSRFQ")
                    .LocalReport.DataSources.Clear()
                    .LocalReport.DataSources.Add(New ReportDataSource("DSRFQ", dt))

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
            MessageBox.Show("Error printing supplier quote: " & ex.ToString, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ─────────────────────────────────────────────────────────────────────────────
    ' Edit item amounts dialog (unchanged)
    ' ─────────────────────────────────────────────────────────────────────────────
    Public Sub editamount()
        Try
            frmrfqupdate.Dispose()
            ' 1) Make sure a row is selected
            If DataGridView1.SelectedRows.Count = 0 Then
                MessageBox.Show("Please select an item to edit.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            ' 2) Read the PK of the RFQ‐item
            Dim rawId = DataGridView1.SelectedRows(0).Cells("ID").Value
            Dim itemId As Integer
            If rawId Is Nothing OrElse Not Integer.TryParse(rawId.ToString(), itemId) Then
                MessageBox.Show("Invalid item ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            ' 3) Open the Update dialog and pre‐fill its controls
            Using upd As New frmrfqupdate()
                upd.lbliid.Text = itemId.ToString()
                upd.combounit.Text = DataGridView1.SelectedRows(0).Cells("RFQ_UNIT").Value.ToString()
                upd.txtdesc.Text = DataGridView1.SelectedRows(0).Cells("RFQ_DESC").Value.ToString()

                ' parse decimals so Format works even if grid cell is DBNull
                Dim qty As Decimal = Convert.ToDecimal(DataGridView1.SelectedRows(0).Cells("RFQ_QTY").Value)
                Dim ucost As Decimal = Convert.ToDecimal(DataGridView1.SelectedRows(0).Cells("RFQ_UCOST").Value)

                upd.txtqty.Text = qty.ToString("N2")
                upd.txtucost.Text = ucost.ToString("N2")
                upd.txttcost.Text = (qty * ucost).ToString("N2")

                upd.cmdsave.Text = "Update"
                upd.ShowDialog(Me)
            End Using
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub

    Private Sub Cmdedit_Click(sender As Object, e As EventArgs) Handles cmdedit.Click
        editamount()
    End Sub

    Private Sub Cmddelete_Click(sender As Object, e As EventArgs) Handles cmddelete.Click
        Me.Dispose()
    End Sub

    ' ─────────────────────────────────────────────────────────────────────────────
    ' Deprecated handlers (type-to-search) — intentionally disabled for DropDownList
    ' ─────────────────────────────────────────────────────────────────────────────
    'Private Sub txtcname_TextUpdate(sender As Object, e As EventArgs) Handles txtcname.TextUpdate
    '    ' Replaced by fixed DropDownList bound to TBL_SUPPLIERS (no manual input).
    '    ' Kept commented to avoid accidental re-wiring.
    'End Sub

    'Private Sub txtcname_KeyDown(sender As Object, e As KeyEventArgs) Handles txtcname.KeyDown
    '    ' Replaced. Prevent manual typing logic.
    'End Sub

    'Private Sub txtcname_Validated(sender As Object, e As EventArgs) Handles txtcname.Validated
    '    ' Replaced. Address/tel now auto-fill via SelectedIndexChanged from the bound list.
    'End Sub

End Class
