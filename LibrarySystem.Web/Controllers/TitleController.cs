using LibrarySystem.Models;
using LibrarySystem.Services.IService;
using LibrarySystem.Utility;
using LibrarySystem.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceStack;

namespace LibrarySystem.Web.Controllers
{
    public class TitleController:Controller
    {
        private IService<Title> _titleService;
        private IService<Section> _sectionService;
        private IService<Author> _authorService;
        private IService<TitleAuthor> _title_author_service;
        private IService<LibraryUnit> _libraryUnitService;

        public TitleController(IService<Title> titleService,
            IService<Section> sectionService,
            IService<Author> authorService,
            IService<TitleAuthor> titleAuthorService,
            IService<LibraryUnit> libraryUnitService)
        {
            _titleService = titleService;
            _sectionService = sectionService;
            _authorService = authorService;
            _title_author_service = titleAuthorService;
            _libraryUnitService = libraryUnitService;
        }

        public async Task<IActionResult> AllTitles(string SearchTerm)
        {
            ViewData["SearchTerm"] = SearchTerm;

            var titles = await _titleService.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                titles = titles
                    .Where(x => x.Name != null && x.Name.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)
                    || x.Description.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            return View(titles);
        }

        [HttpGet]
        public async Task<IActionResult> AddTitle()
        {
            IEnumerable<Section> sections = await _sectionService.GetAllAsync();
            if (sections.Count() == 0) return View("NoSectionsWarning");

            IEnumerable<Author> authors = await _authorService.GetAllAsync();
            if (authors.Count() == 0) return View("NoAuthorsWarning");
            var model = new TitleViewModel
            {
                Sections = _sectionService.GetAllAsync().Result.Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = e.Name
                }),

