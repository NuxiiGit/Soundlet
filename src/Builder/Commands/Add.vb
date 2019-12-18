Public Class Add
    Implements PlaylistManager.Builder.ICommand

    Public Sub Execute(ByRef list As Playlist, ByVal modifiers As String(), ByVal params As String()) Implements PlaylistManager.Builder.ICommand.Execute
        Console.WriteLine("Appending playlist files...")
        For Each filepath As String In params
            For Each record As String In New Playlist(filepath)
                list.Add(record)
            Next
        Next
    End Sub

End Class