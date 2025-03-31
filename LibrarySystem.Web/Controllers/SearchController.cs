using LibrarySystem.Data;
using LibrarySystem.Models;
using LibrarySystem.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

public class SearchController : Controller
{
    private readonly ISearchService _searchService;
    private readonly IService<Section> _sectionService;

    public SearchController(ISearchService searchService, IService<Section> sectionService)
    {
        _searchService = searchService;
        _sectionService = sectionService;
    }

    [HttpGet]
    public async Task<JsonResult> GetSuggestions(string query)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Length < 3)
        {
            return Json(new { message = "Query too short" });
        }

        query = query.Trim().ToLower();

        var results = await _searchService.GetSuggestions(query);

        return Json(results);
    }

    public async Task<IActionResult> Results(string query)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Length < 3)
        {
            return View(new List<SearchResults>());
        }

        query = query.Trim().ToLower();

        var results = await _searchService.GetResults(query);

        return View(results);
    }

    [HttpGet]
    public async Task<IActionResult> ExtendedSearch()
    {
        ViewBag.Sections = (await _sectionService.GetAllAsync()).Select(s => s.Name).ToList();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ExtendedSearch(ExtendedSearchViewModel model)
    {
        var results = await _searchService.ExtendedSearch(model);

        return View("Results", results);
    }
}