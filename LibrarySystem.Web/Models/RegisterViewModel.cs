using System.ComponentModel.DataAnnotations;

public class RegisterViewModel
{
    [Required(ErrorMessage ="Полето е задължително")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Полето е задължително")]
    [DataType(DataType.Password)]
    [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$",
        ErrorMessage = "Паролата трябва да е поне 8 символа, да съдържа поне една главна буква, една малка буква, едно число и един специален символ (@$!%*?&).")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Потвърдете паролата")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Паролите не съвпадат")]
    public string ConfirmPassword { get; set; }

    [Required(ErrorMessage = "Полето е задължително")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Полето е задължително")]
    public string MiddleName { get; set; }

    [Required(ErrorMessage = "Полето е задължително")]
    public string LastName { get; set; }
}