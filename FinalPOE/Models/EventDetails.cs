using System.ComponentModel.DataAnnotations;

namespace RaceDayAP1.Models

{

    public class EventDetails

    {

        [Key]

        public string EventDetails_ID { get; set; } = "";

        public string Event_ID { get; set; } = "";

        public string Event_Name { get; set; } = "";

        public string? Event_Description { get; set; }

        public string? Event_Distance { get; set; }

        public string? Event_Type { get; set; }

    }

}