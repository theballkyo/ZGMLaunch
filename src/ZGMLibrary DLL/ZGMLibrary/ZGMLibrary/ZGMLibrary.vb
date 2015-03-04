Imports System.IO
Imports System.Xml
Imports System.Text.RegularExpressions


Public Class ZGMLibrary
    Public Shared Function GetJavaVersion() As String
        Try
            Dim procStartInfo As New System.Diagnostics.ProcessStartInfo("java", "-version ")
            procStartInfo.RedirectStandardOutput = True
            procStartInfo.RedirectStandardError = True
            procStartInfo.UseShellExecute = False
            procStartInfo.CreateNoWindow = True
            Dim proc As System.Diagnostics.Process = New Process()
            proc.StartInfo = procStartInfo
            proc.Start()
            Dim digits() As String = Regex.Split(proc.StandardError.ReadLine(), """")
            Return digits(1)
        Catch objException As Exception
            Return "555"
        End Try
    End Function
    Function XmlPatchRead(ByVal filename As String, ByRef file_data(,) As String)
        Dim i = 0
        Dim m_xmlr As XmlTextReader
        'Create the XML Reader
        m_xmlr = New XmlTextReader(filename)
        'Disable whitespace so that you don't have to read over whitespaces
        m_xmlr.WhitespaceHandling = WhitespaceHandling.None
        'read the xml declaration and advance to family tag
        m_xmlr.Read()
        'read the family tag
        m_xmlr.Read()
        m_xmlr.Read()
        'read version
        file_data(3, 0) = m_xmlr.GetAttribute("ver").ToString
        'Load the Loop
        While Not m_xmlr.EOF
            'Go to the name tag
            m_xmlr.Read()

            'if not start element exit while loop
            If Not m_xmlr.IsStartElement() Then
                Exit While
            End If
            'Get the Gender Attribute Value
            'Dim genderAttribute = m_xmlr.GetAttribute("time")
            'Read elements firstname and lastname

            m_xmlr.Read()
            file_data(0, i) = m_xmlr.ReadElementString("path")
            file_data(1, i) = m_xmlr.ReadElementString("name")
            file_data(2, i) = m_xmlr.ReadElementString("md5")

            'MsgBox(genderAttribute)
            'Write Result to the Console
            'MsgBox("Gender: " & genderAttribute _
            '  & " filename: " & file_data(0, i) & " md5: " _
            '  & file_data(1, i))
            i = i + 1
        End While
        'close the reader
        m_xmlr.Close()
        Return file_data
    End Function

    Function XmlPatchWrite(ByVal filename As String, ByVal file_data(,) As String)
        ' Create XmlWriterSettings.
        Dim settings As XmlWriterSettings = New XmlWriterSettings()
        settings.Indent = True

        ' Create XmlWriter.
        Using writer As XmlWriter = XmlWriter.Create("C:\employees.xml", settings)
            Dim num_files = UBound(file_data)
            ' Begin writing.
            writer.WriteStartDocument()
            writer.WriteStartElement("ZGMPatch", DateTime.Now.ToString())
            writer.WriteStartElement("zgmversion", "200")

            ' Loop over employees in array.
            For i = 0 To num_files
                writer.WriteStartElement("file")
                writer.WriteElementString("path", file_data(0, i))
                writer.WriteElementString("name", file_data(1, i))
                writer.WriteElementString("md5", file_data(2, i))
                writer.WriteEndElement()
            Next

            ' End document.
            writer.WriteEndElement()
            writer.WriteEndElement()
            writer.WriteEndDocument()
        End Using
        Return True
    End Function
End Class

