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
            // #region mock
            // //var _accountDataTest = new List<Account>()
            // //{
            // //    new Account {AccountNumber = "123", Balance = 100000},
            // //    new Account {AccountNumber = "1234", Balance = 200000},
            // //    new Account {AccountNumber = "12345", Balance = 500000},
            // //    new Account {AccountNumber = "123456", Balance = 0}
            // //}.AsQueryable();

            // var mockAccountSet = new Mock<DbSet<Account>>();
            //// mockAccountSet.As<IQueryable<Account>>().Setup(m => m.Provider).Returns(_accountDataTest.Provider);
            // //mockAccountSet.As<IQueryable<Account>>().Setup(m => m.Expression).Returns(_accountDataTest.Expression);
            // //mockAccountSet.As<IQueryable<Account>>().Setup(m => m.ElementType).Returns(_accountDataTest.ElementType);
            // //mockAccountSet.As<IQueryable<Account>>().Setup(m => m.GetEnumerator()).Returns(_accountDataTest.GetEnumerator());
            // var mockUserTransactionSet = new Mock<DbSet<UserTransaction>>();

            // var mockContext = new Mock<BankingContext>();

            // mockContext.Setup(m => m.UserTransactions).Returns(mockUserTransactionSet.Object);
            // mockContext.Setup(m => m.Accounts).Returns(mockAccountSet.Object);
            // mockAccountSet.Setup(m => m.Add(It.IsAny<Account>())).Verifiable();
            // mockContext.Setup(m => m.SaveChanges()).Verifiable();
            // #endregion
            
            DbContextOptions<BankingContext> options;
            var builder = new DbContextOptionsBuilder<BankingContext>();
            var connectionStringBuilder = new SqliteConnectionStringBuilder()
            {
                DataSource = ":memory:",
            };
            var connectionString = connectionStringBuilder.ToString();
            var connStr = new SqliteConnection(connectionString);

            builder.UseSqlite(connStr);

            //builder.UseInMemoryDatabase();
            options = builder.Options;
            using (var con = new BankingContext(options))
            {
                con.Database.OpenConnection();
                con.Database.EnsureCreated();
            }

            var context = new BankingContext(options);


            var accountRepo = new AccountRepository(context);
            var userTransRepo = new UserTransactionRepository(context);

            accountRepo.CreateAccount("123", "123", "123", 100000);
            accountRepo.CreateAccount("1234", "1234", "1234", 200000);
            accountRepo.CreateAccount("12345", "12345", "12345", 500000);
            accountRepo.CreateAccount("123456", "123456", "123456", 0);
            userTransRepo.Transfer("12345", "123456", 10000);

            //for (int i = 0; i < 25; i++)
            //{
            //    Task.Run(() =>
            //    {
            //        userTransRepo.Transfer("123", "123456", i + 1);
            //    });
            //}
            Task task1 = Task.Run(() =>
            {
                for (int i = 0; i < 25; i++)
                {
                    Task.Run(() =>
                    {
                        userTransRepo.Transfer("123", "123456", i + 1);
                    });
                }

                //Parallel.For(0, 25,
                //                index =>
                //                {
                //                    userTransRepo.Transfer("123", "123456", index + 1);
                //                });
            });

            Task task2 = Task.Run(() =>
            {
                for (int i = 0; i < 25; i++)
                {
                    Task.Run(() =>
                    {
                        userTransRepo.Transfer("1234", "123456", i + 1);
                    });
                }
                //Parallel.For(0, 40,
                //                index =>
                //                {
                //                    userTransRepo.Transfer("1234", "123456", index + 1);
                //                });
            });

            Task task3 = Task.Run(() =>
            {
                for (int i = 0; i < 25; i++)
                {
                    Task.Run(() =>
                    {
                        userTransRepo.Transfer("12345", "123456", i + 1);
                    });
                }
                //Parallel.For(0, 40,
                //                index =>
                //                {
                //                    userTransRepo.Transfer("12345", "123456", index + 1);
                //                });
            });

            // Task.WhenAll(task1, task2, task3).Wait();
            Task.WaitAll(task1, task2, task3);
            var sumTotal1 = context.Accounts.ToList();
            var sumTotal = context.Accounts.ToList().Sum(x => x.Balance);
            var sumSourceBalaceAcc = context.Accounts.Where(a => a.AccountNumber != "123456").Sum(x => x.Balance);
            var balanLastAcc = context.Accounts.FirstOrDefault(_ => _.AccountNumber == "123456").Balance;

            Assert.Equal(sumTotal, 800000);
            Assert.Equal(800000 - sumSourceBalaceAcc, balanLastAcc);
            int i123 = 1;
        }      
    }
}
