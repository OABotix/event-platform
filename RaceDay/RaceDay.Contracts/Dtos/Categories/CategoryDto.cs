namespace RaceDay.Contracts.Dtos.Categories
{
    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal EntryFee { get; set; }
    }
}