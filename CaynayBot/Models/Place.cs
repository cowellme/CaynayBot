namespace CaynayBot.Models
{
    public class Place
    {
        public int Id { get; set; }
        public int Number{ get; set; }
        public int Lots { get; set; }
        public bool IsFree { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}
