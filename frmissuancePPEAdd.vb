Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Threading.Tasks

Public Class frmissuancePPEAdd

    Private ReadOnly _parent As frmissuancePPEFile
    Private _suppressEvents As Boolean = False
    Private _lastQuery As String = ""
    Private _lastLoadTs As DateTime = DateTime.MinValue

    Public Sub New(parent As frmissuancePPEFile)
        InitializeComponent()
        _parent = parent
    End Sub

    Private Sub frmissuancePPEAdd_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            ' UX set-up
            combodesc.DropDownStyle = ComboBoxStyle.DropDown ' allow typing
            combodesc.AutoCompleteMode = AutoCompleteMode.None  ' do NOT auto-change user text
            combodesc.AutoCompleteSource = AutoCompleteSource.None
            combodesc.IntegralHeight = False
            combodesc.MaxDropDownItems = 12

            txtunit.ReadOnly = True
            txtpnumber.ReadOnly = True

            ' Reasonable defaults
            dtdaquired.Value = Date.Today

            comboamount.DropDownStyle = ComboBoxStyle.DropDown   ' allow typing or selection
            comboamount.AutoCompleteMode = AutoCompleteMode.SuggestAppend
            comboamount.AutoCompleteSource = AutoCompleteSource.ListItems
        Catch ex As Exception
            MessageBox.Show("Add form load error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ─────────────────────────────────────────────────────────────────────────────
    ' Live filter for descriptions (LIKE on DESCRIPTIONS, UNITS, IPNO)
    ' We repopulate the dropdown without changing user-typed text.
    ' Debounce a bit to avoid hammering Cloud SQL on each keystroke.
    ' ─────────────────────────────────────────────────────────────────────────────
    Private Async Sub combodesc_TextUpdate(sender As Object, e As EventArgs) Handles combodesc.TextUpdate
        Dim nowTs = DateTime.UtcNow
        If (nowTs - _lastLoadTs).TotalMilliseconds < 200 Then Return ' tiny debounce

        Dim q As String = combodesc.Text.Trim()
        If q = _lastQuery Then Return

        _lastQuery = q
        _lastLoadTs = nowTs
        Await LoadDescSuggestionsAsync(q)
    End Sub

    Private Async Function LoadDescSuggestionsAsync(query As String) As Task
        Dim caret As Integer = combodesc.SelectionStart
        Dim typed As String = combodesc.Text

        Try
            Dim dt As New DataTable()
            Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
                Using cmd As New SqlCommand("
                SELECT TOP 50 DESCRIPTIONS, UNITS, IPNO
                FROM TBL_INVENTORY WITH (NOLOCK)
                WHERE (@q = '')
                   OR (DESCRIPTIONS LIKE '%' + @q + '%'
                    OR UNITS LIKE '%' + @q + '%'
                    OR IPNO LIKE '%' + @q + '%')
                ORDER BY DESCRIPTIONS;", conn)
                    cmd.Parameters.AddWithValue("@q", query)
                    cmd.CommandTimeout = 15
                    Await conn.OpenAsync()                 ' ← no ConfigureAwait(False)
                    Using rdr = Await cmd.ExecuteReaderAsync()
                        dt.Load(rdr)
                    End Using
                End Using
            End Using

            _suppressEvents = True
            combodesc.BeginUpdate()
            combodesc.DataSource = Nothing
            combodesc.Items.Clear()
            combodesc.DataSource = dt
            combodesc.DisplayMember = "DESCRIPTIONS"
            combodesc.ValueMember = "DESCRIPTIONS"
            combodesc.EndUpdate()
        Catch
            ' ignore suggestion failures
        Finally
            _suppressEvents = False
            combodesc.Text = typed
            combodesc.SelectionStart = Math.Min(caret, combodesc.Text.Length)
            combodesc.DroppedDown = True
        End Try
    End Function

    Private Async Function PopulateFromSelectedDescAsync() As Task
        Dim descText As String = combodesc.Text.Trim()
        If String.IsNullOrEmpty(descText) Then Return

        Try
            Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
                Await conn.OpenAsync()

                Using cmd As New SqlCommand("
                SELECT TOP 1 UNITS, IPNO
                FROM TBL_INVENTORY WITH (NOLOCK)
                WHERE DESCRIPTIONS = @d;", conn)
                    cmd.Parameters.AddWithValue("@d", descText)
                    cmd.CommandTimeout = 10
                    Using rdr = Await cmd.ExecuteReaderAsync()
                        If Await rdr.ReadAsync() Then
                            txtunit.Text = rdr("UNITS").ToString()
                            txtpnumber.Text = rdr("IPNO").ToString()
                        End If
                    End Using
                End Using

                Await LoadImageAsync(conn, descText)
                Await LoadDistinctCostsAsync(conn, descText)
            End Using
        Catch ex As Exception
            MessageBox.Show("Lookup error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Function

    ' When user actually selects an item, fill Unit & Property No; load image + amount list
    Private Async Sub combodesc_SelectionChangeCommitted(sender As Object, e As EventArgs) Handles combodesc.SelectionChangeCommitted
        If _suppressEvents Then Return
        Await PopulateFromSelectedDescAsync()
    End Sub

    Private Async Sub combodesc_Leave(sender As Object, e As EventArgs) Handles combodesc.Leave
        ' If they type fully a value that exists (exact match), try to populate
        Await PopulateFromSelectedDescAsync()
    End Sub


    Private Async Function LoadImageAsync(conn As SqlConnection, descText As String) As Task
        PictureBox1.Image = Nothing
        Using cmd As New SqlCommand("
        SELECT TOP 1 IMAGES FROM TBL_EDESC WITH (NOLOCK)
        WHERE EDESC = @d AND IMAGES IS NOT NULL;", conn)
            cmd.Parameters.AddWithValue("@d", descText)
            cmd.CommandTimeout = 10
            Dim obj = Await cmd.ExecuteScalarAsync()
            If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                Dim bytes = DirectCast(obj, Byte())
                If bytes IsNot Nothing AndAlso bytes.Length > 0 Then
                    Using ms As New MemoryStream(bytes)
                        PictureBox1.Image = Image.FromStream(ms)
                        PictureBox1.SizeMode = PictureBoxSizeMode.Zoom
                    End Using
                End If
            End If
        End Using
    End Function

    Private Async Function LoadDistinctCostsAsync(conn As SqlConnection, descText As String) As Task
        comboamount.BeginUpdate()
        comboamount.Items.Clear()
        Using cmd As New SqlCommand("
        SELECT DISTINCT PO_UCOST
        FROM TBL_POITEMS WITH (NOLOCK)
        WHERE PO_DESC = @d
        ORDER BY PO_UCOST;", conn)
            cmd.Parameters.AddWithValue("@d", descText)
            cmd.CommandTimeout = 10
            Using rdr = Await cmd.ExecuteReaderAsync()
                While Await rdr.ReadAsync()
                    Dim decVal As Decimal
                    If Decimal.TryParse(rdr(0).ToString(), decVal) Then
                        comboamount.Items.Add(decVal.ToString("##,###.##"))
                    End If
                End While
            End Using
        End Using
        comboamount.EndUpdate()
    End Function

    Private Sub cmdsave_Click(sender As Object, e As EventArgs) Handles cmdsave.Click
        ' Validate local controls; do NOT save to DB yet. We add to parent grid.
        Dim descText As String = combodesc.Text.Trim()
        Dim unitText As String = txtunit.Text.Trim()
        Dim qtyVal As Decimal
        Dim pNo As String = txtpnumber.Text.Trim()
        Dim amtVal As Decimal

        If String.IsNullOrEmpty(descText) Then
            MessageBox.Show("Please select a Description.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If Not Decimal.TryParse(txtqty.Text.Trim(), qtyVal) OrElse qtyVal <= 0D Then
            MessageBox.Show("Please enter a valid Quantity.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If String.IsNullOrEmpty(unitText) Then
            MessageBox.Show("Unit is required (auto-filled from inventory).", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        If Not Decimal.TryParse(comboamount.Text.Replace(",", ""), amtVal) Then
            MessageBox.Show("Please enter a valid Amount.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        If String.IsNullOrEmpty(pNo) Then
            MessageBox.Show("Property Number is required (auto-filled from inventory).", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If Not Decimal.TryParse(If(comboamount.SelectedItem?.ToString(), comboamount.Text.Trim()), amtVal) Then
            MessageBox.Show("Please enter a valid Amount.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        comboamount.Text = amtVal.ToString("##,###.##")

        ' Push the buffered row to parent grid
        _parent.AddItemRow(qtyVal, unitText, descText, pNo, dtdaquired.Value.Date, amtVal)
        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Sub comboamount_Leave(sender As Object, e As EventArgs) Handles comboamount.Leave
        Dim amtVal As Decimal
        If Decimal.TryParse(comboamount.Text.Replace(",", ""), amtVal) Then
            comboamount.Text = amtVal.ToString("##,###.##")
        End If
    End Sub

    Private Sub cmdclose_Click(sender As Object, e As EventArgs) Handles cmdclose.Click
        Me.Close()
    End Sub

    Private Sub Comboamount_SelectedIndexChanged(sender As Object, e As EventArgs) Handles comboamount.SelectedIndexChanged

    End Sub
End Class
