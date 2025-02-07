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

        public TitleController(IService<Title> titleService,
            IService<Section> sectionService,
            IService<Author> authorService,
            IService<TitleAuthor> titleAuthorService)
        {
            _titleService = titleService;
            _sectionService = sectionService;
            _authorService = authorService;
            _title_author_service = titleAuthorService;
        }

        public async Task<IActionResult> AllTitles()
        {
            return View(await _titleService.GetAllAsync());
        }

        [HttpGet]
        public IActionResult AddTitle()
        {
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
            var title = new Title
            {
                Name = model.Name,
                Description = model.Description,
                SectionId = model.SectionId
            };

            await _titleService.AddAsync(title);
            await _title_author_service.AddAsync(new TitleAuthor { AuthorId = model.AuthorId, TitleId = title.Id });
            return RedirectToAction("AllTitles");
        }

        public async Task<IActionResult> UpdateTitle(int id)
        {
            var entity = await _titleService.GetByIdAsync(id);

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
                SectionId = entity.SectionId
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTitle(TitleViewModel model)
        {
            var title = new Title
            {
                Name = model.Name,
                Description = model.Description,
                SectionId = model.SectionId
            };

            var entity = await _titleService.GetByIdAsync(model.Id);
            entity.Name = title.Name;
            entity.Description = title.Description;
            entity.SectionId = title.SectionId;
            await _titleService.UpdateAsync(entity);
            //await _title_author_service.AddAsync(new TitleAuthor { AuthorId = model.AuthorId, TitleId = title.Id });
            return RedirectToAction("AllTitles");
        }

        public async Task<IActionResult> RemoveTitle(int id)
        {
            await _title_author_service.DeleteWhere(x => x.TitleId == id);
            await _titleService.DeleteAsync(id);
            return RedirectToAction("AllTitles");
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
