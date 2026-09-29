using System;
using System.Collections.Generic;
using System.Text;

namespace ExpenseTracker.Domain.Common
{
    public interface IUserOwned
    {
        Guid UserId { get; }
    }
}
