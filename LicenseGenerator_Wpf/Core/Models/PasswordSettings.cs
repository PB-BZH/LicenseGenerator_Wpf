namespace LicenseGenerator_Wpf.Core.Models;

public sealed class PasswordSettings {
  public string SaltBase64 { get; set; } = string.Empty;
  public string HashBase64 { get; set; } = string.Empty;

  public bool IsConfigured =>
    !string.IsNullOrWhiteSpace(SaltBase64)
    && !string.IsNullOrWhiteSpace(HashBase64);
}