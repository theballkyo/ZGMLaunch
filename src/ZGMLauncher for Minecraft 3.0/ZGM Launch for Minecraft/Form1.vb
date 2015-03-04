﻿Imports System
Imports System.IO
Imports System.Net
Imports Ionic.Zip
Imports System.Threading
Imports System.Security.Principal
Imports System.Timers
Imports ZGMLaunch
Imports System.Text.RegularExpressions
Imports System.Drawing.Text

Public Class Form1
    Dim keyvalue(10) As String
    Dim keyname(10) As String
    Dim keyvaluesv(10) As String
    Dim keynamesv(10) As String
    Dim errorcheck As String
    Dim file_data(3, 60000) As String
    'Dim file_delete(,) As String
    Dim fn As String
    Dim path_file As String
    Dim md5f As String
    Dim server As String = "http://mc.zone-gamer.th.ht/patch/" 'Old website :(
    Dim wc As WebClient = New WebClient
    Dim time As Double

    Dim procStartInfo As New System.Diagnostics.ProcessStartInfo()
    Dim proc As System.Diagnostics.Process = New Process()

    Public Shared ProgramFilesPath As String = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) & "\"
    Public Shared MyPath As String = Application.StartupPath + "\"
    Public Shared server_file_name As String = "zgm"
    Public Shared UrlAuth As String = ""

    'Delegate Sub ChangeTextsSafe(ByVal length As Long, ByVal position As Integer, ByVal speed As Double, ByVal filename As String)
    Delegate Sub DownloadTextSafe(ByVal percentage As Double, ByVal speed As Double, ByVal bytesIn As Double, ByVal totalBytes As Double)
    Delegate Sub ChangeStatusSafe(ByVal text As String)
    Delegate Sub DownloadCompleteSafe(ByVal cancelled As Boolean)
    Delegate Sub showform()
    Delegate Sub GameLogSafe(ByVal text As String)
    Delegate Sub ExitGameSafe()
    Private Declare Unicode Function WritePrivateProfileString Lib "kernel32" _
         Alias "WritePrivateProfileStringW" (ByVal lpApplicationName As String, _
         ByVal lpKeyName As String, ByVal lpString As String, _
        ByVal lpFileName As String) As Int32

    Private Declare Unicode Function GetPrivateProfileString Lib "kernel32" _
    Alias "GetPrivateProfileStringW" (ByVal lpApplicationName As String, _
    ByVal lpKeyName As String, ByVal lpDefault As String, _
    ByVal lpReturnedString As String, ByVal nSize As Int32, _
    ByVal lpFileName As String) As Int32

    Public Overloads Sub ReadINIFile(ByVal INIPath As String, _
                                      ByVal SectionName As String, ByVal KeyName As String(), _
                                      ByRef KeyValue As String())
        Dim Length As Integer
        Dim strData As String
        strData = Space$(1024)
        For i As Integer = 1 To KeyName.Length - 1
            If KeyName(i) <> "" Then
                'This will read the ini file using Section Name and Key
                Length = GetPrivateProfileString(SectionName, KeyName(i), KeyValue(i), _
                                            strData, strData.Length, LTrim(RTrim((INIPath))))
                If Length > 0 Then
                    KeyValue(i) = strData.Substring(0, Length)
                Else
                    KeyValue(i) = ""
                End If
            End If
        Next
    End Sub

    Public Sub INIWrite(ByVal INIPath As String, ByVal SectionName As String, _
ByVal KeyName As String, ByVal TheValue As String)
        Call WritePrivateProfileString(SectionName, KeyName, TheValue, INIPath)
    End Sub


    Public Sub DownloadComplete(ByVal cancelled As Boolean)
        ' Me.txtFileName.Enabled = True
        ' Me.btnDownload.Enabled = True
        ' Me.btnCancel.Enabled = False

        If cancelled Then

            Me.Label4.Text = "Cancelled"

            'MessageBox.Show("ยกเลิกการ Download", "Aborted", MessageBoxButtons.OK, MessageBoxIcon.Information)


        Else
            Me.Label4.Text = "Successfully downloaded"

            MessageBox.Show("Successfully downloaded!", "All OK", MessageBoxButtons.OK, MessageBoxIcon.Information)


        End If

        Me.ProgressBar1.Value = 0
        ' Me.Label5.Text = "Downloading: "
        'Me.Label6.Text = "Save to: "
        Me.Label3.Text = "File size: "
        Me.Label2.Text = "Download speed: "
        Me.Label4.Text = ""

    End Sub

    Public Sub ChangeTexts(ByVal length As Long, ByVal position As Integer, ByVal speed As Double, ByVal filename As String)

        Me.Label3.Text = "File Name : " & filename & " - File Size: " & Math.Round((length / 1024), 2) & " KB"

        'Me.Label5.Text = "Downloading: " & Me.txtFileName.Text

        Me.Label4.Text = "Downloaded " & Math.Round((position / 1024), 2) & " KB of " & Math.Round((length / 1024), 2) & "KB"

        If speed = -1 Then
            Me.Label2.Text = "Speed: calculating..."
        Else
            Me.Label2.Text = "Speed: " & Math.Round((speed / 1024), 2) & " KB/s"
        End If
        If length > 0 Then
            Me.ProgressBar1.Maximum = length
            Me.ProgressBar1.Value = position
        Else
            Me.Label3.Text = "กำลังดาวน์โหลด ... - ไม่สามารถค้นหาขนาดของไฟล์ได้"
            'Me.Label2.Text = "กำลังดาวน์โหลด - ไม่สามารถค้นหาขนาดของไฟล์ได้"
        End If


    End Sub

    Private Sub client_ProgressChanged(ByVal sender As Object, ByVal e As DownloadProgressChangedEventArgs)

        If BackgroundWorker1.CancellationPending Then
            wc.CancelAsync()
        End If

        Dim bytesIn As Double = Double.Parse(e.BytesReceived.ToString())
        Dim totalBytes As Double = Double.Parse(e.TotalBytesToReceive.ToString())
        Dim percentage As Double = bytesIn / totalBytes * 100

        Dim bytesDownloaded As Long = bytesIn
        Dim speed As Double = bytesDownloaded / (time * 102.4)

        'Dim safeDelegate As New change_textSafe(AddressOf change_text)
        'Me.Invoke(safeDelegate, percentage, speed, bytesIn, totalBytes)
        BeginInvoke(New DownloadTextSafe(AddressOf DownloadText), percentage, speed, bytesIn, totalBytes)
    End Sub

    Private Sub client_DownloadCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.AsyncCompletedEventArgs)
        If e.Error IsNot Nothing Then
            'MsgBox(e.Error.Message)
            errorcheck = 1
            BackgroundWorker1.CancelAsync()
        End If
    End Sub

    Private Sub DownloadText(ByVal percentage As Double, ByVal speed As Double, ByVal bytesIn As Double, ByVal totalBytes As Double)
        ProgressBar1.Value = Int32.Parse(Math.Truncate(percentage).ToString())

        Me.Label3.Text = "File Name : " & fn & " - File Size: " & Math.Round((totalBytes / 1024), 2) & " KB"

        If speed < 1024 Then
            Me.Label2.Text = String.Format("Speed: {0:0.00} kb/s", speed)
        Else
            Me.Label2.Text = String.Format("Speed: {0:0.00} mb/s", speed / 1024)
        End If
        'KB
        Dim Filesize = totalBytes / 1024
        Dim sFilesize As String = "??? MB."
        Select Case Filesize
            Case Is < 1024
                sFilesize = String.Format("{0:0.00} KB.", Filesize)
            Case Is < 1048576
                sFilesize = String.Format("{0:0.00} MB.", Filesize / 1024)
            Case Is < 1073741824
                sFilesize = String.Format("{0:0.00} GB.", Filesize / 1024 / 1024)
            Case Is >= 1073741824
                sFilesize = String.Format("{0:0.00} TB.", Filesize / 1024 / 1024 / 1024)
        End Select

        bytesIn = bytesIn / 1024
        If bytesIn < 1024 Then
            Me.Label4.Text = String.Format("Received: {0:0.00} KB of " & sFilesize, bytesIn)
        ElseIf bytesIn < 1048576 Then
            Me.Label4.Text = String.Format("Received: {0:0.00} MB of " & sFilesize, bytesIn / 1024)
        Else
            Me.Label4.Text = String.Format("Received: {0:0.00} GB of " & sFilesize, bytesIn / 1024 / 1024)
        End If

    End Sub

    Private Sub ChangeStatus(ByVal text As String)
        Me.Label2.Text = text
    End Sub

    Public Sub showf()
        Dim frm As Form2
        If Application.OpenForms().OfType(Of Form2).Any Then

        Else
            If keyvalue(6) = "true" Then
                frm = New Form2("online")
                frm.Show()
            Else

                frm = New Form2("offline")
                frm.Show()
            End If
        End If
    End Sub

    Private Sub check()
        'valini(keyname, keyvalue, keyvaluesv)
        If File.Exists(MyPath & "server.ini") Then ' Check Server.ini file

            If (My.Settings.version < keyvaluesv(1)) Then 'Checkversion launch
                MsgBox("ตรวจพบเวอร์ชั่นโปรแกรมใหม่")
                dl(keyvalue(3) & "zgmlaunch.zip", "zgmlaunch.zip")

            ElseIf (keyvalue(2) < keyvaluesv(2)) Then 'Checkversion game
                Dim version As Integer = keyvalue(2) + 1
                dl(keyvalue(3) & "zgmpatch" & version & ".zip", "zgmdl.zip")

            End If

            If (keyvaluesv(1) <> "") Then 'Startgame
                Dim formshow As New showform(AddressOf showf)
                Me.Invoke(formshow)

            End If
        End If

        Exit Sub
    End Sub

    Private Sub dl(ByVal urlfile, ByVal filename)

        'Crating the request and getting the response
        Dim theResponse As HttpWebResponse
        Dim theRequest As HttpWebRequest
        Try 'Checks if the file exist

            theRequest = WebRequest.Create(urlfile)
            theResponse = theRequest.GetResponse

        Catch ex As Exception

            MessageBox.Show("เกิดข้อผิดพลาดขณะ download file. Possibe causes:" & ControlChars.CrLf & _
                            "1) ไม่มีไฟล์ในเซิฟเวอร์" & ControlChars.CrLf & _
                            "2) เซิฟเวอร์ทำงานผิดพลาด(อาจล่มอยู่)", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)

            Dim cancelDelegate As New DownloadCompleteSafe(AddressOf DownloadComplete)

            Me.Invoke(cancelDelegate, True)
            errorcheck = "1"
            Exit Sub
        End Try

        Dim length As Long = theResponse.ContentLength 'Size of the response (in bytes)

        'Dim safedelegate As New ChangeTextsSafe(AddressOf ChangeTexts)
        'Me.Invoke(safedelegate, length, 0, 0, filename) 'Invoke the TreadsafeDelegate

        'If Not Directory.Exists() Then

        'End If

        Dim writeStream As New IO.FileStream(MyPath + filename, IO.FileMode.Create)

        'Replacement for Stream.Position (webResponse stream doesn't support seek)
        Dim nRead As Integer

        'To calculate the download speed
        Dim speedtimer As New Stopwatch
        Dim currentspeed As Double = -1
        Dim readings As Integer = 0

        Do

            If BackgroundWorker1.CancellationPending Then 'If user abort download
                Exit Do
            End If

            speedtimer.Start()

            Dim readBytes(4095) As Byte
            Dim bytesread As Integer = theResponse.GetResponseStream.Read(readBytes, 0, 4096)

            nRead += bytesread

            'Me.Invoke(safedelegate, length, nRead, currentspeed, filename)

            If bytesread = 0 Then Exit Do

            writeStream.Write(readBytes, 0, bytesread)

            speedtimer.Stop()

            readings += 1
            If readings >= 5 Then 'For increase precision, the speed it's calculated only every five cicles
                currentspeed = 20480 / (speedtimer.ElapsedMilliseconds / 1000)
                speedtimer.Reset()
                readings = 0
            End If
        Loop

        'Close the streams
        theResponse.GetResponseStream.Close()
        writeStream.Close()

        If Me.BackgroundWorker1.CancellationPending Then

            IO.File.Delete(MyPath)

            Dim cancelDelegate As New DownloadCompleteSafe(AddressOf DownloadComplete)

            Me.Invoke(cancelDelegate, True)

            Exit Sub

        End If
        If (filename = "zgmlaunch.zip" Or filename = server_file_name + "\.clean\clean.zip" Or filename = server_file_name + "\.bigpatch\patch.zip" Or InStr(filename, server_file_name + "\.bigpatch\")) Then
            unzip(filename)
        End If
    End Sub
    Private Sub dl2(ByVal url, ByVal filename)
        AddHandler wc.DownloadProgressChanged, AddressOf client_ProgressChanged
        AddHandler wc.DownloadFileCompleted, AddressOf client_DownloadCompleted
        time = 0
        wc.DownloadFileAsync(New Uri(url), MyPath + filename)
        Do Until Not wc.IsBusy
            Thread.Sleep(10)
        Loop
    End Sub
    Private Sub unzip(ByVal filename)
        'Unzip
        Using zip As ZipFile = ZipFile.Read(MyPath + filename)

            If (filename = server_file_name + "\.clean\clean.zip") Then

                zip.ExtractAll(MyPath + server_file_name + "\.clean\", Ionic.Zip.ExtractExistingFileAction.OverwriteSilently)

                Dim p As New Process
                p.StartInfo.FileName = MyPath + server_file_name + "\.clean\cleanmain.exe"
                p.Start()
                If Not (p Is Nothing) Then
                    p.WaitForExit()
                End If
                Check_File()

            ElseIf (InStr(filename, server_file_name + "\.bigpatch\")) Then

                zip.ExtractAll(MyPath + server_file_name + "\.bigpatch\", Ionic.Zip.ExtractExistingFileAction.OverwriteSilently)
                Dim p As New Process
                Dim filename2 As String = Replace(filename, ".zip", ".exe")
                Dim filepath As String = """" + MyPath + filename2 + """"
                p.StartInfo.FileName = filepath
                'p.WaitForExit()
                p.Start()
                If Not (p Is Nothing) Then
                    p.WaitForExit()
                End If
            Else
                zip.ExtractAll(MyPath, Ionic.Zip.ExtractExistingFileAction.OverwriteSilently)
            End If

        End Using

        If (filename = "zgmdl.zip") Then
            'INIWrite(path & "config.ini", "client", "versionmc", keyvalue(2) + 1)
            File.Delete(MyPath & "zgmdl.zip")
            check()
        End If

        If (filename = "zgmclient.zip") Then
            'INIWrite(path & "config.ini", "client", "versionmc", keyvaluesv(4))
            File.Delete(MyPath & "zgmclient.zip")
            check()
        End If

        If (filename = "zgmlaunch.zip") Then
            'INIWrite(path & "config.ini", "client", "versionlaunch", keyvaluesv(1))
            File.Delete(MyPath & "zgmlaunch.zip")
            MsgBox("พบโปรแกรมเวอร์ชั่นใหม่" & ControlChars.CrLf & "โปรแกรมจะทำการ Restart !")
            Process.Start("update.exe")
            End
        End If



    End Sub

    Private Sub closeme_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles closeme.Click
        If BackgroundWorker1.IsBusy <> True Then
        Else
            BackgroundWorker1.CancelAsync()
        End If
        If RunGameWorker.IsBusy = True Then
            proc.Kill()
            RunGameWorker.CancelAsync()
        End If
        End
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        'Dim pfc As New PrivateFontCollection()
        'pfc.AddFontFile(MyPath & "Waffle Regular.otf")
        'Label1.Font = New Font(pfc.Families(0), 16, FontStyle.Bold)

        WebBrowser1.Navigate("http://mc.zone-gamer.th.ht/launchnew")
        AddHandler WebBrowser1.DocumentCompleted, New WebBrowserDocumentCompletedEventHandler(AddressOf PageWaiter)
        'Label1.Text = New System.Text.UTF7Encoding().GetString(Convert.FromBase64String("qTIwMTIgWkdNTGF1bmNoIDEuMC4yIERldmVsb3BlZCBieSBUaGViYWxsa3lv"))
        Label3.Text = ""
        Label4.Text = ""
        Label1.Text = "© ZGMLauncher " + My.Settings.fullversion + " :: ZGMLibrary " + ZGM.GetVersion() + " Developed by Theballkyo"
        'Load setting form config.ini 
        Loadsetting()

        ZGM.LockFile(MyPath & server_file_name)
    End Sub
    Private Sub valini(ByRef keyname, ByRef keyvalue)
        keyname(1) = "DirServer"
        keyname(2) = "title"
        keyname(3) = "dlserver"
        keyname(4) = "UrlAuth"
        keyname(5) = "UrlNews"
        keyname(6) = "OpenAuth"
        ReadINIFile(MyPath + "config.ini", "client", keyname, keyvalue)
        'ReadINIFile(path + "server.ini", "server", keyname, keyvaluesv)
    End Sub
    Private Sub Loadsetting()
        'ตัวแปร keyvalue
        '@keyvalue(1) = "DirServer"
        '@keyvalue(2) = "title"
        '@keyvalue(3) = "dlserver"
        '@keyvalue(4) = "UrlAuth"
        '@keyvalue(5) = "UrlNews"
        '@keyvalue(6) = "OpenAuth"
        valini(keyname, keyvalue)
        'server_file_name = keyvalue(1)
        Me.Text = keyvalue(2) + " :: Version " + My.Settings.fullversion
        'server = keyvalue(3)
        UrlAuth = keyvalue(4)
        If keyvalue(5) = "" Then
            WebBrowser1.Navigate("http://mc.zone-gamer.th.ht/freelauncher")
        Else
            WebBrowser1.Navigate(keyvalue(5))
        End If
    End Sub

    Private Sub startgame_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles startgame.Click
        RunGame()
        Return
        'If File.Exists(path & "server.ini") Then
        'File.Delete(path & "server.ini")
        'End If
        startgame.Enabled = False
        'dlgame.Enabled = False
        'valini(keyname, keyvalue, keyvaluesv)
        Timer1.Start()
        If BackgroundWorker1.IsBusy <> True Then
            Me.Label2.Text = "Status : Downloading patchlist"
            BackgroundWorker1.RunWorkerAsync()
        End If
    End Sub

    Private Sub PageWaiter(ByVal sender As Object, ByVal e As WebBrowserDocumentCompletedEventArgs)
        If WebBrowser1.ReadyState = WebBrowserReadyState.Complete Then
            If WebBrowser1.IsOffline Then
                WebBrowser1.Visible = True
            End If
            RemoveHandler WebBrowser1.DocumentCompleted, New WebBrowserDocumentCompletedEventHandler(AddressOf PageWaiter)
        End If
    End Sub

    Private Sub BackgroundWorker1_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        'reset
        errorcheck = "0"

        'Create Folder file
        If Not Directory.Exists(MyPath & server_file_name) Then
            Directory.CreateDirectory(MyPath & server_file_name)
        End If

        'File.Delete(path & "patchlist.xml")

        'Update XML in Server
        ZGM.UpdateXML(server)

        'valini(keyname, keyvalue, keyvaluesv)
        dl2(server & "zgm_patch.xml", "patchlist.xml")
        If errorcheck = "1" Then
            MsgBox("มีข้อผิดพลาดขณะทำการตรวจสอบไฟล์ กรุณาลองใหม่", MsgBoxStyle.Critical)
            Exit Sub
        End If
        Check_File()
    End Sub
    Public Sub Check_File()
        BeginInvoke(New ChangeStatusSafe(AddressOf ChangeStatus), "Status : Checking file ...")
        'MsgBox(CalcMD5((path + "test.xml")))
        Call xml_read.file_data_xml(MyPath + "patchlist.xml", file_data)
        ' Get bounds of the array.
        Dim bound0 As Integer = file_data.GetUpperBound(0)
        Dim bound1 As Integer = file_data.GetUpperBound(1)
        'Check program update
        'Dim zgmversion = file_data(3, 0)
        Dim sv_ver As String = ZGM.CheckUpdate(server_file_name)
        If Regex.IsMatch(sv_ver, "^[0-9]+$") Then
            If My.Settings.version < sv_ver Then
                dl2("http://dl1.zone-gamer.th.ht/mc/zgmlaunch.zip", "zgmlaunch.zip")
                unzip("zgmlaunch.zip")
            End If
        End If
        ' Loop over all elements.

        For i As Integer = 0 To bound1
            '0,3 = zgmversion ; 0,i = path ; 1,i = filename ; 2,i = md5
            path_file = file_data(0, i)
            fn = file_data(1, i)
            md5f = file_data(2, i)

            If fn = "" Then
                Exit For
            End If
            'MsgBox(fn + " : " + md5f)

            'check file
            If File.Exists(MyPath & server_file_name + "\" + path_file + "\" + fn) Then
                Dim md5_this = ZGM.CalcMD5(MyPath & server_file_name + "\" + path_file + "\" + fn)
                md5_this = md5_this.ToLower
                'Check md5
                If md5_this <> md5f Then
                    'Download new file
                    dl2(server + "game" + "/" + path_file + "/" + fn, server_file_name + "\" + path_file + "\" + fn)
                    If errorcheck = "1" Then
                        MsgBox("มีข้อผิดพลาดขณะทำการตรวจสอบไฟล์ กรุณาลองใหม่", MsgBoxStyle.Critical)
                        Exit Sub
                    End If
                End If
            Else
                'Download new file
                If path_file <> "" Then
                    If Not Directory.Exists(MyPath & server_file_name + "\" + path_file) Then
                        Directory.CreateDirectory(MyPath & server_file_name + "\" + path_file)
                    End If
                End If
                dl2(server + "game" + "/" + path_file + "/" + fn, server_file_name + "\" + path_file + "\" + fn)
                If errorcheck = "1" Then
                    MsgBox("มีข้อผิดพลาดขณะทำการตรวจสอบไฟล์ กรุณาลองใหม่", MsgBoxStyle.Critical)
                    Exit Sub
                End If
            End If
        Next
        'MsgBox("ตรวจสอบสำเร็จแล้ว", MsgBoxStyle.OkOnly)


        '    For Each dt As String In file_data
        'If dt = "" Then
        'Exit For
        '   End If
        '  If CalcMD5((path + "test.xml")) <> dt(1) Then
        '' MsgBox("error")
        '  End If
        '  MsgBox(dt)
        ' Next
        'MsgBox(keyvalue(2))
    End Sub
    Private Sub bw1_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted

        If e.Cancelled = True Then
            Me.Label2.Text = "Status : การทำงานถูกยกเลิกโดยผู้ใช้ !"
        ElseIf e.Error IsNot Nothing Then
            Me.Label2.Text = "Status : มีความผิดพลาด " & e.Error.Message

            'Download error
        ElseIf errorcheck = "1" Then
            Me.Label2.Text = "Status : มีความผิดพลาดขณะ Download files."
            MessageBox.Show("เกิดข้อผิดพลาดขณะ download file. Possibe causes:" & ControlChars.CrLf & _
                            "1) ไม่มีไฟล์ในเซิฟเวอร์" & ControlChars.CrLf & _
                            "2) เซิฟเวอร์ทำงานผิดพลาด(อาจล่มอยู่)", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)

            'No Error ?
        Else
            Me.Label2.Text = "Status : Ready!"
            Dim formshow As New showform(AddressOf showf)
            Me.Invoke(formshow)
        End If
        resetui()

    End Sub

    Private Sub about_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles about.Click
        Form3.Show()
    End Sub
    Private Sub resetui()

        startgame.Enabled = True
        'dlgame.Enabled = True
        about.Enabled = True
        Label4.Text = ""
        Label3.Text = ""
    End Sub

    Private Sub startoffline_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles startoffline.Click
        RunGame()
    End Sub

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        time = time + 1
    End Sub
    Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As FormClosingEventArgs) Handles Me.FormClosing
        If BackgroundWorker1.IsBusy = True Then
        Else
            BackgroundWorker1.CancelAsync()
        End If
        If RunGameWorker.IsBusy = True Then
            proc.Kill()
            RunGameWorker.CancelAsync()
        End If

        RichTextBox1.BringToFront()
        RichTextBox1.Visible = False
        e.Cancel = True
    End Sub
    Public Shared Sub Hide_Form1()
        'minimize the form
        Form1.Hide()
        Form1.NotifyIcon1.Visible = True
        Form1.WindowState = FormWindowState.Minimized
        Form1.NotifyIcon1.BalloonTipTitle = "Zone-Gamer Minecraft"
        Form1.NotifyIcon1.BalloonTipText = "Starting Game ..."
        Form1.NotifyIcon1.ShowBalloonTip(500)
    End Sub
    Private Sub NotifyIcon1_MouseDoubleClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles NotifyIcon1.MouseDoubleClick
        NotifyIcon1.Visible = False
        Me.Show()
        Me.WindowState = FormWindowState.Normal
    End Sub

    Public Sub p_OutputDataReceived(ByVal sender As Object, ByVal e As DataReceivedEventArgs)
        BeginInvoke(New GameLogSafe(AddressOf GameLog), e.Data)
    End Sub
    Private Sub GameLog(ByVal text As String)
        RichTextBox1.AppendText(vbLf)
        RichTextBox1.AppendText(" " & text)
        RichTextBox1.SelectionStart = RichTextBox1.Text.Length
        RichTextBox1.ScrollToCaret()

        ''Initializing LWJGL OpenAL

    End Sub

    Private Sub RunGame_DoWork(sender As Object, e As ComponentModel.DoWorkEventArgs) Handles RunGameWorker.DoWork
        If Not File.Exists(MyPath & "start.bat") Then
            File.Delete(MyPath & "start.bat")
            File.Create(MyPath & "start.bat").Dispose()
        End If
        Dim command = "-XX:HeapDumpPath=MojangTricksIntelDriversForPerformance_javaw.exe_minecraft.exe.heapdump -Xmx1G -XX:+UseConcMarkSweepGC -XX:+CMSIncrementalMode -XX:-UseAdaptiveSizePolicy -Xmn128M -Djava.library.path=C:\Users\Gamer\AppData\Roaming\.minecraft\versions\1.8-LiteLoader1.8-1.8-Forge11.14.1.1332\1.8-LiteLoader1.8-1.8-Forge11.14.1.1332-natives-475500669532385 -cp C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\com\mumfrey\liteloader\1.8\liteloader-1.8.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\net\minecraft\launchwrapper\1.11\launchwrapper-1.11.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\ow2\asm\asm-all\5.0.3\asm-all-5.0.3.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\net\minecraftforge\forge\1.8-11.14.1.1332\forge-1.8-11.14.1.1332.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\net\minecraft\launchwrapper\1.11\launchwrapper-1.11.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\ow2\asm\asm-all\5.0.3\asm-all-5.0.3.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\com\typesafe\akka\akka-actor_2.11\2.3.3\akka-actor_2.11-2.3.3.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\com\typesafe\config\1.2.1\config-1.2.1.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\scala-lang\scala-actors-migration_2.11\1.1.0\scala-actors-migration_2.11-1.1.0.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\scala-lang\scala-compiler\2.11.1\scala-compiler-2.11.1.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\scala-lang\plugins\scala-continuations-library_2.11\1.0.2\scala-continuations-library_2.11-1.0.2.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\scala-lang\plugins\scala-continuations-plugin_2.11.1\1.0.2\scala-continuations-plugin_2.11.1-1.0.2.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\scala-lang\scala-library\2.11.1\scala-library-2.11.1.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\scala-lang\scala-parser-combinators_2.11\1.0.1\scala-parser-combinators_2.11-1.0.1.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\scala-lang\scala-reflect\2.11.1\scala-reflect-2.11.1.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\scala-lang\scala-swing_2.11\1.0.1\scala-swing_2.11-1.0.1.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\scala-lang\scala-xml_2.11\1.0.2\scala-xml_2.11-1.0.2.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\lzma\lzma\0.0.1\lzma-0.0.1.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\net\sf\jopt-simple\jopt-simple\4.5\jopt-simple-4.5.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\java3d\vecmath\1.5.2\vecmath-1.5.2.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\net\sf\trove4j\trove4j\3.0.3\trove4j-3.0.3.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\com\ibm\icu\icu4j-core-mojang\51.2\icu4j-core-mojang-51.2.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\net\sf\jopt-simple\jopt-simple\4.6\jopt-simple-4.6.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\com\paulscode\codecjorbis\20101023\codecjorbis-20101023.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\com\paulscode\codecwav\20101023\codecwav-20101023.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\com\paulscode\libraryjavasound\20101123\libraryjavasound-20101123.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\com\paulscode\librarylwjglopenal\20100824\librarylwjglopenal-20100824.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\com\paulscode\soundsystem\20120107\soundsystem-20120107.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\io\netty\netty-all\4.0.15.Final\netty-all-4.0.15.Final.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\com\google\guava\guava\17.0\guava-17.0.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\apache\commons\commons-lang3\3.3.2\commons-lang3-3.3.2.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\commons-io\commons-io\2.4\commons-io-2.4.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\commons-codec\commons-codec\1.9\commons-codec-1.9.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\net\java\jinput\jinput\2.0.5\jinput-2.0.5.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\net\java\jutils\jutils\1.0.0\jutils-1.0.0.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\com\google\code\gson\gson\2.2.4\gson-2.2.4.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\com\mojang\authlib\1.5.17\authlib-1.5.17.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\com\mojang\realms\1.6.1\realms-1.6.1.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\apache\commons\commons-compress\1.8.1\commons-compress-1.8.1.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\apache\httpcomponents\httpclient\4.3.3\httpclient-4.3.3.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\commons-logging\commons-logging\1.1.3\commons-logging-1.1.3.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\apache\httpcomponents\httpcore\4.3.2\httpcore-4.3.2.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\apache\logging\log4j\log4j-api\2.0-beta9\log4j-api-2.0-beta9.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\apache\logging\log4j\log4j-core\2.0-beta9\log4j-core-2.0-beta9.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\lwjgl\lwjgl\lwjgl\2.9.1\lwjgl-2.9.1.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\lwjgl\lwjgl\lwjgl_util\2.9.1\lwjgl_util-2.9.1.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\tv\twitch\twitch\6.5\twitch-6.5.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\versions\1.8\1.8.jar net.minecraft.launchwrapper.Launch --tweakClass com.mumfrey.liteloader.launch.LiteLoaderTweaker --username " & Username.Text & " --version 1.8 --gameDir C:\Users\Gamer\AppData\Roaming\.minecraft --assetsDir C:\Users\Gamer\AppData\Roaming\.minecraft\assets --assetIndex 1.8 --accessToken myaccesstoken --userProperties {} --tweakClass net.minecraftforge.fml.common.launcher.FMLTweaker"
        procStartInfo.Arguments = command
        procStartInfo.FileName = "javaw"
        procStartInfo.RedirectStandardOutput = True
        procStartInfo.RedirectStandardError = False
        'procStartInfo.RedirectStandardInput = True
        procStartInfo.UseShellExecute = False
        procStartInfo.CreateNoWindow = True

        proc.StartInfo = procStartInfo
        AddHandler proc.OutputDataReceived, AddressOf p_OutputDataReceived
        AddHandler proc.Exited, AddressOf p_Exited

        proc.Start()
        ZGM.UnLockFile(MyPath & server_file_name)

        BeginInvoke(New GameLogSafe(AddressOf GameLog), "Start game with command :: " & command)
        proc.BeginOutputReadLine()

        proc.WaitForExit()
    End Sub
    Private Sub RunGame_Success(ByVal sender As Object, ByVal e As System.EventArgs) Handles RunGameWorker.RunWorkerCompleted

    End Sub
    Private Sub p_Exited(ByVal sender As Object, ByVal e As System.EventArgs)
        proc.CancelOutputRead()
        RichTextBox1.BringToFront()
        RichTextBox1.Visible = False
    End Sub

    Private Sub RunGame()
        If Username.TextLength < 3 Or Username.TextLength > 16 Then
            MsgBox("Username must be 3-16 charactor")
        Else
            If RunGameWorker.IsBusy <> True Then
                Label2.Text = "Status : Starting game..."
                RichTextBox1.BringToFront()
                RichTextBox1.Visible = True
                RunGameWorker.RunWorkerAsync()
            End If
        End If
    End Sub
    Private Sub Enter_Start(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Username.KeyDown
        If e.KeyCode = Keys.Enter Then
            RunGame()
        End If
    End Sub
End Class




