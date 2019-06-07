Imports System.Reflection
Imports System.IO.StreamReader
Imports System.IO.StreamWriter
Imports System.IO
Imports System.Xml.XmlReader
Imports System.Xml.XmlWriter
Imports System.Xml

''' <summary>
''' A class which can be used to construct and manage Media Player (Classic) playlist formats.
''' </summary>
Public NotInheritable Class Playlist
    
    ''' <summary>
    ''' Maintains a relationship between the name of a file extension, and its actual playlist extension <see cref="Playlist.Extension"/>.
    ''' </summary>
    Private Shared extensions As Dictionary(Of String, Extension) = New Dictionary(Of String, Extension)
    
    Private paths As List(Of String) = New List(Of String)

    ''' <summary>
    ''' An interface which manages playlist extensions.
    ''' </summary>
    Public Interface Extension
        
        ''' <summary>
        ''' Captures the paths from this stream and inserts them into the <paramref name="paths"/> list.
        ''' </summary>
        ''' <param name="stream">The input stream for this file.</param>
        ''' <param name="paths">The output list to insert path names.</param>
        Sub Decode(ByRef stream As StreamReader, ByRef paths As List(Of String))

        ''' <summary>
        ''' Pushes the paths from the <paramref name="paths"/> array into this stream.
        ''' </summary>
        ''' <param name="stream"></param>
        ''' <param name="paths"></param>
        Sub Encode(ByRef stream As StreamWriter, ByRef paths As List(Of String))

    End Interface

    ''' <summary>
    ''' Uses reflection to compile the dictionary of file extensions at runtime.
    ''' </summary>
    Shared Sub Run()
        Dim template As Type = GetType(Extension)
        For Each dataType As Type In template.Assembly.GetTypes()
            If (dataType.IsClass() AndAlso dataType.GetInterfaces.Contains(template))
                '' valid class type
                extensions.Add("." & dataType.Name.ToUpper(), Activator.CreateInstance(dataType))
            End If
        Next
    End Sub

    

End Class