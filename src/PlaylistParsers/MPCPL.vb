Imports System.IO.StreamReader
Imports System.IO.StreamWriter
Imports System.IO

''' <summary>
''' Decodes and encodes .mpcpl files.
''' </summary>
Public Class MPCPL
    Implements Playlist.Extension

    Private Const HEADER As String = "MPCPLAYLIST"

    ''' <summary>
    ''' <see cref="Playlist.Extension.Decode(ByRef StreamReader)"/>
    ''' </summary>
    ''' <exception cref="IOException">Thrown if there was a problem decoding the file.</exception>
    Public Function Decode(ByRef stream As StreamReader) As String() Implements Playlist.Extension.Decode
        While (Not stream.EndOfStream)
            If (stream.ReadLine().Trim(" ") = HEADER) Then GoTo decode
        End While
        Throw New IOException("Missing playlist header.")
        decode:
        Dim paths As List(Of String) = New List(Of String)
        While (Not stream.EndOfStream)
            Dim record As String() = stream.ReadLine().Split(","c)
            If (record.Length <> 3) Then Throw New IOException("Malformed file structure.")
            If (record(1) = "filename") Then paths.Add(record(2))
        End While
        Return paths.ToArray()
    End Function

    ''' <summary>
    ''' <see cref="Playlist.Extension.Encode(ByRef StreamWriter, ByRef String())"/>
    ''' </summary>
    Public Sub Encode(ByRef stream As StreamWriter, ByRef paths As String()) Implements Playlist.Extension.Encode
        stream.WriteLine(HEADER)
        Dim i As Integer = 1
        For Each path In paths
            stream.WriteLine(i & ",type,0")
            stream.WriteLine(i & ",filename," & path)
            i += 1
        Next
    End Sub

End Class
