using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BankingCore;
using Microsoft.AspNetCore.Authorization;
using BankingAspNetCore.ViewModels;
using System.Security.Claims;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace BankingAspNetCore.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountRepository accountRepo;

        public AccountController(IAccountRepository _accountRepo)
        {
            accountRepo = _accountRepo;
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            HttpContext.Authentication.SignOutAsync("Cookies");
            return View();
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            HttpContext.Authentication.SignOutAsync("Cookies");
            return View();
        }

        [AllowAnonymous]
        public IActionResult Logout()
        {
            HttpContext.Authentication.SignOutAsync("Cookies");
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await accountRepo.Authenticate(model.AccountNumber, model.Password);
            if (result)
            {
                var acc = await accountRepo.GetAccountInfo(model.AccountNumber);
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, acc.AccountNumber),
                    new Claim(ClaimTypes.NameIdentifier, acc.Id.ToString())
                };
                var id = new ClaimsIdentity(claims, "USER_INFO");
                var p = new ClaimsPrincipal(id);
                await HttpContext.Authentication.SignInAsync("Cookies", p);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                accountRepo.CreateAccount(model.AccountNumber, model.AccountName, model.Password, model.Balance);
                ModelState.AddModelError(string.Empty, "Your account has been created successfully and is ready to use");

            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return View(model);
        }
    }
}
