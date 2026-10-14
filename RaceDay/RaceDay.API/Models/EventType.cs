using Microsoft.Extensions.Logging;

namespace RaceDay.API.Models
{
    public class EventType
    {
        public int EventTypeID { get; set; }
        public string TypeName { get; set; } = string.Empty;


        // Navigation property
        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
}
