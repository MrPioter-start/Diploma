using Kursach.Database.WarehouseApp.Database;
using System;
using System.Data;
using System.Security.Cryptography;
using System.Text;

public static class PasswordHelper
{
    private const int SaltSize = 16;
    private const int HashSize = 20;
    private const int Iterations = 10000; 

    public static string HashPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
        {
            throw new ArgumentNullException(nameof(password), "Пароль не может быть пустым.");
        }

        byte[] salt = GenerateSalt();

        byte[] hash = GenerateHash(password, salt);

        return Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);
    }

    public static bool VerifyPassword(string password, string hashedPassword)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashedPassword))
        {
            throw new ArgumentNullException("Пароль или хэш не могут быть пустыми.");
        }

        var parts = hashedPassword.Split(':');
        if (parts.Length != 2)
        {
            throw new ArgumentException("Неверный формат хэша.");
        }

        byte[] salt = Convert.FromBase64String(parts[0]);
        byte[] storedHash = Convert.FromBase64String(parts[1]);

        byte[] hashToVerify = GenerateHash(password, salt);

        return ByteArraysEqual(storedHash, hashToVerify);
    }

    private static byte[] GenerateSalt()
    {
        using (var rng = RandomNumberGenerator.Create())
        {
            byte[] salt = new byte[SaltSize];
            rng.GetBytes(salt);
            return salt;
        }
    }

    private static byte[] GenerateHash(string password, byte[] salt)
    {
        using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations))
        {
            return pbkdf2.GetBytes(HashSize);
        }
    }

    private static bool ByteArraysEqual(byte[] a, byte[] b)
    {
        if (a.Length != b.Length)
        {
            return false;
        }

        uint diff = 0;
        for (int i = 0; i < a.Length; i++)
        {
            diff |= (uint)(a[i] ^ b[i]);
        }

        return diff == 0;
    }
}

