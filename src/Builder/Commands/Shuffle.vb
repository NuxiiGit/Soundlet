Imports mpc_playlist.Builder

Public Class Shuffle
    Implements ICommand

    Public Sub Execute(ByRef list As Playlist, ByVal ParamArray params As String()) Implements ICommand.Execute
        list.Shuffle()
    End Sub

End Class