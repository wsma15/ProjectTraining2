Imports System.Security.Cryptography
Imports System.IO
Imports System.Text

Public Class PasswordHelper
    Private Shared ReadOnly Key As Byte() = Encoding.UTF8.GetBytes("Your16ByteKey123") ' 16 bytes for AES-128
    Private Shared ReadOnly Iv As Byte() = Encoding.UTF8.GetBytes("Your16ByteIV1234")  ' 16 bytes IV

    ' Encrypt a plaintext string
    Public Shared Function HashPassword(ByVal plainText As String) As String
        Using aes As Aes = aes.Create()
            aes.Key = Key
            aes.IV = Iv
            Dim encryptor As ICryptoTransform = aes.CreateEncryptor(aes.Key, aes.IV)
            Using ms As New MemoryStream()
                Using cs As New CryptoStream(ms, encryptor, CryptoStreamMode.Write)
                    Using sw As New StreamWriter(cs)
                        sw.Write(plainText)
                    End Using
                End Using
                Return Convert.ToBase64String(ms.ToArray())
            End Using
        End Using
    End Function

    ' Decrypt an encrypted string
    Public Shared Function Decrypt(ByVal cipherText As String) As String
        Dim cipherBytes As Byte() = Convert.FromBase64String(cipherText)

        Using aes As Aes = aes.Create()
            aes.Key = Key
            aes.IV = Iv
            Dim decryptor As ICryptoTransform = aes.CreateDecryptor(aes.Key, aes.IV)
            Using ms As New MemoryStream(cipherBytes)
                Using cs As New CryptoStream(ms, decryptor, CryptoStreamMode.Read)
                    Using sr As New StreamReader(cs)
                        Return sr.ReadToEnd()
                    End Using
                End Using
            End Using
        End Using
    End Function
End Class
