namespace RaceDayMVC.Models

{

    public class EnrolmentViewModel

    {

        public string? Registration_ID { get; set; }

        public string? Registration_Status { get; set; }

        public DateTime? Registration_Date { get; set; }

        public TimeSpan? Registration_Time { get; set; }

        public string? ParticipantID { get; set; }

        public string? EventID { get; set; }

        public string? Participant_Name { get; set; }

        public string? Participant_Surname { get; set; }

        public string? CategoryID { get; set; }

    }

}