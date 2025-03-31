using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibrarySystem.Data;
using LibrarySystem.Models;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using LibrarySystem.Utility;
using LibrarySystem.Services.IService;
using Microsoft.AspNetCore.Authorization;

namespace LibrarySystem.Controllers
{
    public class MovementController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IService<User> _userService;
        private readonly IService<LibraryUnit> _libraryUnitService;
        private readonly IService<MovementOfLibraryUnit> _movementService;

        public MovementController(UserManager<IdentityUser> userManager,
            IService<User> userService, 
            IService<LibraryUnit> libraryUnitService,
            IService<MovementOfLibraryUnit> movementService)
        {
            _userManager = userManager;
            _userService = userService;
            _libraryUnitService = libraryUnitService;
            _movementService = movementService;
        }

        [Authorize(Roles = $"{SD.AdminRole},{SD.LibrarianRole}")]
        public IActionResult Register()
        {
            return View();
        }

        [Authorize(Roles = $"{SD.AdminRole},{SD.LibrarianRole}")]
        public IActionResult RegisterGiving()
        {
            return View(new MovementViewModel());
        }

        [HttpPost]
        [Authorize(Roles = $"{SD.AdminRole},{SD.LibrarianRole}")]
        public async Task<IActionResult> RegisterGiving(MovementViewModel model)
        {
            if (ModelState.IsValid)
            {
                var identityUser = await _userManager.GetUserAsync(User);
                var user = _userService.GetWhere(x => x.IdentityUserId == identityUser.Id).First();
                LibraryUnit unit = await _libraryUnitService.GetByIdAsync(model.LibraryUnitId);

                var movement = new MovementOfLibraryUnit
                {
                    DateTime = DateTime.Now,
                    Deadline = model.DeadLine,
                    Type = "Giving",
                    LibraryUnitId = model.LibraryUnitId,
                    ReaderId = model.ReaderId,
                    LibrarianId = user.Id,
                    Condition = unit.Condition
                };

                await _movementService.AddAsync(movement);
                unit.IsAvailable = false;
                await _libraryUnitService.UpdateAsync(unit);

                TempData["success"] = "Движението е регистрирано успешно.";

                return RedirectToAction("Register");
            }
            return View(model);
        }

        [Authorize(Roles = $"{SD.AdminRole},{SD.LibrarianRole}")]
        public IActionResult RegisterReturning()
        {
            return View(new MovementReturningViewModel());
        }

        [HttpPost]
        [Authorize(Roles = $"{SD.AdminRole},{SD.LibrarianRole}")]
        public async Task<IActionResult> RegisterReturning(MovementReturningViewModel model)
        {
            if (ModelState.IsValid)
            {
                var identityUser = await _userManager.GetUserAsync(User);
                var user = _userService.GetWhere(x => x.IdentityUserId == identityUser.Id).First();
                LibraryUnit unit = await _libraryUnitService.GetByIdAsync(model.LibraryUnitId);
                MovementOfLibraryUnit oldMovement = _movementService.GetWhere(x => x.LibraryUnitId == model.LibraryUnitId && x.Type == "Giving").OrderBy(x => x.DateTime).First();

                var movement = new MovementOfLibraryUnit
                {
                    DateTime = DateTime.Now,
                    Deadline = model.DeadLine,
                    Type = "Returning",
                    LibraryUnitId = model.LibraryUnitId,
                    ReaderId = oldMovement.ReaderId,
                    LibrarianId = user.Id,
                    Condition = model.Condition
                };

                await _movementService.AddAsync(movement);
                unit.IsAvailable = true;
                unit.Condition = model.Condition;
                await _libraryUnitService.UpdateAsync(unit);

                TempData["success"] = "Движението е регистрирано успешно.";

                return RedirectToAction("Register");
            }
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = $"{SD.AdminRole},{SD.LibrarianRole}")]
        public IActionResult SearchAvailableLibraryUnits(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Json(new { });

            var results = _libraryUnitService.GetWhere(u => u.IsAvailable &&
                (u.InventoryNumber.Contains(query) ||
                 (u.Isbn != null && u.Isbn.Contains(query)) ||
                 u.Title.Name.Contains(query)))
            .Select(u => new
            {
              id = u.Id,
              text = $"{u.InventoryNumber} - {u.Title.Name}"
             })
             .ToList();


            return Json(results);
        }

        [HttpGet]
        [Authorize(Roles = $"{SD.AdminRole},{SD.LibrarianRole}")]
        public IActionResult SearchUnavailableLibraryUnits(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Json(new { });

            var results = _libraryUnitService.GetWhere(u => !u.IsAvailable && !u.IsScrapped &&
                (u.InventoryNumber.Contains(query) ||
                 (u.Isbn != null && u.Isbn.Contains(query)) ||
                 u.Title.Name.Contains(query)))
            .Select(u => new
            {
                id = u.Id,
                text = $"{u.InventoryNumber} - {u.Title.Name}"
             })
             .ToList();
          
            return Json(results);
        }




        [HttpGet]
        [Authorize(Roles = $"{SD.AdminRole},{SD.LibrarianRole}")]
        public async Task<IActionResult> SearchUser(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Json(new { });

            var userIdsInRole = await _userManager.GetUsersInRoleAsync(SD.UserRole);
            var filteredUserIds = userIdsInRole.Select(u => u.Id).ToList();

            var users = _userService.GetWhere(u => filteredUserIds.Contains(u.IdentityUserId) &&
                 (EF.Functions.Like(u.FirstName, $"%{query}%") ||
                  EF.Functions.Like(u.LastName, $"%{query}%")))
           .Select(u => new
           {
             id = u.Id,
             text = $"{u.FirstName} {u.LastName}"
            })
           .ToList();

            return Json(users);
        }
    }
}