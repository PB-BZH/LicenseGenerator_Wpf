using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace LicenseGenerator.Core.Services;

public sealed class LicenseMailService {
  private const int MapiLogonUi = 0x00000001;
  private const int MapiDialog = 0x00000008;
  private const int MapiTo = 1;
  private const int MapiUserAbort = 1;
  private const int MapiSuccess = 0;

  public void SendLicenseByDefaultClient(
    IWin32Window owner,
    string recipient,
    string subject,
    string body,
    string attachmentPath) {

    if (string.IsNullOrWhiteSpace(recipient)) {
      throw new InvalidOperationException(
        "L'adresse email du client doit être renseignée.");
    }

    if (!File.Exists(attachmentPath)) {
      throw new FileNotFoundException(
        "Le fichier de licence est introuvable.",
        attachmentPath);
    }

    int result = TrySendBySimpleMapi(
      owner,
      recipient,
      subject,
      body,
      attachmentPath);

    if (result is MapiSuccess or MapiUserAbort) {
      return;
    }

    if (TryOpenThunderbirdCompose(
        recipient,
        subject,
        body,
        attachmentPath)) {
      return;
    }

    OpenMailToFallback(recipient,subject,body);

    throw new InvalidOperationException(
      "Le client MAPI n'a pas accepté la pièce jointe " +
      $"(code {result}).\n\n" +
      "Thunderbird n'a pas été trouvé automatiquement. " +
      "Un email sans pièce jointe a été préparé en secours.");
  }

  private static int TrySendBySimpleMapi(
    IWin32Window owner,
    string recipient,
    string subject,
    string body,
    string attachmentPath) {

    IntPtr recipientPointer = IntPtr.Zero;
    IntPtr filePointer = IntPtr.Zero;

    try {
      var recipientDescription = new MapiRecipDesc {
        RecipClass = MapiTo,
        Name = recipient,
        Address = $"SMTP:{recipient}"
      };

      recipientPointer = Marshal.AllocHGlobal(
        Marshal.SizeOf<MapiRecipDesc>());

      Marshal.StructureToPtr(
        recipientDescription,
        recipientPointer,
        false);

      var fileDescription = new MapiFileDesc {
        Position = -1,
        Path = attachmentPath,
        Name = Path.GetFileName(attachmentPath)
      };

      filePointer = Marshal.AllocHGlobal(
        Marshal.SizeOf<MapiFileDesc>());

      Marshal.StructureToPtr(
        fileDescription,
        filePointer,
        false);

      MapiMessage message = new() {
        Subject = subject,
        NoteText = body,
        RecipCount = 1,
        Recips = recipientPointer,
        FileCount = 1,
        Files = filePointer
      };

      return MAPISendMail(
        IntPtr.Zero,
        owner.Handle,
        ref message,
        MapiLogonUi | MapiDialog,
        0);
    }
    catch (DllNotFoundException) {
      return -1;
    }
    catch (EntryPointNotFoundException) {
      return -2;
    }
    finally {
      if (recipientPointer != IntPtr.Zero) {
        Marshal.DestroyStructure<MapiRecipDesc>(recipientPointer);
        Marshal.FreeHGlobal(recipientPointer);
      }

      if (filePointer != IntPtr.Zero) {
        Marshal.DestroyStructure<MapiFileDesc>(filePointer);
        Marshal.FreeHGlobal(filePointer);
      }
    }
  }

  private static bool TryOpenThunderbirdCompose(
    string recipient,
    string subject,
    string body,
    string attachmentPath) {

    string? thunderbirdPath =
      FindThunderbirdExecutable();

    if (string.IsNullOrWhiteSpace(thunderbirdPath)
        || !File.Exists(thunderbirdPath)) {
      return false;
    }

    string attachmentUri =
      new Uri(Path.GetFullPath(attachmentPath)).AbsoluteUri;

    string compose =
      $"to='{EscapeThunderbirdValue(recipient)}'," +
      $"subject='{EscapeThunderbirdValue(subject)}'," +
      $"body='{EscapeThunderbirdValue(body)}'," +
      $"attachment='{EscapeThunderbirdValue(attachmentUri)}'";

    ProcessStartInfo startInfo = new() {
      FileName = thunderbirdPath,
      UseShellExecute = false
    };

    startInfo.ArgumentList.Add("-compose");
    startInfo.ArgumentList.Add(compose);

    Process.Start(startInfo);

    return true;
  }

