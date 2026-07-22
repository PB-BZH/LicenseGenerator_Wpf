using System.IO;
using System.Windows;
using LicenseGenerator_Wpf.Core.Models;
using LicenseGenerator_Wpf.Core.Services;

namespace LicenseGenerator_Wpf;

public partial class MainWindow: Window {
  private readonly LicenseFileService _licenseFileService = new();

  private LicenseProfile _currentProfile = new();

  public MainWindow() {
    InitializeComponent();

    ApplyProfileToUI(
      CreateDefaultProfile());
  }

  private static LicenseProfile CreateDefaultProfile() {
    DateOnly today =
      DateOnly.FromDateTime(DateTime.Today);

    return new LicenseProfile {
      ProductId = "GestionPlanningPersonnel",
      LicenseId = "GPP-2026-0001",
      CustomerName = "LE BELLEVUE",
      SiteName = "7 rue Général Gouraud, 29200 BREST",
      EmailContact = "admin@pb-bzh-concept.fr",
      MachineHash = "133A6-623D2-B959E-78894",
      ValidUntil = today.AddYears(1),
      MaintenanceUntil = today.AddYears(1)
    };
  }

  private static LicenseProfile CreateEmptyProfile() {
    DateOnly today =
      DateOnly.FromDateTime(DateTime.Today);

    return new LicenseProfile {
      ValidUntil = today.AddYears(1),
      MaintenanceUntil = today.AddYears(1)
    };
  }

  private LicenseProfile ApplyUIToProfile() {
    LicenseProfile profile =
      _currentProfile;

    profile.ProductId =
      txtProductId.Text?.Trim() ?? string.Empty;

    profile.LicenseId =
      txtLicenseId.Text?.Trim() ?? string.Empty;

    profile.CustomerName =
      txtCustomerName.Text?.Trim() ?? string.Empty;

    profile.SiteName =
      txtSite.Text?.Trim() ?? string.Empty;

    profile.EmailContact =
      txtEmailContact.Text?.Trim() ?? string.Empty;

    profile.MachineHash =
      txtMachineHash.Text?.Trim() ?? string.Empty;

    profile.ValidUntil =
      chkValidUnlimited.IsChecked == true
        ? null
        : ToDateOnly(dtpValidUntil.SelectedDate);

    profile.MaintenanceUntil =
      chkMaintenanceUnlimited.IsChecked == true
        ? null
        : ToDateOnly(dtpMaintenanceUntil.SelectedDate);

    profile.ProductName =
      ConstruireNomProfil(profile);

    return profile;
  }

  private void ApplyProfileToUI(
    LicenseProfile profile) {

    _currentProfile = profile;

    txtProductId.Text =
      profile.ProductId;

    txtLicenseId.Text =
      profile.LicenseId;

    txtCustomerName.Text =
      profile.CustomerName;

    txtSite.Text =
      profile.SiteName;

    txtEmailContact.Text =
      profile.EmailContact;

    txtMachineHash.Text =
      profile.MachineHash;

    chkValidUnlimited.IsChecked =
      profile.ValidUntil is null;

    dtpValidUntil.SelectedDate =
      (profile.ValidUntil
       ?? DateOnly.FromDateTime(DateTime.Today).AddYears(1))
      .ToDateTime(TimeOnly.MinValue);

    dtpValidUntil.IsEnabled =
      profile.ValidUntil is not null;

    chkMaintenanceUnlimited.IsChecked =
      profile.MaintenanceUntil is null;

    dtpMaintenanceUntil.SelectedDate =
      (profile.MaintenanceUntil
       ?? DateOnly.FromDateTime(DateTime.Today).AddYears(1))
      .ToDateTime(TimeOnly.MinValue);

    dtpMaintenanceUntil.IsEnabled =
      profile.MaintenanceUntil is not null;

    txtResult.Text =
      string.Empty;
  }

  private static DateOnly? ToDateOnly(
    DateTime? date) {

    if (date is null) {
      return null;
    }

    return DateOnly.FromDateTime(
      date.Value);
  }

  private static string ConstruireNomProfil(
    LicenseProfile profile) {

    string client =
      profile.CustomerName?.Trim() ?? string.Empty;

    string produit =
      profile.ProductId?.Trim() ?? string.Empty;

    if (!string.IsNullOrWhiteSpace(client)
        && !string.IsNullOrWhiteSpace(produit)) {
      return $"{client} - {produit}";
    }

    if (!string.IsNullOrWhiteSpace(client)) {
      return client;
    }

    if (!string.IsNullOrWhiteSpace(produit)) {
      return produit;
    }

    return "Nouveau profil";
  }

  private void BtnNew_Click(
    object sender,
    RoutedEventArgs e) {

    ApplyProfileToUI(
      CreateEmptyProfile());
  }

  private void BtnGenerate_Click(
    object sender,
    RoutedEventArgs e) {

    try {
      LicenseProfile profile =
        ApplyUIToProfile();

      LicenseGenerationResult result =
        _licenseFileService.GenerateLicenseFile(profile);

      profile.LastLicenseFilePath =
        result.FilePath;

      profile.LastGeneratedAt =
        result.GeneratedAt;

      txtResult.Text =
        File.ReadAllText(result.FilePath);

      txtResult.CaretIndex = 0;
      txtResult.ScrollToHome();

      MessageBox.Show(
        this,
        "La licence a été générée avec succès.\n\n" +
        result.FilePath,
        "Licence générée",
        MessageBoxButton.OK,
        MessageBoxImage.Information);
    }
    catch (Exception exception) {
      MessageBox.Show(
        this,
        exception.Message,
        "Génération impossible",
        MessageBoxButton.OK,
        MessageBoxImage.Error);
    }
  }

  private void ChkValidUnlimited_Checked(
    object sender,
    RoutedEventArgs e) {

    dtpValidUntil.IsEnabled = false;
  }

  private void ChkValidUnlimited_Unchecked(
    object sender,
    RoutedEventArgs e) {

    dtpValidUntil.IsEnabled = true;
  }

  private void ChkMaintenanceUnlimited_Checked(
    object sender,
    RoutedEventArgs e) {

    dtpMaintenanceUntil.IsEnabled = false;
  }

  private void ChkMaintenanceUnlimited_Unchecked(
    object sender,
    RoutedEventArgs e) {

    dtpMaintenanceUntil.IsEnabled = true;
  }
}