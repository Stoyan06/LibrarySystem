using System.ComponentModel.DataAnnotations.Schema;

namespace LibrarySystem.Models
{
    public class TitleAuthor
    {
        public Title Title { get; set; }

        public Author Author { get; set; }

        [ForeignKey(nameof(Title))]
        public int TitleId {  get; set; }

        [ForeignKey(nameof(Author))]
        public int AuthorId { get; set; }
    }
}
