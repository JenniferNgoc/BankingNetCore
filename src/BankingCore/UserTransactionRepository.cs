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
            var account = context.Accounts.AsNoTracking().Include(c => c.UserTransactions).FirstOrDefault(_ => _.AccountNumber == accountNumber);

            if (account == null)
            {
                throw new Exception("Account number does not exist!");
            }

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
            //context.Accounts.Update(account);
            context.Entry(account).State = EntityState.Modified;

            try
            {
                context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new Exception("Your balance was modified by another user after you got the original values. Please try again!");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void Deposit(string accountNumber, decimal amount)
        {
            var account = context.Accounts.AsNoTracking().Include(c => c.UserTransactions).FirstOrDefault(_ => _.AccountNumber == accountNumber);
            if (account == null)
            {
                throw new Exception("Account number does not exist!");
            }
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
            //context.Accounts.Update(account);
            context.Entry(account).State = EntityState.Modified;

            try
            {
                context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new Exception("Your balance was modified by another user after you got the original values. Please try again!");
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async void Transfer(string fromAccountNumber, string toAccountNumber, decimal amount)
        {
            var accountSource = context.Accounts.AsNoTracking().Include(c => c.UserTransactions).FirstOrDefault(_ => _.AccountNumber == fromAccountNumber);
            if (accountSource == null)
            {
                throw new Exception("Source account number does not exist!");
            }

            var accountDestin = context.Accounts.AsNoTracking().Include(c => c.UserTransactions).FirstOrDefault(_ => _.AccountNumber == toAccountNumber);
            if (accountDestin == null)
            {
                throw new Exception("Destination Account number does not exist!");
            }

            if (accountSource.Balance < amount)
            {
                throw new Exception("Balance too low.");
            }

            bool saveFailed = false;
            do
            {
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

                context.Entry(accountSource).State = EntityState.Modified;
                context.Entry(accountDestin).State = EntityState.Modified;

                try
                {
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {

                    context.Entry(accountSource);
                  var a=  context.Entry(accountDestin);
                    saveFailed = true;
                    //context.Entry(accountSource).State = EntityState.Unchanged;
                    //Transfer(fromAccountNumber, toAccountNumber, amount);
                }
                catch (Exception ex)
                {
                    throw;
                }
            } while (saveFailed);
           
        }

        private async void TransferProcess(string fromAccountNumber, string toAccountNumber, decimal amount, bool retry = false)
        {
            var accountSource = context.Accounts.AsNoTracking().Include(c => c.UserTransactions).FirstOrDefault(_ => _.AccountNumber == fromAccountNumber);
            if (accountSource == null)
            {
                throw new Exception("Source account number does not exist!");
            }

            var accountDestin = context.Accounts.AsNoTracking().Include(c => c.UserTransactions).FirstOrDefault(_ => _.AccountNumber == toAccountNumber);
            if (accountDestin == null)
            {
                throw new Exception("Destination Account number does not exist!");
            }

            if (accountSource.Balance < amount)
            {
                throw new Exception("Balance too low.");
            }

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

            //context.Accounts.Update(accountSource);
            //context.Accounts.Update(accountDestin);
            // var bca = context.Entry(accountSource).State;
            context.Entry(accountSource).State = EntityState.Modified;
            context.Entry(accountSource).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {

                //context.Entry(accountSource).
                //context.Entry(accountSource).State = EntityState.Unchanged;
                Transfer(fromAccountNumber, toAccountNumber, amount);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
