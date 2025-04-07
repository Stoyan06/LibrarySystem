using LibrarySystem.Controllers;
using LibrarySystem.Data;
using LibrarySystem.Models;
using LibrarySystem.Services.IService;
using LibrarySystem.Utility;
using LibrarySystem.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class AdminController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private IService<User> _userService;
    private IService<MovementOfLibraryUnit> _movementService;
    private IService<LibraryUnit> _libraryUnitService;

    public AdminController(UserManager<IdentityUser> userManager,
        IService<User> userService,
        RoleManager<IdentityRole> roleManager, IService<MovementOfLibraryUnit> movementService, IService<LibraryUnit> libraryUnitService)
    {
        _userManager = userManager;
        _userService = userService;
        _roleManager = roleManager;
        _movementService = movementService;
        _libraryUnitService = libraryUnitService;
    }

    [Authorize(Roles = SD.AdminRole)]
    public async Task<IActionResult> AdminInfo()
    {
        var user = await _userManager.GetUserAsync(User);

        var userDetails = _userService.GetWhere
            (u => u.IdentityUserId == user.Id)
            .Select(u => new UserViewModel
            {
                Username = user.UserName,
                Email = user.Email,
                FirstName = u.FirstName,
                MiddleName = u.MiddleName,
                LastName = u.LastName
            })
            .FirstOrDefault();

        return View(userDetails);
    }


    [Authorize(Roles = SD.AdminRole)]
    public async Task<IActionResult> EditAdmin()
    {
        var user = await _userManager.GetUserAsync(User);

        var userDetails = _userService.GetWhere
            (u => u.IdentityUserId == user.Id)
            .Select(u => new EditAdminViewModel
            {
                Email = user.Email,
                FirstName = u.FirstName,
                MiddleName = u.MiddleName,
                LastName = u.LastName
            })
            .FirstOrDefault();

        return View(userDetails);
    }

    [HttpPost]
    [Authorize(Roles = SD.AdminRole)]
    public async Task<IActionResult> EditAdmin(EditAdminViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.GetUserAsync(User);

        var userDetails = _userService.GetWhere(u => u.IdentityUserId == user.Id).First();

        userDetails.FirstName = model.FirstName;
        userDetails.MiddleName = model.MiddleName;
        userDetails.LastName = model.LastName;
        user.Email = model.Email;

        await _userService.UpdateAsync(userDetails);
        await _userManager.UpdateAsync(user);
        TempData["success"] = "Успешна промяна на данните.";

        return RedirectToAction("AdminInfo");
    }

    [Authorize(Roles = SD.AdminRole)]
    public IActionResult Dashboard()
    {
        return View();
    }

    [Authorize(Roles = SD.AdminRole)]
    public IActionResult ManageUsers(string searchTerm)
    {

        return View();
    }

    [Authorize(Roles = SD.AdminRole)]
    public async Task<IActionResult> ManageReaders(string searchTerm)
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

    [Authorize(Roles = SD.AdminRole)]
    public async Task<IActionResult> ManageLibrarians(string searchTerm)
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

        foreach (var user in users)
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
            if (userRole != SD.UserRole && userRole != SD.AdminRole)
                list.Add(model);
        }
        return View(list);
    }

    [Authorize(Roles = SD.AdminRole)]
    public IActionResult CreateReader()
    {
        return View();
    }

    [HttpPost]
    [Authorize(Roles = SD.AdminRole)]
    public async Task<IActionResult> CreateReader(ReaderViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        IdentityUser user = await _userManager.FindByEmailAsync(model.Email);

        if (user != null)
        {
            TempData["error"] = "Вече е регистриран потребител с този email адрес.";
            return View(model);
        }

        User newUser = new User()
        {
            FirstName = model.FirstName,
            MiddleName = model.MiddleName,
            LastName = model.LastName
        };

        var identityUser = new IdentityUser() { Email = model.Email, UserName = model.Email };
        var result = await _userManager.CreateAsync(identityUser, model.Password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(identityUser, SD.UserRole);

            newUser.IdentityUserId = identityUser.Id;
            await _userService.AddAsync(newUser);
        }
        TempData["success"] = "Успешно добавен читател.";
        return RedirectToAction("ManageReaders");
    }

    [Authorize(Roles = SD.AdminRole)]
    public async Task<IActionResult> ReaderDetails(int userId)
    {
        User user = await _userService.GetByIdAsync(userId);
        if (user == null)
        {
            return View("~/Views/Shared/NotFound.cshtml");

        }
        IdentityUser identityUser = await _userManager.FindByIdAsync(user.IdentityUserId);
        return View(new UserViewModel
        { FirstName = user.FirstName, LastName = user.LastName, MiddleName = user.MiddleName, Email = identityUser.Email });
    }

    [Authorize(Roles = SD.AdminRole)]
    public async Task<IActionResult> EditReader(int userId)
    {
        User user = await _userService.GetByIdAsync(userId);
        if (user == null)
        {
            return View("~/Views/Shared/NotFound.cshtml");

        }
        IdentityUser identityUser = await _userManager.FindByIdAsync(user.IdentityUserId);
        return View(new EditReaderViewModel
        { FirstName = user.FirstName, LastName = user.LastName, MiddleName = user.MiddleName, Email = identityUser.Email, Id = userId });
    }

    [HttpPost]
    [Authorize(Roles = SD.AdminRole)]
    public async Task<IActionResult> EditReader(EditReaderViewModel model)
    {
        IdentityUser identityUser = await _userManager.FindByEmailAsync(model.Email);
        User user = await _userService.GetByIdAsync(model.Id);

        if (identityUser != null && user.IdentityUserId != identityUser.Id)
        {
            TempData["error"] = "Този email вече е използван за друг акаунт.";
            return View(model);
        }
        else
        {
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.MiddleName = model.MiddleName;
            identityUser = await _userManager.FindByIdAsync(user.IdentityUserId);
            identityUser.Email = model.Email;
            await _userService.UpdateAsync(user);
            await _userManager.UpdateAsync(identityUser);
            TempData["success"] = "Успешна редакция.";
            return RedirectToAction("ManageReaders");
        }
    }

    [Authorize(Roles = SD.AdminRole)]
    public async Task<IActionResult> ConfirmDeleteReader(int userId)
    {
        User reader = await _userService.GetByIdAsync(userId);
        if (reader == null)
        {
            return View("~/Views/Shared/NotFound.cshtml");

        }
        IdentityUser identityUser = await _userManager.FindByIdAsync(reader.IdentityUserId);
        return View(new ReaderViewModel
        { Email = identityUser.Email, FirstName = reader.FirstName, MiddleName = reader.MiddleName, LastName = reader.LastName, Id = reader.Id });
    }

    [Authorize(Roles = SD.AdminRole)]
    public async Task<IActionResult> DeleteReader(int userId)
    {
        User reader = await _userService.GetByIdAsync(userId);
        if (reader == null)
        {
            return View("~/Views/Shared/NotFound.cshtml");

        }
        var userHistory = _movementService.GetWhere(x => x.ReaderId == userId && x.Deadline != null);
        var userHistoryCheck = _movementService.GetWhere(x => x.ReaderId == userId && x.Deadline == null);

        if (userHistory.Count() == userHistoryCheck.Count())
        {
            User user = await _userService.GetByIdAsync(userId);
            IdentityUser identityUser = await _userManager.FindByIdAsync(user.IdentityUserId);
            var savedByUser = _libraryUnitService.GetWhere(x => x.SavedByReaderId == userId);
            foreach (LibraryUnit unit in savedByUser)
            {
                unit.IsSavedByUser = false;
                unit.SavedByReaderId = null;
                await _libraryUnitService.UpdateAsync(unit);
            }
            await _movementService.DeleteWhere(x => x.ReaderId == userId);
            await _userService.DeleteAsync(user.Id);
            await _userManager.RemoveFromRoleAsync(identityUser, SD.UserRole);
            await _userManager.DeleteAsync(identityUser);
            TempData["success"] = "Читателят е премахнат успешно.";
            return RedirectToAction("ManageReaders");
        }
        else
        {
            TempData["error"] = "Читателят не може да бъде премахнат, защото има заети материали, които трябва да върне.";
            return RedirectToAction("ManageReaders");
        }
    }


    [Authorize(Roles = SD.AdminRole)]
    public IActionResult CreateLibrarian()
    {
        return View();
    }

    [HttpPost]
    [Authorize(Roles = SD.AdminRole)]
    public async Task<IActionResult> CreateLibrarian(ReaderViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        IdentityUser user = await _userManager.FindByEmailAsync(model.Email);

        if (user != null)
        {
            TempData["error"] = "Вече е регистриран потребител с този email адрес.";
            return View(model);
        }

        User newUser = new User()
        {
            FirstName = model.FirstName,
            MiddleName = model.MiddleName,
            LastName = model.LastName
        };

        var identityUser = new IdentityUser() { Email = model.Email, UserName = model.Email };
        var result = await _userManager.CreateAsync(identityUser, model.Password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(identityUser, SD.LibrarianRole);

            newUser.IdentityUserId = identityUser.Id;
            await _userService.AddAsync(newUser);
        }
        TempData["success"] = "Успешно добавен библиотекар.";
        return RedirectToAction("ManageLibrarians");
    }

    [Authorize(Roles = SD.AdminRole)]
    public async Task<IActionResult> LibrarianDetails(int userId)
    {
        User user = await _userService.GetByIdAsync(userId);
        if (user == null)
        {
            return View("~/Views/Shared/NotFound.cshtml");

        }
        IdentityUser identityUser = await _userManager.FindByIdAsync(user.IdentityUserId);
        return View(new UserViewModel
        { FirstName = user.FirstName, LastName = user.LastName, MiddleName = user.MiddleName, Email = identityUser.Email });
    }

    [Authorize(Roles = SD.AdminRole)]
    public async Task<IActionResult> EditLibrarian(int userId)
    {
        User user = await _userService.GetByIdAsync(userId);
        if (user == null)
        {
            return View("~/Views/Shared/NotFound.cshtml");

        }
        IdentityUser identityUser = await _userManager.FindByIdAsync(user.IdentityUserId);
        return View(new EditLibrarianViewModel
        { FirstName = user.FirstName, LastName = user.LastName, MiddleName = user.MiddleName, Email = identityUser.Email, Id = userId });
    }

    [HttpPost]
    [Authorize(Roles = SD.AdminRole)]
    public async Task<IActionResult> EditLibrarian(EditLibrarianViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        IdentityUser identityUser = await _userManager.FindByEmailAsync(model.Email);
        User user = await _userService.GetByIdAsync(model.Id);

        if (identityUser != null && user.IdentityUserId != identityUser.Id)
        {
            TempData["error"] = "Този email вече е използван за друг акаунт.";
            return View(model);
        }
        else
        {
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.MiddleName = model.MiddleName;
            identityUser = await _userManager.FindByIdAsync(user.IdentityUserId);
            identityUser.Email = model.Email;
            await _userService.UpdateAsync(user);
            await _userManager.UpdateAsync(identityUser);
            TempData["success"] = "Успешна редакция на библиотекар.";
            return RedirectToAction("ManageLibrarians");
        }
    }

    [Authorize(Roles = SD.AdminRole)]
    public async Task<IActionResult> ConfirmDeleteLibrarian(int userId)
    {
        User librarian = await _userService.GetByIdAsync(userId);
        if (librarian == null)
        {
            return View("~/Views/Shared/NotFound.cshtml");

        }
        IdentityUser identityUser = await _userManager.FindByIdAsync(librarian.IdentityUserId);
        return View(new LibrarianViewModel
        {
            Email = identityUser.Email,
            FirstName = librarian.FirstName,
            MiddleName = librarian.MiddleName,
            LastName = librarian.LastName,
            Id = librarian.Id
        });
    }

    [Authorize(Roles = SD.AdminRole)]
    public async Task<IActionResult> DeleteLibrarian(int userId)
    {
        User user = await _userService.GetByIdAsync(userId);
        if (user == null)
        {
            return View("~/Views/Shared/NotFound.cshtml");

        }
        IdentityUser identityUser = await _userManager.FindByIdAsync(user.IdentityUserId);
        await _userService.DeleteAsync(user.Id);
        await _userManager.RemoveFromRoleAsync(identityUser, SD.LibrarianRole);
        await _userManager.DeleteAsync(identityUser);
        TempData["success"] = "Успешно премахнат библиотекар.";
        return RedirectToAction("ManageLibrarians");
    }

    [Authorize(Roles = SD.AdminRole)]
    public async Task<IActionResult> ChangeReaderPassword(int userId)
    {
        User reader = await _userService.GetByIdAsync(userId);
        if (reader == null)
        {
            return View("~/Views/Shared/NotFound.cshtml");

        }
        IdentityUser identityUser = await _userManager.FindByIdAsync(reader.IdentityUserId);
        return View(new ChangePasswordViewModel
        {
            Email = identityUser.Email,
            FirstName = reader.FirstName,
            MiddleName = reader.MiddleName,
            LastName = reader.LastName,
            Id = reader.Id
        });
    }

    [HttpPost]
    [Authorize(Roles = SD.AdminRole)]
    public async Task<IActionResult> ChangeReaderPassword(ChangePasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            User reader = await _userService.GetByIdAsync(model.Id);
            IdentityUser identityUser = await _userManager.FindByIdAsync(reader.IdentityUserId);
            await _userManager.RemovePasswordAsync(identityUser);
            await _userManager.AddPasswordAsync(identityUser, model.ConfirmNewPassword);
            TempData["success"] = "Паролата е сменена успешно.";
            return RedirectToAction("EditReader", new { userId = model.Id });
        }
        else return View(model);
    }

    [Authorize(Roles = SD.AdminRole)]
    public async Task<IActionResult> ChangeLibrarianPassword(int userId)
    {
        User librarian = await _userService.GetByIdAsync(userId);
        if (librarian == null)
        {
            return View("~/Views/Shared/NotFound.cshtml");

        }
        IdentityUser identityUser = await _userManager.FindByIdAsync(librarian.IdentityUserId);
        return View(new ChangePasswordViewModel
        {
            Email = identityUser.Email,
            FirstName = librarian.FirstName,
            MiddleName = librarian.MiddleName,
            LastName = librarian.LastName,
            Id = librarian.Id
        });
    }

    [HttpPost]
    [Authorize(Roles = SD.AdminRole)]
    public async Task<IActionResult> ChangeLibrarianPassword(ChangePasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            User reader = await _userService.GetByIdAsync(model.Id);
            IdentityUser identityUser = await _userManager.FindByIdAsync(reader.IdentityUserId);
            await _userManager.RemovePasswordAsync(identityUser);
            await _userManager.AddPasswordAsync(identityUser, model.ConfirmNewPassword);

            TempData["success"] = "Паролата е сменена успешно.";
            return RedirectToAction("EditLibrarian", new { userId = model.Id });
        }
        else return View(model);
    }

    [Authorize(Roles = SD.AdminRole)]
    public IActionResult ChangeAdminPassword()
    {
        return View();
    }

    [HttpPost]
    [Authorize(Roles = SD.AdminRole)]
    public async Task<IActionResult> ChangeAdminPassword(ChangeAdminPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var identityUser = await _userManager.GetUserAsync(User);

        var passwordCheck = await _userManager.CheckPasswordAsync(identityUser, model.OldPassword);
        if (!passwordCheck)
        {
            TempData["error"] = "Старата парола е грешна.";
            return View(model);
        }

        var result = await _userManager.ChangePasswordAsync(identityUser, model.OldPassword, model.ConfirmNewPassword);
        if (result.Succeeded)
        {
            TempData["success"] = "Паролата е променена успешно.";
            return RedirectToAction("AdminInfo", "Admin");
        }
        else
        {
            foreach (var error in result.Errors)
            {
                TempData["error"] = error.Description;
            }

            return View(model);
        }
    }
}