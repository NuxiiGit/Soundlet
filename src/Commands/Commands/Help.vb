Imports mpc_playlist.Command

Public Class Help
    Implements Command.Extension

    ''' <summary>
    ''' <see cref="Command.Extension.Description"/>
    ''' </summary>
    Public ReadOnly Property Description As String Implements Extension.Description
        Get
            Return "Lists the help information for all commands."
        End Get
    End Property

    ''' <summary>
    ''' <see cref="Command.Extension.Execute(String())"/>
    ''' </summary>
    ''' <exception cref="ArgumentException">Thrown when <paramref name="params"/> is empty.</exception>
    Public Sub Execute(ParamArray params() As String) Implements Extension.Execute
        Dim maxWidth As UInteger = 0
        For Each pair In Command.Help()
            Dim width As UInteger = pair.Key.Length
            If (width > maxWidth) Then maxWidth = width
        Next
        Console.WriteLine()
        For Each pair In Command.Help()
            Dim name As String = pair.Key
            Dim desc As String = pair.Value
            Dim margin As String = New String(" "c, maxWidth - name.Length) & " | "
            Console.WriteLine("  {0}" & margin & "{1}", name, desc)
        Next
    End Sub

End Class