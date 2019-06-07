Imports System.Reflection
Imports System.IO.StreamReader
Imports System.IO.StreamWriter
Imports System.IO
Imports System.Xml.XmlReader
Imports System.Xml.XmlWriter
Imports System.Xml

''' <summary>
''' A class which can be used to construct and manage Media Player (Classic) playlist formats.
''' </summary>
Public NotInheritable Class Playlist
    
    ''' <summary>
    ''' Maintains a relationship between the name of a file extension, and its actual playlist extension <see cref="Playlist.Extension"/>.
    ''' </summary>
    Private Shared extensions As Dictionary(Of String, Extension) = New Dictionary(Of String, Extension)
    
    Private paths As List(Of String) = New List(Of String)

    ''' <summary>
    ''' An interface which manages playlist extensions.
    ''' </summary>
    Public Interface Extension
        
        ''' <summary>
        ''' Captures the paths from this stream and inserts them into the <paramref name="paths"/> list.
        ''' </summary>
        ''' <param name="stream">The input stream for this file.</param>
        ''' <param name="paths">The output list to insert path names.</param>
        Sub Decode(ByRef stream As StreamReader, ByRef paths As List(Of String))

        ''' <summary>
        ''' Pushes the paths from the <paramref name="paths"/> array into this stream.
        ''' </summary>
        ''' <param name="stream"></param>
        ''' <param name="paths"></param>
        Sub Encode(ByRef stream As StreamWriter, ByRef paths As List(Of String))

    End Interface

    ''' <summary>
    ''' Uses reflection to compile the dictionary of file extensions at runtime.
    ''' </summary>
    Shared Sub Run()
        Dim template As Type = GetType(Extension)
        For Each dataType As Type In template.Assembly.GetTypes()
            If (dataType.IsClass() AndAlso dataType.GetInterfaces.Contains(template))
                '' valid class type
                extensions.Add("." & dataType.Name.ToUpper(), Activator.CreateInstance(dataType))
            End If
        Next
    End Sub

    ''' <summary>
    ''' Loads the contents of a playlist file into the playlist.
    ''' </summary>
    ''' <param name="filepath">The path of the playlist file.</param>
    ''' <exception cref="IOException">Thrown when there was an error loading the file contents.</exception>
    ''' <exception cref="KeyNotFoundException">Thrown when the file extension for <paramref name="filepath"/> is not supported.</exception>
    Public Sub Load(ByVal filepath As String)
        If (Not File.Exists(filepath)) Then Throw New IOException("Playlist file does not exist.")
        Using input As StreamReader = My.Computer.FileSystem.OpenTextFileReader(filepath)
            If (input.EndOfStream) Then Throw New IOException("Playlist file cannot be empty.")
            '' compile paths
            Dim ext As String = Path.GetExtension(filepath).ToUpper()
            If (Not extensions.ContainsKey(ext)) Then Throw New _
                    KeyNotFoundException(String.Format("Unknown playlist file extension {0}.", ext))
            extensions(ext).Decode(input, paths)
        End Using
        Dim dir As String = Directory.GetCurrentDirectory()
        Directory.SetCurrentDirectory(Path.GetDirectoryName(filepath))
        For i As Integer = 0 To (paths.Count - 1)
            paths(i) = Path.GetFullPath(paths(i))
        Next
        Directory.SetCurrentDirectory(dir)
    End Sub

End Class