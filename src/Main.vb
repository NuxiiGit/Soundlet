Imports mpc_playlist.Command

''' <summary>
''' Main module, baby!!!!!!
''' </summary>
Module Main

    ''' <summary>
    ''' Main method, baby!!!!!!!!!!
    ''' </summary>
    Sub Main()
        Dim args As List(Of String) = New List(Of String)(Environment.GetCommandLineArgs())
        args.RemoveAt(0) '' remove the first argument, which is usually the path to the executable
        Try 
            Command.Parse(args.toArray())
        Catch e As Exception
            Console.ForegroundColor = ConsoleColor.DarkRed
            Console.Write("ERROR")
            Console.ResetColor()
            Console.WriteLine(": " & e.Message)
            Console.WriteLine()
            Console.WriteLine(e.StackTrace)
        End Try
    End Sub

End Module
