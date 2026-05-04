using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PTPI.Models;
using PTPI.Services.Interfaces;

namespace PTPI.Controllers
{
    [Authorize]
    public class AccountsController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public IActionResult Create(int personCode)
        {
            return View(new Account { PersonCode = personCode });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Account account)
        {
            ModelState.Remove("Person");
            ModelState.Remove("Transactions");

            if (!ModelState.IsValid)
                return View(account);

            try
            {
                await _accountService.CreateAccountAsync(account);
                TempData["Success"] = "Account created. You can now add transactions below.";
                return RedirectToAction(nameof(Edit), new { id = account.Code });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(account);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var account = await _accountService.GetAccountByIdAsync(id);
            if (account == null) return NotFound();
            return View(account);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Account account)
        {
            if (id != account.Code) return BadRequest();

            ModelState.Remove("Person");
            ModelState.Remove("Transactions");

            if (!ModelState.IsValid)
            {
                var full = await _accountService.GetAccountByIdAsync(id);
                if (full != null)
                {
                    account.Transactions = full.Transactions;
                    account.OutstandingBalance = full.OutstandingBalance;
                    account.IsClosed = full.IsClosed;
                    account.PersonCode = full.PersonCode;
                    account.Person = full.Person;
                }
                return View(account);
            }

            try
            {
                await _accountService.UpdateAccountAsync(account);
                TempData["Success"] = "Account updated successfully.";
                return RedirectToAction(nameof(Edit), new { id });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var full = await _accountService.GetAccountByIdAsync(id);
                if (full != null)
                {
                    account.Transactions = full.Transactions;
                    account.OutstandingBalance = full.OutstandingBalance;
                    account.IsClosed = full.IsClosed;
                    account.PersonCode = full.PersonCode;
                    account.Person = full.Person;
                }
                return View(account);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Close(int id)
        {
            try
            {
                await _accountService.CloseAccountAsync(id);
                TempData["Success"] = "Account closed successfully.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Edit), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reopen(int id)
        {
            try
            {
                await _accountService.ReopenAccountAsync(id);
                TempData["Success"] = "Account reopened successfully.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Edit), new { id });
        }
    }
}
