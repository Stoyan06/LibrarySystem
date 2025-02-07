using LibrarySystem.Models;
using ServiceStack.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibrarySystem.Models
{
    public class ScrappedUnit
    {
        [PrimaryKey]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.ForeignKey(nameof(LibraryUnit))]
        public int LibraryUnitId {  get; set; }

        public LibraryUnit LibraryUnit { get; set; }

        [Required]
        public DateTime DateTimeOfScrapping {  get; set; }

        [System.ComponentModel.DataAnnotations.Schema.ForeignKey(nameof(User))]
        public int LibrarianId { get; set; }

        public User Librarian {  get; set; }

        [Required]
        public string Comment { get; set; }
    }
}
