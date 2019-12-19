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

    ''' <summary>
    ''' Returns whether some predicate satisfies the collection <c>xs</c>.
    ''' </summary>
    ''' <param name="xs">The collection of values to iterate through.</param>
    ''' <param name="p">The predicate to check.</param>
    ''' <param name="modifier">The type of filter method to perform.</param>
    ''' <returns><c>True</c> if the predicate is satisfied according to the modifer rules.</returns>
    Public Function Satisfies(ByVal xs As String(), ByVal p As Func(Of String, Boolean), ByVal modifier As String) As Boolean
        Select modifier
        Case "all": Return Filterer.All(xs, p)
        Case "any": Return Filterer.Any(xs, p)
        Case "either": Return Filterer.Either(xs, p)
        Case "only": Return Filterer.Only(xs, p)
        Case Else: Throw New ArgumentException("Unknown modifier '{0}'", modifier)
        End Select
    End Function

    Private Function All(ByVal xs As String(), ByVal p As Func(Of String, Boolean))
        Return xs.All(p)
    End Function

    Private Function Any(ByVal xs As String(), ByVal p As Func(Of String, Boolean))
        Return xs.Any(p)
    End Function

    Private Function Either(ByVal xs As String(), ByVal p As Func(Of String, Boolean))
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

    Private Function Only(ByVal xs As String(), ByVal p As Func(Of String, Boolean))
        Return xs.Length = 1 AndAlso p(xs(0))
    End Function
    
End Module

Public Class Genre
    Implements PlaylistManager.Builder.ICommand

    Public Sub Execute(ByRef list As Playlist, ByVal modifiers As String(), ByVal params As String()) Implements PlaylistManager.Builder.ICommand.Execute
        Console.WriteLine("Filtering genres...")
        Dim modifier As String = If(modifiers.Length > 0, modifiers(0), "any")
        Filterer.Filter(list, Function(ByVal tag As Id3Tag)
                    Dim genres As String() = tag.Genre _
                            .ToString() _
                            .ToLower() _
                            .Split("/"c)
                    Return Filterer.Satisfies(params, Function(ByVal x As String) genres.Contains(x.ToLower()), modifier)
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