using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using ExpenseTracker.Domain.Common;

namespace ExpenseTracker.Domain.Entities
{
    public class Category : BaseEntity, IUserOwned
    {
        public const int NameMaxLength = 50;

        public Guid UserId { get; private set; }
        public string Name { get; private set; } = null!;
        public string? Color { get; private set;  }         // Hex, e.g. "#FF5733" — for charts in a frontend

        private readonly List<Expense> _expenses = [];
        public IReadOnlyCollection<Expense> Expenses => _expenses.AsReadOnly();

        private Category() { }

        public Category(Guid userId, string name, string? color = null)
        {
            if (userId == Guid.Empty)
            {
                throw new DomainException("Category must belong to a user.");
            }

            UserId = userId;
            Rename(name);
            SetColor(color);



        }

        public void Rename(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new DomainException("Category name is required.");

            }

            name = name.Trim();
            if (name.Length > NameMaxLength)
            {
                throw new DomainException($"Category name cannot exceed {NameMaxLength} characters.");
            }

            Name = name;
        }

        public void SetColor(string? color)
        {
            if (color is not null && !Regex.IsMatch(color, "^#[0-9A-Fa-f]{6}$"))
            {
                throw new DomainException("Color must be a hex value like #FF5733.");
            }

            Color = color;
        }
    }
}
