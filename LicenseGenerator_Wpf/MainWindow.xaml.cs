using System.Text.Json;
using System.Windows;

namespace LicenseGenerator_Wpf;

public partial class MainWindow: Window {
  public MainWindow() {
    InitializeComponent();
    InitialiserFormulaire();
  }

  private void InitialiserFormulaire() {
    dtpValidUntil.SelectedDate =
      DateTime.Today.AddYears(1);

    dtpMaintenanceUntil.SelectedDate =
      DateTime.Today.AddYears(1);

    txtProductId.Text = "GestionPlanningPersonnel";
    txtLicenseId.Text = "GPP-2026-0001";
    txtCustomerName.Text = "LE BELLEVUE";
    txtSite.Text = "7 rue Général Gouraud, 29200 BREST";
    txtEmailContact.Text = "";
    txtMachineHash.Text = "133A6-623D2-B959E-78894";

    txtResult.Text = string.Empty;
  }

  private void BtnNew_Click(
    object sender,
    RoutedEventArgs e) {

    txtProductId.Text = string.Empty;
    txtLicenseId.Text = string.Empty;
    txtCustomerName.Text = string.Empty;
    txtSite.Text = string.Empty;
    txtEmailContact.Text = string.Empty;
    txtMachineHash.Text = string.Empty;

    chkValidUnlimited.IsChecked = false;
    chkMaintenanceUnlimited.IsChecked = false;

    dtpValidUntil.SelectedDate =
      DateTime.Today.AddYears(1);

    dtpMaintenanceUntil.SelectedDate =
      DateTime.Today.AddYears(1);

    txtResult.Text = string.Empty;
  }

  private void BtnGenerate_Click(
    object sender,
    RoutedEventArgs e) {

    object preview = new {
      Product = txtProductId.Text.Trim(),
      LicenseId = txtLicenseId.Text.Trim(),
      CustomerName = txtCustomerName.Text.Trim(),
      SiteName = txtSite.Text.Trim(),
      EmailContact = txtEmailContact.Text.Trim(),
      MachineHash = txtMachineHash.Text.Trim(),
      ValidUntil = chkValidUnlimited.IsChecked == true
        ? null
        : dtpValidUntil.SelectedDate?.ToString("yyyy-MM-dd"),
      MaintenanceUntil = chkMaintenanceUnlimited.IsChecked == true
        ? null
        : dtpMaintenanceUntil.SelectedDate?.ToString("yyyy-MM-dd"),
      Signature = "SIGNATURE_DE_TEST"
    };

    txtResult.Text =
      JsonSerializer.Serialize(
        preview,
        new JsonSerializerOptions {
          WriteIndented = true
        });
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