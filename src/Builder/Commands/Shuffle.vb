Public Class Shuffle
    Implements PlaylistManager.Builder.ICommand

    Public Sub Execute(ByRef list As Playlist, ByVal ParamArray params As String()) Implements PlaylistManager.Builder.ICommand.Execute
        Console.WriteLine("Shuffling the playlist elements")
        list.Shuffle()
    End Sub

End Class