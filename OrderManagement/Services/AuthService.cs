using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Orders.Config;
using Orders.DTOs.Request;
using Orders.DTOs.Response;

namespace Orders.Services
{
    public class AuthService : Interfaces.IAuthService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly List<Models.User> _users;

        public AuthService(JwtSettings jwtSettings)
        {
            _jwtSettings = jwtSettings;
            _users = SeedUsers.Get();
        }

        public Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
        {
            var user = _users.FirstOrDefault(u =>
                u.Username == request.Username && u.Password == request.Password);

            if (user == null)
                return Task.FromResult<LoginResponseDto?>(null);

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.Key);
            var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Name),
                new(ClaimTypes.Role, user.Role),
                new("username", user.Username)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiresAt,
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Task.FromResult<LoginResponseDto?>(new LoginResponseDto
            {
                Token = tokenHandler.WriteToken(token),
                ExpiresAt = expiresAt,
                Role = user.Role,
                Name = user.Name,
                UserId = user.Id
            });
        }
    }
}
