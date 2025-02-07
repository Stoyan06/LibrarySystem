using ServiceStack.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibrarySystem.Models
{
    public class Author
    {
        [PrimaryKey]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Полето е задължително")]
        public string FullName { get; set; }

        public List<TitleAuthor> AuthorTitles { get; set; }
    }
}
