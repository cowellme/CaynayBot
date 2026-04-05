using Microsoft.EntityFrameworkCore;
using CaynayBot.Models;

namespace CaynayBot.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<TUser> Users { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Book> Books { get; set; } = null!;
        public DbSet<Report> Reports { get; set; } = null!;
        public DbSet<Place> Places { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TUser>(entity =>
            {
                entity.HasKey(u => u.ChatId);
                entity.Property(u => u.Username).HasMaxLength(100);
                entity.Property(u => u.FirstName).HasMaxLength(100);
                entity.Property(u => u.LastName).HasMaxLength(100);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Name).HasMaxLength(100);
            });

            modelBuilder.Entity<Report>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Description).HasMaxLength(100);
            });
            base.OnModelCreating(modelBuilder);
        }
    }
}