                Authors = _authorService.GetAllAsync().Result.Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = e.FullName
                })
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> AddTitle(TitleViewModel model)
        {
            if (model.Name != null && model.Description != null && model.SelectedAuthors.Count != 0)
            {
                IEnumerable<Title> titles = await _titleService.GetAllAsync();
                Title titleCheck = titles.Where(x => x.Name == model.Name).FirstOrDefault();

                if (titleCheck != null)
                {
                    model.Sections = _sectionService.GetAllAsync().Result.Select(e => new SelectListItem
                    {
                        Value = e.Id.ToString(),
                        Text = e.Name
                    });

                    model.Authors = _authorService.GetAllAsync().Result.Select(e => new SelectListItem
                    {
                        Value = e.Id.ToString(),
                        Text = e.FullName
                    });
                    TempData["error"] = "Вече има заглавие с това име.";
                    return View(model);
                }

                var title = new Title
                {
                    Name = model.Name,
                    Description = model.Description,
                    SectionId = model.SectionId
                };
                await _titleService.AddAsync(title);
                TempData["success"] = "Успешно добавено заглавие.";
                foreach(int authorId in model.SelectedAuthors)
                {
                    await _title_author_service.AddAsync(new TitleAuthor { AuthorId = authorId, TitleId = title.Id});
                }
                return RedirectToAction("AllTitles");
            }
            else
            {
                model.Sections = _sectionService.GetAllAsync().Result.Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = e.Name
                });

                model.Authors = _authorService.GetAllAsync().Result.Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = e.FullName
                });
                if(model.SelectedAuthors.Count == 0)
                {
                    TempData["error"] = "Изберете един или няколко автори.";
                }
                return View(model);
            } 
        }

        public async Task<IActionResult> UpdateTitle(int id)
        {
            var entity = await _titleService.GetByIdAsync(id);
            IEnumerable<Author> allAuthors = await _authorService.GetAllAsync();
            IEnumerable<TitleAuthor> titleAuthors = await _title_author_service.GetAllAsync();
            List<TitleAuthor> titleAut = titleAuthors.Where(x => x.TitleId == id).ToList();
            List<Author> newAuthors = new List<Author>();
            foreach(TitleAuthor title_author in titleAut)
            {
                newAuthors.Add(allAuthors.Where(x => x.Id == title_author.AuthorId).FirstOrDefault());
            }
            List<int> authorIds = new List<int>();
            foreach(Author author in newAuthors)
            {
                authorIds.Add(author.Id);
            }

            var model = new TitleViewModel
            {
                Sections = _sectionService.GetAllAsync().Result.Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = e.Name
                }),

                Authors = _authorService.GetAllAsync().Result.Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = e.FullName
                }),
                Name = entity.Name,
                Description = entity.Description,
                SectionId = entity.SectionId,
                SelectedAuthors = authorIds
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTitle(TitleViewModel model)
        {
            if (model.Name != null && model.Description != null && model.SelectedAuthors.Count != 0)
            {
                IEnumerable<Title> titles = await _titleService.GetAllAsync();
                Title titleCheck = titles.Where(x => x.Name == model.Name).FirstOrDefault();
                if (titleCheck != null && titleCheck.Id == model.Id) titleCheck = null;

                if (titleCheck != null)
                {
                    model.Sections = _sectionService.GetAllAsync().Result.Select(e => new SelectListItem
                    {
                        Value = e.Id.ToString(),
                        Text = e.Name
                    });

                    model.Authors = _authorService.GetAllAsync().Result.Select(e => new SelectListItem
                    {
                        Value = e.Id.ToString(),
                        Text = e.FullName
                    });
                    TempData["error"] = "Вече има заглавие с това име.";
                    return View(model);
                }

                var title = await _titleService.GetByIdAsync(model.Id);
                title.Name = model.Name;
                title.Description = model.Description;
                title.SectionId = model.SectionId;
 
                await _titleService.UpdateAsync(title);
                TempData["success"] = "Успешно редактирано заглавие.";

                await _title_author_service.DeleteWhere(x => x.TitleId == title.Id);

                foreach (int authorId in model.SelectedAuthors)
                {
                    await _title_author_service.AddAsync(new TitleAuthor { AuthorId = authorId, TitleId = title.Id });
                }
                return RedirectToAction("AllTitles");
            }
            else
            {
                model.Sections = _sectionService.GetAllAsync().Result.Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = e.Name
                });

                model.Authors = _authorService.GetAllAsync().Result.Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = e.FullName
                });
                if (model.SelectedAuthors.Count == 0)
                {
                    TempData["error"] = "Изберете един или няколко автори.";
                }
                return View(model);
            }
        }

        public async Task<IActionResult> RemoveTitle(int id)
        {
            return View("ConfirmDelete", await _titleService.GetByIdAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveTitle(Title title)
        {
            LibraryUnit unit =  _libraryUnitService.GetWhere(x => x.TitleId == title.Id).FirstOrDefault();

            if(unit != null)
            {
                TempData["error"] = "Заглавието не може да бъде изтрито, защото към него има обвързани библиотечни единици.";
                return RedirectToAction("AllTitles");
            }

            await _title_author_service.DeleteWhere(x => x.TitleId == title.Id);
            await _titleService.DeleteAsync(title.Id);
            TempData["success"] = "Заглавието е премахнато успешно.";
            return RedirectToAction("AllTitles");
        }

        public async Task<IActionResult> Details(int id)
        {
            Title title = await _titleService.GetByIdAsync(id);
            Section section = await _sectionService.GetByIdAsync(title.SectionId);

            IEnumerable<TitleAuthor> titleAuthors = await _title_author_service.GetAllAsync();
            IEnumerable<TitleAuthor> newTitleAuthors = titleAuthors.Where(x => x.TitleId == id);

            List<Author> authors = new List<Author>();
            List<string> authorsNames = new List<string>();

            foreach (TitleAuthor titleAuthor in newTitleAuthors)
            {
                authors.Add(await _authorService.GetByIdAsync(titleAuthor.AuthorId));
            }

            foreach (Author author in authors)
            {
                authorsNames.Add(author.FullName);
            }

            TitleViewModel model = new TitleViewModel
            {
                Description = title.Description,
                Name = title.Name,
                SectionName = section.Name,
                SelectedAuthorsNames = authorsNames
            };

            return View(model);
        }
        public async Task<IActionResult> GetBySection(string? section)
        {
            if(section == string.Empty || section == null)
            {
                return RedirectToAction("AllTitles");
            }
            else
            {
                IEnumerable<Section> sections = await _sectionService.GetAllAsync();
                var findSection = sections.FirstOrDefault(x => x.Name == section);
                if(section == null)
                {
                    return View("AllTitles", new List<Title>().AsEnumerable());
                }
                else
                {
                    IEnumerable<Title> titles = await _titleService.GetAllAsync();
                    if (findSection == null)
                        titles = null;
                    else
                        titles = titles.Where(x => x.SectionId == findSection.Id);
                    return View("AllTitles", titles);
                }
            }
        }
    }
}