namespace LicenseGenerator.Core.Models;

public sealed class LicenseProfile {
  public Guid Id { get; set; } = Guid.NewGuid();

  public string ProductName { get; set; } = string.Empty;

  public string ProductId { get; set; } = string.Empty;

  public string LicenseId { get; set; } = string.Empty;

  public string CustomerName { get; set; } = string.Empty;

  public string SiteName { get; set; } = string.Empty;

  public string EmailContact { get; set; } = string.Empty;

  public string MachineHash { get; set; } = string.Empty;

  public DateOnly? ValidUntil { get; set; }

  public DateOnly? MaintenanceUntil { get; set; }

  public string LastLicenseFilePath { get; set; } = string.Empty;

  public DateTime? LastGeneratedAt { get; set; }

  public string DisplayName =>
    !string.IsNullOrWhiteSpace(ProductName)
      ? ProductName
      : BuildDisplayName();

  public LicenseProfile Copy() {
    return new LicenseProfile {
      Id = Id,
      ProductName = ProductName,
      ProductId = ProductId,
      LicenseId = LicenseId,
      CustomerName = CustomerName,
      SiteName = SiteName,
      EmailContact = EmailContact,
      MachineHash = MachineHash,
      ValidUntil = ValidUntil,
      MaintenanceUntil = MaintenanceUntil,
      LastLicenseFilePath = LastLicenseFilePath,
      LastGeneratedAt = LastGeneratedAt
    };
  }

  private string BuildDisplayName() {
    if (!string.IsNullOrWhiteSpace(CustomerName)
        && !string.IsNullOrWhiteSpace(ProductId)) {
      return $"{CustomerName} - {ProductId}";
    }

    if (!string.IsNullOrWhiteSpace(CustomerName)) {
      return CustomerName;
    }

    if (!string.IsNullOrWhiteSpace(ProductId)) {
      return ProductId;
    }

    return "Nouveau profil";
  }
}
