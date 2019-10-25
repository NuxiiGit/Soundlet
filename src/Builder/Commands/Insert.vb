Public Class Insert
    Implements PlaylistManager.Builder.ICommand

    Public Sub Execute(ByRef list As Playlist, ByVal ParamArray params As String()) Implements PlaylistManager.Builder.ICommand.Execute
        Console.WriteLine("Inserting the following elements:")
        For Each record As String In params
            Console.WriteLine(" |> " & record)
            list.Add(record)
        Next
    End Sub

End Class