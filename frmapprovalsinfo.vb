Imports System.Data.SqlClient

Public Class frmApprovalsInfo

    Private Sub frmApprovalsInfo_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            ' ─────────────────────────────────────────────────────────────
            ' Reset / Base config
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

                ' No borders (flat look)
                .BorderStyle = BorderStyle.None
                .CellBorderStyle = DataGridViewCellBorderStyle.None
                .ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
                .AdvancedCellBorderStyle.Left = DataGridViewAdvancedCellBorderStyle.None
                .AdvancedCellBorderStyle.Right = DataGridViewAdvancedCellBorderStyle.None
                .AdvancedCellBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.None
                .AdvancedCellBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.None
            End With

            ' ─────────────────────────────────────────────────────────────
            ' Define columns (Approver + Status)
            ' ─────────────────────────────────────────────────────────────
            DataGridView1.ColumnCount = 2
            With DataGridView1
                .Columns(0).Name = "APPROVAL_APPROVERS"
                .Columns(0).HeaderText = "Approver Name"
                .Columns(0).DataPropertyName = "APPROVAL_APPROVERS"
                .Columns(0).Width = 180

                .Columns(1).Name = "APPROVAL_STATUS"
                .Columns(1).HeaderText = "Status"
                .Columns(1).DataPropertyName = "APPROVAL_STATUS"
                .Columns(1).Width = 120
                .Columns(1).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            End With

            ' ─────────────────────────────────────────────────────────────
            ' Styling: headers, rows, fonts, colors
            ' ─────────────────────────────────────────────────────────────
            Dim hdrColor As Color = Color.FromArgb(73, 94, 252)

            With DataGridView1
                .EnableHeadersVisualStyles = False

                ' Header
                .ColumnHeadersDefaultCellStyle.BackColor = hdrColor
                .ColumnHeadersDefaultCellStyle.ForeColor = Color.White
                .ColumnHeadersDefaultCellStyle.SelectionBackColor = hdrColor
                .ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White
                .ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 11.0!, FontStyle.Bold)
                .ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
                .ColumnHeadersHeight = 40
                .ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing

                ' Rows
                .DefaultCellStyle.Font = New Font("Segoe UI", 10.0!)
                .DefaultCellStyle.ForeColor = Color.Black
                .RowsDefaultCellStyle.BackColor = Color.White
                .AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250)
                .DefaultCellStyle.SelectionBackColor = Color.FromArgb(210, 222, 255)
                .DefaultCellStyle.SelectionForeColor = Color.Black

                ' Row spacing
                .RowTemplate.Height = 30
                .RowTemplate.DefaultCellStyle.Padding = New Padding(0, 4, 0, 4)
            End With

            ' Smooth painting
            EnableDoubleBuffering(DataGridView1)

            ' Finally load the data
            LoadApprovalsInfo()

        Catch ex As Exception
            MessageBox.Show("Error initializing approvals grid: " & ex.Message,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Public Sub LoadApprovalsInfo()
        ' 1) Get PRID from frmAbstractFile
        Dim prIdValue As Integer
        If Not Integer.TryParse(frmabstractfile.lblprid.Text, prIdValue) Then
            MessageBox.Show("Invalid PR ID.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        ' 2) Query the approvals table
        Dim dt As New DataTable()
        Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
            conn.Open()
            Using cmd As New SqlCommand("
                SELECT 
                    APPROVAL_APPROVERS, 
                    APPROVAL_STATUS
                  FROM TBL_APPROVAL
                 WHERE PRID          = @prid
                   AND APPROVAL_TYPE = 'Abstract Of Bids';", conn)
                cmd.Parameters.AddWithValue("@prid", prIdValue)
                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using
            End Using
        End Using

        ' 3) Bind to grid
        DataGridView1.AutoGenerateColumns = True
        DataGridView1.DataSource = dt

    End Sub

End Class
