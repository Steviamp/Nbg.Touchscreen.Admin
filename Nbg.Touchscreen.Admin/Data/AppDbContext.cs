using Microsoft.EntityFrameworkCore;

namespace Nbg.Touchscreen.Admin.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } = default!;
        public DbSet<Pharmacy> Pharmacies { get; set; } = default!;
        public DbSet<Service> Services { get; set; } = default!;
        public DbSet<Queue> Queues { get; set; } = default!;
        public DbSet<AppSetting> AppSettings { get; set; } = default!;
        public DbSet<Prefecture> Prefectures => Set<Prefecture>();
        public DbSet<GlobalHoliday> GlobalHolidays => Set<GlobalHoliday>();
        public DbSet<ServiceHoliday> ServiceHolidays => Set<ServiceHoliday>();

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

            builder.Entity<Service>(e =>
            {
                e.ToTable("Services");
            });

            builder.Entity<Service>(e =>
            {
                e.ToTable("Services");
                e.HasOne(s => s.Pharmacy)
                 .WithMany(p => p.Services)
                 .HasForeignKey(s => s.PharmacyId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<GlobalHoliday>(e =>
            {
                e.Property(x => x.Date);
                e.Property(x => x.RecurringMonth);
                e.Property(x => x.RecurringDay);
                e.HasIndex(x => new { x.RecurringMonth, x.RecurringDay })
                 .HasFilter("[IsRecurring]=1 AND [IsActive]=1")
                 .IsUnique();
                e.HasIndex(x => x.Date)
                 .HasFilter("[IsRecurring]=0 AND [IsActive]=1")
                 .IsUnique();
            });

            builder.Entity<ServiceHoliday>(e =>
            {
                e.HasOne(x => x.Service).WithMany().HasForeignKey(x => x.ServiceId).OnDelete(DeleteBehavior.Cascade);
                e.HasIndex(x => x.ServiceId);
                e.HasIndex(x => new { x.ServiceId, x.RecurringMonth, x.RecurringDay, x.OverrideMode })
                 .HasFilter("[IsRecurring]=1 AND [IsActive]=1")
                 .IsUnique();
                e.HasIndex(x => new { x.ServiceId, x.Date, x.OverrideMode })
                 .HasFilter("[IsRecurring]=0 AND [IsActive]=1")
                 .IsUnique();
            });
        }
    }
}