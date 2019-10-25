Imports mpc_playlist.Builder

Public Class Relative
    Implements ICommand

    Public Sub Execute(ByRef list As Playlist, ByVal ParamArray params As String()) Implements ICommand.Execute
        list.relative = True
    End Sub

End Class