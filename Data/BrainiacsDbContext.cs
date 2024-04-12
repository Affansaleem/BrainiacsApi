using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BrainiacsApi.Models.Profile;
using Microsoft.AspNetCore.Identity;
using BrainiacsApi.Models;

namespace BrainiacsApi.Data
{
    public class BrainiacsDbContext : IdentityDbContext<IdentityUser>
    {
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Follow> Follows { get; set; }

        public BrainiacsDbContext(DbContextOptions<BrainiacsDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            SeedRoles(modelBuilder);
            ConfigureLikes(modelBuilder);
            ConfigureComments(modelBuilder);
            ConfigureFollows(modelBuilder);
        }
        
        private void SeedRoles(ModelBuilder builder)
        {
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole() { Name = "Admin", NormalizedName = "Admin", ConcurrencyStamp = "1" },
                new IdentityRole() { Name = "User", NormalizedName = "User", ConcurrencyStamp = "2" }
            );
        }

        private void ConfigureLikes(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Like>()
                .HasOne(l => l.Post)
                .WithMany(p => p.Likes)
                .HasForeignKey(l => l.PostId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private void ConfigureComments(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private void ConfigureFollows(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Follow>()
                .HasOne(f => f.FollowedUser)
                .WithMany()
                .HasForeignKey(f => f.FollowedUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Follow>()
                .HasOne(f => f.Follower)
                .WithMany()
                .HasForeignKey(f => f.FollowerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
