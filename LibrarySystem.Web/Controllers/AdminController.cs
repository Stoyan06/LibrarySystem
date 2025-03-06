using CloudinaryDotNet.Actions;
using LibrarySystem.Data;
using LibrarySystem.Models;
using LibrarySystem.Services.IService;
using LibrarySystem.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        var users = await _userService.GetAllAsync();

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

            list.Add(model);
        }

        return View(list);
    }
}