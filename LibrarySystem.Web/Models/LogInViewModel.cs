using System.ComponentModel.DataAnnotations;

public class LoginViewModel
{
    [Required(ErrorMessage ="Попълнете полето")]
    [EmailAddress]
    public string Email { get; set; }

    [Required(ErrorMessage = "Попълнете полето")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Display(Name = "Запомни ме")]
    public bool RememberMe { get; set; }
}