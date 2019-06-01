Imports System.IO.StreamReader
Imports System.IO.StreamWriter
Imports System.IO

''' <summary>
''' A module which contains proceedures for managing Media Player Classic playlist formats.
''' </summary>
Module MediaPlayerPlaylist

    Private Const MPC_HEADER As String = "MPCPLAYLIST"
    Private fileExtensions As Dictionary(Of String, ExtensionPtr) = New Dictionary(Of String, ExtensionPtr)
    Private Delegate Sub ExtensionPtr(ByRef stream As StreamReader, ByRef paths As List(Of String))

    Sub New()
        fileExtensions.Add(".mpcpl", AddressOf LoadFormatMPCPL)
    End Sub

    Public Function LoadFormat(ByVal filepath As String) As String()
        If (Not My.Computer.FileSystem.FileExists(filepath)) Then Throw New ArgumentException("Illegal filepath.")
        Dim input As StreamReader = My.Computer.FileSystem.OpenTextFileReader(filepath)
        If input.EndOfStream Then Throw New ArgumentException("File cannot be empty.")
        '' compile paths
        Dim paths As List(Of String) = New List(Of String)
        Dim ext As String = Path.GetExtension(filepath)
        If (Not fileExtensions.ContainsKey(ext)) Then Throw New ArgumentException("Unknown file extension '" & ext & "'.")
        fileExtensions(ext)(input, paths)
        input.Close()
        input.Dispose()
        Return paths.ToArray()
    End Function

    Private Sub LoadFormatMPCPL(Byref stream As StreamReader, ByRef paths As List(Of String))
        If (stream.ReadLine() <> MPC_HEADER) Then Throw New ArgumentException("Invalid file format.")
        While (Not stream.EndOfStream)
            Dim record As String() = stream.ReadLine().Split(","c)
            If (record.Length <> 3) Then Throw New ArgumentException("Malformed file structure.")
            If (record(1) = "filename") Then
                paths.Add(record(2))
            End If
        End While
    End Sub

End Module
