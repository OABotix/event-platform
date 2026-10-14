namespace RaceDay.API.Models
{
    public class Result
    {
        public int ResultID { get; set; }
        public int EnrolmentID { get; set; }
        public TimeSpan FinishTime { get; set; }
        public int FinishPosition { get; set; }
        public DateTime? PublishedAt { get; set; }


        // Navigation property
        public Enrolment? Enrolment { get; set; }
    }
}
