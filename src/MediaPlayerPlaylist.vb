Imports System.IO.StreamReader
Imports System.IO.StreamWriter
Imports System.IO

''' <summary>
''' A module which contains proceedures for managing Media Player Classic playlist formats.
''' </summary>
Module MediaPlayerPlaylist

    Private Const MPC_HEADER As String = "MPCPLAYLIST"

    Public Function LoadFormat(ByVal filepath As String) As String()
        If (Not My.Computer.FileSystem.FileExists(filepath)) Then Throw New ArgumentException("Illegal filepath.")
        Dim input As StreamReader = My.Computer.FileSystem.OpenTextFileReader(filepath)
        Dim output As String() = {}
        LoadFormatMPCPL(input, output)
        input.Close()
        input.Dispose()
        Return output
    End Function

    Private Sub LoadFormatMPCPL(Byref stream As StreamReader, ByRef paths As String())
        If stream.EndOfStream Then Throw New ArgumentException("File cannot be empty.")
        If (stream.ReadLine() <> MPC_HEADER) Then Throw New ArgumentException("Invalid file format.")
        '' compile paths
        Dim pathList As List(Of String) = New List(Of String)
        While (Not stream.EndOfStream)
            Dim record As String() = stream.ReadLine().Split(","c)
            If (record.Length <> 3) Then Throw New ArgumentException("Malformed file structure.")
            If (record(1) = "filename") Then
                pathList.Add(record(2))
            End If
        End While
        paths = pathList.ToArray()
    End Sub

End Module
