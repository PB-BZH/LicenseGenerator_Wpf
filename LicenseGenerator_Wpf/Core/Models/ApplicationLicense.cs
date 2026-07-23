using System.Reflection;

namespace LicenseGenerator_Wpf.Core.Models;

[Obfuscation(
  Feature = "renaming",
  Exclude = true,
  ApplyToMembers = true)]
public sealed class ApplicationLicense {
  public string Product { get; set; } = string.Empty;

  public string LicenseId { get; set; } = string.Empty;

  public string CustomerName { get; set; } = string.Empty;

  public string SiteName { get; set; } = string.Empty;

  public DateOnly IssuedAt { get; set; }

  public DateOnly? ValidUntil { get; set; }

  public DateOnly? MaintenanceUntil { get; set; }

  public string MachineHash { get; set; } = string.Empty;

  public List<string> Features { get; set; } = [];

  public string Signature { get; set; } = string.Empty;
}
