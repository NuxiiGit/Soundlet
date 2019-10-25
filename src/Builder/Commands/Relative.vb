Imports mpc_playlist.Builder

Public Class Relative
    Implements ICommand

    Public Sub Execute(ByRef list As Playlist, ByVal ParamArray params As String()) Implements ICommand.Execute
        Console.WriteLine("Setting playlist structure to relative")
        list.relative = True
    End Sub

End Class