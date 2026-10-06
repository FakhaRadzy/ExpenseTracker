using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace ExpenseTracker.Infrastructure.Identity
{
    public class JwtTokenGenerator(IOptions<JwtSettings> options, TimeProvider timeProvider)
    {
        public (string Token, DateTime ExpiresAtUtc) Generate(AppUser user)
        {
            var settings = options.Value;
            var now = timeProvider.GetUtcNow().UtcDateTime;
            var expiresAtUtc = now.AddMinutes(settings.ExpiryMinutes);

            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = settings.Issuer,
                Audience = settings.Audience,
                IssuedAt = now,
                NotBefore = now,
                Expires = expiresAtUtc,
                Subject = new ClaimsIdentity(
                    [
                        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),         // The user id
                        new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())   // Unique token id

                    ]),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Secret)), SecurityAlgorithms.HmacSha256)
            };

            return (new JsonWebTokenHandler().CreateToken(descriptor), expiresAtUtc);

        }
    }
}
