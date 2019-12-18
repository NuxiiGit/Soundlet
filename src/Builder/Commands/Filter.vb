Imports Id3

Imports System.IO

Public Module Filter
    
    ''' <summary>
    ''' Filters elements from a list.
    ''' </summary>
    ''' <param name="list">The playlist to filter</param>
    ''' <param name="tagKind">The tag to inspect.</param>
    ''' <param name="filterKind">The sort of filter method to perform.</param>
    ''' <param name="predicates">The values to use to check for satisfaction.</param>
    Public Sub Filter(ByRef list As Playlist, ByVal tagKind As String, ByVal filterKind As String, ByVal predicates As String())
        Console.WriteLine("Filtering by {0}...", tagKind)
        Dim i As Integer = 0
        While i < list.Count
            Dim keep As Boolean = False
            Dim record As String = list(i)
            Select Path.GetExtension(record).ToLower()
            Case ".mp3"
                Using mp3 As New Mp3(record)
                    For Each tag As Id3Tag In mp3.GetAllTags()
                        Dim elements As String()
                        Select tagKind
                        Case "genre"
                            elements = tag.Genre.ToString().Split("/"c)
                        Case "artist"
                            elements = tag.Artists.ToString().Split("/"c)
                        Case Else
                            Throw New ArgumentException("Unknown tag '{0}'", tagKind)
                        End Select
                        Select filterKind
                        Case "all"
                            keep = predicates.All(Function(ByVal x As String) elements.Contains(x))
                        Case "any"
                            keep = predicates.Any(Function(ByVal x As String) elements.Contains(x))
                        Case "either"
                            keep = False
                            For Each predicate In predicates
                                If elements.Contains(predicate)
                                    If Not keep
                                        keep = True
                                    Else
                                        keep = False
                                        Exit For
                                    End If
                                End If
                            Next
                        Case Else
                            Throw New ArgumentException("Unknown filter kind '{0}'", filterKind)
                        End Select
                        If keep Then Exit for
                    Next
                End Using
            End Select
            If Not keep
                list.RemoveAt(i)
            Else
                i += 1
            End If
        End While
    End Sub

End Module

Public Class Genre
    Implements PlaylistManager.Builder.ICommand

    Public Sub Execute(ByRef list As Playlist, ByVal modifiers As String(), ByVal params As String()) Implements PlaylistManager.Builder.ICommand.Execute
        Dim filterKind As String = If(modifiers.Length > 0, modifiers(0), "all")
        Filter.Filter(list, "genre", filterKind, params)
    End Sub

End Class

Public Class Artist
    Implements PlaylistManager.Builder.ICommand

    Public Sub Execute(ByRef list As Playlist, ByVal modifiers As String(), ByVal params As String()) Implements PlaylistManager.Builder.ICommand.Execute
        Dim filterKind As String = If(modifiers.Length > 0, modifiers(0), "any")
        Filter.Filter(list, "artist", filterKind, params)
    End Sub

End Class