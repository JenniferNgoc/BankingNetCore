using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BankingAspNetCore
{
    public static class GenericPrincipalExtensions
    {
        public static int GetUserId(this ClaimsPrincipal principal)
        {
            if (principal == null)
                return -1;

            return Convert.ToInt32(principal.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }

        public static string GetAccNo(this ClaimsPrincipal principal)
        {
            if (principal == null)
                return "";

            return principal.FindFirst(ClaimTypes.Name)?.Value;
        }
    }
}
