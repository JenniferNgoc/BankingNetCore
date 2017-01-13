using System;
using System.Security.Cryptography;
using System.Text;

namespace BankingCore
{
    public class StringExtensions
    {
        public static string HashPassword(string password)
        {
            using (var md5 = MD5.Create())
            {
                var hashedBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes);
            }
        }
    }
}
