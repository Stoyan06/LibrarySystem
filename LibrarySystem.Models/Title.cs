using ServiceStack.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Xunit.Sdk;

namespace LibrarySystem.Models
{
    public class Title
    {
        [PrimaryKey]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage ="Полето за име е задължително")]
        public string Name {  get; set; }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Полето за описание е задължително")]
        public string Description { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.ForeignKey(nameof(Section))]
        public int SectionId { get; set; }
        public Section Section { get; set; }

        public List<TitleAuthor> TitleAuthors { get; set; }
    }
}
