using System.ComponentModel.DataAnnotations;

namespace BankingAspNetCore.ViewModels
{
    public class DepositOrWithdrawViewModel
    {
        public string AccountNumber { get; set; }

        public string CurrentBalance { get; set; }

        [Required]
        public decimal Amount { get; set; }
    }

    public class TransferViewModel
    {
        public string FromAccountNumber { get; set; }

        public string CurrentBalance { get; set; }

        [Required]
        public string ToAccountNumber { get; set; }

        [Required]
        public decimal Amount { get; set; }
    }
}
