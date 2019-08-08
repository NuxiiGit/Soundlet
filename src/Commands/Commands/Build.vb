Imports mpc_playlist.Playlist
Imports mpc_playlist.Command
Imports System.IO
Imports Id3

Public Class Build
    Implements Command.Extension

    Public ReadOnly Property Description As String Implements Extension.Description
        Get
            Return "Builds a new playlist from a directory of files." & _
                    Environment.NewLine & Environment.NewLine & "The full syntax of this command is: 'build <destination> <file-mask> [-mask <mask>] [-rating <amount>] [-genres <genres>] [-artists <artists>] [-album <album>] [--append]'" & _
                    Environment.NewLine & Environment.NewLine & "Use 'build <playlist> <file-mask>' to build a playlist containing files relative to the current directory which satisfy the file mask 'file-mask.' Masks follow the usual mask syntax." & _
                    Environment.NewLine & Environment.NewLine & "Use 'build <playlist> <file-mask> -genres <genre>' to build a playlist where each audio file is of a certain genre 'genre.'" & _
                    Environment.NewLine & Environment.NewLine & "Use 'build <playlist> <file-mask> -artists <artist>' to build a playlist where each audio file contans a certain artist 'artist.'"
        End Get
    End Property

    ''' <exception cref="ArgumentException">Thrown when <paramref name="params"/> is empty.</exception>
    Public Sub Execute(ParamArray params() As String) Implements Extension.Execute
        If (params.Length = 0) Then Throw New ArgumentException("You must supply a playlist to update the contents of.")
        If (params.Length = 1) Then Throw New ArgumentException("You must supply a file mask to match files.")
        '' compile a list of file paths which match the file mask
        Dim mask As String = params(1) _
                .Replace("\"c, "/"c) _
                .Replace("/"c, Path.DirectorySeparatorChar)
        Dim root As String = Directory.GetCurrentDirectory() _
                .Replace("\"c, "/"c) _
                .Replace("/"c, Path.DirectorySeparatorChar)
        While (mask.IndexOf("../") = 0)
            mask = mask.Remove(0, 2) '' remove ../ delimiter
            root = Path.GetDirectoryName(root)
        End While
        Dim files As List(Of String) = New List(Of String)
        Dim dirQueue As Queue(Of String) = New Queue(Of String)()
        dirQueue.Enqueue(root)
        While (dirQueue.Count > 0)
            Dim dir As String = dirQueue.Dequeue()
            '' add additional directories
            For Each subDir In Directory.GetDirectories(dir)
                Console.WriteLine("Sub-directory: " & Path.GetFileName(subDir))
                dirQueue.Enqueue(subDir)
            Next
            '' get files
            For Each file As String In Directory.GetFiles(dir)
                If (file.Replace(root, "") Like mask) Then files.Add(file)
            Next
        End While
        '' build playlist
        Dim playlist As Playlist = New Playlist()
        Dim attributes As List(Of String) = New List(Of String)(params)
        attributes.RemoveAt(0) '' remove path
        attributes.RemoveAt(0) '' remove mask
        Dim lastAttribute As String = Nothing
        For Each attribute In attributes
            Select attribute
            Case "-genres", "-genre", "-artists", "-artist", "--append":
                lastAttribute = attribute
            Case Else:
                Select lastAttribute
                Case "-genres", "-genre":
                    '' filter out genres
                    For i As Integer = (files.Count - 1) To 0 Step -1
                        Dim file As String = files(i)
                        Dim genres As String() = {}
                        If (Path.GetExtension(file) = ".mp3")
                            Using mp3 As New Mp3(file)
                                For Each tag In mp3.GetAllTags() 
                                    genres = tag.Genre.ToString().Split("/"c)
                                    Exit For
                                Next
                            End Using
                        End If
                        If (Not genres.Contains(attribute)) Then files.Remove(file)
                    Next
                Case "-artists", "-artist":
                    '' filter out artists
                    For i As Integer = (files.Count - 1) To 0 Step -1
                        Dim file As String = files(i)
                        Dim artists As String() = {}
                        If (Path.GetExtension(file) = ".mp3")
                            Using mp3 As New Mp3(file)
                                For Each tag In mp3.GetAllTags() 
                                    artists = tag.Artists.ToString().Split("/"c)
                                    Exit For
                                Next
                            End Using
                        End If
                        If (Not artists.Contains(attribute)) Then files.Remove(file)
                    Next
                Case "-album":
                    '' filter out albums
                    For i As Integer = (files.Count - 1) To 0 Step -1
                        Dim file As String = files(i)
                        Dim album As String = ""
                        If (Path.GetExtension(file) = ".mp3")
                            Using mp3 As New Mp3(file)
                                For Each tag In mp3.GetAllTags() 
                                    album = tag.Album
                                    Exit For
                                Next
                            End Using
                        End If
                        If (album <> attribute) Then files.Remove(file)
                    Next
                Case Else:
                    Throw New ArgumentException("Expected one of: '--append,' '-genre,' '-artist,' '-album.' Got: '" & attribute & ".'")
                End Select
            End Select
        Next
        Dim dest As String = params(0)
        If (lastAttribute = "--append")
            Try 
                playlist.Load(dest)
            Catch e As IOException
                Console.WriteLine("There was an error opening the playlist for append.")
            End Try
        End If
        '' write to the playlist
        For Each file In files
            Console.WriteLine(" - Adding File: " & Path.GetFileName(file))
            playlist.Add(file)
        Next
        playlist.Save(dest, False)
    End Sub

End Class