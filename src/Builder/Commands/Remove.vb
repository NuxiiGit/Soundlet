Public Class Remove
    Implements PlaylistManager.Builder.ICommand

    Public Sub Execute(ByRef list As Playlist, ByVal ParamArray params As String()) Implements PlaylistManager.Builder.ICommand.Execute
        Console.WriteLine("Removing the following elements:")
        For Each record As String In params
            Console.WriteLine(" |> " & record)
            list.Remove(record)
        Next
    End Sub

End Class