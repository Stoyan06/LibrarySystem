using LibrarySystem.Utility;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.Web.Controllers
{
    public class LibrarianController : Controller
    {
        public IActionResult Dashboard()
        {
            if (User.IsInRole(SD.AdminRole) || User.IsInRole(SD.LibrarianRole))
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
