using LibrarySystem.Data;
using LibrarySystem.Tests;
using Microsoft.EntityFrameworkCore;

public class RepositoryTestDbContext : ApplicationDbContext
{
    public RepositoryTestDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<TestEntity> TestEntities { get; set; }
}