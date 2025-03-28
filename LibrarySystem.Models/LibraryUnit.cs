using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace LibrarySystem.Models
{
    public class LibraryUnit
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string InventoryNumber { get; set; }

        [Required]
        public string Condition { get; set; }

        [Required]
        public string Medium { get; set; }

        [Required]
        public bool IsScrapped { get; set; }

        [ForeignKey(nameof(Title))]
        public int TitleId { get; set; }
        public Title Title { get; set; }

        [AllowNull]
        [RegularExpression(@"^\d{13}$", ErrorMessage = "ISBN трябва да съдържа точно 13 цифри")]
        public string? Isbn { get; set; }

        [Required]
        public string TypeLibraryUnit { get; set; }

        [AllowNull]
        public int? Year { get; set; }

        [AllowNull]
        public string? PublishingHouse { get; set; }

        [ForeignKey(nameof(Image))]
        public int ImageId { get; set; }
        public Image? Image { get; set; }

        public bool IsAvailable {  get; set; }

        public bool IsSavedByUser { get; set; }

        [ForeignKey(nameof(User))]
        public int? SavedByReaderId { get; set; }
    }
}