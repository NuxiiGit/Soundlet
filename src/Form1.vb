Public Class Form1

    Dim WithEvents Player As New WMPLib.WindowsMediaPlayer

    Private Sub Form1_FormClosed(ByVal sender As Object, ByVal e As EventArgs) Handles Me.FormClosed
        Player = Nothing
    End Sub

    Private Sub Form1_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        Player.URL = "C:\tmp\music.mp3"
        Player.settings.volume = 25
        Player.controls.play()
    End Sub

    Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As EventArgs) Handles Me.FormClosing
        Player.controls.stop()
        Player.close()
    End Sub

    Private Sub Player_MediaError(ByVal pMediaObject As Object) Handles Player.MediaError
        Me.Close()
    End Sub

End Class
