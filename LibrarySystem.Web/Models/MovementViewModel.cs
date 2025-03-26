﻿using System;
using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Models
{
    public class MovementViewModel
    {
        public DateOnly? DeadLine { get; set; }

        [Required(ErrorMessage = "Please select a Library Unit.")]
        public int LibraryUnitId { get; set; }

        [Required(ErrorMessage = "Please select a Reader.")]
        public int ReaderId { get; set; }

        //another view model for returning the unit with condition field
        /*
        [Required(ErrorMessage = "Please enter the book condition.")]
        [StringLength(255, ErrorMessage = "Condition must not exceed 255 characters.")]
        public string Condition { get; set; }
        */
    }
}
