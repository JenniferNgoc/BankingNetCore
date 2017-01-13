using BankingCore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankingCore
{
    public class AccountRepository: IAccountRepository
    {
        private static readonly object _lockAccount = new object();

        private BankingContext context;

        public AccountRepository(BankingContext _context)
        {
            context = _context;
        }

        public async Task<Account> GetAccountInfo(string accountNumber)
        {
            var account = await context.Accounts.AsNoTracking().FirstOrDefaultAsync(_ => _.AccountNumber == accountNumber);

            if (account == null)
            {
                throw new Exception("Account number does not exist!");
            }

            return account;
        }

        public async Task<Account> GetAllAccountInfo(string accountNumber)
        {
            var account = await context.Accounts.Include(c => c.UserTransactions).FirstOrDefaultAsync(_ => _.AccountNumber == accountNumber);

            if (account == null)
            {
                throw new Exception("Account number does not exist!");
            }

            return account;
        }

        public async Task<bool> Authenticate(string accountNumber, string password)
        {
            // var hashedPassword = AppUtils.HashPassword(password);
            var hashedPassword = password;

            var user = await context.Accounts.FirstOrDefaultAsync(usr =>
                            usr.AccountNumber == accountNumber &&
                            usr.Password == hashedPassword);
            if (user == null)
                return false;

            return true;
        }

        public Account CreateAccount(string accountNumber, string accountName, string password, decimal balance)
        {
            lock (_lockAccount)
            {
                var account = context.Accounts.FirstOrDefault(_ => _.AccountNumber == accountNumber);
                if (account != null)
                {
                    throw new Exception("Account Number is already taken!");
                }

                account = new Account()
                {
                    AccountName = accountName,
                    AccountNumber = accountNumber,
                    Password = password,
                    Balance = balance
                };

                context.Accounts.Add(account);
                context.SaveChangesAsync();
                return account;
            }
        }
    }
}
