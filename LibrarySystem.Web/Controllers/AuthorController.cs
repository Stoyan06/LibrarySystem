using LibrarySystem.Models;
using LibrarySystem.Services.IService;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.Web.Controllers
{
    public class AuthorController:Controller
    {
        private IService<Author> _authorService;

        public AuthorController(IService<Author> authorService)
        {
            _authorService = authorService;
        }

        public async Task<IActionResult> AllAuthors()
        {
            return View(await _authorService.GetAllAsync());
        }

        public IActionResult AddAuthor()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddAuthor(Author author)
        {
            if(author.FullName == null || author.FullName == string.Empty)
                return View(author);
            await _authorService.AddAsync(author);
            return RedirectToAction("AllAuthors");
        }

        public async Task<IActionResult> UpdateAuthor(int id)
        {
            return View(await _authorService.GetByIdAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAuthor(Author author)
        {
            if (author.FullName == null || author.FullName == string.Empty)
                return View(author);
            await _authorService.UpdateAsync(author);
            return RedirectToAction("AllAuthors");
        }

        public async Task<IActionResult> RemoveAuthor(int id)
        {
            await _authorService.DeleteAsync(id);
            return RedirectToAction("AllAuthors");
        }
    }
}
