namespace RaceDay.Contracts.Dtos.Enrolments
{
    public class CreateEnrolmentRequest
    {
        public int EventId { get; set; }
        public int CategoryId { get; set; }
    }
}