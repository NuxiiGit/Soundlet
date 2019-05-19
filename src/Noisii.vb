Module Noisii

    Sub Main()
        Console.writeLine("Hello!")
        Console.WriteLine("These are your paramters: ")
        For Each arg In Environment.GetCommandLineArgs
            Console.WriteLine(arg)
        Next
        Console.ReadKey
    End Sub

End Module