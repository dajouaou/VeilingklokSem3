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

    public async Task<string> RegisterAsync(
        string email,
        string password,
        string voornaam,
        string achternaam,
        UserRole rol)
    {
        var bestaand = await _gebruikerRepo.GetByEmailAsync(email);
        if (bestaand != null)
            throw new Exception("Email is al in gebruik.");

        var user = new Gebruiker
        {
            Email = email,
            Voornaam = voornaam,
            Achternaam = achternaam,
            Rol = rol,
            PasswordHash = _passwordService.HashPassword(password),
            CreatedAtUtc = DateTime.UtcNow
        };

        await _gebruikerRepo.AddAsync(user);

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
                await _gebruikerRepo.CreateVeilingmeesterAsync(new VM
                {
                    GebruikerId = user.Id,
                    Naam = $"{voornaam} {achternaam}"
                });
                break;
        }

        return _jwtService.GenerateToken(user);
    }


    public async Task<string> LoginAsync(string email, string password)
    {
        var gebruiker = await _gebruikerRepo.GetByEmailAsync(email);
        if (gebruiker == null)
            throw new Exception("Ongeldige login.");

        bool ok = _passwordService.VerifyPassword(password, gebruiker.PasswordHash);
        if (!ok)
            throw new Exception("Ongeldige login.");

        return _jwtService.GenerateToken(gebruiker);
    }
}
