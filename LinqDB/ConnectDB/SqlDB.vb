Imports System.Data.SqlClient
Imports System.Windows.Forms
Imports System.Drawing


Namespace ConnectDB
    Public Class SqlDB
        Protected Shared ErrorConnectionString As String = MessageResources.MSGEC001
        Protected Shared ErrorConnection As String = MessageResources.MSGEC002
        Protected Shared ErrorSetCommandConnection As String = MessageResources.MSGEC003
        Protected Shared ErrorInvalidCommandType As String = MessageResources.MSGEC004
        Protected Shared ErrorDuplicateParameter As String = MessageResources.MSGEC006
        Protected Shared ErrorNullParameter As String = MessageResources.MSGEC005
        Protected Shared ErrorExecuteNonQuery As String = MessageResources.MSGEC010
        Protected Shared ErrorExecuteReader As String = MessageResources.MSGEC011
        Protected Shared ErrorExecuteTable As String = MessageResources.MSGEC012
        Protected Shared ErrorExecuteScalar As String = MessageResources.MSGEC013
        Protected Shared ErrorDatabaseOther As String = MessageResources.MSGEC901
        Protected Shared ErrorUndefined As String = MessageResources.MSGEC902

        Private Shared _DataSource As String = ""
        Private Shared _DataBaseName As String = ""
        Private Shared _UserID As String = ""
        Private Shared _Password As String = ""

        Public Shared Sub SetDatabaseInfo(DataSource As String, DbName As String, DbUserID As String, DbPassword As String)
            _DataSource = DataSource
            _DataBaseName = DbName
            _UserID = DbUserID
            _Password = DbPassword
        End Sub


        Private Shared _err As String
        Public Shared ReadOnly Property ErrorMessage()
            Get
                Return _err
            End Get
        End Property

        Public Shared Function SetDouble(ByVal number As System.Nullable(Of Double)) As String
            Dim ret As String
            If Convert.IsDBNull(number) Then
                ret = "NULL"
            ElseIf number Is Nothing Then
                ret = "NULL"
            Else
                ret = number.ToString()
            End If
            Return ret
        End Function
        Public Shared Function SetDecimal(ByVal number As System.Nullable(Of Decimal)) As String
            Dim ret As String
            If Convert.IsDBNull(number) Then
                ret = "NULL"
            ElseIf number Is Nothing Then
                ret = "NULL"
            Else
                ret = number.ToString()
            End If
            Return ret
        End Function
        Public Shared Function SetString(ByVal str As String) As String
            Dim ret As String = ""
            If str Is Nothing Or str.Trim() = "" Then
                ret = "NULL"
            Else
                ret = Chr(39) & str.Trim().Replace("'", "''") & Chr(39)
            End If
            Return ret
        End Function
        Public Shared Function SetDate() As String
            Return SetDateTime(DateTime.Now)
        End Function
        Public Function SetDate(ByVal DateIn As DateTime) As String
            Return SetDateTime(DateIn)
        End Function
        Public Shared Function SetDateTime() As String
            Return SetDateTime(DateTime.Now)
        End Function
        Public Shared Function SetDateTime(ByVal DateIn As DateTime) As String
            Dim ret As String = ""
            If DateIn.Year = 1 Or Convert.IsDBNull(DateIn) Then
                ret = "NULL"
            ElseIf DateIn.Year > 2500 Then
                Dim vYear As String = DateIn.Year - 543
                ret = "'" & vYear & "-" & DateIn.ToString("MM-dd HH:mm:ss.fff") & "'"
            Else
                ret = "'" & DateIn.Year & "-" & DateIn.ToString("MM-dd HH:mm:ss.fff") & "'"
            End If
            Return ret
        End Function

        Public Shared Function GetExceptionMessage(ByVal ex As SqlException) As String
            Return String.Format(ErrorDatabaseOther, ex.ErrorCode.ToString(), ex.Message)
        End Function

        Public Shared Function GetStartupPath() As String
            Return Application.StartupPath
        End Function

        Protected Shared ReadOnly Property GetConnectionString() As String
            Get
                Try
                    'Return "Data Source=" & _DataSource & ";Initial Catalog=" & _DataBaseName & ";User ID=" & _UserID & ";Password=" & _Password & ";"
                    Return "Data Source=localhost;Integrated Security=True;Connect Timeout=1;"
                Catch ex As Exception
                    Throw New ApplicationException(ErrorConnectionString, ex)
                End Try
            End Get
        End Property


        Public Shared Function GetConnection() As SqlConnection
            Dim conn As SqlConnection
            Try
                conn = New SqlConnection(GetConnectionString())
                conn.Open()
                Return conn
            Catch ex As ApplicationException
                'Throw ex
                _err = ex.Message
            Catch ex As SqlException
                Try
                    conn = New SqlConnection(GetConnectionString())
                    conn.Open()
                    Return conn
                Catch ex1 As SqlException
                    _err = ex1.Message
                End Try
            Catch ex As Exception
                'Throw New ApplicationException(GetExceptionMessage(ex), ex)
                _err = ex.Message
            End Try
        End Function

        Public Shared Function GetConnection(ByVal connString As String) As SqlConnection
            Dim conn As SqlConnection
            Try
                conn = New SqlConnection(connString)
                conn.Open()
                Return conn
            Catch ex As ApplicationException
                'Throw ex
                _err = ex.Message
            Catch ex As SqlException
                _err = ex.Message
                'Throw New ApplicationException(ErrorConnection, ex)
            Catch ex As Exception
                'Throw New ApplicationException(GetExceptionMessage(ex), ex)
                _err = ex.Message
            End Try
        End Function


        Public Shared Function ChkConnection(ByVal connString As String) As Boolean
            Dim ret As Boolean = False
            Dim conn As SqlConnection
            Try
                conn = New SqlConnection(connString)
                conn.Open()

                ret = True
                conn.Close()
            Catch ex As ApplicationException
                'Throw ex
                _err = ex.Message
                ret = False
            Catch ex As SqlException
                Try
                    conn = New SqlConnection(connString)
                    conn.Open()
                    ret = True
                    conn.Close()
                Catch ex1 As SqlException
                    _err = ex1.Message
                    ret = False
                End Try
            Catch ex As Exception
                'Throw New ApplicationException(GetExceptionMessage(ex), ex)
                _err = ex.Message
                ret = False
            End Try

            Return ret
        End Function
        Public Shared Function GetNextID(ByVal fldName As String, ByVal tbName As String, ByVal trans As SqlTransaction) As Long
            Dim ret As Long
            Dim Sql As String = "select max(" & fldName & ") maxID from " & tbName
            Dim dt As DataTable = ExecuteTable(Sql, trans)
            If dt.Rows.Count > 0 Then
                If Convert.IsDBNull(dt.Rows(0)("maxID")) = False Then
                    ret = Convert.ToInt64(dt.Rows(0)("maxID").ToString()) + 1
                Else
                    ret = 1
                End If
            Else
                ret = 1
            End If
            Return ret
        End Function

        Public Shared Function GetLastID(ByVal tbName As String, ByVal trans As SqlTransaction) As Long
            Dim ret As Long
            Dim Sql As String = "select IDENT_CURRENT('" & tbName & "') lastID "
            Dim dt As DataTable = ExecuteTable(Sql, trans)
            If Convert.IsDBNull(dt.Rows(0)("lastID")) = False Then
                ret = Convert.ToInt64(dt.Rows(0)("lastID").ToString())
            Else
                ret = 1
            End If

            Return ret
        End Function

        Public Shared Function GetDateNow(ByVal rtType As String, ByVal trans As SqlTransaction) As String
            'rtType = D  ให้คืนค่าเป็นวันที่ในรูปแบบ yyyy-MM-dd
            'rtType = T  ให้คืนค่าเป็นเวลาปัจจุบัน hh:mm
            Dim ret As String
            Dim dt As DataTable = ExecuteTable("select convert(varchar(10),getdate(),120) date_now, CONVERT(varchar(5),getdate(),108) time_now", trans)
            If rtType = "D" Then
                ret = dt.Rows(0)("date_now")
            ElseIf rtType = "T" Then
                ret = dt.Rows(0)("time_now")
            Else
                ret = dt.Rows(0)("date_now") & " " & dt.Rows(0)("time_now")
            End If

            Return ret
        End Function

        Public Shared Function GetDateNowFromDB(ByVal trans As SqlTransaction) As Date
            Dim ret As String
            Dim dt As DataTable = ExecuteTable("select getdate() date_now", trans)
            ret = dt.Rows(0)("date_now")
            Return ret
        End Function

        Public Shared Function ExecuteTable(ByVal sql As String) As DataTable
            Return ExecuteTable(sql, Nothing, Nothing, Nothing)
        End Function
        Public Shared Function ExecuteTable(sql As String, cmdParam() As SqlParameter) As DataTable
            Return ExecuteTable(sql, Nothing, Nothing, cmdParam)
        End Function
        Public Shared Function ExecuteTable(ByVal sql As String, ByVal conn As SqlConnection) As DataTable
            Return ExecuteTable(sql, conn, Nothing, Nothing)
        End Function
        Public Shared Function ExecuteTable(ByVal sql As String, ByVal trans As SqlTransaction) As DataTable
            Return ExecuteTable(sql, Nothing, trans, Nothing)
        End Function
        Public Shared Function ExecuteTable(sql As String, trans As SqlTransaction, cmdParam() As SqlParameter) As DataTable
            Return ExecuteTable(sql, Nothing, trans, cmdParam)
        End Function
        Public Shared Function ExecuteTable(ByVal sql As String, ByVal conn As SqlConnection, ByVal trans As SqlTransaction, cmdParam() As SqlParameter) As DataTable
            Dim cmd As New SqlCommand
            Dim adapter As New SqlDataAdapter
            adapter.SelectCommand = cmd
            Dim LetClose As Boolean = False
            Dim dt As New DataTable
            Try
                'If conn Is Nothing And trans Is Nothing Then
                '    conn = GetConnection()
                'ElseIf trans IsNot Nothing And conn Is Nothing Then
                '    conn = trans.Connection
                'End If

                If trans IsNot Nothing And conn Is Nothing Then
                    conn = trans.Connection
                ElseIf conn Is Nothing Then
                    conn = GetConnection()
                    LetClose = True
                End If

                BuildCommand(cmd, conn, trans, CommandType.Text, sql, cmdParam)
                adapter.Fill(dt)
                adapter.Dispose()
                If LetClose = True Then
                    cmd.Dispose()
                    conn.Close()
                    SqlConnection.ClearAllPools()
                End If
            Catch ex As ApplicationException
                _err = sql & " $$$$ " & ex.Message
                adapter.Dispose()
            Catch ex As SqlException
                _err = sql & " $$$$ " & ex.Message
                adapter.Dispose()
            Catch ex As Exception
                _err = sql & " $$$$ " & ex.Message
                adapter.Dispose()
            End Try

            Return dt
        End Function

        Public Shared Function ExecuteReader(ByVal Sql As String) As SqlDataReader
            Return ExecuteReader(Sql, Nothing, Nothing)
        End Function
        Public Shared Function ExecuteReader(sql As String, cmdParam() As SqlParameter)
            Return ExecuteReader(sql, Nothing, cmdParam)
        End Function
        Public Shared Function ExecuteReader(ByVal Sql As String, ByVal trans As SqlTransaction) As SqlDataReader
            Return ExecuteReader(Sql, Nothing, trans, Nothing)
        End Function

        Public Shared Function ExecuteReader(ByVal Sql As String, ByVal conn As SqlConnection) As SqlDataReader
            Return ExecuteReader(Sql, conn, Nothing, Nothing)
        End Function

        Public Shared Function ExecuteReader(ByVal Sql As String, ByVal trans As SqlTransaction, cmdParam() As SqlParameter) As SqlDataReader
            Return ExecuteReader(Sql, Nothing, trans, cmdParam)
        End Function

        Public Shared Function ExecuteReader(ByVal Sql As String, ByVal conn As SqlConnection, ByVal trans As SqlTransaction, cmdParam() As SqlParameter) As SqlDataReader
            Dim command As New SqlCommand()
            Dim reader As SqlDataReader
            Dim LetClose As Boolean = False

            Try
                If trans IsNot Nothing And conn Is Nothing Then
                    conn = trans.Connection
                ElseIf conn Is Nothing Then
                    conn = GetConnection()
                    LetClose = True
                End If

                BuildCommand(command, conn, trans, CommandType.Text, Sql, cmdParam)
                If LetClose = True Then
                    reader = command.ExecuteReader(CommandBehavior.CloseConnection)
                    SqlConnection.ClearAllPools()
                Else
                    reader = command.ExecuteReader()
                End If
            Catch ex As ApplicationException
                _err = Sql & " $$$$ " & ex.Message
            Catch ex As SqlException
                _err = Sql & " $$$$ " & ex.Message
            Catch ex As Exception
                _err = Sql & " $$$$ " & ex.Message
            End Try

            Return reader
        End Function


        Public Shared Function ExecuteNonQuery(ByVal Sql As String) As ExecuteDataInfo
            Return ExecuteNonQuery(Sql, Nothing, Nothing)
        End Function
        Public Shared Function ExecuteNonQuery(sql As String, cmdParms() As SqlParameter) As ExecuteDataInfo
            Return ExecuteNonQuery(sql, Nothing, cmdParms)
        End Function
        Public Shared Function ExecuteNonQuery(ByVal Sql As String, ByVal conn As SqlConnection) As ExecuteDataInfo
            Return ExecuteNonQuery(Sql, conn, Nothing, Nothing)
        End Function
        Public Shared Function ExecuteNonQuery(ByVal Sql As String, ByVal trans As SqlTransaction) As ExecuteDataInfo
            Return ExecuteNonQuery(Sql, Nothing, trans, Nothing)
        End Function
        Public Shared Function ExecuteNonQuery(ByVal Sql As String, ByVal trans As SqlTransaction, ByVal cmdParms() As SqlParameter) As ExecuteDataInfo
            Return ExecuteNonQuery(Sql, Nothing, trans, cmdParms)
        End Function
        Public Shared Function ExecuteNonQuery(ByVal Sql As String, ByVal conn As SqlConnection, ByVal trans As SqlTransaction, ByVal cmdParms() As SqlParameter) As ExecuteDataInfo
            Dim ret As New ExecuteDataInfo
            Dim command As New SqlCommand
            ret.SqlStatement = Sql

            Try
                If trans Is Nothing Then
                    conn = GetConnection()
                    BuildCommand(command, conn, trans, CommandType.Text, Sql, cmdParms)
                    ret.RecordEffected = command.ExecuteNonQuery()
                    ret.IsSuccess = True
                Else
                    If trans IsNot Nothing And conn Is Nothing Then
                        conn = trans.Connection
                    End If

                    BuildCommand(command, trans.Connection, trans, CommandType.Text, Sql, cmdParms)
                    ret.RecordEffected = command.ExecuteNonQuery()
                    ret.IsSuccess = True
                End If
            Catch ex As ApplicationException
                _err = Sql & " $$$$ " & ex.Message & " " & ex.StackTrace
                ret.ErrorMessage = "ApplicationException: " & _err
            Catch ex As SqlException
                _err = Sql & " $$$$ " & ex.Message & " " & ex.StackTrace
                ret.ErrorMessage = "SqlException: " & _err
            Catch ex As Exception
                _err = Sql & " $$$$ " & ex.Message & " " & ex.StackTrace
                ret.ErrorMessage = "Exception: " & _err
            End Try

            Return ret
        End Function

