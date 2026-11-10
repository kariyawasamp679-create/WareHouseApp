using System;
using System.Security.Cryptography;

namespace WareHouseApp.Security
{
    /// <summary>Salted PBKDF2 password hashing - passwords are never stored in plain text.</summary>
    public static class PasswordHasher
    {
        private const int Iterations = 10000;

        public static string GenerateSalt()
        {
            byte[] salt = new byte[16];
            using (var rng = new RNGCryptoServiceProvider()) rng.GetBytes(salt);
            return Convert.ToBase64String(salt);
        }

        public static string Hash(string password, string salt)
        {
            using (var kdf = new Rfc2898DeriveBytes(password, Convert.FromBase64String(salt), Iterations))
                return Convert.ToBase64String(kdf.GetBytes(32));
        }

        public static bool Verify(string password, string salt, string expectedHash)
        {
            string actual = Hash(password, salt);
            if (actual.Length != expectedHash.Length) return false;
            int diff = 0;
            for (int i = 0; i < actual.Length; i++) diff |= actual[i] ^ expectedHash[i];
            return diff == 0;
        }
    }
}
