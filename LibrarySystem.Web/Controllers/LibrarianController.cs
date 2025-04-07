using LibrarySystem.Models;
using LibrarySystem.Services.IService;
using LibrarySystem.Utility;
using LibrarySystem.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.Web.Controllers
{
    public class LibrarianController : Controller
    {
        private readonly IService<User> _userService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IService<MovementOfLibraryUnit> _movementOfLibraryUnitService;
        private readonly IService<LibraryUnit> _libraryUnitService;
        private readonly RoleManager<IdentityRole> _roleManager;

        public LibrarianController(IService<User> userService, UserManager<IdentityUser> userManager, 
            IService<MovementOfLibraryUnit> movementOfLibraryUnitService, IService<LibraryUnit> libraryUnitService,
            RoleManager<IdentityRole> roleManager)
        {
            _userService = userService;
            _userManager = userManager;
            _movementOfLibraryUnitService = movementOfLibraryUnitService;
            _libraryUnitService = libraryUnitService;
            _roleManager = roleManager;
        }

        [Authorize(Roles = $"{SD.AdminRole},{SD.LibrarianRole}")]
        public IActionResult Dashboard()
        {
            return View();
        }

        public async Task<IActionResult> AllReaders(string searchTerm)
        {
            var users = await _userService.GetAllAsync();
            var allRoles = _roleManager.Roles;

            if (!string.IsNullOrEmpty(searchTerm))
            {
                users = users.Where(u =>
                    u.FirstName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    u.MiddleName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    u.LastName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                    ).ToList();
            }
            List<UserViewModel> list = new List<UserViewModel>();

            ViewData["SearchTerm"] = searchTerm;

            foreach (User user in users)
            {
                var identityUser = await _userManager.FindByIdAsync(user.IdentityUserId);
                var roles = await _userManager.GetRolesAsync(identityUser);
                string userRole = roles.FirstOrDefault();

                UserViewModel model = new UserViewModel
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    MiddleName = user.MiddleName,
                    LastName = user.LastName,
                    Username = identityUser.UserName,
                    Role = userRole
                };

                if (userRole != SD.LibrarianRole && userRole != SD.AdminRole)
                    list.Add(model);
            }
            return View(list);
        }

        public async Task<IActionResult> UserHistory(int id)
        {
            List<UserHistoryViewModel> userHistoryViewModels = new List<UserHistoryViewModel>();
            User user = await _userService.GetByIdAsync(id);
            IEnumerable<MovementOfLibraryUnit> movements = _movementOfLibraryUnitService.GetWhere(x => x.ReaderId == user.Id).ToList();

            foreach (MovementOfLibraryUnit movement in movements)
            {
                UserHistoryViewModel model = new UserHistoryViewModel()
                {
                    DateTime = movement.DateTime,
                    DeadLine = movement.Deadline,
                    TypeOperation = movement.Type,
                    LibraryUnit = await _libraryUnitService.GetByIdAsync(movement.LibraryUnitId),
                    Librarian = await _userService.GetByIdAsync(movement.LibrarianId),
                    Condition = movement.Condition
                };
                userHistoryViewModels.Add(model);
            }

            return View(userHistoryViewModels);
        }
    }
}
