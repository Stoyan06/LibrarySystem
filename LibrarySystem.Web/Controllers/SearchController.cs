using LibrarySystem.Data;
using LibrarySystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

public class SearchController : Controller
{
    private readonly ApplicationDbContext _context;

    public SearchController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public JsonResult GetSuggestions(string query)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Length < 3)
        {
            return Json(new { message = "Query too short" });
        }

        query = query.Trim().ToLower();

        var results = _context.LibraryUnits
            .Include(b => b.Title).ThenInclude(t => t.TitleAuthors).ThenInclude(ta => ta.Author)
            .Include(b => b.Image)
            .Where(b => !b.IsScrapped && (
                        EF.Functions.Like(b.InventoryNumber.ToLower(), $"%{query}%") ||
                        EF.Functions.Like(b.Isbn.ToLower(), $"%{query}%") ||
                        EF.Functions.Like(b.Title.Name.ToLower(), $"%{query}%") ||
                        EF.Functions.Like(b.Title.Description.ToLower(), $"%{query}%") ||
                        EF.Functions.Like(b.Title.Section.Name.ToLower(), $"%{query}%") ||
                        EF.Functions.Like(b.PublishingHouse.ToLower(), $"%{query}%") ||
                        b.Year.ToString().Contains(query) ||
                        b.Title.TitleAuthors.Any(ta => EF.Functions.Like(ta.Author.FullName.ToLower(), $"%{query}%"))
                    ))
            .Select(b => new
            {
                id = b.Id,
                title = b.Title.Name,
                author = string.Join(", ", b.Title.TitleAuthors.Select(ta => ta.Author.FullName)),
                inventoryNumber = b.InventoryNumber,
                isbn = b.Isbn,
                image = b.Image != null ? b.Image.DestinationLink : null
            })
            .Take(5)
            .ToList();

        return Json(results);
    }

    public IActionResult Results(string query)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Length < 3)
        {
            return View(new List<SearchResult>());
        }

        query = query.Trim().ToLower();

        var results = _context.LibraryUnits
            .Include(b => b.Title).ThenInclude(t => t.TitleAuthors).ThenInclude(ta => ta.Author)
            .Include(b => b.Title.Section)
            .Include(b => b.Image)
            .Where(b => !b.IsScrapped && (
                        EF.Functions.Like(b.InventoryNumber.ToLower(), $"%{query}%") ||
                        EF.Functions.Like(b.Isbn.ToLower(), $"%{query}%") ||
                        EF.Functions.Like(b.Title.Name.ToLower(), $"%{query}%") ||
                        EF.Functions.Like(b.Title.Description.ToLower(), $"%{query}%") ||
                        EF.Functions.Like(b.Title.Section.Name.ToLower(), $"%{query}%") ||
                        EF.Functions.Like(b.PublishingHouse.ToLower(), $"%{query}%") ||
                        b.Year.ToString().Contains(query) ||
                        b.Title.TitleAuthors.Any(ta => EF.Functions.Like(ta.Author.FullName.ToLower(), $"%{query}%"))
                    ))
            .Select(b => new SearchResult
            {
                Id = b.Id,
                Title = b.Title.Name,
                Author = string.Join(", ", b.Title.TitleAuthors.Select(ta => ta.Author.FullName)),
                Section = b.Title.Section.Name,
                Year = b.Year,
                InventoryNumber = b.InventoryNumber,
                Isbn = b.Isbn,
                ImageUrl = b.Image != null ? b.Image.DestinationLink : null,
                Description = b.Title.Description,
                PublishingHouse = b.PublishingHouse
            })
            .ToList();

        return View(results);
    }

    [HttpGet]
    public IActionResult ExtendedSearch()
    {
        ViewBag.Sections = _context.Sections.Select(s => s.Name).ToList();
        return View();
    }

    [HttpPost]
    public IActionResult ExtendedSearch(ExtendedSearchViewModel model)
    {
        var query = _context.LibraryUnits
            .Include(b => b.Title).ThenInclude(t => t.TitleAuthors).ThenInclude(ta => ta.Author)
            .Include(b => b.Title.Section)
            .Include(b => b.Image)
            .Where(b => !b.IsScrapped) // ✅ Only non-scrapped
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(model.Section))
            query = query.Where(b => b.Title.Section.Name == model.Section);

        if (!string.IsNullOrWhiteSpace(model.Title))
            query = query.Where(b => EF.Functions.Like(b.Title.Name.ToLower(), $"%{model.Title.ToLower()}%"));

        if (!string.IsNullOrWhiteSpace(model.Author))
            query = query.Where(b => b.Title.TitleAuthors.Any(ta => EF.Functions.Like(ta.Author.FullName.ToLower(), $"%{model.Author.ToLower()}%")));

        if (!string.IsNullOrWhiteSpace(model.Condition))
            query = query.Where(b => EF.Functions.Like(b.Condition.ToLower(), $"%{model.Condition.ToLower()}%"));

        if (!string.IsNullOrWhiteSpace(model.Medium))
            query = query.Where(b => EF.Functions.Like(b.Medium.ToLower(), $"%{model.Medium.ToLower()}%"));

        if (!string.IsNullOrWhiteSpace(model.PublishingHouse))
            query = query.Where(b => EF.Functions.Like(b.PublishingHouse.ToLower(), $"%{model.PublishingHouse.ToLower()}%"));

        var results = query.Select(b => new SearchResult
        {
            Id = b.Id,
            Title = b.Title.Name,
            Author = string.Join(", ", b.Title.TitleAuthors.Select(ta => ta.Author.FullName)),
            Section = b.Title.Section.Name,
            Year = b.Year,
            InventoryNumber = b.InventoryNumber,
            Isbn = b.Isbn,
            ImageUrl = b.Image != null ? b.Image.DestinationLink : null,
            Description = b.Title.Description,
            PublishingHouse = b.PublishingHouse
        }).ToList();

        return View("Results", results);
    }

    // ✅ NEW: Extended Search for Scrapped Units
    [HttpGet]
    public IActionResult ExtendedSearchScrapped()
    {
        ViewBag.Sections = _context.Sections.Select(s => s.Name).ToList();
        return View("ExtendedSearchScrapped");
    }

    [HttpPost]
    public IActionResult ExtendedSearchScrapped(ExtendedSearchViewModel model)
    {
        var query = _context.LibraryUnits
            .Include(b => b.Title).ThenInclude(t => t.TitleAuthors).ThenInclude(ta => ta.Author)
            .Include(b => b.Title.Section)
            .Include(b => b.Image)
            .Where(b => b.IsScrapped) // ✅ Only scrapped units
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(model.Section))
            query = query.Where(b => b.Title.Section.Name == model.Section);

        if (!string.IsNullOrWhiteSpace(model.Title))
            query = query.Where(b => EF.Functions.Like(b.Title.Name.ToLower(), $"%{model.Title.ToLower()}%"));

        if (!string.IsNullOrWhiteSpace(model.Author))
            query = query.Where(b => b.Title.TitleAuthors.Any(ta => EF.Functions.Like(ta.Author.FullName.ToLower(), $"%{model.Author.ToLower()}%")));

        if (!string.IsNullOrWhiteSpace(model.Condition))
            query = query.Where(b => EF.Functions.Like(b.Condition.ToLower(), $"%{model.Condition.ToLower()}%"));

        if (!string.IsNullOrWhiteSpace(model.Medium))
            query = query.Where(b => EF.Functions.Like(b.Medium.ToLower(), $"%{model.Medium.ToLower()}%"));

        if (!string.IsNullOrWhiteSpace(model.PublishingHouse))
            query = query.Where(b => EF.Functions.Like(b.PublishingHouse.ToLower(), $"%{model.PublishingHouse.ToLower()}%"));

        var results = query.Select(b => new SearchResult
        {
            Id = b.Id,
            Title = b.Title.Name,
            Author = string.Join(", ", b.Title.TitleAuthors.Select(ta => ta.Author.FullName)),
            Section = b.Title.Section.Name,
            Year = b.Year,
            InventoryNumber = b.InventoryNumber,
            Isbn = b.Isbn,
            ImageUrl = b.Image != null ? b.Image.DestinationLink : null,
            Description = b.Title.Description,
            PublishingHouse = b.PublishingHouse
        }).ToList();

        return View("Results", results);
    }
}
