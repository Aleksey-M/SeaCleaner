using System;
using System.Security.Cryptography;
using System.Text;

namespace SeaCleaner.Server.Data
{
    internal static class PasswordHelper
    {
        public static string GetHash(string password)
        {
            using var hash = SHA256.Create();
            byte[] bytes = hash.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
