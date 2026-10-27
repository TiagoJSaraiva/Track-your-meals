using TrackYourMeals.Models;

namespace TrackYourMeals.Services;

public sealed class AppSettings
{
    // Nulo até o usuário escolher; enquanto isso o app segue o tema do Windows.
    public AppTheme? Theme { get; set; }

    public double ZoomLevel { get; set; } = 1.0;
}
