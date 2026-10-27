using System.Reflection;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace TrackYourMeals;

public partial class HelpWindow : Window
{
    public HelpWindow()
    {
        InitializeComponent();

        // A versão do assembly vem da tag da release (ver .github/workflows/release.yml).
        var version = Assembly.GetExecutingAssembly().GetName().Version;
        VersionText.Text = version is null ? string.Empty : $"Versão {version.ToString(3)}";
    }

    private void Close_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
