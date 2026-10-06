using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Interfaces;
using System.IdentityModel.Tokens.Jwt;

namespace ExpenseTracker.Api.Services
{
    public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
    {
        public Guid UserId
        {
            get
            {
                var subject = httpContextAccessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

                return Guid.TryParse(subject, out var userId) ? userId : throw new UnauthorizedException("You must be logged in.");
            }
        }
    }
}
