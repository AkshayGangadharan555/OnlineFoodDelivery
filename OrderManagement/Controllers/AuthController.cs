using Microsoft.AspNetCore.Mvc;
using Orders.DTOs.Request;
using Orders.DTOs.Response;
using Orders.Services.Interfaces;

namespace Orders.Controllers
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

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto request)
        {
            var result = await _authService.LoginAsync(request);

            if (result == null)
                return Unauthorized(new ApiResponse<string>(401, "Invalid username or password", ""));

            return Ok(new ApiResponse<LoginResponseDto>(200, "Login successful", result));
        }
    }
}
