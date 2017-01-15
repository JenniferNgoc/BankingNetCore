using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BankingCore;
using Microsoft.AspNetCore.Authorization;

namespace BankingAspNetCore.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IAccountRepository accountRepo;
        private readonly IUserTransactionRepository userTransRepo;

        public DashboardController(IAccountRepository _accountRepo, IUserTransactionRepository _userTransRepo)
        {
            accountRepo = _accountRepo;
            userTransRepo = _userTransRepo;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var accInfo = accountRepo.GetAllAccountInfo(User.GetAccNo());
            return View(accInfo);
        }

        [Authorize]
        public async Task<IActionResult> Deposit()
        {
            var accInfo = accountRepo.GetAccountInfo(User.GetAccNo());
            return View(accInfo);
        }
        
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Deposit(decimal amount)
        {
            try
            {
                userTransRepo.Deposit(User.GetAccNo(), amount);
                ViewData["Message"] = "Deposit request successfully submitted";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var accInfo = accountRepo.GetAccountInfo(User.GetAccNo());
                return View(accInfo);
            }

            return View("Complete");
        }

        [Authorize]
        public async Task<IActionResult> Withdraw()
        {
            var accInfo = accountRepo.GetAccountInfo(User.GetAccNo());
            return View(accInfo);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Withdraw(decimal amount)
        {
            try
            {
                userTransRepo.Withdraw(User.GetAccNo(), amount);
                ViewData["Message"] = "Withdraw request successfully submitted";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var accInfo = accountRepo.GetAccountInfo(User.GetAccNo());
                return View(accInfo);
            }

            return View("Complete");
        }

        [Authorize]
        public async Task<IActionResult> Transfer()
        {
            var accInfo = accountRepo.GetAccountInfo(User.GetAccNo());
            return View(accInfo);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Transfer(string toAccountNumber, decimal amount)
        {
            try
            {
                userTransRepo.Transfer(User.GetAccNo(), toAccountNumber, amount);
                ViewData["Message"] = "Your request is successfully submitted";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var accInfo = accountRepo.GetAccountInfo(User.GetAccNo());
                return View(accInfo);
            }

            return View("Complete");
        }
    }
}
