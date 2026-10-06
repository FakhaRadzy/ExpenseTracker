

using Microsoft.AspNetCore.DataProtection;

namespace ExpenseTracker.Infrastructure.Identity
{
    public class JwtSettings
    {
        public const string SectionName = "Jwt";

        public string Issuer { get; init; } = string.Empty;         // Who create the token 
        public string Audience { get; init; } = string.Empty;       // Who the token is for
        public string Secret { get; init; } = string.Empty;         // signing key — from User Secrets, never Git
        public int ExpiryMinutes { get; init; } = 60;
    }
}
