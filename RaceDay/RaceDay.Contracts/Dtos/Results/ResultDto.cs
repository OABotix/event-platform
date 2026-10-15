namespace RaceDay.Contracts.Dtos.Results
{
    public class ResultDto
    {
        public int ResultId { get; set; }
        public int EnrolmentId { get; set; }
        public string ParticipantName { get; set; } = string.Empty;
        public string EventName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public TimeSpan FinishTime { get; set; }
        public int FinishPosition { get; set; }
        public DateTime? PublishedAt { get; set; }
    }
}