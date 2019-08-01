Imports mpc_playlist.Playlist
Imports mpc_playlist.Command

Public Class Update
    Implements Command.Extension

    Public ReadOnly Property Description As String Implements Extension.Description
        Get
            Return "Adds or removes contents of a playlist" & _
                    Environment.NewLine & Environment.NewLine & "The full syntax of this command is: 'update <playlist> [--absolute/relative] [-add <files>] [-del <files>]'" & _
                    Environment.NewLine & Environment.NewLine & "Use 'update <playlist> --absolute' to force all filepaths to be absolute paths. Alternatively, use 'update <playlist> --relative' to make all filepaths become relative to the playlist file; this is useful when there is a strong bond between your playlist files and your audio files." & _
                    Environment.NewLine & Environment.NewLine & "Use 'update <playlist> --add file1 [file2] [...]' to add a list of audio files to the playlist. The same can be done for deletion: use 'update <playlist> --del file1 [file2] [...]' to remove a list of audio files to the playlist."
        End Get
    End Property

    ''' <exception cref="ArgumentException">Thrown when <paramref name="params"/> is empty.</exception>
    Public Sub Execute(ParamArray params() As String) Implements Extension.Execute
        If (params.Length = 0) Then Throw New ArgumentException("You must supply a playlist to update the contents of.")
        Dim path As String = params(0)
        Dim playlist As Playlist = New Playlist(path)
        Dim iter As LinkedList(Of String) = New LinkedList(Of String)(params)
        '' iterate through parameters
        Dim relative As Boolean = False
        Dim x As LinkedListNode(Of String) = iter.First.Next
        While (x IsNot Nothing) 
            Dim attribute As String = x.Value
            x = x.Next
            Select attribute
            Case "--absolute":
                '' make the playlist absolute
                relative = False
            Case "--relative":
                relative = True
            Case "-add":
                While ((x IsNot Nothing) AndAlso (Not {"--absolute", "--relative", "-add", "-del"}.Contains(x.Value)))
                    '' add x to the playlist
                    Console.WriteLine(x.Value)
                    playlist.Add(x.Value)
                    x = x.Next
                End While
            Case "-del":
                While ((x IsNot Nothing) AndAlso (Not {"--absolute", "--relative", "-add", "-del"}.Contains(x.Value)))
                    '' remove x from the playlist
                    playlist.Remove(x.Value)
                    x = x.Next
                End While
            Case Else:
                Throw New ArgumentException("Expected one of: '--absolute,' '--relative,' '-add,' '-del.' Got: '" & attribute & ".'")
            End Select
        End While
        playlist.Save(path, relative)
    End Sub

End Class