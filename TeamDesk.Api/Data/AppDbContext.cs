using Microsoft.EntityFrameworkCore;
using TeamDesk.Api.Entities;
using TeamDesk.Api.Entities.enums;   // ← mets "Enums" quand tu auras renommé le dossier

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
            // ---------- Relations (Fluent API) ----------
            // Deux clés étrangères vers User : EF ne peut pas deviner seul, on les configure.
            // Restrict : on ne peut pas supprimer un User qui a des demandes (évite les cascades multiples).
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

            // ---------- Contraintes ----------
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();                  // deux comptes ne peuvent pas avoir le même email

            modelBuilder.Entity<Request>().Property(r => r.Title).HasMaxLength(200);
            modelBuilder.Entity<Category>().Property(c => c.Name).HasMaxLength(100);
            modelBuilder.Entity<User>().Property(u => u.Name).HasMaxLength(100);
            modelBuilder.Entity<User>().Property(u => u.Email).HasMaxLength(256);

            // ---------- Données de test ----------
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Matériel" },
                new Category { Id = 2, Name = "Accès" },
                new Category { Id = 3, Name = "Support" }
            );

            // Hash de "Passw0rd!" généré UNE fois avec PasswordHasher, puis copié ici.
            // On n'appelle pas HashPassword() ici : le sel aléatoire changerait le hash
            // à chaque migration.
            const string passwordHash =
                "AQAAAAIAAYagAAAAELvS9PsS+t505UmPGlCsMXE8r1UEWTE8elW5WLjvmYnBEchMzeIn+Grk3AGM8BLQ0w==";

            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Name = "Karim Manager", Email = "karim@teamdesk.local", PasswordHash = passwordHash, Role = UserRole.Manager },
                new User { Id = 2, Name = "Sara User", Email = "sara@teamdesk.local", PasswordHash = passwordHash, Role = UserRole.User },
                new User { Id = 3, Name = "Yassine Tech", Email = "yassine@teamdesk.local", PasswordHash = passwordHash, Role = UserRole.User },
                new User { Id = 4, Name = "Omar Manager", Email = "omar@gamil.com", PasswordHash = passwordHash, Role = UserRole.Manager },
                new User { Id = 5, Name = "user user", Email = "user@gamil.com", PasswordHash = passwordHash, Role = UserRole.User }
            );
        }
    }
}