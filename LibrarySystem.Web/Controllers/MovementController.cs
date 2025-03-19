using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibrarySystem.Data;
using LibrarySystem.Models;
using System.Linq;

namespace LibrarySystem.Controllers
{
    public class MovementController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MovementController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Register()
        {
            return View(new MovementViewModel());
        }

        [HttpPost]
        public IActionResult Register(MovementViewModel model)
        {
            if (ModelState.IsValid)
            {
                var movement = new MovementOfLibraryUnit
                {
                    DateTime = model.ActionDateTime,
                    Deadline = model.DeadLine,
                    Type = model.OperationType,
                    LibraryUnitId = model.LibraryUnitId,
                    ReaderId = model.ReaderId,
                    LibrarianId = model.LibrarianId,
                    Condition = model.Condition
                };

                _context.MovementsOfLibraryUnits.Add(movement);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult SearchLibraryUnit(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Json(new { });

            var results = _context.LibraryUnits
                .Include(u => u.Title)
                .Where(u => u.InventoryNumber.Contains(query) ||
                            (u.Isbn != null && u.Isbn.Contains(query)) ||
                            u.Title.Name.Contains(query))
                .Select(u => new
                {
                    id = u.Id,
                    text = $"{u.InventoryNumber} - {u.Title.Name}"
                })
                .ToList();

            return Json(results);
        }

        [HttpGet]
        public IActionResult SearchUser(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Json(new { });

            var results = _context.Users
                .Where(u => u.FirstName.Contains(query) || u.LastName.Contains(query))
                .Select(u => new
                {
                    id = u.Id,
                    text = $"{u.FirstName} {u.LastName}"
                })
                .ToList();

            return Json(results);
        }
    }
}
