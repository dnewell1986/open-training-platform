namespace OpenTrainingPlatform.Application.Results;

public record UploadCourseResult(
    Guid Id,
    string Title,
    string? Description,
    string Status,
    UploadedPackageInfo Package,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public record UploadedPackageInfo(
    Guid Id,
    string OriginalFileName,
    long SizeBytes,
    string? Checksum,
    DateTimeOffset UploadedAt);
