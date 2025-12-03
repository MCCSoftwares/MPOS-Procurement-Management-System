Imports System.Data.SqlClient

Public Class frmrfqadd

    ' Holds the PRNO the user actually picked
    Private _selectedPRNo As String = String.Empty

    Private Sub frmrfqadd_Load(sender As Object, e As EventArgs) Handles MyBase.Load
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
                WHERE PRSTATUS = '[P.R] Approved'
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
        Dim input = comboprno.Text.Trim()

        ' 1) Validate or accept full-typed match
        If String.IsNullOrEmpty(_selectedPRNo) Then
            If comboprno.Items.Contains(input) Then
                _selectedPRNo = input
            Else
                MessageBox.Show("Please select a valid PR number from the list.", "Validation",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
                comboprno.DroppedDown = True
                comboprno.SelectionStart = input.Length
                Return
            End If
        End If

        ' 2) Fetch ID, PRDATE, FCLUSTER, PURPOSE and OFFSEC in one query
        Dim prId As Integer
        Dim prDate As DateTime
        Dim fcluster As String
        Dim purpose As String
        Dim offsec As String
        Dim ttype As String

        Using conn As New SqlConnection(frmmain.txtdb.Text)
            conn.Open()
            Const sql = "
            SELECT 
              ID,
              PTTYPE,
              PRDATE, 
              FCLUSTER, 
              PRPURPOSE, 
              OFFSEC
            FROM TBL_PR
            WHERE PRNO = @PRNO
        "
            Using cmd As New SqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@PRNO", _selectedPRNo)
                Using rdr = cmd.ExecuteReader()
                    If rdr.Read() Then
                        prId = rdr.GetInt32(rdr.GetOrdinal("ID"))
                        prDate = rdr.GetDateTime(rdr.GetOrdinal("PRDATE"))
                        fcluster = rdr("FCLUSTER").ToString()
                        purpose = rdr("PRPURPOSE").ToString()
                        offsec = rdr("OFFSEC").ToString()
                        ttype = rdr("PTTYPE").ToString()
                    Else
                        MessageBox.Show("PR record not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return
                    End If
                End Using
            End Using
        End Using
        frmrfqfile.Dispose()

        ' 3) Populate and show your RFQ form
        Using rfqForm As New frmrfqfile()
            rfqForm.lblprid.Text = prId.ToString()
            rfqForm.txtprno.Text = _selectedPRNo
            rfqForm.txtprdate.Text = prDate.ToString("MM/dd/yyyy")
            rfqForm.dtsdate.Value = DateValue(Now).AddDays(3)
            rfqForm.txtfund.Text = fcluster
            rfqForm.txtpurpose.Text = purpose
            rfqForm.txtoeuser.Text = offsec
            rfqForm.dtrecieved.Value = DateTime.Now.ToString("MM/dd/yyyy")
            rfqForm.txtstatus.Text = "Processing"
            rfqForm.cmdaddsup.Enabled = False
            rfqForm.cmddelete.Enabled = False
            rfqForm.cmdedit.Enabled = False
            rfqForm.cmdprint.Enabled = False
            rfqForm.LBLTTYPE.Text = ttype
            rfqForm.txtcanby.Text = frmmain.lblaname.Text
            rfqForm.lbltitle.Text = "REQUEST FOR QUOTATION | PR #:" & _selectedPRNo
            rfqForm.ShowDialog()
        End Using
        Me.Dispose()
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

End Class
