Imports mpc_playlist.Playlist
Imports mpc_playlist.Command

Public Class Update
    Implements Command.Extension

    Public ReadOnly Property Description As String Implements Extension.Description
        Get
            Return "Adds or removes contents of a playlist." & _
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
        Dim values As List(Of String) = New List(Of String)(params)
        values.RemoveAt(0)
        Dim relative As Boolean = False
        Dim attribute As String = Nothing
        For Each value In values
            Select value
            Case "--absolute":
                '' make the playlist absolute
                relative = False
            Case "--relative":
                '' make the playlist relative
                relative = True
            Case "-add", "-del":
                attribute = value
            Case Else:
                Select attribute
                Case "-add":
                    '' add value to playlist
                    playlist.Add(value)
                Case "-del":
                    '' remove value from playlist
                    playlist.Remove(value)
                Case Else:
                    Throw New ArgumentException("Expected one of: '--absolute,' '--relative,' '-add,' '-del.' Got: '" & attribute & ".'")
                End Select
            End Select
        Next
        playlist.Save(path, relative)
    End Sub

End Class