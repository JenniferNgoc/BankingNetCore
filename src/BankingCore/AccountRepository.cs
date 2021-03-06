﻿using BankingCore.Models;
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

        public Account GetAccountInfo(string accountNumber)
        {
            var account =  context.Accounts.AsNoTracking().FirstOrDefault(_ => _.AccountNumber == accountNumber);

            if (account == null)
            {
                throw new Exception("Account number does not exist!");
            }

            return account;
        }

        public Account GetAllAccountInfo(string accountNumber)
        {
            var account = context.Accounts.Include(c => c.UserTransactions).FirstOrDefault(_ => _.AccountNumber == accountNumber);

            if (account == null)
            {
                throw new Exception("Account number does not exist!");
            }

            return account;
        }

        public bool Authenticate(string accountNumber, string password)
        {
            var hashedPassword = StringExtensions.HashPassword(password);
        
            var user = context.Accounts.FirstOrDefault(usr =>
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
                var account = context.Accounts.AsNoTracking().FirstOrDefault(_ => _.AccountNumber == accountNumber);
                if (account != null)
                {
                    throw new Exception("Account Number is already taken!");
                }

                account = new Account()
                {
                    AccountName = accountName,
                    AccountNumber = accountNumber,
                    Password = StringExtensions.HashPassword(password),
                    Balance = balance
                };

                context.Accounts.Add(account);
                context.SaveChanges();
                return account;
            }
        }
    }
}
