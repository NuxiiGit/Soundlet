Imports PlaylistManager.Builder

Module Main

    Sub Main()
        Dim args As List(Of String) = New List(Of String)(Environment.GetCommandLineArgs())
        Try 
            Select args.Count()
            Case 1
                Console.WriteLine("Hello, you are using this wrong")
                Console.WriteLine()
                Console.Write("Please press any key to exit")
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
