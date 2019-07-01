using System;

namespace RP_Server_Scripts.Authentication
{
    public sealed class BCrypPasswordService : IPasswordService
    {
        internal BCrypPasswordService() { }

        public string CreatePasswordHash(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(password));
            }

            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hash)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(password));
            }

            if (string.IsNullOrEmpty(hash))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(hash));
            }

            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}
