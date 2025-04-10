namespace LibrarySystem.Tests
{
    using NUnit.Framework;
    using Microsoft.EntityFrameworkCore;
    using System.Threading.Tasks;
    using LibrarySystem.Data;
    using LibrarySystem.Data.Repository;
    using System.Collections.Generic;
    using System.Linq;
    public class RepositoryTests
    {
        private RepositoryTestDbContext _context;
        private Repository<TestEntity> _repository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new RepositoryTestDbContext(options);
            _repository = new Repository<TestEntity>(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task AddAsync_ShouldAddEntity()
        {
            var entity = new TestEntity { Id = 1, Name = "Test" };

            await _repository.AddAsync(entity);

            var result = await _repository.GetByIdAsync(1);
            Assert.IsNotNull(result);
            Assert.AreEqual("Test", result.Name);
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnAllEntities()
        {
            await _repository.AddAsync(new TestEntity { Id = 1, Name = "A" });
            await _repository.AddAsync(new TestEntity { Id = 2, Name = "B" });

            var result = await _repository.GetAllAsync();

            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task DeleteAsync_ShouldRemoveEntity()
        {
            var entity = new TestEntity { Id = 1, Name = "ToDelete" };
            await _repository.AddAsync(entity);

            await _repository.DeleteAsync(1);

            var result = await _repository.GetByIdAsync(1);
            Assert.IsNull(result);
        }

        [Test]
        public void GetWhere_ShouldReturnMatchingEntities()
        {
            _context.AddRange(
                new TestEntity { Id = 1, Name = "Match" },
                new TestEntity { Id = 2, Name = "NoMatch" }
            );
            _context.SaveChanges();

            var result = _repository.GetWhere(x => x.Name == "Match");

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("Match", result.First().Name);
        }

        [Test]
        public async Task DeleteWhere_ShouldDeleteMatchingEntities()
        {
            _context.AddRange(
                new TestEntity { Id = 1, Name = "Delete" },
                new TestEntity { Id = 2, Name = "Keep" }
            );
            _context.SaveChanges();

            await _repository.DeleteWhere(x => x.Name == "Delete");

            var remaining = _repository.GetAll().ToList();
            Assert.AreEqual(1, remaining.Count);
            Assert.AreEqual("Keep", remaining[0].Name);
        }

        [Test]
        public async Task UpdateAsync_ShouldUpdateEntity()
        {
            var entity = new TestEntity { Name = "Original" };
            await _repository.AddAsync(entity);

            entity.Name = "Updated";
            await _repository.UpdateAsync(entity);

            var updatedEntity = await _repository.GetByIdAsync(entity.Id);

            Assert.NotNull(updatedEntity);
            Assert.AreEqual("Updated", updatedEntity.Name);
        }

    }

    public class TestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}