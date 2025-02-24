using LibrarySystem.Models;
using LibrarySystem.Services.IService;
using LibrarySystem.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

public class AccountController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IService<User> _userService;
    public AccountController(
        UserManager<IdentityUser> userManager,
       SignInManager<IdentityUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        IService<User> userService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _userService = userService;

    }

    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, false);
            // var result = await _signInManager.PasswordSignInAsync("Admin", model.Password, model.RememberMe, false);            

            if (result.Succeeded)
            {
                TempData["Success"] = "Влизането е успешно";
                return RedirectToAction("Index", "Home");
            }
            TempData["Error"] = "Invalid login attempt.";
            ModelState.AddModelError("", "Invalid login attempt.");

        }
        return View(model);
    }

    public IActionResult Register() => View();


    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new IdentityUser { UserName = model.Email, Email = model.Email};
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                User userNew = new User
                {
                    FirstName = model.FirstName,
                    MiddleName = model.MiddleName,
                    LastName = model.LastName,
                    // Свързване с новосъздадения потребител
                    IdentityUserId = user.Id
                };

                  await _userService.AddAsync(userNew);
                  await _userManager.AddToRoleAsync(user, SD.UserRole); // По подразбиране новите потребители са "User"                    await _signInManager.SignInAsync(user, isPersistent: true);

                  await _signInManager.SignInAsync(user, isPersistent: false);
                TempData["Success"] = "Успешна регистрация!";
                return RedirectToAction("Index","Home");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }

        var existingUser = await _userManager.FindByEmailAsync(model.Email);
        if (existingUser != null)
        {
            ModelState.AddModelError("Email", "Този email вече е използван за регистрацията на акаунт!");
            return View(model);
        }
        return View(model);
    }

    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    public IActionResult AccessDenied()
    {
        return View();
    }
}