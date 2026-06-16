using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using OpenTrainingPlatform.Infrastructure.Data;

namespace OpenTrainingPlatform.Infrastructure;

/// <summary>
/// Used by dotnet-ef CLI tools at design time (migrations).
/// Not registered in DI — only instantiated by the tooling.
/// </summary>
public sealed class OpenTrainingPlatformDbContextDesignTimeFactory : IDesignTimeDbContextFactory<OpenTrainingPlatformDbContext>
{
    public OpenTrainingPlatformDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<OpenTrainingPlatformDbContext>();
        optionsBuilder
            .UseNpgsql(
                configuration.GetConnectionString("DefaultConnection")
                ?? "Host=localhost;Port=5432;Database=open_training_platform_db;Username=postgres;Password=postgres")
            .UseSnakeCaseNamingConvention();

        return new OpenTrainingPlatformDbContext(optionsBuilder.Options);
    }
}
