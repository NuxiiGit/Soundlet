Imports System.Reflection
Imports mpc_playlist.Playlist

''' <summary>
''' A module which provides a command <see cref="Builder.ICommand"/> interface and procedures for parsing and executing commands to build a playlist file.
''' </summary>
Public Module Builder

    ''' <summary>
    ''' The separator used to distinguish commands from normal command line arguments.
    ''' </summary>
    Const PREFIX As Char = "-"c

    ''' <summary>
    ''' Maintains a relationship between the name of a command, and its actual command extension <see cref="Builder.ICommand"/>.
    ''' </summary>
    Private extensions As Dictionary(Of String, ICommand) = New Dictionary(Of String, ICommand)

    ''' <summary>
    ''' An interface which exposes command extensions.
    ''' </summary>
    Public Interface ICommand
        
        ''' <summary>
        ''' Executes this command using this parameter array.
        ''' </summary>
        ''' <param name="params">The array of parameters to pass.</param>
        ''' <exception cref="ArgumentException">Thrown if there was a problem with the parameter array.</exception>
        Sub Execute(ByRef list As Playlist, ByVal ParamArray params As String())

    End Interface

    ''' <summary>
    ''' Uses reflection to compile the dictionary of command extensions at runtime.
    ''' </summary>
    Sub New()
        Dim template As Type = GetType(ICommand)
        For Each dataType As Type In template.Assembly.GetTypes()
            If dataType.IsClass() AndAlso dataType.GetInterfaces.Contains(template) Then _
                    extensions.Add(dataType.Name.ToUpper(), Activator.CreateInstance(dataType))
        Next
    End Sub

    ''' <summary>
    ''' Parses a list of tokens and executes the commands.
    ''' </summary>
    ''' <param name="filepath">The directory to use when searching for candidate sound files.</param>
    ''' <param name="dest">The target location of the playlist file.</param>
    ''' <param name="command">The list of command tokens.</param>
    Public Sub Make(ByVal filepath As String, ByVal dest As String, ByVal ParamArray command As String())
        Dim list As Playlist = New Playlist(filepath)
        ' run optional commands
        For Each pair In Builder.Parse(command)
            pair.Item1.Execute(list, pair.Item2)
        Next
        list.Save(dest)
    End Sub

    ''' <summary>
    ''' Converts a list of tokens into a list of command objects and their arguments.
    ''' </summary>
    ''' <param name="tokens"></param>
    ''' <returns>An array of tuples containing the command to call, and the parameters to call the command with.</returns>
    Private Function Parse(ByVal tokens As String()) As Tuple(Of ICommand, String())()
        Dim commands As List(Of Tuple(Of ICommand, String())) = New List(Of Tuple(Of ICommand, String()))
        Dim len As Integer = tokens.Length
        If len <> 0
            Dim command As ICommand = FindCommand(tokens(0))
            Dim arguments As List(Of String) = New List(Of String)
            For i As Integer = 1 To len - 1
                Dim token As String = tokens(i)
                If IsCommand(token)
                    commands.Add(New Tuple(Of ICommand, String())(command, arguments.ToArray))
                    command = FindCommand(token)
                    arguments.Clear()
                Else
                    arguments.Add(token)
                End If
            Next
            commands.Add(New Tuple(Of ICommand, String())(command, arguments.ToArray))
        End If
        Return commands.ToArray()
    End Function
    
    ''' <summary>
    ''' Returns whether a token is like a command.
    ''' </summary>
    ''' <param name="token"></param>
    ''' <returns><c>True</c> if the token starts with <see cref="PREFIX"/> and <c>False</c> otherwise.</returns>
    Private Function IsCommand(ByVal token As String) As Boolean
        Return token Like PREFIX & "[!" & PREFIX & "]*"
    End Function

    ''' <summary>
    ''' Finds the command of this token, if it exists.
    ''' </summary>
    ''' <param name="token">The token to check.</param>
    ''' <returns>The callable command object for this token.</returns>
    ''' <exception cref="KeyNotFoundException">Thrown if the command does not exist.</exception>
    ''' <exception cref="ArgumentException">Thrown if the token is not a valid command.</exception>
    Private Function FindCommand(ByVal token As String) As ICommand
        If IsCommand(token)
            token = token.Remove(0, 1).ToUpper()
            If extensions.ContainsKey(token) Then Return extensions(token)
            Throw New KeyNotFoundException("Command with name '" & token & "' does not exist!")
        End If
        Throw New ArgumentException("'" & token & "' is not a valid command name.")
    End Function

End Module