using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTrainingPlatform.Application.Abstractions;
using OpenTrainingPlatform.Infrastructure.Data;
using OpenTrainingPlatform.Infrastructure.Repositories;
using OpenTrainingPlatform.Infrastructure.Services;
using OpenTrainingPlatform.Infrastructure.Storage;
using OpenTrainingPlatform.Infrastructure.Validation;

namespace OpenTrainingPlatform.Infrastructure;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<OpenTrainingPlatformDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                .UseSnakeCaseNamingConvention());

        // Repositories
        services.AddScoped<ICourseRepository, EfCourseRepository>();

        // Storage
        var packagesBasePath = configuration["Storage:PackagesPath"] ?? "packages";
        services.AddSingleton<ICoursePackageStorage>(new LocalCoursePackageStorage(packagesBasePath));

        // Validation
        services.AddScoped<ICoursePackageValidator, ZipCoursePackageValidator>();

        // Application Services
        services.AddScoped<ICourseService, CourseService>();

        return services;
    }
}