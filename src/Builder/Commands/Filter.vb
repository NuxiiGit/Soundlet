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
        Case Else: Throw New ArgumentException("Unknown modifier '{0}'", modifier)
        End Select
    End Function

    Private Function All(ByVal xs As String(), ByVal p As Func(Of String, Boolean)) As Boolean
        If xs.Length < 1 Then Return False
        For Each x In xs
            If Not Truthy(x, p) Then Return False
        Next
        Return True
    End Function

    Private Function Any(ByVal xs As String(), ByVal p As Func(Of String, Boolean)) As Boolean
        For Each x In xs
            If Truthy(x, p) Then Return True
        Next
        Return False
    End Function

    Private Function Either(ByVal xs As String(), ByVal p As Func(Of String, Boolean)) As Boolean
        Dim satisfied As Boolean = False
        For Each x In xs
            If Truthy(x, p)
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

    Private Function Truthy(ByVal x As String, ByVal p As Func(Of String, Boolean)) As Boolean
        If x.Length < 1 Then Return False
        If x(0) = "!"c
            Return Not p(x.Remove(0, 1))
        Else
            Return p(x)
        End If
    End Function

End Module

Public Class Genre
    Implements PlaylistManager.Builder.ICommand

    Public Sub Execute(ByRef list As Playlist, ByVal modifiers As String(), ByVal params As String()) Implements PlaylistManager.Builder.ICommand.Execute
        Dim modifier As String = If(modifiers.Length > 0, modifiers(0), "all")
        Console.WriteLine("Filtering {0} genres...", modifier)
        For Each param In params : Console.WriteLine(" | {0}", param) : Next
        Filterer.Filter(list, Function(ByVal tag As Id3Tag)
                    Dim elements As String() = tag.Genre.ToString().ToLower().Split("/"c)
                    Return Filterer.Satisfies(params, Function(ByVal x As String) elements.Contains(x.ToLower()), modifier)
                End Function)
    End Sub

End Class

Public Class Artist
    Implements PlaylistManager.Builder.ICommand

    Public Sub Execute(ByRef list As Playlist, ByVal modifiers As String(), ByVal params As String()) Implements PlaylistManager.Builder.ICommand.Execute
        Dim modifier As String = If(modifiers.Length > 0, modifiers(0), "any")
        Console.WriteLine("Filtering {0} artists...", modifier)
        For Each param In params : Console.WriteLine(" | {0}", param) : Next
        Filterer.Filter(list, Function(ByVal tag As Id3Tag)
                    Dim elements As String() = tag.Artists.ToString().ToLower().Split("/"c)
                    Return Filterer.Satisfies(params, Function(ByVal x As String) elements.Contains(x.ToLower()), modifier)
                End Function)
    End Sub

End Class

Public Class Album
    Implements PlaylistManager.Builder.ICommand

    Public Sub Execute(ByRef list As Playlist, ByVal modifiers As String(), ByVal params As String()) Implements PlaylistManager.Builder.ICommand.Execute
        Dim modifier As String = If(modifiers.Length > 0, modifiers(0), "any")
        Console.WriteLine("Filtering {0} albums...", modifier)
        For Each param In params : Console.WriteLine(" | {0}", param) : Next
        Filterer.Filter(list, Function(ByVal tag As Id3Tag)
                    Dim element As String = tag.Album.ToString().ToLower()
                    Return Filterer.Satisfies(params, Function(ByVal x As String) element = x.ToLower(), modifier)
                End Function)
    End Sub

End Class

Public Class Year
    Implements PlaylistManager.Builder.ICommand

    Public Sub Execute(ByRef list As Playlist, ByVal modifiers As String(), ByVal params As String()) Implements PlaylistManager.Builder.ICommand.Execute
        Dim modifier As String = If(modifiers.Length > 0, modifiers(0), "any")
        Console.WriteLine("Filtering {0} years...", modifier)
        For Each param In params : Console.WriteLine(" | {0}", param) : Next
        Filterer.Filter(list, Function(ByVal tag As Id3Tag)
                    Dim element As String = tag.Year.ToString()
                    Return Filterer.Satisfies(params, Function(ByVal x As String) element = x, modifier)
                End Function)
    End Sub

End Class