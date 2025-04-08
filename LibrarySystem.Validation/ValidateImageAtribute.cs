using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace LibrarySystem.Validation
{
    public class ValidateImageAttribute : ValidationAttribute
    {
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
        private readonly string[] _allowedMimeTypes = { "image/jpeg", "image/png", "image/gif", "image/bmp", "image/webp" };

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var file = value as IFormFile;

            if (file == null)
                return ValidationResult.Success;

            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!_allowedExtensions.Contains(extension))
                return new ValidationResult("Невалиден тип файл. Разрешени са само изображения.");

            if (!_allowedMimeTypes.Contains(file.ContentType))
                return new ValidationResult("Невалиден тип MIME. Разрешени са само изображения.");

            return ValidationResult.Success;
        }
    }
}