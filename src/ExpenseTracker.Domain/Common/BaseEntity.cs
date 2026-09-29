using System;
using System.Collections.Generic;
using System.Text;

namespace ExpenseTracker.Domain.Common
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();

        //Set automatically by an EF Core interceptor (Phase 2) — don't set this manually
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
    }
}
