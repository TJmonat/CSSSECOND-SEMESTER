namespace RaceDayMVC.Models

{

    public class MyResultViewModel

    {

        public string RaceResult_ID { get; set; } = "";

        public string Registration_ID { get; set; } = "";

        public string Event_ID { get; set; } = "";

        public string? Event_Name { get; set; }

        public DateTime Event_Date { get; set; }

        public string? Event_Location { get; set; }

        public string? Category_ID { get; set; }

        public string? Category_Name { get; set; }

        public int Finishing_Position { get; set; }

        public TimeSpan Finish_Time { get; set; }

        public int TotalParticipants { get; set; }

    }

}
