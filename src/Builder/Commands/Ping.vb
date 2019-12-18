Public Class Ping
    Implements PlaylistManager.Builder.ICommand

    Public Sub Execute(ByRef list As Playlist, ByVal modifiers As String(), ByVal params As String()) Implements PlaylistManager.Builder.ICommand.Execute
        Console.WriteLine("Modifiers:")
        For Each modifier In modifiers
            Console.WriteLine("  | '{0}'", modifier)
        Next
        Console.WriteLine("Params")
        For Each param In params
            Console.WriteLine("  | '{0}'", param)
        Next
    End Sub

End Class
