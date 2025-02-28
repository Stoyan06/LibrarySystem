using LibrarySystem.Data;
using LibrarySystem.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class AdminController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ApplicationDbContext _context;

    public AdminController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
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
}