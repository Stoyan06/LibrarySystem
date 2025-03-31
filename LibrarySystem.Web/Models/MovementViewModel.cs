using System;
using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Models
{
    public class MovementViewModel
    {
        public DateOnly? DeadLine { get; set; }

        [Required(ErrorMessage = "Изберете библиотечна единица.")]
        public int LibraryUnitId { get; set; }

        [Required(ErrorMessage = "Изберете читател.")]
        public int ReaderId { get; set; }
    }
}
