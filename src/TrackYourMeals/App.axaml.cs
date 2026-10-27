using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using TrackYourMeals.Data;
using TrackYourMeals.Services;
using TrackYourMeals.ViewModels;

namespace TrackYourMeals;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.ShutdownMode = ShutdownMode.OnMainWindowClose;
            Dispatcher.UIThread.UnhandledException += OnDispatcherUnhandledException;
            _ = StartAsync(desktop);
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static async Task StartAsync(IClassicDesktopStyleApplicationLifetime desktop)
    {
        var dbContextFactory = new TrackYourMealsDbContextFactory();
        var settingsService = new SettingsService();
        var themeService = new ThemeService();

        var settings = settingsService.Load();
        themeService.Apply(settings.Theme ?? ThemeService.GetSystemTheme());

        try
        {
            var databaseInitializer = new DatabaseInitializer(dbContextFactory);
            await databaseInitializer.InitializeAsync();

            var viewModel = new MainViewModel(
                new DietDayService(dbContextFactory),
                new FoodService(dbContextFactory),
                new FoodLogService(dbContextFactory),
                settingsService,
                themeService,
                settings);

            await viewModel.LoadAsync();

            var mainWindow = new MainWindow
            {
                DataContext = viewModel
            };

            desktop.MainWindow = mainWindow;
            mainWindow.Show();
        }
        catch (Exception ex)
        {
            await MessageWindow.ShowAsync($"Não foi possível iniciar o aplicativo.\n\n{ex.Message}");
            desktop.Shutdown(1);
        }
    }

    private void OnDispatcherUnhandledException(object? sender, DispatcherUnhandledExceptionEventArgs e)
    {
        e.Handled = true;

        var owner = (ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
        _ = MessageWindow.ShowAsync(
            $"O aplicativo encontrou um erro, mas continuará aberto.\n\n{e.Exception.Message}",
            owner);
    }
}
