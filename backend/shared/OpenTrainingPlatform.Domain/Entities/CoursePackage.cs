using OpenTrainingPlatform.Domain.Enums;

namespace OpenTrainingPlatform.Domain.Entities;

public class CoursePackage
{
    public Guid Id { get; init; } = Guid.CreateVersion7();

    public required Guid CourseId { get; set; }

    public required string OriginalFileName { get; set; }

    public string? ContentType { get; set; }

    public required long SizeBytes { get; set; }

    public required string StoragePath { get; set; }

    public string? Checksum { get; set; }

    public required CoursePackageStatus UploadStatus { get; set; }

    public string? ValidationMessage { get; set; }

    public required DateTimeOffset UploadedAt { get; set; }
}