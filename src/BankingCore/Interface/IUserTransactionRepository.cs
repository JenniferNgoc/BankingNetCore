using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankingCore
{
    public interface IUserTransactionRepository
    {
        void Withdraw(string accountNumber, decimal amount);
        void Deposit(string accountNumber, decimal amount);
        void Transfer(string fromAccountNumber, string toAccountNumber, decimal amount);
    }
}
