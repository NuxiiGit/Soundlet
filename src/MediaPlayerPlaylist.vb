Imports System.IO.StreamReader
Imports System.IO.StreamWriter
Imports System.IO

''' <summary>
''' A module which contains proceedures for managing Media Player Classic playlist formats.
''' </summary>
Module MediaPlayerPlaylist

    Private Const MPC_HEADER As String = "MPCPLAYLIST"

    Public Function LoadFormatMPCPL(Byval filepath As String) As String()
        If (Not My.Computer.FileSystem.FileExists(filepath)) Then Throw New ArgumentException("Illegal filepath.")
        Dim input As StreamReader = My.Computer.FileSystem.OpenTextFileReader(filepath)
        If input.EndOfStream Then Throw New ArgumentException("File cannot be empty.")
        If (input.ReadLine() <> MPC_HEADER) Then Throw New ArgumentException("Invalid file format.")
        '' compile paths
        Dim paths As ArrayList = New ArrayList()
        While (Not input.EndOfStream)
            Dim record As String() = input.ReadLine().Split(","c)
            If (record.Length <> 3) Then Throw New ArgumentException("Malformed file structure.")
            If (record(1) = "filename") Then
                paths.Add(2)
            End If
        End While
        input.Close()
        input.Dispose()
        Return paths.ToArray()
    End Function

End Module
