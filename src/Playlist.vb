Imports System.IO.StreamReader
Imports System.IO.StreamWriter
Imports System.IO
Imports System.Reflection

''' <summary>
''' A class which can be used to construct and manage Media Player (Classic) playlist formats.
''' </summary>
Public NotInheritable Class Playlist
    Implements IList(Of String)

    ''' <summary>
    ''' Maintains a relationship between the name of a file extension, and its actual playlist extension <see cref="Playlist.Extension"/>.
    ''' </summary>
    Private Shared extensions As Dictionary(Of String, Extension) = New Dictionary(Of String, Extension)
    
    ''' <summary>
    ''' Stores the list of filepaths for this playlist.
    ''' </summary>
    Private paths As List(Of String) = New List(Of String)

    ''' <summary>
    ''' Gets and sets a filepath by index.
    ''' </summary>
    ''' <param name="index">The index to lookup.</param>
    ''' <returns>A filepath found under this index.</returns>
    Default Public Property Item(index As Integer) As String Implements IList(Of String).Item
        Get
            Return paths(index)
        End Get
        Set(filepath As String)
            paths(index) = Path.GetFullPath(filepath)
        End Set
    End Property

    ''' <summary>
    ''' Gets the number of filepaths in this playlist.
    ''' </summary>
    ''' <returns>The size of <c>paths</c></returns>
    Public ReadOnly Property Count As Integer Implements ICollection(Of String).Count
        Get
            Return paths.Count
        End Get
    End Property

    ''' <summary>
    ''' Returns whether this collection is ReadOnly.
    ''' </summary>
    ''' <remarks>It is not.</remarks>
    ''' <returns><c>True</c> if this collection is ReadOnly and <c>False</c> otherwise.</returns>
    Public ReadOnly Property IsReadOnly As Boolean Implements ICollection(Of String).IsReadOnly
        Get
            Return False
        End Get
    End Property

    ''' <summary>
    ''' An interface which manages playlist extensions.
    ''' </summary>
    Public Interface Extension
        
        ''' <summary>
        ''' Captures the paths from this stream and inserts them into a <c>String()</c>.
        ''' </summary>
        ''' <param name="stream">The input stream for this file.</param>
        Function Decode(ByRef stream As StreamReader) As String()

        ''' <summary>
        ''' Pushes the paths from the <paramref name="paths"/> array into this stream.
        ''' </summary>
        ''' <param name="stream">The output stream for this file.</param>
        ''' <param name="paths">The array of filepaths to encode.</param>
        Sub Encode(ByRef stream As StreamWriter, ByRef paths As String())

    End Interface

    ''' <summary>
    ''' Uses reflection to compile the dictionary of file extensions at runtime.
    ''' </summary>
    Shared Sub New()
        Dim template As Type = GetType(Extension)
        For Each dataType As Type In template.Assembly.GetTypes()
            If (dataType.IsClass() AndAlso dataType.GetInterfaces.Contains(template))
                '' valid class type
                extensions.Add("." & dataType.Name.ToUpper(), Activator.CreateInstance(dataType))
            End If
        Next
    End Sub

    ''' <summary>
    ''' Default constructor.
    ''' </summary>
    ''' <remarks>Does nothing.</remarks>
    Sub New() : End Sub

    ''' <summary>
    ''' Constructs a playlist from a playlist file.
    ''' </summary>
    ''' <param name="filepath">The path of the playlist file.</param>
    Sub New(ByVal filepath As String)
        Load(filepath)
    End Sub

    ''' <summary>
    ''' Constructs a playlist from an <c>IEnumerable</c> of filepaths.
    ''' </summary>
    ''' <param name="enumerator">An <c>IEnumerable</c> of filepaths.</param>
    Sub New(ByVal enumerator As IEnumerable(Of String))
        Me.New(enumerator.ToArray)
    End Sub

    ''' <summary>
    ''' Constructs a playlist from an array of filepaths.
    ''' </summary>
    ''' <param name="paths">An array of filepaths.</param>
    Sub New(ByVal paths As String())
        For Each path In paths
            Add(path)
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
            Dim dir As String = Path.GetDirectoryName(filepath)
            For Each path As String In extensions(ext).Decode(input)
                paths.Add(Playlist.ToAbsolute(dir, path))
            Next
        End Using
    End Sub
    
    ''' <summary>
    ''' Encodes the contents of the this playlist into a valid playlist file.
    ''' </summary>
    ''' <param name="filepath">The path of the playlist file.</param>
    ''' <exception cref="KeyNotFoundException">Thrown when the file extension for <paramref name="filepath"/> is not supported.</exception>
    Public Sub Save(ByVal filepath As String, Optional ByVal relative As Boolean = False)
        Dim outputPaths As String() = paths.ToArray()
        If (relative)
            '' convert the paths to be relative to 'filepath'
            Dim dir As String = Path.GetDirectoryName(filepath)
            For i As Integer = 0 To (outputPaths.Length - 1)
                outputPaths(i) = Playlist.ToRelative(dir, outputPaths(i))
            Next
        End If
        Using output As StreamWriter = My.Computer.FileSystem.OpenTextFileWriter(filepath, false)
            Dim ext As String = Path.GetExtension(filepath)
            If (Not extensions.ContainsKey(ext)) Then Throw New _
                    KeyNotFoundException(String.Format("Unknown playlist file extension {0}.", ext))
            extensions(ext).Encode(output, outputPaths)
        End Using
    End Sub

    ''' <summary>
    ''' Converts a filepath to an absolute representation.
    ''' </summary>
    ''' <param name="local">The local directory.</param>
    ''' <param name="filepath">The filepath to convert.</param>
    Private Shared Function ToAbsolute(ByVal local As String, ByVal filepath As String) As String
        Dim dir As String = Directory.GetCurrentDirectory()
        Directory.SetCurrentDirectory(local)
        filepath = Path.GetFullPath(filepath)
        Directory.SetCurrentDirectory(dir)
        Return filepath
    End Function

    ''' <summary>
    ''' Converts a filepath to a relative representation.
    ''' </summary>
    ''' <param name="local">The local directory.</param>
    ''' <param name="filepath">The filepath to convert.</param>
    Private Shared Function ToRelative(ByVal local As String, ByVal filepath As String) As String
        Dim Static delimiter As String = Path.DirectorySeparatorChar
        Dim dir As String = Directory.GetCurrentDirectory()
        Directory.SetCurrentDirectory(local)
        Dim backtracks As String = ""
        Do
            If (local Is Nothing) Then Exit Do
            If (filepath.Contains(local))
                filepath = filepath.Replace(local, backtracks)
                If ((filepath IsNot Nothing) AndAlso (filepath(0) = delimiter)) Then _
                        filepath = filepath.Remove(0, 1)
                Exit Do
            End If
            local = Path.GetDirectoryName(local)
            backtracks = ".." & delimiter & backtracks
        Loop
        Directory.SetCurrentDirectory(dir)
        Return filepath
    End Function

    ''' <summary>
    ''' Implements the iterator for the playlist.
    ''' </summary>
    ''' <returns>An <c>IEnumerator</c> of playlist sound file paths.</returns>
    Public Iterator Function GetEnumerator() As IEnumerator(Of String) Implements IEnumerable(Of String).GetEnumerator
        For Each path In paths
            Yield path
        Next
    End Function

    ''' <summary>
    ''' Implements the iterator for the playlist.
    ''' </summary>
    ''' <remarks>Required by interface.</remarks>
    ''' <returns>An <c>IEnumerator</c> of playlist sound file paths.</returns>
    Private Function GetEnumeratorB() As IEnumerator Implements IEnumerable.GetEnumerator
        Return GetEnumerator()
    End Function

    ''' <summary>
    ''' Searches for the index of a specific filepath in the playlist.
    ''' </summary>
    ''' <param name="filepath"></param>
    ''' <returns></returns>
    Public Function IndexOf(filepath As String) As Integer Implements IList(Of String).IndexOf
        Return paths.IndexOf(Path.GetFullPath(filepath))
    End Function

    ''' <summary>
    ''' Inserts a filepath into a specific part of the playlist.
    ''' </summary>
    ''' <param name="index">The index to insert the <paramref name="filepath"/> into.</param>
    ''' <param name="filepath">The filepath of the sound file to insert.</param>
    Public Sub Insert(index As Integer, filepath As String) Implements IList(Of String).Insert
        paths.Insert(index, Path.GetFullPath(filepath))
    End Sub

    ''' <summary>
    ''' Removes a filepath from a specific part of the playlist.
    ''' </summary>
    ''' <param name="index">The index to remove.</param>
    Public Sub RemoveAt(index As Integer) Implements IList(Of String).RemoveAt
        paths.RemoveAt(index)
    End Sub

    ''' <summary>
    ''' Adds a filepath to this playlist.
    ''' </summary>
    ''' <param name="filepath">The filepath to add.</param>
    Public Sub Add(filepath As String) Implements ICollection(Of String).Add
        paths.Add(Path.GetFullPath(filepath))
    End Sub

    ''' <summary>
    ''' Clears the playlist of its current filepaths.
    ''' </summary>
    Public Sub Clear() Implements ICollection(Of String).Clear
        paths.Clear()
    End Sub

    ''' <summary>
    ''' Returns whether the playlist contains a filepath.
    ''' </summary>
    ''' <param name="filepath">The filepath to search for.</param>
    ''' <returns><c>True</c> if the filepath exists and <c>False</c> otherwise.</returns>
    Public Function Contains(filepath As String) As Boolean Implements ICollection(Of String).Contains
        Return paths.Contains(Path.GetFullPath(filepath))
    End Function

    ''' <summary>
    ''' Converts the playlist into an array.
    ''' </summary>
    ''' <param name="array">The array to copy the playlist filepaths to.</param>
    ''' <param name="arrayIndex">The starting index.</param>
    Public Sub CopyTo(array As String(), arrayIndex As Integer) Implements ICollection(Of String).CopyTo
        For Each filepath In paths
            array(arrayIndex) = filepath
            arrayIndex += 1
        Next
    End Sub

    ''' <summary>
    ''' Removes a specific filepath from the playlist.
    ''' </summary>
    ''' <param name="filepath">The filepath to remove.</param>
    Public Function Remove(filepath As String) As Boolean Implements ICollection(Of String).Remove
        Return paths.Remove(Path.GetFullPath(filepath))
    End Function

End Class