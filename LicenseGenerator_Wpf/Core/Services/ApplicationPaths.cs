namespace LicenseGenerator.Core.Services;

public static class ApplicationPaths {
  public static string ProfilesDirectory {
    get {
      string directory = Path.Combine(
        ApplicationDataDirectory,
        "Profiles");

      Directory.CreateDirectory(directory);

      return directory;
    }
  }

  public static string ApplicationDataDirectory {
    get {
      string directory = Path.Combine(
        Environment.GetFolderPath(
          Environment.SpecialFolder.LocalApplicationData),
        "PB-BZH",
        "LicenseGenerator");

      Directory.CreateDirectory(directory);

      return directory;
    }
  }

  public static string ProfilesFilePath =>
    Path.Combine(ApplicationDataDirectory,"profiles.json");

  public static string LicenseOutputDirectory {
    get {
      string directory = Path.Combine(
        ApplicationDataDirectory,
        "Licences");

      Directory.CreateDirectory(directory);

      return directory;
    }
  }
}
