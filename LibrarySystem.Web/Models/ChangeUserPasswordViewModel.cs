using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Web.Models
{
    public class ChangeUserPasswordViewModel
    {
        [Required(ErrorMessage = "Полето е задължително")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Полето е задължително")]
        [DataType(DataType.Password)]
        [Display(Name = "Нова парола")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$",
        ErrorMessage = "Паролата трябва да е поне 8 символа, да съдържа главна и малка буква, цифра и специален символ.")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Полето е задължително")]
        [DataType(DataType.Password)]
        [Display(Name = "Потвърди новата парола")]
        [Compare("NewPassword", ErrorMessage = "Паролите не съвпадат.")]
        public string ConfirmNewPassword { get; set; }
    }
}
