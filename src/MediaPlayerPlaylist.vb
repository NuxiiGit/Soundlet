Imports System.IO.StreamReader
Imports System.IO.StreamWriter
Imports System.IO
Imports System.Xml.XmlReader
Imports System.Xml.XmlWriter
Imports System.Xml

''' <summary>
''' A module which contains proceedures for managing Media Player Classic playlist formats.
''' </summary>
Module MediaPlayerPlaylist

    Private Const MPCPL_HEADER As String = "MPCPLAYLIST"
    Private Const PLS_HEADER As String = "[playlist]"

    ''' <summary>
    ''' Maintains a relationship between file extensions and thei decoder/encoder function pointers.
    ''' </summary>
    Private fileExtensions As Dictionary(Of String, Pair(Of ExtensionPtrIn, ExtensionPtrOut)) _
        = New Dictionary(Of String, Pair(Of ExtensionPtrIn, ExtensionPtrOut))

    ''' <summary>
    ''' A private structure used to store a pair of extension function pointers
    ''' </summary>
    Private Structure Pair(Of T, S)

        Public ReadOnly left As T
        Public ReadOnly right As S
        
        Public Sub New(ByRef left As T, ByRef right As S)
            Me.left = left
            Me.right = right
        End Sub

    End Structure

    ''' <summary>
    ''' Captures the paths from this stream and inserts them into the <paramref name="paths"/> list.
    ''' </summary>
    ''' <param name="stream">The input stream for this file.</param>
    ''' <param name="paths">The output list to insert path names.</param>
    Public Delegate Sub ExtensionPtrIn(ByRef stream As StreamReader, ByRef paths As List(Of String))

    ''' <summary>
    ''' Pushes the paths from the <paramref name="paths"/> array into this stream.
    ''' </summary>
    ''' <param name="stream"></param>
    ''' <param name="paths"></param>
    Public Delegate Sub ExtensionPtrOut(ByRef stream As StreamWriter, ByRef paths As String())

    ''' <summary>
    ''' Adds default filepaths at runtime.
    ''' </summary>
    Sub New()
        AddExtension(".mpcpl", AddressOf LoadFormatMPCPL, AddressOf SaveFormatMPCPL)
        AddExtension(".pls", AddressOf LoadFormatPLS, AddressOf SaveFormatPLS)
        AddExtension(".m3u", AddressOf LoadFormatM3U, AddressOf SaveFormatM3U)
        AddExtension(".asx", AddressOf LoadFormatASX, AddressOf SaveFormatASX)
    End Sub

    ''' <summary>
    ''' Adds a new file extension.
    ''' </summary>
    ''' <param name="read">The address of the function pointer to decode this file extension. <see cref="ExtensionPtrIn"/></param>
    ''' <param name="write">The address of the function pointer to encode this file extension. <see cref="ExtensionPtrOut"/></param>
    Public Sub AddExtension(ByVal ext As String, ByRef read As ExtensionPtrIn, ByRef write As ExtensionPtrOut)
        fileExtensions.Add(ext, New Pair(Of ExtensionPtrIn, ExtensionPtrOut)(read, write))
    End Sub

    ''' <summary>
    ''' Removes a file extension
    ''' </summary>
    ''' <exception cref="ArgumentException">Thrown when the extension does not exist.</exception>
    Public Sub RemoveExtension(ByVal ext As String)
        If (Not fileExtensions.ContainsKey(ext)) Then Throw New ArgumentException("Extension does not exist.")
        fileExtensions.Remove(ext)
    End Sub

    ''' <summary>
    ''' returns whether a file extension exists
    ''' </summary>
    ''' <returns><c>True</c> of the extension exists and <c>False</c> otherwise.</returns>
    Public Function ContainsExtension(ByVal ext As String) As Boolean
        Return fileExtensions.ContainsKey(ext)
    End Function

    ''' <summary>
    ''' Decodes the contents of a valid playlist file and returns an array of sound filepaths within.
    ''' </summary>
    ''' <param name="filepath">The path of the playlist file.</param>
    ''' <returns>A <c>String()</c> of filepaths.</returns>
    ''' <exception cref="IOException">Thrown when there was an error loading the file contents.</exception>
    ''' <exception cref="ArgumentException">Thrown when the file extension for <paramref name="filepath"/> is not supported.</exception>
    Public Function LoadFormat(ByVal filepath As String) As String()
        If (Not My.Computer.FileSystem.FileExists(filepath)) Then Throw New IOException("Illegal filepath.")
        Dim paths As List(Of String) = New List(Of String)
        Using input As StreamReader = My.Computer.FileSystem.OpenTextFileReader(filepath)
            If input.EndOfStream Then Throw New IOException("File cannot be empty.")
            '' compile paths
            Dim ext As String = Path.GetExtension(filepath)
            If (Not fileExtensions.ContainsKey(ext)) Then Throw New ArgumentException("Unknown file extension '" & ext & "'.")
            fileExtensions(ext).left(input, paths)
        End Using
        Dim dir As String = Directory.GetCurrentDirectory()
        Directory.SetCurrentDirectory(Path.GetDirectoryName(filepath))
        For i As Integer = 0 To (paths.Count - 1)
            paths(i) = Path.GetFullPath(paths(i))
        Next
        Directory.SetCurrentDirectory(dir)
        Return paths.ToArray()
    End Function

    ''' <summary>
    ''' Encodes the contents of the <paramref name="paths"/> array into a valid playlist file.
    ''' </summary>
    ''' <param name="filepath">The path of the playlist file.</param>
    ''' <exception cref="ArgumentException">Thrown when the file extension for <paramref name="filepath"/> is not supported.</exception>
    Public Sub SaveFormat(ByVal filepath As String, ByVal paths As String(), Optional ByVal relative As Boolean = False)
        If (relative)
            '' convert the paths to be relative to 'filepath'
            Dim delimiter As String = Path.DirectorySeparatorChar
            Dim dir As String = Directory.GetCurrentDirectory()
            Directory.SetCurrentDirectory(Path.GetDirectoryName(filepath))
            For i As Integer = 0 To (paths.Length - 1)
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
            fileExtensions(ext).right(output, paths)
        End Using
    End Sub

    ''' <summary>
    ''' <see cref="ExtensionPtrIn"/>
    ''' </summary>
    ''' <exception cref="IOException">Thrown when there was an error loading the file contents.</exception>
    Private Sub LoadFormatMPCPL(Byref stream As StreamReader, ByRef paths As List(Of String))
        If (stream.ReadLine().Trim(" ") <> MPCPL_HEADER) Then Throw New IOException("Invalid file format.")
        While (Not stream.EndOfStream)
            Dim record As String() = stream.ReadLine().Split(","c)
            If (record.Length <> 3) Then Throw New IOException("Malformed file structure.")
            If (record(1) = "filename") Then paths.Add(record(2))
        End While
    End Sub

    ''' <summary>
    ''' <see cref="ExtensionPtrOut"/>
    ''' </summary>
    Private Sub SaveFormatMPCPL(Byref stream As StreamWriter, ByRef paths As String())
        stream.WriteLine(MPCPL_HEADER)
        Dim i As Integer = 1
        For Each path In paths
            stream.WriteLine(i & ",type,0")
            stream.WriteLine(i & ",filename," & path)
            i += 1
        Next
    End Sub

    ''' <summary>
    ''' <see cref="ExtensionPtrIn"/>
    ''' </summary>
    ''' <exception cref="IOException">Thrown when there was an error loading the file contents.</exception>
    Private Sub LoadFormatPLS(Byref stream As StreamReader, ByRef paths As List(Of String))
        While (Not stream.EndOfStream)
            If (stream.ReadLine().Trim(" ") = PLS_HEADER) Then GoTo decode
        End While
        Throw New IOException("Invalid file format.")
        decode:
        While (Not stream.EndOfStream)
            Dim record As String() = stream.ReadLine().Split("="c)
            If (record.Length <> 2) Then Throw New IOException("Malformed file structure.")
            If (record(0) Like "File*") Then paths.Add(record(1))
        End While
    End Sub

    ''' <summary>
    ''' <see cref="ExtensionPtrOut"/>
    ''' </summary>
    Private Sub SaveFormatPLS(Byref stream As StreamWriter, ByRef paths As String())
        stream.WriteLine(PLS_HEADER)
        Dim i As Integer = 1
        For Each path In paths
            stream.WriteLine("File" & i & "=" & path)
            i += 1
        Next
        stream.WriteLine("NumberOfEntries=" & i)
        stream.WriteLine("Version=1")
    End Sub

    ''' <summary>
    ''' <see cref="ExtensionPtrIn"/>
    ''' </summary>
    Private Sub LoadFormatM3U(Byref stream As StreamReader, ByRef paths As List(Of String))
        While (Not stream.EndOfStream)
            paths.Add(stream.ReadLine())
        End While
    End Sub

    ''' <summary>
    ''' <see cref="ExtensionPtrOut"/>
    ''' </summary>
    Private Sub SaveFormatM3U(Byref stream As StreamWriter, ByRef paths As String())
        For Each path In paths
            stream.WriteLine(path)
        Next
    End Sub

    ''' <summary>
    ''' <see cref="ExtensionPtrIn"/>
    ''' </summary>
    ''' <exception cref="IOException">Thrown when there was an error loading the file contents.</exception>
    Private Sub LoadFormatASX(Byref stream As StreamReader, ByRef paths As List(Of String))
        Using xml As XmlReader = XmlReader.Create(stream)
            If (Not xml.ReadToFollowing("ASX")) Then Throw New IOException("Invalid file format.")
            While (xml.ReadToFollowing("Entry"))
                If (Not (xml.ReadToDescendant("Ref") _
                        AndAlso xml.MoveToFirstAttribute())) Then Throw New IOException("Malformed file structure.")
                paths.Add(xml.Value)
            End While
        End Using
    End Sub

    ''' <summary>
    ''' <see cref="ExtensionPtrOut"/>
    ''' </summary>
    Private Sub SaveFormatASX(Byref stream As StreamWriter, ByRef paths As String())
        stream.WriteLine("<ASX version = ""3.0"" >")
        For Each path In paths
            stream.WriteLine("<Entry><Ref href = """ & path & """/></Entry>")
        Next
        stream.WriteLine("</ASX>")
    End Sub    

End Module