Imports System.IO.StreamReader
Imports System.IO.StreamWriter
Imports System.IO

Public Class PLS
    Implements Playlist.Extension

    Private Const HEADER As String = "[playlist]"

    Public Function Decode(ByRef stream As StreamReader) As String() Implements Playlist.Extension.Decode
        While Not stream.EndOfStream
            If stream.ReadLine().Trim(" ") = HEADER Then GoTo decode
        End While
        Throw New IOException("Missing playlist header.")
        decode:
        Dim paths As List(Of String) = New List(Of String)
        While Not stream.EndOfStream
            Dim record As String() = stream.ReadLine().Split("="c)
            If record.Length <> 2 Then Throw New IOException("Malformed file structure.")
            If record(0) Like "File*" Then paths.Add(record(1))
        End While
        Return paths.ToArray()
    End Function

    Public Sub Encode(ByRef stream As StreamWriter, ByRef paths As String()) Implements Playlist.Extension.Encode
        stream.WriteLine(HEADER)
        Dim i As Integer = 1
        For Each record As String In paths
            stream.WriteLine("File" & i & "=" & record)
            i += 1
        Next
        stream.WriteLine("NumberOfEntries=" & i)
        stream.WriteLine("Version=3")
    End Sub

End Class
