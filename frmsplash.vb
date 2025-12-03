Imports System.ComponentModel
Imports System.IO
Imports System.Windows.Forms
Imports System.Reflection
Imports Wsh = IWshRuntimeLibrary   ' <— alias avoids File type conflict
Imports System.Deployment.Application

Public Class frmsplash
    Private WithEvents splashTimer As New Timer() With {.Interval = 3000}

    Private Sub frmsplash_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim assemblyVer As Version = Assembly.GetExecutingAssembly().GetName().Version
        Dim textToShow As String

        If ApplicationDeployment.IsNetworkDeployed Then
            Dim deployVer As Version = ApplicationDeployment.CurrentDeployment.CurrentVersion
            textToShow = $"Version {deployVer}  (assembly {assemblyVer})"
        Else
            textToShow = $"Version {assemblyVer}  (not network deployed)"
        End If

        lblversion.Text = textToShow

        ' your GIF logic...
        Dim imagePath As String = Path.Combine(Application.StartupPath, "Resources", "Loading.gif")
        If System.IO.File.Exists(imagePath) Then
            PictureBox1.Image = Image.FromFile(imagePath)
        Else
            MessageBox.Show("The image file 'Loading.gif' could not be found in the Resources folder.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If

        splashTimer.Start()
    End Sub

    Private Sub splashTimer_Tick(sender As Object, e As EventArgs) Handles splashTimer.Tick
        splashTimer.Stop()

        ' Create desktop shortcut once (safe to call repeatedly)
        EnsureDesktopShortcut()

        Me.Dispose()
        DateFormatHelper.SetDateFormat()
        frmmain.getconnstring()
        frmmain.Show()
    End Sub

    Private Sub EnsureDesktopShortcut()
        Try
            ' Uses the values you set in Publish → Options → Description
            Dim publisher As String = My.Application.Info.CompanyName
            Dim product As String = My.Application.Info.ProductName

            ' Start-menu appref path (ClickOnce creates this)
            Dim startMenu As String = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Programs),
                publisher
            )
            Dim appref As String = Path.Combine(startMenu, product & ".appref-ms")

            ' Fallback: search if exact path not found (handles different folder names)
            If Not System.IO.File.Exists(appref) AndAlso Directory.Exists(startMenu) Then
                Dim candidates = Directory.GetFiles(startMenu, "*.appref-ms", SearchOption.AllDirectories)
                appref = candidates.FirstOrDefault(Function(p) Path.GetFileNameWithoutExtension(p).IndexOf(product, StringComparison.OrdinalIgnoreCase) >= 0)
                If String.IsNullOrEmpty(appref) Then Return
            ElseIf Not System.IO.File.Exists(appref) Then
                Return
            End If

            ' Create desktop shortcut if missing
            Dim desktop As String = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)
            Dim shortcutPath As String = Path.Combine(desktop, product & ".lnk")
            If System.IO.File.Exists(shortcutPath) Then Return

            Dim shell As New Wsh.WshShell()
            Dim sc As Wsh.IWshShortcut = CType(shell.CreateShortcut(shortcutPath), Wsh.IWshShortcut)
            sc.TargetPath = appref                 ' point to the ClickOnce .appref-ms
            sc.WorkingDirectory = Path.GetDirectoryName(appref)
            sc.IconLocation = appref & ",0"
            sc.WindowStyle = 1
            sc.Description = product
            sc.Save()
        Catch
            ' swallow or log; shortcut creation is non-critical
        End Try
    End Sub
End Class
