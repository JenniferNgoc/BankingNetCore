using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankingCore.Models
{
    public enum TransactionType
    {
        Deposit = 1,
        Withdraw = 2
    }

    public class UserTransaction
    {
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        public string TransactionNo { get; set; }
        public int AccountId { get; set; }
        public TransactionType Type { get; set; }
        public decimal StartBalance { get; set; }
        public decimal EndBalance { get; set; }
        public decimal Amount { get; set; }

        //[ForeignKey("AccountId")]
        public Account Account { get; set; }
    }
}
