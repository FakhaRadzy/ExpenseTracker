using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using ExpenseTracker.Domain.Common;
using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Domain.Entities
{
    public class Expense : BaseEntity, IUserOwned
    {
        public const int DescriptionMaxLength = 200;
        public const int NotesMaxLength = 1000;

        public Guid UserId { get; private set; }
        public decimal Amount { get; private set; }
        public string Description { get; private set; } = null;
        public DateOnly Date { get; private set; }
        public PaymentMethod PaymentMethod { get; private set; }

        public string? Notes { get; private set; }


        public Guid CategoryId { get; private set; }
        public Category Category { get; private set; } = null;

        private Expense() { }

        public Expense(Guid userId, Guid categoryId, decimal amount, string description, DateOnly date, PaymentMethod paymentMethod, string? notes = null)
        {
            if (userId == Guid.Empty)
            {
                throw new DomainException("Expense must belong to a user.");
            }

            UserId = userId;
            Update(categoryId, amount, description, date, paymentMethod, notes);
        }

        public void Update(Guid categoryId, decimal amount, string description, DateOnly date, PaymentMethod paymentMethod, string? notes = null)
        {
            if (categoryId == Guid.Empty)
            {
                throw new DomainException("Expense must have a category.");
            }

            if (amount <= 0)
            {
                throw new DomainException("Amount must be greater than zero.");
            }

            if (decimal.Round(amount, 2) != amount)
            {
                throw new DomainException("Amount cannot have more than 2 decimal places.");
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                throw new DomainException("Description is required.");
            }

            description = description.Trim();
            if (description.Length > DescriptionMaxLength)
            {
                throw new DomainException($"Description cannot exceed {DescriptionMaxLength} characters.");
            }

            if (!Enum.IsDefined(paymentMethod))
            {
                throw new DomainException("Invalid payment method.");
            }

            notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
            if(notes?.Length > NotesMaxLength)
            {
                throw new DomainException($"Notes cannot exceed {NotesMaxLength} characters.");
            }

            CategoryId = categoryId;
            Amount = amount;
            Description = description;
            Date = date;
            PaymentMethod = paymentMethod;
            Notes = notes;
        }
    }
}
