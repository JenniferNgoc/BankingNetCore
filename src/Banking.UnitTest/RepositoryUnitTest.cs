using BankingCore;
using BankingCore.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.DependencyInjection;

namespace Banking.UnitTest
{
    public class RepositoryUnitTest
    {
        [Fact]
        public void TransferConcurencyUnitTest1()
        {
            var db = new DbContextOptionsBuilder<BankingContext>();
            db.UseInMemoryDatabase();

            var context = new BankingContext(db.Options);


            var accountRepo = new AccountRepository(context);
            var userTransRepo = new UserTransactionRepository(context);

            accountRepo.CreateAccount("123", "123", "123", 100000);
            accountRepo.CreateAccount("1234", "1234", "1234", 200000);
            accountRepo.CreateAccount("12345", "12345", "12345", 500000);
            accountRepo.CreateAccount("123456", "123456", "123456", 0);
            userTransRepo.Transfer("12345", "123456", 10000);
            userTransRepo.Transfer("123", "123456", 5555);
            userTransRepo.Transfer("1234", "123456", 1232);
            userTransRepo.Transfer("12345", "123456", 32238);

            var sumTotal = context.Accounts.ToList().Sum(x => x.Balance);
            var sumSourceBalaceAcc = context.Accounts.Where(a => a.AccountNumber != "123456").Sum(x => x.Balance);
            var balanLastAcc = context.Accounts.FirstOrDefault(_ => _.AccountNumber == "123456").Balance;

            Assert.Equal(sumTotal, 800000);
            Assert.Equal(800000 - sumSourceBalaceAcc, balanLastAcc);
        }
    }
}
