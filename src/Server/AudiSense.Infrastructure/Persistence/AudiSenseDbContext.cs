using AudiSense.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace AudiSense.Infrastructure.Persistence;

public class AudiSenseDbContext : DbContext
{
    public AudiSenseDbContext(DbContextOptions<AudiSenseDbContext> options) : base(options)
    {
    }

    public DbSet<HearingTest> HearingTests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<HearingTest>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TesterName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Result).IsRequired().HasMaxLength(500);
            entity.Property(e => e.DateConducted).IsRequired();
        });

        // Seed data for testing
        modelBuilder.Entity<HearingTest>().HasData(
            new HearingTest
            {
                Id = 1,
                TesterName = "Dr. Smith",
                DateConducted = DateTime.Now.AddDays(-7),
                Result = "Normal hearing range"
            },
            new HearingTest
            {
                Id = 2,
                TesterName = "Dr. Johnson",
                DateConducted = DateTime.Now.AddDays(-3),
                Result = "Mild hearing loss detected"
            }
        );
    }
}
