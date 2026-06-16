namespace OpenTrainingPlatform.Application.Abstractions;

/// <summary>
/// Defines the contract for SCORM package validation.
/// </summary>
public interface ICoursePackageValidator
{
    /// <summary>
    /// Validates a SCORM course package and computes its checksum.
    /// </summary>
    /// <param name="stream">The package file stream to validate.</param>
    /// <param name="fileName">The filename of the package (for extension validation).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A result containing validation status and error message if invalid.</returns>
    Task<PackageValidationResult> ValidateAsync(
        Stream stream,
        string fileName,
        CancellationToken cancellationToken);
}

/// <summary>
/// Represents the result of package validation.
/// </summary>
public record PackageValidationResult(
    bool IsValid,
    string? ErrorMessage,
    string? Checksum);
