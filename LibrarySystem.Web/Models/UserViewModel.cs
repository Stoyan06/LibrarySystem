using System;
using System.ComponentModel.DataAnnotations;

public class UserViewModel
{
    public int Id { get; set; }

    public string Username { get; set; }

    public string Email { get; set; }

    [Required(ErrorMessage = "Въведете име")]
    public string FirstName { get; set; }

    public string MiddleName { get; set; }

    [Required(ErrorMessage = "Въведете фамилия")]
    public string LastName { get; set; }

    [DataType(DataType.Password)]
    public string OldPassword { get; set; }

    [DataType(DataType.Password)]
    public string NewPassword { get; set; }

    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "Новите пароли не съвпадат.")]
    public string ConfirmNewPassword { get; set; }

    public string? Role { get; set; }

    public string PhoneNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
    public string SearchTerm { get; set; }
}