namespace OpenTrainingPlatform.Infrastructure.Storage;

using OpenTrainingPlatform.Application.Abstractions;

/// <summary>
/// Local filesystem implementation of package storage.
/// Stores uploaded SCORM packages in a configured directory.
/// </summary>
public class LocalCoursePackageStorage : ICoursePackageStorage
{
    private readonly string _basePath;

    public LocalCoursePackageStorage(string basePath = "packages")
    {
        _basePath = Path.Combine(AppContext.BaseDirectory, basePath);
        Directory.CreateDirectory(_basePath);
    }

    public async Task<string> SavePackageAsync(
        Guid packageId,
        Guid courseId,
        Stream stream,
        string fileName,
        CancellationToken cancellationToken)
    {
        var courseDir = Path.Combine(_basePath, courseId.ToString());
        Directory.CreateDirectory(courseDir);

        var storagePath = Path.Combine(courseDir, $"{packageId}{Path.GetExtension(fileName)}");

        await using var fileStream = File.Create(storagePath);
        await stream.CopyToAsync(fileStream, cancellationToken);

        return storagePath;
    }

    public Task<bool> DeletePackageAsync(
        string storagePath,
        CancellationToken cancellationToken)
    {
        try
        {
            if (File.Exists(storagePath))
            {
                File.Delete(storagePath);
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    public Task<Stream?> GetPackageAsync(
        string storagePath,
        CancellationToken cancellationToken)
    {
        if (File.Exists(storagePath))
        {
            var stream = File.OpenRead(storagePath);
            return Task.FromResult<Stream?>(stream);
        }

        return Task.FromResult<Stream?>(null);
    }
}
