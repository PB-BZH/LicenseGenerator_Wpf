using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using LicenseGenerator_Wpf.Core.Models;

namespace LicenseGenerator_Wpf.Core.Services;

public sealed class LicenseFileService {
  private static readonly JsonSerializerOptions SigningJsonOptions = new() {
    WriteIndented = false,
    PropertyNamingPolicy = null
  };

  private static readonly JsonSerializerOptions OutputJsonOptions = new() {
    WriteIndented = true
  };

  public LicenseGenerationResult GenerateLicenseFile(
    LicenseProfile profile) {

    Validate(profile);

    DateTime generatedAt = DateTime.Now;

    string fileName = BuildFileName(profile,generatedAt);
    string filePath = Path.Combine(
      ApplicationPaths.LicenseOutputDirectory,
      fileName);

    ApplicationLicense license = new() {
      Product = profile.ProductId.Trim(),
      LicenseId = profile.LicenseId.Trim(),
      CustomerName = profile.CustomerName.Trim(),
      SiteName = profile.SiteName.Trim(),
      IssuedAt = DateOnly.FromDateTime(DateTime.Today),
      ValidUntil = profile.ValidUntil,
      MaintenanceUntil = profile.MaintenanceUntil,
      MachineHash = profile.MachineHash.Trim(),
      Features = [
        "Planning",
        "Pdf",
        "Mail",
        "Absences"
      ],
      Signature = string.Empty
    };

    using RSA rsa = LoadPrivateKey();

    string unsignedJson =
      JsonSerializer.Serialize(
        license,
        SigningJsonOptions);

    byte[] signature =
      rsa.SignData(
        Encoding.UTF8.GetBytes(unsignedJson),
        HashAlgorithmName.SHA256,
        RSASignaturePadding.Pkcs1);

    license.Signature =
      Convert.ToBase64String(signature);

    string json =
      JsonSerializer.Serialize(
        license,
        OutputJsonOptions);

    File.WriteAllText(
      filePath,
      json,
      new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

    return new LicenseGenerationResult {
      FilePath = filePath,
      FileName = fileName,
      GeneratedAt = generatedAt
    };
  }

  private static void Validate(LicenseProfile profile) {
    if (string.IsNullOrWhiteSpace(profile.ProductId)) {
      throw new InvalidOperationException(
        "Le produit doit être renseigné.");
    }

    if (string.IsNullOrWhiteSpace(profile.LicenseId)) {
      throw new InvalidOperationException(
        "L'identifiant de licence doit être renseigné.");
    }

    if (string.IsNullOrWhiteSpace(profile.CustomerName)) {
      throw new InvalidOperationException(
        "Le client doit être renseigné.");
    }

    if (string.IsNullOrWhiteSpace(profile.MachineHash)) {
      throw new InvalidOperationException(
        "L'identifiant machine doit être renseigné.");
    }
  }

  public void CreateNewKeyPair(
  bool overwriteExistingKeys = false) {

    string privateKeyPath = GetPrivateKeyPath();
    string publicKeyPath = GetPublicKeyPath();

    if (!overwriteExistingKeys
        && (File.Exists(privateKeyPath)
            || File.Exists(publicKeyPath))) {
      throw new InvalidOperationException(
        "Une paire de clés existe déjà.\n\n" +
        "La création d'une nouvelle paire est bloquée pour éviter " +
        "de rendre les licences existantes incompatibles.");
    }

    using RSA rsa = RSA.Create(4096);

    File.WriteAllText(
      privateKeyPath,
      rsa.ExportRSAPrivateKeyPem());

    File.WriteAllText(
      publicKeyPath,
      rsa.ExportSubjectPublicKeyInfoPem());
  }

  private static string GetPublicKeyPath() {
    return Path.Combine(
      GetKeyDirectory(),
      "public_key.pem");
  }

  private static string BuildFileName(
    LicenseProfile profile,
    DateTime generatedAt) {

    string customer = SanitizeFileName(profile.CustomerName);
    string product = SanitizeFileName(profile.ProductId);

    return
      $"license-{product}-{customer}-{generatedAt:yyyyMMdd-HHmmss}.lic";
  }

  private static string SanitizeFileName(string value) {
    string sanitized = value.Trim();

    foreach (char invalidChar in Path.GetInvalidFileNameChars()) {
      sanitized = sanitized.Replace(invalidChar,'-');
    }

    return string.IsNullOrWhiteSpace(sanitized)
      ? "client"
      : sanitized;
  }

  private static RSA LoadPrivateKey() {
    string privateKeyPath =
      GetPrivateKeyPath();

    if (!File.Exists(privateKeyPath)) {
      throw new InvalidOperationException(
        "La clé privée est introuvable.\n\n" +
        "Le générateur de licence attend la clé privée ici :\n\n" +
        privateKeyPath + "\n\n" +
        "Copiez la clé privée PB-BZH officielle à cet emplacement.\n\n" +
        "Aucune nouvelle paire de clés n'a été créée automatiquement afin " +
        "d'éviter de générer des licences incompatibles avec les applications publiées.");
    }

    RSA rsa = RSA.Create();

    rsa.ImportFromPem(
      File.ReadAllText(privateKeyPath));

    return rsa;
  }

  private static string GetKeyDirectory() {
    string directory = Path.Combine(
      Environment.GetFolderPath(
        Environment.SpecialFolder.LocalApplicationData),
      "PB-BZH",
      "LicenseGenerator");

    Directory.CreateDirectory(directory);

    return directory;
  }

  private static string GetPrivateKeyPath() {
    return Path.Combine(
      GetKeyDirectory(),
      "private_key.pem");
  }
}
