Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Threading.Tasks

Public Class frmissuanceSEPAdd

    Private ReadOnly _parent As frmissuanceSEPFile
    Private _suppressEvents As Boolean = False
    Private _lastQuery As String = ""
    Private _lastLoadTs As DateTime = DateTime.MinValue

    Public Sub New(parent As frmissuanceSEPFile)
        InitializeComponent()
        _parent = parent
    End Sub

    Private Sub frmissuanceSEPAdd_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            combodesc.DropDownStyle = ComboBoxStyle.DropDown
            combodesc.AutoCompleteMode = AutoCompleteMode.None
            combodesc.AutoCompleteSource = AutoCompleteSource.None
            combodesc.IntegralHeight = False
            combodesc.MaxDropDownItems = 12

            txtunit.ReadOnly = True
            txtpnumber.ReadOnly = True

            txtqty.Text = "1"
            comboamount.DropDownStyle = ComboBoxStyle.DropDown
            comboamount.AutoCompleteMode = AutoCompleteMode.SuggestAppend
            comboamount.AutoCompleteSource = AutoCompleteSource.ListItems

            txttcost.Text = "0.00"
            txteulife.Text = ""   ' NVARCHAR free text
        Catch ex As Exception
            MessageBox.Show("Add form load error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ── Live search on Description ───────────────────────────────────────────────
    Private Async Sub combodesc_TextUpdate(sender As Object, e As EventArgs) Handles combodesc.TextUpdate
        Dim nowTs = DateTime.UtcNow
        If (nowTs - _lastLoadTs).TotalMilliseconds < 200 Then Return
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
                    Await conn.OpenAsync()
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
            ' ignore suggestion failure
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
                        Else
                            txtunit.Clear()
                            txtpnumber.Clear()
                        End If
                    End Using
                End Using

                ' Fill historical unit costs
                Await LoadDistinctCostsAsync(conn, descText)
            End Using

        Catch ex As Exception
            MessageBox.Show("Lookup error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
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

    Private Async Sub combodesc_SelectionChangeCommitted(sender As Object, e As EventArgs) Handles combodesc.SelectionChangeCommitted
        If _suppressEvents Then Return
        Await PopulateFromSelectedDescAsync()
    End Sub

    Private Async Sub combodesc_Leave(sender As Object, e As EventArgs) Handles combodesc.Leave
        Await PopulateFromSelectedDescAsync()
    End Sub

    ' ── Amount & Qty → Total Cost ────────────────────────────────────────────────
    Private Sub comboamount_Leave(sender As Object, e As EventArgs) Handles comboamount.Leave
        Dim amt As Decimal
        If Decimal.TryParse(SanitizeNum(comboamount.Text), amt) Then
            comboamount.Text = amt.ToString("##,###.##")
        End If
        RecalcTotalCost()
    End Sub

    Private Sub comboamount_SelectedIndexChanged(sender As Object, e As EventArgs) Handles comboamount.SelectedIndexChanged
        RecalcTotalCost()
    End Sub

    Private Sub txtqty_TextChanged(sender As Object, e As EventArgs) Handles txtqty.TextChanged
        RecalcTotalCost()
    End Sub

    Private Sub RecalcTotalCost()
        Dim qty As Decimal
        Dim amt As Decimal
        Decimal.TryParse(SanitizeNum(txtqty.Text), qty)
        Decimal.TryParse(SanitizeNum(comboamount.Text), amt)
        Dim t = qty * amt
        txttcost.Text = t.ToString("##,###.##")
    End Sub

    Private Shared Function SanitizeNum(s As String) As String
        Return If(s, "").Replace(",", "").Trim()
    End Function

    ' ── Save: push item + header fields to parent ────────────────────────────────
    Private Sub cmdsave_Click(sender As Object, e As EventArgs) Handles cmdsave.Click
        Try
            Dim descText As String = combodesc.Text.Trim()
            Dim unitText As String = txtunit.Text.Trim()
            Dim pNo As String = txtpnumber.Text.Trim()
            Dim euLife As String = txteulife.Text.Trim()

            Dim qtyVal As Decimal
            Dim amtVal As Decimal
            Dim tcostVal As Decimal

            If String.IsNullOrEmpty(descText) Then
                MessageBox.Show("Please select a Description.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If
            If Not Decimal.TryParse(SanitizeNum(txtqty.Text), qtyVal) OrElse qtyVal <= 0D Then
                MessageBox.Show("Please enter a valid Quantity.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If
            If String.IsNullOrEmpty(unitText) Then
                MessageBox.Show("Unit is required (auto-filled from inventory).", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If
            If String.IsNullOrEmpty(pNo) Then
                MessageBox.Show("Property Number is required (auto-filled from inventory).", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If
            If Not Decimal.TryParse(SanitizeNum(If(comboamount.SelectedItem?.ToString(), comboamount.Text)), amtVal) Then
                MessageBox.Show("Please enter a valid Amount.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If
            If Not Decimal.TryParse(SanitizeNum(txttcost.Text), tcostVal) Then
                tcostVal = qtyVal * amtVal
                txttcost.Text = tcostVal.ToString("##,###.##")
            End If

            ' Get header values exposed by the parent form
            ' Dim icsNo As String = _parent.HeaderICSNo
            ' Dim icsDate As Date = _parent.HeaderICSDate
            ' Dim fcluster As String = _parent.HeaderFCluster
            ' Dim parMrToName As String = _parent.HeaderParMRToName
            ' Dim parMrToPos As String = _parent.HeaderParMRToPosition
            'Dim preparedByName As String = _parent.HeaderPreparedByName

            ' Hand off to parent grid (Save-at-parent pattern)
            _parent.AddItemRow(
                qtyVal, unitText, descText, pNo,
                amtVal, tcostVal, euLife
                 )

            Me.DialogResult = DialogResult.OK
            Me.Close()
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try

    End Sub

    Private Sub cmdclose_Click(sender As Object, e As EventArgs) Handles cmdclose.Click
        Me.Close()
    End Sub

End Class
