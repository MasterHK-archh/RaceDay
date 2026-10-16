using System.Text.Json.Serialization;

namespace RaceDayAPI.Models
{
    public class Result
    {
        public int ResultId { get; set; }
        public int EnrolmentId { get; set; }

        [JsonIgnore] // <-- stops Enrolment.Result.Enrolment cycle
        public Enrolment? Enrolment { get; set; }

        public string FinishTime { get; set; } = string.Empty;
        public int FinishPosition { get; set; }
    }
}