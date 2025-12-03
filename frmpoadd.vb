Imports System.Data.SqlClient
Public Class frmpoadd
    Private _selectedPRNo As String = String.Empty
    Private Sub Frmpoadd_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' 1) Initial load: show the most recent 25 pending PRs
        LoadPRSuggestions("")

        ' 2) Enable autocomplete behavior
        comboprno.AutoCompleteMode = AutoCompleteMode.SuggestAppend
        comboprno.AutoCompleteSource = AutoCompleteSource.CustomSource

        ' 3) Start with Continue disabled
        cmdcontinue.Enabled = False
    End Sub


    ''' <summary>
    ''' Queries TBL_PR for the top 25 pending PRNOs matching the filter (or blank for all),
    ''' and repopulates comboprno.Items and its AutoCompleteCustomSource.
    ''' </summary>
    Private Sub LoadPRSuggestions(filter As String)
        Dim prList As New List(Of String)()

        Using conn As New SqlConnection(frmmain.txtdb.Text)
            conn.Open()
            Dim sql = "
                SELECT TOP 50 PRNO
                FROM TBL_PR
                WHERE PRSTATUS = '[AOB] Completed'
            "


            If filter.Length > 0 Then
                sql &= " AND PRNO LIKE @pattern"
            End If
            sql &= " ORDER BY PRDATE DESC"

            Using cmd As New SqlCommand(sql, conn)
                If filter.Length > 0 Then
                    cmd.Parameters.AddWithValue("@pattern", "%" & filter.Replace("'", "''") & "%")
                End If
                Using rdr = cmd.ExecuteReader()
                    While rdr.Read()
                        prList.Add(rdr.GetString(0))
                    End While
                End Using
            End Using
        End Using

        ' Rebuild the drop-down list and autocomplete source
        comboprno.Items.Clear()
        Dim ac As New AutoCompleteStringCollection()
        For Each pr In prList
            comboprno.Items.Add(pr)
            ac.Add(pr)
        Next
        comboprno.AutoCompleteCustomSource = ac
    End Sub

    Private Sub comboprno_SelectionChangeCommitted(sender As Object, e As EventArgs) Handles comboprno.SelectionChangeCommitted
        ' 6) When the user actually picks one of the dropdown items:
        If comboprno.SelectedIndex >= 0 Then
            _selectedPRNo = comboprno.SelectedItem.ToString()
            comboprno.Text = _selectedPRNo     ' overwrite with pure PRNO
            comboprno.DroppedDown = False
            cmdcontinue.Enabled = True         ' now we can Continue
        End If
    End Sub

    Private Sub cmdcontinue_Click(sender As Object, e As EventArgs) Handles cmdcontinue.Click
        Try
            ' 1) Make sure a PR Number is selected
            Dim prNo As String = comboprno.Text.Trim()
            If String.IsNullOrEmpty(prNo) Then
                MessageBox.Show("Please select a PR Number.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            ' 2) Variables to hold data
            Dim prId As Integer
            Dim prDate As DateTime
            Dim prPurpose As String = ""
            Dim rfqMProc As String = ""
            Dim rfqFunds As String = ""
            Dim nCompany As String = ""
            Dim cAddress As String = ""
            Dim supName As String = ""

            ' 3) Fetch from the database
            Using conn As New SqlConnection(frmmain.txtdb.Text)
                conn.Open()

                ' 3a) Get PR details
                Using cmd As New SqlCommand("
                SELECT ID, PRDATE, PRPURPOSE
                  FROM TBL_PR
                 WHERE PRNO = @prno;", conn)
                    cmd.Parameters.AddWithValue("@prno", prNo)
                    Using rdr = cmd.ExecuteReader()
                        If rdr.Read() Then
                            prId = Convert.ToInt32(rdr("ID"))
                            prDate = Convert.ToDateTime(rdr("PRDATE"))
                            prPurpose = rdr("PRPURPOSE")?.ToString().Trim()
                        Else
                            MessageBox.Show($"PR Number '{prNo}' not found.", "Error",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Return
                        End If
                    End Using
                End Using

                ' 3b) Get RFQ master (assumes one row per PR)
                Using cmd As New SqlCommand("
                SELECT TOP 1 RFQ_MPROC, RFQ_FUNDS
                  FROM TBL_RFQ
                 WHERE RFQ_PRID = @prid;", conn)
                    cmd.Parameters.AddWithValue("@prid", prId)
                    Using rdr = cmd.ExecuteReader()
                        If rdr.Read() Then
                            rfqMProc = rdr("RFQ_MPROC")?.ToString().Trim()
                            rfqFunds = rdr("RFQ_FUNDS")?.ToString().Trim()
                        End If
                    End Using
                End Using

                ' 3c) Get the supplier row with the smallest RFQ_SUM
                Using cmd As New SqlCommand("
                SELECT TOP 1 RFQ_NCOMPANY, RFQ_CADDRESS, RFQ_PNAME
                  FROM TBL_RFQSUPPLIER
                 WHERE RFQ_PRID = @prid
              ORDER BY RFQ_SUM ASC;", conn)
                    cmd.Parameters.AddWithValue("@prid", prId)
                    Using rdr = cmd.ExecuteReader()
                        If rdr.Read() Then
                            nCompany = rdr("RFQ_NCOMPANY")?.ToString().Trim()
                            cAddress = rdr("RFQ_CADDRESS")?.ToString().Trim()
                            supName = rdr("RFQ_PNAME")?.ToString().Trim()
                        End If
                    End Using
                End Using
            End Using

            ' 4) Open the PO file form and populate its controls
            Using poForm As New frmpofile()
                With poForm
                    .lblprid.Text = prId.ToString()
                    .lblprno.Text = prNo
                    .txtpono.Text = String.Empty
                    .dtpodate.Value = prDate.AddDays(7).ToString("MM/dd/yyyy")
                    .txtmproc.Text = rfqMProc
                    .txtstatus.Text = "Processing"
                    .txtncompany.Text = nCompany
                    .txtcaddress.Text = cAddress
                    .lblsupname.Text = supName
                    .txtctin.Text = String.Empty
                    .txtpurpose.Text = prPurpose
                    .txtpdelivery.Text = "MPOS"
                    ' if you want the delivery date blank, you can either set
                    ' if you want the delivery date blank, you can either set
                    ' .dtpDelivery.Value = DateTime.Today or leave its default.
                    ' .dtpDelivery.Value = DateTime.Today
                    .txtfcluster.Text = rfqFunds
                    .cmdprint.Enabled = False
                    .cmdsave.Text = "Save"
                    Me.Dispose()
                    .ShowDialog()

                End With
            End Using


        Catch ex As Exception
            MessageBox.Show("Error preparing PO form: " & ex.Message,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub






    Private Sub comboprno_TextChanged(sender As Object, e As EventArgs) Handles comboprno.TextChanged
        _selectedPRNo = String.Empty
        cmdcontinue.Enabled = False

        Dim txt = comboprno.Text.Trim()
        If txt.Length > 0 Then
            LoadPRSuggestions(txt)
            comboprno.DroppedDown = True
            Cursor.Show()                       ' <— force the pointer back
            comboprno.SelectionStart = txt.Length
        Else
            LoadPRSuggestions("")
        End If
    End Sub

    Private Sub comboprno_DropDown(sender As Object, e As EventArgs) Handles comboprno.DropDown
        Cursor.Show()
    End Sub

    Private Sub Comboprno_SelectedIndexChanged(sender As Object, e As EventArgs) Handles comboprno.SelectedIndexChanged

    End Sub
End Class