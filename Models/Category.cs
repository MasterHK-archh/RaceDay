using System.Text.Json.Serialization;

namespace RaceDayAPI.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public int EventId { get; set; }

        [JsonIgnore] // <-- stops the Event.Categories.Event cycle
        public Event? Event { get; set; }

        public string Name { get; set; } = string.Empty;

        [JsonIgnore]
        public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();
    }
}