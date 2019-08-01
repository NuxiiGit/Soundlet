Imports System.Reflection

''' <summary>
''' A modules which provides a command <see cref="Command.Extension"/> interface and procedures for parsing and executing these commands.
''' </summary>
Public Module Command

    ''' <summary>
    ''' Maintains a relationship between the name of a command, and its actual command extension <see cref="Command.Extension"/>.
    ''' </summary>
    Private extensions As Dictionary(Of String, Extension) = New Dictionary(Of String, Extension)

    ''' <summary>
    ''' An interface which exposes command extensions.
    ''' </summary>
    Public Interface Extension
        
        ''' <summary>
        ''' Executes this command using this parameter array.
        ''' </summary>
        ''' <param name="params">The array of parameters to pass.</param>
        Sub Execute(ByVal ParamArray params As String())

        ''' <summary>
        ''' The description of this command.
        ''' </summary>
        ''' <returns>An in-depth description of what this command does, it's syntax, and how to use it.</returns>
        ReadOnly Property Description As String

    End Interface

    ''' <summary>
    ''' Uses reflection to compile the dictionary of command extensions at runtime.
    ''' </summary>
    Sub New()
        Dim template As Type = GetType(Extension)
        For Each dataType As Type In template.Assembly.GetTypes()
            If (dataType.IsClass() AndAlso dataType.GetInterfaces.Contains(template))
                '' valid class type
                extensions.Add(dataType.Name.ToLower(), Activator.CreateInstance(dataType))
            End If
        Next
    End Sub
    
    ''' <summary>
    ''' Parses a single command.
    ''' </summary>
    ''' <param name="args"></param>
    ''' <exception cref="ArgumentNullException">Thrown if <paramref name="args"/> is <c>Nothing</c>.</exception>
    ''' <exception cref="ArgumentException">Thrown if <paramref name="args"/> is empty.</exception>
    ''' <exception cref="KeyNotFoundException">Thrown if this command does not exist.</exception>
    Public Sub Parse(ByVal ParamArray args As String())
        If (args Is Nothing) Then Throw New ArgumentNullException("Args must not be Nothing!")
        If (args.Length = 0) Then Throw New ArgumentException("Args must not be empty!")
        Dim name As String = args(0)
        If (Not extensions.ContainsKey(name)) Then _
                Throw New KeyNotFoundException("Command '" & name & "' does not exist!")
        Dim params As List(Of String) = New List(Of String)(args)
        params.RemoveAt(0)
        extensions(name).Execute(params.ToArray())
    End Sub

    ''' <summary>
    ''' Returns help information.
    ''' </summary>
    ''' <returns>An iterator of help information.</returns>
    Public Iterator Function Help() As IEnumerable(Of KeyValuePair(Of String, String))
        For Each command In extensions
            Dim name As String = command.Key
            Dim desc As String = command.Value.Description
            Yield New KeyValuePair(Of String, String)(name, desc)
        Next
    End Function

    ''' <summary>
    ''' Returns help information of a specific command.
    ''' </summary>
    ''' <returns>The description of this command.</returns>
    ''' <exception cref="KeyNotFoundException">Thrown if this command does not exist.</exception>
    Public Function Help(ByVal name As String) As String
        If (Not extensions.ContainsKey(name)) Then _
                Throw New KeyNotFoundException("Command '" & name & "' does not exist!")
        Return extensions(name).Description
    End Function

End Module