#Region "Set Parameter "
        Private Shared Function SetParameter(ParameterName As String, pType As SqlDbType, ParameterValue As Object) As SqlParameter
            Dim p As New SqlParameter(ParameterName, pType)
            If Convert.IsDBNull(ParameterValue) = False Then
                p.Value = ParameterValue
            Else
                p.Value = DBNull.Value
            End If
            Return p
        End Function

        Public Shared Function SetText(ParameterName As String, ParameterValue As String) As SqlParameter
            Return SetParameter(ParameterName, SqlDbType.VarChar, ParameterValue)
        End Function

        Public Shared Function SetInt(ParameterName As String, ParameterValue As Int16) As SqlParameter
            Return SetParameter(ParameterName, SqlDbType.Int, ParameterValue)
        End Function

        Public Shared Function SetBigInt(ParameterName As String, ParameterValue As Int64) As SqlParameter
            Return SetParameter(ParameterName, SqlDbType.BigInt, ParameterValue)
        End Function

        Public Shared Function SetFloat(ParameterName As String, ParameterValue As Double) As SqlParameter
            Return SetParameter(ParameterName, SqlDbType.Float, ParameterValue)
        End Function

        Public Shared Function SetDouble(ParameterName As String, ParameterValue As Double) As SqlParameter
            Return SetParameter(ParameterName, SqlDbType.Decimal, ParameterValue)
        End Function

        Public Shared Function SetDateTime(ParameterName As String, ParameterValue As DateTime) As SqlParameter
            Return SetParameter(ParameterName, SqlDbType.DateTime, ParameterValue)
        End Function

        Public Shared Function SetImage(ParameterName As String, ParameterValue As Image) As SqlParameter
            Return SetParameter(ParameterName, SqlDbType.Image, ParameterValue)
        End Function
