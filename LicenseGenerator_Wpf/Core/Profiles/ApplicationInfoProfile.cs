/*
╔════════════════════════════════════════════════════════════════════════════════╗
║                                                                                ║
║                    ───────────────────────────────────                         ║
║                      © Copyright PB-BZH Concept 2026                           ║
║                    ───────────────────────────────────                         ║
║                                                                                ║
║                 contact : mailto:admin@pb-bzh-concept.fr                       ║
╚════════════════════════════════════════════════════════════════════════════════╝

╔════════════════════════════════════════════════════════════════════════════════╗
║  Auteur : Patrick Bourges - PB-BZH Concept                                     ║
║  Le 26/7/2026 - 11:39
╟────────────────────────────────────────────────────────────────────────────────║
║     Projet Visual Studio Professional 2026 : PB.BZH.Help.Library
╟────────────────────────────────────────────────────────────────────────────────║
║     Version : 1.0.0
╟────────────────────────────────────────────────────────────────────────────────║
║                Visual Studio Professional 2026 - Insiders                      ║
║                ──────────────────────────────────────────                      ║
║  Langage     : C# 14                                                           ║
║  Technologie : .NET 10 / WinForms                                              ║
║  Plateforme  : Windows 10 / Windows 11                                         ║
║  Encodage    : UTF-8                                                           ║
╟────────────────────────────────────────────────────────────────────────────────║
║  Nom de fichier : ApplicationInfoProfile.cs
╚════════════════════════════════════════════════════════════════════════════════╝
*/
namespace LicenseGenerator_Wpf.Core.Profiles;

public sealed class ApplicationInfoProfile {
  public ProductInfo Product { get; set; } = new();
  public UpdateInfo Update { get; set; } = new();
}

public sealed class UpdateInfo {
  public string ProductName { get; set; } = string.Empty;
  public string Version { get; set; } = string.Empty;
  public string Publisher { get; set; } = string.Empty;
  public string DownloadPage { get; set; } = "https://www.pb-bzh-concept.fr";
  public string PrivacyPage { get; set; } = "https://www.pb-bzh-concept.fr/softwares/privacy.php";
  public string MsiUrl { get; set; } = string.Empty;
  public string WebSetupUrl { get; set; } = string.Empty;
  public string UpdateManifestUrl { get; set; } = string.Empty;
  public string ReleaseDate { get; set; } = string.Empty;
  public string ApplicationId { get; set; } = string.Empty;
}

public sealed class ProductInfo {
  public string ProductName { get; set; } = string.Empty;
  public string Manufacturer { get; set; } = "PB BZH Concept";
  public string Version { get; set; } = "1.0.0";
  public string Description { get; set; } = string.Empty;
  public string UpgradeCode { get; set; } = string.Empty;
  public string IconPath { get; set; } = string.Empty;
  public object? LogoPath { get; set; } = null;
  public string DownloadPageUrl { get; set; } = "https://www.pb-bzh-concept.fr";
  public string PrivacyPageUrl { get; set; } = "https://www.pb-bzh-concept.fr/softwares/privacy.php";
  public string Copyright { get; set; } = "© Copyright PB BZH Concept 2026";
  public string EmailContact { get; set; } = "admin@pb-bzh-concept.fr";
  public string ProductId { get; set; } = string.Empty;
}