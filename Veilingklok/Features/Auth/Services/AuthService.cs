using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.Auth.Dtos;
using Veilingklok.Infrastructure.Repositories;

namespace Veilingklok.Features.Auth.Services;

// Service die alle auth-logica bevat (registreren, inloggen, token maken)
public class AuthService
{
    private readonly IGebruikerRepository _gebruikerRepo;
    private readonly PasswordService _passwordService;
    private readonly JwtService _jwtService;

    // Injecteert repository, password-logica en JWT-logica
    public AuthService(
        IGebruikerRepository gebruikerRepo,
        PasswordService passwordService,
        JwtService jwtService)
    {
        _gebruikerRepo = gebruikerRepo;
        _passwordService = passwordService;
        _jwtService = jwtService;
    }

    // Registreert een nieuwe gebruiker en geeft een JWT token terug
    public async Task<string> RegisterAsync(
        string email,
        string password,
        string voornaam,
        string achternaam,
        UserRole rol)
    {
        // Checkt of het emailadres al bestaat
        var bestaand = await _gebruikerRepo.GetByEmailAsync(email);
        if (bestaand != null)
            throw new Exception("Email is al in gebruik.");

        // Maakt een nieuwe gebruiker aan
        var user = new Gebruiker
        {
            Email = email,
            Voornaam = voornaam,
            Achternaam = achternaam,
            Rol = rol,
            PasswordHash = _passwordService.HashPassword(password),
            CreatedAtUtc = DateTime.UtcNow
        };

        // Slaat gebruiker op in de database
        await _gebruikerRepo.AddAsync(user);

        // Maakt het bijbehorende rol-object aan (koper, aanvoerder of veilingmeester)
        switch (rol)
        {
            case UserRole.Koper:
                await _gebruikerRepo.CreateKoperAsync(new Koper
                {
                    GebruikerId = user.Id,
                    Naam = $"{voornaam} {achternaam}"
                });
                break;

            case UserRole.Aanvoerder:
                await _gebruikerRepo.CreateAanvoerderAsync(new Aanvoerder
                {
                    GebruikerId = user.Id,
                    Naam = $"{voornaam} {achternaam}"
                });
                break;

            case UserRole.Veilingmeester:
                await _gebruikerRepo.CreateVeilingmeesterAsync(new Veilingmeester
                {
                    GebruikerId = user.Id,
                    Naam = $"{voornaam} {achternaam}"
                });
                break;
        }

        // Genereert en geeft een JWT token terug
        return _jwtService.GenerateToken(user);
    }


    // Logt een gebruiker in en geeft een JWT token terug
    public async Task<string> LoginAsync(string email, string password)
    {
        // Haalt gebruiker op via email
        var gebruiker = await _gebruikerRepo.GetByEmailAsync(email);
        if (gebruiker == null)
            throw new Exception("Ongeldige login.");

        // Checkt of het wachtwoord klopt
        bool ok = _passwordService.VerifyPassword(password, gebruiker.PasswordHash);
        if (!ok)
            throw new Exception("Ongeldige login.");

        // Genereert en geeft een JWT token terug
        return _jwtService.GenerateToken(gebruiker);
    }
}
