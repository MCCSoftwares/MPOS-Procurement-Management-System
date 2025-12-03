Public Class frmppmpfile
    Private Sub Frmppmpfile_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            ' Clear existing columns
            DataGridView1.Columns.Clear()
            DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray
            DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black

            DataGridView1.BackgroundColor = Color.Gainsboro
            DataGridView1.DefaultCellStyle.SelectionBackColor = Color.SteelBlue
            DataGridView1.RowHeadersDefaultCellStyle.SelectionForeColor = Color.White
            DataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True
            DataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect
            DataGridView1.AllowUserToResizeColumns = False

            DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 120, 215)
            DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
            DataGridView1.ColumnHeadersDefaultCellStyle.Font = New Font(DataGridView1.Font, FontStyle.Bold)
            DataGridView1.RowsDefaultCellStyle.BackColor = Color.FromArgb(230, 230, 230)
            DataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(244, 244, 244)

            DataGridView1.DefaultCellStyle.Font = New Font("Calibri", 11)
            DataGridView1.ColumnHeadersDefaultCellStyle.Font = New Font("Calibri", 11)
            DataGridView1.AutoGenerateColumns = False
            DataGridView1.RowHeadersVisible = False
            DataGridView1.EnableHeadersVisualStyles = False


            ' Add basic columns
            DataGridView1.Columns.Add("Commodity", "COMMODITY (NATURE AND DESCRIPTION)")
            DataGridView1.Columns.Add("Unit", "UNIT")
            DataGridView1.Columns.Add("UnitPrice", "UNIT PRICE")
            DataGridView1.Columns(0).Width = "500"

            ' Total Calendar
            DataGridView1.Columns.Add("TotalQty", "TOTAL QTY.")
            DataGridView1.Columns.Add("TotalAmount", "TOTAL AMOUNT")
            DataGridView1.Columns(1).Width = "100"

            ' 1st Quarter
            DataGridView1.Columns.Add("Q1Qty", "1ST QTR QTY.")
            DataGridView1.Columns.Add("Q1Amount", "1ST QTR AMOUNT")
            DataGridView1.Columns(2).Width = "100"

            ' 2nd Quarter
            DataGridView1.Columns.Add("Q2Qty", "2ND QTR QTY.")
            DataGridView1.Columns.Add("Q2Amount", "2ND QTR AMOUNT")
            DataGridView1.Columns(3).Width = "100"

            ' 3rd Quarter
            DataGridView1.Columns.Add("Q3Qty", "3RD QTR QTY.")
            DataGridView1.Columns.Add("Q3Amount", "3RD QTR AMOUNT")
            DataGridView1.Columns(4).Width = "100"

            ' 4th Quarter
            DataGridView1.Columns.Add("Q4Qty", "4TH QTR QTY.")
            DataGridView1.Columns.Add("Q4Amount", "4TH QTR AMOUNT")
            DataGridView1.Columns(5).Width = "100"


        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub
End Class