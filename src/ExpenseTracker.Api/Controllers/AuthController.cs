using ExpenseTracker.Application.Auth;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request, CancellationToken ct)
        {
            return Ok(await authService.RegisterAsync(request, ct));
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken ct)
        {
            return Ok(await authService.LoginAsync(request, ct));
        }
    }
}
