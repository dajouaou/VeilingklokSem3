using System.Security.Claims;
using Veilingklok.Core.Enums;
using Veilingklok.Features.Auth.Services;

public class JwtServiceTests
{
    // Test: JwtService mag niet starten zonder Jwt:Key in de config
    // Doel: afdwingen dat de app veilig is geconfigureerd
    [Fact]
    public void Ctor_WithoutJwtKey_ThrowsException()
    {
        // Config zonder Jwt:Key
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>())
            .Build();

        // Verwacht een exception bij het maken van JwtService
        var ex = Assert.Throws<Exception>(() => new JwtService(config));

        // Check dat de foutmelding over de JWT key gaat
        Assert.Contains("JWT Key", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    // Test: GenerateToken geeft een geldig JWT formaat terug
    // Doel: controleren dat het resultaat echt een JWT is
    [Fact]
    public void GenerateToken_ReturnsValidJwtFormat()
    {
        // Service met geldige key en een testgebruiker
        var service = CreateJwtServiceWithKey("THIS_IS_A_TEST_KEY_MIN_32_CHARS_LONG!!");
        var gebruiker = CreateGebruiker();

        // Token genereren
        var token = service.GenerateToken(gebruiker);

        // Token mag niet leeg zijn en moet uit 3 delen bestaan
        Assert.False(string.IsNullOrWhiteSpace(token));
        Assert.Equal(3, token.Split('.').Length);
    }

    // Test: alle belangrijke claims zitten in het token
    // Doel: backend en frontend kunnen vertrouwen op token-inhoud
    [Fact]
    public void GenerateToken_IncludesRoleEmailNameIdClaims()
    {
        // Service en testgebruiker
        var service = CreateJwtServiceWithKey("THIS_IS_A_TEST_KEY_MIN_32_CHARS_LONG!!");
        var gebruiker = CreateGebruiker();

        // Token genereren
        var tokenString = service.GenerateToken(gebruiker);

        // Token uitlezen zonder validatie (alleen inhoud checken)
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(tokenString);

        // Helper om claims makkelijk te lezen
        string? GetClaim(string type) =>
            jwt.Claims.FirstOrDefault(c => c.Type == type)?.Value;

        // Check dat alle verwachte claims aanwezig en correct zijn
        Assert.Equal(gebruiker.Id.ToString(), GetClaim(ClaimTypes.NameIdentifier));
        Assert.Equal(gebruiker.Email, GetClaim(ClaimTypes.Email));
        Assert.Equal(gebruiker.Rol.ToString(), GetClaim(ClaimTypes.Role));
        Assert.Equal(gebruiker.Voornaam, GetClaim(ClaimTypes.GivenName));
        Assert.Equal(gebruiker.Achternaam, GetClaim(ClaimTypes.Surname));
        Assert.Equal($"{gebruiker.Voornaam} {gebruiker.Achternaam}", GetClaim(ClaimTypes.Name));
    }

    // Helper: maakt een JwtService met een testkey
    // Doel: hergebruik in meerdere tests, geen duplicatie
    private static JwtService CreateJwtServiceWithKey(string key)
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = key
            })
            .Build();

        return new JwtService(config);
    }

    // Helper: maakt een vaste testgebruiker
    // Doel: consistente input voor alle JWT tests
    private static Gebruiker CreateGebruiker()
    {
        return new Gebruiker
        {
            Id = 123,
            Email = "sofia@test.nl",
            Voornaam = "Sofia",
            Achternaam = "Qahqai",
            Rol = UserRole.Koper,
            PasswordHash = "irrelevant.for.jwt.tests",
            CreatedAtUtc = DateTime.UtcNow
        };
    }
}



