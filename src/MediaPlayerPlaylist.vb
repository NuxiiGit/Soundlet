Imports System.IO.StreamReader
Imports System.IO.StreamWriter
Imports System.IO

''' <summary>
''' A module which contains proceedures for managing Media Player Classic playlist formats.
''' </summary>
Module MediaPlayerPlaylist

    Private Const MPCPL_HEADER As String = "MPCPLAYLIST"
    Private Const PLS_HEADER As String = "[playlist]"

    ''' <summary>
    ''' Maintains a relation between file extensions and their decoder function.
    ''' </summary>
    Private fileExtensions As Dictionary(Of String, ExtensionPtr) = New Dictionary(Of String, ExtensionPtr)

    ''' <summary>
    ''' Captures the paths from this stream and inserts them into the <paramref name="paths"/> list.
    ''' </summary>
    ''' <param name="stream">The input stream for this file.</param>
    ''' <param name="paths">The output list to insert path names.</param>
    Private Delegate Sub ExtensionPtr(ByRef stream As StreamReader, ByRef paths As List(Of String))

    ''' <summary>
    ''' Adds default filepaths at runtime.
    ''' </summary>
    Sub New()
        fileExtensions.Add(".mpcpl", AddressOf LoadFormatMPCPL)
        fileExtensions.Add(".pls", AddressOf LoadFormatPLS)
    End Sub

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
        fileExtensions(ext)(input, paths)
        input.Close()
        input.Dispose()
        Return paths.ToArray()
    End Function

    ''' <summary>
    ''' <see cref="ExtensionPtr"/>
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
    ''' <see cref="ExtensionPtr"/>
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

End Module
