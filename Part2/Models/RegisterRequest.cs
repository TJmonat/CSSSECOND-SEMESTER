using System.ComponentModel.DataAnnotations;

namespace RaceDayAP1.Models

{

    public class RegisterRequest

    {

        [Required]

        [StringLength(25)]

        public string Name { get; set; } = "";

        [Required]

        [StringLength(25)]

        public string Surname { get; set; } = "";

        [Required]

        [EmailAddress]

        [StringLength(50)]

        public string Email { get; set; } = "";

        [Required]

        [MinLength(8)]

        [StringLength(100)]

        public string Password { get; set; } = "";

        [Required]

        [RegularExpression("^(Participant|Organiser)$",

            ErrorMessage = "Role must be Participant or Organiser.")]

        public string UserRole { get; set; } = "";

    }

}