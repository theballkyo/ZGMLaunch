Imports System.Text.RegularExpressions
Imports Microsoft.Win32
Imports System.Net

Module Game
    Dim library_list() As String = {
            "libraries\com\google\code\gson\gson\2.2.4\gson-2.2.4.jar",
            "libraries\com\google\guava\guava\17.0\guava-17.0.jar",
            "libraries\com\ibm\icu\icu4j-core-mojang\51.2\icu4j-core-mojang-51.2.jar",
            "libraries\com\mojang\authlib\1.5.17\authlib-1.5.17.jar",
            "libraries\com\mojang\realms\1.6.1\realms-1.6.1.jar",
            "libraries\com\mumfrey\liteloader\1.8\liteloader-1.8.jar",
            "libraries\com\paulscode\codecjorbis\20101023\codecjorbis-20101023.jar",
            "libraries\com\paulscode\codecwav\20101023\codecwav-20101023.jar",
            "libraries\com\paulscode\libraryjavasound\20101123\libraryjavasound-20101123.jar",
            "libraries\com\paulscode\librarylwjglopenal\20100824\librarylwjglopenal-20100824.jar",
            "libraries\com\paulscode\soundsystem\20120107\soundsystem-20120107.jar",
            "libraries\com\typesafe\akka\akka-actor_2.11\2.3.3\akka-actor_2.11-2.3.3.jar",
            "libraries\com\typesafe\config\1.2.1\config-1.2.1.jar",
            "libraries\commons-codec\commons-codec\1.9\commons-codec-1.9.jar",
            "libraries\commons-io\commons-io\2.4\commons-io-2.4.jar",
            "libraries\commons-logging\commons-logging\1.1.3\commons-logging-1.1.3.jar",
            "libraries\io\netty\netty-all\4.0.15.Final\netty-all-4.0.15.Final.jar",
            "libraries\java3d\vecmath\1.5.2\vecmath-1.5.2.jar",
            "libraries\lzma\lzma\0.0.1\lzma-0.0.1.jar",
            "libraries\net\java\jinput\jinput\2.0.5\jinput-2.0.5.jar",
            "libraries\net\java\jutils\jutils\1.0.0\jutils-1.0.0.jar",
            "libraries\net\minecraft\launchwrapper\1.11\launchwrapper-1.11.jar",
            "libraries\net\minecraft\launchwrapper\1.11\launchwrapper-1.11.jar",
            "libraries\net\minecraftforge\forge\forge.jar",
            "libraries\net\sf\jopt-simple\jopt-simple\4.5\jopt-simple-4.5.jar",
            "libraries\net\sf\jopt-simple\jopt-simple\4.6\jopt-simple-4.6.jar",
            "libraries\net\sf\trove4j\trove4j\3.0.3\trove4j-3.0.3.jar",
            "libraries\org\apache\commons\commons-compress\1.8.1\commons-compress-1.8.1.jar",
            "libraries\org\apache\commons\commons-lang3\3.3.2\commons-lang3-3.3.2.jar",
            "libraries\org\apache\httpcomponents\httpclient\4.3.3\httpclient-4.3.3.jar",
            "libraries\org\apache\httpcomponents\httpcore\4.3.2\httpcore-4.3.2.jar",
            "libraries\org\apache\logging\log4j\log4j-api\2.0-beta9\log4j-api-2.0-beta9.jar",
            "libraries\org\apache\logging\log4j\log4j-core\2.0-beta9\log4j-core-2.0-beta9.jar",
            "libraries\org\lwjgl\lwjgl\lwjgl\2.9.1\lwjgl-2.9.1.jar",
            "libraries\org\lwjgl\lwjgl\lwjgl_util\2.9.1\lwjgl_util-2.9.1.jar",
            "libraries\org\ow2\asm\asm-all\5.0.3\asm-all-5.0.3.jar",
            "libraries\org\ow2\asm\asm-all\5.0.3\asm-all-5.0.3.jar",
            "libraries\org\scala-lang\plugins\scala-continuations-library_2.11\1.0.2\scala-continuations-library_2.11-1.0.2.jar",
            "libraries\org\scala-lang\plugins\scala-continuations-plugin_2.11.1\1.0.2\scala-continuations-plugin_2.11.1-1.0.2.jar",
            "libraries\org\scala-lang\scala-actors-migration_2.11\1.1.0\scala-actors-migration_2.11-1.1.0.jar",
            "libraries\org\scala-lang\scala-compiler\2.11.1\scala-compiler-2.11.1.jar",
            "libraries\org\scala-lang\scala-library\2.11.1\scala-library-2.11.1.jar",
            "libraries\org\scala-lang\scala-parser-combinators_2.11\1.0.1\scala-parser-combinators_2.11-1.0.1.jar",
            "libraries\org\scala-lang\scala-reflect\2.11.1\scala-reflect-2.11.1.jar",
            "libraries\org\scala-lang\scala-swing_2.11\1.0.1\scala-swing_2.11-1.0.1.jar",
            "libraries\org\scala-lang\scala-xml_2.11\1.0.2\scala-xml_2.11-1.0.2.jar",
            "libraries\tv\twitch\twitch\6.5\twitch-6.5.jar"
            }
    Function getCommand(gamePath As String, gamePath_ As String, name As String, version As String)

        Dim library As String
        Dim os_bit As Integer = getJavaBit(version)

        ' -XX:HeapDumpPath=MojangTricksIntelDriversForPerformance_javaw.exe_minecraft.exe.heapdump -Xmx512M -XX:+UseConcMarkSweepGC -XX:+CMSIncrementalMode -XX:-UseAdaptiveSizePolicy -Xmn128M 
        Dim c As String = "-XX:HeapDumpPath=MojangTricksIntelDriversForPerformance_javaw.exe_minecraft.exe.heapdump -Xmx1G " & _
                          "-XX:+UseConcMarkSweepGC -XX:+CMSIncrementalMode -XX:-UseAdaptiveSizePolicy -Xmn128M -Djava.library.path=" & _
                          """" & gamePath & "Djava\lite" & """ -cp "
        For Each library In library_list
            c &= """" & gamePath & library & """;"
        Next

        c &= """" & gamePath & "versions\1.8\1.8.jar"" " & "net.minecraft.launchwrapper.Launch --tweakClass com.mumfrey.liteloader.launch.LiteLoaderTweaker --username " & name & _
             " --version 1.8 --gameDir """ & gamePath_ & """ --assetsDir """ & gamePath & "assets"" --assetIndex 1.8 --accessToken myaccesstoken --userProperties {} --tweakClass net.minecraftforge.fml.common.launcher.FMLTweaker"

        Return c
    End Function
    Function getVerJava() As String
        Dim ver32 As String = ""
        Dim ver64 As String = ""
        Try
            ver32 = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, RegistryView.Registry32). _
                    OpenSubKey("SOFTWARE\JavaSoft\Java Runtime Environment").GetValue("CurrentVersion")
            Return ver32
        Catch ex As Exception

        End Try
        Try
            ver64 = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, RegistryView.Registry64). _
                OpenSubKey("SOFTWARE\JavaSoft\Java Runtime Environment").GetValue("CurrentVersion")
            Return ver64
        Catch ex As Exception

        End Try

        Return Nothing
    End Function

    Function checkVerJava(ByVal version As String)
        'Dim ver As String

        'ver = My.Computer.Registry.GetValue("HKEY_LOCAL_MACHINE\SOFTWARE\JavaSoft\Java Runtime Environment", "CurrentVersion", Nothing)
        'Return ver
        If getVerJava().Contains(version) Then
            Return True
        End If
        Return False
    End Function

    Function getJavaBit(ByVal version As String)
        Dim java_ver = getVerJava()
        Dim ver32 As String = ""
        Dim ver64 As String = ""
        Try
            ver64 = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, RegistryView.Registry64). _
                OpenSubKey("SOFTWARE\JavaSoft\Java Runtime Environment\" & java_ver).GetValue("JavaHome")
            Return 64
        Catch ex As Exception

        End Try
        Try
            ver32 = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, RegistryView.Registry32). _
                    OpenSubKey("SOFTWARE\JavaSoft\Java Runtime Environment\" & java_ver).GetValue("JavaHome")
            Return 32
        Catch ex As Exception

        End Try

        Return Nothing
    End Function


    Function getJavaPath(version As String)

        Dim java_ver = getVerJava()
        Dim ver32 As String = ""
        Dim ver64 As String = ""
        Try
            ver64 = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, RegistryView.Registry64). _
                OpenSubKey("SOFTWARE\JavaSoft\Java Runtime Environment\" & java_ver).GetValue("JavaHome")
            Return ver64
        Catch ex As Exception

        End Try
        Try
            ver32 = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, RegistryView.Registry32). _
                    OpenSubKey("SOFTWARE\JavaSoft\Java Runtime Environment\" & java_ver).GetValue("JavaHome")
            Return ver32
        Catch ex As Exception

        End Try
        
        Return Nothing
    End Function

    Public Function CheckForInternetConnection() As Boolean
        Try
            Using client = New WebClient()
                Using stream = client.OpenRead("http://www.google.com")
                    Return True
                End Using
            End Using
        Catch
            Return False
        End Try
    End Function
End Module