#End Region

        Private Shared Sub BuildCommand(ByVal cmd As SqlCommand, ByVal conn As SqlConnection, ByVal trans As SqlTransaction, ByVal cmdType As CommandType, ByVal cmdText As String, ByVal cmdParms() As SqlParameter)
            If conn.State <> ConnectionState.Open Then
                Try
                    conn.Open()
                Catch ex As SqlException
                    'Throw New ApplicationException(GetExceptionMessage(ex), ex)
                    _err = ex.Message
                Catch ex As ApplicationException
                    'Throw (ex)
                    _err = ex.Message
                Catch ex As Exception
                    'Throw New ApplicationException(ErrorConnection, ex)
                    _err = ex.Message
                End Try
            End If

            Try
                cmd.Connection = conn
            Catch ex As Exception
                'Throw New ApplicationException(ErrorSetCommandConnection, ex)
                _err = ex.Message
            End Try
            cmd.CommandText = cmdText

            If trans IsNot Nothing Then
                cmd.Transaction = trans
            End If

            Try
                cmd.CommandType = cmdType
                cmd.CommandTimeout = 240
            Catch ex As ArgumentException
                'Throw New ApplicationException(ErrorInvalidCommandType, ex)
                _err = ex.Message
            End Try

            If cmdParms IsNot Nothing Then
                For Each parm As SqlParameter In cmdParms

                    Try
                        If parm IsNot Nothing Then
                            cmd.Parameters.Add(parm)
                        End If
                    Catch ex As ArgumentNullException
                        'Throw New ApplicationException(ErrorNullParameter, ex)
                        _err = ex.Message
                    Catch ex As ArgumentException
                        'Throw New ApplicationException(ErrorDuplicateParameter, ex)
                        _err = ex.Message
                    End Try
                Next
            End If
        End Sub

