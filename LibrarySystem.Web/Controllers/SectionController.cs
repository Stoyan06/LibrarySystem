using LibrarySystem.Models;
using LibrarySystem.Services.IService;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.Web.Controllers
{
    public class SectionController : Controller
    {
        private IService<Section> _sectionService;

        public SectionController(IService<Section> sectionService)
        {
            _sectionService = sectionService;
        }

        public async Task<IActionResult> AllSections()
        {
            return View(await _sectionService.GetAllAsync());
        }

        public IActionResult AddSection()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddSection(Section section)
        {
            if(section.Name == null || section.Name == string.Empty)
            {
                return View(section);
            }

            if (section.Description == null || section.Description == string.Empty)
                section.Description = "Няма описание";

            await _sectionService.AddAsync(section);
            return RedirectToAction("AllSections");
        }

        public async Task<IActionResult> UpdateSection(int id)
        {
            return View(await _sectionService.GetByIdAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSection(Section section)
        {

            if (section.Name == null || section.Name == string.Empty)
            {
                return View(section);
            }

            if (section.Description == null || section.Description == string.Empty)
                section.Description = "Няма описание";

            await _sectionService.UpdateAsync(section);
            return RedirectToAction("AllSections");
        }

        public async Task<IActionResult> RemoveSection(int id)
        {
            await _sectionService.DeleteAsync(id);
            return RedirectToAction("AllSections");
        }
    }
}