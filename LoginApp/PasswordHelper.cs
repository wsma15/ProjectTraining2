using System;
using System.Security.Cryptography;
using System.IO;
using System.Text;

public class PasswordHelper
{
    // Example key and IV
    private static readonly byte[] Key = Encoding.UTF8.GetBytes("Your16ByteKey123"); // 16 bytes for AES-128
    private static readonly byte[] Iv = Encoding.UTF8.GetBytes("Your16ByteIV1234"); // 16 bytes IV
    
    // Encrypt a plaintext string
    public static string HashPassword(string plainText)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = Key;
            aes.IV = Iv;

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter sw = new StreamWriter(cs))
                    {
                        sw.Write(plainText);
                    }
                }

                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }

    // Decrypt an encrypted string
    public static string Decrypt(string cipherText)
    {
        byte[] cipherBytes = Convert.FromBase64String(cipherText);

        using (Aes aes = Aes.Create())
        {
            aes.Key = Key;
            aes.IV = Iv;

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using (MemoryStream ms = new MemoryStream(cipherBytes))
            {
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader sr = new StreamReader(cs))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
        }
    }


}

/*public static class PasswordHelper
{
    private const int SaltSize = 16; // 128 bit
    private const int HashSize = 20; // 160 bit
    private const int Iterations = 10000;

    public static string HashPassword(string password)
    {
        // Generate a random salt
        byte[] salt = new byte[SaltSize];
        using (var rng = new RNGCryptoServiceProvider())
        {
            rng.GetBytes(salt);
        }

        // Generate the hash
        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations);
        byte[] hash = pbkdf2.GetBytes(HashSize);

        // Combine salt and hash
        byte[] hashBytes = new byte[SaltSize + HashSize];
        Array.Copy(salt, 0, hashBytes, 0, SaltSize);
        Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

        // Convert to base64
        return Convert.ToBase64String(hashBytes);
    }

    public static bool VerifyPassword(string password, string hashedPassword)
    {
        // Extract salt and hash
        byte[] hashBytes = Convert.FromBase64String(hashedPassword);
        byte[] salt = new byte[SaltSize];
        Array.Copy(hashBytes, 0, salt, 0, SaltSize);
        byte[] storedHash = new byte[HashSize];
        Array.Copy(hashBytes, SaltSize, storedHash, 0, HashSize);

        // Generate the hash with the provided password
        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations);
        byte[] hash = pbkdf2.GetBytes(HashSize);

        // Compare the hashes
        for (int i = 0; i < HashSize; i++)
        {
            if (hash[i] != storedHash[i])
            {
                return false;
            }
        }
        return true;
    }
}
*/