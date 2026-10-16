using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using TicketSistemi.DTOs.Auth;
using TicketSistemi.Models;
using TicketSistemi.Services.Abstractions;

namespace TicketSistemi.Services.Implementations
{
    public sealed class TokenService(IConfiguration configuration) : ITokenService
    {
        public AuthResponse CreateToken(ApplicationUser user, IList<string> roles)
        {
            var jwt = configuration.GetSection("Jwt");
            var now = DateTimeOffset.UtcNow;
            var expiresAt = now.AddMinutes(jwt.GetValue<int>("ExpiresMinutes"));
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.UserName!),
                new(ClaimTypes.Email, user.Email!),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            claims.AddRange(roles.Select(role=> new Claim(ClaimTypes.Role, role)));
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"],
                audience: jwt["Audience"],
                claims: claims,
                notBefore: now.UtcDateTime,
                expires: expiresAt.UtcDateTime,
                signingCredentials: credentials
                );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return new AuthResponse(tokenString, expiresAt);
        }
    }
}
