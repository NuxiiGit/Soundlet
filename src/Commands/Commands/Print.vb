Imports mpc_playlist.Playlist
Imports mpc_playlist.Command

Public Class Print
    Implements Command.Extension

    ''' <summary>
    ''' <see cref="Command.Extension.Description"/>
    ''' </summary>
    Public ReadOnly Property Description As String Implements Extension.Description
        Get
            Return "Use 'print <playlist>' to print the contents of 'playlist.'"
        End Get
    End Property

    ''' <summary>
    ''' <see cref="Command.Extension.Execute(String())"/>
    ''' </summary>
    ''' <exception cref="ArgumentException">Thrown when <paramref name="params"/> is empty.</exception>
    Public Sub Execute(ParamArray params() As String) Implements Extension.Execute
        If (params.Length = 0) Then Throw New ArgumentException("You must supply a playlist to print the contents of.")
        Dim playlist As Playlist = New Playlist(params(0))
        For i As Integer = 0 To (playlist.Count - 1)
            Console.WriteLine("Track #{0}: {1}", i, playlist(i))
        Next
    End Sub

End Class