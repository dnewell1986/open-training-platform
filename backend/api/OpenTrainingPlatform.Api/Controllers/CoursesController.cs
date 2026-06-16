namespace OpenTrainingPlatform.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using OpenTrainingPlatform.Api.Requests;
using OpenTrainingPlatform.Api.Responses;
using OpenTrainingPlatform.Application.Abstractions;
using OpenTrainingPlatform.Application.Results;

/// <summary>
/// Manages course uploads, retrieval, and deletion.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CoursesController(ICourseService courseService) : ControllerBase
{
    private const long MaxUploadFileSizeBytes = 104_857_600; // 100 MB

    /// <summary>
    /// Uploads a new SCORM course package.
    /// </summary>
    /// <param name="request">The upload request containing the SCORM zip file.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The newly created course with upload metadata.</returns>
    /// <response code="201">Course package accepted and created.</response>
    /// <response code="400">Missing file, invalid file type, invalid archive, or missing manifest.</response>
    /// <response code="413">File exceeds configured size limit.</response>
    /// <response code="500">Unexpected upload or persistence failure.</response>
    [HttpPost]
    [ProducesResponseType(typeof(CourseResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status413PayloadTooLarge)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UploadCourseAsync(
        [FromForm] CourseUploadRequest request,
        CancellationToken cancellationToken)
    {
        var validationError = ValidateUploadRequest(request);
        if (validationError is not null)
        {
            return validationError;
        }

        await using var packageStream = await CreateSeekableStreamAsync(request.Package, cancellationToken);

        var result = await courseService.UploadCourseAsync(
            packageStream,
            request.Package.FileName,
            request.Package.ContentType,
            request.Package.Length,
            request.Title,
            request.Description,
            cancellationToken);

        return CreatedAtAction("GetCourse", new { courseId = result.Id }, CourseResponse.ToCourseResponse(result));
    }

    /// <summary>
    /// Retrieves a paginated list of courses.
    /// </summary>
    /// <param name="status">Optional filter by course status (e.g., 'uploaded', 'invalid', 'deleted').</param>
    /// <param name="page">The page number (1-based). Defaults to 1.</param>
    /// <param name="pageSize">The number of items per page. Defaults to 25, maximum 100.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of courses.</returns>
    /// <response code="200">Courses returned successfully.</response>
    [HttpGet]
    [ProducesResponseType(typeof(CourseListResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListCoursesAsync(
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25,
        CancellationToken cancellationToken = default)
    {
        // Validate and constrain pagination parameters
        if (page < 1)
        {
            page = 1;
        }

        if (pageSize < 1)
        {
            pageSize = 25;
        }
        else if (pageSize > 100)
        {
            pageSize = 100;
        }

        var result = await courseService.ListCoursesAsync(
            status,
            page,
            pageSize,
            cancellationToken);

        var response = new CourseListResponse
        {
            Items = result.Items
                .Select(item => new CourseListItemDto
                {
                    Id = item.Id,
                    Title = item.Title,
                    Description = item.Description,
                    Status = item.Status,
                    PackageFileName = item.PackageFileName,
                    PackageSizeBytes = item.PackageSizeBytes,
                    CreatedAt = item.CreatedAt,
                    UpdatedAt = item.UpdatedAt
                })
                .ToList(),
            Page = result.Page,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount
        };

        return Ok(response);
    }

    /// <summary>
    /// Retrieves detailed information about a specific course.
    /// </summary>
    /// <param name="courseId">The ID of the course.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The course details.</returns>
    /// <response code="200">Course found.</response>
    /// <response code="404">Course does not exist or was deleted.</response>
    [HttpGet("{courseId:guid}")]
    [ProducesResponseType(typeof(CourseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCourseAsync(
        Guid courseId,
        CancellationToken cancellationToken)
    {
        var result = await courseService.GetCourseDetailsAsync(courseId, cancellationToken);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(CourseResponse.ToCourseResponse(result));
    }

    /// <summary>
    /// Deletes a course and its associated package file.
    /// </summary>
    /// <param name="courseId">The ID of the course to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Course deleted successfully.</response>
    /// <response code="404">Course does not exist or was already deleted.</response>
    [HttpDelete("{courseId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCourseAsync(
        Guid courseId,
        CancellationToken cancellationToken)
    {
        var deleted = await courseService.DeleteCourseAsync(courseId, cancellationToken);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    private IActionResult? ValidateUploadRequest(CourseUploadRequest request)
    {
        if (request.Package == null)
        {
            return BadRequest(new { error = "File is required." });
        }

        if (request.Package.Length == 0)
        {
            return BadRequest(new { error = "File size must be greater than zero." });
        }

        if (request.Package.Length > MaxUploadFileSizeBytes)
        {
            return StatusCode(StatusCodes.Status413PayloadTooLarge, new
            {
                error = $"File exceeds maximum size of {MaxUploadFileSizeBytes / (1024 * 1024)} MB."
            });
        }

        var fileExtension = Path.GetExtension(request.Package.FileName).ToLowerInvariant();
        if (fileExtension != ".zip")
        {
            return BadRequest(new { error = "File must be a .zip archive." });
        }

        return null;
    }

    private static async Task<MemoryStream> CreateSeekableStreamAsync(
        IFormFile package,
        CancellationToken cancellationToken)
    {
        var memoryStream = new MemoryStream();
        await using var formFileStream = package.OpenReadStream();
        await formFileStream.CopyToAsync(memoryStream, cancellationToken);
        memoryStream.Seek(0, SeekOrigin.Begin);

        return memoryStream;
    }
}
