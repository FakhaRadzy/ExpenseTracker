
namespace ExpenseTracker.Application.Common.Exceptions
{
    // Throw when login fails or no valid user is present -> 401
    public class UnauthorizedException(string message) : Exception(message);
}
