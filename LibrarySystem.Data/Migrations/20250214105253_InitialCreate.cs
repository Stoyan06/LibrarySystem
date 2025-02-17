using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LibrarySystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "authors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_authors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sections", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MiddleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_users_roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "roles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "titles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SectionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_titles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_titles_sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "sections",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "library_units",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InventoryNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Condition = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Medium = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsScrapped = table.Column<bool>(type: "bit", nullable: false),
                    TitleId = table.Column<int>(type: "int", nullable: false),
                    Isbn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TypeLibraryUnit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: true),
                    PublishingHouse = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_library_units", x => x.Id);
                    table.ForeignKey(
                        name: "FK_library_units_titles_TitleId",
                        column: x => x.TitleId,
                        principalTable: "titles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "titles_authors",
                columns: table => new
                {
                    TitleId = table.Column<int>(type: "int", nullable: false),
                    AuthorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_titles_authors", x => new { x.AuthorId, x.TitleId });
                    table.ForeignKey(
                        name: "FK_titles_authors_authors_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "authors",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_titles_authors_titles_TitleId",
                        column: x => x.TitleId,
                        principalTable: "titles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "favorites",
                columns: table => new
                {
                    ReaderId = table.Column<int>(type: "int", nullable: false),
                    LibraryUnitId = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_favorites", x => new { x.LibraryUnitId, x.ReaderId });
                    table.ForeignKey(
                        name: "FK_favorites_library_units_LibraryUnitId",
                        column: x => x.LibraryUnitId,
                        principalTable: "library_units",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_favorites_users_ReaderId",
                        column: x => x.ReaderId,
                        principalTable: "users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "images",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsMain = table.Column<bool>(type: "bit", nullable: false),
                    LibraryUnitId = table.Column<int>(type: "int", nullable: false),
                    DestinationLink = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_images", x => x.Id);
                    table.ForeignKey(
                        name: "FK_images_library_units_LibraryUnitId",
                        column: x => x.LibraryUnitId,
                        principalTable: "library_units",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "movements_of_library_units",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Deadline = table.Column<DateOnly>(type: "date", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LibraryUnitId = table.Column<int>(type: "int", nullable: false),
                    ReaderId = table.Column<int>(type: "int", nullable: true),
                    LibrarianId = table.Column<int>(type: "int", nullable: true),
                    Condition = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_movements_of_library_units", x => x.Id);
                    table.ForeignKey(
                        name: "FK_movements_of_library_units_library_units_LibraryUnitId",
                        column: x => x.LibraryUnitId,
                        principalTable: "library_units",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_movements_of_library_units_users_LibrarianId",
                        column: x => x.LibrarianId,
                        principalTable: "users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_movements_of_library_units_users_ReaderId",
                        column: x => x.ReaderId,
                        principalTable: "users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_movements_of_library_units_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "scrapped_units",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LibraryUnitId = table.Column<int>(type: "int", nullable: false),
                    DateTimeOfScrapping = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LibrarianId = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_scrapped_units", x => x.Id);
                    table.ForeignKey(
                        name: "FK_scrapped_units_library_units_LibraryUnitId",
                        column: x => x.LibraryUnitId,
                        principalTable: "library_units",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_scrapped_units_users_LibrarianId",
                        column: x => x.LibrarianId,
                        principalTable: "users",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "administrator" },
                    { 2, "librarian" },
                    { 3, "reader" }
                });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "Id", "Email", "FirstName", "LastName", "MiddleName", "Password", "RoleId", "Username" },
                values: new object[] { 1, "stoyanzlankov06@gmail.com", "Stoyan", "Zlankov", "Penkov", "stoyan06pass", 1, "stoyan06" });

            migrationBuilder.CreateIndex(
                name: "IX_favorites_ReaderId",
                table: "favorites",
                column: "ReaderId");

            migrationBuilder.CreateIndex(
                name: "IX_images_LibraryUnitId",
                table: "images",
                column: "LibraryUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_library_units_TitleId",
                table: "library_units",
                column: "TitleId");

            migrationBuilder.CreateIndex(
                name: "IX_movements_of_library_units_LibrarianId",
                table: "movements_of_library_units",
                column: "LibrarianId");

            migrationBuilder.CreateIndex(
                name: "IX_movements_of_library_units_LibraryUnitId",
                table: "movements_of_library_units",
                column: "LibraryUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_movements_of_library_units_ReaderId",
                table: "movements_of_library_units",
                column: "ReaderId");

            migrationBuilder.CreateIndex(
                name: "IX_movements_of_library_units_UserId",
                table: "movements_of_library_units",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_scrapped_units_LibrarianId",
                table: "scrapped_units",
                column: "LibrarianId");

            migrationBuilder.CreateIndex(
                name: "IX_scrapped_units_LibraryUnitId",
                table: "scrapped_units",
                column: "LibraryUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_titles_SectionId",
                table: "titles",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_titles_authors_TitleId",
                table: "titles_authors",
                column: "TitleId");

            migrationBuilder.CreateIndex(
                name: "IX_users_RoleId",
                table: "users",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "favorites");

            migrationBuilder.DropTable(
                name: "images");

            migrationBuilder.DropTable(
                name: "movements_of_library_units");

            migrationBuilder.DropTable(
                name: "scrapped_units");

            migrationBuilder.DropTable(
                name: "titles_authors");

            migrationBuilder.DropTable(
                name: "library_units");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "authors");

            migrationBuilder.DropTable(
                name: "titles");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "sections");
        }
    }
}
