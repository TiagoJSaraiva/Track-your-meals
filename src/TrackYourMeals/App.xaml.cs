using System.Windows;
using System.Windows.Threading;
using TrackYourMeals.Data;
using TrackYourMeals.Services;
using TrackYourMeals.ViewModels;

namespace TrackYourMeals;

public partial class App : Application
{
    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        DispatcherUnhandledException += OnDispatcherUnhandledException;

        var dbContextFactory = new TrackYourMealsDbContextFactory();
        var settingsService = new SettingsService();
        var themeService = new ThemeService();

        var settings = settingsService.Load();
        themeService.Apply(settings.Theme ?? ThemeService.GetSystemTheme());

        // Aplica a barra de título do tema em toda janela que for aberta.
        EventManager.RegisterClassHandler(
            typeof(Window),
            FrameworkElement.LoadedEvent,
            new RoutedEventHandler((sender, _) => themeService.ApplyTitleBar((Window)sender)));

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

            MainWindow = mainWindow;
            mainWindow.Show();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Não foi possível iniciar o aplicativo.\n\n{ex.Message}",
                "Track Your Meals",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            Shutdown(1);
        }
    }

    private static void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        MessageBox.Show(
            $"O aplicativo encontrou um erro, mas continuara aberto.\n\n{e.Exception.Message}",
            "Track Your Meals",
            MessageBoxButton.OK,
            MessageBoxImage.Error);

        e.Handled = true;
    }
}
