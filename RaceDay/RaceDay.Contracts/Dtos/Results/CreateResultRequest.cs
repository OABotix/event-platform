namespace RaceDay.Contracts.Dtos.Results
{
    public class CreateResultRequest
    {
        public int EnrolmentId { get; set; }
        public TimeSpan FinishTime { get; set; }
        public int FinishPosition { get; set; }
    }
}