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
        Const PADDING As Integer = 2
        Dim longestLength As UInteger = 0
        For Each pair In Command.Help()
            Dim length As UInteger = pair.Key.Length
            If (length > longestLength) Then longestLength = length
        Next
        Console.WriteLine()
        For Each pair In Command.Help()
            Dim name As String = pair.Key
            Dim desc As String = pair.Value
            Dim margin As String = New String(" ", PADDING) & name & New String(" "c, longestLength - name.Length) & " | "
            Dim summary As String = If(
                    desc.Contains(Environment.NewLine),
                    desc.Split(Environment.NewLine)(0),
                    desc)
            Dim trail As String = " ..."
            Dim contentWidth As Integer = Console.BufferWidth - PADDING - margin.Length - trail.Length
            If (summary.Length > contentWidth) Then summary = summary.Substring(0, contentWidth) & trail
            Console.WriteLine(margin & summary)
        Next
    End Sub

End Class