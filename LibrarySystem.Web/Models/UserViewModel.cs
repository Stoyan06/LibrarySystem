using System;
using System.ComponentModel.DataAnnotations;

public class UserViewModel
{
    public int Id { get; set; } // Needed for identifying users in actions (Edit, Delete, Details)

    public string Username { get; set; }

    public string Email { get; set; }

    [Required(ErrorMessage = "Моля, въведете име.")]
    public string FirstName { get; set; }

    public string MiddleName { get; set; }

    [Required(ErrorMessage = "Моля, въведете фамилия.")]
    public string LastName { get; set; }

    // Password fields
    [DataType(DataType.Password)]
    public string OldPassword { get; set; }

    [DataType(DataType.Password)]
    public string NewPassword { get; set; }

    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "Новите пароли не съвпадат.")]
    public string ConfirmNewPassword { get; set; }

    public string? Role { get; set; }

    // Additional properties needed for the view
    public string PhoneNumber { get; set; } // Contact info  
    public DateTime CreatedAt { get; set; } // Date of account creation  
    public bool IsActive { get; set; } // Whether the account is active  

    // Search functionality
    public string SearchTerm { get; set; } // Allows searching users by multiple fields
}
