Imports System.Data
Imports System.Data.SqlClient
Public Class frmdeliveryadd

    Public Property InPOID As Integer
    Public Property InPRID As Integer
    Public Property InPONo As String
    Public Property InPRNo As String
    Public Property InPDelivery As String
    Public Property InDDelivery As Date


    Private Sub cmdconfirm_Click(sender As Object, e As EventArgs) Handles cmdconfirm.Click
        If Not cmdconfirm.Enabled Then Return
        cmdconfirm.Enabled = False

        Try

            If Me.txtttype.Text = "Direct Payment" Then


                ' ... your existing validation (PRID/POID/date) ...

                Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
                    conn.Open()

                    Using tx As SqlTransaction = conn.BeginTransaction(IsolationLevel.ReadCommitted)
                        Try
                            ' Ensure immediate rollback on connection errors
                            Using cmdOn As New SqlCommand("SET XACT_ABORT ON; SET NOCOUNT ON;", conn, tx)
                                cmdOn.ExecuteNonQuery()
                            End Using

                            ' 1) Insert into TBL_IAR (same as yours) -> get newIarId
                            Dim newIarId As Integer
                            Using cmdIar As New SqlCommand("
                        INSERT INTO TBL_IAR
                            (PRNO, PRID, POID, PONO, PODATE,
                             IARNO, IARDATE, CNAME, FCLUSTER,
                             REQOFFICE, RESCODE, INVOICENO, INVOICEDATE,
                             DINSPECTED, CINSPECT, DACCEPT, ASTATUS, AOFFICER, STATUS, DDATE, PDELIVERY, TTYPE)
                        VALUES
                            (@PRNO, @PRID, @POID, @PONO, @PODATE,
                             NULL, NULL, @CNAME, @FCLUSTER,
                             @REQOFFICE, @RESCODE, NULL, NULL,
                             NULL, NULL, NULL, NULL, NULL, @STATUS, @DDATE, @PDELIVERY, @TTYPE);
                        SELECT CAST(SCOPE_IDENTITY() AS INT);", conn, tx)

                                cmdIar.CommandTimeout = 30
                                cmdIar.Parameters.Add("@PRNO", SqlDbType.NVarChar, 100).Value = txtprno.Text.Trim()
                                cmdIar.Parameters.Add("@PRID", SqlDbType.Int).Value = CInt(lblprid.Text)
                                cmdIar.Parameters.Add("@POID", SqlDbType.Int).Value = CInt(lblpoid.Text)
                                cmdIar.Parameters.Add("@PONO", SqlDbType.NVarChar, 100).Value = txtpono.Text.Trim()
                                cmdIar.Parameters.Add("@PODATE", SqlDbType.DateTime).Value = DateTime.Parse(lblpodate.Text)
                                cmdIar.Parameters.Add("@CNAME", SqlDbType.NVarChar, 200).Value = txtcname.Text.Trim()
                                cmdIar.Parameters.Add("@FCLUSTER", SqlDbType.NVarChar, 200).Value = txtfcluster.Text.Trim()
                                cmdIar.Parameters.Add("@REQOFFICE", SqlDbType.NVarChar, 200).Value = txtreqoff.Text.Trim()
                                cmdIar.Parameters.Add("@RESCODE", SqlDbType.NVarChar, 100).Value = txtrcenter.Text.Trim()
                                cmdIar.Parameters.Add("@STATUS", SqlDbType.NVarChar, 50).Value = "Pending"
                                cmdIar.Parameters.Add("@PDELIVERY", SqlDbType.NVarChar, 50).Value = txtpdelivery.Text.Trim()
                                cmdIar.Parameters.Add("@DDATE", SqlDbType.DateTime).Value = DateTime.Parse(txtddelivery.Text)
                                cmdIar.Parameters.Add("@TTYPE", SqlDbType.NVarChar, 200).Value = txtttype.Text.Trim()

                                newIarId = CInt(cmdIar.ExecuteScalar())
                            End Using

                            ' 2) De-duplicated insert into TBL_IARITEMS
                            Using cmdItems As New SqlCommand("
                                SET NOCOUNT ON;
                                INSERT INTO TBL_IARITEMS
                                    (IARID, POID, PRID, ITEMNO, ACODE, ATITLE, DESCRIPTIONS, UNITS, QUANTITY)
                                SELECT
                                    @IARID,
                                    p.POID,
                                    p.PRID,
                                    COALESCE(e0.IPNO,  N'') AS ITEMNO,
                                    COALESCE(e0.ACODE, N'') AS ACODE,
                                    COALESCE(e0.ATITLE,N'') AS ATITLE,
                                    p.PO_DESC                AS DESCRIPTIONS,
                                    p.PO_UNIT                AS UNITS,
                                    p.PO_QTY                 AS QUANTITY
                                FROM (
                                    SELECT DISTINCT POID, PRID, PO_DESC, PO_UNIT, PO_QTY
                                    FROM TBL_POITEMS
                                    WHERE POID = @POID
                                ) AS p
                                OUTER APPLY (
                                    SELECT TOP (1) IPNO, ACODE, ATITLE
                                    FROM TBL_EDESC
                                    WHERE EDESC = p.PO_DESC
                                    ORDER BY ID
                                ) AS e0
                                WHERE NOT EXISTS (
                                    SELECT 1
                                    FROM TBL_IARITEMS x
                                    WHERE x.IARID = @IARID
                                      AND x.POID  = p.POID
                                      AND x.PRID  = p.PRID
                                      AND x.DESCRIPTIONS = p.PO_DESC
                                      AND x.UNITS = p.PO_UNIT
                                      AND x.QUANTITY = p.PO_QTY
                                );", conn, tx)

                                cmdItems.Parameters.Add("@IARID", SqlDbType.Int).Value = newIarId
                                cmdItems.Parameters.Add("@POID", SqlDbType.Int).Value = CInt(lblpoid.Text)
                                cmdItems.ExecuteNonQuery()
                                ' Optional: check inserted count if you want to warn when zero.
                            End Using

                            tx.Commit()

                            Dim parent = TryCast(Me.Owner, frmPOFile)
                            If parent IsNot Nothing Then
                                parent.submitpo() ' must be Public
                            End If


                            '/////////////////////////////////// NOTIFICATION
                            NotificationHelper.AddNotification($"Delivery information for Purchase Order {txtpono.Text} has been initialized by {frmmain.lblaname.Text.Trim()}.")
                            '/////////////////////////////////// NOTIFICATION

                            MessageBox.Show("Delivery confirmation saved. IAR and items added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                            Me.Dispose()

                        Catch exTx As Exception
                            Try : tx.Rollback() : Catch : End Try
                            MessageBox.Show("Save failed; all changes reverted." & Environment.NewLine & exTx.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        End Try
                    End Using
                End Using

            Else
                ' ─────────────────────────────────────────────────────────────
                ' Cash Advance / Reimbursement (NO PO; IAR items from RFQ)
                ' Uses TBL_RFQ; Delivery date is NOT required (set NULL)
                ' ─────────────────────────────────────────────────────────────
                Dim prId As Integer
                If Not Integer.TryParse(lblprid.Text, prId) Then
                    MessageBox.Show("Invalid PRID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Exit Sub
                End If

                Dim rfqId As Integer = 0
                Dim haveLblRfq As Boolean = False
                Try
                    ' Optional: haveLblRfq = Integer.TryParse(lblrfqid.Text, rfqId)
                Catch
                End Try

                Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
                    conn.Open()

                    If Not haveLblRfq Then
                        Using cmdFindRfq As New SqlCommand("
                SELECT TOP (1) ID
                FROM TBL_RFQ
                WHERE RFQ_PRID = @PRID
                ORDER BY ID DESC;", conn)
                            cmdFindRfq.Parameters.Add("@PRID", SqlDbType.Int).Value = prId
                            Dim o = cmdFindRfq.ExecuteScalar()
                            If o IsNot Nothing Then Integer.TryParse(o.ToString(), rfqId)
                        End Using
                    End If

                    If rfqId <= 0 Then
                        MessageBox.Show("RFQ not found for this PR. Please complete RFQ first.", "Missing RFQ",
                                        MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Exit Sub
                    End If

                    Using tx As SqlTransaction = conn.BeginTransaction(IsolationLevel.ReadCommitted)
                        Try
                            Using cmdOn As New SqlCommand("SET XACT_ABORT ON; SET NOCOUNT ON;", conn, tx)
                                cmdOn.ExecuteNonQuery()
                            End Using

                            ' 1) Insert IAR header — NO PO fields for CA/Reimb; DDATE = NULL
                            Dim newIarId As Integer
                            Using cmdIar As New SqlCommand("
                    INSERT INTO TBL_IAR
                        (PRNO, PRID, POID, PONO, PODATE,
                         IARNO, IARDATE, CNAME, FCLUSTER,
                         REQOFFICE, RESCODE, INVOICENO, INVOICEDATE,
                         DINSPECTED, CINSPECT, DACCEPT, ASTATUS, AOFFICER, STATUS, DDATE, PDELIVERY, TTYPE)
                    VALUES
                        (@PRNO, @PRID, NULL, NULL, NULL,
                         NULL, NULL, @CNAME, @FCLUSTER,
                         @REQOFFICE, @RESCODE, NULL, NULL,
                         NULL, NULL, NULL, NULL, NULL, @STATUS, @DDATE, @PDELIVERY, @TTYPE);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);", conn, tx)

                                cmdIar.Parameters.Add("@PRNO", SqlDbType.NVarChar, 100).Value = txtprno.Text.Trim()
                                cmdIar.Parameters.Add("@PRID", SqlDbType.Int).Value = prId
                                cmdIar.Parameters.Add("@CNAME", SqlDbType.NVarChar, 200).Value = txtcname.Text.Trim()
                                cmdIar.Parameters.Add("@FCLUSTER", SqlDbType.NVarChar, 200).Value = txtfcluster.Text.Trim()
                                cmdIar.Parameters.Add("@REQOFFICE", SqlDbType.NVarChar, 200).Value = txtreqoff.Text.Trim()
                                cmdIar.Parameters.Add("@RESCODE", SqlDbType.NVarChar, 100).Value = txtrcenter.Text.Trim()
                                cmdIar.Parameters.Add("@STATUS", SqlDbType.NVarChar, 50).Value = "Pending"
                                cmdIar.Parameters.Add("@PDELIVERY", SqlDbType.NVarChar, 50).Value = If(String.IsNullOrWhiteSpace(txtpdelivery.Text), "N/A", txtpdelivery.Text.Trim())
                                ' ⬇️ IMPORTANT: Delivery date not required -> set NULL
                                cmdIar.Parameters.Add("@DDATE", SqlDbType.[Date]).Value = DBNull.Value
                                cmdIar.Parameters.Add("@TTYPE", SqlDbType.NVarChar, 200).Value = txtttype.Text.Trim()

                                newIarId = CInt(cmdIar.ExecuteScalar())
                            End Using

                            ' 2) Insert IAR ITEMS from RFQ ITEMS (ITEMNO = EDESC.IPNO)
                            Using cmdItems As New SqlCommand("
                    SET NOCOUNT ON;
                    INSERT INTO TBL_IARITEMS
                        (IARID, POID, PRID, ITEMNO, ACODE, ATITLE, DESCRIPTIONS, UNITS, QUANTITY)
                    SELECT
                        @IARID,
                        NULL,
                        R.PRID,
                        COALESCE(E.IPNO,  N'') AS ITEMNO,
                        COALESCE(E.ACODE, N'') AS ACODE,
                        COALESCE(E.ATITLE,N'') AS ATITLE,
                        R.RFQ_DESC             AS DESCRIPTIONS,
                        R.RFQ_UNIT             AS UNITS,
                        R.RFQ_QTY              AS QUANTITY
                    FROM (
                        SELECT DISTINCT RFQID, PRID, RFQ_ITEMNO, RFQ_DESC, RFQ_UNIT, RFQ_QTY
                        FROM TBL_RFQITEMS
                        WHERE RFQID = @RFQID AND PRID = @PRID
                    ) AS R
                    OUTER APPLY (
                        SELECT TOP (1) IPNO, ACODE, ATITLE
                        FROM TBL_EDESC
                        WHERE EDESC = R.RFQ_DESC
                        ORDER BY ID
                    ) AS E
                    WHERE NOT EXISTS (
                        SELECT 1
                        FROM TBL_IARITEMS X
                        WHERE X.IARID = @IARID
                          AND X.PRID  = R.PRID
                          AND X.DESCRIPTIONS = R.RFQ_DESC
                          AND X.UNITS = R.RFQ_UNIT
                          AND X.QUANTITY = R.RFQ_QTY
                    );", conn, tx)

                                cmdItems.Parameters.Add("@IARID", SqlDbType.Int).Value = newIarId
                                cmdItems.Parameters.Add("@RFQID", SqlDbType.Int).Value = rfqId
                                cmdItems.Parameters.Add("@PRID", SqlDbType.Int).Value = prId
                                cmdItems.ExecuteNonQuery()
                            End Using

                            ' 3) Update PR and RFQ status (no ABSTRACT table)
                            Using cmdUpdPr As New SqlCommand("
                    UPDATE TBL_PR
                    SET PRSTATUS = '[IAR] Pending'
                    WHERE ID = @PRID;", conn, tx)
                                cmdUpdPr.Parameters.Add("@PRID", SqlDbType.Int).Value = prId
                                cmdUpdPr.ExecuteNonQuery()
                            End Using

                            Using cmdUpdRfq As New SqlCommand("
                    UPDATE TBL_RFQ
                    SET RFQ_STATUS = 'Completed'
                    WHERE ID = @RFQID;", conn, tx)
                                cmdUpdRfq.Parameters.Add("@RFQID", SqlDbType.Int).Value = rfqId
                                cmdUpdRfq.ExecuteNonQuery()
                            End Using

                            tx.Commit()

                            NotificationHelper.AddNotification(
                                $"Delivery (CA/Reimb) for PR {txtprno.Text} initialized from RFQ by {frmmain.lblaname.Text.Trim()}.")

                            MessageBox.Show("Delivery confirmation saved (CA/Reimb). IAR and items added from RFQ successfully. PR/RFQ status updated.",
                                            "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                            Me.Dispose()

                        Catch exTx As Exception
                            Try : tx.Rollback() : Catch : End Try
                            MessageBox.Show("Save failed; all changes reverted." & Environment.NewLine & exTx.Message,
                                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        End Try
                    End Using
                End Using

                ' Update frmrfqfile UI (replaced frmabstractfile)
                frmrfqfile.cmdsubmit.Enabled = False
                frmrfqfile.cmdscancel.Enabled = True
                frmrfqfile.cmdsave.Enabled = False
                'frmrfqfile.cmdrefresh.Enabled = False
            End If



        Catch ex As Exception
            MessageBox.Show("Unexpected error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            cmdconfirm.Enabled = True
        End Try
    End Sub



    Private Sub TextBox2_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtpono.KeyPress
        e.Handled = True
    End Sub

    Private Sub TextBox3_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtreqoff.KeyPress
        e.Handled = True
    End Sub

    Private Sub TextBox4_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtrcenter.KeyPress
        e.Handled = True
    End Sub

    Private Sub TextBox7_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtcname.KeyPress
        e.Handled = True
    End Sub

    Private Sub TextBox5_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtpdelivery.KeyPress
        e.Handled = True
    End Sub

    Private Sub TextBox6_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtddelivery.KeyPress
        e.Handled = True
    End Sub

    Private Sub Frmdeliveryadd_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            ' ───────────────────────────────────────────────
            ' Copy values from frmpofile controls
            ' ───────────────────────────────────────────────

            ' Date (delivery) from DTP
            '  If frmPOFile.dtddelivery IsNot Nothing Then
            ' txtddelivery.Text = frmPOFile.dtddelivery.Value.ToString("MM/dd/yyyy")
            '  End If

            ' ───────────────────────────────────────────────
            ' Pull OFFSEC, RCCODE, FCLUSTER from TBL_PR via PRID
            ' ───────────────────────────────────────────────
            Dim prId As Integer
            If Not Integer.TryParse(lblprid.Text, prId) Then
                MessageBox.Show("Invalid PRID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            Using conn As New SqlConnection(My.Forms.frmmain.txtdb.Text)
                conn.Open()
                Using cmd As New SqlCommand("
                    SELECT OFFSEC, RCCODE, FCLUSTER
                    FROM TBL_PR
                    WHERE ID = @PRID;", conn)

                    cmd.Parameters.Add("@PRID", SqlDbType.Int).Value = prId

                    Using rdr = cmd.ExecuteReader(CommandBehavior.SingleRow)
                        If rdr.Read() Then
                            txtreqoff.Text = If(TryCast(rdr("OFFSEC"), String), "").ToString()
                            txtrcenter.Text = If(TryCast(rdr("RCCODE"), String), "").ToString()
                            txtfcluster.Text = If(TryCast(rdr("FCLUSTER"), String), "").ToString()
                        Else
                            MessageBox.Show("PR record not found for the given PRID.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        End If
                    End Using
                End Using
            End Using

        Catch ex As Exception
            MessageBox.Show("Load error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub txtprno_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtprno.KeyPress
        e.Handled = True
    End Sub

    Private Sub txtfcluster_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtfcluster.KeyPress
        e.Handled = True
    End Sub


    Private Sub txtttype_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtttype.KeyPress
        e.Handled = True
    End Sub
End Class