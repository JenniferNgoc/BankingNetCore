using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BankingCore.Models
{
    public class Account
    {
        [ScaffoldColumn(false)]
        public int Id { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string Password { get; set; }

        //[ConcurrencyCheck]
        public decimal Balance { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public virtual IList<UserTransaction> UserTransactions { get; set; }
    }
}
