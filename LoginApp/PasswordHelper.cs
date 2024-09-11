using System;
using System.Security.Cryptography;

public static class PasswordHelper
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
