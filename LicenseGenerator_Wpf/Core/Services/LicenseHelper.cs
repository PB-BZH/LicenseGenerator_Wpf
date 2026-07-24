using System.Windows;
using LicenseGenerator_Wpf.Core.Profiles;
using PB.BZH.Licensing.Wpf;
using PB.BZH.Licensing.Wpf.Core.Models;
using PB.BZH.Licensing.Wpf.Core.Services;

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

  public static LicenseOptions ConstruireLicenseOptions(
    ServiceManagerProfile profile) {

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
      ApplicationFolder = applicationFolder
    };
  }

  public static LicenseService CreerLicenseService(
    ServiceManagerProfile profile) {

    return new LicenseService(
      ConstruireLicenseOptions(profile));
  }

  public static bool VerifierLicenceAuDemarrage(
    ServiceManagerProfile profile,
    Window? owner = null) {

    if (!TechnicalLicenseRequired) {
      return true;
    }

    LicenseService licenseService =
      CreerLicenseService(profile);

    return VerifierLicenceObligatoire(
      licenseService,
      owner);
  }

  private static bool VerifierLicenceObligatoire(
    LicenseService licenseService,
    Window? owner) {

    LicenseValidationResult licenseResult =
      licenseService.ValidateInstalledLicense();

    if (licenseResult.IsValid) {
      return true;
    }

    LicenseActivationView activationView = new(
      licenseService,
      messageErreur: licenseResult.Message,
      activationObligatoire: true);

    if (owner is not null) {
      activationView.Owner = owner;
      activationView.WindowStartupLocation =
        WindowStartupLocation.CenterOwner;
    }
    else {
      activationView.WindowStartupLocation =
        WindowStartupLocation.CenterScreen;
    }

    bool? dialogResult =
      activationView.ShowDialog();

    if (dialogResult != true) {
      return false;
    }

    licenseResult =
      licenseService.ValidateInstalledLicense();

    return licenseResult.IsValid;
  }

  public static LicenseOptions ConstruireDisplayOptions(
    ServiceManagerProfile profile) {

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
      LicenseActivationView activationView = new(
        licenseService,
        resultat.Message,
        activationObligatoire: false);

      activationView.Owner = owner;
      activationView.WindowStartupLocation =
        WindowStartupLocation.CenterOwner;

      activationView.ShowDialog();
      return;
    }

    LicenseOptions displayOptions =
      ConstruireDisplayOptions(profile);

    LicenseInfoView infoView = new(
      resultat.License,
      displayOptions);

    infoView.Owner = owner;
    infoView.WindowStartupLocation =
      WindowStartupLocation.CenterOwner;

    infoView.ShowDialog();
  }

  public static void ImporterLicence(
    Window owner,
    LicenseService licenseService) {

    LicenseActivationView activationView = new(
      licenseService,
      messageErreur: "Importer une nouvelle licence.",
      activationObligatoire: false);

    activationView.Owner = owner;
    activationView.WindowStartupLocation =
      WindowStartupLocation.CenterOwner;

    activationView.ShowDialog();
  }
}