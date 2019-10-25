Imports mpc_playlist.Builder

Public Class Ping
    Implements ICommand

    Public Sub Execute(ByRef list As Playlist, ByVal ParamArray params As String()) Implements ICommand.Execute
        Console.WriteLine("Pong!")
    End Sub

End Class
