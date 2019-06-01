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
        AddExtension(".mpcpl", AddressOf LoadFormatMPCPL, AddressOf SaveFormatNOTHING)
        AddExtension(".pls", AddressOf LoadFormatPLS, AddressOf SaveFormatNOTHING)
        AddExtension(".m3u", AddressOf LoadFormatM3U, AddressOf SaveFormatNOTHING)
        AddExtension(".asx", AddressOf LoadFormatASX, AddressOf SaveFormatNOTHING)
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
    Public Function LoadFormat(ByVal filepath As String) As String()
        If (Not My.Computer.FileSystem.FileExists(filepath)) Then Throw New IOException("Illegal filepath.")
        Dim input As StreamReader = My.Computer.FileSystem.OpenTextFileReader(filepath)
        If input.EndOfStream Then Throw New IOException("File cannot be empty.")
        '' compile paths
        Dim paths As List(Of String) = New List(Of String)
        Dim ext As String = Path.GetExtension(filepath)
        If (Not fileExtensions.ContainsKey(ext)) Then Throw New IOException("Unknown file extension '" & ext & "'.")
        fileExtensions(ext).left(input, paths)
        input.Close()
        input.Dispose()
        Return paths.ToArray()
    End Function

    ''' <summary>
    ''' <see cref="ExtensionPtrIn"/>
    ''' </summary>
    ''' <exception cref="IOException">Thrown when there was an error loading the file contents.</exception>
    Private Sub LoadFormatMPCPL(Byref stream As StreamReader, ByRef paths As List(Of String))
        If (stream.ReadLine().Trim(" ") <> MPCPL_HEADER) Then Throw New IOException("Invalid file format.")
        While (Not stream.EndOfStream)
            Dim record As String() = stream.ReadLine().Split(","c)
            If (record.Length <> 3) Then Throw New IOException("Malformed file structure.")
            If (record(1) = "filename") Then
                paths.Add(record(2))
            End If
        End While
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
            If (record(0) Like "File*") Then
                paths.Add(record(1))
            End If
        End While
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
    ''' <see cref="ExtensionPtrIn"/>
    ''' </summary>
    Private Sub LoadFormatASX(Byref stream As StreamReader, ByRef paths As List(Of String))
        Dim xml As XmlReader = XmlReader.Create(stream)
        If (Not xml.ReadToFollowing("ASX")) Then Throw New IOException("Invalid file format.")
        While (xml.ReadToFollowing("Entry"))
            If (Not (xml.ReadToDescendant("Ref") _
                    AndAlso xml.MoveToFirstAttribute())) Then Throw New IOException("Malformed file structure.")
            paths.Add(xml.Value)
        End While
    End Sub

    ''' <summary>
    ''' <see cref="ExtensionPtrOut"/>
    ''' </summary>
    Private Sub SaveFormatNOTHING(Byref stream As StreamWriter, ByRef paths As String())
        
    End Sub

End Module
