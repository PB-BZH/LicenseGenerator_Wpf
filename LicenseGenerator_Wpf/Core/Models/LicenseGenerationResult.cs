namespace LicenseGenerator_Wpf.Core.Models;

public sealed class LicenseGenerationResult {
  public required string FilePath { get; init; }

  public required string FileName { get; init; }

  public required DateTime GeneratedAt { get; init; }
}
