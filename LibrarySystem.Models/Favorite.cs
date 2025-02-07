using ServiceStack.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace LibrarySystem.Models
{
    public class Favorite
    {
        public User Reader {  get; set; }
        
        public LibraryUnit LibraryUnit { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.ForeignKey(nameof(User))]
        public int ReaderId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.ForeignKey(nameof(LibraryUnit))]
        public int LibraryUnitId { get; set; }

        [AllowNull]
        public string Comment { get; set; }
    }
}
