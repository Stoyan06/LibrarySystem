using LibrarySystem.Data;
using LibrarySystem.Data.Repository;
using LibrarySystem.Models;
using LibrarySystem.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibrarySystem.Tests
{
    public class SectionServiceTests : IDisposable
    {
        private ApplicationDbContext _context;
        private IRepository<Section> _repository;
        private SectionService _sectionService;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _context.Database.EnsureCreated();

            _repository = new Repository<Section>(_context);
            _sectionService = new SectionService(_repository);

            SeedTestData();
        }

        private void SeedTestData()
        {
            var section1 = new Section { Id = 1, Name = "Science", Description = "Science Books" };
            var section2 = new Section { Id = 2, Name = "Literature", Description = "Literature Books" };

            _context.Sections.AddRange(section1, section2);
            _context.SaveChanges();
        }

        [Test]
        public async Task GetAllAsync_ReturnsAllSections()
        {
            var result = await _sectionService.GetAllAsync();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task GetByIdAsync_ReturnsCorrectSection()
        {
            var result = await _sectionService.GetByIdAsync(1);
            Assert.IsNotNull(result);
            Assert.AreEqual("Science", result.Name);
        }

        [Test]
        public async Task AddAsync_AddsNewSection()
        {
            var newSection = new Section { Id = 3, Name = "Technology", Description = "Tech Books" };
            await _sectionService.AddAsync(newSection);

            var allSections = await _sectionService.GetAllAsync();
            Assert.AreEqual(3, allSections.Count());
        }

        [Test]
        public async Task UpdateAsync_UpdatesExistingSection()
        {
            var section = await _sectionService.GetByIdAsync(1);
            section.Description = "Updated Description";
            await _sectionService.UpdateAsync(section);

            var updated = await _sectionService.GetByIdAsync(1);
            Assert.AreEqual("Updated Description", updated.Description);
        }

        [Test]
        public async Task DeleteAsync_DeletesSection()
        {
            await _sectionService.DeleteAsync(2);
            var allSections = await _sectionService.GetAllAsync();
            Assert.AreEqual(1, allSections.Count());
        }

        [Test]
        public async Task ExisisSection_ReturnsTrueIfExists()
        {
            var exists = await _sectionService.ExisisSection("Science");
            Assert.IsTrue(exists);
        }

        [Test]
        public async Task ExisisSection_ReturnsFalseIfNotExists()
        {
            var exists = await _sectionService.ExisisSection("Math");
            Assert.IsFalse(exists);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
