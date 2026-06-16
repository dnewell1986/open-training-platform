namespace OpenTrainingPlatform.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using OpenTrainingPlatform.Application.Abstractions;
using OpenTrainingPlatform.Domain.Entities;
using OpenTrainingPlatform.Domain.Enums;
using OpenTrainingPlatform.Infrastructure.Data;

/// <summary>
/// Entity Framework Core implementation of the course repository.
/// </summary>
public class EfCourseRepository(OpenTrainingPlatformDbContext dbContext) : ICourseRepository
{
    public async Task CreateCourseWithPackageAsync(
        Course course,
        CoursePackage package,
        CancellationToken cancellationToken)
    {
        await dbContext.Courses.AddAsync(course, cancellationToken);
        await dbContext.CoursePackages.AddAsync(package, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<(Course? Course, CoursePackage? Package)> GetCourseWithPackageAsync(
        Guid courseId,
        CancellationToken cancellationToken)
    {
        var course = await dbContext.Courses
            .Where(c => c.Id == courseId && c.Status != CourseStatus.Deleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (course is null)
        {
            return (null, null);
        }

        var package = await dbContext.CoursePackages
            .Where(cp => cp.CourseId == courseId && cp.UploadStatus != CoursePackageStatus.Deleted)
            .FirstOrDefaultAsync(cancellationToken);

        return (course, package);
    }

    public async Task<(List<(Course Course, CoursePackage? Package)> Items, int TotalCount)> ListCoursesAsync(
        CourseStatus? status,
        int skip,
        int take,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Courses
            .Where(c => c.Status != CourseStatus.Deleted);

        if (status.HasValue)
        {
            query = query.Where(c => c.Status == status);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var courses = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        var items = new List<(Course, CoursePackage?)>(courses.Count);

        foreach (var course in courses)
        {
            var package = await dbContext.CoursePackages
                .Where(cp => cp.CourseId == course.Id && cp.UploadStatus != CoursePackageStatus.Deleted)
                .FirstOrDefaultAsync(cancellationToken);

            items.Add((course, package));
        }

        return (items, totalCount);
    }

    public async Task<bool> DeleteCourseAsync(
        Guid courseId,
        CancellationToken cancellationToken)
    {
        var course = await dbContext.Courses
            .Where(c => c.Id == courseId && c.Status != CourseStatus.Deleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (course is null)
        {
            return false;
        }

        course.Status = CourseStatus.Deleted;
        course.UpdatedAt = DateTimeOffset.UtcNow;

        var packages = await dbContext.CoursePackages
            .Where(cp => cp.CourseId == courseId && cp.UploadStatus != CoursePackageStatus.Deleted)
            .ToListAsync(cancellationToken);

        foreach (var package in packages)
        {
            package.UploadStatus = CoursePackageStatus.Deleted;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
