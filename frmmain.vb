Imports System.IO
Imports System.Data.SqlClient

Public Class frmmain

    Inherits Form

    Private _connMonitor As ConnectionLatencyMonitor
    Private Sub Cmdpatient_Click(sender As Object, e As EventArgs) Handles cmdpatient.Click
        frmprocurement.ShowDialog()
    End Sub


    Private Sub Button1_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles txtconfig.Click
        frmconfig.Dispose()
        frmconfig.ShowDialog()
    End Sub

    Private Sub frmmain_Load(sender As Object, e As EventArgs) Handles MyBase.Load

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

                ' 🔲 Flat look
                .BorderStyle = BorderStyle.None
                .CellBorderStyle = DataGridViewCellBorderStyle.None
                .ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
                .AdvancedCellBorderStyle.Left = DataGridViewAdvancedCellBorderStyle.None
                .AdvancedCellBorderStyle.Right = DataGridViewAdvancedCellBorderStyle.None
                .AdvancedCellBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.None
                .AdvancedCellBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.None
            End With

            ' ─────────────────────────────────────────────────────────────
            ' Columns (ONLY the fields you specified)
            ' ─────────────────────────────────────────────────────────────
            DataGridView1.ColumnCount = 2

            With DataGridView1
                .Columns(0).Name = "NOT_DT"
                .Columns(0).HeaderText = "Date and Time"
                .Columns(0).DataPropertyName = "NOT_DT"
                .Columns(0).DefaultCellStyle.Format = "MM/dd/yyyy hh:mm tt"
                .Columns(0).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
                .Columns(0).Width = 200

                .Columns(1).Name = "NOT_MESSAGE"
                .Columns(1).HeaderText = "Message"
                .Columns(1).DataPropertyName = "NOT_MESSAGE"
            End With

            ' ─────────────────────────────────────────────────────────────
            ' Styling
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
                .ColumnHeadersHeight = 52
                .ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing

                ' Rows
                .DefaultCellStyle.Font = New Font("Segoe UI", 10.0!)
                .DefaultCellStyle.ForeColor = Color.Black
                .RowsDefaultCellStyle.BackColor = Color.White
                .AlternatingRowsDefaultCellStyle.BackColor = Color.White
                .DefaultCellStyle.SelectionBackColor = Color.FromArgb(210, 222, 255)
                .DefaultCellStyle.SelectionForeColor = Color.Black

                ' Row spacing
                .RowTemplate.Height = 34
                .RowTemplate.DefaultCellStyle.Padding = New Padding(0, 6, 0, 6)
            End With

            ' Smooth paint
            EnableDoubleBuffering(DataGridView1)

            ' ─────────────────────────────────────────────────────────────
            ' Load data
            ' ─────────────────────────────────────────────────────────────

            EnableDoubleBuffering(Me)
            ' 1) Handle clicks on the form’s background
            AddHandler Me.Click, AddressOf HideMenuIfVisible

            ' 2) Recursively attach to all child controls

            AttachGlobalClickHandler(Me)
            If lblaposition.Text = "End User" Then
                loadenduserUI()
            Else

                LoadNotifications()
                LoadDashboardCounts()
                Panel9.Visible = True
                Panel10.Visible = True
            End If

            _connMonitor = New ConnectionLatencyMonitor(
                Function() Me.txtdb.Text,
                Me.lblcstatus,
                Me,
                pollMs:=10000,        ' 10s is a nice low-noise cadence
                openTimeoutSec:=10,   ' good for Cloud SQL
                cmdTimeoutSec:=3,
                fastThresholdMs:=150,
                moderateThresholdMs:=350
            )
            _connMonitor.Start()

        Catch ex As Exception
            MessageBox.Show("Error initializing notification grid: " & ex.Message,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try



    End Sub

    Private Sub frmmain_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed
        _connMonitor?.[Stop]()
    End Sub

    Public Sub loadenduserUI()
        Panel9.Visible = False
        Panel10.Visible = False
        txtconfig.Visible = False
        cmdsupply.Enabled = False
        cmddelivery.Enabled = False
        cmdmsupply.Enabled = False
        cmdsadmin.Visible = False
        cmdissuance.Enabled = False
        cmdwac.Enabled = False
        'cmdreport.Enabled = False
    End Sub

    Public Sub loadprocurementUI()
        Panel9.Visible = True
        Panel10.Visible = True
        txtconfig.Visible = False
        cmdsupply.Enabled = False
        cmddelivery.Enabled = False
        cmdmsupply.Enabled = False
        cmdsadmin.Visible = False
        cmdissuance.Enabled = False
        cmdwac.Enabled = False
        'cmdreport.Enabled = False
    End Sub

    Public Sub loadsupplyUI()
        Panel9.Visible = True
        Panel10.Visible = True
        txtconfig.Visible = False
        cmdsupply.Enabled = True
        cmddelivery.Enabled = True
        cmdmsupply.Enabled = True
        cmdsadmin.Visible = False
        cmdissuance.Enabled = True
        cmdwac.Enabled = True
        '   cmdreport.Enabled = True
    End Sub

    Public Sub loadadminUI()
        Panel9.Visible = True
        Panel10.Visible = True
        txtconfig.Visible = True
        cmdsupply.Enabled = True
        cmddelivery.Enabled = True
        cmdmsupply.Enabled = True
        cmdsadmin.Visible = False
        cmdissuance.Enabled = True
        cmdwac.Enabled = True
        '  cmdreport.Enabled = True
    End Sub

    Public Sub loadsuperadminUI()
        Panel9.Visible = True
        Panel10.Visible = True
        txtconfig.Visible = True
        cmdsupply.Enabled = True
        cmddelivery.Enabled = True
        cmdmsupply.Enabled = True
        cmdsadmin.Visible = True
        cmdissuance.Enabled = True
        cmdwac.Enabled = True
        '  cmdreport.Enabled = True
    End Sub


    Private Sub LoadDashboardCounts()
        Try
            Dim today As Date = Date.Today
            Dim yearStart As New Date(today.Year, 1, 1)
            Dim nextYearStart As Date = yearStart.AddYears(1)
            Dim sevenDaysAgo As Date = today.AddDays(-7)

            Using conn As New SqlConnection(Me.txtdb.Text) ' your connection string textbox on frmmain
                conn.Open()

                ' One trip, four result sets:
                Dim sql As String =
                "DECLARE @YearStart date = @pYearStart, @NextYearStart date = @pNextYearStart, @Today date = @pToday, @SevenDaysAgo date = @pSevenDaysAgo;" & vbCrLf &
                "-- 1) PR this year" & vbCrLf &
                "SELECT COUNT(*) AS Cnt FROM TBL_PR WITH (NOLOCK) WHERE PRDATE >= @YearStart AND PRDATE < @NextYearStart;" & vbCrLf &
                "-- 2) PO this year" & vbCrLf &
                "SELECT COUNT(*) AS Cnt FROM TBL_PO WITH (NOLOCK) WHERE PO_DATE >= @YearStart AND PO_DATE < @NextYearStart;" & vbCrLf &
                "-- 3) IAR within last 7 days (based on DDATE)" & vbCrLf &
                "SELECT COUNT(*) AS Cnt FROM TBL_IAR WITH (NOLOCK) WHERE CONVERT(date, DDATE) >= @SevenDaysAgo AND CONVERT(date, DDATE) <= @Today;" & vbCrLf &
                "-- 4) Total inventory rows" & vbCrLf &
                "SELECT COUNT(*) AS Cnt FROM TBL_INVENTORY WITH (NOLOCK);"

                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@pYearStart", yearStart)
                    cmd.Parameters.AddWithValue("@pNextYearStart", nextYearStart)
                    cmd.Parameters.AddWithValue("@pToday", today)
                    cmd.Parameters.AddWithValue("@pSevenDaysAgo", sevenDaysAgo)

                    Using rdr = cmd.ExecuteReader()
                        ' 1) PR count this year -> lblprno.Text
                        Dim prCount As Integer = 0
                        If rdr.Read() AndAlso Not rdr.IsDBNull(0) Then prCount = rdr.GetInt32(0)
                        If Not rdr.NextResult() Then Throw New Exception("Missing PO result set.")

                        ' 2) PO count this year -> lblpono.Text
                        Dim poCount As Integer = 0
                        If rdr.Read() AndAlso Not rdr.IsDBNull(0) Then poCount = rdr.GetInt32(0)
                        If Not rdr.NextResult() Then Throw New Exception("Missing IAR result set.")

                        ' 3) IAR within last 7 days -> lbliar7.Text (rename if your label is different)
                        Dim iar7Count As Integer = 0
                        If rdr.Read() AndAlso Not rdr.IsDBNull(0) Then iar7Count = rdr.GetInt32(0)
                        If Not rdr.NextResult() Then Throw New Exception("Missing inventory result set.")

                        ' 4) Total inventory rows -> lblinventory.Text (rename if your label is different)
                        Dim inventoryCount As Integer = 0
                        If rdr.Read() AndAlso Not rdr.IsDBNull(0) Then inventoryCount = rdr.GetInt32(0)

                        ' Assign to your labels
                        lblprno.Text = prCount.ToString("N0")
                        lblpono.Text = poCount.ToString("N0")

                        ' Make sure these labels exist on frmmain or change the names below:
                        If Me.Controls.Find("lbliar7", True).Any() Then
                            lbldelivery.Text = iar7Count.ToString("N0")
                        End If
                        If Me.Controls.Find("lblinventory", True).Any() Then
                            lblinventory.Text = inventoryCount.ToString("N0")
                        End If
                    End Using
                End Using
            End Using

        Catch ex As Exception
            MessageBox.Show("Failed to load counts: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Public Sub LoadNotifications()
        Try
            Dim dt As New DataTable()
            Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
                conn.Open()
                Const sql As String = "
                SELECT TOP 100
                    NOT_DT,
                    NOT_MESSAGE
                FROM TBL_NOTIF
                ORDER BY NOT_DT DESC;"
                Using da As New SqlDataAdapter(sql, conn)
                    da.Fill(dt)
                End Using
            End Using

            DataGridView1.DataSource = dt

        Catch ex As Exception
            MessageBox.Show("Error loading notifications: " & ex.Message,
                        "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub



    Public Sub getconnstring()
        Try
            Dim configPath As String = Application.StartupPath & "\server.config"

            If File.Exists(configPath) Then
                Dim serverIP As String = ""
                Dim port As String = ""
                Dim username As String = ""
                Dim password As String = ""

                Dim lines() As String = File.ReadAllLines(configPath)

                For Each line In lines
                    If line.StartsWith("ServerIP=") Then
                        serverIP = line.Replace("ServerIP=", "").Trim()
                    ElseIf line.StartsWith("Port=") Then
                        port = line.Replace("Port=", "").Trim()
                    ElseIf line.StartsWith("Username=") Then
                        username = line.Replace("Username=", "").Trim()
                    ElseIf line.StartsWith("Password=") Then
                        password = line.Replace("Password=", "").Trim()
                    End If
                Next

                ' ✅ Build and assign the full connection string
                txtdb.Text = $"Server={serverIP},{port};Database=MPOSDB;User Id={username};Password={password};"
                frmlogin.lbldb.Text = txtdb.Text
                frmlogin.ShowDialog()
            Else
                MessageBox.Show("Configuration file not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        Catch ex As Exception
            MessageBox.Show("There was an Error:" & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub frmmain_FormClosing(
          sender As Object,
          e As FormClosingEventArgs
      ) Handles Me.FormClosing

        ' Only intercept user‑initiated closes
        If e.CloseReason = CloseReason.UserClosing Then
            Dim result = MessageBox.Show(
                "Are you sure you want to exit?",
                "Confirm Exit",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question)

            If result = DialogResult.No Then
                ' Cancel the close
                e.Cancel = True
            Else
                ' Let it close — Application.Exit is implicit once main form closes
                ' but you can call it explicitly if you have background threads:
                Application.Exit()
            End If
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        logout()
    End Sub

    Public Sub logout()
        Try
            lblaname.Text = ""
            lblaposition.Text = ""
            Me.Hide()
            frmlogin.lbldb.Text = txtdb.Text
            frmlogin.ShowDialog()

        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub

    '  Private Sub Lblaname_Click(sender As Object, e As EventArgs) Handles lblaname.Click
    '    Try
    '  If Paneluenu.Visible = False Then
    'Paneluenu.Visible = True
    '   Else
    '   Paneluenu.Visible = False
    '    End If


    ' Catch ex As Exception

    '  End Try
    ' End Sub



    Private Sub Paneluenu_Paint(sender As Object, e As PaintEventArgs) Handles Paneluenu.Paint

    End Sub

    Private Sub Paneluenu_LostFocus(sender As Object, e As EventArgs) Handles Paneluenu.LostFocus
        'Paneluenu.Visible = False
    End Sub



    Private Const WM_LBUTTONDOWN As Integer = &H201

    Protected Overrides Sub WndProc(ByRef m As Message)
        If m.Msg = WM_LBUTTONDOWN Then
            ' Extract the X/Y from lParam
            Dim lParamVal = m.LParam.ToInt32()
            Dim x = lParamVal And &HFFFF
            Dim y = (lParamVal >> 16) And &HFFFF
            Dim pt = New Point(x, y)

            ' If menu is visible, and the click is not on the menu or the toggle label, hide it
            If Paneluenu.Visible _
               AndAlso Not Paneluenu.Bounds.Contains(pt) _
               AndAlso Not lblaname.Bounds.Contains(pt) Then

                Paneluenu.Visible = False
            End If
        End If

        MyBase.WndProc(m)
    End Sub

    Private Sub AttachGlobalClickHandler(parent As Control)
        For Each ctrl As Control In parent.Controls
            ' Skip the toggle label and the menu panel (and its children)
            If ctrl Is lblaname OrElse ctrl Is Paneluenu Then
                Continue For
            End If

            ' Add handler to hide menu
            AddHandler ctrl.Click, AddressOf HideMenuIfVisible

            ' Recurse into children (unless they're inside paneluenu)
            If Not Paneluenu.Controls.Contains(ctrl) Then
                AttachGlobalClickHandler(ctrl)
            End If
        Next
    End Sub

    ''' <summary>
    ''' Hides paneluenu if it’s currently visible.
    ''' </summary>
    Private Sub HideMenuIfVisible(sender As Object, e As EventArgs)
        If Paneluenu.Visible Then
            Paneluenu.Visible = False
        End If
    End Sub

    ''' <summary>
    ''' Toggle the menu panel when lblaname is clicked.
    ''' </summary>
    Private Sub lblaname_Click(sender As Object, e As EventArgs) Handles lblaname.Click
        Paneluenu.Visible = Not Paneluenu.Visible
    End Sub

    Private Sub Button4_Click_1(sender As Object, e As EventArgs) Handles Button4.Click
        Button2.PerformClick()
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        exitsystem()
    End Sub

    Public Sub exitsystem()
        If MessageBox.Show(
                "Are you sure you want to exit?",
                "Confirm Exit",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) = vbYes Then
            Application.Exit()
        End If

    End Sub

    Private Sub Panel9_Paint(sender As Object, e As PaintEventArgs) Handles Panel9.Paint

    End Sub

    Private Sub Panel10_Paint(sender As Object, e As PaintEventArgs) Handles Panel10.Paint

    End Sub

    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles cmdapproval.Click
        Try
            'frmapprovals.Dispose()
            frmapprovals.ShowDialog()

        Catch ex As Exception

        End Try
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles cmddelivery.Click
        frmdelivery.Dispose()
        frmdelivery.ShowDialog()
    End Sub

    Private Sub Cmdadmit_Click(sender As Object, e As EventArgs) Handles cmdsupply.Click
        frminventory.Dispose()
        frminventory.ShowDialog()
    End Sub

    Private Sub Button10_Click(sender As Object, e As EventArgs) Handles cmdissuance.Click
        Try
            frmissuance.ShowDialog()
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub

    Private Sub Button11_Click(sender As Object, e As EventArgs) Handles cmdmsupply.Click
        frmequipment.Dispose()
        frmequipment.ShowDialog()

    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Try
            frmabout.Dispose()
            frmabout.ShowDialog()
        Catch ex As Exception

        End Try

    End Sub

    Private Sub FlowLayoutPanel2_Paint(sender As Object, e As PaintEventArgs) Handles FlowLayoutPanel2.Paint

    End Sub

    Private Sub Cmdsadmin_Click(sender As Object, e As EventArgs) Handles cmdsadmin.Click
        frmsuperadmin.Dispose()
        frmsuperadmin.ShowDialog()
    End Sub

    Private Sub Cmdaccount_Click(sender As Object, e As EventArgs) Handles cmdaccount.Click
        Try
            frmconfiguser.Dispose()
            frmconfiguser.lblid.Text = lbluid.Text
            frmconfiguser.LoadUser()
            frmconfiguser.ShowDialog()

        Catch ex As Exception

        End Try
    End Sub

    Private Sub Cmddisposal_Click(sender As Object, e As EventArgs) Handles cmdwac.Click
        frmwac.ShowDialog()
    End Sub


    Private Sub Button1_Click_1(sender As Object, e As EventArgs) Handles Button1.Click
        frmrsmi.ShowDialog()
    End Sub

    Private Sub Button3_Click_1(sender As Object, e As EventArgs) Handles Button3.Click
        frmStockCard.ShowDialog()
    End Sub

    Private Sub Button9_Click_1(sender As Object, e As EventArgs) Handles Button9.Click
        frmris.ShowDialog()
    End Sub

    Private Sub Button10_Click_1(sender As Object, e As EventArgs) Handles Button10.Click
        frmsupplier.ShowDialog()
    End Sub

    Private Sub Cmdrcppe_Click(sender As Object, e As EventArgs) Handles cmdrcppe.Click
        Try
            frmRCPPE.Dispose()
            frmRCPPE.ShowDialog()
        Catch ex As Exception

        End Try
    End Sub
End Class
