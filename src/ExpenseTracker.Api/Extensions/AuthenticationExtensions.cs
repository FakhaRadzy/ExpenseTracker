using System.Text;
using ExpenseTracker.Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace ExpenseTracker.Api.Extensions
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwt = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>() ?? throw new InvalidOperationException("Missing 'jwt' section configuration.");

            // Fail fast at startup  with a clear message instead  of a confusing crypto error later
            if (Encoding.UTF8.GetByteCount(jwt.Secret) < 32)
            {
                throw new InvalidOperationException("Jwt: Secret is missing or shorter than 32 bytes. Set it with: dotnet user-secrets set \"Jwt:Secret\" \"<key>\"");
            }

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;           // Keep claim names as-is ("sub", not a long URL)

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwt.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwt.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Secret)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30)        // Default is 5 minutes — too generous
                };
            });

            services.AddAuthorization();

            return services;
        }
    }
}
