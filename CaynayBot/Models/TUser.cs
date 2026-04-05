namespace CaynayBot.Models
{
    public enum ChaynayRole
    {
        Admin,
        Manager
    }
    public enum StateUser
    {
        New, Registred,
        Register0,
        Registr1,
        ApproveAdmin,
        Order0,
        Order1,
        Order01,
        Order02
    }
    public class TUser
    {
        public long ChatId { get; set; }
        public int Age { get; set; }
        public int SearchAge { get; set; }
        public string? City { get; set; }
        public string? AboutMe { get; set; }
        public string? PhotoId { get; set; }
        public string? Username { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true; 
        public bool IsApproved { get; set; } = false;
        public bool IsBanned { get; set; } = false;
        public StateUser State { get; set; } = StateUser.New;
        public ChaynayRole Role { get; set; }
        public string? Viewed { get; set; }
        public int Tryes { get; internal set; } = 0;
        public long LastMessageId { get; set; }
    }
}
