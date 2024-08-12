using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Lot> Lots { get; set; }
        public DbSet<Bid> Bids { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasKey(u => u.Id);
            modelBuilder.Entity<Category>().HasKey(c => c.Id);
            modelBuilder.Entity<Lot>().HasKey(l => l.Id);
            modelBuilder.Entity<Bid>().HasKey(b => b.Id);

            modelBuilder.Entity<Lot>();

            modelBuilder.Entity<Lot>()
                .HasOne(l => l.Category)
                .WithMany()
                .HasForeignKey(l => l.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Bid>()
                .HasOne(b => b.User)
                .WithMany()
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Bid>()
                .HasOne(b => b.Lot)
                .WithMany()
                .HasForeignKey(b => b.LotId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
