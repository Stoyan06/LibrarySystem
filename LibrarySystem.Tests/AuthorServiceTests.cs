using LibrarySystem.Data;
using LibrarySystem.Data.Repository;
using LibrarySystem.Models;
using LibrarySystem.Services;
using LibrarySystem.Tests.TestDbContexts;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LibrarySystem.Tests
{
    public class AuthorServiceTests
    {
        private AuthorServiceTestDbContext _context;
        private AuthorService _service;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Unique DB per test
                .Options;

            _context = new AuthorServiceTestDbContext(options);
            var repository = new Repository<Author>(_context);
            _service = new AuthorService(repository);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task AddAsync_ShouldAddAuthor()
        {
            var author = new Author { FullName = "J.K. Rowling" };

            await _service.AddAsync(author);

            Assert.AreEqual(1, _context.Authors.Count());
            Assert.AreEqual("J.K. Rowling", _context.Authors.First().FullName);
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnAuthor()
        {
            var author = new Author { FullName = "Stephen King" };
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            var result = await _service.GetByIdAsync(author.Id);

            Assert.NotNull(result);
            Assert.AreEqual("Stephen King", result.FullName);
        }

        [Test]
        public async Task ExistsAuthor_ShouldReturnTrue_IfExists()
        {
            var author = new Author { FullName = "George Orwell" };
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            var exists = await _service.ExistsAuthor("George Orwell");

            Assert.IsTrue(exists);
        }

        [Test]
        public async Task ExistsAuthor_ShouldReturnFalse_IfNotExists()
        {
            var exists = await _service.ExistsAuthor("Unknown Author");

            Assert.IsFalse(exists);
        }

        [Test]
        public async Task UpdateAsync_ShouldUpdateAuthor()
        {
            var author = new Author { FullName = "Old Name" };
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            author.FullName = "New Name";
            await _service.UpdateAsync(author);

            var updated = await _context.Authors.FindAsync(author.Id);
            Assert.AreEqual("New Name", updated.FullName);
        }

        [Test]
        public async Task DeleteAsync_ShouldRemoveAuthor()
        {
            var author = new Author { FullName = "To Be Deleted" };
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            await _service.DeleteAsync(author.Id);

            var deleted = await _context.Authors.FindAsync(author.Id);
            Assert.IsNull(deleted);
        }

        [Test]
        public async Task DeleteWhere_ShouldRemoveAuthorsMatchingPredicate()
        {
            _context.Authors.AddRange(
                new Author { FullName = "Delete Me" },
                new Author { FullName = "Keep Me" }
            );
            await _context.SaveChangesAsync();

            await _service.DeleteWhere(a => a.FullName.Contains("Delete"));

            Assert.AreEqual(1, _context.Authors.Count());
            Assert.AreEqual("Keep Me", _context.Authors.First().FullName);
        }

        [Test]
        public void GetWhere_ShouldReturnMatchingAuthors()
        {
            _context.Authors.AddRange(
                new Author { FullName = "Author One" },
                new Author { FullName = "Author Two" }
            );
            _context.SaveChanges();

            var results = _service.GetWhere(a => a.FullName.Contains("Two"));

            Assert.AreEqual(1, results.Count());
            Assert.AreEqual("Author Two", results.First().FullName);
        }
    }
}
