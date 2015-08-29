Imports System
Imports System.IO
Imports System.Net
Imports Ionic.Zip
Imports System.Threading
Imports System.Security.Principal
Imports System.Timers
Imports ZGMLaunch
Imports System.Text.RegularExpressions
Imports System.Drawing.Text
Imports Microsoft.Win32
Imports Newtonsoft.Json.Linq

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
    Dim server As String = "http://enjoyprice.in.th/mc/patch/" 'Old website :(
    Dim wc As WebClient = New WebClient
    Dim dl_status As Integer = 0
    Dim current_delay = 0
    Dim count_delay = 0
    Dim time As Double
    Dim log As String = ""
    Dim startGameVer As String = "180forge"

    Dim Library As ZGMLibrary.ZGMLibrary
    Dim Config As ZGMLibrary.ZGMConfig

    Dim procStartInfo As New System.Diagnostics.ProcessStartInfo()
    Dim proc As System.Diagnostics.Process = New Process()

    Dim t1 As New Threading.Thread(AddressOf RunGameCommand)

    Const DIR_SV As Integer = 0
    Const MCNAME As Integer = 1
    Const REMEMBER_MCNAME As Integer = 2
    Const DLSERVER As Integer = 3

    '3,0 = zgmversion ; 0,i = path ; 1,i = filename ; 2,i = md5
    Public Const XML_ZGMVER As Integer = 3
    Public Const XML_FILE As Integer = 1
    Public Const XML_MD5 As Integer = 2
    Public Const XML_ As Integer = 0

    Public Shared ProgramFilesPath As String = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) & "\"
    Public Shared appdata As String = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
    Public Shared MyPath As String = Application.StartupPath + "\"
    Public Shared server_file_name As String = "zgm"
    Public Shared UrlAuth As String = ""
    Public Shared gamePath = MyPath & server_file_name & "\"
    Public Shared gamePath_ = MyPath & server_file_name

    'Delegate Sub ChangeTextsSafe(ByVal length As Long, ByVal position As Integer, ByVal speed As Double, ByVal filename As String)
    Delegate Sub DownloadTextSafe(ByVal percentage As Double, ByVal speed As Double, ByVal bytesIn As Double, ByVal totalBytes As Double)
    Delegate Sub ChangeStatusSafe(ByVal text As String)
    Delegate Sub DownloadCompleteSafe(ByVal cancelled As Boolean)
    Delegate Sub showform()
    Delegate Sub SelectVersionSafe()
    Delegate Sub GameLogSafe(ByVal text As String)
    Delegate Sub GameExitedSafe()
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

        If current_delay < count_delay Then
            BeginInvoke(New DownloadTextSafe(AddressOf DownloadText), percentage, speed, bytesIn, totalBytes)
            current_delay = count_delay
        End If
        ' Thread.Sleep(50)
    End Sub

    Private Sub client_DownloadCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.AsyncCompletedEventArgs)
        dl_status = 1
        If e.Error IsNot Nothing Then
            'MsgBox(e.Error.Message)
            errorcheck = 1
            BackgroundWorker1.CancelAsync()
        End If
        BeginInvoke(New DownloadTextSafe(AddressOf DownloadText), 100, 0, 100, 100)
        BeginInvoke(New ChangeStatusSafe(AddressOf ChangeStatus), "Status : Checking file ...")
    End Sub

    Private Sub DownloadText(ByVal percentage As Double, ByVal speed As Double, ByVal bytesIn As Double, ByVal totalBytes As Double)
        If Int32.Parse(Math.Truncate(percentage).ToString()) >= 0 Then
            ProgressBar1.Value = Int32.Parse(Math.Truncate(percentage).ToString())
        End If


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
                            "2) เซิฟเวอร์ทำงานผิดพลาด(อาจล่มอยู่)" & ControlChars.CrLf & _
                            "3) File = " & filename & urlfile, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)

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
        time = 1.0
        Do

            If BackgroundWorker1.CancellationPending Then 'If user abort download
                Exit Do
            End If

            speedtimer.Start()

            Dim readBytes(4096) As Byte
            Dim bytesread As Integer = theResponse.GetResponseStream.Read(readBytes, 0, 4095)
            nRead += bytesread

            BeginInvoke(New DownloadTextSafe(AddressOf DownloadText), nRead / length * 100, nRead / (time * 102.4), nRead, length)
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
            'Thread.Sleep(50)
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
    End Sub
    Private Sub dl2(ByVal url, ByVal filename)
        dl_status = 0
        AddHandler wc.DownloadProgressChanged, AddressOf client_ProgressChanged
        AddHandler wc.DownloadFileCompleted, AddressOf client_DownloadCompleted
        time = 0
        While wc.IsBusy = True

        End While
        'wc.DownloadFileAsync(New Uri(url), MyPath + filename)
        wc.DownloadFileAsync(New Uri(url), MyPath + filename)
        While dl_status = 0
            Thread.Sleep(10)
        End While

    End Sub
    Private Sub unzip(ByVal filename)
        'Unzip
        Try
            Using zip As ZipFile = ZipFile.Read(MyPath + filename)
                zip.ExtractAll(MyPath, Ionic.Zip.ExtractExistingFileAction.OverwriteSilently)
            End Using
        Catch
            BeginInvoke(New GameExitedSafe(AddressOf GameExited))
        End Try
        If (filename = "zgmlaunch.zip") Then
            'INIWrite(path & "config.ini", "client", "versionlaunch", keyvaluesv(1))
            File.Delete(MyPath & "zgmlaunch.zip")
            MsgBox("พบโปรแกรมเวอร์ชั่นใหม่" & ControlChars.CrLf & "โปรแกรมจะทำการ Restart !")
            Process.Start("update.exe")
            End
        End If
    End Sub

    Private Sub closeme_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles closeme.Click
        GameExited()
        'MsgBox("Thanks for use my programe :D")
        Me.Close()
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Loadsetting()
        'ZGM.LockFile(MyPath & server_file_name)

    End Sub
    Private Sub valini(ByRef keyname, ByRef keyvalue)
        'keyname(1) = "DirServer"
        'keyname(2) = "title"
        'keyname(3) = "dlserver"
        'keyname(4) = "UrlAuth"
        'keyname(5) = "UrlNews"
        'keyname(6) = "OpenAuth"
        keyname(MCNAME) = "McName"
        keyname(REMEMBER_MCNAME) = "Remember"
        keyname(DLSERVER) = "DLserver"
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

        'server_file_name = keyvalue(1)
        'Me.Text = keyvalue(2) + " :: Version " + My.Settings.fullversion
        'server = keyvalue(3)
        'UrlAuth = keyvalue(4)
        'If keyvalue(5) = "" Then
        '    WebBrowser1.Navigate("http://mc.zone-gamer.th.ht/freelauncher")
        'Else
        '    WebBrowser1.Navigate(keyvalue(5))
        'End If
        If File.Exists(MyPath & "config.ini") <> True Then
            File.Create(MyPath & "config.ini").Close()
            INIWrite(MyPath & "config.ini", "client", "McName", "user")
            INIWrite(MyPath & "config.ini", "client", "Remember", "1")
            INIWrite(MyPath & "config.ini", "client", "DLserver", "http://sv1.enjoyprice.in.th/mc/patch/")
        End If

        valini(keyname, keyvalue)

        If keyvalue(MCNAME) = "" Then
            INIWrite(MyPath & "config.ini", "client", "McName", "user")
        End If
        If keyvalue(REMEMBER_MCNAME) = "" Then
            INIWrite(MyPath & "config.ini", "client", "Remember", "1")
        End If
        If keyvalue(DLSERVER) = "" Then
            INIWrite(MyPath & "config.ini", "client", "DLserver", "http://sv1.enjoyprice.in.th/mc/patch/")
        End If

        valini(keyname, keyvalue)

        If keyvalue(REMEMBER_MCNAME) Then
            remember.Checked = True
            Username.Text = keyvalue(MCNAME)
        End If

        server = keyvalue(DLSERVER)

        WebBrowser1.Navigate(server & "patchnews2.htm?ver=" & CLng(DateTime.UtcNow.Subtract(New DateTime(1970, 1, 1)).TotalMilliseconds))
        AddHandler WebBrowser1.DocumentCompleted, New WebBrowserDocumentCompletedEventHandler(AddressOf PageWaiter)

        Label3.Text = ""
        Label4.Text = ""
        Me.Text = "ZGMLaunch :: Version " + My.Settings.fullversion + " :: Build date " + ZGMLibrary.ZGMLibrary.RetrieveLinkerTimestamp(MyPath & "ZGM Launch for Minecraft.exe")
        Label1.Text = "© ZGMLauncher " + My.Settings.fullversion + " :: ZGMLibrary " + ZGM.GetVersion() + " Dev by Theballkyo"
    End Sub

    Private Sub startgame_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles startgame.Click
        SelectVersion.Show()
    End Sub

    Public Shared Sub Runsystem()
        Form1.startgame.Enabled = False

        Form1.Timer1.Start()
        Form1.Label2.Text = "Status : Checking internet connection"
        Try
            My.Computer.Network.Ping("www.enjoyprice.in.th")
            If Form1.BackgroundWorker1.IsBusy <> True Then
                Form1.Label2.Text = "Status : Downloading patchlist"
                Form1.BackgroundWorker1.RunWorkerAsync()
            End If
        Catch
            MsgBox("Can't connect to server, start in offline mode")
            Form1.RunGame()
        End Try

    End Sub

    Private Sub PageWaiter(ByVal sender As Object, ByVal e As WebBrowserDocumentCompletedEventArgs)
        If WebBrowser1.ReadyState = WebBrowserReadyState.Complete Then
            If Not WebBrowser1.IsOffline Then
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
        'dl2(server & "zgm_patch.xml", "patchlist.xml")
        'If errorcheck = "1" Then
        'MsgBox("มีข้อผิดพลาดขณะทำการตรวจสอบไฟล์ กรุณาลองใหม่", MsgBoxStyle.Critical)
        'Exit Sub
        'End If
        NewCheckFile()
    End Sub

    Public Sub NewCheckFile()
        BeginInvoke(New ChangeStatusSafe(AddressOf ChangeStatus), "Status : Checking file ...")
        dl2(server & "zgm_list.json", "patchlist.json")
        Dim json As String = File.ReadAllText(MyPath & "patchlist.json")
        Dim ser As JObject = JObject.Parse(json)
        Dim data As List(Of JToken) = ser.Children().ToList
        Dim output As String = ""
        For Each item As JProperty In data
            item.CreateReader()
            Select Case item.Name
                Case "version"
                    If My.Settings.version < Convert.ToInt16(item.Value) Then
                        dl2(server & "zgmlaunch.zip", "zgmlaunch.zip")
                        unzip("zgmlaunch.zip")
                    End If
                Case "files"
                    For Each file_ As JObject In item.Values
                        Dim name As String = file_("name") '/
                        Dim md5 As String = file_("md5")

                        'MsgBox(path_file)
                        Dim full_path_file_local As String = server_file_name + "\" + name.Replace("/", "\")
                        Dim full_path_file_dl As String = server & "game" & "/" & name

                        Dim p() As String = name.Split("/")
                        fn = p.GetValue(p.Length - 1)
                        Array.Resize(p, p.Length - 1)

                        Dim path_file As String = MyPath + server_file_name + "\" + Join(p, "\")

                        Path.Combine(full_path_file_local)

                        If File.Exists(full_path_file_local) Then
                            If ZGM.CalcMD5(full_path_file_local).ToLower <> md5.ToLower Then
                                dl2(full_path_file_dl, full_path_file_local)
                                Thread.Sleep(20)
                                If errorcheck = "1" Then
                                    MsgBox("มีข้อผิดพลาดขณะทำการตรวจสอบไฟล์ กรุณาลองใหม่", MsgBoxStyle.Critical)
                                    Exit Sub
                                End If
                            End If
                        Else
                            If path_file <> "" Then
                                If Not Directory.Exists(path_file) Then
                                    Directory.CreateDirectory(path_file)
                                End If
                            End If
                            dl2(full_path_file_dl, full_path_file_local)
                            Thread.Sleep(20)
                            If errorcheck = "1" Then
                                MsgBox("มีข้อผิดพลาดขณะทำการตรวจสอบไฟล์ กรุณาลองใหม่", MsgBoxStyle.Critical)
                                Exit Sub
                            End If
                        End If

                    Next
                Case "gameversion"
                    startGameVer = item.Value
            End Select
        Next

        ' Sync mods files
        Dim modsFiles = IO.Directory.GetFiles(server_file_name & "\mods", "*.*", SearchOption.AllDirectories)
        dl2(server & "modslist.json", "modslist.json")
        json = File.ReadAllText(MyPath & "modslist.json")
        ser = JObject.Parse(json)
        data = ser.Children().ToList
        Dim is_del = False

        For Each modsfile As String In modsFiles
            is_del = True
            For Each item As JProperty In data
                item.CreateReader()
                If item.Name = "files" Then
                    For Each file_ As JObject In item.Values
                        Dim name As String = file_("name")
                        Dim full_path_file As String = server_file_name + "\mods\" + name.Replace("/", "\")
                        ' MsgBox(modsfile & " : " & full_path_file)
                        If modsfile = full_path_file Then
                            is_del = False
                            Exit For
                        End If
                    Next
                End If
            Next
            If is_del Then
                File.Delete(modsfile)
            End If
        Next

    End Sub

    Private Sub bw1_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted

        If e.Cancelled = True Then
            Me.Label2.Text = "Status : การทำงานถูกยกเลิกโดยผู้ใช้ !"
        ElseIf e.Error IsNot Nothing Then
            Me.Label2.Text = "Status : มีความผิดพลาด " & e.Error.Message
            MsgBox(e.Error.Message, MsgBoxStyle.Critical)

            'Download error
        ElseIf errorcheck = "1" Then
            Me.Label2.Text = "Status : มีความผิดพลาดขณะ Download files."
            MessageBox.Show("เกิดข้อผิดพลาดขณะ download file. Possibe causes:" & ControlChars.CrLf & _
                            "1) ไม่มีไฟล์ในเซิฟเวอร์" & ControlChars.CrLf & _
                            "2) เซิฟเวอร์ทำงานผิดพลาด(อาจล่มอยู่)", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)

            'No Error ?
        Else
            Me.Label2.Text = "Status : Ready!"
            RunGame()
            'Dim formshow As New showform(AddressOf showf)
            'Me.Invoke(formshow)
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
        If Not Directory.Exists(gamePath_) Then
            Directory.CreateDirectory(gamePath_)
        End If
        Try
            Process.Start(gamePath_)
        Catch ex As Exception
            MsgBox(ex.Message())
        End Try

    End Sub

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        time = time + 1
    End Sub
    Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As FormClosingEventArgs) Handles Me.FormClosing
        GameExited()

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
    Private Sub GameLog(ByVal text As String)
        'RichTextBox1.AppendText(vbLf)
        RichTextBox1.AppendText(" " & text)
        RichTextBox1.SelectionStart = RichTextBox1.Text.Length
        RichTextBox1.ScrollToCaret()
        'If RichTextBox1.TextLength > 10000 Then
        '    RichTextBox1.Clear()
        'End If
        ''Initializing LWJGL OpenAL

    End Sub
    Private Sub ProcDataReceived(ByVal sender As System.Object, ByVal d As DataReceivedEventArgs)
        Try
            If Not d.Data.Contains("Unable to play unknown soundEvent: minecraft:none") Then
                log &= vbLf & d.Data
            End If
        Catch

        End Try

    End Sub
    Private Sub GameExited()
        MsgBox("มีความผิดพลาด กรุณารันโปรแกรมใหม่อีกครั้ง")
        Try
            If t1.IsAlive Or t1.IsThreadPoolThread Then
                If Not proc.HasExited Then
                    proc.Kill()
                End If
                t1.Abort()
            End If
        Catch

        End Try
    End Sub

    Private Sub RunGame()
        If Username.TextLength < 3 Or Username.TextLength > 16 Then
            MsgBox("Username must be 3-16 character")
            Label2.Text = "Status : Username not correct."
            resetui()
        Else
            If Not Directory.Exists(gamePath & "assets") Then
                Label2.Text = "Status : Copying Assets..."
                If Directory.Exists(appdata & "\.minecraft\assets") Then
                    My.Computer.FileSystem.CopyDirectory(appdata & "\.minecraft\assets", gamePath & "assets", True)
                End If
            End If
            If Not t1.IsAlive Then
                Label2.Text = "Status : Starting game..."
                RichTextBox1.BringToFront()
                RichTextBox1.Visible = True
                'Save McName
                If keyvalue(REMEMBER_MCNAME) = 1 Then
                    INIWrite(MyPath & "config.ini", "client", "McName", Username.Text)
                End If
                t1 = New Threading.Thread(AddressOf RunGameCommand)
                t1.Start()
            End If
        End If
    End Sub

    Private Sub RunGameCommand()
        Dim javaPath As String = ""
        'MsgBox(javaPath & "\bin\javaw.exe")
        'If javaPath = Nothing Or Game.getVerJava() = Nothing Then
        'MsgBox("กรุณาลง Java เวอร์ชั่น 1.8 ขึ้นไป")
        'BeginInvoke(New GameExitedSafe(AddressOf GameExited))
        'End If
        If Game.checkVerJava("1.8") = False Then
            BeginInvoke(New GameLogSafe(AddressOf GameLog), "Download Java... Please wait")
            If Environment.Is64BitOperatingSystem Then
                If Not File.Exists(MyPath & "jre1.8.zip") Then
                    dl2(server + "jre1.8_x64.zip", "jre1.8.zip")
                End If
                javaPath = MyPath & "jre1.8_x64"
            Else
                If Not File.Exists(MyPath & "jre1.8.zip") Then
                    dl2(server + "jre1.8_x86.zip", "jre1.8.zip")
                End If
                javaPath = MyPath & "jre1.8_x86"
            End If

            If errorcheck = "1" Then
                BeginInvoke(New GameExitedSafe(AddressOf GameExited))
            End If

        Else
                javaPath = Game.getJavaPath("1.8")
        End If
        MsgBox("test")
        Try
            procStartInfo.Arguments = Game.getCommand(gamePath, gamePath_, Username.Text, SelectVersion.selectVersion)
            procStartInfo.FileName = javaPath & "\bin\javaw.exe"
            procStartInfo.RedirectStandardOutput = True
            procStartInfo.UseShellExecute = False
            procStartInfo.CreateNoWindow = True
            proc = New Process()
            proc.StartInfo = procStartInfo
            AddHandler proc.OutputDataReceived, AddressOf ProcDataReceived
            AddHandler proc.Exited, AddressOf GameExited
            proc.Start()
            proc.BeginOutputReadLine()
            BeginInvoke(New GameLogSafe(AddressOf GameLog), "Start game...")
            BeginInvoke(New GameLogSafe(AddressOf GameLog), "Javapath = " & javaPath & "\bin\javaw.exe")
            BeginInvoke(New GameLogSafe(AddressOf GameLog), "Java Bit = " & Game.getJavaBit("1.8"))
            BeginInvoke(New GameLogSafe(AddressOf GameLog), Game.getCommand(gamePath, gamePath_, Username.Text, SelectVersion.selectVersion))
            While Not proc.HasExited
                Thread.Sleep(200)
                If log <> "" Then
                    BeginInvoke(New GameLogSafe(AddressOf GameLog), log)
                End If
                log = ""
            End While
        Catch ex As Exception
            MsgBox(ex)
        End Try

        'Exited Game and Stop Threading
        BeginInvoke(New GameExitedSafe(AddressOf GameExited))
        'Stop Thread
        't1.Abort()

    End Sub
    Private Sub Enter_Start(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Username.KeyDown
        If e.KeyCode = Keys.Enter Then
            SelectVersion.Show()
        End If
    End Sub

    Private Sub remember_CheckedChanged(sender As Object, e As EventArgs) Handles remember.CheckedChanged
        INIWrite(MyPath & "config.ini", "client", "Remember", 1)
        If Not remember.Checked Then
            INIWrite(MyPath & "config.ini", "client", "Remember", 0)
        End If

    End Sub

    Private Sub delay_Tick(sender As Object, e As EventArgs) Handles delay.Tick
        count_delay += 1
    End Sub
End Class