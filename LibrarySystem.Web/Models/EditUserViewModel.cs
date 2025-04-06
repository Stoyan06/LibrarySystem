using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Web.Models
{
    public class EditUserViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Полето е задължително")]
        [MinLength(3,ErrorMessage ="Изискват се минимум 3 символа")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Полето е задължително")]
        [MinLength(3, ErrorMessage = "Изискват се минимум 3 символа")]
        public string MiddleName { get; set; }

        [Required(ErrorMessage = "Полето е задължително")]
        [MinLength(3, ErrorMessage = "Изискват се минимум 3 символа")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Полето е задължително")]
        [EmailAddress(ErrorMessage = "Невалиден формат на email адреса")]
        public string Email { get; set; }
    }
}
