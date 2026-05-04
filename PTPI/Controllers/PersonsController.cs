using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PTPI.Models;
using PTPI.Services.Interfaces;

namespace PTPI.Controllers
{
    [Authorize]
    public class PersonsController : Controller
    {
        private readonly IPersonService _personService;

        public PersonsController(IPersonService personService)
        {
            _personService = personService;
        }

        public async Task<IActionResult> Index(string? searchTerm, int page = 1)
        {
            var model = await _personService.GetPersonsAsync(searchTerm, page);
            return View(model);
        }

        public IActionResult Create()
        {
            return View(new Person());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Person person)
        {
            if (!ModelState.IsValid)
                return View(person);

            try
            {
                await _personService.CreatePersonAsync(person);
                TempData["Success"] = "Person created. You can now add accounts below.";
                return RedirectToAction(nameof(Edit), new { id = person.Code });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(person);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var person = await _personService.GetPersonByIdAsync(id);
            if (person == null) return NotFound();
            return View(person);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Person person)
        {
            if (id != person.Code) return BadRequest();

            if (!ModelState.IsValid)
            {
                var existing = await _personService.GetPersonByIdAsync(id);
                if (existing != null)
                    person.Accounts = existing.Accounts;
                return View(person);
            }

            try
            {
                await _personService.UpdatePersonAsync(person);
                TempData["Success"] = "Person updated successfully.";
                return RedirectToAction(nameof(Edit), new { id });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var existing = await _personService.GetPersonByIdAsync(id);
                if (existing != null)
                    person.Accounts = existing.Accounts;
                return View(person);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            var person = await _personService.GetPersonByIdAsync(id);
            if (person == null) return NotFound();
            return View(person);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _personService.DeletePersonAsync(id);
                TempData["Success"] = "Person deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Delete), new { id });
            }
        }
    }
}
