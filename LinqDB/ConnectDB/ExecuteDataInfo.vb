Namespace ConnectDB
    Public Class ExecuteDataInfo
        Dim _IsSuccess As Boolean = False
        Dim _NewID As Long = 0
        Dim _SqlStatement As String = ""
        Dim _ErrorMessage As String = ""
        Dim _InfoMessage As String = ""
        Dim _RecordEffected As Integer = 0

        Public Property IsSuccess As Boolean
            Get
                Return _IsSuccess
            End Get
            Set(value As Boolean)
                _IsSuccess = value
            End Set
        End Property
        Public Property NewID As Long
            Get
                Return _NewID
            End Get
            Set(value As Long)
                _NewID = value
            End Set
        End Property

        Public Property SqlStatement As String
            Get
                Return _SqlStatement.Trim
            End Get
            Set(value As String)
                _SqlStatement = value
            End Set
        End Property

        Public Property ErrorMessage As String
            Get
                Return _ErrorMessage.Trim
            End Get
            Set(value As String)
                _ErrorMessage = value
            End Set
        End Property

        Public Property InfoMessage As String
            Get
                Return _InfoMessage.Trim
            End Get
            Set(value As String)
                _InfoMessage = value
            End Set
        End Property

        Public Property RecordEffected As Integer
            Get
                Return _RecordEffected
            End Get
            Set(value As Integer)
                _RecordEffected = value
            End Set
        End Property

    End Class
End Namespace

