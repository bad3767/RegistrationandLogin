using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockManagement.DTOs;
using StockManagement.Services;

namespace StockManagement.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var user = await _authService.Register(request);
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var token = await _authService.Login(request);
            if (token == null) return Unauthorized();
            return Ok(new { Token = token });
        }
        [HttpGet("get-user")]
        // [Authorize] 
        public async Task<IActionResult> GetUser()
        {
            try
            {
                var authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
                if (authHeader == null || !authHeader.StartsWith("Bearer "))
                    return Unauthorized("Missing or invalid Authorization header.");

                var token = authHeader.Substring("Bearer ".Length).Trim();
                Console.WriteLine($"Token received: {token}");

                var email = _authService.DecodeToken(token);
                Console.WriteLine($"email : {email}");
                if (email == null)
                    return Unauthorized("Invalid token.");

                var user = await _authService.GetUserByEmail(email);
                if (user == null)
                    return NotFound("User not found.");

                var userResponse = new UserResponse
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Role = user.Role
                };

                Response.Headers["Authorization"] = $"Bearer {token}";

                return Ok(userResponse);
            }
            catch
            {
                return Unauthorized("Error decoding token.");
            }
        }



    }
}
