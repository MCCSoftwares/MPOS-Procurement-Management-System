Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Threading
Imports System.Threading.Tasks

Public Class ConnectionLatencyMonitor
    Private ReadOnly _getConnString As Func(Of String)
    Private ReadOnly _label As Label
    Private ReadOnly _owner As Form

    Private ReadOnly _pollMs As Integer
    Private ReadOnly _openTimeoutSec As Integer
    Private ReadOnly _cmdTimeoutSec As Integer
    Private ReadOnly _fastMs As Integer
    Private ReadOnly _moderateMs As Integer

    ' Error-burst detection
    Private ReadOnly _errorBurstCount As Integer
    Private ReadOnly _errorBurstWindowSec As Integer
    Private ReadOnly _maxRetries As Integer = 2            ' small retry for transient errors
    Private ReadOnly _retryBaseDelayMs As Integer = 150

    Private ReadOnly _errorTimes As New Queue(Of DateTime)
    Private _cts As CancellationTokenSource

    Public Sub New(
        getConnString As Func(Of String),
        targetLabel As Label,
        ownerForm As Form,
        Optional pollMs As Integer = 7000,
        Optional openTimeoutSec As Integer = 10,           ' ← give Cloud SQL more room
        Optional cmdTimeoutSec As Integer = 3,
        Optional fastThresholdMs As Integer = 150,
        Optional moderateThresholdMs As Integer = 350,
        Optional errorBurstCount As Integer = 3,
        Optional errorBurstWindowSec As Integer = 30
    )
        _getConnString = getConnString
        _label = targetLabel
        _owner = ownerForm
        _pollMs = pollMs
        _openTimeoutSec = openTimeoutSec
        _cmdTimeoutSec = cmdTimeoutSec
        _fastMs = fastThresholdMs
        _moderateMs = moderateThresholdMs
        _errorBurstCount = errorBurstCount
        _errorBurstWindowSec = errorBurstWindowSec
    End Sub

    Public Sub Start()
        If _cts IsNot Nothing Then Return
        _cts = New CancellationTokenSource()

        ' Optional: initial UI
        SafeOnUI(Sub()
                     If _label IsNot Nothing AndAlso Not _label.IsDisposed Then
                         _label.AutoSize = True
                         _label.Text = "● Checking…"
                         _label.ForeColor = Color.Gray
                     End If
                 End Sub)

        ' small jitter to avoid sync across many clients
        Dim rnd As New Random()
        Task.Run(Async Function()
                     Try
                         Await Task.Delay(rnd.Next(250, 900), _cts.Token)
                         Await MonitorLoopAsync(_cts.Token)
                     Catch
                         ' ignore on shutdown
                     End Try
                 End Function)
    End Sub

    Public Sub [Stop]()
        If _cts Is Nothing Then Return
        _cts.Cancel()
        _cts.Dispose()
        _cts = Nothing
    End Sub

    Private Async Function MonitorLoopAsync(ct As CancellationToken) As Task
        While Not ct.IsCancellationRequested
            Dim res = Await TestDbLatencyOnceWithRetryAsync(ct)
            UpdateStatusLabel(res)
            If Not res.Ok Then
                NoteErrorAndMaybeShowForm()
            End If

            Try
                Await Task.Delay(_pollMs, ct)
            Catch
                Exit While
            End Try
        End While
    End Function

    Private Structure LatencyResult
        Public Ok As Boolean
        Public Ms As Long
        Public Message As String
    End Structure

    ' --- NEW: small retry wrapper for transient cloud blips --------------------
    Private Async Function TestDbLatencyOnceWithRetryAsync(ct As CancellationToken) As Task(Of LatencyResult)
        Dim attempt As Integer = 0
        Dim lastErr As String = ""
        Dim sw As New Stopwatch()

        While attempt <= _maxRetries AndAlso Not ct.IsCancellationRequested
            Dim res = Await TestDbLatencyOnceAsync(ct)
            If res.Ok Then
                Return res
            End If

            lastErr = res.Message
            attempt += 1

            If attempt <= _maxRetries AndAlso IsTransient(res.Message) Then
                ' exponential backoff with jitter
                Dim delayMs = _retryBaseDelayMs * CInt(Math.Pow(2, attempt - 1))
                delayMs += (New Random()).Next(0, 120)
                Try
                    Await Task.Delay(delayMs, ct)
                Catch
                    Exit While
                End Try
            Else
                Exit While
            End If
        End While

        Return New LatencyResult With {.Ok = False, .Ms = 0, .Message = lastErr}
    End Function

    ' Simple transient classification (covers common network/timeout cases)
    Private Function IsTransient(msg As String) As Boolean
        If String.IsNullOrEmpty(msg) Then Return False
        Dim m = msg.ToLowerInvariant()

        ' General timeouts / network resets
        If m.Contains("timeout") OrElse m.Contains("operation timed out") Then Return True
        If m.Contains("semaphore timeout") Then Return True
        If m.Contains("the wait operation timed out") Then Return True
        If m.Contains("a connection attempt failed") Then Return True
        If m.Contains("transport-level error") Then Return True
        If m.Contains("the connection was terminated") Then Return True
        If m.Contains("forcibly closed") OrElse m.Contains("connection reset") Then Return True
        If m.Contains("error: -2") OrElse m.Contains("error: 258") Then Return True ' Sql timeout / wait timeout
        If m.Contains("error: 53") OrElse m.Contains("error: 10060") OrElse m.Contains("10054") Then Return True ' network
        Return False
    End Function

    Private Async Function TestDbLatencyOnceAsync(ct As CancellationToken) As Task(Of LatencyResult)
        Dim cs As String = ""
        Try
            cs = _getConnString?.Invoke()
        Catch
        End Try

        If String.IsNullOrWhiteSpace(cs) Then
            Return New LatencyResult With {.Ok = False, .Ms = 0, .Message = "No connection string"}
        End If

        Dim sb As SqlConnectionStringBuilder
        Try
            sb = New SqlConnectionStringBuilder(cs)
        Catch ex As Exception
            Return New LatencyResult With {.Ok = False, .Ms = 0, .Message = "Bad connection string"}
        End Try

        ' --- IMPORTANT CHANGE:
        ' Do NOT clamp ConnectTimeout down to a tiny value.
        ' Give it at least _openTimeoutSec (Cloud SQL often needs >3s initially).
        Dim current = If(sb.ConnectTimeout <= 0, 15, sb.ConnectTimeout)
        sb.ConnectTimeout = Math.Max(current, _openTimeoutSec)

        ' Ensure encryption defaults are sane for Cloud SQL
        ' (skip if user already set explicitly)
        If Not cs.ToLowerInvariant().Contains("encrypt=") Then
            sb.Encrypt = True
        End If
        If Not cs.ToLowerInvariant().Contains("trustservercertificate=") AndAlso Not cs.ToLowerInvariant().Contains("certificate=") Then
            ' If you're not deploying a cert, this avoids validation issues over public IP.
            sb.TrustServerCertificate = True
        End If

        ' Linked CTS to enforce a *total* budget for open + command
        Dim totalBudgetMs As Integer = (sb.ConnectTimeout * 1000) + (_cmdTimeoutSec * 1000)
        Using timeoutCts As New CancellationTokenSource(totalBudgetMs)
            Using linked As CancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(ct, timeoutCts.Token)
                Try
                    Using conn As New SqlConnection(sb.ToString())
                        Using cmd As New SqlCommand("SET NOCOUNT ON; SELECT 1;", conn)
                            cmd.CommandTimeout = _cmdTimeoutSec

                            Dim sw As New Stopwatch()
                            sw.Start()
                            Await conn.OpenAsync(linked.Token).ConfigureAwait(False)
                            Dim result = Await cmd.ExecuteScalarAsync(linked.Token).ConfigureAwait(False)
                            sw.Stop()

                            ' sanity check
                            If result Is Nothing OrElse Convert.ToInt32(result) <> 1 Then
                                Return New LatencyResult With {.Ok = False, .Ms = 0, .Message = "Ping failed"}
                            End If

                            Return New LatencyResult With {.Ok = True, .Ms = sw.ElapsedMilliseconds, .Message = "OK"}
                        End Using
                    End Using
                Catch ex As OperationCanceledException
                    Return New LatencyResult With {.Ok = False, .Ms = 0, .Message = "Timeout/Cancelled"}
                Catch ex As Exception
                    Return New LatencyResult With {.Ok = False, .Ms = 0, .Message = ex.Message}
                End Try
            End Using
        End Using
    End Function

    Private Sub UpdateStatusLabel(result As LatencyResult)
        SafeOnUI(Sub()
                     If _label Is Nothing OrElse _label.IsDisposed Then Return

                     If Not result.Ok Then
                         _label.Text = "● Error: " & result.Message
                         _label.ForeColor = Color.Red
                         Return
                     End If

                     Dim text As String
                     Dim c As Color

                     If result.Ms < _fastMs Then
                         c = Color.FromArgb(0, 153, 51) ' green
                         text = $"● Connection: Fast ({result.Ms} ms)"
                     ElseIf result.Ms < _moderateMs Then
                         c = Color.FromArgb(255, 140, 0) ' orange
                         text = $"● Connection: Moderate ({result.Ms} ms)"
                     Else
                         c = Color.Red
                         text = $"● Connection: Slow ({result.Ms} ms)"
                     End If

                     _label.Text = text
                     _label.ForeColor = c
                 End Sub)
    End Sub

    Private Sub NoteErrorAndMaybeShowForm()
        Dim now As DateTime = DateTime.UtcNow
        SyncLock _errorTimes
            _errorTimes.Enqueue(now)
            ' prune outside window
            While _errorTimes.Count > 0 AndAlso (now - _errorTimes.Peek()).TotalSeconds > _errorBurstWindowSec
                _errorTimes.Dequeue()
            End While
            If _errorTimes.Count >= _errorBurstCount Then
                ' reset to avoid repeated opens
                _errorTimes.Clear()
                SafeOnUI(Sub() ShowConnErrorForm())
            End If
        End SyncLock
    End Sub

    Private Sub ShowConnErrorForm()
        If _owner Is Nothing OrElse _owner.IsDisposed Then Return

        ' Avoid multiple instances
        For Each f As Form In Application.OpenForms
            If TypeOf f Is frmconnerror Then
                If Not f.WindowState = FormWindowState.Minimized Then
                    f.Activate()
                Else
                    f.WindowState = FormWindowState.Normal
                    f.Activate()
                End If
                Return
            End If
        Next

        Try
            Dim f As New frmconnerror()
            f.StartPosition = FormStartPosition.CenterParent
            f.Show(_owner)
        Catch
            Try
                Dim f As New frmconnerror()
                f.StartPosition = FormStartPosition.CenterScreen
                f.Show()
            Catch
                ' swallow
            End Try
        End Try
    End Sub

    Private Sub SafeOnUI(action As Action)
        If action Is Nothing Then Return
        Try
            If _owner IsNot Nothing AndAlso Not _owner.IsDisposed AndAlso _owner.InvokeRequired Then
                _owner.BeginInvoke(action)
            Else
                action()
            End If
        Catch
            ' ignore UI race conditions during shutdown
        End Try
    End Sub
End Class
