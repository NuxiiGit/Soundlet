Imports mpc_playlist.Builder

Public Class Remove
    Implements ICommand

    Public Sub Execute(ByRef list As Playlist, ByVal ParamArray params As String()) Implements ICommand.Execute
        For Each record As String In params
            list.Remove(record)
        Next
    End Sub

End Class