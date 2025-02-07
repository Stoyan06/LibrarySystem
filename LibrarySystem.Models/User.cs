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
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string MiddleName { get; set; }

        [Required]
        public string LastName { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.ForeignKey(nameof(Models.Role))]
        public int RoleId { get; set; }

        public Role Role { get; set; }

        public List<Favorite> Favorites { get; set; }

        public List<MovementOfLibraryUnit> MovementsOfLibraryUnits { get; set; }
    }
}
