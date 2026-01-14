using Microsoft.AspNetCore.Mvc;
using Veilingklok.Core.Enums;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.Auth.Dtos;
using Veilingklok.Features.Auth.Services;
using Microsoft.EntityFrameworkCore;

namespace Veilingklok.Features.Auth.Controllers
{
    // Controller voor alles wat met registreren en inloggen te maken heeft
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly IGebruikerRepository _gebruikerRepo;

        // Injecteert de auth-logica en de gebruiker-repository
        public AuthController(AuthService authService, IGebruikerRepository gebruikerRepo)
        {
            _authService = authService;
            _gebruikerRepo = gebruikerRepo;
        }


        // Endpoint om een nieuwe gebruiker te registreren
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest dto)
        {
            // Checkt of er al een gebruiker met deze email bestaat
            var existingUser = await _gebruikerRepo.GetByEmailAsync(dto.Email);

            if (existingUser != null)
            {
                return BadRequest(new { message = "Gebruiker bestaat al" });
            }

            try
            {
                // Checkt of de rol geldig is
                if (dto.Rol != UserRole.Koper &&
                    dto.Rol != UserRole.Aanvoerder &&
                    dto.Rol != UserRole.Veilingmeester)
                {
                    return BadRequest(new { message = "Ongeldige rol." });
                }

                // Maakt een nieuwe gebruiker aan en genereert een JWT token
                string token = await _authService.RegisterAsync(
                    dto.Email,
                    dto.Password,
                    dto.Voornaam,
                    dto.Achternaam,
                    dto.Rol
                );

                // Stuurt token en rol terug naar de client
                return Ok(new { token, role = dto.Rol.ToString() });
            }
            catch (Exception e)
            {
                // Stuurt foutmelding terug als registreren faalt
                return BadRequest(new { message = e.Message });
            }
        }



        // Endpoint om in te loggen
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            try
            {
                // Haalt gebruiker op via email
                var gebruiker = await _gebruikerRepo.GetByEmailAsync(request.Email);
                if (gebruiker == null)
                    return BadRequest(new { message = "Ongeldige login" });

                // Checkt wachtwoord en maakt JWT token
                string token = await _authService.LoginAsync(request.Email, request.Password);

                // Stuurt token en rol terug
                return Ok(new { token, role = gebruiker.Rol.ToString() });  
            }
            catch (Exception e)
            {
                // Stuurt foutmelding terug als login faalt
                return BadRequest(new { message = e.Message });
            }
        }

    }
}
