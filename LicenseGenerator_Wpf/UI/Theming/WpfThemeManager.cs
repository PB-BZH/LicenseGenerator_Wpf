using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace LicenseGenerator_Wpf.UI.Theming;

public static class WpfThemeManager {
  private static readonly Brush DarkBackBrush =
    new SolidColorBrush(Color.FromRgb(32,32,32));

  private static readonly Brush DarkMediumBrush =
    new SolidColorBrush(Color.FromRgb(25,25,25));

  private static readonly Brush DarkInputBrush =
    new SolidColorBrush(Color.FromRgb(45,45,45));

  private static readonly Brush DarkBorderBrush =
    new SolidColorBrush(Color.FromRgb(70,70,70));

  private static readonly Brush LightTextBrush =
    Brushes.White;

  private static readonly Brush MutedTextBrush =
    new SolidColorBrush(Color.FromRgb(180,180,180));

  private static readonly Brush ButtonHoverBrush =
    new SolidColorBrush(Color.FromRgb(55,55,55));

  private static readonly Brush ButtonPressedBrush =
    new SolidColorBrush(Color.FromRgb(25,25,25));

  public static void ApplyDarkTheme(
    Window window) {

    window.Background = DarkBackBrush;
    window.Foreground = LightTextBrush;

    ApplyDarkTitleBar(window);

    if (window.Content is DependencyObject content) {
      ApplyRecursive(content);
    }

    window.Loaded += (_,_) => {
      if (window.Content is DependencyObject loadedContent) {
        ApplyRecursive(loadedContent);
      }
    };
  }

  private static void ApplyRecursive(
    DependencyObject element) {

    ApplyElement(element);

    int childCount =
      VisualTreeHelper.GetChildrenCount(element);

    for (int index = 0;index < childCount;index++) {
      DependencyObject child =
        VisualTreeHelper.GetChild(element,index);

      ApplyRecursive(child);
    }
  }

  private static void ApplyElement(
    DependencyObject element) {

    bool keepForeColor =
      element is FrameworkElement frameworkElement
      && string.Equals(
        frameworkElement.Tag?.ToString(),
        "KeepForeColor",
        StringComparison.OrdinalIgnoreCase);

    if (element is Panel panel) {
      panel.Background = DarkBackBrush;
    }

    else if (element is Border border) {
      border.Background = DarkBackBrush;
      border.BorderBrush = DarkBorderBrush;
    }

    else if (element is TextBlock textBlock) {
      if (!keepForeColor) {
        textBlock.Foreground = LightTextBrush;
      }
    }

    else if (element is TextBox textBox) {
      textBox.Background = DarkInputBrush;
      textBox.BorderBrush = DarkBorderBrush;

      if (!keepForeColor) {
        textBox.Foreground = LightTextBrush;
      }
    }

    else if (element is PasswordBox passwordBox) {
      passwordBox.Background = DarkInputBrush;
      passwordBox.BorderBrush = DarkBorderBrush;

      if (!keepForeColor) {
        passwordBox.Foreground = LightTextBrush;
      }
    }

    else if (element is ComboBox comboBox) {
      comboBox.Background = DarkInputBrush;
      comboBox.BorderBrush = DarkBorderBrush;

      if (!keepForeColor) {
        comboBox.Foreground = LightTextBrush;
      }
    }

    //else if (element is DatePicker datePicker) {
    //  datePicker.Background = DarkInputBrush;
    //  datePicker.BorderBrush = DarkBorderBrush;

    //  if (!keepForeColor) {
    //    datePicker.Foreground = LightTextBrush;
    //  }
    //}

    else if (element is CheckBox checkBox) {
      if (!keepForeColor) {
        checkBox.Foreground = LightTextBrush;
      }
    }

    else if (element is Button button) {
      StyleDarkButton(button);
    }
  }

  public static void StyleDarkButton(
    Button button) {

    button.Background = DarkMediumBrush;
    button.Foreground = LightTextBrush;
    button.BorderBrush = DarkBorderBrush;
    button.Cursor = Cursors.Hand;
    button.Padding = new Thickness(12,5,12,5);

    button.Resources[SystemColors.ControlBrushKey] =
      ButtonHoverBrush;

    button.Resources[SystemColors.HighlightBrushKey] =
      ButtonPressedBrush;
  }

  private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
  private const int DWMWA_USE_IMMERSIVE_DARK_MODE_OLD = 19;
  private const int DWMWA_CAPTION_COLOR = 35;
  private const int DWMWA_TEXT_COLOR = 36;

  [DllImport("dwmapi.dll")]
  private static extern int DwmSetWindowAttribute(
    IntPtr hwnd,
    int attribute,
    ref int attributeValue,
    int attributeSize);

  public static void ApplyDarkTitleBar(
    Window window) {

    if (window is null) {
      return;
    }

    IntPtr hwnd =
      new WindowInteropHelper(window).Handle;

    if (hwnd == IntPtr.Zero) {
      window.SourceInitialized += (_,_) =>
        ApplyDarkTitleBar(window);

      return;
    }

    int enabled = 1;

    DwmSetWindowAttribute(
      hwnd,
      DWMWA_USE_IMMERSIVE_DARK_MODE,
      ref enabled,
      sizeof(int));

    DwmSetWindowAttribute(
      hwnd,
      DWMWA_USE_IMMERSIVE_DARK_MODE_OLD,
      ref enabled,
      sizeof(int));

    if (OperatingSystem.IsWindowsVersionAtLeast(10,0,22000)) {
      int captionColor = 0x00202020;
      int textColor = 0x00FFFFFF;

      DwmSetWindowAttribute(
        hwnd,
        DWMWA_CAPTION_COLOR,
        ref captionColor,
        sizeof(int));

      DwmSetWindowAttribute(
        hwnd,
        DWMWA_TEXT_COLOR,
        ref textColor,
        sizeof(int));
    }
  }
}