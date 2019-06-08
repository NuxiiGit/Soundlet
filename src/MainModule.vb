
Module MainModule

    Sub Main()
        
        Dim pls As Playlist = New Playlist()

        pls.Load(Console.ReadLine().Trim(""""c))

        pls.Add("this.path")

        For each path In pls
            Console.WriteLine(path)
        Next

        Console.ReadKey()

    End Sub

End Module
