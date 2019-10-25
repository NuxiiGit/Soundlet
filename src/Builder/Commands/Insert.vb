Imports mpc_playlist.Builder

Public Class Insert
    Implements ICommand

    Public Sub Execute(ByRef list As Playlist, ByVal ParamArray params As String()) Implements ICommand.Execute
        Console.WriteLine("Inserting the following elements:")
        For Each record As String In params
            Console.WriteLine(" |> " & record)
            list.Add(record)
        Next
    End Sub

End Class