using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiAuth2F.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);
            if (!string.IsNullOrEmpty(result.TwoFactorSecret) && !string.IsNullOrEmpty(result.QrCodeUrl))
            {
                // Devuelve el secreto y la URL para QR
                return Ok(result);
            }
            if (result.Message.Contains("ya está registrado"))
                return Conflict(result);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);
            if (!result.Requires2FA && result.TempToken == null)
                return Unauthorized(result);
            return Ok(result);
        }

        [HttpPost("verify-2fa")]
        public async Task<IActionResult> Verify2FA([FromBody] TwoFactorRequest request)
        {
            var result = await _authService.Verify2FAAsync(request);
            if (!result.Success)
                return Unauthorized(result);
            return Ok(result);
        }
    }
}
