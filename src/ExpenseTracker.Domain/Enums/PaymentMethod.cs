using System;
using System.Collections.Generic;
using System.Text;

namespace ExpenseTracker.Domain.Enums
{
    public enum PaymentMethod
    {
        Cash = 1,
        DebitCard = 2,
        CreditCard = 3,
        BankTransfer = 4,
        EWallet = 5,        //Touch 'n Go, ShopeePay, GrabPay, etc..
        Other = 99
    }
}
