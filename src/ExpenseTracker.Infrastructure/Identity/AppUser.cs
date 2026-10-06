using Microsoft.AspNetCore.Identity;

namespace ExpenseTracker.Infrastructure.Identity
{
    // Identity's user, with a Guid key to match the UserId in our domain entities 
    public class AppUser : IdentityUser<Guid>;
}
