using ExpenseTracker.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ExpenseTracker.Infrastructure.Persistance.Interceptors
{
    public class AuditableEntityInterceptor(TimeProvider timeProvider) : SaveChangesInterceptor
    {

    }
}
