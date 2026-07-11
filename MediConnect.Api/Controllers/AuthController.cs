using Microsoft.AspNetCore.Mvc;
using MediConnect.Api.Dtos;
using MediConnect.Api.Services;

namespace MediConnect.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var success = await _authService.RegisterAsync(request);
            if (!success) return BadRequest(new { message = "Email already registered." });
            return Ok(new { message = "Registration successful." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);
            if (result is null) return Unauthorized(new { message = "Invalid email or password." });
            return Ok(result);
        }
    }
}
