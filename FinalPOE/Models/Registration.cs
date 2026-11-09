namespace RaceDayAP1.Models
{
    public class Registration
    {
        public string Registration_ID { get; set; } = "";

        public string? Registration_Status { get; set; }

        public DateTime? Registration_Date { get; set; }

        public TimeSpan? Registration_Time { get; set; }

        public string? ParticipantID { get; set; }

        public string? EventID { get; set; }
    }
}
