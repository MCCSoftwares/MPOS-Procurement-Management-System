Imports System.Data.SqlClient

Public Class frmabstractadd

    ' Holds the PRNO the user actually picked
    Private _selectedPRNo As String = String.Empty

    Private Sub frmabstractadd_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' 1) Initial load: fetch all RFQ_PRNOs
        LoadPRSuggestions(String.Empty)

        ' 2) Enable autocomplete behavior
        comboabsprno.AutoCompleteMode = AutoCompleteMode.SuggestAppend
        comboabsprno.AutoCompleteSource = AutoCompleteSource.CustomSource

        ' 3) Start with Continue disabled
        cmdcontinue.Enabled = False
    End Sub

    ''' <summary>
    ''' Queries TBL_RFQ for distinct RFQ_PRNOs matching the filter (or all if blank),
    ''' then repopulates comboabsprno.Items and its AutoCompleteCustomSource.
    ''' Only PRs with status "[RFQ] Completed" are included (ready for AOB).
    ''' </summary>
    Private Sub LoadPRSuggestions(filter As String)
        Dim prList As New List(Of String)()

        Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
            conn.Open()

            Dim sql As String =
"SELECT DISTINCT rfq.RFQ_PRNO
   FROM TBL_RFQ AS rfq
   INNER JOIN TBL_PR  AS pr
           ON rfq.RFQ_PRID = pr.ID
  WHERE pr.PRSTATUS = '[RFQ] Completed'"

            If filter.Length > 0 Then
                sql &= vbCrLf & "    AND rfq.RFQ_PRNO LIKE @pattern"
            End If

            sql &= vbCrLf & "ORDER BY rfq.RFQ_PRNO;"

            Using cmd As New SqlCommand(sql, conn)
                If filter.Length > 0 Then
                    cmd.Parameters.Add("@pattern", SqlDbType.NVarChar, 100).Value = "%" & filter & "%"
                End If

                Using rdr = cmd.ExecuteReader()
                    While rdr.Read()
                        If Not rdr.IsDBNull(0) Then
                            prList.Add(rdr.GetString(0))
                        End If
                    End While
                End Using
            End Using
        End Using

        ' Rebuild the combo’s items and autocomplete list
        comboabsprno.Items.Clear()
        Dim ac As New AutoCompleteStringCollection()
        For Each pr In prList
            comboabsprno.Items.Add(pr)
            ac.Add(pr)
        Next
        comboabsprno.AutoCompleteCustomSource = ac
    End Sub

    ''' <summary>
    ''' Fired when the user selects an item from the drop-down.
    ''' Locks in the choice and enables Continue.
    ''' </summary>
    Private Sub comboabsprno_SelectionChangeCommitted(sender As Object, e As EventArgs) _
        Handles comboabsprno.SelectionChangeCommitted

        If comboabsprno.SelectedIndex >= 0 Then
            _selectedPRNo = comboabsprno.SelectedItem.ToString()
            comboabsprno.Text = _selectedPRNo     ' ensure exact value
            comboabsprno.DroppedDown = False
            cmdcontinue.Enabled = True
        End If
    End Sub

    ''' <summary>
    ''' Validate and proceed to the abstract-file form, filling controls as requested.
    ''' </summary>
    Private Sub cmdcontinue_Click(sender As Object, e As EventArgs) Handles cmdcontinue.Click
        Try
            Dim input = comboabsprno.Text.Trim()

            ' Allow typed exact match as well as selection
            If String.IsNullOrEmpty(_selectedPRNo) Then
                If comboabsprno.Items.Contains(input) Then
                    _selectedPRNo = input
                Else
                    MessageBox.Show("Please select a valid PR number from the list.", "Validation",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    comboabsprno.DroppedDown = True
                    comboabsprno.SelectionStart = input.Length
                    Return
                End If
            End If

            ' Pull the RFQ record for the chosen PR No.
            Dim rfqPrId As Integer = 0
            Dim rfqMProc As String = ""
            Dim rfqPurpose As String = ""
            Dim rfqOEUser As String = ""
            Dim rfqId As Integer = 0 ' kept locally in case you need it later
            Dim TTYPE As String

            Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
                conn.Open()
                Const sql As String =
                "SELECT TOP 1
                        ID,
                        RFQ_PRID,
                        RFQ_PRNO,
                        RFQ_MPROC,
                        RFQ_PURPOSE,
                        RFQ_OEUSER,
                        RFQ_TTYPE
                   FROM TBL_RFQ
                  WHERE RFQ_PRNO = @prno;"

                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.Add("@prno", SqlDbType.NVarChar, 100).Value = _selectedPRNo

                    Using rdr = cmd.ExecuteReader()
                        If rdr.Read() Then
                            rfqId = If(rdr.IsDBNull(0), 0, Convert.ToInt32(rdr(0)))
                            rfqPrId = If(rdr.IsDBNull(1), 0, Convert.ToInt32(rdr(1)))
                            ' rdr(2) is RFQ_PRNO (already have as _selectedPRNo)
                            rfqMProc = If(rdr.IsDBNull(3), "", rdr(3).ToString())
                            rfqPurpose = If(rdr.IsDBNull(4), "", rdr(4).ToString())
                            rfqOEUser = If(rdr.IsDBNull(5), "", rdr(5).ToString())
                            TTYPE = If(rdr.IsDBNull(6), "", rdr(6).ToString())
                        Else
                            MessageBox.Show("RFQ record not found.", "Error",
                                            MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Return
                        End If
                    End Using
                End Using
            End Using

            ' Open frmabstractfile and fill the requested controls
            Dim absForm As New frmabstractfile()

            ' REQUIRED MAPPINGS (as requested)
            absForm.txtprno.Text = _selectedPRNo               ' TBL_RFQ.RFQ_PRNO
            absForm.lblprid.Text = rfqPrId.ToString()          ' TBL_RFQ.RFQ_PRID
            absForm.txtstatus.Text = "Pending"                 ' literal
            absForm.txtpurpose.Text = rfqPurpose               ' TBL_RFQ.RFQ_PURPOSE
            absForm.txtmproc.Text = rfqMProc                   ' TBL_RFQ.RFQ_MPROC
            absForm.txteuser.Text = rfqOEUser
            absForm.lblrfqid.Text = rfqId
            absForm.LBLTTYPE.Text = TTYPE

            ' TBL_RFQ.RFQ_OEUSER

            ' Optional quality-of-life defaults you likely still want:
            ' (Uncomment if these controls exist and you want them set)
            'absForm.lblrfqid.Text = rfqId.ToString()
            'absForm.txtdabstract.Text = DateTime.Now.ToString("MM/dd/yyyy")
            absForm.cmdprint.Enabled = False
            absForm.cmdrefresh.Enabled = False
            absForm.cmdsubmit.Enabled = False
            absForm.cmdscancel.Enabled = False
            absForm.panelless.Enabled = False
            Me.Dispose()
            absForm.ShowDialog()

        Catch ex As Exception
            MessageBox.Show("Error while continuing to Abstract form: " & ex.Message,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ''' <summary>
    ''' On every text change, clear any previous selection, reload suggestions using LIKE,
    ''' and re-open the dropdown.
    ''' </summary>
    Private Sub comboabsprno_TextChanged(sender As Object, e As EventArgs) Handles comboabsprno.TextChanged
        _selectedPRNo = String.Empty
        cmdcontinue.Enabled = False

        Dim txt = comboabsprno.Text.Trim()
        If txt.Length > 0 Then
            LoadPRSuggestions(txt)
            comboabsprno.DroppedDown = True
            Cursor.Show()
            comboabsprno.SelectionStart = txt.Length
        Else
            LoadPRSuggestions(String.Empty)
        End If
    End Sub

    Private Sub comboabsprno_DropDown(sender As Object, e As EventArgs) Handles comboabsprno.DropDown
        Cursor.Show()
    End Sub

End Class
