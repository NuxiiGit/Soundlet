﻿Imports mpc_playlist.Builder

Module Main

    Sub Main()
        Console.WriteLine(System.IO.Path.GetFullPath("."))
        'Builder.Execute("", "", {"-test", "a", "-- b", "nice", "ok"})
        Console.ReadKey()
        'Dim args As List(Of String) = New List(Of String)(Environment.GetCommandLineArgs())
        'args.RemoveAt(0) '' remove the first argument, which is usually the path to the executable
        'Try 
        '    Command.Parse(args.toArray())
        'Catch e As Exception
        '    Console.ForegroundColor = ConsoleColor.DarkRed
        '    Console.Write("ERROR")
        '    Console.ResetColor()
        '    Console.WriteLine("({0}): {1}", e.GetType.ToString(), e.Message)
        '    ''Console.WriteLine()
        '    ''Console.WriteLine(e.StackTrace)
        'End Try
    End Sub

End Module
