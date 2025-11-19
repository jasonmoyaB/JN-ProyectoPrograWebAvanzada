using System.Security.Cryptography;
using System.Text;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Helpers
{
    public static class PasswordHelper
    {
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iterations = 10000;

        public static string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("La contraseña no puede estar vacía.", nameof(password));

            using var rng = RandomNumberGenerator.Create();
            byte[] salt = new byte[SaltSize];
            rng.GetBytes(salt);

            byte[] hash = HashPasswordWithSalt(password, salt, Iterations);

            byte[] hashBytes = new byte[SaltSize + HashSize];
            Array.Copy(salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

            return Convert.ToBase64String(hashBytes);
        }

        public static bool VerifyPassword(string password, string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(passwordHash))
                return false;

            bool isLegacyHash = IsLegacyHashFormat(passwordHash);

            if (isLegacyHash)
            {
                return VerifyPasswordLegacy(password, passwordHash);
            }

            try
            {
                byte[] hashBytes = Convert.FromBase64String(passwordHash);

                if (hashBytes.Length != SaltSize + HashSize)
                {
                    return VerifyPasswordLegacy(password, passwordHash);
                }

                byte[] salt = new byte[SaltSize];
                Array.Copy(hashBytes, 0, salt, 0, SaltSize);

                byte[] storedHash = new byte[HashSize];
                Array.Copy(hashBytes, SaltSize, storedHash, 0, HashSize);

                byte[] computedHash = HashPasswordWithSalt(password, salt, Iterations);

                return ConstantTimeEquals(storedHash, computedHash);
            }
            catch
            {
                return VerifyPasswordLegacy(password, passwordHash);
            }
        }

        private static bool IsLegacyHashFormat(string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(passwordHash))
                return false;

            string cleanHash = passwordHash.Trim().ToLowerInvariant();

            if (cleanHash.Length >= 32 && cleanHash.Length <= 128)
            {
                foreach (char c in cleanHash)
                {
                    if (!((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f')))
                    {
                        return false;
                    }
                }
                return true;
            }

            return false;
        }

        private static bool VerifyPasswordLegacy(string password, string passwordHash)
        {
            try
            {
                using var sha = SHA256.Create();
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha.ComputeHash(bytes);
                var hashString = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                
                string normalizedStored = passwordHash.Trim().ToLowerInvariant();
                
                return ConstantTimeEqualsString(hashString, normalizedStored);
            }
            catch
            {
                return false;
            }
        }

        private static bool ConstantTimeEqualsString(string a, string b)
        {
            if (a.Length != b.Length)
                return false;

            uint diff = 0;
            int minLength = Math.Min(a.Length, b.Length);
            for (int i = 0; i < minLength; i++)
            {
                diff |= (uint)(a[i] ^ b[i]);
            }

            return diff == 0 && a.Length == b.Length;
        }

        private static byte[] HashPasswordWithSalt(string password, byte[] salt, int iterations)
        {
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            return pbkdf2.GetBytes(HashSize);
        }

        private static bool ConstantTimeEquals(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;

            uint diff = 0;
            for (int i = 0; i < a.Length; i++)
            {
                diff |= (uint)(a[i] ^ b[i]);
            }

            return diff == 0;
        }
    }
}
