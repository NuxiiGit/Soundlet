Public Class Subtract
    Implements PlaylistManager.Builder.ICommand

    Public Sub Execute(ByRef list As Playlist, ByVal ParamArray params As String()) Implements PlaylistManager.Builder.ICommand.Execute
        Console.WriteLine("Subtracting the following playlist files:")
        For Each filepath As String In params
            Console.WriteLine(" |> " & filepath)
            For Each record In New Playlist(filepath)
                list.Remove(record)
            Next
        Next
    End Sub

End Class