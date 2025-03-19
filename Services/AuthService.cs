using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StockManagement.Data;
using StockManagement.DTOs;
using StockManagement.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StockManagement.Services
{
    public class AuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly EncryptionService _encryptionService;
        private readonly IConfiguration _config;

        public AuthService(ApplicationDbContext context, EncryptionService encryptionService, IConfiguration config)
        {
            _context = context;
            _encryptionService = encryptionService;
            _config = config;
        }

        public async Task<UserResponse> Register(RegisterRequest request)
        {
            var encryptedPassword = _encryptionService.Encrypt(request.Password);
            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordEncrypted = encryptedPassword
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserResponse { Id = user.Id, Username = user.Username, Email = user.Email, Role = user.Role };
        }

        public async Task<string> Login(LoginRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null || !_encryptionService.Decrypt(user.PasswordEncrypted).Equals(request.Password))
                return null;

            return GenerateJwtToken(user);
        }

        // Get user by email
        public async Task<UserResponse> GetUserByEmail(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return null;

            return new UserResponse
            {
                Username = user.Username,
                Email = user.Email,
                Role = user.Role
            };
        }




        private string GenerateJwtToken(User user)
{
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    var claims = new[]
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),  // Store User ID
        new Claim(JwtRegisteredClaimNames.Email, user.Email),        // ✅ Store Email
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.Role, user.Role)
    };

    var token = new JwtSecurityToken(
        _config["JwtSettings:Issuer"], 
        _config["JwtSettings:Issuer"], 
        claims, 
        expires: DateTime.UtcNow.AddHours(3), 
        signingCredentials: creds
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}


        // Decode JWT Token and extract email
        public string DecodeToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["JwtSettings:Key"]); // FIXED: Use _config["Jwt:Key"]
            Console.WriteLine("key: ", key);

            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            }
            catch
            {
                return null;
            }
        }


    }
}
