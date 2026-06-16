namespace OpenTrainingPlatform.Application.Abstractions;

using OpenTrainingPlatform.Domain.Entities;
using OpenTrainingPlatform.Domain.Enums;

/// <summary>
/// Defines the contract for course data access.
/// </summary>
public interface ICourseRepository
{
    /// <summary>
    /// Creates a new course and its associated package record.
    /// </summary>
    /// <param name="course">The course entity to create.</param>
    /// <param name="package">The course package entity to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created course and package entities.</returns>
    Task CreateCourseWithPackageAsync(
        Course course,
        CoursePackage package,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a course by ID, including its package if it exists.
    /// </summary>
    /// <param name="courseId">The ID of the course to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The course with its package, or null if not found or deleted.</returns>
    Task<(Course? Course, CoursePackage? Package)> GetCourseWithPackageAsync(
        Guid courseId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a paginated list of courses with optional status filter, excluding deleted courses.
    /// </summary>
    /// <param name="status">Optional filter by course status.</param>
    /// <param name="skip">The number of items to skip.</param>
    /// <param name="take">The number of items to take.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A tuple containing the list of courses with packages and the total count.</returns>
    Task<(List<(Course Course, CoursePackage? Package)> Items, int TotalCount)> ListCoursesAsync(
        CourseStatus? status,
        int skip,
        int take,
        CancellationToken cancellationToken);

    /// <summary>
    /// Marks a course and its package as deleted.
    /// </summary>
    /// <param name="courseId">The ID of the course to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the course was deleted; false if it does not exist or was already deleted.</returns>
    Task<bool> DeleteCourseAsync(
        Guid courseId,
        CancellationToken cancellationToken);
}
