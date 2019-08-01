Imports mpc_playlist
Imports mpc_playlist.Command

Public Class Test
    Implements Command.Extension

    ''' <summary>
    ''' <see cref="Command.Extension.Description"/>
    ''' </summary>
    Public ReadOnly Property Description As String Implements Extension.Description
        Get
            Throw New NotImplementedException()
        End Get
    End Property

    ''' <summary>
    ''' <see cref="Command.Extension.Execute(String())"/>
    ''' </summary>
    Public Sub Execute(ParamArray params() As String) Implements Extension.Execute
        Throw New NotImplementedException()
    End Sub

End Class