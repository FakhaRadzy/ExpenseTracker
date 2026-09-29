using ExpenseTracker.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpenseTracker.Domain.Entities
{
    public class Budget : BaseEntity, IUserOwned
    {
        public Guid UserId { get; private set; }
        public Guid CategoryId { get; private set; }
        public Category Category { get; private set; }


        public int Year { get; private set; }

        public int Month { get; private set; }
        public decimal Limit { get; private set; }

        private Budget() { }

        public Budget(Guid userId, Guid categoryId, int year, int month, decimal limit)
        {
            if (userId == Guid.Empty)
            {
                throw new DomainException("Budget must belong to a user.");
            }

            if (categoryId == Guid.Empty)
            {
                throw new DomainException("Budget must have a category");
            }

            if (year is < 2000 or > 2100)
            {
                throw new DomainException("Year must be between 2000 and 2100");
            }

            if (month is < 1 or > 12)
            {
                throw new DomainException("Month must be between 1 and 12");
            }

            UserId = userId;
            CategoryId = categoryId;
            Year = year;
            Month = month;
            ChangeLimit(limit);
        }

        public void ChangeLimit(decimal limit)
        {
            if (limit < 0)
            {
                throw new DomainException("Budget limit must be greater than zero"); 
            }

            Limit = limit;
        }

        public bool IsExceededBy(decimal amountSpent) => amountSpent > Limit;

    }
}
