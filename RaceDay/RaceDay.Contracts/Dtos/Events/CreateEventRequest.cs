namespace RaceDay.Contracts.Dtos.Events
{
    public class CreateEventRequest
    {
        public string EventName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public decimal Distance { get; set; }
        public int EventTypeId { get; set; }
    }
}