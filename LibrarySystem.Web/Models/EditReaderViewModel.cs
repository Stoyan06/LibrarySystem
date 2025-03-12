using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

public class EditReaderViewModel
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