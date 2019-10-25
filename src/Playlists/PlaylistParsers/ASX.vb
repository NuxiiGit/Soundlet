Imports System.IO.StreamReader
Imports System.IO.StreamWriter
Imports System.IO
Imports System.Xml.XmlReader
Imports System.Xml.XmlWriter
Imports System.Xml

''' <summary>
''' Decodes and encodes .asx files.
''' </summary>
Public Class ASX
    Implements Playlist.Extension

    ''' <summary>
    ''' <see cref="Playlist.Extension.Decode(ByRef StreamReader)"/>
    ''' </summary>
    Public Function Decode(ByRef stream As StreamReader) As String() Implements Playlist.Extension.Decode
        Dim paths As List(Of String) = New List(Of String)
        Using xml As XmlReader = XmlReader.Create(stream)
            If xml.ReadToFollowing("ASX")
                While xml.ReadToFollowing("Entry")
                    If Not (xml.ReadToDescendant("Ref") AndAlso xml.MoveToFirstAttribute()) Then _
                            Throw New IOException("Malformed file structure.")
                    paths.Add(xml.Value)
                End While
            End if
        End Using
        Return paths.ToArray()
    End Function

    ''' <summary>
    ''' <see cref="Playlist.Extension.Encode(ByRef StreamWriter, ByRef String())"/>
    ''' </summary>
    Public Sub Encode(ByRef stream As StreamWriter, ByRef paths As String()) Implements Playlist.Extension.Encode
        stream.WriteLine("<ASX version = ""3.0"" >")
        For Each record As String In paths
            stream.WriteLine("<Entry><Ref href = """ & record & """/></Entry>")
        Next
        stream.WriteLine("</ASX>")
    End Sub

End Class
