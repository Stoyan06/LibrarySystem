using ServiceStack.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace LibrarySystem.Models
{
    public class Section
    {
        [PrimaryKey]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage= "Полето за име е задължително")]
        public string Name { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        public string Description { get; set; }
    }
}