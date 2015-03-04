Imports System.IO
Imports System.Threading
Imports ZGMLaunch

Public Class Form2
    Dim procStartInfo As New System.Diagnostics.ProcessStartInfo()
    Dim proc As System.Diagnostics.Process
    Dim path As String = Form1.MyPath
    Dim server_file_name As String = Form1.server_file_name
    Dim mode As String
    Dim bit As String
    Dim status As Int32
    Delegate Sub ChangeStatusSafe(ByVal text As String)
    Public Sub New(ByVal m As String)
        InitializeComponent()
        mode = m
    End Sub
    'Private Sub Form2_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
    '    Dim startIndex As Int32 = 85
    '    Dim endIndex As Int32 = startIndex + Len("PRESS HERE") - 3
    '    'RichTextBox1.Select(startIndex, endIndex)
    '    'RichTextBox1.SelectionColor = Color.Blue
    '    If (File.Exists(path & "start.bat")) Then
    '        File.Delete(path & "start.bat")
    '    End If
    '    If mode = "offline" Then
    '        TextBox1.Enabled = True
    '        'TextBox2.Enabled = False
    '        'TextBox2.Visible = False
    '        Label2.Visible = False

    '    End If
    '    Form1.WindowState = FormWindowState.Minimized
    '    ZGM.LockFile(path & server_file_name)
    'End Sub

    'Private Sub RichTextBox1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
    '    'If (RichTextBox1.SelectionColor = Color.Blue) Then
    '    'MessageBox.Show("Press here!")
    '    'Process.Start("http://forums.zone-gamer.th.ht/")
    '    ' End If
    'End Sub

    'Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
    '    MsgBox(ZGMLibrary.ZGMLibrary.GetJavaVersion())
    '    status = 0
    '    Button1.Enabled = False
    '    If (mode = "online") Then
    '        If (TextBox1.TextLength >= 3 And TextBox1.TextLength <= 15) Then
    '            If (TextBox2.TextLength > 0) Then
    '                If BackgroundWorker1.IsBusy <> True Then
    '                    BackgroundWorker1.RunWorkerAsync()
    '                End If
    '            Else
    '                MsgBox("กรุณากรอก Password")
    '                Button1.Enabled = True
    '            End If
    '        Else
    '            MsgBox("กรุณากรอก Username มีความยาว 3-15 ตัวอักษร")
    '            Button1.Enabled = True
    '        End If

    '    Else
    '        If (TextBox1.TextLength >= 3 And TextBox1.TextLength <= 15) Then
    '            If BackgroundWorker1.IsBusy <> True Then
    '                Me.TopMost = False
    '                Form1.Hide_Form1()
    '                BackgroundWorker1.RunWorkerAsync()
    '            End If
    '        Else
    '            MsgBox("กรุณากรอก Username มีความยาว 3-15 ตัวอักษร")
    '            Button1.Enabled = True
    '        End If
    '    End If
    'End Sub
    'Private Sub BackgroundWorker1_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
    '    startgame(0)

    'End Sub
    'Private Sub bw1_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted
    '    If (status = 1) Then
    '        'hide form to system tray
    '        'Form1.Hide_Form1()
    '        'Me.Close()
    '        'Delete start.bat
    '        Thread.Sleep(3500)
    '        File.Delete(path & "start.bat")
    '    ElseIf (status = 0) Then
    '        MsgBox("Username หรือ Password ผิดพลาด", MsgBoxStyle.Critical, "error")
    '        Button1.Enabled = True
    '    ElseIf (status = 2) Then
    '        MsgBox("การเชื่อมต่อกับเซิฟเวอร์ผิดพลาด", MsgBoxStyle.Critical, "error")
    '        Button1.Enabled = True
    '    End If
    'End Sub
    'Private Sub startgame(ByVal keycode As String)
    '    Dim username As String = TextBox1.Text

    '    'Dim javapath As String = ZGM.FindJava()
    '    'If (javapath = "0") Then
    '    ' MsgBox("ไม่พบ Java ในเครื่องคุณ กรุณาติดตั้ง Java7 ก่อนใช้งาน")
    '    'ElseIf (javapath = "java6") Then
    '    'MsgBox("ตรวจสอบพบ Java6 กรุณาอัพเดทเป็น Java7 ก่อนใช้งาน")
    '    'End If

    '    If Not File.Exists(path & "start.bat") Then
    '        File.Delete(path & "start.bat")
    '        File.Create(path & "start.bat").Dispose()
    '    End If
    '    Dim command = "-XX:HeapDumpPath=MojangTricksIntelDriversForPerformance_javaw.exe_minecraft.exe.heapdump -Xmx1G -XX:+UseConcMarkSweepGC -XX:+CMSIncrementalMode -XX:-UseAdaptiveSizePolicy -Xmn128M -Djava.library.path=C:\Users\Gamer\AppData\Roaming\.minecraft\versions\1.8-LiteLoader1.8-1.8-Forge11.14.1.1332\1.8-LiteLoader1.8-1.8-Forge11.14.1.1332-natives-475500669532385 -cp C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\com\mumfrey\liteloader\1.8\liteloader-1.8.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\net\minecraft\launchwrapper\1.11\launchwrapper-1.11.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\ow2\asm\asm-all\5.0.3\asm-all-5.0.3.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\net\minecraftforge\forge\1.8-11.14.1.1332\forge-1.8-11.14.1.1332.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\net\minecraft\launchwrapper\1.11\launchwrapper-1.11.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\ow2\asm\asm-all\5.0.3\asm-all-5.0.3.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\com\typesafe\akka\akka-actor_2.11\2.3.3\akka-actor_2.11-2.3.3.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\com\typesafe\config\1.2.1\config-1.2.1.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\scala-lang\scala-actors-migration_2.11\1.1.0\scala-actors-migration_2.11-1.1.0.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\scala-lang\scala-compiler\2.11.1\scala-compiler-2.11.1.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\scala-lang\plugins\scala-continuations-library_2.11\1.0.2\scala-continuations-library_2.11-1.0.2.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\scala-lang\plugins\scala-continuations-plugin_2.11.1\1.0.2\scala-continuations-plugin_2.11.1-1.0.2.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\scala-lang\scala-library\2.11.1\scala-library-2.11.1.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\scala-lang\scala-parser-combinators_2.11\1.0.1\scala-parser-combinators_2.11-1.0.1.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\scala-lang\scala-reflect\2.11.1\scala-reflect-2.11.1.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\scala-lang\scala-swing_2.11\1.0.1\scala-swing_2.11-1.0.1.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\scala-lang\scala-xml_2.11\1.0.2\scala-xml_2.11-1.0.2.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\lzma\lzma\0.0.1\lzma-0.0.1.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\net\sf\jopt-simple\jopt-simple\4.5\jopt-simple-4.5.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\java3d\vecmath\1.5.2\vecmath-1.5.2.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\net\sf\trove4j\trove4j\3.0.3\trove4j-3.0.3.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\com\ibm\icu\icu4j-core-mojang\51.2\icu4j-core-mojang-51.2.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\net\sf\jopt-simple\jopt-simple\4.6\jopt-simple-4.6.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\com\paulscode\codecjorbis\20101023\codecjorbis-20101023.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\com\paulscode\codecwav\20101023\codecwav-20101023.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\com\paulscode\libraryjavasound\20101123\libraryjavasound-20101123.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\com\paulscode\librarylwjglopenal\20100824\librarylwjglopenal-20100824.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\com\paulscode\soundsystem\20120107\soundsystem-20120107.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\io\netty\netty-all\4.0.15.Final\netty-all-4.0.15.Final.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\com\google\guava\guava\17.0\guava-17.0.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\apache\commons\commons-lang3\3.3.2\commons-lang3-3.3.2.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\commons-io\commons-io\2.4\commons-io-2.4.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\commons-codec\commons-codec\1.9\commons-codec-1.9.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\net\java\jinput\jinput\2.0.5\jinput-2.0.5.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\net\java\jutils\jutils\1.0.0\jutils-1.0.0.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\com\google\code\gson\gson\2.2.4\gson-2.2.4.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\com\mojang\authlib\1.5.17\authlib-1.5.17.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\com\mojang\realms\1.6.1\realms-1.6.1.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\apache\commons\commons-compress\1.8.1\commons-compress-1.8.1.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\apache\httpcomponents\httpclient\4.3.3\httpclient-4.3.3.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\commons-logging\commons-logging\1.1.3\commons-logging-1.1.3.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\apache\httpcomponents\httpcore\4.3.2\httpcore-4.3.2.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\apache\logging\log4j\log4j-api\2.0-beta9\log4j-api-2.0-beta9.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\apache\logging\log4j\log4j-core\2.0-beta9\log4j-core-2.0-beta9.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\lwjgl\lwjgl\lwjgl\2.9.1\lwjgl-2.9.1.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\lwjgl\lwjgl\lwjgl_util\2.9.1\lwjgl_util-2.9.1.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\tv\twitch\twitch\6.5\twitch-6.5.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\versions\1.8\1.8.jar net.minecraft.launchwrapper.Launch --tweakClass com.mumfrey.liteloader.launch.LiteLoaderTweaker --username " & TextBox1.Text & " --version 1.8 --gameDir C:\Users\Gamer\AppData\Roaming\.minecraft --assetsDir C:\Users\Gamer\AppData\Roaming\.minecraft\assets --assetIndex 1.8 --accessToken myaccesstoken --userProperties {} --tweakClass net.minecraftforge.fml.common.launcher.FMLTweaker"
    '    'Dim procStartInfo As New System.Diagnostics.ProcessStartInfo("java", command)
    '    procStartInfo.Arguments = command
    '    procStartInfo.FileName = "javaw"
    '    procStartInfo.RedirectStandardOutput = True
    '    procStartInfo.RedirectStandardError = False
    '    procStartInfo.RedirectStandardInput = True
    '    procStartInfo.UseShellExecute = False
    '    procStartInfo.CreateNoWindow = True
    '    proc = New Process()
    '    proc.StartInfo = procStartInfo
    '    AddHandler proc.OutputDataReceived, AddressOf p_OutputDataReceived

    '    'AddHandler proc.ErrorDataReceived, AddressOf p_OutputDataReceived
    '    'proc.Start()

    '    'Dim swriter As StreamWriter
    '    'swriter = File.AppendText(path & "start.bat")
    '    ''swriter.WriteLine("@echo off")
    '    'swriter.WriteLine("set APPDATA=" & path & server_file_name)
    '    'swriter.WriteLine("cd %APPDATA%")
    '    'swriter.WriteLine("java -XX:HeapDumpPath=MojangTricksIntelDriversForPerformance_javaw.exe_minecraft.exe.heapdump -Xmx1G -XX:+UseConcMarkSweepGC -XX:+CMSIncrementalMode -XX:-UseAdaptiveSizePolicy -Xmn128M -Djava.library.path=C:\Users\Gamer\AppData\Roaming\.minecraft\versions\1.8-LiteLoader1.8-1.8-Forge11.14.1.1332\1.8-LiteLoader1.8-1.8-Forge11.14.1.1332-natives-475500669532385 -cp C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\com\mumfrey\liteloader\1.8\liteloader-1.8.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\net\minecraft\launchwrapper\1.11\launchwrapper-1.11.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\ow2\asm\asm-all\5.0.3\asm-all-5.0.3.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\net\minecraftforge\forge\1.8-11.14.1.1332\forge-1.8-11.14.1.1332.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\net\minecraft\launchwrapper\1.11\launchwrapper-1.11.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\ow2\asm\asm-all\5.0.3\asm-all-5.0.3.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\com\typesafe\akka\akka-actor_2.11\2.3.3\akka-actor_2.11-2.3.3.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\com\typesafe\config\1.2.1\config-1.2.1.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\scala-lang\scala-actors-migration_2.11\1.1.0\scala-actors-migration_2.11-1.1.0.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\scala-lang\scala-compiler\2.11.1\scala-compiler-2.11.1.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\scala-lang\plugins\scala-continuations-library_2.11\1.0.2\scala-continuations-library_2.11-1.0.2.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\scala-lang\plugins\scala-continuations-plugin_2.11.1\1.0.2\scala-continuations-plugin_2.11.1-1.0.2.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\scala-lang\scala-library\2.11.1\scala-library-2.11.1.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\scala-lang\scala-parser-combinators_2.11\1.0.1\scala-parser-combinators_2.11-1.0.1.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\scala-lang\scala-reflect\2.11.1\scala-reflect-2.11.1.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\scala-lang\scala-swing_2.11\1.0.1\scala-swing_2.11-1.0.1.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\scala-lang\scala-xml_2.11\1.0.2\scala-xml_2.11-1.0.2.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\lzma\lzma\0.0.1\lzma-0.0.1.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\net\sf\jopt-simple\jopt-simple\4.5\jopt-simple-4.5.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\java3d\vecmath\1.5.2\vecmath-1.5.2.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\net\sf\trove4j\trove4j\3.0.3\trove4j-3.0.3.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\com\ibm\icu\icu4j-core-mojang\51.2\icu4j-core-mojang-51.2.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\net\sf\jopt-simple\jopt-simple\4.6\jopt-simple-4.6.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\com\paulscode\codecjorbis\20101023\codecjorbis-20101023.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\com\paulscode\codecwav\20101023\codecwav-20101023.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\com\paulscode\libraryjavasound\20101123\libraryjavasound-20101123.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\com\paulscode\librarylwjglopenal\20100824\librarylwjglopenal-20100824.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\com\paulscode\soundsystem\20120107\soundsystem-20120107.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\io\netty\netty-all\4.0.15.Final\netty-all-4.0.15.Final.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\com\google\guava\guava\17.0\guava-17.0.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\apache\commons\commons-lang3\3.3.2\commons-lang3-3.3.2.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\commons-io\commons-io\2.4\commons-io-2.4.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\commons-codec\commons-codec\1.9\commons-codec-1.9.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\net\java\jinput\jinput\2.0.5\jinput-2.0.5.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\net\java\jutils\jutils\1.0.0\jutils-1.0.0.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\com\google\code\gson\gson\2.2.4\gson-2.2.4.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\com\mojang\authlib\1.5.17\authlib-1.5.17.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\com\mojang\realms\1.6.1\realms-1.6.1.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\apache\commons\commons-compress\1.8.1\commons-compress-1.8.1.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\apache\httpcomponents\httpclient\4.3.3\httpclient-4.3.3.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\commons-logging\commons-logging\1.1.3\commons-logging-1.1.3.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\apache\httpcomponents\httpcore\4.3.2\httpcore-4.3.2.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\apache\logging\log4j\log4j-api\2.0-beta9\log4j-api-2.0-beta9.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\apache\logging\log4j\log4j-core\2.0-beta9\log4j-core-2.0-beta9.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\lwjgl\lwjgl\lwjgl\2.9.1\lwjgl-2.9.1.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\lwjgl\lwjgl\lwjgl_util\2.9.1\lwjgl_util-2.9.1.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\tv\twitch\twitch\6.5\twitch-6.5.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\versions\1.8\1.8.jar net.minecraft.launchwrapper.Launch --tweakClass com.mumfrey.liteloader.launch.LiteLoaderTweaker --username " & TextBox1.Text & " --version 1.8 --gameDir C:\Users\Gamer\AppData\Roaming\.minecraft --assetsDir C:\Users\Gamer\AppData\Roaming\.minecraft\assets --accessToken myaccesstoken --userProperties {} --tweakClass net.minecraftforge.fml.common.launcher.FMLTweaker")
    '    'swriter.WriteLine("pause")
    '    'swriter.Close()

    '    'Dim p As New ProcessStartInfo
    '    'p.FileName = path & "start.bat"
    '    'p.UseShellExecute = True
    '    ZGM.UnLockFile(path & server_file_name)
    '    'Process.Start(p)
    '    proc.Start()

    '    BeginInvoke(New ChangeStatusSafe(AddressOf ChangeStatus), "Start game with command :: " & command)
    '    proc.BeginOutputReadLine()
    '    ' proc.BeginErrorReadLine()
    '    'While proc.StandardOutput.Peek() > -1
    '    '    BeginInvoke(New ChangeStatusSafe(AddressOf ChangeStatus), proc.StandardOutput.ReadLine())
    '    '    proc.StandardOutput.Read()
    '    'End While

    '    'MsgBox("SSs")
    '    proc.WaitForExit()
    '    status = 1
    'End Sub
    'Public Sub p_OutputDataReceived(ByVal sender As Object, ByVal e As DataReceivedEventArgs)
    '    BeginInvoke(New ChangeStatusSafe(AddressOf ChangeStatus), e.Data)
    'End Sub
    'Private Sub ChangeStatus(ByVal text As String)
    '    RichTextBox1.AppendText(vbLf)
    '    RichTextBox1.AppendText(" " & text)
    '    RichTextBox1.SelectionStart = RichTextBox1.Text.Length
    '    RichTextBox1.ScrollToCaret()
    'End Sub
End Class
