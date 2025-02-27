using LibrarySystem.Utility;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.Web.Controllers
{
    public class AdminController : Controller
    {
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
}
