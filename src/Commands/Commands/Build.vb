Imports mpc_playlist.Playlist
Imports mpc_playlist.Command
Imports Id3

Public Class Build
    Implements Command.Extension

    Public ReadOnly Property Description As String Implements Extension.Description
        Get
            Return "Builds a new playlist from a directory of files." & _
                    Environment.NewLine & Environment.NewLine & "The full syntax of this command is: 'build <destination> <directories> [--absolute/relative] [--append] [-mask <mask>] [-rating <amount>] [-genres <genres>] [-artists <artists>] [-album <album>]'" & _
                    Environment.NewLine & Environment.NewLine & "Use 'build <playlist> <directory> -mask <mask>' to build a playlist containing files inside 'directory' which match the pattern 'mask.' Masks follow the usual FILE MASK syntax." & _
                    Environment.NewLine & Environment.NewLine & "Use 'build <playlist> <directory> -rating <amount>' to build a playlist where each audio file must exceed a minimum rating of 'amount.'" & _
                    Environment.NewLine & Environment.NewLine & "Use 'build <playlist> <directory> -genres <genre>' to build a playlist where each audio file is of a certain genre 'genre.'" & _
                    Environment.NewLine & Environment.NewLine & "Use 'build <playlist> <directory> -artists <artist>' to build a playlist where each audio file contans a certain artist 'artist.'" & _
                    Environment.NewLine & Environment.NewLine & "Use 'build <playlist> <directory> -album <album>' to build a playlist where each audio file is part of a certain album 'album.'"
        End Get
    End Property

    ''' <exception cref="ArgumentException">Thrown when <paramref name="params"/> is empty.</exception>
    Public Sub Execute(ParamArray params() As String) Implements Extension.Execute
        If (params.Length = 0) Then Throw New ArgumentException("You must supply a playlist to update the contents of.")
        Dim path As String = params(0)
        '' TODO
    End Sub

End Class