using LibrarySystem.Models;
using LibrarySystem.Services.IService;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.Web.Controllers
{
    public class AuthorController:Controller
    {
        private IAuthorService _authorService;

        public AuthorController(IAuthorService authorService)
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
            if(await _authorService.ExistsAuthor(author.FullName) == false)
            {
                await _authorService.AddAsync(author);
                TempData["success"] = "Успешно добавен автор";
                return RedirectToAction("AllAuthors");
            }
            else
            {
                TempData["error"] = "Вече съществува автор с това име";
                return RedirectToAction("AddAuthor");
            }
        }

        public async Task<IActionResult> UpdateAuthor(int id)
        {
            return View(await _authorService.GetByIdAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAuthor(Author author)
        {
            Author getAuthor = await _authorService.GetByIdAsync(author.Id);

            if (author.FullName == null || author.FullName == string.Empty)
                return View(author);
            if(await _authorService.ExistsAuthor(author.FullName) == false || getAuthor.FullName == author.FullName)
            {
                await _authorService.UpdateAsync(author);
                TempData["success"] = "Успешно редактиран автор";
                return RedirectToAction("AllAuthors");
            }
            else
            {
                TempData["error"] = "Вече съществува автор с тези имена";
                return RedirectToAction("UpdateAuthor", author.Id);
            }
        }

        public async Task<IActionResult> RemoveAuthor(int id)
        {
            Author aut = await _authorService.GetByIdAsync(id);
            return View("ConfirmDelete",aut);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveAuthor(Author author)
        {
            try
            {
                await _authorService.DeleteAsync(author.Id);
                TempData["success"] = "Успешно премахнат автор";
                return RedirectToAction("AllAuthors");
            }
            catch(Exception ex)
            {
                TempData["error"] = "Има обвързани заглавия с този автор";
                return RedirectToAction("AllAuthors");
            }
        }
    }
}
