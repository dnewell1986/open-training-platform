namespace OpenTrainingPlatform.Api.Responses;

public class CourseListItemDto
{
    public required Guid Id { get; set; }

    public required string Title { get; set; }

    public string? Description { get; set; }

    public required string Status { get; set; }

    public string? PackageFileName { get; set; }

    public long? PackageSizeBytes { get; set; }

    public required DateTimeOffset CreatedAt { get; set; }

    public required DateTimeOffset UpdatedAt { get; set; }
}
