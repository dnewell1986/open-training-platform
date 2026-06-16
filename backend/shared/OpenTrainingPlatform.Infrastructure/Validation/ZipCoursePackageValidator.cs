namespace OpenTrainingPlatform.Infrastructure.Validation;

using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using OpenTrainingPlatform.Application.Abstractions;

/// <summary>
/// Validates SCORM course packages (zip archives with imsmanifest.xml).
/// </summary>
public class ZipCoursePackageValidator : ICoursePackageValidator
{
    public async Task<PackageValidationResult> ValidateAsync(
        Stream stream,
        string fileName,
        CancellationToken cancellationToken)
    {
        try
        {
            // Validate extension
            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            if (ext != ".zip")
            {
                return new PackageValidationResult(false, "File must be a .zip archive.", null);
            }

            // Reset stream position for reading
            stream.Seek(0, SeekOrigin.Begin);

            // Compute checksum while validating
            string checksum;
            using (var sha256 = SHA256.Create())
            {
                checksum = Convert.ToHexString(await ComputeHashAsync(stream, sha256, cancellationToken));
            }

            // Reset stream for zip validation
            stream.Seek(0, SeekOrigin.Begin);

            // Validate zip structure and manifest
            try
            {
                using (var archive = new ZipArchive(stream, ZipArchiveMode.Read, leaveOpen: true))
                {
                    // Check for imsmanifest.xml at root
                    var manifestEntry = archive.Entries.FirstOrDefault(e =>
                        e.FullName.Equals("imsmanifest.xml", StringComparison.OrdinalIgnoreCase));

                    if (manifestEntry == null)
                    {
                        return new PackageValidationResult(
                            false,
                            "SCORM package must contain imsmanifest.xml at the root.",
                            null);
                    }
                }
            }
            catch (InvalidOperationException)
            {
                return new PackageValidationResult(false, "File is not a valid zip archive.", null);
            }

            return new PackageValidationResult(true, null, checksum);
        }
        catch (Exception ex)
        {
            return new PackageValidationResult(false, $"Validation failed: {ex.Message}", null);
        }
    }

    private static async Task<byte[]> ComputeHashAsync(
        Stream stream,
        HashAlgorithm algorithm,
        CancellationToken cancellationToken)
    {
        const int bufferSize = 81920;
        byte[] buffer = new byte[bufferSize];
        int bytesRead;

        while ((bytesRead = await stream.ReadAsync(buffer, 0, bufferSize, cancellationToken)) > 0)
        {
            algorithm.TransformBlock(buffer, 0, bytesRead, null, 0);
        }

        algorithm.TransformFinalBlock(buffer, 0, 0);

        // Reset stream position after hashing
        stream.Seek(0, SeekOrigin.Begin);

        return algorithm.Hash ?? [];
    }
}
