Imports PlaylistManager.Builder
Imports PlaylistManager.Playlist

Module Main

    Sub Main()
        Dim args As List(Of String) = New List(Of String)(Environment.GetCommandLineArgs())
        Try 
            Select args.Count()
            Case 1
                Console.WriteLine("To use this tool, please type")
                Console.WriteLine()
                Console.WriteLine("  mpc-pls <directory> <destination>")
                Console.WriteLine()
                Console.WriteLine("where 'directory' is the location of the directory containing your media files, and 'destination' is the location you want to save the final playlist file at.")
                Console.WriteLine()
                Console.WriteLine("Here is a list of currently supported playlist file extensions:")
                Console.WriteLine()
                For Each ext As String In Playlist.GetExtensions()
                    Console.WriteLine(" - " & ext.ToLower())
                Next
                Console.WriteLine()
                Console.WriteLine("Finally, here is a list of additional commands:")
                Console.WriteLine()
                For Each name As String In Builder.GetCommands()
                    Console.WriteLine(" - " & name.ToLower())
                Next
                Console.WriteLine()
                Console.WriteLine("You can use these by typing the symbol '" & Builder.PREFIX & "' followed by the name of the command. For example")
                Console.WriteLine()
                Console.WriteLine("  mpc-pls old.pls new.pls " & Builder.PREFIX & "insert ./music")
                Console.WriteLine()
                Console.Write("Press any key to exit.")
                Console.ReadKey()
                Console.WriteLine()
            Case 3
                Builder.Make(args(1), args(2))
            Case Is > 3
                Builder.Make(args(1), args(2), args.Skip(3).ToArray())
            Case Else
                Throw New ArgumentException("Please supply at least two arguments")
            End Select
        Catch e As Exception
            Console.WriteLine(e.Message)
        End Try
    End Sub

End Module
