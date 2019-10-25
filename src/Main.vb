Imports PlaylistManager.Builder

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
                Console.WriteLine("Here is a list of additional commands:" & vbCrLf)
                For Each name As String In Builder.GetCommands()
                    Console.WriteLine(" - " & name.ToLower())
                Next
                Console.WriteLine(vbCrLf & "You can use these by typing the symbol '" & Builder.PREFIX & "' followed by the name of the command. E.g. '" & Builder.PREFIX & "remove <arguments>'.")
                Console.Write(vbCrLf & "Press any key to exit.")
                Console.ReadKey()
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
