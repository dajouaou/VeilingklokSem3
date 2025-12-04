using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.Auth.Dtos;
using Veilingklok.Infrastructure.Repositories;

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

    public async Task<string> RegisterAsync(string email, string password, string voornaam, string achternaam, UserRole rol)
    {
        var existingUser = await _gebruikerRepo.GetByEmailAsync(email);
        if (existingUser != null)
            throw new Exception("Email is al in gebruik.");

        var hashed = _passwordService.HashPassword(password);

        var user = new Gebruiker
        {
            Email = email,
            Voornaam = voornaam,
            Achternaam = achternaam,
            Rol = rol,
            PasswordHash = hashed,
            CreatedAtUtc = DateTime.UtcNow
        };

        if (rol == UserRole.Aanvoerder)
        {
            user.Aanvoerder = new Aanvoerder
            {
                Naam = $"{voornaam} {achternaam}",
                Email = email
            };
        }

        await _gebruikerRepo.AddAsync(user);

        var token = _jwtService.GenerateToken(user);
        return token;
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
