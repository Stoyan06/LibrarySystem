using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibrarySystem.Data;
using LibrarySystem.Models;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using LibrarySystem.Utility;
using LibrarySystem.Services.IService;
using Microsoft.AspNetCore.Authorization;
using LibrarySystem.Web.Models;

namespace LibrarySystem.Controllers
{
    public class MovementController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IService<User> _userService;
        private readonly IService<LibraryUnit> _libraryUnitService;
        private readonly IService<MovementOfLibraryUnit> _movementService;
        private readonly ISearchService _searchService;
        private readonly IService<Title> _titleService;

        public MovementController(UserManager<IdentityUser> userManager,
            IService<User> userService, 
            IService<LibraryUnit> libraryUnitService,
            IService<MovementOfLibraryUnit> movementService, ISearchService searchService, IService<Title> titleService)
        {
            _userManager = userManager;
            _userService = userService;
            _libraryUnitService = libraryUnitService;
            _movementService = movementService;
            _searchService = searchService;
            _titleService = titleService;
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
            LibraryUnit unit = await _libraryUnitService.GetByIdAsync(model.LibraryUnitId);
            if (unit.IsSavedByUser)
            {
                if(unit.SavedByReaderId != model.ReaderId)
                {
                    User reader = await _userService.GetByIdAsync(unit.SavedByReaderId.Value);
                    MovementWarningViewModel warningModel = new MovementWarningViewModel()
                    {
                        DeadLine = model.DeadLine,
                        LibraryUnitId = model.LibraryUnitId,
                        ReaderId = model.ReaderId,
                        ReaderName = $"{reader.FirstName} {reader.MiddleName} {reader.LastName}"
                    };
                    return View("SavedWarning", warningModel);
                }
            }

            if(model.DeadLine <= DateOnly.FromDateTime(DateTime.Now))
            {
                TempData["error"] = "Крайният срок не е валиден.";
                return View(model);
            }

            if (ModelState.IsValid)
            {
                var identityUser = await _userManager.GetUserAsync(User);
                var user = _userService.GetWhere(x => x.IdentityUserId == identityUser.Id).First();

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
                unit.IsSavedByUser = false;
                unit.SavedByReaderId = null;
                await _libraryUnitService.UpdateAsync(unit);

                TempData["success"] = "Движението е регистрирано успешно.";

                return RedirectToAction("Register");
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = $"{SD.AdminRole},{SD.LibrarianRole}")]
        public async Task<IActionResult> RegisterGivingConfirmed(MovementWarningViewModel model)
        {
            if (model.DeadLine <= DateOnly.FromDateTime(DateTime.Now))
            {
                TempData["error"] = "Крайният срок не е валиден.";
                MovementViewModel mModel = new MovementViewModel() { ReaderId = model.ReaderId,
                    LibraryUnitId = model.LibraryUnitId, DeadLine = model.DeadLine };
                return RedirectToAction("RegisterGiving",mModel);
            }

            if (ModelState.IsValid)
            {
                var identityUser = await _userManager.GetUserAsync(User);
                var user = _userService.GetWhere(x => x.IdentityUserId == identityUser.Id).First();
                LibraryUnit libUnit = await _libraryUnitService.GetByIdAsync(model.LibraryUnitId);

                var movement = new MovementOfLibraryUnit
                {
                    DateTime = DateTime.Now,
                    Deadline = model.DeadLine,
                    Type = "Giving",
                    LibraryUnitId = model.LibraryUnitId,
                    ReaderId = model.ReaderId,
                    LibrarianId = user.Id,
                    Condition = libUnit.Condition
                };

                await _movementService.AddAsync(movement);
                libUnit.IsAvailable = false;
                await _libraryUnitService.UpdateAsync(libUnit);

                TempData["success"] = "Движението е регистрирано успешно.";

                return RedirectToAction("Register");
            }
            MovementViewModel mModel2 = new MovementViewModel()
            {
                ReaderId = model.ReaderId,
                LibraryUnitId = model.LibraryUnitId,
                DeadLine = model.DeadLine
            };
            return RedirectToAction("RegisterGiving", mModel2);
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
                MovementOfLibraryUnit oldMovement = _movementService.GetWhere(x => x.LibraryUnitId == model.LibraryUnitId && x.Type == "Giving").OrderByDescending(x => x.DateTime).First();

                var movement = new MovementOfLibraryUnit
                {
                    DateTime = DateTime.Now,
                    Deadline = null,
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
        public async Task<IActionResult> SearchAvailableLibraryUnits(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Json(new { });

            var results = (from u in await _libraryUnitService.GetAllAsync()
                           join t in await _titleService.GetAllAsync() on u.TitleId equals t.Id
                           where u.IsAvailable && (
                               u.InventoryNumber.Contains(query) ||
                               (u.Isbn != null && u.Isbn.Contains(query)) ||
                               t.Name.Contains(query))
                           select new
                           {
                               id = u.Id,
                               text = u.InventoryNumber + " - " + t.Name
                           }).ToList();


            return Json(results);
        }

        [HttpGet]
        [Authorize(Roles = $"{SD.AdminRole},{SD.LibrarianRole}")]
        public async Task<IActionResult> SearchUnavailableLibraryUnits(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Json(new { });

            var results = (from u in await _libraryUnitService.GetAllAsync()
                           join t in await _titleService.GetAllAsync() on u.TitleId equals t.Id
                           where !u.IsAvailable && !u.IsScrapped && (
                               u.InventoryNumber.Contains(query) ||
                               (u.Isbn != null && u.Isbn.Contains(query)) ||
                               t.Name.Contains(query))
                           select new
                           {
                               id = u.Id,
                               text = u.InventoryNumber + " - " + t.Name
                           }).ToList();

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
             text = $"{u.FirstName} {u.MiddleName} {u.LastName}"
            })
           .ToList();

            return Json(users);
        }
    }
}