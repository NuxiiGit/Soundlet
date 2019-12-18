Public Class Relative
    Implements PlaylistManager.Builder.ICommand

    Public Sub Execute(ByRef list As Playlist, ByVal modifiers As String(), ByVal params As String()) Implements PlaylistManager.Builder.ICommand.Execute
        Console.WriteLine("Setting playlist structure to relative...")
        list.relative = True
    End Sub

End Class