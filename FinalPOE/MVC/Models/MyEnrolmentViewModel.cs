namespace RaceDayMVC.Models

{

    public class MyEnrolmentViewModel

    {

        public string Registration_ID { get; set; } = "";

        public string Event_ID { get; set; } = "";

        public string? Event_Name { get; set; }

        public string? Event_Location { get; set; }

        public DateTime Event_Date { get; set; }

        public string? Category_ID { get; set; }

        public string? Category_Name { get; set; }

        public DateTime? Registration_Date { get; set; }

        public TimeSpan? Registration_Time { get; set; }

        public string? Registration_Status { get; set; }

    }

}