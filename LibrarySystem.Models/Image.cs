using ServiceStack.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibrarySystem.Models
{
    public class Image
    {
        [PrimaryKey]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id {  get; set; }

        [Required]
        public bool IsMain {  get; set; }

        [System.ComponentModel.DataAnnotations.Schema.ForeignKey(nameof(LibraryUnit))]
        public int LibraryUnitId { get; set; }

        public LibraryUnit LibraryUnit { get; set; }

        [Required]
        public string DestinationLink {  get; set; }
    }
}
