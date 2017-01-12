using System.ComponentModel.DataAnnotations;

namespace BankingAspNetCore.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        public string AccountNumber { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        public string AccountNumber { get; set; }

        [Required]
        public string AccountName { get; set; }

        [Required]
        public decimal Balance { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
