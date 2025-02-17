using LibrarySystem.Models;
using LibrarySystem.Services;
using LibrarySystem.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using static System.Collections.Specialized.BitVector32;

namespace LibrarySystem.Web.Controllers
{
    public class LibraryUnitController : Controller
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

                LibraryUnitViewModel model = new LibraryUnitViewModel
                {
                    Id = libraryUnit.Id,
                    InventoryNumber = libraryUnit.InventoryNumber,
                    Condition = libraryUnit.Condition,
                    Medium = libraryUnit.Medium,
                    isScrapped = libraryUnit.IsScrapped,
                    TitleId = libraryUnit.TitleId,
                    TitleName = title.Name,
                    //Isbn = libraryUnit.Isbn,
                    TypeLibraryUnit = libraryUnit.TypeLibraryUnit,
                    //Year = (int)libraryUnit.Year,
                    //PublishingHouse = libraryUnit.PublishingHouse
                };

                if (libraryUnit.Isbn == null) model.Isbn = null;
                else model.Isbn = libraryUnit.Isbn;


                if (libraryUnit.Year == null) model.Year = null;
                else model.Year = libraryUnit.Year;


                if (libraryUnit.PublishingHouse == null) model.PublishingHouse = null;
                else model.PublishingHouse = libraryUnit.PublishingHouse;

                list.Add(model);
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
        public async Task<IActionResult> AddLibraryUnit(LibraryUnitViewModel model)
        {
            LibraryUnit unit = _libraryUnitService.GetWhere(x => x.InventoryNumber == model.InventoryNumber).FirstOrDefault();

            if (unit == null)
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
                else
                {
                    LibraryUnit newUnit = new LibraryUnit
                    {
                        InventoryNumber = model.InventoryNumber,
                        Condition = model.Condition,
                        Medium = model.Medium,
                        IsScrapped = false,
                        TitleId = model.TitleId,
                        Isbn = model.Isbn,
                        TypeLibraryUnit = model.TypeLibraryUnit,
                        Year = model.Year,
                        PublishingHouse = model.PublishingHouse
                    };
                    await _libraryUnitService.AddAsync(newUnit);
                    TempData["success"] = "Успешно добавена библиотечна единица";
                    return RedirectToAction("AllLibraryUnits");
                }
            }
            else
            {
                TempData["error"] = "Вече съществува библиотечна единица с този инвентарен номер";

                model.Titles = _titleService.GetAllAsync().Result.Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = e.Name
                });
                return View(model);
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            LibraryUnit unit = await _libraryUnitService.GetByIdAsync(id);
            Title title = _titleService.GetWhere(x => x.Id == unit.TitleId).FirstOrDefault();

            LibraryUnitViewModel model = new LibraryUnitViewModel
            {
                Id = unit.Id,
                InventoryNumber = unit.InventoryNumber,
                Condition = unit.Condition,
                Medium = unit.Medium,
                isScrapped = unit.IsScrapped,
                TitleId = unit.TitleId,
                TitleName = title.Name,
                //Isbn = libraryUnit.Isbn,
                TypeLibraryUnit = unit.TypeLibraryUnit,
                //Year = (int)libraryUnit.Year,
                //PublishingHouse = libraryUnit.PublishingHouse
            };

            if (unit.Isbn == null) model.Isbn = null;
            else model.Isbn = unit.Isbn;


            if (unit.Year == null) model.Year = null;
            else model.Year = unit.Year;


            if (unit.PublishingHouse == null) model.PublishingHouse = null;
            else model.PublishingHouse = unit.PublishingHouse;


            return View(model);
        }

        public async Task<IActionResult> Update(int id)
        {
            LibraryUnit unit = await _libraryUnitService.GetByIdAsync(id);
            Title title = _titleService.GetWhere(x => x.Id == unit.TitleId).FirstOrDefault();

            IEnumerable<Title> allTitles = await _titleService.GetAllAsync();

            LibraryUnitViewModel model = new LibraryUnitViewModel
            {
                Titles = _titleService.GetAllAsync().Result.Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = e.Name
                }),
                Id = unit.Id,
                InventoryNumber = unit.InventoryNumber,
                Condition = unit.Condition,
                Medium = unit.Medium,
                isScrapped = unit.IsScrapped,
                TitleId = unit.TitleId,
                TitleName = title.Name,
                //Isbn = libraryUnit.Isbn,
                TypeLibraryUnit = unit.TypeLibraryUnit,
                //Year = (int)libraryUnit.Year,
                //PublishingHouse = libraryUnit.PublishingHouse
            };

            if (unit.Isbn == null) model.Isbn = null;
            else model.Isbn = unit.Isbn;


            if (unit.Year == null) model.Year = null;
            else model.Year = unit.Year;


            if (unit.PublishingHouse == null) model.PublishingHouse = null;
            else model.PublishingHouse = unit.PublishingHouse;


            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(LibraryUnitViewModel model)
        {
            LibraryUnit unit = await _libraryUnitService.GetByIdAsync(model.Id);
            IEnumerable<LibraryUnit> libraryUnits = _libraryUnitService.GetWhere(x => x.InventoryNumber == model.InventoryNumber);
            libraryUnits = libraryUnits.Where(x => x.Id != model.Id);

            if (!libraryUnits.Any())
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
                else
                {
                    unit.InventoryNumber = model.InventoryNumber;
                    unit.Condition = model.Condition;
                    unit.Medium = model.Medium;
                    unit.IsScrapped = false;
                    unit.TitleId = model.TitleId;
                    unit.Isbn = model.Isbn;
                    unit.TypeLibraryUnit = model.TypeLibraryUnit;
                    unit.Year = model.Year;
                    unit.PublishingHouse = model.PublishingHouse;
                   
                    await _libraryUnitService.UpdateAsync(unit);
                    TempData["success"] = "Успешно редактирана библиотечна единица";
                    return RedirectToAction("AllLibraryUnits");
                }
            }
            else
            {
                TempData["error"] = "Вече съществува библиотечна единица с този инвентарен номер";

                model.Titles = _titleService.GetAllAsync().Result.Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = e.Name
                });
                return View(model);
            }
        }

        public async Task<IActionResult> Scrape(int id)
        {
            LibraryUnit unit = await _libraryUnitService.GetByIdAsync(id);
            Title title = _titleService.GetWhere(x => x.Id == unit.TitleId).FirstOrDefault();

            LibraryUnitViewModel model = new LibraryUnitViewModel
            {
                Id = unit.Id,
                InventoryNumber = unit.InventoryNumber,
                Condition = unit.Condition,
                Medium = unit.Medium,
                isScrapped = unit.IsScrapped,
                TitleId = unit.TitleId,
                TitleName = title.Name,
                //Isbn = libraryUnit.Isbn,
                TypeLibraryUnit = unit.TypeLibraryUnit,
                //Year = (int)libraryUnit.Year,
                //PublishingHouse = libraryUnit.PublishingHouse
            };

            if (unit.Isbn == null) model.Isbn = null;
            else model.Isbn = unit.Isbn;


            if (unit.Year == null) model.Year = null;
            else model.Year = unit.Year;


            if (unit.PublishingHouse == null) model.PublishingHouse = null;
            else model.PublishingHouse = unit.PublishingHouse;

            return View("ConfirmScrape", model);
        }
    }
}