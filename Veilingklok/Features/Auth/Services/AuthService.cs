using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Entities;
using Veilingklok.Core.Interfaces;
using Veilingklok.Core.Shared;
using Veilingklok.Features.Auth.Dtos;
using Veilingklok.Infrastructure.Auth;
using Veilingklok.Infrastructure.Database;

namespace Veilingklok.Features.Auth.Services;

public sealed class AuthService : IAuthService
{
    private readonly MyContext _db;
    private readonly IMapper _mapper;
    private readonly JwtTokenGenerator _jwt;
    private readonly IValidator<RegisterDto> _registerValidator;
    private readonly IValidator<LoginDto> _loginValidator;

    public AuthService(
        MyContext db,
        IMapper mapper,
        JwtTokenGenerator jwt,
        IValidator<RegisterDto> registerValidator,
        IValidator<LoginDto> loginValidator)
    {
        _db = db;
        _mapper = mapper;
        _jwt = jwt;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
    }

    public async Task<Result<AuthResponseDto>> RegisterAsync(RegisterDto dto)
    {
        var validation = await _registerValidator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            var msg = string.Join(" | ", validation.Errors.Select(e => e.ErrorMessage));
            return Result.Fail<AuthResponseDto>(msg);
        }

        var username = dto.Username.Trim();
        var email = dto.Email.Trim().ToLowerInvariant();

        var exists = await _db.Gebruikers.AnyAsync(u =>
            u.Username == username || u.Email == email);

        if (exists)
            return Result.Fail<AuthResponseDto>("Username of e-mail is al in gebruik.");

        var user = new Gebruiker
        {
            Username = username,
            Email = email,
            Naam = dto.Naam?.Trim(),
            Role = dto.Role,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Wachtwoord),
            CreatedAtUtc = DateTime.UtcNow
        };

        _db.Gebruikers.Add(user);
        await _db.SaveChangesAsync();

        var token = _jwt.GenerateToken(user);

        return Result.Success(new AuthResponseDto
        {
            GebruikerId = user.Id,
            Username = user.Username,
            Role = user.Role,
            Token = token
        });
    }

    public async Task<Result<AuthResponseDto>> LoginAsync(LoginDto dto)
    {
        var validation = await _loginValidator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            var msg = string.Join(" | ", validation.Errors.Select(e => e.ErrorMessage));
            return Result.Fail<AuthResponseDto>(msg);
        }

        var lookup = dto.UsernameOrEmail.Trim();

        var user = await _db.Gebruikers.FirstOrDefaultAsync(u =>
            u.Username == lookup || u.Email == lookup);

        if (user is null)
            return Result.Fail<AuthResponseDto>("Ongeldige inloggegevens.");

        var ok = BCrypt.Net.BCrypt.Verify(dto.Wachtwoord, user.PasswordHash);
        if (!ok)
            return Result.Fail<AuthResponseDto>("Ongeldige inloggegevens.");

        var token = _jwt.GenerateToken(user);

        return Result.Success(new AuthResponseDto
        {
            GebruikerId = user.Id,
            Username = user.Username,
            Role = user.Role,
            Token = token
        });
    }

    public async Task<Result<GebruikerDto>> GetSelfAsync(int gebruikerId)
    {
        var user = await _db.Gebruikers
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == gebruikerId);

        if (user is null)
            return Result.Fail<GebruikerDto>("Gebruiker niet gevonden.");

        return Result.Success(_mapper.Map<GebruikerDto>(user));
    }
}
