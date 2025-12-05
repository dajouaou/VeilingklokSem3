using Microsoft.AspNetCore.Mvc;
using Veilingklok.Core.Enums;
using Veilingklok.Core.Interfaces;
using Veilingklok.Features.Auth.Dtos;
using Veilingklok.Features.Auth.Services;

namespace Veilingklok.Features.Auth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly IGebruikerRepository _gebruikerRepo;

        public AuthController(AuthService authService, IGebruikerRepository gebruikerRepo)
        {
            _authService = authService;
            _gebruikerRepo = gebruikerRepo;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest dto)
        {
            try
            {
                // Beperk rol tot Koper of Aanvoerder
                if (dto.Rol != UserRole.Koper && dto.Rol != UserRole.Aanvoerder)
                    return BadRequest(new { message = "Ongeldige rol." });

                string token = await _authService.RegisterAsync(
                    dto.Email,
                    dto.Password,
                    dto.Voornaam,
                    dto.Achternaam,
                    dto.Rol
                );

                return Ok(new { token, role = dto.Rol.ToString() });


            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            try
            {
                var gebruiker = await _gebruikerRepo.GetByEmailAsync(request.Email);
                if (gebruiker == null)
                    return BadRequest(new { message = "Ongeldige login" });

                string token = await _authService.LoginAsync(request.Email, request.Password);

                return Ok(new { token, role = gebruiker.Rol.ToString() });  // ⭐ TOEGEVOEGD
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

    }
}
