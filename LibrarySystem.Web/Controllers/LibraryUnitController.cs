using LibrarySystem.Models;
using LibrarySystem.Services;
using LibrarySystem.Services.IService;
using LibrarySystem.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize(Roles = $"{SD.AdminRole},{SD.LibrarianRole}")]
        public async Task<IActionResult> ScrappedLibraryUnits()
        {
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

        [Authorize(Roles = $"{SD.AdminRole},{SD.LibrarianRole}")]
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
        [Authorize(Roles = $"{SD.AdminRole},{SD.LibrarianRole}")]
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
                        IsAvailable = true,
                        IsSavedByUser = false
                    };
                    await _libraryUnitService.AddAsync(newUnit);
                    TempData["success"] = "Успешно добавена библиотечна единица.";
                    return RedirectToAction("AllLibraryUnits");
                }
            }
            else
            {
                if(unit != null && unitIsbn != null)
                    TempData["error"] = "Вече съществува библиотечна единица с този инвентарен номер и с този ISBN.";
                else if (unit != null && unitIsbn == null)
                TempData["error"] = "Вече съществува библиотечна единица с този инвентарен номер.";

                else if(unitIsbn != null && unit == null)
                    TempData["error"] = "Вече съществува библиотечна единица с този ISBN.";

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
            // Try to get the referer (previous page)
            var referer = Request.Headers["Referer"].ToString();

            // Store the referer in session only if it's not the details page itself
            if (!string.IsNullOrEmpty(referer) && !referer.Contains("Details"))
            {
                HttpContext.Session.SetString("PreviousUrl", referer);
            }

            LibraryUnit unit = await _libraryUnitService.GetByIdAsync(id);
            if(unit == null)
            {
                return View("~/Views/Shared/NotFound.cshtml");

            }
            Title title = _titleService.GetWhere(x => x.Id == unit.TitleId).FirstOrDefault();

            // Retrieve the previous URL from session, or fallback to the AllLibraryUnits page if not available
            var previousUrl = HttpContext.Session.GetString("PreviousUrl") ?? Url.Action("AllLibraryUnits");

            // Pass the previous URL to the view via ViewBag
            ViewBag.PreviousUrl = previousUrl;

            LibraryUnitViewModel model = new LibraryUnitViewModel
            {
                Id = unit.Id,
                InventoryNumber = unit.InventoryNumber,
                Condition = unit.Condition,
                Medium = unit.Medium,
                isScrapped = unit.IsScrapped,
                TitleId = unit.TitleId,
                TitleName = title.Name,
                TypeLibraryUnit = unit.TypeLibraryUnit,
                Image = await _imageService.GetByIdAsync(unit.ImageId),
                Isbn = unit.Isbn,
                Year = unit.Year,
                PublishingHouse = unit.PublishingHouse
            };

            return View(model);
        }





        [Authorize(Roles = $"{SD.AdminRole},{SD.LibrarianRole}")]
        public async Task<IActionResult> UpdateLibraryUnit(int id)
        {
            LibraryUnit unit = await _libraryUnitService.GetByIdAsync(id);
            if (unit == null)
            {
                return View("~/Views/Shared/NotFound.cshtml");

            }
            if (unit.IsScrapped)
            {
                return View("ScrappedWarning");
            }

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
                TypeLibraryUnit = unit.TypeLibraryUnit,
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
        [Authorize(Roles = $"{SD.AdminRole},{SD.LibrarianRole}")]
        public async Task<IActionResult> UpdateLibraryUnit(LibraryUnitUpdateViewModel model)
        {
           
            LibraryUnit unit = await _libraryUnitService.GetByIdAsync(model.Id);
            if (unit == null)
            {
                return View("~/Views/Shared/NotFound.cshtml");

            }
            if (unit.IsScrapped)
            {
                return View("ScrappedWarning");
            }
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
                        Image image = await _imageService.GetByIdAsync(unit.ImageId);
                        image.DestinationLink = ImageLink;
                        await _imageService.UpdateAsync(image);
                        unit.ImageId = image.Id;
                        unit.Image = image;
                    }
                    await _libraryUnitService.UpdateAsync(unit);
                    TempData["success"] = "Библиотечната единица е редактирана успешно.";
                    return RedirectToAction("AllLibraryUnits");
                }
            }
            else
            {
                TempData["error"] = "Вече съществува библиотечна единица с този инвентарен номер.";

                model.Titles = _titleService.GetAllAsync().Result.Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = e.Name
                });
                return View(model);
            }
        }

        [Authorize(Roles = $"{SD.AdminRole},{SD.LibrarianRole}")]
        public async Task<IActionResult> Scrape(int id)
        {

            LibraryUnit unit = await _libraryUnitService.GetByIdAsync(id);
            if (unit == null)
            {
                return View("~/Views/Shared/NotFound.cshtml");

            }
            if (unit.IsScrapped)
            {
                return View("ScrappedWarning");
            }
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
                TypeLibraryUnit = unit.TypeLibraryUnit,
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
        [Authorize(Roles = $"{SD.AdminRole},{SD.LibrarianRole}")]
        public async Task<IActionResult> Scrape(LibraryUnitViewModel model)
        {
            LibraryUnit unit = await _libraryUnitService.GetByIdAsync (model.Id);
            if (unit == null)
            {
                return View("~/Views/Shared/NotFound.cshtml");

            }
            if (unit.IsAvailable == false)
            {
                TempData["error"] = "Единицата не може да бъде бракувана, защото е заета от читател.";
                return RedirectToAction("AllLibraryUnits");
            }

            unit.IsScrapped = true;
            unit.IsAvailable = false;
            unit.IsSavedByUser = false;
            unit.SavedByReaderId = null;
            await _libraryUnitService.UpdateAsync (unit);
            var identityUser = await _userManager.GetUserAsync(User);
            string identityUserId = identityUser.Id;
            User user = _userService.GetWhere(x => x.IdentityUserId == identityUserId).First();
            await _scrappedUnitService.AddAsync(new ScrappedUnit { LibrarianId = user.Id, DateTimeOfScrapping = DateTime.Now, LibraryUnitId = unit.Id });
            TempData["success"] = "Единицата е бракувана успешно.";
            return RedirectToAction("AllLibraryUnits");
        }

        [Authorize(Roles = $"{SD.UserRole}")]
        public async Task<IActionResult> Reserve(int id)
        {
            var unit = await _libraryUnitService.GetByIdAsync(id);
            if (unit == null)
            {
                return View("~/Views/Shared/NotFound.cshtml");

            }
            var identityUser = await _userManager.GetUserAsync(User);
            var user = _userService.GetWhere(x => x.IdentityUserId == identityUser.Id).FirstOrDefault();

            string message;
            string status;

            if (unit.IsSavedByUser)
            {
                if (unit.SavedByReaderId == user.Id)
                    message = "Вече сте запазили тази единица.";
                else
                    message = "Тази единица вече е запазена от друг потребител.";

                status = "error";
            }
            else if (!unit.IsAvailable)
            {
                message = "Тази единица е заета от читател.";
                status = "error";
            }
            else
            {
                unit.IsSavedByUser = true;
                unit.SavedByReaderId = user.Id;
                await _libraryUnitService.UpdateAsync(unit);
                message = "Успешно запазихте единицата.";
                status = "success";
            }

            TempData[status] = message;

            // Redirect to same page using the Referer + #reserved anchor
            var referer = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrWhiteSpace(referer) && !referer.Contains("#reserved"))
                referer += "#reserved";

            return Redirect(referer);
        }

        [Authorize(Roles = $"{SD.UserRole}")]
        public async Task<IActionResult> SavedUnits()
        {
            // Get the current logged-in user
            var identityUser = await _userManager.GetUserAsync(User);
            var user = _userService.GetWhere(x => x.IdentityUserId == identityUser.Id).ToList().First();

            // Get the list of library units saved by the user
            var savedUnits = _libraryUnitService.GetWhere(x => x.SavedByReaderId == user.Id).ToList();

            List<SavedLibraryUnitViewModel> viewModel = new List<SavedLibraryUnitViewModel>();

            foreach (LibraryUnit unit in savedUnits)
            {
                Title title = await _titleService.GetByIdAsync(unit.TitleId);

                viewModel.Add(
                    new SavedLibraryUnitViewModel
                    {
                        InventoryNumber = unit.InventoryNumber,
                        IsAvailable = unit.IsAvailable,
                        Title = title.Name,
                        UnitId = unit.Id
                    });
            }

            return View(viewModel);
        }

        [Authorize(Roles = $"{SD.UserRole}")]
        public async Task<IActionResult> RemoveFromSaved(int id)
        {
            // Get the current logged-in user
            var identityUser = await _userManager.GetUserAsync(User);
            var user = _userService.GetWhere(x => x.IdentityUserId == identityUser.Id).First();

            // Find the saved unit and remove it
            var unit = await _libraryUnitService.GetByIdAsync(id);
            if (unit != null && unit.SavedByReaderId == user.Id)
            {
                unit.IsSavedByUser = false;
                unit.SavedByReaderId = null;
                await _libraryUnitService.UpdateAsync(unit);

                TempData["success"] = "Успешно премахнато от запазените единици.";
            }
            return RedirectToAction("SavedUnits");
        }
    }
}