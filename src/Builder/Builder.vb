Imports System.Reflection
Imports PlaylistManager.Playlist

''' <summary>
''' A module which provides a command <see cref="Builder.ICommand"/> interface and procedures for parsing and executing commands to build a playlist file.
''' </summary>
Public Module Builder

    ''' <summary>
    ''' The separator used to distinguish commands from normal command line arguments.
    ''' </summary>
    Public Const PREFIX As Char = "-"c

    ''' <summary>
    ''' Maintains a relationship between the name of a command, and its actual command extension <see cref="Builder.ICommand"/>.
    ''' </summary>
    Private validCommands As Dictionary(Of String, ICommand) = New Dictionary(Of String, ICommand)

    ''' <summary>
    ''' An interface which exposes command extensions.
    ''' </summary>
    Public Interface ICommand
        
        ''' <summary>
        ''' Executes this command using this parameter array.
        ''' </summary>
        ''' <param name="modifiers">The array of command modifiers to pass.</param>
        ''' <param name="params">The array of parameters to pass.</param>
        ''' <exception cref="ArgumentException">Thrown if there was a problem with the parameter array.</exception>
        Sub Execute(ByRef list As Playlist, ByVal modifiers As String(), ByVal params As String())

    End Interface

    ''' <summary>
    ''' A struct which encapsulates a command and its arguments.
    ''' </summary>
    Public Structure CommandData
        
        ''' <summary>
        ''' The command object.
        ''' </summary>
        Public command As ICommand

        ''' <summary>
        ''' The optional command modifiers.
        ''' </summary>
        ''' <remarks>Delimited by colons.</remarks>
        Public modifiers As String()

        ''' <summary>
        ''' The parameters to pass to the command.
        ''' </summary>
        Public params As String()

    End Structure

    ''' <summary>
    ''' Uses reflection to compile the dictionary of command extensions at runtime.
    ''' </summary>
    Sub New()
        Dim template As Type = GetType(ICommand)
        For Each dataType As Type In template.Assembly.GetTypes()
            If dataType.IsClass() AndAlso dataType.GetInterfaces.Contains(template) Then _
                    validCommands.Add(dataType.Name.ToUpper(), Activator.CreateInstance(dataType))
        Next
    End Sub

    ''' <summary>
    ''' Returns an array of all possible commands.
    ''' </summary>
    ''' <returns>An array of all available commands.</returns>
    Public Function GetCommands() As String()
        Return validCommands.Keys.ToArray()
    End Function

    ''' <summary>
    ''' Parses a list of tokens and executes the commands.
    ''' </summary>
    ''' <param name="filepath">The directory to use when searching for candidate sound files.</param>
    ''' <param name="tokens">The list of command tokens.</param>
    Public Function Make(ByVal filepath As String, ByVal ParamArray tokens As String())
        Dim list As Playlist = New Playlist(filepath)
        ' run optional commands
        For Each record In Builder.Parse(tokens)
            record.command.Execute(list, record.modifiers, record.params)
        Next
        Return list
    End Function

    ''' <summary>
    ''' Converts a list of tokens into a list of command objects and their arguments.
    ''' </summary>
    ''' <param name="tokens"></param>
    ''' <returns>An array of tuples containing the command to call, and the parameters to call the command with.</returns>
    Private Function Parse(ByVal tokens As String()) As CommandData()
        Dim commands As List(Of CommandData) = New List(Of CommandData)
        Dim len As Integer = tokens.Length
        Dim i As Integer = 0
        While i < len
            Dim header As List(Of String) = tokens(i).Split(":"c).ToList()
            If header.Count = 0 Then Throw New ArgumentNullException("Commands must not be empty!")
            Dim commandName As String = header(0).Remove(0, 1).ToUpper()
            If Not validCommands.ContainsKey(commandName) Then _
                    Throw New KeyNotFoundException("Command with name '" & commandName & "' does not exist!")
            Dim command As ICommand = validCommands(commandName)
            header.RemoveAt(0)
            Dim modifiers As String() = header.ToArray()
            Dim paramList As List(Of String) = New List(Of String)
            i += 1
            While i < len
                Dim param As String = tokens(i)
                If IsCommandName(param) Then Exit While
                paramList.Add(param)
                i += 1
            End While
            commands.Add(New CommandData With {
                .command = command,
                .modifiers = modifiers,
                .params = paramList.ToArray()
            })
        End While
        Return commands.ToArray()
    End Function
    
    ''' <summary>
    ''' Returns whether a token is like a command.
    ''' </summary>
    ''' <param name="token">The token to check.</param>
    ''' <returns><c>True</c> if the token starts with <see cref="PREFIX"/> and <c>False</c> otherwise.</returns>
    Private Function IsCommandName(ByVal token As String) As Boolean
        Return token Like PREFIX & "[!" & PREFIX & "]*"
    End Function

End Module