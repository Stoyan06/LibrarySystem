using CloudinaryDotNet.Actions;
using LibrarySystem.Data;
using LibrarySystem.Models;
using LibrarySystem.Services.IService;
using LibrarySystem.Utility;
using LibrarySystem.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

public class UserController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private IService<User> _userService;
    private IService<MovementOfLibraryUnit> _movementOfLibraryUnitService;
    private IService<LibraryUnit> _libraryUnitService;

    public UserController(UserManager<IdentityUser> userManager, IService<User> userService,
        RoleManager<IdentityRole> roleManager, IService<MovementOfLibraryUnit> movementOfLibraryUnitService,
        IService<LibraryUnit> libraryUnitService)
    {
        _userManager = userManager;
        _userService = userService;
        _roleManager = roleManager;
        _movementOfLibraryUnitService = movementOfLibraryUnitService;
        _libraryUnitService = libraryUnitService;
    }

    public async Task<IActionResult> UserInfo()
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
    public async Task<IActionResult> EditUser()
    {

        var user = await _userManager.GetUserAsync(User);

        var userDetails = _userService.GetWhere
            (u => u.IdentityUserId == user.Id)
            .Select(u => new EditUserViewModel
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
    public async Task<IActionResult> EditUser(EditUserViewModel model)
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
        TempData["success"] = "Информацията за потребителя е променена успешно.";

        return RedirectToAction("UserInfo");
    }

    public IActionResult Dashboard()
    {
        return View();
    }

    
    public async Task<IActionResult> ChangeUserPassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ChangeUserPassword(ChangeUserPasswordViewModel model)
    {
        
            if (!ModelState.IsValid)
            return View(model);

        var identityUser = await _userManager.GetUserAsync(User);

        var passwordCheck = await _userManager.CheckPasswordAsync(identityUser, model.OldPassword);
        if (!passwordCheck)
        {
            TempData["error"] = "Въведената стара парола е грешна.";
            return View(model);
        }

        var result = await _userManager.ChangePasswordAsync(identityUser, model.OldPassword, model.ConfirmNewPassword);
        if (result.Succeeded)
        {
            TempData["success"] = "Паролата е променена успешно.";
            return RedirectToAction("UserInfo", "User");
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

    public async Task<IActionResult> UserHistory()
    {
        List<UserHistoryViewModel> userHistoryViewModels = new List<UserHistoryViewModel>();
        string userId = _userManager.GetUserId(User);
        User user = _userService.GetWhere(x => x.IdentityUserId == userId).First();
        IEnumerable<MovementOfLibraryUnit> movements = _movementOfLibraryUnitService.GetWhere(x => x.ReaderId == user.Id).ToList();

        foreach(MovementOfLibraryUnit movement in movements)
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