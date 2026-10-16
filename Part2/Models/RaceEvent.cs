namespace RaceDayAP1.Models
{
    public class RaceEvent
    {
        public string Event_ID { get; set; } = "";

        public string? Event_Location { get; set; }

        public DateTime Event_Date { get; set; }

        public string? CategoryID { get; set; }

        public string? EventOrganizerID { get; set; }

        public string? VenueID { get; set; }
    }
}
