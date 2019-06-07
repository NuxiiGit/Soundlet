
Module MainModule

    Sub Main()

        Playlist.Run()
        Console.ReadKey()

        Dim a As Integer() = {1, 2, 3}.Select(Function(x) x * 2).ToArray()
        For each item In a
            Console.WriteLine(item)
        Next

        Dim path As String = Console.ReadLine().Trim("""")
        Dim pls As String() = MediaPlayerPlaylist.Decode(path)
        Console.WriteLine("Reading...")
        Console.WriteLine("Contents:")
        For Each file In pls
            Console.WriteLine(file)
        Next
        Console.ReadKey()
        MediaPlayerPlaylist.Encode(Console.ReadLine(), pls, true)
        Console.WriteLine("Writing...")
        Console.ReadKey()
    End Sub

End Module
