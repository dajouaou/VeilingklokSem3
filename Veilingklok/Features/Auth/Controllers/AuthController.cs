using Microsoft.AspNetCore.Mvc;
using Veilingklok.Features.Auth.Services;
using Veilingklok.Features.Auth.Dtos;

namespace Veilingklok.Features.Auth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            try
            {
                // Gebruik 'request', niet 'dto'
                string token = await _authService.RegisterAsync(
                    request.Email,
                    request.Password,
                    request.Voornaam,
                    request.Achternaam
                );

                return Ok(new { token });
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
                string token = await _authService.LoginAsync(request.Email, request.Password);
                return Ok(new { token });
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }
    }
}
