Imports mpc_playlist.Playlist
Imports mpc_playlist.Command

Module Main

    Sub Main()
        
        Command.Parse("test", "x")

        Dim audio As Id3.Mp3 = New Id3.Mp3(Console.ReadLine().Trim(""""c))
        for each tag in audio.GetAllTags()
            dim artists As String = tag.Artists
            dim genre As String = tag.Genre
            Console.WriteLine("Arist(s): " & artists)
            Console.WriteLine("Genre(s):" & genre)
        Next
        Console.WriteLine("ok")

        Dim pls As Playlist = New Playlist()

        pls.Load(Console.ReadLine().Trim(""""c))

        For each path In pls
            Console.WriteLine(path)
        Next

        Console.ReadKey()

    End Sub

End Module
