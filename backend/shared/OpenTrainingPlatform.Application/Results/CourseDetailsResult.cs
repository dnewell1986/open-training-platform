namespace OpenTrainingPlatform.Application.Results;

public record CourseDetailsResult(
    Guid Id,
    string Title,
    string? Description,
    string Status,
    CoursePackageInfo? Package,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public record CoursePackageInfo(
    Guid Id,
    string OriginalFileName,
    long SizeBytes,
    string? Checksum,
    DateTimeOffset UploadedAt);
