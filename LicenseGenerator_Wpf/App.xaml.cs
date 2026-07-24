using System.Reflection;
using System.Windows;

namespace LicenseGenerator_Wpf;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
[Obfuscation(
  Feature = "renaming",
  Exclude = true,
  ApplyToMembers = true)]
public partial class App: Application {
}

