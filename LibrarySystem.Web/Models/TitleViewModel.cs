using LibrarySystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace LibrarySystem.Web.Models
{
    public class TitleViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int SectionId { get; set; }
        public IEnumerable<SelectListItem> Sections { get; set; }

        public int AuthorId {  get; set; }
        public IEnumerable<SelectListItem> Authors { get; set; }

        public List<string> SelectedAuthors { get; set; } = new List<string>();
    }
}
