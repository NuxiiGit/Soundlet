Imports mpc_playlist.MediaPlayerPlaylist

Module PlaylistManager

    Sub Main()
        For Each file In MediaPlayerPlaylist.LoadFormat(Console.ReadLine())
            Console.WriteLine(file)
        Next
        Console.ReadKey()
    End Sub

End Module
