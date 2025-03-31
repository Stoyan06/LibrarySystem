using CloudinaryDotNet.Actions;
using LibrarySystem.Data;
using LibrarySystem.Models;
using LibrarySystem.Services.IService;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Services
{
    public class SearchService:ISearchService
    {
        private readonly ApplicationDbContext _context;

        public SearchService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<object>> GetSuggestions(string query)
        {
            query = query.Trim().ToLower();

            var results = await _context.LibraryUnits
                .Include(b => b.Title).ThenInclude(t => t.TitleAuthors).ThenInclude(ta => ta.Author)
                .Include(b => b.Image)
                .Include(b => b.Title.Section)
                .Where(b => !b.IsScrapped &&
                            (EF.Functions.Like(b.InventoryNumber.ToLower(), $"%{query}%") ||
                             EF.Functions.Like(b.Isbn.ToLower(), $"%{query}%") ||
                             EF.Functions.Like(b.Title.Name.ToLower(), $"%{query}%") ||
                             EF.Functions.Like(b.Title.Description.ToLower(), $"%{query}%") ||
                             EF.Functions.Like(b.Title.Section.Name.ToLower(), $"%{query}%") ||
                             EF.Functions.Like(b.PublishingHouse.ToLower(), $"%{query}%") ||
                             b.Year.ToString().Contains(query) ||
                             b.Title.TitleAuthors.Any(ta => EF.Functions.Like(ta.Author.FullName.ToLower(), $"%{query}%"))))
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
                .ToListAsync();

            return results;
        }

        public async Task<List<SearchResults>> GetResults(string query)
        {
            query = query.Trim().ToLower();

            var results = await _context.LibraryUnits
                .Include(b => b.Title).ThenInclude(t => t.TitleAuthors).ThenInclude(ta => ta.Author)
                .Include(b => b.Image)
                .Include(b => b.Title.Section)
                .Where(b => !b.IsScrapped &&
                            (EF.Functions.Like(b.InventoryNumber.ToLower(), $"%{query}%") ||
                             EF.Functions.Like(b.Isbn.ToLower(), $"%{query}%") ||
                             EF.Functions.Like(b.Title.Name.ToLower(), $"%{query}%") ||
                             EF.Functions.Like(b.Title.Description.ToLower(), $"%{query}%") ||
                             EF.Functions.Like(b.Title.Section.Name.ToLower(), $"%{query}%") ||
                             EF.Functions.Like(b.PublishingHouse.ToLower(), $"%{query}%") ||
                             b.Year.ToString().Contains(query) ||
                             b.Title.TitleAuthors.Any(ta => EF.Functions.Like(ta.Author.FullName.ToLower(), $"%{query}%"))))
                .Select(b => new SearchResults
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
                .ToListAsync();

            return results;
        }

        public async Task<List<SearchResults>> ExtendedSearch(ExtendedSearchViewModel model)
        {
            var query = _context.LibraryUnits
                .Include(b => b.Title).ThenInclude(t => t.TitleAuthors).ThenInclude(ta => ta.Author)
                .Include(b => b.Image)
                .Include(b => b.Title.Section)
                .Where(b => !b.IsScrapped);

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

            var results = await query.Select(b => new SearchResults
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
            }).ToListAsync();

            return results;
        }
    }
}