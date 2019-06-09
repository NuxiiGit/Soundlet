Imports mpc_playlist.Playlist

Module MainModule

    Sub Main()
        
        Dim pls As Playlist = New Playlist()

        pls.Load(Console.ReadLine().Trim(""""c))

        pls.Add("this.path")
        
        pls = New Playlist(pls.Concat(New Playlist({"test.png"})))

        For each path In pls
            Console.WriteLine(path)
        Next

        Console.ReadKey()

    End Sub

End Module
