using System.Drawing;

namespace LicenseGenerator_Wpf.Core.Profiles {
  public sealed class ServiceManagerProfile {
    public ProductOptions Product { get; set; } = new();
    public UpdateManifest UpdateManifest { get; set; } = new();
  }

  public sealed class UpdateManifest {
    public string ProductName { get; set; } = "";
    public string Version { get; set; } = "";
    public string Publisher { get; set; } = "";
    public string DownloadPage { get; set; } = "https://www.pb-bzh-concept.fr";
    public string PrivacyPage { get; set; } = "https://www.pb-bzh-concept.fr/privacy.php";
    public string MsiUrl { get; set; } = "";
    public string WebSetupUrl { get; set; } = "";
    public string UpdateManifestUrl { get; set; } = "";
    public string ReleaseDate { get; set; } = "";
    public string ApplicationId { get; set; } = "";
  }

  public sealed class ProductOptions {
    public string ProductName { get; set; } = "License Generator";
    public string ProductId { get; set; } = "LicenseGenerator";
    public string ProductFolder { get; set; } = "LicenseGenerator";
    public string Manufacturer { get; set; } = "PB BZH Concept";
    public string Version { get; set; } = "1.0.0";
    public string Description { get; set; } = "Generate license keys for PB BZH Concept products";
    public string UpgradeCode { get; set; } = "";
    public string IconPath { get; set; } = "";
    public Image? LogoImage { get; set; } = Properties.Resources.Application;
    public string DownloadPageUrl { get; set; } = "https://www.pb-bzh-concept.fr";
    public string PrivacyPageUrl { get; set; } = "https://www.pb-bzh-concept.fr/privacy.php";
    public string Copyright { get; set; } = "© Copyright PB BZH Concept 2026";
    public string EmailContact { get; set; } = "admin@pb-bzh-concept.fr";
  }
}