  private static string EscapeThunderbirdValue(
    string value) {

    return value
      .Replace("\\","\\\\")
      .Replace("'","\\'")
      .Replace("\r\n","\n")
      .Replace("\r","\n");
  }

  private static string? FindThunderbirdExecutable() {
    string? appPath =
      ReadThunderbirdAppPath(Registry.CurrentUser)
      ?? ReadThunderbirdAppPath(Registry.LocalMachine);

    if (!string.IsNullOrWhiteSpace(appPath)
        && File.Exists(appPath)) {
      return appPath;
    }

    string[] candidatePaths = [
      Path.Combine(
        Environment.GetFolderPath(
          Environment.SpecialFolder.ProgramFiles),
        "Mozilla Thunderbird",
        "thunderbird.exe"),
      Path.Combine(
        Environment.GetFolderPath(
          Environment.SpecialFolder.ProgramFilesX86),
        "Mozilla Thunderbird",
        "thunderbird.exe")
    ];

    return candidatePaths.FirstOrDefault(File.Exists);
  }

  private static string? ReadThunderbirdAppPath(
    RegistryKey rootKey) {

    using RegistryKey? key = rootKey.OpenSubKey(
      @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\thunderbird.exe");

    return key?.GetValue(null)?.ToString();
  }

  private static void OpenMailToFallback(
    string recipient,
    string subject,
    string body) {

    string uri =
      $"mailto:{Uri.EscapeDataString(recipient)}" +
      $"?subject={Uri.EscapeDataString(subject)}" +
      $"&body={Uri.EscapeDataString(body)}";

    Process.Start(new ProcessStartInfo {
      FileName = uri,
      UseShellExecute = true
    });
  }

  [DllImport("MAPI32.DLL",CharSet = CharSet.Ansi)]
  private static extern int MAPISendMail(
    IntPtr session,
    IntPtr uiParam,
    ref MapiMessage message,
    int flags,
    int reserved);

  [StructLayout(LayoutKind.Sequential,CharSet = CharSet.Ansi)]
  private struct MapiMessage {
    public int Reserved;

    [MarshalAs(UnmanagedType.LPStr)]
    public string? Subject;

    [MarshalAs(UnmanagedType.LPStr)]
    public string? NoteText;

    [MarshalAs(UnmanagedType.LPStr)]
    public string? MessageType;

    [MarshalAs(UnmanagedType.LPStr)]
    public string? DateReceived;

    [MarshalAs(UnmanagedType.LPStr)]
    public string? ConversationId;

    public int Flags;
    public IntPtr Originator;
    public int RecipCount;
    public IntPtr Recips;
    public int FileCount;
    public IntPtr Files;
  }

  [StructLayout(LayoutKind.Sequential,CharSet = CharSet.Ansi)]
  private struct MapiRecipDesc {
    public int Reserved;
    public int RecipClass;

    [MarshalAs(UnmanagedType.LPStr)]
    public string? Name;

    [MarshalAs(UnmanagedType.LPStr)]
    public string? Address;

    public int EidSize;
    public IntPtr EntryId;
  }

  [StructLayout(LayoutKind.Sequential,CharSet = CharSet.Ansi)]
  private struct MapiFileDesc {
    public int Reserved;
    public int Flags;
    public int Position;

    [MarshalAs(UnmanagedType.LPStr)]
    public string? Path;

    [MarshalAs(UnmanagedType.LPStr)]
    public string? Name;

    public IntPtr Type;
  }
}
