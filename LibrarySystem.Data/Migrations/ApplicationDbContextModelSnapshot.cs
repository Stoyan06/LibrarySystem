﻿// <auto-generated />
using System;
using LibrarySystem.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace LibrarySystem.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("LibrarySystem.Models.Author", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("authors", (string)null);
                });

            modelBuilder.Entity("LibrarySystem.Models.Favorite", b =>
                {
                    b.Property<int>("LibraryUnitId")
                        .HasColumnType("int");

                    b.Property<int>("ReaderId")
                        .HasColumnType("int");

                    b.Property<string>("Comment")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("LibraryUnitId", "ReaderId");

                    b.HasIndex("ReaderId");

                    b.ToTable("favorites", (string)null);
                });

            modelBuilder.Entity("LibrarySystem.Models.Image", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("DestinationLink")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsMain")
                        .HasColumnType("bit");

                    b.Property<int>("LibraryUnitId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("LibraryUnitId");

                    b.ToTable("images", (string)null);
                });

            modelBuilder.Entity("LibrarySystem.Models.LibraryUnit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Condition")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("InventoryNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsScrapped")
                        .HasColumnType("bit");

                    b.Property<string>("Isbn")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Medium")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PublishingHouse")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TitleId")
                        .HasColumnType("int");

                    b.Property<string>("TypeLibraryUnit")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Year")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("TitleId");

                    b.ToTable("library_units", (string)null);
                });

            modelBuilder.Entity("LibrarySystem.Models.MovementOfLibraryUnit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Condition")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("datetime2");

                    b.Property<DateOnly>("Deadline")
                        .HasColumnType("date");

                    b.Property<int?>("LibrarianId")
                        .HasColumnType("int");

                    b.Property<int>("LibraryUnitId")
                        .HasColumnType("int");

                    b.Property<int?>("ReaderId")
                        .HasColumnType("int");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("LibrarianId");

                    b.HasIndex("LibraryUnitId");

                    b.HasIndex("ReaderId");

                    b.HasIndex("UserId");

                    b.ToTable("movements_of_library_units", (string)null);
                });

            modelBuilder.Entity("LibrarySystem.Models.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("roles", (string)null);

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "administrator"
                        },
                        new
                        {
                            Id = 2,
                            Name = "librarian"
                        },
                        new
                        {
                            Id = 3,
                            Name = "reader"
                        });
                });

            modelBuilder.Entity("LibrarySystem.Models.ScrappedUnit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Comment")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateTimeOfScrapping")
                        .HasColumnType("datetime2");

                    b.Property<int>("LibrarianId")
                        .HasColumnType("int");

                    b.Property<int>("LibraryUnitId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("LibrarianId");

                    b.HasIndex("LibraryUnitId");

                    b.ToTable("scrapped_units", (string)null);
                });

            modelBuilder.Entity("LibrarySystem.Models.Section", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("sections", (string)null);
                });

            modelBuilder.Entity("LibrarySystem.Models.Title", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SectionId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("SectionId");

                    b.ToTable("titles", (string)null);
                });

            modelBuilder.Entity("LibrarySystem.Models.TitleAuthor", b =>
                {
                    b.Property<int>("AuthorId")
                        .HasColumnType("int");

                    b.Property<int>("TitleId")
                        .HasColumnType("int");

                    b.HasKey("AuthorId", "TitleId");

                    b.HasIndex("TitleId");

                    b.ToTable("titles_authors", (string)null);
                });

            modelBuilder.Entity("LibrarySystem.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MiddleName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("users", (string)null);

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Email = "stoyanzlankov06@gmail.com",
                            FirstName = "Stoyan",
                            LastName = "Zlankov",
                            MiddleName = "Penkov",
                            Password = "stoyan06pass",
                            RoleId = 1,
                            Username = "stoyan06"
                        });
                });

            modelBuilder.Entity("LibrarySystem.Models.Favorite", b =>
                {
                    b.HasOne("LibrarySystem.Models.LibraryUnit", "LibraryUnit")
                        .WithMany("FavoriteToUsers")
                        .HasForeignKey("LibraryUnitId")
                        .IsRequired();

                    b.HasOne("LibrarySystem.Models.User", "Reader")
                        .WithMany("Favorites")
                        .HasForeignKey("ReaderId")
                        .IsRequired();

                    b.Navigation("LibraryUnit");

                    b.Navigation("Reader");
                });

            modelBuilder.Entity("LibrarySystem.Models.Image", b =>
                {
                    b.HasOne("LibrarySystem.Models.LibraryUnit", "LibraryUnit")
                        .WithMany()
                        .HasForeignKey("LibraryUnitId")
                        .IsRequired();

                    b.Navigation("LibraryUnit");
                });

            modelBuilder.Entity("LibrarySystem.Models.LibraryUnit", b =>
                {
                    b.HasOne("LibrarySystem.Models.Title", "Title")
                        .WithMany()
                        .HasForeignKey("TitleId")
                        .IsRequired();

                    b.Navigation("Title");
                });

            modelBuilder.Entity("LibrarySystem.Models.MovementOfLibraryUnit", b =>
                {
                    b.HasOne("LibrarySystem.Models.User", "Librarian")
                        .WithMany()
                        .HasForeignKey("LibrarianId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("LibrarySystem.Models.LibraryUnit", "LibraryUnit")
                        .WithMany()
                        .HasForeignKey("LibraryUnitId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("LibrarySystem.Models.User", "Reader")
                        .WithMany()
                        .HasForeignKey("ReaderId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("LibrarySystem.Models.User", null)
                        .WithMany("MovementsOfLibraryUnits")
                        .HasForeignKey("UserId");

                    b.Navigation("Librarian");

                    b.Navigation("LibraryUnit");

                    b.Navigation("Reader");
                });

            modelBuilder.Entity("LibrarySystem.Models.ScrappedUnit", b =>
                {
                    b.HasOne("LibrarySystem.Models.User", "Librarian")
                        .WithMany()
                        .HasForeignKey("LibrarianId")
                        .IsRequired();

                    b.HasOne("LibrarySystem.Models.LibraryUnit", "LibraryUnit")
                        .WithMany()
                        .HasForeignKey("LibraryUnitId")
                        .IsRequired();

                    b.Navigation("Librarian");

                    b.Navigation("LibraryUnit");
                });

            modelBuilder.Entity("LibrarySystem.Models.Title", b =>
                {
                    b.HasOne("LibrarySystem.Models.Section", "Section")
                        .WithMany()
                        .HasForeignKey("SectionId")
                        .IsRequired();

                    b.Navigation("Section");
                });

            modelBuilder.Entity("LibrarySystem.Models.TitleAuthor", b =>
                {
                    b.HasOne("LibrarySystem.Models.Author", "Author")
                        .WithMany("AuthorTitles")
                        .HasForeignKey("AuthorId")
                        .IsRequired();

                    b.HasOne("LibrarySystem.Models.Title", "Title")
                        .WithMany("TitleAuthors")
                        .HasForeignKey("TitleId")
                        .IsRequired();

                    b.Navigation("Author");

                    b.Navigation("Title");
                });

            modelBuilder.Entity("LibrarySystem.Models.User", b =>
                {
                    b.HasOne("LibrarySystem.Models.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .IsRequired();

                    b.Navigation("Role");
                });

            modelBuilder.Entity("LibrarySystem.Models.Author", b =>
                {
                    b.Navigation("AuthorTitles");
                });

            modelBuilder.Entity("LibrarySystem.Models.LibraryUnit", b =>
                {
                    b.Navigation("FavoriteToUsers");
                });

            modelBuilder.Entity("LibrarySystem.Models.Title", b =>
                {
                    b.Navigation("TitleAuthors");
                });

            modelBuilder.Entity("LibrarySystem.Models.User", b =>
                {
                    b.Navigation("Favorites");

                    b.Navigation("MovementsOfLibraryUnits");
                });
#pragma warning restore 612, 618
        }
    }
}
