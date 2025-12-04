Imports System.Data
Imports System.Data.SqlClient
Imports System.Drawing
Imports Microsoft.Reporting.WinForms

Public Class frmRCPPE

    Private isInitializing As Boolean = True

    '────────────────────────────────────────────────────────
    '  FORM LOAD
    '────────────────────────────────────────────────────────
    Private Sub frmRCPPE_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            SetupGrid()
            LoadTypeCombo()
            LoadEndUserCombo()
        Catch ex As Exception
            MessageBox.Show("Error initializing PPE form:" & Environment.NewLine & ex.Message,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            isInitializing = False
        End Try
    End Sub

    '────────────────────────────────────────────────────────
    '  COMBO LOADERS
    '────────────────────────────────────────────────────────
    Private Sub LoadTypeCombo()
        combotype.Items.Clear()
        combotype.Items.Add("-- Select Type --")

        Using conn As New SqlConnection(frmmain.txtdb.Text)
            conn.Open()
            Const sql As String =
                "SELECT DISTINCT type_of_ppe " &
                "FROM TBL_PPE WITH (NOLOCK) " &
                "WHERE ISNULL(type_of_ppe,'') <> '' " &
                "ORDER BY type_of_ppe;"

            Using cmd As New SqlCommand(sql, conn)
                Using rdr As SqlDataReader = cmd.ExecuteReader()
                    While rdr.Read()
                        combotype.Items.Add(rdr("type_of_ppe").ToString())
                    End While
                End Using
            End Using
        End Using

        combotype.SelectedIndex = 0
    End Sub

    Private Sub LoadEndUserCombo()
        comboenduser.Items.Clear()
        comboenduser.Items.Add("-- Select End-User --")

        Using conn As New SqlConnection(frmmain.txtdb.Text)
            conn.Open()
            Const sql As String =
                "SELECT DISTINCT par_to " &
                "FROM TBL_PPE WITH (NOLOCK) " &
                "WHERE ISNULL(par_to,'') <> '' " &
                "ORDER BY par_to;"

            Using cmd As New SqlCommand(sql, conn)
                Using rdr As SqlDataReader = cmd.ExecuteReader()
                    While rdr.Read()
                        comboenduser.Items.Add(rdr("par_to").ToString())
                    End While
                End Using
            End Using
        End Using

        comboenduser.SelectedIndex = 0
    End Sub

    '────────────────────────────────────────────────────────
    '  COMBO EVENTS → LOAD GRID + HEADER FIELDS
    '────────────────────────────────────────────────────────
    Private Sub combotype_SelectedIndexChanged(sender As Object, e As EventArgs) _
        Handles combotype.SelectedIndexChanged

        If isInitializing Then Return
        LoadGridIfReady()
    End Sub

    Private Sub comboenduser_SelectedIndexChanged(sender As Object, e As EventArgs) _
        Handles comboenduser.SelectedIndexChanged

        If isInitializing Then Return
        LoadGridIfReady()
    End Sub

    Private Sub LoadGridIfReady()
        If combotype.SelectedIndex <= 0 OrElse comboenduser.SelectedIndex <= 0 Then
            DataGridView1.Rows.Clear()
            txtfcluster.Text = String.Empty
            txtpardate.Text = String.Empty
            Exit Sub
        End If

        Dim ppeType As String = combotype.Text.Trim()
        Dim endUser As String = comboenduser.Text.Trim()

        LoadPPERecords(ppeType, endUser)
        LoadHeaderFieldsForSelection(ppeType, endUser)
    End Sub

    '────────────────────────────────────────────────────────
    '  LOAD HEADER FIELDS (par_date / fcluster)
    '────────────────────────────────────────────────────────
    Private Sub LoadHeaderFieldsForSelection(ppeType As String, endUser As String)
        txtfcluster.Text = String.Empty
        txtpardate.Text = String.Empty

        Try
            Using conn As New SqlConnection(frmmain.txtdb.Text)
                conn.Open()

                Const sql As String =
                    "SELECT TOP 1 par_date, fcluster " &
                    "FROM TBL_PPE WITH (NOLOCK) " &
                    "WHERE type_of_ppe = @type AND par_to = @par " &
                    "ORDER BY par_date DESC;"

                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.Add("@type", SqlDbType.VarChar, 100).Value = ppeType
                    cmd.Parameters.Add("@par", SqlDbType.VarChar, 100).Value = endUser

                    Using rdr As SqlDataReader = cmd.ExecuteReader()
                        If rdr.Read() Then
                            ' As requested:
                            ' txtfcluster = TBL_PPE.par_date
                            ' txtpardate  = TBL_PPE.fcluster
                            If Not Convert.IsDBNull(rdr("par_date")) Then
                                Dim d As Date = CDate(rdr("par_date"))
                                txtpardate.Text = d.ToString("MMMM dd, yyyy")
                            End If

                            If Not Convert.IsDBNull(rdr("fcluster")) Then
                                txtfcluster.Text = rdr("fcluster").ToString()
                            End If
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error loading header info (PAR Date / Fund Cluster):" &
                            Environment.NewLine & ex.Message,
                            "Header Load Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning)
        End Try
    End Sub

    '────────────────────────────────────────────────────────
    '  LOAD PPE RECORDS INTO GRID
    '────────────────────────────────────────────────────────
    Private Sub LoadPPERecords(ppeType As String, endUser As String)
        DataGridView1.Rows.Clear()

        Using conn As New SqlConnection(frmmain.txtdb.Text)
            conn.Open()

            Const sql As String =
                "SELECT ID, type_of_fixed_asset, description, property_number, unit, unit_cost, " &
                "       qty_property_card, qty_physical_count, shortage_qty, shortage_value, rcppe_remarks " &
                "FROM TBL_PPE WITH (NOLOCK) " &
                "WHERE type_of_ppe = @type AND par_to = @par " &
                "ORDER BY type_of_fixed_asset, property_number;"

            Using cmd As New SqlCommand(sql, conn)
                cmd.Parameters.Add("@type", SqlDbType.VarChar, 100).Value = ppeType
                cmd.Parameters.Add("@par", SqlDbType.VarChar, 100).Value = endUser

                Using rdr As SqlDataReader = cmd.ExecuteReader()
                    Dim totalUnitValue As Decimal = 0D

                    While rdr.Read()
                        Dim idx As Integer = DataGridView1.Rows.Add()
                        Dim r As DataGridViewRow = DataGridView1.Rows(idx)

                        r.Cells("colID").Value = rdr("ID")

                        r.Cells("colArticle").Value = rdr("type_of_fixed_asset").ToString()
                        r.Cells("colDescription").Value = rdr("description").ToString()
                        r.Cells("colPropertyNo").Value = rdr("property_number").ToString()
                        r.Cells("colUnitMeasure").Value = rdr("unit").ToString()

                        Dim unitVal As Decimal = 0D
                        If Not Convert.IsDBNull(rdr("unit_cost")) Then
                            unitVal = Convert.ToDecimal(rdr("unit_cost"))
                        End If
                        r.Cells("colUnitValue").Value = unitVal

                        ' editable columns (load existing values if any)
                        If Not Convert.IsDBNull(rdr("qty_property_card")) Then
                            r.Cells("colQtyCard").Value = rdr("qty_property_card")
                        End If
                        If Not Convert.IsDBNull(rdr("qty_physical_count")) Then
                            r.Cells("colQtyPhysical").Value = rdr("qty_physical_count")
                        End If
                        If Not Convert.IsDBNull(rdr("shortage_qty")) Then
                            r.Cells("colShortQty").Value = rdr("shortage_qty")
                        End If
                        If Not Convert.IsDBNull(rdr("shortage_value")) Then
                            r.Cells("colShortValue").Value = rdr("shortage_value")
                        End If
                        If Not Convert.IsDBNull(rdr("rcppe_remarks")) Then
                            r.Cells("colRemarks").Value = rdr("rcppe_remarks").ToString()
                        End If

                        totalUnitValue += unitVal
                    End While

                    ' Add TOTAL row
                    If DataGridView1.Rows.Count > 0 Then
                        Dim totalIndex As Integer = DataGridView1.Rows.Add()
                        Dim totalRow As DataGridViewRow = DataGridView1.Rows(totalIndex)

                        For c As Integer = 0 To DataGridView1.Columns.Count - 1
                            totalRow.Cells(c).Value = Nothing
                        Next

                        totalRow.Cells("colArticle").Value = "Total"
                        totalRow.Cells("colUnitValue").Value = totalUnitValue

                        totalRow.ReadOnly = True
                        totalRow.DefaultCellStyle.Font =
                            New Font(DataGridView1.Font, FontStyle.Bold)
                        totalRow.DefaultCellStyle.BackColor =
                            Color.FromArgb(240, 240, 240)
                        totalRow.DefaultCellStyle.SelectionBackColor =
                            Color.FromArgb(240, 240, 240)
                        totalRow.DefaultCellStyle.SelectionForeColor =
                            Color.Black
                    End If
                End Using
            End Using
        End Using
    End Sub

    '────────────────────────────────────────────────────────
    '  GRID SETUP / STYLE
    '────────────────────────────────────────────────────────
    Private Sub SetupGrid()
        With DataGridView1
            .DataSource = Nothing
            .Rows.Clear()
            .Columns.Clear()
            .AutoGenerateColumns = False
            .RowHeadersVisible = False
            .AllowUserToAddRows = False
            .AllowUserToResizeRows = False
            .MultiSelect = False

            .ReadOnly = False
            .EditMode = DataGridViewEditMode.EditOnEnter
            .SelectionMode = DataGridViewSelectionMode.CellSelect

            .BorderStyle = BorderStyle.None
            .CellBorderStyle = DataGridViewCellBorderStyle.None
            .ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
            .AdvancedCellBorderStyle.Left = DataGridViewAdvancedCellBorderStyle.None
            .AdvancedCellBorderStyle.Right = DataGridViewAdvancedCellBorderStyle.None
            .AdvancedCellBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.None
            .AdvancedCellBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.None

            Dim hdrColor As Color = Color.FromArgb(73, 94, 252)

            .EnableHeadersVisualStyles = False
            .ColumnHeadersDefaultCellStyle.BackColor = hdrColor
            .ColumnHeadersDefaultCellStyle.ForeColor = Color.White
            .ColumnHeadersDefaultCellStyle.SelectionBackColor = hdrColor
            .ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White
            .ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 10.0!, FontStyle.Bold)
            .ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .ColumnHeadersHeight = 48
            .ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing

            .DefaultCellStyle.Font = New Font("Segoe UI", 10.0!)
            .DefaultCellStyle.ForeColor = Color.Black
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            .RowsDefaultCellStyle.BackColor = Color.White
            .AlternatingRowsDefaultCellStyle.BackColor = Color.White
            .DefaultCellStyle.SelectionBackColor = Color.FromArgb(210, 222, 255)
            .DefaultCellStyle.SelectionForeColor = Color.Black

            .RowTemplate.Height = 30
            .RowTemplate.DefaultCellStyle.Padding = New Padding(2, 4, 2, 4)
        End With

        ' ID (hidden)
        Dim colID As New DataGridViewTextBoxColumn() With {
            .Name = "colID",
            .HeaderText = "ID",
            .Visible = False,
            .ReadOnly = True
        }

        ' Article - left
        Dim colArticle As New DataGridViewTextBoxColumn() With {
            .Name = "colArticle",
            .HeaderText = "Article",
            .ReadOnly = True,
            .Width = 140
        }
        colArticle.DefaultCellStyle = New DataGridViewCellStyle() With {
            .Alignment = DataGridViewContentAlignment.MiddleLeft
        }

        ' Description - left
        Dim colDesc As New DataGridViewTextBoxColumn() With {
            .Name = "colDescription",
            .HeaderText = "Description",
            .ReadOnly = True,
            .Width = 280
        }
        colDesc.DefaultCellStyle = New DataGridViewCellStyle() With {
            .Alignment = DataGridViewContentAlignment.MiddleLeft
        }

        ' Property No. - center
        Dim colPropNo As New DataGridViewTextBoxColumn() With {
            .Name = "colPropertyNo",
            .HeaderText = "Property No.",
            .ReadOnly = True,
            .Width = 150
        }
        colPropNo.DefaultCellStyle = New DataGridViewCellStyle() With {
            .Alignment = DataGridViewContentAlignment.MiddleCenter
        }

        ' Unit of Measure - center
        Dim colUnit As New DataGridViewTextBoxColumn() With {
            .Name = "colUnitMeasure",
            .HeaderText = "Unit of Measure",
            .ReadOnly = True,
            .Width = 100
        }
        colUnit.DefaultCellStyle = New DataGridViewCellStyle() With {
            .Alignment = DataGridViewContentAlignment.MiddleCenter
        }

        ' Unit Value - right
        Dim colUnitVal As New DataGridViewTextBoxColumn() With {
            .Name = "colUnitValue",
            .HeaderText = "Unit Value",
            .ReadOnly = True,
            .Width = 110
        }
        colUnitVal.DefaultCellStyle = New DataGridViewCellStyle() With {
            .Alignment = DataGridViewContentAlignment.MiddleRight,
            .Format = "N2"
        }

        ' Qty per Property Card - center (editable)
        Dim colQtyCard As New DataGridViewTextBoxColumn() With {
            .Name = "colQtyCard",
            .HeaderText = "Qty. per" & Environment.NewLine & "Property Card",
            .ReadOnly = False,
            .Width = 90
        }
        colQtyCard.DefaultCellStyle = New DataGridViewCellStyle() With {
            .Alignment = DataGridViewContentAlignment.MiddleCenter
        }

        ' Qty per Physical Count - center (editable)
        Dim colQtyPhys As New DataGridViewTextBoxColumn() With {
            .Name = "colQtyPhysical",
            .HeaderText = "Qty. per" & Environment.NewLine & "Physical Count",
            .ReadOnly = False,
            .Width = 90
        }
        colQtyPhys.DefaultCellStyle = New DataGridViewCellStyle() With {
            .Alignment = DataGridViewContentAlignment.MiddleCenter
        }

        ' Shortage/Overage Qty - center (editable)
        Dim colShortQty As New DataGridViewTextBoxColumn() With {
            .Name = "colShortQty",
            .HeaderText = "Shortage/Overage" & Environment.NewLine & "Qty.",
            .ReadOnly = False,
            .Width = 90
        }
        colShortQty.DefaultCellStyle = New DataGridViewCellStyle() With {
            .Alignment = DataGridViewContentAlignment.MiddleCenter
        }

        ' Shortage/Overage Value - center (editable)
        Dim colShortVal As New DataGridViewTextBoxColumn() With {
            .Name = "colShortValue",
            .HeaderText = "Shortage/Overage" & Environment.NewLine & "Value",
            .ReadOnly = False,
            .Width = 110
        }
        colShortVal.DefaultCellStyle = New DataGridViewCellStyle() With {
            .Alignment = DataGridViewContentAlignment.MiddleCenter,
            .Format = "N2"
        }

        ' Remarks - left (editable)
        Dim colRemarks As New DataGridViewTextBoxColumn() With {
            .Name = "colRemarks",
            .HeaderText = "Remarks",
            .ReadOnly = False,
            .Width = 200
        }
        colRemarks.DefaultCellStyle = New DataGridViewCellStyle() With {
            .Alignment = DataGridViewContentAlignment.MiddleLeft
        }

        DataGridView1.Columns.AddRange(New DataGridViewColumn() {
            colID,
            colArticle,
            colDesc,
            colPropNo,
            colUnit,
            colUnitVal,
            colQtyCard,
            colQtyPhys,
            colShortQty,
            colShortVal,
            colRemarks
        })
    End Sub

    '────────────────────────────────────────────────────────
    '  SAVE CHANGES PER ROW (LIVE SAVE)
    '────────────────────────────────────────────────────────
    Private Sub DataGridView1_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) _
        Handles DataGridView1.CellEndEdit

        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Return

        Dim row As DataGridViewRow = DataGridView1.Rows(e.RowIndex)

        ' Skip Total row
        Dim art As String = If(row.Cells("colArticle").Value, "").ToString()
        If art = "Total" Then Return

        ' Need valid ID
        If row.Cells("colID").Value Is Nothing OrElse IsDBNull(row.Cells("colID").Value) Then Return

        Dim colName As String = DataGridView1.Columns(e.ColumnIndex).Name

        ' Validate numeric cells
        If colName = "colQtyCard" OrElse
           colName = "colQtyPhysical" OrElse
           colName = "colShortQty" OrElse
           colName = "colShortValue" Then

            Dim txt As String = If(row.Cells(e.ColumnIndex).Value, "").ToString().Trim()

            If txt <> "" Then
                Dim decVal As Decimal
                If Not Decimal.TryParse(txt, decVal) Then
                    MessageBox.Show("Please enter a numeric value.",
                                    "Invalid Input",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning)
                    row.Cells(e.ColumnIndex).Value = Nothing
                    Exit Sub
                Else
                    ' store normalized numeric value
                    row.Cells(e.ColumnIndex).Value = decVal
                End If
            End If
        End If

        ' Save whole row to DB (only PPE fields)
        SaveRowToDatabase(row)
    End Sub

    Private Sub SaveRowToDatabase(row As DataGridViewRow)
        Try
            Dim id As Integer = Convert.ToInt32(row.Cells("colID").Value)

            Dim qtyCard As Decimal? = GetNullableDecimal(row.Cells("colQtyCard").Value)
            Dim qtyPhysical As Decimal? = GetNullableDecimal(row.Cells("colQtyPhysical").Value)
            Dim shortQty As Decimal? = GetNullableDecimal(row.Cells("colShortQty").Value)
            Dim shortVal As Decimal? = GetNullableDecimal(row.Cells("colShortValue").Value)
            Dim remarks As String = If(row.Cells("colRemarks").Value, "").ToString().Trim()

            Using conn As New SqlConnection(frmmain.txtdb.Text)
                conn.Open()

                Const sql As String =
                    "UPDATE TBL_PPE SET " &
                    "qty_property_card = @qtyCard, " &
                    "qty_physical_count = @qtyPhysical, " &
                    "shortage_qty = @shortQty, " &
                    "shortage_value = @shortVal, " &
                    "rcppe_remarks = @remarks " &
                    "WHERE ID = @ID;"

                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.Add("@qtyCard", SqlDbType.Decimal).Value =
                        If(qtyCard.HasValue, CType(qtyCard.Value, Object), DBNull.Value)
                    cmd.Parameters.Add("@qtyPhysical", SqlDbType.Decimal).Value =
                        If(qtyPhysical.HasValue, CType(qtyPhysical.Value, Object), DBNull.Value)
                    cmd.Parameters.Add("@shortQty", SqlDbType.Decimal).Value =
                        If(shortQty.HasValue, CType(shortQty.Value, Object), DBNull.Value)
                    cmd.Parameters.Add("@shortVal", SqlDbType.Decimal).Value =
                        If(shortVal.HasValue, CType(shortVal.Value, Object), DBNull.Value)
                    cmd.Parameters.Add("@remarks", SqlDbType.VarChar, 500).Value =
                        If(remarks = "", CType(DBNull.Value, Object), remarks)
                    cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id

                    cmd.ExecuteNonQuery()
                End Using
            End Using

        Catch ex As Exception
            MessageBox.Show("Error saving changes:" & Environment.NewLine & ex.Message,
                            "Save Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error)
        End Try
    End Sub

    Private Function GetNullableDecimal(value As Object) As Decimal?
        Dim s As String = If(value, "").ToString().Trim()
        If s = "" Then Return Nothing
        Dim d As Decimal
        If Decimal.TryParse(s, d) Then
            Return d
        End If
        Return Nothing
    End Function

    '────────────────────────────────────────────────────────
    '  SIGNATORIES FORM
    '────────────────────────────────────────────────────────
    Private Sub Cmdsignatories_Click(sender As Object, e As EventArgs) Handles cmdsignatories.Click
        Try
            FRMRCPPESignatory.Dispose()
            FRMRCPPESignatory.ShowDialog()
        Catch ex As Exception
            ' ignore / log if needed
        End Try
    End Sub

    '────────────────────────────────────────────────────────
    '  PRINT → RDLC (rptrcppe.rdlc) IN frmpprev
    '────────────────────────────────────────────────────────
    Private Sub cmdprint_Click(sender As Object, e As EventArgs) Handles cmdprint.Click
        Try
            '--------------------------------------------------
            ' 0) Validate selection
            '--------------------------------------------------
            If combotype.SelectedIndex <= 0 Then
                MessageBox.Show("Please select a PPE Type before printing.",
                            "Missing Type",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            If comboenduser.SelectedIndex <= 0 Then
                MessageBox.Show("Please select an End-User before printing.",
                            "Missing End-User",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            DataGridView1.EndEdit()

            '--------------------------------------------------
            ' 1) Build DTRCPPE from grid
            '--------------------------------------------------
            Dim dt As New DataTable("DTRCPPE")
            dt.Columns.Add("Article", GetType(String))
            dt.Columns.Add("Description", GetType(String))
            dt.Columns.Add("PropertyNo", GetType(String))
            dt.Columns.Add("UnitMeasure", GetType(String))
            dt.Columns.Add("UnitValue", GetType(Decimal))
            dt.Columns.Add("QtyCard", GetType(Decimal))
            dt.Columns.Add("QtyPhysical", GetType(Decimal))
            dt.Columns.Add("ShortQty", GetType(Decimal))
            dt.Columns.Add("ShortValue", GetType(Decimal))
            dt.Columns.Add("Remarks", GetType(String))

            Dim hasDetail As Boolean = False

            For Each r As DataGridViewRow In DataGridView1.Rows
                If r.IsNewRow Then Continue For

                Dim art As String = If(r.Cells("colArticle").Value, "").ToString()
                If art = "" OrElse art = "Total" Then Continue For

                hasDetail = True

                Dim dr As DataRow = dt.NewRow()
                dr("Article") = art
                dr("Description") = If(r.Cells("colDescription").Value, "").ToString()
                dr("PropertyNo") = If(r.Cells("colPropertyNo").Value, "").ToString()
                dr("UnitMeasure") = If(r.Cells("colUnitMeasure").Value, "").ToString()
                dr("UnitValue") = SafeToDecimal(r.Cells("colUnitValue").Value)
                dr("QtyCard") = SafeToDecimal(r.Cells("colQtyCard").Value)
                dr("QtyPhysical") = SafeToDecimal(r.Cells("colQtyPhysical").Value)
                dr("ShortQty") = SafeToDecimal(r.Cells("colShortQty").Value)
                dr("ShortValue") = SafeToDecimal(r.Cells("colShortValue").Value)
                dr("Remarks") = If(r.Cells("colRemarks").Value, "").ToString()
                dt.Rows.Add(dr)
            Next

            If Not hasDetail Then
                MessageBox.Show("No PPE records to print for the selected Type and End-User.",
                            "No Data",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)
                Exit Sub
            End If

            '--------------------------------------------------
            ' 2) Build parameters (HTML: Name<br>Position)
            '--------------------------------------------------
            Dim paramList As New List(Of ReportParameter)()

            Using conn As New SqlConnection(frmmain.txtdb.Text)
                conn.Open()

                Const sqlSign As String = "SELECT TOP 1 * FROM TBL_RCPPE_SIGNATORY WITH (NOLOCK);"

                Using cmd As New SqlCommand(sqlSign, conn)
                    Using rdr As SqlDataReader = cmd.ExecuteReader()
                        If rdr.Read() Then

                            Dim GetVal As Func(Of String, String) =
                            Function(col As String) As String
                                If rdr.IsDBNull(rdr.GetOrdinal(col)) Then Return ""
                                Return rdr(col).ToString().Trim()
                            End Function

                            ' sig1..sig10  (each: Name<br>Position)
                            ' sig1..sig10  (each: <u>Name</u><br>Position)
                            For i As Integer = 1 To 10
                                Dim name As String = GetVal("Sig" & i)
                                Dim pos As String = GetVal("Sig" & i & "_pos")

                                Dim htmlVal As String = "<u>" & name & "</u>"
                                If pos <> "" Then
                                    htmlVal &= "<br>" & pos
                                End If

                                paramList.Add(New ReportParameter("sig" & i, htmlVal))
                            Next

                            ' appby (Approved_by + app_pos)
                            Dim appName As String = GetVal("Approved_by")
                            Dim appPos As String = GetVal("app_pos")

                            Dim appHtml As String = "<u>" & appName & "</u>"
                            If appPos <> "" Then
                                appHtml &= "<br>" & appPos
                            End If
                            paramList.Add(New ReportParameter("appby", appHtml))

                            ' veriby (Verified_by + veri_pos)
                            Dim veriName As String = GetVal("Verified_by")
                            Dim veriPos As String = GetVal("veri_pos")

                            Dim veriHtml As String = "<u>" & veriName & "</u>"
                            If veriPos <> "" Then
                                veriHtml &= "<br>" & veriPos
                            End If
                            paramList.Add(New ReportParameter("veriby", veriHtml))

                        End If
                    End Using
                End Using
            End Using

            ' Other text parameters
            Dim fclusterText As String = txtfcluster.Text.Trim()
            Dim parDateText As String = txtpardate.Text.Trim()
            Dim typeText As String = combotype.Text.Trim()
            Dim endUserText As String = comboenduser.Text.Trim()

            Dim forWhichText As String =
            endUserText & " MPOS-BARMM is accountable, having assumed such accountability on " & parDateText

            Dim asOfText As String = Date.Now.ToString("MMMM dd, yyyy")

            paramList.Add(New ReportParameter("fcluster", fclusterText))
            paramList.Add(New ReportParameter("forwhich", forWhichText))
            paramList.Add(New ReportParameter("type", typeText))
            paramList.Add(New ReportParameter("asof", asOfText))

            '--------------------------------------------------
            ' 3) Show RDLC in frmpprev
            '--------------------------------------------------
            frmpprev.Dispose()

            Dim rptPath As String =
            System.IO.Path.Combine(Application.StartupPath, "Report\rptrcppe.rdlc")

            With frmpprev.ReportViewer1
                .Reset()
                .LocalReport.ReportPath = rptPath
                .LocalReport.DataSources.Clear()
                .LocalReport.DataSources.Add(New ReportDataSource("DSRCPPE", dt))
                .LocalReport.SetParameters(paramList)
                .SetDisplayMode(DisplayMode.PrintLayout)
                .ZoomMode = ZoomMode.Percent
                .ZoomPercent = 100
                .RefreshReport()
            End With

            frmpprev.panelsubmit.Visible = False
            frmpprev.ShowDialog()

        Catch ex As Exception
            MessageBox.Show("Error generating PPE Report:" & Environment.NewLine & ex.Message,
                        "Print Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub



    Private Function SafeToDecimal(value As Object) As Decimal
        If value Is Nothing OrElse IsDBNull(value) Then Return 0D
        Dim s As String = value.ToString().Trim()
        If s = "" Then Return 0D
        Dim d As Decimal
        If Decimal.TryParse(s, d) Then
            Return d
        End If
        Return 0D
    End Function

End Class
