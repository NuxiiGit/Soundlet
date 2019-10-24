Imports mpc_playlist.Builder

Public Class Test
    Implements ICommand

    Public Sub Execute(ByRef list As Playlist, ByVal ParamArray params As String()) Implements ICommand.Execute
        Console.WriteLine("Gotcha!")
    End Sub

End Class
