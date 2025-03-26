using LibrarySystem.Models;
using LibrarySystem.Services;
using LibrarySystem.Services.IService;
using LibrarySystem.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Query.Internal;
using NuGet.Protocol.Core.Types;
using static System.Collections.Specialized.BitVector32;

namespace LibrarySystem.Web.Controllers
{
    public class LibraryUnitController : Controller
    {
        private IService<LibraryUnit> _libraryUnitService;
        private IService<Title> _titleService;
        private CloudinaryService _cloudinaryService;
        private IService<Image> _imageService;
        private IService<ScrappedUnit> _scrappedUnitService;
        private IService<User> _userService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public LibraryUnitController(IService<LibraryUnit> libraryUnitService,
            IService<Title> titleService, CloudinaryService cloudinaryService,
            IService<Image> imageService, IService<ScrappedUnit> screppedUnitService,
            IService<User> userService, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _libraryUnitService = libraryUnitService;
            _titleService = titleService;
            _cloudinaryService = cloudinaryService;
            _imageService = imageService;
            _scrappedUnitService = screppedUnitService;
            _userService = userService;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> AllLibraryUnits()
        {
            if(!User.IsInRole(SD.LibrarianRole) && !User.IsInRole(SD.AdminRole)){
                return View("AccessDenied");
            }
            List<LibraryUnitViewModel> list = new List<LibraryUnitViewModel>();
            IEnumerable<LibraryUnit> libraryUnits = await _libraryUnitService.GetAllAsync();

            foreach (LibraryUnit libraryUnit in libraryUnits)
            {
                Title title = await _titleService.GetByIdAsync(libraryUnit.TitleId);
                Image image = _imageService.GetWhere(x => x.Id == libraryUnit.ImageId).FirstOrDefault();

                LibraryUnitViewModel model = new LibraryUnitViewModel
                {
                    Id = libraryUnit.Id,
                    InventoryNumber = libraryUnit.InventoryNumber,
                    Condition = libraryUnit.Condition,
                    Medium = libraryUnit.Medium,
                    isScrapped = libraryUnit.IsScrapped,
                    TitleId = libraryUnit.TitleId,
                    TitleName = title.Name,
                    TypeLibraryUnit = libraryUnit.TypeLibraryUnit,
                    Image = image
                };

                if (libraryUnit.Isbn == null) model.Isbn = null;
                else model.Isbn = libraryUnit.Isbn;


                if (libraryUnit.Year == null) model.Year = null;
                else model.Year = libraryUnit.Year;


                if (libraryUnit.PublishingHouse == null) model.PublishingHouse = null;
                else model.PublishingHouse = libraryUnit.PublishingHouse;

                list.Add(model);
            }

            return View(list.Where(x => x.isScrapped == false));
        }

        public async Task<IActionResult> ScrappedLibraryUnits()
        {
            if (!User.IsInRole(SD.LibrarianRole) && !User.IsInRole(SD.AdminRole))
            {
                return View("AccessDenied");
            }
            List<LibraryUnitViewModel> list = new List<LibraryUnitViewModel>();
            IEnumerable<LibraryUnit> libraryUnits = _libraryUnitService.GetWhere(x => x.IsScrapped == true).ToList();

            foreach (LibraryUnit libraryUnit in libraryUnits)
            {
                Title title = await _titleService.GetByIdAsync(libraryUnit.TitleId);
                Image image = _imageService.GetWhere(x => x.Id == libraryUnit.ImageId).FirstOrDefault();

                LibraryUnitViewModel model = new LibraryUnitViewModel
                {
                    Id = libraryUnit.Id,
                    InventoryNumber = libraryUnit.InventoryNumber,
                    Condition = libraryUnit.Condition,
                    Medium = libraryUnit.Medium,
                    isScrapped = libraryUnit.IsScrapped,
                    TitleId = libraryUnit.TitleId,
                    TitleName = title.Name,
                    TypeLibraryUnit = libraryUnit.TypeLibraryUnit,
                    Image = image
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

            LibraryUnit unitIsbn = null;

            if(model.Isbn != String.Empty && model.Isbn != null)
            {
                unitIsbn = _libraryUnitService.GetWhere(x => x.Isbn == model.Isbn).FirstOrDefault();
            }

            if (unit == null && unitIsbn == null)
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
                    var ImageLink = await _cloudinaryService.UploadImageAsync(model.UploadedImage);
                    await _imageService.AddAsync(new Image { DestinationLink = ImageLink });
                    Image image = _imageService.GetWhere(x => x.DestinationLink == ImageLink).First();

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
                        PublishingHouse = model.PublishingHouse,
                        ImageId = image.Id,
                        IsAvailable = true
                    };
                    await _libraryUnitService.AddAsync(newUnit);
                    TempData["success"] = "Успешно добавена библиотечна единица";
                    return RedirectToAction("AllLibraryUnits");
                }
            }
            else
            {
                if(unit != null && unitIsbn != null)
                    TempData["error"] = "Вече съществува библиотечна единица с този инвентарен номер и с този ISBN";
                else if (unit != null && unitIsbn == null)
                TempData["error"] = "Вече съществува библиотечна единица с този инвентарен номер";

                else if(unitIsbn != null && unit == null)
                    TempData["error"] = "Вече съществува библиотечна единица с този ISBN";

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
                Image = await _imageService.GetByIdAsync(unit.ImageId)
            };

            if (unit.Isbn == null) model.Isbn = null;
            else model.Isbn = unit.Isbn;


            if (unit.Year == null) model.Year = null;
            else model.Year = unit.Year;


            if (unit.PublishingHouse == null) model.PublishingHouse = null;
            else model.PublishingHouse = unit.PublishingHouse;


            return View(model);
        }

