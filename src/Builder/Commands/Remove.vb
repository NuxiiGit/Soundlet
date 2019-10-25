Imports mpc_playlist.Builder

Public Class Remove
    Implements ICommand

    Public Sub Execute(ByRef list As Playlist, ByVal ParamArray params As String()) Implements ICommand.Execute
        Console.WriteLine("Removing the following elements:")
        For Each record As String In params
            Console.WriteLine(" |> " & record)
            list.Remove(record)
        Next
    End Sub

End Class