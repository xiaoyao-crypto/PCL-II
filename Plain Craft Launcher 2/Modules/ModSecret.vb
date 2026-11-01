'由于包含加解密等安全信息，本文件中的部分代码已被删除

Imports System.Linq

Friend Module ModSecret

    '标注 PCL 的不同分支，仅用于替换标记
    Public Const VersionBranchMain As String = "OpenSource"
    '在开源版的注册表与常规版的注册表隔离，以防数据冲突
    Public Const RegFolder As String = "PCLDebug"
    '用于微软登录的 ClientId
    'PCL-II 分支：内置公开的微软登录 Client ID（来源：PCL 官方社区版 PCL-CE v2.15.1 发行版内置值，已通过微软设备码端点验证可用），保证正版登录开箱即用；环境变量 PCL_MS_CLIENT_ID 可覆盖
    Public OAuthClientId As String = If(Environment.GetEnvironmentVariable("PCL_MS_CLIENT_ID"), "c14b0370-8d75-42f8-b329-5b60d39e319f")
    'CurseForge API Key
    Public CurseForgeAPIKey As String = If(Environment.GetEnvironmentVariable("PCL_CURSEFORGE_API_KEY"), "")
    '用于匿名数据收集的腾讯云日志服务上报 URL，形如 https://{region}.cls.tencentcs.com/track?topic_id={topic_id}
    Public Const ClsBaseUrl As String = ""

