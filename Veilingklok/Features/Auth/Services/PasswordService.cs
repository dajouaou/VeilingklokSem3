namespace Veilingklok.Features.Auth.Services
{
    using System.Security.Cryptography;
    using Microsoft.AspNetCore.Cryptography.KeyDerivation;
    using Veilingklok.Core.Entities;

    public class PasswordService
    {
        public string HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);

            string hashed = Convert.ToBase64String(
                KeyDerivation.Pbkdf2(
                    password,
                    salt,
                    KeyDerivationPrf.HMACSHA256,
                    10000,
                    256 / 8));

            return $"{Convert.ToBase64String(salt)}.{hashed}";
        }

        public bool VerifyPassword(string password, string hashedPasswordWithSalt)
        {
            var parts = hashedPasswordWithSalt.Split('.');
            var salt = Convert.FromBase64String(parts[0]);
            var hash = parts[1];

            string hashedInput = Convert.ToBase64String(
                KeyDerivation.Pbkdf2(
                    password,
                    salt,
                    KeyDerivationPrf.HMACSHA256,
                    10000,
                    256 / 8));

            return hash == hashedInput;
        }
    }

}