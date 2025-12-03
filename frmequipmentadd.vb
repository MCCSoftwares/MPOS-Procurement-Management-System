Imports System.Data.SqlClient
Imports System.IO
Imports System.Drawing.Imaging
Imports System.Linq
Imports System.Threading.Tasks

Public Class FrmEquipmentAdd
    Private isUpdate As Boolean = False
    Private isImageDeleted As Boolean = False
    Private imageData As Byte() = Nothing
    Private Const MaxChars As Integer = 3500

    ' Values returned to caller (not used now, but harmless to keep)
    Public Property SavedAction As String = Nothing  ' "insert" or "update"
    Public Property SavedID As Integer = 0
    Public Property SavedEDESC As String = Nothing
    Public Property SavedATITLE As String = Nothing
    Public Property SavedACODE As String = Nothing
    Public Property SavedIPNO As String = Nothing
    Public Property SavedUNIT As String = Nothing

    Private Sub UpdateCharacterCount()
        Dim charsLeft As Integer = MaxChars - txtdesc.TextLength
        lblCharCount.Text = charsLeft.ToString() & "/" & MaxChars.ToString() & " Characters left"
    End Sub

    Private Async Sub FrmEquipmentAdd_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            isUpdate = Not String.IsNullOrWhiteSpace(lblid.Text) AndAlso lblid.Text <> "0"
            cmdsave.Text = If(isUpdate, "Update", "Save")
            UpdateCharacterCount()

            If isUpdate Then
                Await LoadItemAsync()
            End If
        Catch ex As Exception
            MessageBox.Show("Error initializing form: " & ex.Message,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Public Async Function LoadItemAsync() As Task
        Using conn As SqlConnection = GetTunedConnection(),
              cmd As New SqlCommand("
                SELECT EDESC, ATITLE, ACODE, IPNO, UNIT, IMAGES
                FROM TBL_EDESC WHERE ID = @ID;", conn)
            cmd.Parameters.Add("@ID", SqlDbType.Int).Value = CInt(lblid.Text)
            cmd.CommandTimeout = 30

            Await conn.OpenAsync()
            Using rdr = Await cmd.ExecuteReaderAsync()
                If Await rdr.ReadAsync() Then
                    txtdesc.Text = rdr("EDESC").ToString()
                    txtatitle.Text = rdr("ATITLE").ToString()
                    txtacode.Text = rdr("ACODE").ToString()
                    txtipno.Text = rdr("IPNO").ToString()
                    txtunit.Text = rdr("UNIT").ToString()

                    If Not IsDBNull(rdr("IMAGES")) Then
                        imageData = DirectCast(rdr("IMAGES"), Byte())
                        Using ms As New MemoryStream(imageData)
                            PictureBox1.Image = Image.FromStream(ms)
                        End Using
                    Else
                        PictureBox1.Image = Nothing
                        imageData = Nothing
                    End If
                End If
            End Using
        End Using
    End Function

    Private Async Sub LinkLblselect_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLblselect.LinkClicked
        Using ofd As New OpenFileDialog()
            ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif"
            If ofd.ShowDialog() = DialogResult.OK Then
                Try
                    UseWait(True)
                    Dim path = ofd.FileName
                    Dim bytes As Byte() = Await Task.Run(Function()
                                                             Using orig As Image = Image.FromFile(path)
                                                                 Return GetCompressedImageBytes(orig)
                                                             End Using
                                                         End Function)
                    imageData = bytes
                    Using msPreview As New MemoryStream(imageData)
                        PictureBox1.Image = Image.FromStream(msPreview)
                    End Using
                    isImageDeleted = False
                Catch ex As Exception
                    MessageBox.Show("Image load/compress error: " & ex.Message, "Image", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Finally
                    UseWait(False)
                End Try
            End If
        End Using
    End Sub

    Private Sub LinkLbldelete_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLbldelete.LinkClicked
        If MessageBox.Show("Remove image?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            If PictureBox1.Image IsNot Nothing Then
                PictureBox1.Image.Dispose()
                PictureBox1.Image = Nothing
            End If
            imageData = Nothing
            isImageDeleted = True
        End If
    End Sub

    Private Async Sub cmdSave_Click(sender As Object, e As EventArgs) Handles cmdsave.Click
        Try
            Dim descText As String = txtdesc.Text.Trim()
            Dim acodeText As String = txtacode.Text.Trim()
            Dim ipnoText As String = txtipno.Text.Trim()
            Dim atitleText As String = txtatitle.Text.Trim()
            Dim unitText As String = txtunit.Text.Trim()

            If descText = "" OrElse acodeText = "" Then
                MessageBox.Show("Description and Account Code are required.",
                                "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            UseWait(True)

            Using conn As SqlConnection = GetTunedConnection()
                Await conn.OpenAsync()

                Using tran = conn.BeginTransaction(IsolationLevel.ReadCommitted)

                    Dim dupSql As String =
                        "SELECT " &
                        " DupDesc = CASE WHEN EXISTS(SELECT 1 FROM TBL_EDESC WHERE EDESC = @EDESC AND (@ID IS NULL OR ID <> @ID)) THEN 1 ELSE 0 END," &
                        " DupIP   = CASE WHEN (@IPNO <> N'') AND EXISTS(SELECT 1 FROM TBL_EDESC WHERE IPNO = @IPNO AND (@ID IS NULL OR ID <> @ID)) THEN 1 ELSE 0 END;"

                    Using chk As New SqlCommand(dupSql, conn, tran)
                        chk.CommandTimeout = 30
                        chk.Parameters.Add("@EDESC", SqlDbType.NVarChar).Value = descText
                        chk.Parameters.Add("@IPNO", SqlDbType.NVarChar).Value = ipnoText
                        If isUpdate Then
                            chk.Parameters.Add("@ID", SqlDbType.Int).Value = CInt(lblid.Text)
                        Else
                            chk.Parameters.Add("@ID", SqlDbType.Int).Value = DBNull.Value
                        End If

                        Dim dupDesc As Boolean = False
                        Dim dupIP As Boolean = False
                        Using rdr = Await chk.ExecuteReaderAsync()
                            If Await rdr.ReadAsync() Then
                                dupDesc = Convert.ToInt32(rdr("DupDesc")) = 1
                                dupIP = Convert.ToInt32(rdr("DupIP")) = 1
                            End If
                        End Using
                        If dupDesc Then
                            tran.Rollback()
                            MessageBox.Show("That Description already exists.", "Duplicate", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            txtdesc.Focus() : Return
                        End If
                        If dupIP Then
                            tran.Rollback()
                            MessageBox.Show("That Item/Property Number already exists.", "Duplicate", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            txtipno.Focus() : Return
                        End If
                    End Using

                    Dim sql As String
                    Dim newId As Integer = 0

                    If isUpdate Then
                        sql = "UPDATE TBL_EDESC SET " &
                              " EDESC=@EDESC, ATITLE=@ATITLE, ACODE=@ACODE, IPNO=@IPNO, UNIT=@UNIT" &
                              If(isImageDeleted, ", IMAGES = NULL", If(imageData IsNot Nothing, ", IMAGES=@IMAGES", "")) &
                              " WHERE ID=@ID;"
                        Using cmd As New SqlCommand(sql, conn, tran)
                            cmd.CommandTimeout = 30
                            cmd.Parameters.Add("@EDESC", SqlDbType.NVarChar).Value = descText
                            cmd.Parameters.Add("@ATITLE", SqlDbType.NVarChar).Value = atitleText
                            cmd.Parameters.Add("@ACODE", SqlDbType.NVarChar).Value = acodeText
                            cmd.Parameters.Add("@IPNO", SqlDbType.NVarChar).Value = ipnoText
                            cmd.Parameters.Add("@UNIT", SqlDbType.NVarChar).Value = unitText
                            If Not isImageDeleted AndAlso imageData IsNot Nothing Then
                                cmd.Parameters.Add("@IMAGES", SqlDbType.VarBinary, -1).Value = imageData
                            End If
                            cmd.Parameters.Add("@ID", SqlDbType.Int).Value = CInt(lblid.Text)
                            Await cmd.ExecuteNonQueryAsync()
                            newId = CInt(lblid.Text)
                            SavedAction = "update"
                        End Using
                    Else
                        sql = "INSERT INTO TBL_EDESC (EDESC, ATITLE, ACODE, IPNO, UNIT" &
                              If(imageData IsNot Nothing, ", IMAGES", "") &
                              ") OUTPUT INSERTED.ID VALUES (@EDESC, @ATITLE, @ACODE, @IPNO, @UNIT" &
                              If(imageData IsNot Nothing, ", @IMAGES", "") & ");"
                        Using cmd As New SqlCommand(sql, conn, tran)
                            cmd.CommandTimeout = 30
                            cmd.Parameters.Add("@EDESC", SqlDbType.NVarChar).Value = descText
                            cmd.Parameters.Add("@ATITLE", SqlDbType.NVarChar).Value = atitleText
                            cmd.Parameters.Add("@ACODE", SqlDbType.NVarChar).Value = acodeText
                            cmd.Parameters.Add("@IPNO", SqlDbType.NVarChar).Value = ipnoText
                            cmd.Parameters.Add("@UNIT", SqlDbType.NVarChar).Value = unitText
                            If imageData IsNot Nothing Then
                                cmd.Parameters.Add("@IMAGES", SqlDbType.VarBinary, -1).Value = imageData
                            End If
                            Dim obj = Await cmd.ExecuteScalarAsync()
                            newId = Convert.ToInt32(obj)
                            SavedAction = "insert"
                        End Using
                    End If

                    tran.Commit()

                    SavedID = newId
                    SavedEDESC = descText
                    SavedATITLE = atitleText
                    SavedACODE = acodeText
                    SavedIPNO = ipnoText
                    SavedUNIT = unitText
                End Using
            End Using

            UseWait(False)
            MessageBox.Show(If(isUpdate, "Record updated successfully.", "Record saved successfully."),
                            "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

            Me.DialogResult = DialogResult.OK
            Me.Close()

        Catch ex As Exception
            MessageBox.Show("Error saving record: " & ex.Message,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If Not Me.IsDisposed AndAlso Not Me.Disposing Then
                UseWait(False)
            End If
        End Try
    End Sub

    Private Sub cmdClose_Click(sender As Object, e As EventArgs) Handles cmdclose.Click
        Me.Close()
    End Sub

    Private Sub UseWait(busy As Boolean)
        Me.UseWaitCursor = busy
        Me.Cursor = If(busy, Cursors.WaitCursor, Cursors.Default)
        If Not Me.IsDisposed AndAlso Not Me.Disposing Then
            cmdsave.Enabled = Not busy
            cmdclose.Enabled = Not busy
            LinkLblselect.Enabled = Not busy
            LinkLbldelete.Enabled = Not busy
            txtdesc.ReadOnly = busy
            txtatitle.ReadOnly = busy
            txtacode.ReadOnly = busy
            txtipno.ReadOnly = busy
            txtunit.ReadOnly = busy
        End If
    End Sub

    Private Function GetCompressedImageBytes(orig As Image) As Byte()
        Dim maxW As Integer = 1024, maxH As Integer = 768
        Dim ratioW As Double = orig.Width / CDbl(maxW)
        Dim ratioH As Double = orig.Height / CDbl(maxH)
        Dim ratio As Double = Math.Max(ratioW, ratioH)
        If ratio < 1.0 Then ratio = 1.0
        Dim newW As Integer = CInt(Math.Round(orig.Width / ratio))
        Dim newH As Integer = CInt(Math.Round(orig.Height / ratio))

        Using thumb As New Bitmap(newW, newH)
            Using g As Graphics = Graphics.FromImage(thumb)
                g.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
                g.SmoothingMode = Drawing2D.SmoothingMode.HighQuality
                g.PixelOffsetMode = Drawing2D.PixelOffsetMode.HighQuality
                g.CompositingQuality = Drawing2D.CompositingQuality.HighQuality
                g.DrawImage(orig, 0, 0, newW, newH)
            End Using

            Dim jpegEnc = ImageCodecInfo.GetImageEncoders().First(Function(ic) ic.MimeType = "image/jpeg")
            Using eps As New EncoderParameters(1)
                eps.Param(0) = New EncoderParameter(Encoder.Quality, 45L)
                Using ms As New MemoryStream()
                    thumb.Save(ms, jpegEnc, eps)
                    Return ms.ToArray()
                End Using
            End Using
        End Using
    End Function

    Private Sub Txtdesc_TextChanged(sender As Object, e As EventArgs) Handles txtdesc.TextChanged
        UpdateCharacterCount()
    End Sub

    Private Function GetTunedConnection() As SqlConnection
        Dim cs As String = frmmain.txtdb.Text
        Dim b As New SqlConnectionStringBuilder(cs)
        b.Pooling = True
        If b.MinPoolSize < 5 Then b.MinPoolSize = 5
        If b.MaxPoolSize < 100 Then b.MaxPoolSize = 100
        If b.ConnectTimeout > 5 Then b.ConnectTimeout = 5
        If b.PacketSize < 32767 Then b.PacketSize = 32767
        Return New SqlConnection(b.ConnectionString)
    End Function
End Class
