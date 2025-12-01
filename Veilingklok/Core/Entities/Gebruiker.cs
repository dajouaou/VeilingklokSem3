namespace Veilingklok.Core.Entities
{
    public class Gebruiker
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Voornaam { get; set; } = string.Empty;
        public string Achternaam { get; set; } = string.Empty;
        public string Rol { get; set; } = "Gebruiker";
    }

}
