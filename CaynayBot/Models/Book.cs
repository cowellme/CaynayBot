namespace CaynayBot.Models
{
    public enum BookType
    {
        Online = 1,
        Phone = 2
    }
    public class Book
    {
        public int Id { get; set; }
        public DateTime TimeFrom { get; set; }
        public BookType Type { get; set; }
        public DateTime TimeTo { get; set; }
        public int NumberOfPlace { get; set; }
        public long User { get; set; }
    }
}