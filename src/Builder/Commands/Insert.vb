Imports mpc_playlist.Builder

Public Class Insert
    Implements ICommand

    Public Sub Execute(ByRef list As Playlist, ByVal ParamArray params As String()) Implements ICommand.Execute
        For Each record As String In params
            list.Add(record)
        Next
    End Sub

End Class