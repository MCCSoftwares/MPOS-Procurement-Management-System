Public Class frmppmp
    Private Sub Frmppmp_Load(sender As Object, e As EventArgs) Handles MyBase.Load
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

            ' Add the columns
            DataGridView1.Columns.Add("Date", "DATE")
            DataGridView1.Columns.Add("PPMP_ID", "PPMP ID")
            DataGridView1.Columns.Add("ContractPackageNumber", "CONTRACT PACKAGE NUMBER")
            DataGridView1.Columns.Add("Agency", "AGENCY")
            DataGridView1.Columns.Add("Section", "SECTION")
            DataGridView1.Columns.Add("Category", "CATEGORY")
            DataGridView1.Columns.Add("Items", "ITEMS")
            DataGridView1.Columns.Add("Quantity", "QUANTITY")
            DataGridView1.Columns.Add("Amount", "AMOUNT")
            DataGridView1.Columns.Add("RequestedBy", "REQUESTED BY")
            DataGridView1.Columns.Add("Status", "STATUS")

            ' (Optional) Adjust FillWeight if you want some columns wider than others
            DataGridView1.Columns("Items").FillWeight = 200
            DataGridView1.Columns("Date").FillWeight = 70
            DataGridView1.Columns("PPMP_ID").FillWeight = 80
            DataGridView1.Columns("Amount").FillWeight = 90

        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub

    Private Sub Label3_Click(sender As Object, e As EventArgs) Handles Label3.Click

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        frmppmpfile.ShowDialog()

    End Sub
End Class