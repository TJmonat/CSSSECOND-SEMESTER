using Microsoft.AspNetCore.Http;

namespace RaceDayMVC.Models

{

    public class EventViewModel

    {

        public string Event_ID { get; set; } = "";

        public string? Event_Name { get; set; }

        public string? Event_Description { get; set; }

        public DateTime Event_Date { get; set; }

        public string? Event_Location { get; set; }

        public decimal? Event_Distance { get; set; }

        public string? Event_Type { get; set; }

        public string? CategoryID { get; set; }

        public string? EventOrganizerID { get; set; }

        public string? VenueID { get; set; }

        public int TotalEnrolments { get; set; }

        

        public string? BannerImageUrl { get; set; }

       
        public IFormFile? BannerImage { get; set; }

    }

}