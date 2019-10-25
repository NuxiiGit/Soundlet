﻿Imports System.IO.StreamReader
Imports System.IO.StreamWriter
Imports System.IO

''' <summary>
''' Decodes and encodes .m3u files.
''' </summary>
Public Class M3U
    Implements Playlist.Extension

    ''' <summary>
    ''' <see cref="Playlist.Extension.Decode(ByRef StreamReader)"/>
    ''' </summary>
    Public Function Decode(ByRef stream As StreamReader) As String() Implements Playlist.Extension.Decode
        Dim paths As List(Of String) = New List(Of String)
        While Not stream.EndOfStream
            paths.add(stream.ReadLine())
        End While
        Return paths.ToArray()
    End Function

    ''' <summary>
    ''' <see cref="Playlist.Extension.Encode(ByRef StreamWriter, ByRef String())"/>
    ''' </summary>
    Public Sub Encode(ByRef stream As StreamWriter, ByRef paths As String()) Implements Playlist.Extension.Encode
        For Each record As String In paths
            stream.WriteLine(record)
        Next
    End Sub

End Class
