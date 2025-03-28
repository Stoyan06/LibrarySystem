using LibrarySystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace LibrarySystem.Data
{
    public class ApplicationDbContext:IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Author> Authors { get; set; }

        public DbSet<Image> Images { get; set; }

        public DbSet<LibraryUnit> LibraryUnits { get; set; }

        public DbSet<MovementOfLibraryUnit> MovementsOfLibraryUnits { get;set; }

        public DbSet<ScrappedUnit> ScrappedUnits { get; set; }

        public DbSet<Section> Sections { get; set; }

        public DbSet<Title> Titles { get; set; }

        public DbSet<TitleAuthor> TitlesAuthors { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
        }
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Conventions.Remove(typeof(CascadeDeleteConvention));
            configurationBuilder.Conventions.Remove(typeof(SqlServerOnDeleteConvention));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TitleAuthor>().HasKey(x => new { x.AuthorId, x.TitleId });

            modelBuilder.Entity<Title>().HasMany(x => x.TitleAuthors).WithOne(x => x.Title);
            modelBuilder.Entity<Author>().HasMany(x => x.AuthorTitles).WithOne(x => x.Author);

            modelBuilder.Entity<MovementOfLibraryUnit>()
           .HasOne(x => x.Reader)
           .WithMany()
           .HasForeignKey(x => x.ReaderId)
           .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<MovementOfLibraryUnit>()
            .HasOne(x => x.Librarian)
            .WithMany()
            .HasForeignKey(x => x.LibrarianId)
            .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<MovementOfLibraryUnit>()
            .HasOne(x => x.LibraryUnit)
            .WithMany()
            .HasForeignKey(x => x.LibraryUnitId)
            .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Author>().ToTable("authors");
            modelBuilder.Entity<Image>().ToTable("images");
            modelBuilder.Entity<LibraryUnit>().ToTable("library_units");
            modelBuilder.Entity<MovementOfLibraryUnit>().ToTable("movements_of_library_units");
            modelBuilder.Entity<ScrappedUnit>().ToTable("scrapped_units");
            modelBuilder.Entity<Section>().ToTable("sections");
            modelBuilder.Entity<Title>().ToTable("titles");
            modelBuilder.Entity<TitleAuthor>().ToTable("titles_authors");
            modelBuilder.Entity<User>().ToTable("users");
        }
    }
}