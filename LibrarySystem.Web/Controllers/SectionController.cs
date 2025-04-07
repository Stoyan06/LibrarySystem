using LibrarySystem.Models;
using LibrarySystem.Services.IService;
using LibrarySystem.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Web.Controllers
{
    public class SectionController : Controller
    {
        private ISectionService _sectionService;

        public SectionController(ISectionService sectionService)
        {
            _sectionService = sectionService;
        }

        public async Task<IActionResult> AllSectionsList(string SearchTerm)
        {
            ViewData["SearchTerm"] = SearchTerm;

            var sections = await _sectionService.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                sections = sections
                    .Where(x => x.Name != null && x.Name.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)
                    || x.Description.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            return View(sections);
        }

        [Authorize(Roles = $"{SD.AdminRole},{SD.LibrarianRole}")]
        public async Task<IActionResult> AllSections(string SearchTerm)
        {
            ViewData["SearchTerm"] = SearchTerm;

            var sections = await _sectionService.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                sections = sections
                    .Where(x => x.Name != null && x.Name.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)
                    || x.Description.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            return View(sections);
        }

        [Authorize(Roles = $"{SD.AdminRole},{SD.LibrarianRole}")]
        public IActionResult AddSection()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = $"{SD.AdminRole},{SD.LibrarianRole}")]
        public async Task<IActionResult> AddSection(Section section)
        {
            if(section.Name == null || section.Name == string.Empty)
            {
                return View(section);
            }

            if (section.Description == null || section.Description == string.Empty)
                section.Description = "Няма описание";

            if(await _sectionService.ExisisSection(section.Name) == false)
            {
                await _sectionService.AddAsync(section);
                TempData["success"] = "Успешно добавен раздел.";
                return RedirectToAction("AllSections");
            }
            else
            {
                TempData["error"] = "Вече съществува раздел с това име.";
                return RedirectToAction("AddSection");
            }
        }

        [Authorize(Roles = $"{SD.AdminRole},{SD.LibrarianRole}")]
        public async Task<IActionResult> UpdateSection(int id)
        {
            Section sec = await _sectionService.GetByIdAsync(id);
            if (sec == null)
            {
                return View("~/Views/Shared/NotFound.cshtml");

            }
            return View(sec);
        }

        [HttpPost]
        [Authorize(Roles = $"{SD.AdminRole},{SD.LibrarianRole}")]
        public async Task<IActionResult> UpdateSection(Section section)
        {

            if (section.Name == null || section.Name == string.Empty)
            {
                return View(section);
            }

            if (section.Description == null || section.Description == string.Empty)
                section.Description = "Няма описание";

            Section check = await _sectionService.GetByIdAsync(section.Id);

            if (await _sectionService.ExisisSection(section.Name) == false 
                || check.Name == section.Name)
            {
                await _sectionService.UpdateAsync(section);
                TempData["success"] = "Успешно редактиран раздел.";
                return RedirectToAction("AllSections");
            }
            else
            {
                TempData["error"] = "Вече съществува раздел с това име.";
                return RedirectToAction("UpdateSection", section.Id);
            }
        }

        [Authorize(Roles = $"{SD.AdminRole},{SD.LibrarianRole}")]
        public async Task<IActionResult> RemoveSection(int id)
        {
            Section sec = await _sectionService.GetByIdAsync(id);
            if (sec == null)
            {
                return View("~/Views/Shared/NotFound.cshtml");

            }
            return View("ConfirmDelete", sec);
        }

        [HttpPost]
        [Authorize(Roles = $"{SD.AdminRole},{SD.LibrarianRole}")]
        public async Task<IActionResult> RemoveSection(Section section)
        {
            try
            {
                await _sectionService.DeleteAsync(section.Id);
                TempData["success"] = "Успешно изтрит раздел.";
                return RedirectToAction("AllSections");
            }
            catch (Exception ex)
            {
                TempData["error"] = "Разделът не може да бъде изтрит, защото към него има обвързани заглавия.";
                return RedirectToAction("AllSections");
            }
        }
    }
}