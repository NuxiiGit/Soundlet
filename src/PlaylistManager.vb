Imports mpc_playlist.MediaPlayerPlaylist

Module PlaylistManager

    Sub Main()
        Dim playlist As String() = MediaPlayerPlaylist.LoadFormat(Console.ReadLine())
        Console.WriteLine("Reading...")
        Console.WriteLine("Contents:")
        For Each file In playlist
            Console.WriteLine(file)
        Next
        Console.ReadKey()
        MediaPlayerPlaylist.SaveFormat(Console.ReadLine(), playlist, true)
        Console.WriteLine("Writing...")
        Console.ReadKey()
    End Sub

End Module
