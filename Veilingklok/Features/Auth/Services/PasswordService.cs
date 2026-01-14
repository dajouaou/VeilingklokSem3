namespace Veilingklok.Features.Auth.Services 
{ 
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Veilingklok.Core.Entities;

    // Service die wachtwoorden veilig hasht en controleert
    public class PasswordService
    {
        // Maakt van een wachtwoord een veilige hash met salt
        public string HashPassword(string password)
        {
            // Genereert een random salt
            byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);

            // Hasht het wachtwoord met PBKDF2
            string hashed = Convert.ToBase64String(
                KeyDerivation.Pbkdf2(
                    password,
                    salt,
                    KeyDerivationPrf.HMACSHA256,
                    10000,
                    256 / 8));

            // Combineert salt en hash in één string voor opslag
            return $"{Convert.ToBase64String(salt)}.{hashed}";
        }

        // Checkt of een wachtwoord klopt met de opgeslagen hash
        public bool VerifyPassword(string password, string hashedPasswordWithSalt)
        {
            // Splitst salt en hash uit de databasewaarde
            var parts = hashedPasswordWithSalt.Split('.');
            var salt = Convert.FromBase64String(parts[0]);
            var hash = parts[1];

            // Hasht het ingevoerde wachtwoord opnieuw met dezelfde salt
            string hashedInput = Convert.ToBase64String(
                KeyDerivation.Pbkdf2(
                    password,
                    salt,
                    KeyDerivationPrf.HMACSHA256,
                    10000,
                    256 / 8));

            // Vergelijkt de hashes
            return hash == hashedInput;
        }
    }

}
