using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PTPI.Models;
using PTPI.Services.Interfaces;

namespace PTPI.Controllers
{
    [Authorize]
    public class TransactionsController : Controller
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        public IActionResult Create(int accountCode)
        {
            return View(new Transaction
            {
                AccountCode = accountCode,
                TransactionDate = DateTime.Today
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Transaction transaction)
        {
            ModelState.Remove("Account");
            ModelState.Remove("CaptureDate");

            if (!ModelState.IsValid)
                return View(transaction);

            try
            {
                await _transactionService.CreateTransactionAsync(transaction);
                TempData["Success"] = "Transaction saved successfully.";
                return RedirectToAction("Edit", "Accounts", new { id = transaction.AccountCode });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(transaction);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var transaction = await _transactionService.GetTransactionByIdAsync(id);
            if (transaction == null) return NotFound();
            return View(transaction);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Transaction transaction)
        {
            if (id != transaction.Code) return BadRequest();

            ModelState.Remove("Account");
            ModelState.Remove("CaptureDate");

            if (!ModelState.IsValid)
            {
                var existing = await _transactionService.GetTransactionByIdAsync(id);
                if (existing != null)
                {
                    transaction.CaptureDate = existing.CaptureDate;
                    transaction.Account = existing.Account;
                }
                return View(transaction);
            }

            try
            {
                await _transactionService.UpdateTransactionAsync(transaction);
                TempData["Success"] = "Transaction updated successfully.";
                return RedirectToAction("Edit", "Accounts", new { id = transaction.AccountCode });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var existing = await _transactionService.GetTransactionByIdAsync(id);
                if (existing != null)
                {
                    transaction.CaptureDate = existing.CaptureDate;
                    transaction.Account = existing.Account;
                }
                return View(transaction);
            }
        }
    }
}
