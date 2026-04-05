namespace CaynayBot.Models
{
    public class Report
    {
        public int Id { get; set; }
        public long CreatorChatId { get; set; }
        public string? Description { get; set; }
        public int ProductId { get; set; }
        public int Count { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}