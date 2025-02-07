using LibrarySystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace LibrarySystem.Data
{
    public class ApplicationDbContext:DbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Author> Authors { get; set; }

        public DbSet<Favorite> Favorites { get; set; }

        public DbSet<Image> Images { get; set; }

        public DbSet<LibraryUnit> LibraryUnits { get; set; }

        public DbSet<MovementOfLibraryUnit> MovementsOfLibraryUnits { get;set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<ScrappedUnit> ScrappedUnits { get; set; }

        public DbSet<Section> Sections { get; set; }

        public DbSet<Title> Titles { get; set; }

        public DbSet<TitleAuthor> TitlesAuthors { get; set; }
        public DbSet<User> Users { get; set; }
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Conventions.Remove(typeof(CascadeDeleteConvention));
            configurationBuilder.Conventions.Remove(typeof(SqlServerOnDeleteConvention));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TitleAuthor>().HasKey(x => new { x.AuthorId, x.TitleId });
            modelBuilder.Entity<Favorite>().HasKey(x => new { x.LibraryUnitId, x.ReaderId });

            modelBuilder.Entity<Title>().HasMany(x => x.TitleAuthors).WithOne(x => x.Title);
            modelBuilder.Entity<Author>().HasMany(x => x.AuthorTitles).WithOne(x => x.Author);

            modelBuilder.Entity<User>().HasMany(x => x.Favorites).WithOne(x => x.Reader);
            modelBuilder.Entity<LibraryUnit>().HasMany(x => x.FavoriteToUsers).WithOne(x => x.LibraryUnit);

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
            modelBuilder.Entity<Favorite>().ToTable("favorites");
            modelBuilder.Entity<Image>().ToTable("images");
            modelBuilder.Entity<LibraryUnit>().ToTable("library_units");
            modelBuilder.Entity<MovementOfLibraryUnit>().ToTable("movements_of_library_units");
            modelBuilder.Entity<Role>().ToTable("roles");
            modelBuilder.Entity<ScrappedUnit>().ToTable("scrapped_units");
            modelBuilder.Entity<Section>().ToTable("sections");
            modelBuilder.Entity<Title>().ToTable("titles");
            modelBuilder.Entity<TitleAuthor>().ToTable("titles_authors");
            modelBuilder.Entity<User>().ToTable("users");

            modelBuilder.Entity<Role>().HasData(
                new Role {Id=1, Name = "administrator"},
                new Role {Id=2, Name = "librarian" },
                new Role {Id=3, Name = "reader" }
            );

            modelBuilder.Entity<User>().HasData(
                 new User
                 {
                     Id=1,
                     Email = "stoyanzlankov06@gmail.com",
                     FirstName = "Stoyan",
                     MiddleName = "Penkov",
                     LastName = "Zlankov",
                     Username = "stoyan06",
                     RoleId = 1,
                     Password = "stoyan06pass"
                 }
                );
        }
    }
}
