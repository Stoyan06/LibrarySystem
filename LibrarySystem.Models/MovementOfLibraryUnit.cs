using ServiceStack.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace LibrarySystem.Models
{
    public class MovementOfLibraryUnit
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        public DateTime DateTime { get; set; }

        [AllowNull]
        public DateOnly Deadline { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        public string Type {  get; set; }

        [System.ComponentModel.DataAnnotations.Schema.ForeignKey(nameof(LibraryUnit))]
        public int LibraryUnitId { get; set; }

        public LibraryUnit LibraryUnit { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.ForeignKey(nameof(LibrarySystem.Models.User))]
        public int? ReaderId { get; set; }

        public User Reader {  get; set; }

        [System.ComponentModel.DataAnnotations.Schema.ForeignKey(nameof(LibrarySystem.Models.User))]
        public int? LibrarianId { get; set; }

        public User Librarian { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        public string Condition { get; set; }
    }
}