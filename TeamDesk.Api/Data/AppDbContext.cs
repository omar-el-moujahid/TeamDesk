using Microsoft.EntityFrameworkCore;
using TeamDesk.Api.Entities;
using TeamDesk.Api.Entities.enums;

namespace TeamDesk.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<User> Users => Set<User>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Request> Requests => Set<Request>();
        public DbSet<Comment> Comments => Set<Comment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Request>()
                .HasOne(r => r.CreatedBy)
                .WithMany(u => u.CreatedRequests)
                .HasForeignKey(r => r.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Request>()
                .HasOne(r => r.AssignedTo)
                .WithMany(u => u.AssignedRequests)
                .HasForeignKey(r => r.AssignedToId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Author)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
               .HasIndex(u => u.Email)
               .IsUnique();

            modelBuilder.Entity<Request>().Property(r => r.Title).HasMaxLength(200);
            modelBuilder.Entity<Category>().Property(c => c.Name).HasMaxLength(100);
            modelBuilder.Entity<User>().Property(u => u.Name).HasMaxLength(100);
            modelBuilder.Entity<User>().Property(u => u.Email).HasMaxLength(256);

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Matériel" },
                new Category { Id = 2, Name = "Accès" },
                new Category { Id = 3, Name = "Support" }
            );

            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Name = "Karim Manager", Email = "karim@teamdesk.local", PasswordHash = "TEMP", Role = UserRole.Manager },
                new User { Id = 2, Name = "Sara User", Email = "sara@teamdesk.local", PasswordHash = "TEMP", Role = UserRole.User },
                new User { Id = 3, Name = "Yassine Tech", Email = "yassine@teamdesk.local", PasswordHash = "TEMP", Role = UserRole.User }
            );

        }

    }
}
