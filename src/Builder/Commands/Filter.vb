Imports Id3

Imports System.IO

Public Module Filterer

    ''' <summary>
    ''' Filters elements from a list.
    ''' </summary>
    ''' <param name="list">The playlist to filter.</param>
    ''' <param name="p">The predicate to call for each tag.</param>
    Public Sub Filter(ByRef list As Playlist, ByVal p As Func(Of Id3Tag, Boolean))
        Dim i As Integer = 0
        While i < list.Count
            Dim keep As Boolean = False
            Dim record As String = list(i)
            Select Path.GetExtension(record).ToLower()
            Case ".mp3"
                Using mp3 As New Mp3(record)
                    For Each tag As Id3Tag In mp3.GetAllTags()
                        If p(tag)
                            keep = True
                            Exit For
                        End If
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

    Public Function All(ByVal xs As String(), p As Func(Of String, Boolean))
        Return xs.All(p)
    End Function

    Public Function Any(ByVal xs As String(), p As Func(Of String, Boolean))
        Return xs.Any(p)
    End Function

    Public Function Either(ByVal xs As String(), p As Func(Of String, Boolean))
        Dim satisfied As Boolean = False
        For Each x In xs
            If p(x)
                If Not satisfied
                    satisfied = True
                Else
                    satisfied = False
                    Exit For
                End If
            End If
        Next
        Return satisfied
    End Function

    Public Function Only(ByVal xs As String(), p As Func(Of String, Boolean))
        Return xs.Length = 1 AndAlso p(xs(0))
    End Function
    
    '''' <summary>
    '''' Filters elements from a list.
    '''' </summary>
    '''' <param name="list">The playlist to filter</param>
    '''' <param name="tagKind">The tag to inspect.</param>
    '''' <param name="filterKind">The sort of filter method to perform.</param>
    '''' <param name="predicates">The values to use to check for satisfaction.</param>
    'Public Sub Filter(ByRef list As Playlist, ByVal tagKind As String, ByVal filterKind As String, ByVal predicates As String())
    '    predicates = predicates.Select(Function(ByVal x As String) x.ToLower()).ToArray()
    '    Console.WriteLine("Filtering by {0}...", tagKind)
    '    Dim i As Integer = 0
    '    While i < list.Count
    '        Dim keep As Boolean = False
    '        Dim record As String = list(i)
    '        Select Path.GetExtension(record).ToLower()
    '        Case ".mp3"
    '            Using mp3 As New Mp3(record)
    '                For Each tag As Id3Tag In mp3.GetAllTags()
    '                    Dim feature As String
    '                    Select tagKind
    '                    Case "genre"
    '                        feature = tag.Genre.ToString()
    '                    Case "artist"
    '                        feature = tag.Artists.ToString()
    '                    Case "album"
    '                        feature = tag.Album.ToString()
    '                    Case "year"
    '                        feature = tag.Year.ToString()
    '                    Case Else
    '                        Throw New ArgumentException("Unknown tag '{0}'", tagKind)
    '                    End Select
    '                    Dim elements As String() = feature.ToLower().Split("/"c)
    '                    Select filterKind
    '                    Case "all"
    '                        keep = predicates.All(Function(ByVal x As String) elements.Contains(x))
    '                    Case "any"
    '                        keep = predicates.Any(Function(ByVal x As String) elements.Contains(x))
    '                    Case "either"
    '                        keep = False
    '                        For Each predicate In predicates
    '                            If elements.Contains(predicate)
    '                                If Not keep
    '                                    keep = True
    '                                Else
    '                                    keep = False
    '                                    Exit For
    '                                End If
    '                            End If
    '                        Next
    '                    Case "only"
    '                        If elements.Length = 1
    '                            Dim element As String = elements(0)
    '                            keep = predicates.Any(Function(ByVal x As String) element = x)
    '                        End If
    '                    Case Else
    '                        Throw New ArgumentException("Unknown filter kind '{0}'", filterKind)
    '                    End Select
    '                    If keep Then Exit for
    '                Next
    '            End Using
    '        End Select
    '        If Not keep
    '            list.RemoveAt(i)
    '        Else
    '            i += 1
    '        End If
    '    End While
    'End Sub

End Module

Public Class Genre
    Implements PlaylistManager.Builder.ICommand

    Public Sub Execute(ByRef list As Playlist, ByVal modifiers As String(), ByVal params As String()) Implements PlaylistManager.Builder.ICommand.Execute
        Console.WriteLine("Filtering genres...")
        params = params.Select(Function(ByVal x As String) x.ToLower()).ToArray()
        Dim filterKind As String = If(modifiers.Length > 0, modifiers(0), "all")
        Filterer.Filter(list, Function(ByVal tag As Id3Tag)
                    Dim genres As String() = tag.Genre _
                            .ToString() _
                            .ToLower() _
                            .Split("/"c)
                    Select filterKind
                    Case "all": Return Filterer.All(params, Function(ByVal x As String) genres.Contains(x))
                    Case "any": Return Filterer.Any(params, Function(ByVal x As String) genres.Contains(x))
                    Case "either": Return Filterer.Either(params, Function(ByVal x As String) genres.Contains(x))
                    Case "only": Return Filterer.Only(params, Function(ByVal x As String) genres.Contains(x))
                    Case Else: Throw New ArgumentException("Unknown modifier '{0}'", filterKind)
                    End Select
                End Function)
    End Sub

End Class

'Public Class Artist
'    Implements PlaylistManager.Builder.ICommand

'    Public Sub Execute(ByRef list As Playlist, ByVal modifiers As String(), ByVal params As String()) Implements PlaylistManager.Builder.ICommand.Execute
'        Dim filterKind As String = If(modifiers.Length > 0, modifiers(0), "any")
'        Filter.Filter(list, "artist", filterKind, params)
'    End Sub

'End Class

'Public Class Album
'    Implements PlaylistManager.Builder.ICommand

'    Public Sub Execute(ByRef list As Playlist, ByVal modifiers As String(), ByVal params As String()) Implements PlaylistManager.Builder.ICommand.Execute
'        Dim filterKind As String = If(modifiers.Length > 0, modifiers(0), "any")
'        Filter.Filter(list, "album", filterKind, params)
'    End Sub

'End Class

'Public Class Year
'    Implements PlaylistManager.Builder.ICommand

'    Public Sub Execute(ByRef list As Playlist, ByVal modifiers As String(), ByVal params As String()) Implements PlaylistManager.Builder.ICommand.Execute
'        Dim filterKind As String = If(modifiers.Length > 0, modifiers(0), "any")
'        Filter.Filter(list, "year", filterKind, params)
'    End Sub

'End Class