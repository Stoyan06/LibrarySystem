﻿using System.ComponentModel.DataAnnotations;
using Xunit.Sdk;

public class LibrarianViewModel
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

    [Required(ErrorMessage = "Полето е задължително")]
    [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$",
        ErrorMessage = "Паролата трябва да е поне 8 символа, да съдържа поне една главна буква, една малка буква, едно число и един специален символ (@$!%*?&).")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Потвърдете паролата")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Паролите не съвпадат")]
    public string ConfirmPassword { get; set; }
}