#Region "网络鉴权"

    Public Const RestrictMcDownload As Boolean = False 'PCL-II 分支：恢复 MC 下载功能
    Friend Function SecretCdnSign(UrlWithMark As String) As String
        If Not UrlWithMark.EndsWithF("{CDN}") Then Return UrlWithMark
        Return UrlWithMark.Replace("{CDN}", "").Replace(" ", "%20")
    End Function
    ''' <summary>
    ''' 设置 Headers 的 UA、Referer。
    ''' </summary>
    Friend Sub SecretHeadersSign(Url As String, ByRef Req As HttpRequestMessage, Optional SimulateBrowserHeaders As Boolean = False)
        If Not Req.Headers.UserAgent.Any Then
            If Url.Contains("baidupcs.com") OrElse Url.Contains("baidu.com") Then
                Req.Headers.Add("User-Agent", "LogStatistic")  '#4951
            ElseIf SimulateBrowserHeaders Then
                Req.Headers.Add("User-Agent", $"PCL2/{VersionBaseName}.{CInt(BuildType)} Mozilla/5.0 AppleWebKit/537.36 Chrome/63.0.3239.132 Safari/537.36")
            Else
                Req.Headers.Add("User-Agent", $"PCL2/{VersionBaseName}.{CInt(BuildType)}")
            End If
        End If
        If Not SimulateBrowserHeaders Then Req.Headers.Add("Referer", $"http://{VersionCode}.open.pcl2.server/")
        If Url.Contains("api.curseforge.com") OrElse Url.Contains("forgecdn.net") Then Req.Headers.Add("x-api-key", CurseForgeAPIKey)
    End Sub

#End Region

#Region "主题"

    Public Color1 As New MyColor(52, 61, 74)
    Public Color2 As New MyColor(11, 91, 203)
    Public Color3 As New MyColor(19, 112, 243)
    Public Color4 As New MyColor(72, 144, 245)
    Public Color5 As New MyColor(150, 192, 249)
    Public Color6 As New MyColor(213, 230, 253)
    Public Color7 As New MyColor(222, 236, 253)
    Public Color8 As New MyColor(234, 242, 254)
    Public ColorBg0 As New MyColor(150, 192, 249)
    Public ColorBg1 As New MyColor(190, Color7)
    Public ColorGray1 As New MyColor(64, 64, 64)
    Public ColorGray2 As New MyColor(115, 115, 115)
    Public ColorGray3 As New MyColor(140, 140, 140)
    Public ColorGray4 As New MyColor(166, 166, 166)
    Public ColorGray5 As New MyColor(204, 204, 204)
    Public ColorGray6 As New MyColor(235, 235, 235)
    Public ColorGray7 As New MyColor(240, 240, 240)
    Public ColorGray8 As New MyColor(245, 245, 245)
    Public ColorSemiTransparent As New MyColor(1, Color8)

    Public ThemeNow As Integer = -1
    Public ColorHue As Integer = 210, ColorSat As Integer = 85, ColorLightAdjust As Integer = 0, ColorHueTopbarDelta As OneOf(Of Integer, Integer()) = 0
    Public ThemeDontClick As Integer = 0

    Public Sub ThemeRefresh(Optional NewTheme As Integer = -1)
        Try
            If ThemeNow = NewTheme AndAlso NewTheme >= 0 Then Return
            If NewTheme >= 0 Then ThemeNow = NewTheme

            Color1 = New MyColor().FromHSL2(ColorHue, ColorSat * 0.2, 25 + ColorLightAdjust * 0.3)
            Color2 = New MyColor().FromHSL2(ColorHue, ColorSat, 45 + ColorLightAdjust)
            Color3 = New MyColor().FromHSL2(ColorHue, ColorSat, 55 + ColorLightAdjust)
            Color4 = New MyColor().FromHSL2(ColorHue, ColorSat, 65 + ColorLightAdjust)
            Color5 = New MyColor().FromHSL2(ColorHue, ColorSat, 80 + ColorLightAdjust * 0.4)
            Color6 = New MyColor().FromHSL2(ColorHue, ColorSat, 91 + ColorLightAdjust * 0.1)
            Color7 = New MyColor().FromHSL2(ColorHue, ColorSat, 95)
            Color8 = New MyColor().FromHSL2(ColorHue, ColorSat, 97)
            ColorBg0 = Color4 * 0.4 + Color5 * 0.4 + ColorGray4 * 0.2
            ColorBg1 = New MyColor(190, Color7)

            ColorSemiTransparent = New MyColor(1, Color8)
            Application.Current.Resources("ColorBrush1") = New SolidColorBrush(Color1)
            Application.Current.Resources("ColorBrush2") = New SolidColorBrush(Color2)
            Application.Current.Resources("ColorBrush3") = New SolidColorBrush(Color3)
            Application.Current.Resources("ColorBrush4") = New SolidColorBrush(Color4)
            Application.Current.Resources("ColorBrush5") = New SolidColorBrush(Color5)
            Application.Current.Resources("ColorBrush6") = New SolidColorBrush(Color6)
            Application.Current.Resources("ColorBrush7") = New SolidColorBrush(Color7)
            Application.Current.Resources("ColorBrush8") = New SolidColorBrush(Color8)
            Application.Current.Resources("ColorBrushBg0") = New SolidColorBrush(ColorBg0)
            Application.Current.Resources("ColorBrushBg1") = New SolidColorBrush(ColorBg1)
            Application.Current.Resources("ColorObject1") = CType(Color1, Color)
            Application.Current.Resources("ColorObject2") = CType(Color2, Color)
            Application.Current.Resources("ColorObject3") = CType(Color3, Color)
            Application.Current.Resources("ColorObject4") = CType(Color4, Color)
            Application.Current.Resources("ColorObject5") = CType(Color5, Color)
            Application.Current.Resources("ColorObject6") = CType(Color6, Color)
            Application.Current.Resources("ColorObject7") = CType(Color7, Color)
            Application.Current.Resources("ColorObject8") = CType(Color8, Color)
            Application.Current.Resources("ColorObjectBg0") = CType(ColorBg0, Color)
            Application.Current.Resources("ColorObjectBg1") = CType(ColorBg1, Color)
            ThemeRefreshMain()
        Catch ex As Exception
            Logger.Error(ex, "刷新主题颜色失败", LogBehavior.Toast)
        End Try
    End Sub
    Public Sub ThemeRefreshMain()
        RunInUi(
        Sub()
            If Not FrmMain.IsLoaded Then Return
            '顶部条背景
            Dim Brush = New LinearGradientBrush With {.EndPoint = New Point(1, 0), .StartPoint = New Point(0, 0)}
            Dim Deltas = ColorHueTopbarDelta.Switch(Function(d) New Integer() {-d, 0, d}, Function(d) d)
            Brush.GradientStops.Add(New GradientStop With {.Offset = 0, .Color = New MyColor().FromHSL2(ColorHue + Deltas(0), ColorSat, 48 + ColorLightAdjust)})
            Brush.GradientStops.Add(New GradientStop With {.Offset = 0.5, .Color = New MyColor().FromHSL2(ColorHue + Deltas(1), ColorSat, 54 + ColorLightAdjust)})
            Brush.GradientStops.Add(New GradientStop With {.Offset = 1, .Color = New MyColor().FromHSL2(ColorHue + Deltas(2), ColorSat, 48 + ColorLightAdjust)})
            FrmMain.PanTitle.Background = Brush
            FrmMain.PanTitle.Background.Freeze()
            '主页面背景
            If Settings.Get(Of Boolean)("UiBackgroundColorful") Then
                Brush = New LinearGradientBrush With {.EndPoint = New Point(0.1, 1), .StartPoint = New Point(0.9, 0)}
                Brush.GradientStops.Add(New GradientStop With {.Offset = -0.1, .Color = New MyColor().FromHSL2(ColorHue - 15, ColorSat * 0.8, 91)})
                Brush.GradientStops.Add(New GradientStop With {.Offset = 0.4, .Color = New MyColor().FromHSL2(ColorHue, ColorSat * 0.8, 91)})
                Brush.GradientStops.Add(New GradientStop With {.Offset = 1.1, .Color = New MyColor().FromHSL2(ColorHue + 15, ColorSat * 0.8, 91)})
                FrmMain.PanForm.Background = Brush
            Else
                FrmMain.PanForm.Background = New MyColor(245, 245, 245)
            End If
            FrmMain.PanForm.Background.Freeze()
        End Sub)
    End Sub
    'PCL-II 分支：解锁全部隐藏主题与自定义配色（含正版登录、反馈等成就式解锁）
    Friend Sub ThemeCheckAll(EffectSetup As Boolean)
        If FrmSetupUI Is Nothing Then Return
        Dim Unlock = Sub()
            With FrmSetupUI
                .RadioLauncherTheme5.IsEnabled = True
                .RadioLauncherTheme5Gray.Opacity = 0
                .RadioLauncherTheme5.Opacity = 1
                .RadioLauncherTheme6.IsEnabled = True
                .RadioLauncherTheme7.IsEnabled = True
                .RadioLauncherTheme8.IsEnabled = True
                .RadioLauncherTheme9.IsEnabled = True
                .RadioLauncherTheme10.IsEnabled = True
                .RadioLauncherTheme11.IsEnabled = True
                .RadioLauncherTheme12.IsEnabled = True
                .RadioLauncherTheme13.IsEnabled = True
                .RadioLauncherTheme14.IsEnabled = True
                .PanLauncherHide.Visibility = Visibility.Collapsed
            End With
        End Sub
        If RunInUi() Then
            Unlock()
        Else
            Application.Current.Dispatcher.Invoke(Unlock)
        End If
    End Sub
    Friend Function ThemeCheckOne(Id As Integer) As Boolean
        Return True
    End Function
    Friend Function ThemeUnlock(Id As Integer, Optional ShowDoubleHint As Boolean = True, Optional UnlockHint As String = Nothing) As Boolean
        'PCL-II 分支：全部主题已解锁；若调用方要求且提供了提示文本则弹出解锁提示
        If ShowDoubleHint AndAlso Not String.IsNullOrEmpty(UnlockHint) Then MyMsgBox(UnlockHint, "提示")
        Return True
    End Function

#End Region

#Region "更新"

    'PCL-II 分支：更新源为 xiaoyao-crypto/PCL-II 的 GitHub Release
    Private Const UpdateRepoOwner As String = "xiaoyao-crypto"
    Private Const UpdateRepoName As String = "PCL-II"
    Private UpdateNewExePath As String = ""
    Friend IsUpdateWaitingRestart As Boolean = False

    Friend Sub UpdateCheckByButton()
        Hint("正在检查更新……")
        Try
            '获取最新 Release 信息
            Dim ReleaseJson = NetRequestByLoader($"https://api.github.com/repos/{UpdateRepoOwner}/{UpdateRepoName}/releases/latest", Timeout:=20000, IsJson:=True, SimulateBrowserHeaders:=True)
            Dim Release = JObject.Parse(ReleaseJson)
            Dim TagName = Release("tag_name").ToString()
            '获取该 Release 对应提交
            Dim CommitJson = NetRequestByLoader($"https://api.github.com/repos/{UpdateRepoOwner}/{UpdateRepoName}/commits/{System.Uri.EscapeDataString(TagName)}", Timeout:=20000, IsJson:=True, SimulateBrowserHeaders:=True)
            Dim RemoteCommit = JObject.Parse(CommitJson)("sha").ToString()
            '与当前版本比较（本地 CommitHash 由构建时注入）
            If RemoteCommit.ToLowerInvariant() = CommitHash.ToLowerInvariant() Then
                MyMsgBox("当前已是最新版本（" & TagName & "）。", "检查更新")
                Return
            End If
            '查找 exe 资产
            Dim Assets = Release("assets").ToObject(Of List(Of JObject))()
            Dim ExeAsset = Assets.FirstOrDefault(Function(a) a("name").ToString().ToLowerInvariant().EndsWith(".exe"))
            If ExeAsset Is Nothing Then
                MyMsgBox("发现新版本 " & TagName & "，但该版本未附带可自动更新的程序文件（exe）。" & vbCrLf & "请到 GitHub Releases 页面手动下载：https://github.com/" & UpdateRepoOwner & "/" & UpdateRepoName & "/releases", "检查更新")
                Return
            End If
            '确认更新
            If MyMsgBox("发现新版本：" & TagName & vbCrLf & vbCrLf & "是否立即下载并更新？" & vbCrLf & "更新完成后需重启 PCL 生效。", "发现新版本", "更新", "取消") <> 1 Then Return
            Hint("正在下载更新……")
            UpdateNewExePath = PathTemp & "PCL-II-Update-" & GetUuid() & ".exe"
            NetDownloadByLoader(ExeAsset("browser_download_url").ToString(), UpdateNewExePath)
            '校验下载文件是否为有效程序
            If Not System.IO.File.Exists(UpdateNewExePath) Then Throw New Exception("下载失败：文件不存在")
            If New System.IO.FileInfo(UpdateNewExePath).Length < 1024 * 1024 Then Throw New Exception("下载失败：文件大小异常")
            Dim Header(1) As Byte
            Using fs = System.IO.File.OpenRead(UpdateNewExePath)
                fs.Read(Header, 0, 2)
            End Using
            If Header(0) <> Asc("M"c) OrElse Header(1) <> Asc("Z"c) Then Throw New Exception("下载失败：文件格式不正确")
            '提示重启完成更新
            If MyMsgBox("新版本 " & TagName & " 已下载完成。" & vbCrLf & "重启 PCL 后将自动完成更新，立即重启吗？", "更新完成", "立即重启", "稍后") = 1 Then
                UpdateRestart(True)
            Else
                IsUpdateWaitingRestart = True '稍后更新：退出 PCL 时自动完成
                Hint("更新将在退出 PCL 时自动完成。")
            End If
        Catch ex As Exception
            ex.ThrowIfCanceled()
            MyMsgBox("检查更新失败：" & ex.Message & vbCrLf & vbCrLf & "请检查网络连接后重试，或到 GitHub Releases 页面手动下载。", "检查更新")
        End Try
    End Sub

    Public Sub UpdateRestart(TriggerRestartAndByEnd As Boolean)
        'PCL-II 分支：exe 正在运行无法直接覆盖，通过延迟替换脚本（cmd 独立进程）完成更新
        If UpdateNewExePath = "" OrElse Not System.IO.File.Exists(UpdateNewExePath) Then Return
        Dim OldExePath = PathExe
        If PathUtils.IsSame(UpdateNewExePath, OldExePath) Then Return
        Dim BatPath = PathTemp & "PCL-II-Update.bat"
        Dim OldEscaped = OldExePath.Replace("%", "%%")
        Dim NewEscaped = UpdateNewExePath.Replace("%", "%%")
        Dim BatContent = "@echo off" & vbCrLf &
            "setlocal EnableDelayedExpansion" & vbCrLf &
            "set /a n=0" & vbCrLf &
            ":retry" & vbCrLf &
            "ping 127.0.0.1 -n 2 >nul" & vbCrLf &
            "del /f /q """ & OldEscaped & """ 2>nul" & vbCrLf &
            "if not exist """ & OldEscaped & """ goto replace" & vbCrLf &
            "set /a n+=1" & vbCrLf &
            "if !n! lss 30 goto retry" & vbCrLf &
            "goto giveup" & vbCrLf &
            ":replace" & vbCrLf &
            "move /y """ & NewEscaped & """ """ & OldEscaped & """ >nul 2>&1" & vbCrLf &
            "if errorlevel 1 goto giveup" & vbCrLf &
            "start """"" & OldEscaped & """" & vbCrLf &
            ":giveup" & vbCrLf &
            "del /f /q ""%~f0"" >nul 2>&1" & vbCrLf
        System.IO.File.WriteAllText(BatPath, BatContent)
        Try
            System.Diagnostics.Process.Start(New System.Diagnostics.ProcessStartInfo("cmd.exe", "/c call """ & BatPath & """") With {
                .WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                .CreateNoWindow = True,
                .UseShellExecute = False
            })
        Catch ex As Exception
            Logger.Warn(ex, "启动更新脚本失败")
            MyMsgBox("自动更新失败，请到 GitHub Releases 页面手动下载：https://github.com/" & UpdateRepoOwner & "/" & UpdateRepoName & "/releases", "更新失败")
            Return
        End Try
        Environment.Exit(0)
    End Sub

    Public Sub UpdateReplace(ProcessId As Integer, OldFileName As String, NewFileName As String, TriggerRestart As Boolean)
        'PCL-II 分支：兼容 --update 命令行调用（等待旧进程退出后替换文件）
        Try
            If ProcessId <> 0 Then
                Try
                    Dim OldProcess = System.Diagnostics.Process.GetProcessById(ProcessId)
                    OldProcess.WaitForExit()
                Catch ex As Exception
                End Try
            End If
            Try
                System.IO.File.Replace(NewFileName, OldFileName, Nothing, True)
            Catch ex As Exception
                System.IO.File.Delete(OldFileName)
                System.IO.File.Move(NewFileName, OldFileName)
            End Try
            If TriggerRestart AndAlso System.IO.File.Exists(OldFileName) Then
                System.Diagnostics.Process.Start(OldFileName)
            End If
        Catch ex As Exception
            Logger.Warn(ex, "命令行更新替换失败")
        End Try
        Environment.Exit(0)
    End Sub

    ''' <summary>
    ''' 确保 PathTemp/Latest.exe 是最新正式版的 PCL，它会被用于整合包打包。
    ''' 如果不是，则下载一个。
    ''' </summary>
    Friend Sub DownloadLatestPCL(Optional LoaderToSyncProgress As LoaderBase = Nothing)
        '注意：如果要自行实现这个功能，请换用另一个文件路径，以免与官方版本冲突
    End Sub

#End Region

#Region "联网配置"

    ''' <summary>
    ''' 联网获取的配置信息。
    ''' 若获取失败或仍在获取中，可能为 Nothing。
    ''' </summary>
    Public ServerConfig As JObject

    Public ServerLoader As New LoaderTask(Of Integer, Integer)("PCL 配置更新", Sub() Logger.Info("该版本中不包含更新通知功能……"), Priority:=ThreadPriority.BelowNormal) With
        {.ReloadTimeout = 1000 * 60 * 60} '超时 1 小时

#End Region

#Region "赞助等级"

    Public ReadOnly Property CurrentRank As DonationRank
        Get
            Return DonationRank.None
        End Get
    End Property

    Public Sub InputPotatoCode(IsUpdating As Boolean)
    End Sub
    Friend Sub GeneratePotatoCode()
    End Sub

    ''' <summary>
    ''' 获取设备识别码。
    ''' </summary>
    Friend Function GetIdentify() As String
        Return "0000-0000-0000-0000"
    End Function

#End Region

End Module
