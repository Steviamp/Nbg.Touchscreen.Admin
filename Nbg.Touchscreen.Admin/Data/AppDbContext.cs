using Microsoft.EntityFrameworkCore;

namespace Nbg.Touchscreen.Admin.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>(e =>
            {
                e.ToTable("Users");
                e.HasIndex(x => x.Email).IsUnique();
                e.Property(x => x.Name).HasMaxLength(100).IsRequired();
                e.Property(x => x.Email).HasMaxLength(256).IsRequired();
                e.Property(x => x.Password).HasMaxLength(200).IsRequired();
                e.Property(x => x.Role).HasMaxLength(20).IsRequired();
            });
        }
    }
}