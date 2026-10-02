using ExpenseTracker.Application.Common.Interfaces;
using Microsoft.Identity.Client;

namespace ExpenseTracker.Api.Services
{
    public class DevCurrentUserService : ICurrentUserService
    {
        public Guid UserId { get; } = Guid.Parse("00000000-0000-0000-0000-000000000001");

    }
}
