using Microsoft.AspNetCore.Identity;
using ServiceStack.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibrarySystem.Models
{
    public class User
    {
        [PrimaryKey]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string MiddleName { get; set; }

        [Required]
        public string LastName { get; set; }

        public List<MovementOfLibraryUnit> MovementsOfLibraryUnits { get; set; }

        public IdentityUser IdentityUser { get;set; }

        public string IdentityUserId {  get; set; }
    }
}