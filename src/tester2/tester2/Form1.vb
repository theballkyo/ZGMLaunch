Imports System.Threading
Imports Microsoft.Win32
Imports System.Text.RegularExpressions

Public Class Form1
    Dim procStartInfo As New System.Diagnostics.ProcessStartInfo()
    Dim proc As System.Diagnostics.Process = New Process()
    Dim t1 As New Threading.Thread(AddressOf run)
    Delegate Sub ChangeTextSafe(ByVal text As String)
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        'Dim procStartInfo As New System.Diagnostics.ProcessStartInfo()
        'Dim proc As System.Diagnostics.Process = New Process()
        'procStartInfo.Arguments = "-version "
        'procStartInfo.FileName = "java"
        'procStartInfo.RedirectStandardOutput = True
        'procStartInfo.RedirectStandardError = True
        'procStartInfo.UseShellExecute = False
        'procStartInfo.CreateNoWindow = True

        'proc = New Process()
        'proc.StartInfo = procStartInfo

        'Try
        '    proc.Start()
        '    t1 = New Threading.Thread(AddressOf run)
        '    t1.Start()
        'Catch ex As Exception
        '    MsgBox(ex.Message())
        'End Try
        ' MsgBox(Registry.LocalMachine.OpenSubKey("SOFTWARE\JavaSoft\Java Runtime Environment\1.8").GetValue("JavaHome"))
        'Dim output As String = proc.StandardError.ReadToEnd()
        Dim ver32 As String = ""
        Dim ver64 As String = ""
        Try
            ver32 = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, RegistryView.Registry32). _
                    OpenSubKey("SOFTWARE\JavaSoft\Java Runtime Environment").GetValue("CurrentVersion")
        Catch ex As Exception

        End Try
        Try
            ver64 = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, RegistryView.Registry64). _
                OpenSubKey("SOFTWARE\JavaSoft\Java Runtime Environment").GetValue("CurrentVersion")
        Catch ex As Exception

        End Try
        MsgBox(ver32 & " : " & ver64)
        'MsgBox(output)
    End Sub

    Private Sub run()
        Dim command = "-XX:HeapDumpPath=MojangTricksIntelDriversForPerformance_javaw.exe_minecraft.exe.heapdump -Xmx1G -XX:+UseConcMarkSweepGC -XX:+CMSIncrementalMode -XX:-UseAdaptiveSizePolicy -Xmn128M -Djava.library.path=C:\Users\Gamer\AppData\Roaming\.minecraft\versions\1.8-LiteLoader1.8-1.8-Forge11.14.1.1332\1.8-LiteLoader1.8-1.8-Forge11.14.1.1332-natives-475500669532385 -cp C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\com\mumfrey\liteloader\1.8\liteloader-1.8.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\net\minecraft\launchwrapper\1.11\launchwrapper-1.11.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\ow2\asm\asm-all\5.0.3\asm-all-5.0.3.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\net\minecraftforge\forge\1.8-11.14.1.1332\forge-1.8-11.14.1.1332.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\net\minecraft\launchwrapper\1.11\launchwrapper-1.11.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\ow2\asm\asm-all\5.0.3\asm-all-5.0.3.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\com\typesafe\akka\akka-actor_2.11\2.3.3\akka-actor_2.11-2.3.3.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\com\typesafe\config\1.2.1\config-1.2.1.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\scala-lang\scala-actors-migration_2.11\1.1.0\scala-actors-migration_2.11-1.1.0.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\scala-lang\scala-compiler\2.11.1\scala-compiler-2.11.1.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\scala-lang\plugins\scala-continuations-library_2.11\1.0.2\scala-continuations-library_2.11-1.0.2.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\scala-lang\plugins\scala-continuations-plugin_2.11.1\1.0.2\scala-continuations-plugin_2.11.1-1.0.2.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\scala-lang\scala-library\2.11.1\scala-library-2.11.1.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\scala-lang\scala-parser-combinators_2.11\1.0.1\scala-parser-combinators_2.11-1.0.1.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\scala-lang\scala-reflect\2.11.1\scala-reflect-2.11.1.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\scala-lang\scala-swing_2.11\1.0.1\scala-swing_2.11-1.0.1.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\scala-lang\scala-xml_2.11\1.0.2\scala-xml_2.11-1.0.2.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\lzma\lzma\0.0.1\lzma-0.0.1.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\net\sf\jopt-simple\jopt-simple\4.5\jopt-simple-4.5.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\java3d\vecmath\1.5.2\vecmath-1.5.2.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\net\sf\trove4j\trove4j\3.0.3\trove4j-3.0.3.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\com\ibm\icu\icu4j-core-mojang\51.2\icu4j-core-mojang-51.2.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\net\sf\jopt-simple\jopt-simple\4.6\jopt-simple-4.6.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\com\paulscode\codecjorbis\20101023\codecjorbis-20101023.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\com\paulscode\codecwav\20101023\codecwav-20101023.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\com\paulscode\libraryjavasound\20101123\libraryjavasound-20101123.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\com\paulscode\librarylwjglopenal\20100824\librarylwjglopenal-20100824.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\com\paulscode\soundsystem\20120107\soundsystem-20120107.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\io\netty\netty-all\4.0.15.Final\netty-all-4.0.15.Final.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\com\google\guava\guava\17.0\guava-17.0.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\apache\commons\commons-lang3\3.3.2\commons-lang3-3.3.2.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\commons-io\commons-io\2.4\commons-io-2.4.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\commons-codec\commons-codec\1.9\commons-codec-1.9.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\net\java\jinput\jinput\2.0.5\jinput-2.0.5.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\net\java\jutils\jutils\1.0.0\jutils-1.0.0.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\com\google\code\gson\gson\2.2.4\gson-2.2.4.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\com\mojang\authlib\1.5.17\authlib-1.5.17.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\com\mojang\realms\1.6.1\realms-1.6.1.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\apache\commons\commons-compress\1.8.1\commons-compress-1.8.1.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\apache\httpcomponents\httpclient\4.3.3\httpclient-4.3.3.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\commons-logging\commons-logging\1.1.3\commons-logging-1.1.3.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\apache\httpcomponents\httpcore\4.3.2\httpcore-4.3.2.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\apache\logging\log4j\log4j-api\2.0-beta9\log4j-api-2.0-beta9.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\apache\logging\log4j\log4j-core\2.0-beta9\log4j-core-2.0-beta9.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\lwjgl\lwjgl\lwjgl\2.9.1\lwjgl-2.9.1.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\org\lwjgl\lwjgl\lwjgl_util\2.9.1\lwjgl_util-2.9.1.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\libraries\tv\twitch\twitch\6.5\twitch-6.5.jar;C:\Users\Gamer\AppData\Roaming\.minecraft\versions\1.8\1.8.jar net.minecraft.launchwrapper.Launch --tweakClass com.mumfrey.liteloader.launch.LiteLoaderTweaker --username " & Username.Text & " --version 1.8 --gameDir C:\Users\Gamer\AppData\Roaming\.minecraft --assetsDir C:\Users\Gamer\AppData\Roaming\.minecraft\assets --assetIndex 1.8 --accessToken myaccesstoken --userProperties {} --tweakClass net.minecraftforge.fml.common.launcher.FMLTweaker"
        procStartInfo.Arguments = "-version"
        procStartInfo.FileName = "java"
        procStartInfo.RedirectStandardOutput = True
        procStartInfo.RedirectStandardError = True
        'procStartInfo.RedirectStandardInput = True
        procStartInfo.UseShellExecute = False
        procStartInfo.CreateNoWindow = True

        proc = New Process()
        proc.StartInfo = procStartInfo
        'AddHandler proc.OutputDataReceived, AddressOf p_OutputDataReceived
        'AddHandler proc.Exited, AddressOf p_Exited
        proc.Start()
        While proc.StandardError.Peek() > -1
            Dim t = proc.StandardError.ReadLine()
            BeginInvoke(New ChangeTextSafe(AddressOf ChangeText), t)
            proc.StandardError.Read()
        End While

    End Sub

    Private Sub ChangeText(ByVal text As String)
        RichTextBox1.AppendText(text)
    End Sub
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If t1.IsAlive Then
            If Not proc.HasExited Then
                proc.Kill()
            End If
            t1.Abort()
            MsgBox("TEST")
        End If
    End Sub
End Class
