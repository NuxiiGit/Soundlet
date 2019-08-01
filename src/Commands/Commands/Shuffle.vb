Imports mpc_playlist.Playlist
Imports mpc_playlist.Command

Public Class Shuffle
    Implements Command.Extension

    ''' <summary>
    ''' <see cref="Command.Extension.Description"/>
    ''' </summary>
    Public ReadOnly Property Description As String Implements Extension.Description
        Get
            Return "Shuffles the contents of a playlist." & _
                    Environment.NewLine & _
                    Environment.NewLine & "Use 'shuffle <playlist>' to shuffle the contents of 'playlist.'"
        End Get
    End Property

    ''' <summary>
    ''' <see cref="Command.Extension.Execute(String())"/>
    ''' </summary>
    ''' <exception cref="ArgumentException">Thrown when <paramref name="params"/> is empty.</exception>
    Public Sub Execute(ParamArray params() As String) Implements Extension.Execute
        If (params.Length = 0) Then Throw New ArgumentException("You must supply a playlist to shuffle the contents of.")
        Dim path As String = params(0)
        Dim playlist As Playlist = New Playlist(path)
        playlist.Shuffle()
        playlist.Save(path, true)
    End Sub

End Class