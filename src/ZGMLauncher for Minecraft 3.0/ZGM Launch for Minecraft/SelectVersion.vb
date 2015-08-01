Public Class SelectVersion

    Public Shared selectVersion As String = ""
    Private Sub SelectVersion_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If ListBox1.SelectedIndex = -1 Then
            MsgBox("Please select game version.")
            Exit Sub
        End If
        Select Case ListBox1.SelectedItem
            Case "1.8 with MODs and Shaders"
                selectVersion = "180forgeshaders"
                MsgBox("ยังไม่เปิดให้บริการ")
                Exit Sub
            Case "1.8 with MODs - Recommend"
                selectVersion = "180forge"
            Case "1.8"
                selectVersion = "180"
                'MsgBox("ยังไม่เปิดให้บริการ")
                'Exit Sub
            Case "1.7.10 with MODs"
                selectVersion = "1710forge"
            Case "1.7.10"
                selectVersion = "1710"
                MsgBox("ยังไม่เปิดให้บริการ")
                Exit Sub
        End Select
        Form1.Runsystem()
        Me.Close()
    End Sub

    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox1.SelectedIndexChanged

    End Sub
End Class