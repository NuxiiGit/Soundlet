
Module MainModule

    Sub Main()
        Dim playlist As MediaPlayerPlaylist = New MediaPlayerPlaylist()
        playlist.Load(Console.ReadLine())
        Console.WriteLine("Reading...")
        Console.WriteLine("Contents:")
        For Each file In playlist
            Console.WriteLine(file)
        Next
        Console.ReadKey()
        playlist.Save(Console.ReadLine(), true)
        Console.WriteLine("Writing...")
        Console.ReadKey()
    End Sub

End Module
