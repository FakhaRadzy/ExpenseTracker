using System;
using System.Collections.Generic;
using System.Text;

namespace ExpenseTracker.Domain.Common
{
    public class DomainException(string message) : Exception(message);
}
