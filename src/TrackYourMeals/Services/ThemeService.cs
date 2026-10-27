using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using Microsoft.Win32;
using TrackYourMeals.Models;

namespace TrackYourMeals.Services;

public sealed class ThemeService
{
    private const int DwmwaUseImmersiveDarkMode = 20;

    private const string PersonalizeRegistryKey = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";

    public AppTheme CurrentTheme { get; private set; } = AppTheme.Light;

    public void Apply(AppTheme theme)
    {
        var dictionaries = Application.Current.Resources.MergedDictionaries;
        var themeDictionary = new ResourceDictionary
        {
            Source = new Uri(GetThemeSource(theme), UriKind.Absolute)
        };

        var current = dictionaries.FirstOrDefault(dictionary =>
            dictionary.Source?.OriginalString.Contains("Themes/", StringComparison.OrdinalIgnoreCase) == true);

        if (current is null)
        {
            dictionaries.Insert(0, themeDictionary);
        }
        else
        {
            dictionaries[dictionaries.IndexOf(current)] = themeDictionary;
        }

        CurrentTheme = theme;

        foreach (Window window in Application.Current.Windows)
        {
            ApplyTitleBar(window);
        }
    }

    public void ApplyTitleBar(Window window)
    {
        var handle = new WindowInteropHelper(window).Handle;
        if (handle == IntPtr.Zero)
        {
            return;
        }

        var useDarkMode = CurrentTheme == AppTheme.Dark ? 1 : 0;

        // Disponível no Windows 10 20H1+ e Windows 11; em versões antigas a chamada é ignorada.
        _ = DwmSetWindowAttribute(handle, DwmwaUseImmersiveDarkMode, ref useDarkMode, sizeof(int));
    }

    public static AppTheme GetSystemTheme()
    {
        using var key = Registry.CurrentUser.OpenSubKey(PersonalizeRegistryKey);
        return key?.GetValue("AppsUseLightTheme") is int value && value == 0
            ? AppTheme.Dark
            : AppTheme.Light;
    }

    private static string GetThemeSource(AppTheme theme)
    {
        var fileName = theme == AppTheme.Dark ? "DarkTheme.xaml" : "LightTheme.xaml";
        return $"pack://application:,,,/Themes/{fileName}";
    }

    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attribute, ref int value, int size);
}
