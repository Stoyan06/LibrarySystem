using LibrarySystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace LibrarySystem.Web.Models
{
    public class TitleViewModel
    {
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Полето за име е задължително")]
        public string Name { get; set; }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Полето за описание(анотация) е задължително")]
        public string Description { get; set; }
        public int SectionId { get; set; }
        public IEnumerable<SelectListItem> Sections { get; set; }

        public IEnumerable<SelectListItem> Authors { get; set; }

        public List<int> SelectedAuthors { get; set; } = new List<int>();
    }
}
