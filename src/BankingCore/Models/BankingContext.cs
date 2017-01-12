using Microsoft.EntityFrameworkCore;

namespace BankingCore.Models
{
    public class BankingContext : DbContext
    {
        public BankingContext(DbContextOptions<BankingContext> options) : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<UserTransaction> UserTransactions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<UserTransaction>()
               .HasOne(p => p.Account)
               .WithMany(b => b.UserTransactions)
               .HasForeignKey(p => p.AccountId);
        }
    }
}
