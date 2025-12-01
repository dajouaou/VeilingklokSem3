using Veilingklok.Core.Entities;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.Auth.Dtos;

namespace Veilingklok.Features.Auth.Services;

public class AuthService
{
    private readonly IGebruikerRepository _gebruikerRepo;
    private readonly PasswordService _passwordService;
    private readonly JwtService _jwtService;

    public AuthService(
        IGebruikerRepository gebruikerRepo,
        PasswordService passwordService,
        JwtService jwtService)
    {
        _gebruikerRepo = gebruikerRepo;
        _passwordService = passwordService;
        _jwtService = jwtService;
    }

    public async Task<string> RegisterAsync(string email, string password, string voornaam, string achternaam)
    {
        // Controleer of email al bestaat
        var bestaandeGebruiker = await _gebruikerRepo.GetByEmailAsync(email);
        if (bestaandeGebruiker != null)
            throw new Exception("Email bestaat al.");

        // Nieuwe gebruiker aanmaken
        var gebruiker = new Gebruiker
        {
            Email = email,
            Voornaam = voornaam,
            Achternaam = achternaam,
            PasswordHash = _passwordService.HashPassword(password)
        };

        await _gebruikerRepo.AddAsync(gebruiker);

        // JWT genereren
        return _jwtService.GenerateToken(gebruiker);
    }

    public async Task<string> LoginAsync(string email, string password)
    {
        var gebruiker = await _gebruikerRepo.GetByEmailAsync(email);
        if (gebruiker == null)
            throw new Exception("Ongeldige login.");

        bool wachtwoordCorrect = _passwordService.VerifyPassword(password, gebruiker.PasswordHash);
        if (!wachtwoordCorrect)
            throw new Exception("Ongeldige login.");

        return _jwtService.GenerateToken(gebruiker);
    }
}
