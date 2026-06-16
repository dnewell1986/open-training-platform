using OpenTrainingPlatform.Domain.Enums;

namespace OpenTrainingPlatform.Domain.Entities;

public class Course
{
    public Guid Id { get; init; } = Guid.CreateVersion7();

    public required string Title { get; set; }

    public string? Description { get; set; }

    public required CourseStatus Status { get; set; }

    public required DateTimeOffset CreatedAt { get; set; }

    public required DateTimeOffset UpdatedAt { get; set; }
}