        public async Task<IActionResult> UpdateLibraryUnit(int id)
        {
            LibraryUnit unit = await _libraryUnitService.GetByIdAsync(id);
            Title title = _titleService.GetWhere(x => x.Id == unit.TitleId).FirstOrDefault();

            IEnumerable<Title> allTitles = await _titleService.GetAllAsync();

            LibraryUnitUpdateViewModel model = new LibraryUnitUpdateViewModel
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
                Image = await _imageService.GetByIdAsync(unit.ImageId)
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
        public async Task<IActionResult> UpdateLibraryUnit(LibraryUnitUpdateViewModel model)
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
                    model.Image = await _imageService.GetByIdAsync(unit.ImageId);

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
                    
                    if(model.UploadedImage != null)
                    {
                        var ImageLink = await _cloudinaryService.UploadImageAsync(model.UploadedImage);
                        //await _imageService.AddAsync(new Image { DestinationLink = ImageLink });
                        Image image = await _imageService.GetByIdAsync(unit.ImageId);
                        //to upload the pic successfully
                        image.DestinationLink = ImageLink;
                        await _imageService.UpdateAsync(image);
                    }
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

        [HttpPost]
        public async Task<IActionResult> Scrape(LibraryUnitViewModel model)
        {
            LibraryUnit unit = await _libraryUnitService.GetByIdAsync (model.Id);
            if(unit.IsAvailable == false)
            {
                TempData["error"] = "Единицата не може да бъде бракувана, защото е заета от читател!";
                return RedirectToAction("AllLibraryUnits");
            }

            unit.IsScrapped = true;
            unit.IsAvailable = false;
            await _libraryUnitService.UpdateAsync (unit);
            var identityUser = await _userManager.GetUserAsync(User);
            string identityUserId = identityUser.Id;
            User user = _userService.GetWhere(x => x.IdentityUserId == identityUserId).First();
            await _scrappedUnitService.AddAsync(new ScrappedUnit { LibrarianId = user.Id, DateTimeOfScrapping = DateTime.Now, LibraryUnitId = unit.Id });
            TempData["success"] = "Успешно бракувана единица";
            return RedirectToAction("AllLibraryUnits");
        }
    }
}