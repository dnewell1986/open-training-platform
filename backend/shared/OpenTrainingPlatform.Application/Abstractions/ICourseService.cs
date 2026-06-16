namespace OpenTrainingPlatform.Application.Abstractions;

using OpenTrainingPlatform.Application.Results;

/// <summary>
/// Defines the contract for course management use cases.
/// </summary>
public interface ICourseService
{
    /// <summary>
    /// Uploads a SCORM course package and creates a new course record.
    /// </summary>
    /// <param name="packageStream">The SCORM zip file stream.</param>
    /// <param name="fileName">The original filename of the uploaded package.</param>
    /// <param name="contentType">The content type of the uploaded file.</param>
    /// <param name="fileSize">The size of the uploaded file in bytes.</param>
    /// <param name="title">Optional display title. If not provided, will be derived from the filename.</param>
    /// <param name="description">Optional display description.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The uploaded course information.</returns>
    Task<UploadCourseResult> UploadCourseAsync(
        Stream packageStream,
        string fileName,
        string? contentType,
        long fileSize,
        string? title,
        string? description,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a paginated list of courses.
    /// </summary>
    /// <param name="status">Optional filter by course status.</param>
    /// <param name="page">The page number (1-based). Defaults to 1.</param>
    /// <param name="pageSize">The number of items per page. Defaults to 25.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of courses with pagination information.</returns>
    Task<CourseListResult> ListCoursesAsync(
        string? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves detailed information about a specific course.
    /// </summary>
    /// <param name="courseId">The ID of the course to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The course details, or null if the course does not exist.</returns>
    Task<CourseDetailsResult?> GetCourseDetailsAsync(
        Guid courseId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Deletes a course and its associated package file.
    /// </summary>
    /// <param name="courseId">The ID of the course to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the course was successfully deleted; false if it does not exist.</returns>
    Task<bool> DeleteCourseAsync(
        Guid courseId,
        CancellationToken cancellationToken);
}
