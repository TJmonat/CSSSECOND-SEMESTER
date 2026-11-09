namespace RaceDayAP1.Models
{
    public class UserAccount
    {
        public int UserAccount_ID { get; set; }

        public string Email { get; set; } = "";

        public string PasswordHash { get; set; } = "";

        public string UserRole { get; set; } = "";

        public string? Participant_ID { get; set; }

        public string? Event_Organizer_ID { get; set; }
    }
}
