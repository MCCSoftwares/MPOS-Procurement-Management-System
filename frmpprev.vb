Imports System.Data.SqlClient
Imports Microsoft.Reporting.WinForms



Public Class frmpprev
    Private _lastPdf As Byte()

    Private Sub cmdsubmit_Click(sender As Object, e As EventArgs) Handles cmdsubmit.Click

        If lbltransaction.Text = "PR" Then


            ' ────────────────────────────────────────────────────────────
            ' 0) Ensure a PR is selected in frmprfile
            ' ────────────────────────────────────────────────────────────
            If String.IsNullOrWhiteSpace(My.Forms.frmprfile.lblid.Text) Then
                MessageBox.Show("No PR selected.", "Submit", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            ' ────────────────────────────────────────────────────────────
            ' 1) Confirm with the user
            ' ────────────────────────────────────────────────────────────
            Dim ask = MessageBox.Show(
            "This Purchase Request will be sent for approval and will no longer be editable." &
            vbCrLf & vbCrLf & "Do you want to continue?",
            "Send for Approval",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question)

            If ask <> DialogResult.Yes Then Return

            Try
                ' ────────────────────────────────────────────────────────────
                ' 2) Update PRSTATUS in TBL_PR
                ' ────────────────────────────────────────────────────────────
                Using conn As New SqlClient.SqlConnection(My.Forms.frmmain.txtdb.Text)
                    conn.Open()
                    Using cmd As New SqlClient.SqlCommand("
                UPDATE TBL_PR
                   SET PRSTATUS = '[P.R] For Approval'
                 WHERE ID = @id;", conn)
                        cmd.Parameters.AddWithValue("@id", Convert.ToInt32(My.Forms.frmprfile.lblid.Text))
                        cmd.ExecuteNonQuery()
                    End Using
                End Using

                MessageBox.Show("Purchase Request submitted for approval.", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)

                ' ────────────────────────────────────────────────────────────
                ' 3) Update UI on this preview form
                ' ────────────────────────────────────────────────────────────
                Me.cmdsubmit.Visible = True
                frmprfile.txtstatus.Text = "[P.R] For Approval"
                frmprfile.cmdadd.Enabled = False
                frmprfile.cmdedit.Enabled = False
                frmprfile.cmdaddapp.Enabled = False
                frmprfile.cmddelete.Enabled = False
                frmprfile.cmdsave.Enabled = False
                '  Me.cmdapprove.Enabled = False

                ' ────────────────────────────────────────────────────────────
                ' 4) Refresh the PR list in frmprfile
                ' ────────────────────────────────────────────────────────────
                ' frmprfile.LoadRecords()

                ' ────────────────────────────────────────────────────────────
                ' 5) Send a notification
                ' ────────────────────────────────────────────────────────────
                NotificationHelper.AddNotification(
                $"Purchase Request for PR Number {My.Forms.frmprfile.txtprno.Text.Trim()} " &
                $"has been submitted for approval by {My.Forms.frmmain.lblaname.Text.Trim()}.")

                ' ────────────────────────────────────────────────────────────
                ' 6) Render the RDLC preview to PDF in memory
                ' ────────────────────────────────────────────────────────────
                Dim warnings As Warning() = Nothing
                Dim streamIds As String() = Nothing
                Dim mimeType As String = Nothing
                Dim encoding As String = Nothing
                Dim extension As String = Nothing
                Dim deviceInfo As String =
              "<DeviceInfo>" &
                "<OutputFormat>PDF</OutputFormat>" &
              "</DeviceInfo>"

                Dim pdfBytes() As Byte = ReportViewer1.LocalReport.Render(
                format:="PDF",
                deviceInfo:=deviceInfo,
                mimeType:=mimeType,
                encoding:=encoding,
                fileNameExtension:=extension,
                streams:=streamIds,
                warnings:=warnings)

                ' ────────────────────────────────────────────────────────────
                ' 7) Insert into TBL_APPROVAL
                ' ────────────────────────────────────────────────────────────
                Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
                    conn.Open()
                    Const sql = "
                    INSERT INTO TBL_APPROVAL
                      (DATES,
                       PRID,
                       PRNO,
                       APPROVAL_NAME,
                       APPROVAL_TYPE,
                       APPROVAL_PDF,
                       APPROVAL_STATUS,
                       APPROVAL_APPROVERS)
                    VALUES
                      (@dates,
                       @prid,
                       @prno,
                       @name,
                       @type,
                       @pdf,
                       @status,
                       @approvers);"
                    Using cmd As New SqlCommand(sql, conn)
                        cmd.Parameters.Add("@dates", SqlDbType.DateTime).Value = DateTime.Now
                        cmd.Parameters.Add("@prid", SqlDbType.Int).Value = Convert.ToInt32(My.Forms.frmprfile.lblid.Text)
                        cmd.Parameters.Add("@prno", SqlDbType.NVarChar, 50).Value = My.Forms.frmprfile.txtprno.Text.Trim()
                        cmd.Parameters.Add("@name", SqlDbType.NVarChar, 100).Value = My.Forms.frmmain.lblaname.Text.Trim()
                        cmd.Parameters.Add("@type", SqlDbType.NVarChar, 50).Value = "Purchase Request"
                        cmd.Parameters.Add("@status", SqlDbType.NVarChar, 50).Value = "Pending"
                        cmd.Parameters.Add("@approvers", SqlDbType.NVarChar, 200).Value = "Administrator"
                        Dim p = cmd.Parameters.Add("@pdf", SqlDbType.VarBinary)
                        p.Value = If(pdfBytes IsNot Nothing AndAlso pdfBytes.Length > 0, CType(pdfBytes, Object), DBNull.Value)
                        cmd.ExecuteNonQuery()
                    End Using
                End Using

                ' ────────────────────────────────────────────────────────────
                ' 8) Refresh notifications on main form
                ' ────────────────────────────────────────────────────────────
                My.Forms.frmmain.LoadNotifications()
                My.Forms.frmpr.loadrecords()


            Catch ex As Exception
                MessageBox.Show("Error submitting approval: " & ex.ToString,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try


        End If

    End Sub
    Private Sub frmpprev_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.ReportViewer1.RefreshReport()
    End Sub

    Private Sub ReportViewer1_Load(sender As Object, e As EventArgs) Handles ReportViewer1.Load

    End Sub
End Class
