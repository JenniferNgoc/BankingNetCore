using BankingCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankingCore
{
    public interface IAccountRepository
    {
        Account GetAccountInfo(string accountNumber);
        Account GetAllAccountInfo(string accountNumber);
        bool Authenticate(string accountNumber, string password);
        Account CreateAccount(string accountNumber, string accountName, string password, decimal balance);
    }
}
