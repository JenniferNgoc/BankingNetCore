using BankingCore;
using BankingCore.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Banking.UnitTest
{
    public class RepositoryUnitTest
    {
        [Fact]
        public void TransferConcurencyUnitTest1()
        {
            DbContextOptions<BankingContext> options;
            var builder = new DbContextOptionsBuilder<BankingContext>();
            builder.UseInMemoryDatabase();
            options = builder.Options;

            var accountRepo = new AccountRepository(new BankingContext(options));
            var userTransRepo = new UserTransactionRepository(new BankingContext(options));

            accountRepo.CreateAccount("123", "123", "123", 100000);
            accountRepo.CreateAccount("1234", "1234", "1234", 200000);
            accountRepo.CreateAccount("12345", "12345", "12345", 500000);
            accountRepo.CreateAccount("123456", "123456", "123456", 0);
          

            Task task1 = Task.Run(() =>
            {
                Parallel.For(0, 25,
                                index =>
                                {
                                    userTransRepo.Transfer("123", "123456", index);
                                });
            });

            Task task2 = Task.Run(() =>
            {
                Parallel.For(0, 40,
                                index =>
                                {
                                    userTransRepo.Transfer("1234", "123456", index);
                                });
            });

            Task task3 = Task.Run(() =>
            {
                Parallel.For(0, 40,
                                index =>
                                {
                                    userTransRepo.Transfer("12345", "123456", index);
                                });
            });
            userTransRepo.Transfer("12345", "123456", 10000);

            Task.WaitAll(task1, task2, task3);

            var sumTotal1 = new BankingContext(options).Accounts.ToList();
            var sumTotal = new BankingContext(options).Accounts.ToList().Sum(x => x.Balance);
            var sumSourceBalaceAcc = new BankingContext(options).Accounts.Where(a => a.AccountNumber != "123456").Sum(x => x.Balance);
            var balanLastAcc = new BankingContext(options).Accounts.FirstOrDefault(_ => _.AccountNumber == "123456").Balance;

            Assert.Equal(sumTotal, 800000);
            Assert.Equal(800000 - sumSourceBalaceAcc, balanLastAcc);
        }
    }
}
