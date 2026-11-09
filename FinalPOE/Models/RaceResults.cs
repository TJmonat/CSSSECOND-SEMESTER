using System.ComponentModel.DataAnnotations;

namespace RaceDayAP1.Models

{

    public class RaceResult

    {
        [Key]
        public string RaceResult_ID { get; set; } = "";

        public string Registration_ID { get; set; } = "";

        public int Finishing_Position { get; set; }

        public TimeSpan Finish_Time { get; set; }

    }

}