using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibrarySystem.Models
{
    public class MovementViewModel
    {
        [Required]
        public DateTime ActionDateTime { get; set; } = DateTime.Now;

        public DateOnly? DeadLine { get; set; }

        [Required]
        public string OperationType { get; set; } // "Giving" or "Returning"

        [Required]
        public int LibraryUnitId { get; set; }

        public string LibraryUnitInfo { get; set; } // For displaying selected unit

        [Required]
        public int ReaderId { get; set; }

        public string ReaderInfo { get; set; } // For displaying selected reader

        [Required]
        public int LibrarianId { get; set; }

        public string LibrarianInfo { get; set; } // For displaying selected librarian

        [Required]
        public string Condition { get; set; }
    }
}
