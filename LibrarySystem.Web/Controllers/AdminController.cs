using CloudinaryDotNet.Actions;
using LibrarySystem.Data;
using LibrarySystem.Models;
using LibrarySystem.Services.IService;
using LibrarySystem.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

public class AdminController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _context;
    private IService<User> _userService;

    public AdminController(UserManager<IdentityUser> userManager,
        ApplicationDbContext context, IService<User> userService,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _context = context;
        _userService = userService;
        _roleManager = roleManager;
    }

    public async Task<IActionResult> AdminInfo()
    {
        if (User.IsInRole(SD.AdminRole))
        {
            var user = await _userManager.GetUserAsync(User);

            var userDetails = _context.Users
                .Where(u => u.IdentityUserId == user.Id)
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
        else
        {
            return View("AccessDenied");
        }
    }
    public async Task<IActionResult> EditAdmin()
    {
        if(!User.IsInRole(SD.AdminRole)) return View("AccessDenied");

        var user = await _userManager.GetUserAsync(User);

        var userDetails = _context.Users
            .Where(u => u.IdentityUserId == user.Id)
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

    [HttpPost]
    public async Task<IActionResult> EditAdmin(UserViewModel model)
    {
        if (!User.IsInRole(SD.AdminRole)) return View("AccessDenied");

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.GetUserAsync(User);

        var userDetails = await _context.Users.FirstOrDefaultAsync(u => u.IdentityUserId == user.Id);
        

        // Update user details
        userDetails.FirstName = model.FirstName;
        userDetails.MiddleName = model.MiddleName;
        userDetails.LastName = model.LastName;

        // Save changes
        _context.Users.Update(userDetails);
        await _context.SaveChangesAsync();

        // Handle password change
        if (!string.IsNullOrEmpty(model.NewPassword))
        {
            if (string.IsNullOrEmpty(model.OldPassword))
            {
                ModelState.AddModelError("", "Моля, въведете старата парола.");
                return View(model);
            }

            var passwordCheck = await _userManager.CheckPasswordAsync(user, model.OldPassword);
            if (!passwordCheck)
            {
                ModelState.AddModelError("", "Старата парола е грешна.");
                return View(model);
            }

            if (model.NewPassword != model.ConfirmNewPassword)
            {
                ModelState.AddModelError("", "Новите пароли не съвпадат.");
                return View(model);
            }

            var passwordChangeResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!passwordChangeResult.Succeeded)
            {
                foreach (var error in passwordChangeResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }
        }

        return RedirectToAction("AdminInfo");
    }

    public IActionResult Dashboard()
    {
        if (User.IsInRole(SD.AdminRole))
        {
            return View();
        }
        else
        {
            return View("AccessDenied");
        }
    }

    public async Task<IActionResult> ManageUsers(string searchTerm)
    {
        if (!User.IsInRole(SD.AdminRole)) return View("AccessDenied");

        return View();
    }

    public async Task<IActionResult> ManageReaders(string searchTerm)
    {
        if (!User.IsInRole(SD.AdminRole)) return View("AccessDenied");

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
            var roles = await _userManager.GetRolesAsync(identityUser); // Get user roles
            string userRole = roles.FirstOrDefault(); // Get first role or set default

            UserViewModel model = new UserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
                Username = identityUser.UserName,
                Role = userRole // Assign the retrieved role
            };

            if(userRole != SD.LibrarianRole && userRole != SD.AdminRole)
            list.Add(model);
        }
        return View(list);
    }

    public async Task<IActionResult> ManageLibrarians(string searchTerm)
    {
        if (!User.IsInRole(SD.AdminRole)) return View("AccessDenied");

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
            var roles = await _userManager.GetRolesAsync(identityUser); // Get user roles
            string userRole = roles.FirstOrDefault(); // Get first role or set default

            UserViewModel model = new UserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
                Username = identityUser.UserName,
                Role = userRole // Assign the retrieved role
            };
            if (userRole != SD.UserRole && userRole != SD.AdminRole)
                list.Add(model);
        }
        return View(list);
    }

    public IActionResult CreateReader()
    {
        if (User.IsInRole(SD.AdminRole))
            return View();
        else return View("AccessDenied");
    }

    [HttpPost]
    public async Task<IActionResult> CreateReader(ReaderViewModel model)
    {
        if (!User.IsInRole(SD.AdminRole))
        return View("AccessDenied");

        if (!ModelState.IsValid)
            return View(model);

        IdentityUser user = await _userManager.FindByEmailAsync(model.Email);

        if (user != null)
        {
            TempData["error"] = "Вече е регистриран потребител с този имейл";
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
        TempData["success"] = "Успешно добавен читател";
        return RedirectToAction("ManageReaders");
    }

    public async Task<IActionResult> ReaderDetails(int userId)
    {
        if (!User.IsInRole(SD.AdminRole))
            return View("AccessDenied");

        User user = await _userService.GetByIdAsync(userId);
        IdentityUser identityUser = await _userManager.FindByIdAsync(user.IdentityUserId);
        return View(new UserViewModel 
        { FirstName = user.FirstName, LastName = user.LastName, MiddleName = user.MiddleName, Email = identityUser.Email});
    }

    public async Task<IActionResult> EditReader(int userId)
    {
        if (!User.IsInRole(SD.AdminRole))
            return View("AccessDenied");

        User user = await _userService.GetByIdAsync(userId);
        IdentityUser identityUser = await _userManager.FindByIdAsync(user.IdentityUserId);
        return View(new EditReaderViewModel
        { FirstName = user.FirstName, LastName = user.LastName, MiddleName = user.MiddleName, Email = identityUser.Email, Id = userId });
    }

    [HttpPost]
    public async Task<IActionResult> EditReader(EditReaderViewModel model)
    {

        if (!User.IsInRole(SD.AdminRole))
            return View("AccessDenied");
        IdentityUser identityUser = await _userManager.FindByEmailAsync(model.Email);
        User user = await _userService.GetByIdAsync(model.Id);

        if (identityUser != null && user.IdentityUserId != identityUser.Id)
        {
            TempData["error"] = "Този имейл вече е използван за друг акаунт";
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
            TempData["success"] = "Успешна редакция";
            return RedirectToAction("ManageReaders");
        }
    }
    public async Task<IActionResult> ConfirmDeleteReader(int userId)
    {
        if(!User.IsInRole(SD.AdminRole))
            return View("AccessDenied");

        User reader = await _userService.GetByIdAsync(userId);
        IdentityUser identityUser = await _userManager.FindByIdAsync(reader.IdentityUserId);
        return View(new ReaderViewModel 
        { Email = identityUser.Email, FirstName = reader.FirstName, MiddleName = reader.MiddleName, LastName = reader.LastName, Id = reader.Id});
    }

    public async Task<IActionResult> DeleteReader(int userId)
    {
        if(!User.IsInRole(SD.AdminRole))
            return View("AccessDenied");

        User user = await _userService.GetByIdAsync(userId);
        IdentityUser identityUser = await _userManager.FindByIdAsync(user.IdentityUserId);
        await _userService.DeleteAsync(user.Id);
        await _userManager.RemoveFromRoleAsync(identityUser, SD.UserRole);
        await _userManager.DeleteAsync(identityUser);
        TempData["success"] = "Успешно премахнат читател";
        return RedirectToAction("ManageReaders");
    }

    //librarian
    public IActionResult CreateLibrarian()
    {
        if (User.IsInRole(SD.AdminRole))
            return View();
        else return View("AccessDenied");
    }

    [HttpPost]
    public async Task<IActionResult> CreateLibrarian(ReaderViewModel model)
    {
        if (!User.IsInRole(SD.AdminRole))
            return View("AccessDenied");

        if (!ModelState.IsValid)
            return View(model);

        IdentityUser user = await _userManager.FindByEmailAsync(model.Email);

        if (user != null)
        {
            TempData["error"] = "Вече е регистриран потребител с този имейл";
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
        TempData["success"] = "Успешно добавен библиотекар";
        return RedirectToAction("ManageLibrarians");
    }

    public async Task<IActionResult> LibrarianDetails(int userId)
    {
        if (!User.IsInRole(SD.AdminRole))
            return View("AccessDenied");

        User user = await _userService.GetByIdAsync(userId);
        IdentityUser identityUser = await _userManager.FindByIdAsync(user.IdentityUserId);
        return View(new UserViewModel
        { FirstName = user.FirstName, LastName = user.LastName, MiddleName = user.MiddleName, Email = identityUser.Email });
    }

    public async Task<IActionResult> EditLibrarian(int userId)
    {
        if (!User.IsInRole(SD.AdminRole))
            return View("AccessDenied");

        User user = await _userService.GetByIdAsync(userId);
        IdentityUser identityUser = await _userManager.FindByIdAsync(user.IdentityUserId);
        return View(new EditLibrarianViewModel
        { FirstName = user.FirstName, LastName = user.LastName, MiddleName = user.MiddleName, Email = identityUser.Email, Id = userId });
    }

    [HttpPost]
    public async Task<IActionResult> EditLibrarian(EditLibrarianViewModel model)
    {

        if (!User.IsInRole(SD.AdminRole))
            return View("AccessDenied");
        IdentityUser identityUser = await _userManager.FindByEmailAsync(model.Email);
        User user = await _userService.GetByIdAsync(model.Id);

        if (identityUser != null && user.IdentityUserId != identityUser.Id)
        {
            TempData["error"] = "Този имейл вече е използван за друг акаунт";
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
            TempData["success"] = "Успешна редакция";
            return RedirectToAction("ManageLibrarians");
        }
    }
    public async Task<IActionResult> ConfirmDeleteLibrarian(int userId)
    {
        if (!User.IsInRole(SD.AdminRole))
            return View("AccessDenied");

        User reader = await _userService.GetByIdAsync(userId);
        IdentityUser identityUser = await _userManager.FindByIdAsync(reader.IdentityUserId);
        return View(new LibrarianViewModel
        { Email = identityUser.Email, FirstName = reader.FirstName, MiddleName = reader.MiddleName, LastName = reader.LastName, Id = reader.Id });
    }

    public async Task<IActionResult> DeleteLibrarian(int userId)
    {
        if (!User.IsInRole(SD.AdminRole))
            return View("AccessDenied");

        User user = await _userService.GetByIdAsync(userId);
        IdentityUser identityUser = await _userManager.FindByIdAsync(user.IdentityUserId);
        await _userService.DeleteAsync(user.Id);
        await _userManager.RemoveFromRoleAsync(identityUser, SD.LibrarianRole);
        await _userManager.DeleteAsync(identityUser);
        TempData["success"] = "Успешно премахнат библиотекар";
        return RedirectToAction("ManageLibrarians");
    }

    public async Task<IActionResult> ChangeReaderPassword(int userId)
    {

        if (!User.IsInRole(SD.AdminRole))
            return View("AccessDenied");

        User reader = await _userService.GetByIdAsync(userId);
        IdentityUser identityUser = await _userManager.FindByIdAsync(reader.IdentityUserId);
        return View(new ChangePasswordViewModel 
        { Email = identityUser.Email, 
            FirstName = reader.FirstName,
            MiddleName = reader.MiddleName, LastName = reader.LastName, Id = reader.Id});
    }

    [HttpPost]
    public async Task<IActionResult> ChangeReaderPassword(ChangePasswordViewModel model)
    {
        if (!User.IsInRole(SD.AdminRole))
            return View("AccessDenied");

        if (ModelState.IsValid)
        {
            User reader = await _userService.GetByIdAsync(model.Id);
            IdentityUser identityUser = await _userManager.FindByIdAsync(reader.IdentityUserId);
            await _userManager.RemovePasswordAsync(identityUser);
            await _userManager.AddPasswordAsync(identityUser, model.ConfirmNewPassword);
            TempData["success"] = "Успешна смяна на паролата";
            return RedirectToAction("EditReader", new {userId = model.Id});
        }
        else return View(model);
    }

    public async Task<IActionResult> ChangeLibrarianPassword(int userId)
    {

        if (!User.IsInRole(SD.AdminRole))
            return View("AccessDenied");

        User reader = await _userService.GetByIdAsync(userId);
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
    public async Task<IActionResult> ChangeLibrarianPassword(ChangePasswordViewModel model)
    {
        if (!User.IsInRole(SD.AdminRole))
            return View("AccessDenied");

        if (ModelState.IsValid)
        {
            User reader = await _userService.GetByIdAsync(model.Id);
            IdentityUser identityUser = await _userManager.FindByIdAsync(reader.IdentityUserId);
            await _userManager.RemovePasswordAsync(identityUser);
            await _userManager.AddPasswordAsync(identityUser, model.ConfirmNewPassword);

            TempData["success"] = "Успешна смяна на паролата";
            return RedirectToAction("EditLibrarian",new { userId = model.Id });
        }
        else return View(model);
    }

    public async Task<IActionResult> ChangeAdminPassword()
    {
        if (!User.IsInRole(SD.AdminRole))
            return View("AccessDenied");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ChangeAdminPassword(ChangePasswordViewModel model)
    {
        if (!User.IsInRole(SD.AdminRole))
            return View("AccessDenied");

        var admins = await _userManager.GetUsersInRoleAsync(SD.AdminRole);
        var admin = admins.FirstOrDefault();

        if (!ModelState.IsValid)
        {
            return View(model);
        }
        
        //if(model.OldPassword.ToHashSet().ToString() == admin.PasswordHash)
        //{
            TempData["success"] = "success";
        //}
        //else
       // {
            TempData["error"] = "error";
       // }
        return RedirectToAction("AdminInfo");
    }
}