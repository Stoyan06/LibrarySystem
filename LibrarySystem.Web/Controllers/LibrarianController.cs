using LibrarySystem.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.Web.Controllers
{
    public class LibrarianController : Controller
    {
        [Authorize(Roles = $"{SD.AdminRole},{SD.LibrarianRole}")]
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