#Region " Encrypt/Decrypt "
        Private Shared EncryptionKey As String = "TIT_Encrypt2016"
        Public Shared Function EnCripPwd(ByVal passString As String) As String
            Dim AES As New System.Security.Cryptography.RijndaelManaged
            Dim Hash_AES As New System.Security.Cryptography.MD5CryptoServiceProvider
            Dim encrypted As String = ""
            Try
                Dim hash(31) As Byte
                Dim temp As Byte() = Hash_AES.ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(EncryptionKey))
                Array.Copy(temp, 0, hash, 0, 16)
                Array.Copy(temp, 0, hash, 15, 16)
                AES.Key = hash
                AES.Mode = Security.Cryptography.CipherMode.ECB
                Dim DESEncrypter As System.Security.Cryptography.ICryptoTransform = AES.CreateEncryptor
                Dim Buffer As Byte() = System.Text.ASCIIEncoding.ASCII.GetBytes(passString)
                encrypted = Convert.ToBase64String(DESEncrypter.TransformFinalBlock(Buffer, 0, Buffer.Length))

            Catch ex As Exception
            End Try

            Return encrypted
        End Function

        Public Shared Function DeCripPwd(ByVal passString As String) As String
            Dim AES As New System.Security.Cryptography.RijndaelManaged
            Dim Hash_AES As New System.Security.Cryptography.MD5CryptoServiceProvider
            Dim decrypted As String = ""
            Try
                Dim hash(31) As Byte
                Dim temp As Byte() = Hash_AES.ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(EncryptionKey))
                Array.Copy(temp, 0, hash, 0, 16)
                Array.Copy(temp, 0, hash, 15, 16)
                AES.Key = hash
                AES.Mode = Security.Cryptography.CipherMode.ECB
                Dim DESDecrypter As System.Security.Cryptography.ICryptoTransform = AES.CreateDecryptor
                Dim Buffer As Byte() = Convert.FromBase64String(passString)
                decrypted = System.Text.ASCIIEncoding.ASCII.GetString(DESDecrypter.TransformFinalBlock(Buffer, 0, Buffer.Length))
            Catch ex As Exception
            End Try

            Return decrypted
        End Function
#End Region

    End Class


End Namespace

