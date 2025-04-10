using LibrarySystem.Data;
using LibrarySystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Tests.TestDbContexts
{
    public class AuthorServiceTestDbContext : ApplicationDbContext
    {
        public AuthorServiceTestDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<TestEntity> TestEntities { get; set; }
        public DbSet<Author> Authors { get; set; }
    }
}
