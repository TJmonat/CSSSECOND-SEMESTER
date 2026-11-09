using System.ComponentModel.DataAnnotations;

namespace RaceDayMVC.Models

{

    public class RegisterViewModel

    {

        [Required(ErrorMessage = "Please enter your name.")]

        [StringLength(25, ErrorMessage = "Name cannot exceed 25 characters.")]

        public string Name { get; set; } = "";

        [Required(ErrorMessage = "Please enter your surname.")]

        [StringLength(25, ErrorMessage = "Surname cannot exceed 25 characters.")]

        public string Surname { get; set; } = "";

        [Required(ErrorMessage = "Please enter your email address.")]

        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]

        [StringLength(50, ErrorMessage = "Email cannot exceed 50 characters.")]

        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Please enter a password.")]

        [MinLength(8, ErrorMessage = "Password must contain at least 8 characters.")]

        public string Password { get; set; } = "";

        [Required(ErrorMessage = "Please select an account type.")]

        [RegularExpression("^(Participant|Organiser)$",

            ErrorMessage = "Please select Participant or Organiser.")]

        public string UserRole { get; set; } = "";

    }

}