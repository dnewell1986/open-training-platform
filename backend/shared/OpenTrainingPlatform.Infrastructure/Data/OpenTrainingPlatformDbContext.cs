using Microsoft.EntityFrameworkCore;
using OpenTrainingPlatform.Domain.Entities;
using OpenTrainingPlatform.Domain.Enums;

namespace OpenTrainingPlatform.Infrastructure.Data;

public class OpenTrainingPlatformDbContext : DbContext
{
    public OpenTrainingPlatformDbContext(DbContextOptions<OpenTrainingPlatformDbContext> options)
        : base(options)
    {
    }

    public DbSet<Course> Courses => Set<Course>();
    public DbSet<CoursePackage> CoursePackages => Set<CoursePackage>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSnakeCaseNamingConvention();
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<CourseStatus>().HaveConversion<string>();
        configurationBuilder.Properties<CoursePackageStatus>().HaveConversion<string>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Course>()
            .Property(c => c.Id)
            .ValueGeneratedNever();

        modelBuilder.Entity<Course>()
            .Property(c => c.Title)
            .HasMaxLength(200);

        modelBuilder.Entity<Course>()
            .Property(c => c.Description)
            .HasMaxLength(500);

        modelBuilder.Entity<Course>()
            .HasIndex(c => c.Status);

        modelBuilder.Entity<Course>()
            .HasIndex(c => c.CreatedAt);

        modelBuilder.Entity<CoursePackage>()
            .Property(cp => cp.Id)
            .ValueGeneratedNever();

        modelBuilder.Entity<CoursePackage>()
            .Property(cp => cp.OriginalFileName)
            .HasMaxLength(255);

        modelBuilder.Entity<CoursePackage>()
            .Property(cp => cp.ContentType)
            .HasMaxLength(255);

        modelBuilder.Entity<CoursePackage>()
            .Property(cp => cp.StoragePath)
            .HasMaxLength(1024);

        modelBuilder.Entity<CoursePackage>()
            .Property(cp => cp.Checksum)
            .HasMaxLength(128);

        modelBuilder.Entity<CoursePackage>()
            .Property(cp => cp.ValidationMessage)
            .HasMaxLength(2000);

        modelBuilder.Entity<CoursePackage>()
            .HasIndex(cp => cp.CourseId);

        modelBuilder.Entity<CoursePackage>()
            .HasOne<Course>()
            .WithMany()
            .HasForeignKey(cp => cp.CourseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}