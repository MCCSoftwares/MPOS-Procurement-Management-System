Imports System.Data.SqlClient
Imports System.IO
Public Class frmapprovals


    Private Sub frmapprovals_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            ' ─────────────────────────────────────────────────────────────
            ' Base grid setup
            ' ─────────────────────────────────────────────────────────────
            With DataGridView1
                .DataSource = Nothing
                .Rows.Clear()
                .Columns.Clear()
                .AutoGenerateColumns = False
                .RowHeadersVisible = False
                .AllowUserToAddRows = False
                .AllowUserToResizeRows = False
                .MultiSelect = False
                .SelectionMode = DataGridViewSelectionMode.FullRowSelect

                .BorderStyle = BorderStyle.None
                .CellBorderStyle = DataGridViewCellBorderStyle.None
                .ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
                .EnableHeadersVisualStyles = False
            End With

            ' ─────────────────────────────────────────────────────────────
            ' Define columns
            ' ─────────────────────────────────────────────────────────────
            DataGridView1.ColumnCount = 6
            With DataGridView1
                .Columns(0).Name = "ID"
                .Columns(0).HeaderText = "ID"
                .Columns(0).DataPropertyName = "ID"
                .Columns(0).Visible = False

                .Columns(1).Name = "PRID"
                .Columns(1).HeaderText = "PRID"
                .Columns(1).DataPropertyName = "PRID"
                .Columns(1).Visible = False

                .Columns(2).Name = "PRNO"
                .Columns(2).HeaderText = "PR Number"
                .Columns(2).DataPropertyName = "PRNO"

                .Columns(3).Name = "APPROVAL_TYPE"
                .Columns(3).HeaderText = "Form"
                .Columns(3).DataPropertyName = "APPROVAL_TYPE"

                .Columns(4).Name = "APPROVAL_NAME"
                .Columns(4).HeaderText = "Requested by"
                .Columns(4).DataPropertyName = "APPROVAL_NAME"

                .Columns(5).Name = "APPROVAL_STATUS"
                .Columns(5).HeaderText = "Status"
                .Columns(5).DataPropertyName = "APPROVAL_STATUS"
            End With

            ' ─────────────────────────────────────────────────────────────
            ' Add a Preview button column
            ' ─────────────────────────────────────────────────────────────
            Dim btnCol As New DataGridViewButtonColumn() With {
                .Name = "PreviewPdf",
                .HeaderText = "Preview",
                .Text = "Open PDF",
                .UseColumnTextForButtonValue = True
            }
            DataGridView1.Columns.Add(btnCol)

            ' ─────────────────────────────────────────────────────────────
            ' Style headers & rows (your usual Calibri/Segoe style, etc.)
            ' ─────────────────────────────────────────────────────────────
            Dim hdrColor As Color = Color.FromArgb(86, 106, 255)
            With DataGridView1
                .ColumnHeadersDefaultCellStyle.BackColor = hdrColor
                .ColumnHeadersDefaultCellStyle.ForeColor = Color.White
                .ColumnHeadersDefaultCellStyle.SelectionBackColor = hdrColor
                .ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White
                .ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 10, FontStyle.Bold)
                .ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
                .ColumnHeadersHeight = 48
                .ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing

                .DefaultCellStyle.Font = New Font("Segoe UI", 9, FontStyle.Regular)
                .DefaultCellStyle.ForeColor = Color.Black
                .RowsDefaultCellStyle.BackColor = Color.White
                .AlternatingRowsDefaultCellStyle.BackColor = Color.Gainsboro
                .DefaultCellStyle.SelectionBackColor = Color.FromArgb(210, 222, 255)
                .DefaultCellStyle.SelectionForeColor = Color.Black

                .RowTemplate.Height = 30
            End With

            EnableDoubleBuffering(DataGridView1)

            ' ─────────────────────────────────────────────────────────────
            ' Finally, load data
            ' ─────────────────────────────────────────────────────────────
            LoadApprovals()

        Catch ex As Exception
            MessageBox.Show("Error initializing approvals grid: " & ex.Message,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Public Sub LoadApprovals()
        Dim approver As String = "Administrator"
        Dim dt As New DataTable()

        Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
            conn.Open()

            ' We wrap the comma‑separated list and use LIKE to ensure we only match whole names
            Const sql As String = "
            SELECT
               ID,
               PRNO,PRID,
               APPROVAL_TYPE,
               APPROVAL_NAME,
               APPROVAL_STATUS
            FROM TBL_APPROVAL
            WHERE APPROVAL_APPROVERS = @approver AND APPROVAL_STATUS = @pending
            ORDER BY DATES DESC;"

            Using cmd As New SqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@approver", approver)
                cmd.Parameters.AddWithValue("@pending", "Pending")
                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using
            End Using
        End Using

        DataGridView1.DataSource = dt
    End Sub


    Private Sub DataGridView1_CellContentClick(
        sender As Object,
        e As DataGridViewCellEventArgs
    ) Handles DataGridView1.CellContentClick

        ' Only handle the Preview button column
        If e.RowIndex < 0 OrElse
       DataGridView1.Columns(e.ColumnIndex).Name <> "PreviewPdf" Then
            Return
        End If

        Try
            ' 1) Get the bound row’s DataRowView
            Dim drv = TryCast(DataGridView1.Rows(e.RowIndex).DataBoundItem, DataRowView)
            If drv Is Nothing Then
                MessageBox.Show("Cannot determine the record.", "Preview PDF",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            ' 2) Extract both ID and PRNO
            Dim approvalId As Integer = Convert.ToInt32(drv("ID"))
            Dim prNo As String = drv("PRNO").ToString().Replace("/", "-") ' sanitize

            ' 3) Fetch the PDF blob
            Dim pdfBytes() As Byte
            Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
                conn.Open()
                Using cmd As New SqlCommand(
                "SELECT APPROVAL_PDF FROM TBL_APPROVAL WHERE ID = @id", conn)
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = approvalId
                    Dim result = cmd.ExecuteScalar()
                    If result Is Nothing OrElse Convert.IsDBNull(result) Then
                        MessageBox.Show("No PDF attached for this record.", "Preview PDF",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information)
                        Return
                    End If
                    pdfBytes = CType(result, Byte())
                End Using
            End Using

            ' 4) Write out to a temp file named after the PR number
            Dim tempFile As String = Path.Combine(
            Path.GetTempPath(), $"{prNo}.pdf")
            File.WriteAllBytes(tempFile, pdfBytes)

            ' 5) Launch default PDF viewer/browser
            Process.Start(New ProcessStartInfo(tempFile) With {
            .UseShellExecute = True
        })

        Catch ex As Exception
            MessageBox.Show("Error previewing PDF: " & ex.Message,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub




    ''' <summary>
    ''' Enables double buffering to reduce flicker.
    ''' </summary>
    Private Sub EnableDoubleBuffering(dgv As DataGridView)
        Dim prop = dgv.GetType().GetProperty("DoubleBuffered",
                     Reflection.BindingFlags.Instance Or Reflection.BindingFlags.NonPublic)
        prop?.SetValue(dgv, True, Nothing)
    End Sub


    Private Sub cmdpprove_Click(sender As Object, e As EventArgs) Handles cmdpprove.Click
        Try
            ' 1) Ensure a row is selected
            If DataGridView1.CurrentRow Is Nothing Then
                MessageBox.Show("Please select an approval to process.", "Approve",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            ' 2) Retrieve the Approval ID, PRID, PRNO and APPROVAL_TYPE
            Dim drv = TryCast(DataGridView1.CurrentRow.DataBoundItem, DataRowView)
            If drv Is Nothing Then
                MessageBox.Show("Unable to determine selected record.", "Approve",
                                MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            Dim approvalId As Integer = Convert.ToInt32(drv("ID"))
            Dim prID As Integer = Convert.ToInt32(drv("PRID"))        ' ← now available
            Dim prNo As String = drv("PRNO").ToString()
            Dim approvalType As String = drv("APPROVAL_TYPE").ToString()

            ' ───────────────────────────────────────────────────────────────
            ' PURCHASE ORDER BRANCH (exactly your original, unchanged)
            ' ───────────────────────────────────────────────────────────────
            If approvalType = "Purchase Request" Then
                Dim confirmPO = MessageBox.Show(
                    $"Are you sure you want to APPROVE PR {prNo}?",
                    "Confirm Approval",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question)
                If confirmPO <> DialogResult.Yes Then Return

                Using conn As New SqlClient.SqlConnection(My.Forms.frmmain.txtdb.Text)
                    conn.Open()
                    Using tran = conn.BeginTransaction()
                        ' 4a) Mark approval as Approved
                        Using cmd As New SqlClient.SqlCommand(
                            "UPDATE TBL_APPROVAL
                            SET APPROVAL_STATUS = 'Approved'
                          WHERE ID = @id;", conn, tran)
                            cmd.Parameters.Add("@id", SqlDbType.Int).Value = approvalId
                            cmd.ExecuteNonQuery()
                        End Using

                        ' 4b) Update the PR status by PRNO
                        Using cmd2 As New SqlClient.SqlCommand(
                            "UPDATE TBL_PR
                            SET PRSTATUS = @status,
                                APPBY     = @appby
                          WHERE PRNO     = @prno;", conn, tran)
                            cmd2.Parameters.Add("@status", SqlDbType.NVarChar, 50).Value = "[P.R] Approved"
                            cmd2.Parameters.Add("@appby", SqlDbType.NVarChar, 100).Value = My.Forms.frmmain.lblaname.Text.Trim()
                            cmd2.Parameters.Add("@prno", SqlDbType.NVarChar, 50).Value = prNo     ' ← use prNo here!
                            cmd2.ExecuteNonQuery()
                        End Using

                        tran.Commit()
                    End Using
                End Using

                MessageBox.Show($"PR {prNo} has been approved successfully.",
                                "Approved", MessageBoxButtons.OK, MessageBoxIcon.Information)
                LoadApprovals()
                NotificationHelper.AddNotification(
                    $"Purchase Request for PR Number {prNo} has been Approved by {frmmain.lblaname.Text.Trim()}.")
                Return
            End If


            ' ───────────────────────────────────────────────────────────────
            ' ABSTRACT OF BIDS BRANCH
            ' ───────────────────────────────────────────────────────────────
            If approvalType = "Abstract of Bids" Then
                Dim confirmAOB = MessageBox.Show(
                    $"Are you sure you want to APPROVE Abstract Of Bids for PR {prNo}?",
                    "Confirm Approval",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question)
                If confirmAOB <> DialogResult.Yes Then Return

                Dim pendingCount As Integer

                Using conn As New SqlClient.SqlConnection(My.Forms.frmmain.txtdb.Text)
                    conn.Open()
                    Using tran = conn.BeginTransaction()
                        ' a) Mark this approval row as Approved
                        Using cmd As New SqlClient.SqlCommand(
                            "UPDATE TBL_APPROVAL
                            SET APPROVAL_STATUS = 'Approved'
                          WHERE ID = @id;", conn, tran)
                            cmd.Parameters.Add("@id", SqlDbType.Int).Value = approvalId
                            cmd.ExecuteNonQuery()
                        End Using

                        ' b) Count remaining un‑approved AOB rows for this PRID
                        Using checkCmd As New SqlClient.SqlCommand(
                            "SELECT COUNT(*) 
                           FROM TBL_APPROVAL 
                          WHERE PRID = @prid 
                            AND APPROVAL_TYPE = 'Abstract Of Bids' 
                            AND APPROVAL_STATUS <> 'Approved';", conn, tran)
                            checkCmd.Parameters.Add("@prid", SqlDbType.Int).Value = prID
                            pendingCount = Convert.ToInt32(checkCmd.ExecuteScalar())
                        End Using

                        ' c) If this was the last approver, update both PR and Abstract
                        If pendingCount = 0 Then
                            ' Update PR status by its ID
                            Using cmdPr As New SqlClient.SqlCommand(
                                "UPDATE TBL_PR
                                SET PRSTATUS = '[AOB] Approved'
                              WHERE ID = @prid;", conn, tran)
                                cmdPr.Parameters.Add("@prid", SqlDbType.Int).Value = prID
                                cmdPr.ExecuteNonQuery()
                            End Using

                            ' Update Abstract status
                            Using cmdAbs As New SqlClient.SqlCommand(
                                "UPDATE TBL_ABSTRACT
                                SET ABS_STATUS = 'Approved'
                              WHERE ABS_PRID = @prid;", conn, tran)
                                cmdAbs.Parameters.Add("@prid", SqlDbType.Int).Value = prID
                                cmdAbs.ExecuteNonQuery()
                            End Using

                            Using cmdRfq As New SqlClient.SqlCommand(
                              "UPDATE TBL_RFQ
                                SET RFQ_STATUS = 'Approved'
                              WHERE RFQ_PRID = @prid;", conn, tran)
                                cmdRfq.Parameters.Add("@prid", SqlDbType.Int).Value = prID
                                cmdRfq.ExecuteNonQuery()
                            End Using
                        End If

                        tran.Commit()
                    End Using
                End Using

                ' 5) Notify the user and refresh
                If pendingCount = 0 Then
                    MessageBox.Show(
                        $"Abstract of Bids for PR {prNo} has been fully approved and status set to [AOB] Approved.",
                        "Approved", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Else
                    MessageBox.Show(
                        $"Abstract of Bids for PR {prNo} has been approved by {frmmain.lblaname.Text.Trim()}. Waiting for other approvals.",
                        "Approved", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If

                LoadApprovals()
                NotificationHelper.AddNotification(
                    $"Abstract Of Bids for PR Number {prNo} has been approved by {frmmain.lblaname.Text.Trim()}.")
                Return
            End If


            ' ───────────────────────────────────────────────────────────────
            ' Fallback for unexpected types
            ' ───────────────────────────────────────────────────────────────
            MessageBox.Show("Unknown approval type: " & approvalType, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)

        Catch ex As Exception
            MessageBox.Show("Error approving request: " & ex.Message,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub




End Class