namespace OpenTrainingPlatform.Api.Responses;

public class CoursePackageDto
{
    public required Guid Id { get; set; }

    public required string OriginalFileName { get; set; }

    public required long SizeBytes { get; set; }

    public string? Checksum { get; set; }

    public required DateTimeOffset UploadedAt { get; set; }
}
