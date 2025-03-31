using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Web.Models
{
    public class EditAdminViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Полето е задължително")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Полето е задължително")]
        public string MiddleName { get; set; }

        [Required(ErrorMessage = "Полето е задължително")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Полето е задължително")]
        [EmailAddress(ErrorMessage = "Невалиден формат на email адреса")]
        public string Email { get; set; }
    }
}