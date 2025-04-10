using LibrarySystem.Data;
using LibrarySystem.Models;
using LibrarySystem.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibrarySystem.Tests
{
    [TestFixture]
    public class SearchServiceTests
    {
        private ApplicationDbContext _context;
        private SearchService _searchService;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "LibrarySystem_TestDb")
                .Options;

            _context = new ApplicationDbContext(options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            SeedTestData();

            _searchService = new SearchService(_context);
        }

        private void SeedTestData()
        {
            var section = new Section { Id = 1, Name = "Science", Description = "Science books" };
            var author = new Author { Id = 1, FullName = "Isaac Newton" };
            var title = new Title { Id = 1, Name = "Physics Fundamentals", Description = "Basic physics", Section = section };
            var titleAuthor = new TitleAuthor { TitleId = title.Id, AuthorId = author.Id, Title = title, Author = author };
            var image = new Image { Id = 1, DestinationLink = "http://example.com/image.jpg" };

            var libraryUnit = new LibraryUnit
            {
                Id = 1,
                Title = title,
                TitleId = title.Id,
                TypeLibraryUnit = "Book",
                Condition = "New",
                Medium = "Hardcover",
                PublishingHouse = "Oxford",
                Isbn = "9781234567890",
                InventoryNumber = "INV123",
                Year = 2020,
                Image = image,
                ImageId = image.Id,
                IsScrapped = false
            };

            _context.Sections.Add(section);
            _context.Authors.Add(author);
            _context.Titles.Add(title);
            _context.TitlesAuthors.Add(titleAuthor);
            _context.Images.Add(image);
            _context.LibraryUnits.Add(libraryUnit);

            _context.SaveChanges();
        }

        [Test]
        public async Task GetSuggestions_ReturnsCorrectResults()
        {
            var result = await _searchService.GetSuggestions("Physics");

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Any());
        }

        [Test]
        public async Task GetResults_ReturnsCorrectSearchResults()
        {
            var result = await _searchService.GetResults("Physics");

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Any());
            Assert.AreEqual("Physics Fundamentals", result.First().Title);
        }

        [Test]
        public async Task ExtendedSearch_ReturnsFilteredResults()
        {
            var model = new ExtendedSearchViewModel
            {
                Title = "Physics",
                Section = "Science",
                Author = "Isaac Newton",
                Medium = "Hardcover",
                Condition = "New",
                PublishingHouse = "Oxford"
            };

            var result = await _searchService.ExtendedSearch(model);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Any());
            Assert.AreEqual("Physics Fundamentals", result.First().Title);
        }
        [TearDown]
        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
