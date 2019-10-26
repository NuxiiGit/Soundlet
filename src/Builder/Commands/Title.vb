Imports System.IO

Public Class Title
    Implements PlaylistManager.Builder.ICommand

    Public Sub Execute(ByRef list As Playlist, ByVal ParamArray params As String()) Implements PlaylistManager.Builder.ICommand.Execute
        Console.WriteLine("Matching titles...")
        For Each pattern As String In params
            Dim i As Integer = 0
            While i < list.Count
                Dim record As String = list(i)
                If Not (Path.GetFileName(record) Like pattern)
                    list.RemoveAt(i)
                Else
                    i += 1
                End If
            End While
        Next
    End Sub

End Class