using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using LicenseGenerator_Wpf.Core.Models;

namespace LicenseGenerator_Wpf.Core.Services;

public sealed class PasswordService {
  private static readonly JsonSerializerOptions JsonOptions = new() {
    WriteIndented = true
  };

  private static string SettingsFilePath =>
    Path.Combine(
      ApplicationPaths.ApplicationDataDirectory,
      "password.json");

  public PasswordSettings LoadSettings() {
    if (!File.Exists(SettingsFilePath)) {
      return new PasswordSettings();
    }

    try {
      string json = File.ReadAllText(SettingsFilePath);

      return JsonSerializer.Deserialize<PasswordSettings>(
          json,
          JsonOptions)
        ?? new PasswordSettings();
    }
    catch {
      return new PasswordSettings();
    }
  }

  public void SavePassword(
    string password) {

    if (string.IsNullOrWhiteSpace(password)) {
      throw new InvalidOperationException(
        "Le mot de passe ne peut pas être vide.");
    }

    byte[] salt = RandomNumberGenerator.GetBytes(32);

    byte[] hash =
      HashPassword(
        password,
        salt);

    PasswordSettings settings = new() {
      SaltBase64 = Convert.ToBase64String(salt),
      HashBase64 = Convert.ToBase64String(hash)
    };

    string json =
      JsonSerializer.Serialize(
        settings,
        JsonOptions);

    File.WriteAllText(
      SettingsFilePath,
      json);
  }

  public bool ValidatePassword(
    string password) {

    PasswordSettings settings =
      LoadSettings();

    if (!settings.IsConfigured) {
      return false;
    }

    byte[] salt =
      Convert.FromBase64String(settings.SaltBase64);

    byte[] expectedHash =
      Convert.FromBase64String(settings.HashBase64);

    byte[] actualHash =
      HashPassword(
        password,
        salt);

    return CryptographicOperations.FixedTimeEquals(
      actualHash,
      expectedHash);
  }

  private static byte[] HashPassword(
    string password,
    byte[] salt) {

    byte[] passwordBytes =
      Encoding.UTF8.GetBytes(password);

    byte[] input =
      new byte[salt.Length + passwordBytes.Length];

    Buffer.BlockCopy(
      salt,
      0,
      input,
      0,
      salt.Length);

    Buffer.BlockCopy(
      passwordBytes,
      0,
      input,
      salt.Length,
      passwordBytes.Length);

    return SHA256.HashData(input);
  }
}