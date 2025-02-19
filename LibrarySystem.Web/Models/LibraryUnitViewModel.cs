using LibrarySystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceStack;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Xunit.Sdk;

public class LibraryUnitViewModel
{
    public int Id { get; set; }

    [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Полето за инвентарен номер е задължително")]
    public string InventoryNumber { get; set; }


    [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Полето за състояние е задължително")]
    public string Condition { get; set; }


    [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Полето за носител е задължително")]
    public string Medium { get; set; }

    public bool? isScrapped {  get; set; }

    public int TitleId {  get; set; }

    public string? TitleName {  get; set; }

    [RegularExpression(@"^\d{13}$", ErrorMessage = "ISBN трябва да съдържа точно 13 цифри")]
    public string? Isbn { get; set; }

    [Required(ErrorMessage="Полето за вид библиотечна единица е задължително")]
    public string TypeLibraryUnit {  get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Годината трябва да бъде положително число, по-голямо от 0")]
    public int? Year { get; set; }

    public IEnumerable<SelectListItem>? Titles { get; set; }

    public string? PublishingHouse {  get; set; }

    public int? ImageId { get; set; }
    public Image? Image { get; set; }

    [Required(ErrorMessage="Моля, добавете изображение")]
    public IFormFile UploadedImage { get; set; }
}
