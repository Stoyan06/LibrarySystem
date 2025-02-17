using Microsoft.EntityFrameworkCore;
using ServiceStack.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace LibrarySystem.Models
{
    public class LibraryUnit
    {
        [Key]
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        public string InventoryNumber {  get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        public string Condition { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        public string Medium { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        public bool IsScrapped {  get; set; }

        [System.ComponentModel.DataAnnotations.Schema.ForeignKey(nameof(Title))]
        public int TitleId {  get; set; }

        public Title Title { get; set; }

        [AllowNull]
        [RegularExpression(@"^\d{13}$", ErrorMessage = "ISBN трябва да съдържа точно 13 цифри")]
        public string? Isbn { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        public string TypeLibraryUnit {  get; set; }

        [AllowNull]
        public int? Year {  get; set; }

        [AllowNull]
        public string? PublishingHouse {  get; set; }

        public List<Favorite> FavoriteToUsers { get; set; }
    }
}
