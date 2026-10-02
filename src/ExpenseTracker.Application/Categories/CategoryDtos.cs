using System;
using System.Collections.Generic;
using System.Text;

namespace ExpenseTracker.Application.Categories
{
    public record CategoryRequest(string Name, string? Color);

    public record CategoryResponse(Guid Id, string Name, string? Color);
}
