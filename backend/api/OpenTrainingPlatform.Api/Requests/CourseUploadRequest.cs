namespace OpenTrainingPlatform.Api.Requests;

public class CourseUploadRequest
{
    /// <summary>
    /// SCORM zip file to upload.
    /// </summary>
    public required IFormFile Package { get; set; }

    /// <summary>
    /// Optional display title override. If not provided, will be derived from the filename.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Optional display description.
    /// </summary>
    public string? Description { get; set; }
}
