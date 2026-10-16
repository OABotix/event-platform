namespace RaceDay.Contracts.Dtos.Categories
{
    public class CreateCategoryRequest
    {
        public string CategoryName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal EntryFee { get; set; }
    }
}