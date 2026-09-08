using System.Windows;
using LicenseGenerator_Wpf.Core.Profiles;
using PB.BZH.Licensing.Core.Models;
using PB.BZH.Licensing.Core.Services;
using PB.BZH.Licensing.Wpf.UI.Windows;

namespace LicenseGenerator_Wpf.Core.Services;

public static class LicenseHelper {
  public static bool TechnicalLicenseRequired {
    get {
#if TECHNICAL_LICENSE_REQUIRED
      return true;
#else
      return false;
#endif
    }
  }

  public static LicenseOptions ConstruireLicenseOptions(ServiceManagerProfile profile) {
    string productId =
      !string.IsNullOrWhiteSpace(profile.Product.ProductId)
        ? profile.Product.ProductId
        : "LicenseGenerator";

    string applicationFolder =
      !string.IsNullOrWhiteSpace(profile.Product.ProductFolder)
        ? profile.Product.ProductFolder
        : productId;

    return new LicenseOptions {
      ProductId = productId,
      ApplicationFolder = applicationFolder,
    };
  }

  public static LicenseService CreerLicenseService(ServiceManagerProfile profile) {
    return new LicenseService(ConstruireLicenseOptions(profile));
  }

  public static bool VerifierLicenceAuDemarrage(ServiceManagerProfile profile) {
    if (!TechnicalLicenseRequired) {
      return true;
    }
    LicenseService licenseService = CreerLicenseService(profile);
    return VerifierLicenceObligatoire(licenseService);
  }

  private static bool VerifierLicenceObligatoire(LicenseService licenseService) {
    LicenseValidationResult licenseResult = licenseService.ValidateInstalledLicense();
    if (licenseResult.IsValid) {
      return true;
    }
    //using var activationForm = new LicenseActivationForm(
    //  licenseService,
    //  messageErreur: licenseResult.Message,
    //  activationObligatoire: true);
    //if (activationForm.ShowDialog() != DialogResult.OK) {
    //  return false;
    //}
    licenseResult = licenseService.ValidateInstalledLicense();
    return licenseResult.IsValid;
  }

  public static LicenseOptions ConstruireDisplayOptions(ServiceManagerProfile profile) {
    return new LicenseOptions {
      LogoImage = profile.Product.LogoImage
    };
  }

  public static void AfficherLicence(
      Window owner,
      LicenseService licenseService,
      ServiceManagerProfile profile) {

    LicenseValidationResult resultat =
      licenseService.ValidateInstalledLicense();

    if (!resultat.IsValid || resultat.License is null) {

      LicenseActivationWindow activationWindow = new(
        licenseService,
        resultat.Message,
        activationObligatoire: false);

      activationWindow.Owner = owner;

      activationWindow.WindowStartupLocation =
        WindowStartupLocation.CenterOwner;

      activationWindow.ShowDialog();

      return;
    }

    LicenseOptions displayOptions =
      ConstruireDisplayOptions(profile);

    LicenseInfoWindow infoWindow = new(
      resultat.License,
      displayOptions);

    infoWindow.Owner = owner;

    infoWindow.WindowStartupLocation =
      WindowStartupLocation.CenterOwner;

    infoWindow.ShowDialog();
  }

  public static void ImporterLicence(
      Window owner,
      LicenseService licenseService) {

    LicenseActivationWindow activationWindow = new(
      licenseService,
      messageErreur: "Importer une nouvelle licence.",
      activationObligatoire: false);

    activationWindow.Owner = owner;

    activationWindow.WindowStartupLocation =
      WindowStartupLocation.CenterOwner;

    activationWindow.ShowDialog();
  }
}