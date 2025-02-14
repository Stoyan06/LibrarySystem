using LibrarySystem.Models;
using LibrarySystem.Services;
using LibrarySystem.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using static System.Collections.Specialized.BitVector32;

namespace LibrarySystem.Web.Controllers
{
    public class LibraryUnitController:Controller
    {
        private IService<LibraryUnit> _libraryUnitService;
        private IService<Title> _titleService;

        public LibraryUnitController(IService<LibraryUnit> libraryUnitService, IService<Title> titleService)
        {
            _libraryUnitService = libraryUnitService;
            _titleService = titleService;
        }

        public async Task<IActionResult> AllLibraryUnits()
        {
            List<LibraryUnitViewModel> list = new List<LibraryUnitViewModel>();
            IEnumerable<LibraryUnit> libraryUnits = await _libraryUnitService.GetAllAsync();

            foreach (LibraryUnit libraryUnit in libraryUnits)
            {
                Title title = await _titleService.GetByIdAsync(libraryUnit.TitleId);

                list.Add(new LibraryUnitViewModel
                {
                    InventoryNumber = libraryUnit.InventoryNumber,
                    Condition = libraryUnit.Condition,
                    Medium = libraryUnit.Medium,
                    isScrapped = libraryUnit.IsScrapped,
                    TitleId = libraryUnit.TitleId,
                    TitleName = title.Name,
                    Isbn = libraryUnit.Isbn,
                    TypeLibraryUnit = libraryUnit.TypeLibraryUnit,
                    Year = libraryUnit.Year,
                    PublishingHouse = libraryUnit.PublishingHouse
                });
            }

            return View(list);
        }

        public async Task<IActionResult> AddLibraryUnit()
        {
            IEnumerable<Title> allTitles = await _titleService.GetAllAsync();
            if (!allTitles.Any())
                return View("NoTitlesWarning");
            else
            {
                LibraryUnitViewModel model = new LibraryUnitViewModel()
                {
                    Titles = _titleService.GetAllAsync().Result.Select(e => new SelectListItem
                    {
                        Value = e.Id.ToString(),
                        Text = e.Name
                    }),
                };
                return View(model);
            }
        }

        [HttpPost]
        public IActionResult AddLibraryUnit(LibraryUnitViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Titles = _titleService.GetAllAsync().Result.Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = e.Name
                });

                return View(model);
            }
            else return View("AllLibraryUnits");
        }
    }
}
