using System;
using System.Collections.Generic;
using System.Text;

namespace ExpenseTracker.Application.Common.Exceptions
{
    // Throw when a request clashes with the current state (duplicate name, item still in use...)
    public class ConflictException(string message) : Exception(message);
   
}
