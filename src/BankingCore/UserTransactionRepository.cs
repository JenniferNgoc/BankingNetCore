using BankingCore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankingCore
{
    public class UserTransactionRepository : IUserTransactionRepository
    {
        private static readonly object _lockTransaction = new object();

        private BankingContext context;

        public UserTransactionRepository(BankingContext _context)
        {
            context = _context;
        }

        public void Withdraw(string accountNumber, decimal amount)
        {
            if (amount <= 0 || string.IsNullOrEmpty(accountNumber))
            {
                throw new Exception("Invalid Parameter Input");
            }

            var account = context.Accounts.AsNoTracking().Include(c => c.UserTransactions).FirstOrDefault(_ => _.AccountNumber == accountNumber);

            if (account == null)
            {
                throw new Exception("Account number does not exist!");
            }

            bool saveFailed;
            do
            {
                saveFailed = false;
                if (account.Balance < amount)
                {
                    throw new Exception("Balance too low.");
                }
                var startBalance = account.Balance;

                account.Balance -= amount;

                var userTrans = new UserTransaction()
                {
                    AccountId = account.Id,
                    StartBalance = startBalance,
                    EndBalance = account.Balance,
                    Amount = amount,
                    TransactionNo = Guid.NewGuid().ToString(),
                    Type = TransactionType.Withdraw
                };

                account.UserTransactions.Add(userTrans);
                context.Accounts.Update(account);

                try
                {
                    context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    context.Entry(account).Reload();
                    saveFailed = true;
                }
                catch (Exception ex)
                {
                    throw;
                }
            } while (saveFailed);
        }

        public void Deposit(string accountNumber, decimal amount)
        {
            if (amount <= 0 || string.IsNullOrEmpty(accountNumber))
            {
                throw new Exception("Invalid Parameter Input");
            }

            var account = context.Accounts.AsNoTracking().Include(c => c.UserTransactions).FirstOrDefault(_ => _.AccountNumber == accountNumber);
            if (account == null)
            {
                throw new Exception("Account number does not exist!");
            }

            bool saveFailed;
            do
            {
                saveFailed = false;
                var startBalance = account.Balance;
                account.Balance += amount;

                var userTrans = new UserTransaction()
                {
                    AccountId = account.Id,
                    StartBalance = startBalance,
                    EndBalance = account.Balance,
                    Amount = amount,
                    TransactionNo = Guid.NewGuid().ToString(),
                    Type = TransactionType.Deposit
                };

                account.UserTransactions.Add(userTrans);
                context.Accounts.Update(account);

                try
                {
                    context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    context.Entry(account).Reload();
                    saveFailed = true;
                }
                catch (Exception ex)
                {
                    throw;
                }
            } while (saveFailed);
        }

        public void Transfer(string fromAccountNumber, string toAccountNumber, decimal amount)
        {
            if (amount <= 0 || string.IsNullOrEmpty(fromAccountNumber)
                || string.IsNullOrEmpty(toAccountNumber) || fromAccountNumber == toAccountNumber)
            {
                throw new Exception("Invalid Parameter Input");
            }

            var accountSource = context.Accounts.Include(c => c.UserTransactions).FirstOrDefault(_ => _.AccountNumber == fromAccountNumber);
            if (accountSource == null)
            {
                throw new Exception("Source account number does not exist!");
            }

            var accountDestin = context.Accounts.Include(c => c.UserTransactions).FirstOrDefault(_ => _.AccountNumber == toAccountNumber);
            if (accountDestin == null)
            {
                throw new Exception("Destination Account number does not exist!");
            }

            if (accountSource.Balance < amount)
            {
                throw new Exception("Balance too low.");
            }

            bool saveFailed;
            do
            {
                saveFailed = false;                

                var startBalanceSource = accountSource.Balance;
                var startBalanceDestin = accountDestin.Balance;
                accountSource.Balance -= amount;
                accountDestin.Balance += amount;

                var tranAccountSource = new UserTransaction()
                {
                    AccountId = accountSource.Id,
                    StartBalance = startBalanceSource,
                    EndBalance = accountSource.Balance,
                    TransactionNo = Guid.NewGuid().ToString(),
                    Amount = amount,
                    Type = TransactionType.Withdraw
                };

                var tranAccountDestin = new UserTransaction()
                {
                    AccountId = accountDestin.Id,
                    StartBalance = startBalanceDestin,
                    EndBalance = accountDestin.Balance,
                    TransactionNo = Guid.NewGuid().ToString(),
                    Amount = amount,
                    Type = TransactionType.Deposit
                };
                accountSource.UserTransactions.Add(tranAccountSource);
                accountDestin.UserTransactions.Add(tranAccountDestin);

                context.Accounts.Update(accountSource);
                context.Accounts.Update(accountDestin);

                try
                {
                    context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    context.Entry(accountSource).Reload();
                    context.Entry(accountDestin).Reload();
                    saveFailed = true;
                }
                catch (Exception ex)
                {
                    throw;
                }
            } while (saveFailed);
        }
    }
}
