Public Class Add
    Implements PlaylistManager.Builder.ICommand

    Public Sub Execute(ByRef list As Playlist, ByVal ParamArray params As String()) Implements PlaylistManager.Builder.ICommand.Execute
        Console.WriteLine("Appending the following playlist files:")
        For Each filepath As String In params
            Console.WriteLine(" - " & filepath)
            For Each record In New Playlist(filepath)
                list.Add(record)
            Next
        Next
    End Sub

End Class