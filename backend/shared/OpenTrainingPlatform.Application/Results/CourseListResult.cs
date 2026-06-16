namespace OpenTrainingPlatform.Application.Results;

public record CourseListResult(
    IReadOnlyList<CourseListItemResult> Items,
    int Page,
    int PageSize,
    int TotalCount);

public record CourseListItemResult(
    Guid Id,
    string Title,
    string? Description,
    string Status,
    string? PackageFileName,
    long? PackageSizeBytes,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
