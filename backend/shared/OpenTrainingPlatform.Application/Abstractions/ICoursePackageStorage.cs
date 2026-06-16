namespace OpenTrainingPlatform.Application.Abstractions;

/// <summary>
/// Defines the contract for course package file storage.
/// </summary>
public interface ICoursePackageStorage
{
    /// <summary>
    /// Saves an uploaded course package file.
    /// </summary>
    /// <param name="packageId">The unique identifier for the package.</param>
    /// <param name="courseId">The associated course ID.</param>
    /// <param name="stream">The file stream to store.</param>
    /// <param name="fileName">The original filename.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The storage path or key where the file was saved.</returns>
    Task<string> SavePackageAsync(
        Guid packageId,
        Guid courseId,
        Stream stream,
        string fileName,
        CancellationToken cancellationToken);

    /// <summary>
    /// Deletes a stored course package file.
    /// </summary>
    /// <param name="storagePath">The storage path or key of the file to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the file was deleted; false if it does not exist.</returns>
    Task<bool> DeletePackageAsync(
        string storagePath,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a stored course package file.
    /// </summary>
    /// <param name="storagePath">The storage path or key of the file to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A stream of the file, or null if it does not exist.</returns>
    Task<Stream?> GetPackageAsync(
        string storagePath,
        CancellationToken cancellationToken);
}
