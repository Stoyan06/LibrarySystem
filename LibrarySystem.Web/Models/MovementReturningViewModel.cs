using System;
using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Models
{
    public class MovementReturningViewModel
    {
        public DateOnly? DeadLine { get; set; }

        [Required(ErrorMessage = "Изберете библиотечна единица.")]
        public int LibraryUnitId { get; set; }

        [Required(ErrorMessage = "Изберете читател.")]
        public int ReaderId { get; set; }

        [Required(ErrorMessage = "Въведете текущото състояние на единицата.")]
        public string Condition { get; set; }
    }
}
