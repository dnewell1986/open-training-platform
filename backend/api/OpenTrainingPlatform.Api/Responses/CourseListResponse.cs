namespace OpenTrainingPlatform.Api.Responses;

public class CourseListResponse
{
    public required IReadOnlyList<CourseListItemDto> Items { get; set; }

    public required int Page { get; set; }

    public required int PageSize { get; set; }

    public required int TotalCount { get; set; }
}
