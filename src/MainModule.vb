Imports Playlist = mpc_playlist.MediaPlayerPlaylist

Module MainModule

    Sub Main()

        Dim a As Integer() = {1, 2, 3}.Select(Function(x) x * 2).ToArray()
        For each item In a
            Console.WriteLine(item)
        Next

        Dim path As String = Console.ReadLine().Trim("""")
        Dim pls As String() = Playlist.Load(path)
        Console.WriteLine("Reading...")
        Console.WriteLine("Contents:")
        For Each file In pls
            Console.WriteLine(file)
        Next
        Console.ReadKey()
        Playlist.Save(Console.ReadLine(), pls, true)
        Console.WriteLine("Writing...")
        Console.ReadKey()
    End Sub

End Module
