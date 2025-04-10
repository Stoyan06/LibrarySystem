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
    public class ServiceTests : IDisposable
    {
        private ApplicationDbContext _context;
        private Repository<Section> _repository;
        private Service<Section> _service;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _context.Database.EnsureCreated();

            _repository = new Repository<Section>(_context);
            _service = new Service<Section>(_repository);

            SeedTestData();
        }

        private void SeedTestData()
        {
            _context.Sections.AddRange(
                new Section { Id = 1, Name = "History", Description = "History books" },
                new Section { Id = 2, Name = "Mathematics", Description = "Math books" }
            );
            _context.SaveChanges();
        }

        [Test]
        public async Task GetAllAsync_ReturnsAllItems()
        {
            var result = await _service.GetAllAsync();
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task GetByIdAsync_ReturnsCorrectItem()
        {
            var result = await _service.GetByIdAsync(1);
            Assert.IsNotNull(result);
            Assert.AreEqual("History", result.Name);
        }

        [Test]
        public async Task AddAsync_AddsNewItem()
        {
            var newSection = new Section { Id = 3, Name = "Biology", Description = "Biology books" };
            await _service.AddAsync(newSection);

            var result = await _service.GetAllAsync();
            Assert.AreEqual(3, result.Count());
        }

        [Test]
        public async Task UpdateAsync_UpdatesExistingItem()
        {
            var section = await _service.GetByIdAsync(2);
            section.Description = "Updated description";
            await _service.UpdateAsync(section);

            var updated = await _service.GetByIdAsync(2);
            Assert.AreEqual("Updated description", updated.Description);
        }

        [Test]
        public async Task DeleteAsync_DeletesItem()
        {
            await _service.DeleteAsync(1);

            var result = await _service.GetAllAsync();
            Assert.AreEqual(1, result.Count());
        }

        [Test]
        public async Task DeleteWhere_DeletesMatchingItems()
        {
            await _service.DeleteWhere(s => s.Name == "Mathematics");

            var result = await _service.GetAllAsync();
            Assert.AreEqual(1, result.Count());
        }

        [Test]
        public void GetWhere_ReturnsMatchingItems()
        {
            var result = _service.GetWhere(s => s.Name.Contains("Math"));
            Assert.AreEqual(1, result.Count());
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
