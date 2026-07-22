using LicenseGenerator.Core.Models;
using LicenseGenerator.UI.Forms;

namespace LicenseGenerator.Core.Services;

public sealed class ApplicationAccessService {
  private readonly PasswordService _passwordService = new();

  public bool AuthorizeStartup() {
    PasswordSettings settings =
      _passwordService.LoadSettings();

    if (!settings.IsConfigured) {
      return ConfigureFirstPasswordIfAllowed();
    }

    return AskExistingPassword();
  }

  private bool ConfigureFirstPasswordIfAllowed() {
    if (ConfigurationExistanteDetectee()) {
      MessageBox.Show(
        "La configuration de sécurité est absente.\r\n\r\n" +
        "Le fichier password.json est introuvable alors que l'application " +
        "contient déjà des profils ou une clé privée.\r\n\r\n" +
        "Par sécurité, l'application va se fermer.",
        "Sécurité",
        MessageBoxButtons.OK,
        MessageBoxIcon.Error);

      return false;
    }

    using PasswordForm setupForm = new(
      "Définissez le mot de passe d'accès à l'application.",
      confirmPassword: true);

    if (setupForm.ShowDialog() != DialogResult.OK) {
      return false;
    }

    _passwordService.SavePassword(setupForm.Password);
    return true;
  }

  private bool AskExistingPassword() {
    using PasswordForm loginForm = new(
      "Saisissez le mot de passe d'accès à l'application.",
      confirmPassword: false);

    if (loginForm.ShowDialog() != DialogResult.OK) {
      return false;
    }

    if (_passwordService.ValidatePassword(loginForm.Password)) {
      return true;
    }

    MessageBox.Show(
      "Mot de passe incorrect.",
      "Mot de passe",
      MessageBoxButtons.OK,
      MessageBoxIcon.Warning);

    return false;
  }

  private static bool ConfigurationExistanteDetectee() {
    bool privateKeyExists =
      File.Exists(
        Path.Combine(
          ApplicationPaths.ApplicationDataDirectory,
          "private_key.pem"));

    bool profilesExist =
      Directory.Exists(ApplicationPaths.ProfilesDirectory)
      && Directory
        .EnumerateFiles(
          ApplicationPaths.ProfilesDirectory,
          "*.json")
        .Any();

    return privateKeyExists || profilesExist;
  }
}