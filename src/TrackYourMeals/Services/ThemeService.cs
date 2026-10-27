using Avalonia;
using Avalonia.Platform;
using Avalonia.Styling;
using TrackYourMeals.Models;

namespace TrackYourMeals.Services;

public sealed class ThemeService
{
    public AppTheme CurrentTheme { get; private set; } = AppTheme.Light;

    // A variante escolhe o dicionário de Themes/ e também ajusta a barra de título de cada janela.
    public void Apply(AppTheme theme)
    {
        Application.Current!.RequestedThemeVariant = theme == AppTheme.Dark
            ? ThemeVariant.Dark
            : ThemeVariant.Light;

        CurrentTheme = theme;
    }

    public static AppTheme GetSystemTheme()
    {
        var colors = Application.Current?.PlatformSettings?.GetColorValues();
        return colors?.ThemeVariant == PlatformThemeVariant.Dark
            ? AppTheme.Dark
            : AppTheme.Light;
    }
}
