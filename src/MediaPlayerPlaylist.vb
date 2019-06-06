Imports System.IO.StreamReader
Imports System.IO.StreamWriter
Imports System.IO
Imports System.Xml.XmlReader
Imports System.Xml.XmlWriter
Imports System.Xml

''' <summary>
''' A module which contains proceedures for managing Media Player Classic playlist formats.
''' </summary>
Class MediaPlayerPlaylist
    Implements IEnumerable

    ''' <summary>
    ''' Maintains a relationship between file extensions and thei decoder/encoder function pointers.
    ''' </summary>
    Private Shared fileExtensions As Dictionary(Of String, Tuple(Of ExtensionPtrIn, ExtensionPtrOut)) _
        = New Dictionary(Of String, Tuple(Of ExtensionPtrIn, ExtensionPtrOut))
    
    ''' <summary>
    ''' Maintains a record of filepaths for this playlist.
    ''' </summary>
    Private paths As List(Of String) = New List(Of String)
    
    ''' <summary>
    ''' Captures the paths from this stream and inserts them into the <paramref name="paths"/> list.
    ''' </summary>
    ''' <param name="stream">The input stream for this file.</param>
    ''' <param name="paths">The output list to insert path names.</param>
    Private Delegate Sub ExtensionPtrIn(ByRef stream As StreamReader, ByRef paths As List(Of String))

    ''' <summary>
    ''' Pushes the paths from the <paramref name="paths"/> array into this stream.
    ''' </summary>
    ''' <param name="stream"></param>
    ''' <param name="paths"></param>
    Private Delegate Sub ExtensionPtrOut(ByRef stream As StreamWriter, ByRef paths As List(Of String))

    ''' <summary>
    ''' Adds file extension types at runtime.
    ''' </summary>
    Shared Sub New()
        '' .mpcpl files
        fileExtensions.Add(".mpcpl", Tuple.Create(Of ExtensionPtrIn, ExtensionPtrOut)(
                Sub(Byref stream As StreamReader, ByRef paths As List(Of String))
                    If (stream.ReadLine().Trim(" ") <> "MPCPLAYLIST") Then Throw New IOException("Invalid file format.")
                    While (Not stream.EndOfStream)
                        Dim record As String() = stream.ReadLine().Split(","c)
                        If (record.Length <> 3) Then Throw New IOException("Malformed file structure.")
                        If (record(1) = "filename") Then paths.Add(record(2))
                    End While
                End Sub,
                Sub(Byref stream As StreamWriter, ByRef paths As List(Of String))
                    stream.WriteLine("MPCPLAYLIST")
                    Dim i As Integer = 1
                    For Each path In paths
                        stream.WriteLine(i & ",type,0")
                        stream.WriteLine(i & ",filename," & path)
                        i += 1
                    Next
                End Sub))
        '' .pls files
        fileExtensions.Add(".pls", Tuple.Create(Of ExtensionPtrIn, ExtensionPtrOut)(
                Sub(Byref stream As StreamReader, ByRef paths As List(Of String))
                    While (Not stream.EndOfStream)
                        If (stream.ReadLine().Trim(" ") = "[playlist]") Then GoTo decode
                    End While
                    Throw New IOException("Invalid file format.")
                    decode:
                    While (Not stream.EndOfStream)
                        Dim record As String() = stream.ReadLine().Split("="c)
                        If (record.Length <> 2) Then Throw New IOException("Malformed file structure.")
                        If (record(0) Like "File*") Then paths.Add(record(1))
                    End While
                End Sub,
                Sub(Byref stream As StreamWriter, ByRef paths As List(Of String))
                    stream.WriteLine("[playlist]")
                    Dim i As Integer = 1
                    For Each path In paths
                        stream.WriteLine("File" & i & "=" & path)
                        i += 1
                    Next
                    stream.WriteLine("NumberOfEntries=" & i)
                    stream.WriteLine("Version=1")
                End Sub))
        '' .m3u files
        fileExtensions.Add(".m3u", Tuple.Create(Of ExtensionPtrIn, ExtensionPtrOut)(
                Sub(Byref stream As StreamReader, ByRef paths As List(Of String))
                    While (Not stream.EndOfStream)
                        paths.Add(stream.ReadLine())
                    End While
                End Sub,
                Sub(Byref stream As StreamWriter, ByRef paths As List(Of String))
                    For Each path In paths
                        stream.WriteLine(path)
                    Next
                End Sub))
        '' .asx files
        fileExtensions.Add(".asx", Tuple.Create(Of ExtensionPtrIn, ExtensionPtrOut)(
                Sub(Byref stream As StreamReader, ByRef paths As List(Of String))
                    Using xml As XmlReader = XmlReader.Create(stream)
                        If (Not xml.ReadToFollowing("ASX")) Then Throw New IOException("Invalid file format.")
                        While (xml.ReadToFollowing("Entry"))
                            If (Not (xml.ReadToDescendant("Ref") _
                                    AndAlso xml.MoveToFirstAttribute())) Then Throw New IOException("Malformed file structure.")
                            paths.Add(xml.Value)
                        End While
                    End Using
                End Sub,
                Sub(Byref stream As StreamWriter, ByRef paths As List(Of String))
                    stream.WriteLine("<ASX version = ""3.0"" >")
                    For Each path In paths
                        stream.WriteLine("<Entry><Ref href = """ & path & """/></Entry>")
                    Next
                    stream.WriteLine("</ASX>")
                End Sub))
    End Sub

    ''' <summary>
    ''' Constructs a new, empty playlist.
    ''' </summary>
    Sub New() : End Sub
    
    ''' <summary>
    ''' Decodes the contents of a valid playlist file.
    ''' </summary>
    ''' <param name="filepath">The path of the playlist file.</param>
    ''' <exception cref="IOException">Thrown when there was an error loading the file contents.</exception>
    ''' <exception cref="ArgumentException">Thrown when the file extension for <paramref name="filepath"/> is not supported.</exception>
    Public Sub Load(ByVal filepath As String)
        If (Not My.Computer.FileSystem.FileExists(filepath)) Then Throw New IOException("Illegal filepath.")
        paths.Clear()
        Using input As StreamReader = My.Computer.FileSystem.OpenTextFileReader(filepath)
            If input.EndOfStream Then Throw New IOException("File cannot be empty.")
            '' compile paths
            Dim ext As String = Path.GetExtension(filepath)
            If (Not fileExtensions.ContainsKey(ext)) Then Throw New ArgumentException("Unknown file extension '" & ext & "'.")
            Call (fileExtensions(ext).Item1)(input, paths)
        End Using
        Dim dir As String = Directory.GetCurrentDirectory()
        Directory.SetCurrentDirectory(Path.GetDirectoryName(filepath))
        For i As Integer = 0 To (paths.Count - 1)
            paths(i) = Path.GetFullPath(paths(i))
        Next
        Directory.SetCurrentDirectory(dir)
    End Sub
    
    ''' <summary>
    ''' Encodes the contents of the playlist into a valid playlist file.
    ''' </summary>
    ''' <param name="filepath">The path of the playlist file.</param>
    ''' <exception cref="ArgumentException">Thrown when the file extension for <paramref name="filepath"/> is not supported.</exception>
    Public Sub Save(ByVal filepath As String, Optional ByVal relative As Boolean = False)
        If (relative)
            '' convert the paths to be relative to 'filepath'
            Dim delimiter As String = Path.DirectorySeparatorChar
            Dim dir As String = Directory.GetCurrentDirectory()
            Directory.SetCurrentDirectory(Path.GetDirectoryName(filepath))
            For i As Integer = 0 To (paths.Count - 1)
                Dim targetPath As String = Path.GetFullPath(paths(i))
                Dim localPath As String = filepath
                Dim reversals As String = ""
                Do
                    localPath = Path.GetDirectoryName(localPath)
                    If (localPath Is Nothing) Then Exit Do
                    If (targetPath.Contains(localPath))
                        targetPath = targetPath.replace(localPath, reversals)
                        If ((targetPath IsNot Nothing) AndAlso (targetPath(0) = delimiter)) Then targetPath = targetPath.Remove(0, 1)
                        Exit Do
                    End If
                    reversals = ".." & delimiter & reversals
                Loop
                paths(i) = targetPath
            Next
            Directory.SetCurrentDirectory(dir)
        End If
        Using output As StreamWriter = My.Computer.FileSystem.OpenTextFileWriter(filepath, false)
            Dim ext As String = Path.GetExtension(filepath)
            If (Not fileExtensions.ContainsKey(ext)) Then Throw New ArgumentException("Unknown file extension '" & ext & "'.")
            Call (fileExtensions(ext).Item2)(output, paths)
        End Using
    End Sub

    ''' <summary>
    ''' Implements the <c>IEnumerable</c> interface.
    ''' </summary>
    ''' <returns>An iterator containing paths for this playlist.</returns>
    Public Iterator Function GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        For Each path In paths
            Yield path
        Next
    End Function

    ''' <summary>
    ''' Returns the string representation of this playlist.
    ''' </summary>
    ''' <returns>A sequence of sound filepaths, separated by commas.</returns>
    Public Overrides Function ToString() As String
        Dim str As String = ""
        For Each path In paths
            If (str <> "") Then str = str & ", "
            str = str & path
        Next
        Return str
    End Function

End Class