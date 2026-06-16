namespace OpenTrainingPlatform.Api.Responses;

using OpenTrainingPlatform.Application.Results;

public class CourseResponse
{
    public required Guid Id { get; set; }

    public required string Title { get; set; }

    public string? Description { get; set; }

    public required string Status { get; set; }

    public CoursePackageDto? Package { get; set; }

    public required DateTimeOffset CreatedAt { get; set; }

    public required DateTimeOffset UpdatedAt { get; set; }

    public static CourseResponse ToCourseResponse(UploadCourseResult result)
    {
        return new CourseResponse
        {
            Id = result.Id,
            Title = result.Title,
            Description = result.Description,
            Status = result.Status,
            Package = new CoursePackageDto
            {
                Id = result.Package.Id,
                OriginalFileName = result.Package.OriginalFileName,
                SizeBytes = result.Package.SizeBytes,
                Checksum = result.Package.Checksum,
                UploadedAt = result.Package.UploadedAt
            },
            CreatedAt = result.CreatedAt,
            UpdatedAt = result.UpdatedAt
        };
    }

    public static CourseResponse ToCourseResponse(CourseDetailsResult result)
    {
        return new CourseResponse
        {
            Id = result.Id,
            Title = result.Title,
            Description = result.Description,
            Status = result.Status,
            Package = result.Package != null
                ? new CoursePackageDto
                {
                    Id = result.Package.Id,
                    OriginalFileName = result.Package.OriginalFileName,
                    SizeBytes = result.Package.SizeBytes,
                    Checksum = result.Package.Checksum,
                    UploadedAt = result.Package.UploadedAt
                }
                : null,
            CreatedAt = result.CreatedAt,
            UpdatedAt = result.UpdatedAt
        };
    }
}
