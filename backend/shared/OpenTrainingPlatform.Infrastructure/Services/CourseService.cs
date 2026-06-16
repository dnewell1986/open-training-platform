namespace OpenTrainingPlatform.Infrastructure.Services;

using OpenTrainingPlatform.Application.Abstractions;
using OpenTrainingPlatform.Application.Results;
using OpenTrainingPlatform.Domain.Entities;
using OpenTrainingPlatform.Domain.Enums;

/// <summary>
/// Orchestrates course management use cases.
/// Coordinates between repository, storage, and validation components.
/// </summary>
public class CourseService(
    ICourseRepository courseRepository,
    ICoursePackageStorage packageStorage,
    ICoursePackageValidator packageValidator) : ICourseService
{
    public async Task<UploadCourseResult> UploadCourseAsync(
        Stream packageStream,
        string fileName,
        string? contentType,
        long fileSize,
        string? title,
        string? description,
        CancellationToken cancellationToken)
    {
        // Validate the package
        var validationResult = await packageValidator.ValidateAsync(packageStream, fileName, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new InvalidOperationException($"Package validation failed: {validationResult.ErrorMessage}");
        }

        // Create domain entities
        var courseId = Guid.NewGuid();
        var packageId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;

        var course = new Course
        {
            Id = courseId,
            Title = title ?? Path.GetFileNameWithoutExtension(fileName),
            Description = description,
            Status = CourseStatus.Uploaded,
            CreatedAt = now,
            UpdatedAt = now
        };

        var coursePackage = new CoursePackage
        {
            Id = packageId,
            CourseId = courseId,
            OriginalFileName = fileName,
            ContentType = contentType,
            SizeBytes = fileSize,
            StoragePath = string.Empty, // Will be set after storage
            Checksum = validationResult.Checksum,
            UploadStatus = CoursePackageStatus.Accepted,
            ValidationMessage = null,
            UploadedAt = now
        };

        try
        {
            // Reset stream for storage
            packageStream.Seek(0, SeekOrigin.Begin);

            // Save the file
            var storagePath = await packageStorage.SavePackageAsync(
                packageId,
                courseId,
                packageStream,
                fileName,
                cancellationToken);

            coursePackage.StoragePath = storagePath;

            // Persist to database
            await courseRepository.CreateCourseWithPackageAsync(
                course,
                coursePackage,
                cancellationToken);

            return new UploadCourseResult(
                courseId,
                course.Title,
                course.Description,
                course.Status.ToString(),
                new UploadedPackageInfo(
                    packageId,
                    coursePackage.OriginalFileName,
                    coursePackage.SizeBytes,
                    coursePackage.Checksum,
                    coursePackage.UploadedAt),
                course.CreatedAt,
                course.UpdatedAt);
        }
        catch
        {
            // Cleanup on failure
            if (!string.IsNullOrEmpty(coursePackage.StoragePath))
            {
                await packageStorage.DeletePackageAsync(coursePackage.StoragePath, cancellationToken);
            }

            throw;
        }
    }

    public async Task<CourseListResult> ListCoursesAsync(
        string? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        // Parse status filter if provided
        CourseStatus? statusFilter = null;
        if (!string.IsNullOrWhiteSpace(status))
        {
            if (Enum.TryParse<CourseStatus>(status, ignoreCase: true, out var parsedStatus))
            {
                statusFilter = parsedStatus;
            }
        }

        // Calculate pagination
        var skip = (page - 1) * pageSize;

        // Query repository
        var (items, totalCount) = await courseRepository.ListCoursesAsync(
            statusFilter,
            skip,
            pageSize,
            cancellationToken);

        // Map to result
        var courseItems = items.Select(item =>
            new CourseListItemResult(
                item.Course.Id,
                item.Course.Title,
                item.Course.Description,
                item.Course.Status.ToString(),
                item.Package?.OriginalFileName,
                item.Package?.SizeBytes,
                item.Course.CreatedAt,
                item.Course.UpdatedAt)).ToList();

        return new CourseListResult(
            courseItems,
            page,
            pageSize,
            totalCount);
    }

    public async Task<CourseDetailsResult?> GetCourseDetailsAsync(
        Guid courseId,
        CancellationToken cancellationToken)
    {
        var (course, package) = await courseRepository.GetCourseWithPackageAsync(courseId, cancellationToken);

        if (course is null)
        {
            return null;
        }

        var packageInfo = package != null
            ? new CoursePackageInfo(
                package.Id,
                package.OriginalFileName,
                package.SizeBytes,
                package.Checksum,
                package.UploadedAt)
            : null;

        return new CourseDetailsResult(
            course.Id,
            course.Title,
            course.Description,
            course.Status.ToString(),
            packageInfo,
            course.CreatedAt,
            course.UpdatedAt);
    }

    public async Task<bool> DeleteCourseAsync(
        Guid courseId,
        CancellationToken cancellationToken)
    {
        // Get the package path before deletion
        var (course, package) = await courseRepository.GetCourseWithPackageAsync(courseId, cancellationToken);

        if (course is null)
        {
            return false;
        }

        // Delete from database
        var deleted = await courseRepository.DeleteCourseAsync(courseId, cancellationToken);

        if (deleted && package != null && !string.IsNullOrEmpty(package.StoragePath))
        {
            // Cleanup the file (fire and forget on error)
            try
            {
                await packageStorage.DeletePackageAsync(package.StoragePath, cancellationToken);
            }
            catch
            {
                // Log but don't fail the operation
            }
        }

        return deleted;
    }
}
