using Microsoft.AspNetCore.Http;

namespace RaceDayMVC.Models

{

    public class ProfileViewModel

    {
        public string Participant_ID { get; set; } = "";
        public string? Participant_Name { get; set; }
        public string? Participant_Surname { get; set; }
        public string? Participant_Email { get; set; }
     
        public string? ProfilePictureUrl { get; set; }        
        public IFormFile? ProfilePicture { get; set; }

    }

}