Imports System.IO

Public Class Title
    Implements PlaylistManager.Builder.ICommand

    Public Sub Execute(ByRef list As Playlist, ByVal ParamArray params As String()) Implements PlaylistManager.Builder.ICommand.Execute
        Console.WriteLine("Filtering out elements whose names arent matches:")
        For Each pattern As String In params
            For Each record In list
                If Path.GetFileName(record) Like pattern
                    Console.WriteLine(" - " & record)
                    list.Remove(record)
                End If
            Next
        Next
    End Sub

End Class