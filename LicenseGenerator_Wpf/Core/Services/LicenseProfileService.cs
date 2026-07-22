using System.Text.Json;
using LicenseGenerator.Core.Models;

namespace LicenseGenerator.Core.Services;

public sealed class LicenseProfileService {
  private static readonly JsonSerializerOptions JsonOptions = new() {
    WriteIndented = true
  };

  public List<LicenseProfile> LoadProfiles() {
    MigrerAncienFichierSiNecessaire();

    return Directory
      .EnumerateFiles(ApplicationPaths.ProfilesDirectory,"*.json")
      .Select(LoadProfile)
      .Where(p => p is not null)
      .Cast<LicenseProfile>()
      .OrderBy(p => p.DisplayName)
      .ThenBy(p => p.ProductId)
      .ToList();
  }

  public LicenseProfile? LoadProfile(string filePath) {
    try {
      string json = File.ReadAllText(filePath);

      if (string.IsNullOrWhiteSpace(json)) {
        return null;
      }

      return JsonSerializer.Deserialize<LicenseProfile>(
        json,
        JsonOptions);
    }
    catch {
      return null;
    }
  }

  public string SaveProfile(LicenseProfile profile,string? previousFilePath) {

    string filePath = GetProfileFilePath(profile);

    string json = JsonSerializer.Serialize(
      profile,
      JsonOptions);

    File.WriteAllText(filePath,json);

    if (!string.IsNullOrWhiteSpace(previousFilePath)
        && File.Exists(previousFilePath)
        && !string.Equals(
          previousFilePath,
          filePath,
          StringComparison.OrdinalIgnoreCase)) {

      File.Delete(previousFilePath);
    }

    return filePath;
  }

  public string SaveProfile(LicenseProfile profile) {
    string filePath = GetProfileFilePath(profile);

    string json = JsonSerializer.Serialize(
      profile,
      JsonOptions);

    File.WriteAllText(filePath,json);
    return filePath;
  }

  public void DeleteProfile(LicenseProfile profile) {
    string filePath = GetProfileFilePath(profile);

    if (File.Exists(filePath)) {
      File.Delete(filePath);
    }
  }

  public LicenseProfile? LoadProfileFromFile(
  string filePath) {

    try {
      string json = File.ReadAllText(filePath);

      if (string.IsNullOrWhiteSpace(json)) {
        return null;
      }

      return JsonSerializer.Deserialize<LicenseProfile>(
        json,
        JsonOptions);
    }
    catch {
      return null;
    }
  }

  public void DeleteProfileFile(
  string filePath) {

    if (File.Exists(filePath)) {
      File.Delete(filePath);
    }
  }

  private static string GetProfileFilePath(
    LicenseProfile profile) {

    string fileName =
      $"{SanitizeFileName(profile.DisplayName)}-{profile.Id:N}.json";

    return Path.Combine(
      ApplicationPaths.ProfilesDirectory,
      fileName);
  }

  private static string SanitizeFileName(string value) {
    string cleaned = string.IsNullOrWhiteSpace(value)
      ? "profil"
      : value.Trim();

    foreach (char invalidChar in Path.GetInvalidFileNameChars()) {
      cleaned = cleaned.Replace(invalidChar,'-');
    }

    return cleaned;
  }

  private void MigrerAncienFichierSiNecessaire() {
    string oldFilePath = ApplicationPaths.ProfilesFilePath;

    if (!File.Exists(oldFilePath)) {
      return;
    }

    bool hasNewProfiles =
      Directory
        .EnumerateFiles(ApplicationPaths.ProfilesDirectory,"*.json")
        .Any();

    if (hasNewProfiles) {
      return;
    }

    string json = File.ReadAllText(oldFilePath);

    if (string.IsNullOrWhiteSpace(json)) {
      return;
    }

    List<LicenseProfile>? oldProfiles =
      JsonSerializer.Deserialize<List<LicenseProfile>>(
        json,
        JsonOptions);

    if (oldProfiles is null || oldProfiles.Count == 0) {
      return;
    }

    foreach (LicenseProfile profile in oldProfiles) {
      SaveProfile(profile);
    }
  }
}