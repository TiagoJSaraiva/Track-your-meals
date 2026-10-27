using System.Collections.ObjectModel;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrackYourMeals.Models;
using TrackYourMeals.Services;

namespace TrackYourMeals.ViewModels;

public sealed class MainViewModel : ObservableObject
{
    private const double MinZoomLevel = 0.5;
    private const double MaxZoomLevel = 2.0;
    private const double ZoomStep = 0.1;

    private readonly DietDayService dietDayService;
    private readonly FoodService foodService;
    private readonly FoodLogService foodLogService;
    private readonly SettingsService settingsService;
    private readonly ThemeService themeService;
    private readonly AppSettings settings;

    private DietDay? currentDay;
    private bool isBusy;
    private string statusMessage = string.Empty;
    private string errorMessage = string.Empty;
    private string newFoodName = string.Empty;
    private string newFoodUnitLabel = "g";
    private string newFoodDailyTargetQuantityText = "0";
    private string newFoodCaloriesPerUnitText = "0";
    private string newFoodProteinPerUnitText = "0";
    private string newFoodCarbsPerUnitText = "0";
    private string newFoodFatPerUnitText = "0";
    private decimal totalCalories;
    private decimal totalProtein;
    private decimal totalCarbs;
    private decimal totalFat;

    public MainViewModel(
        DietDayService dietDayService,
        FoodService foodService,
        FoodLogService foodLogService,
        SettingsService settingsService,
        ThemeService themeService,
        AppSettings settings)
    {
        this.dietDayService = dietDayService;
        this.foodService = foodService;
        this.foodLogService = foodLogService;
        this.settingsService = settingsService;
        this.themeService = themeService;
        this.settings = settings;

        LoadCommand = new AsyncRelayCommand(LoadAsync);
        AddFoodCommand = new AsyncRelayCommand(AddFoodAsync, CanRunCommand);
        CloseDayCommand = new AsyncRelayCommand(CloseDayAsync, CanRunCommand);
        ToggleThemeCommand = new RelayCommand(ToggleTheme);
        ZoomInCommand = new RelayCommand(() => ZoomLevel += ZoomStep, () => ZoomLevel < MaxZoomLevel);
        ZoomOutCommand = new RelayCommand(() => ZoomLevel -= ZoomStep, () => ZoomLevel > MinZoomLevel);
        ResetZoomCommand = new RelayCommand(() => ZoomLevel = 1.0);

        settings.ZoomLevel = ClampZoomLevel(settings.ZoomLevel);
    }

    public ObservableCollection<FoodRowViewModel> Foods { get; } = new();

    public ObservableCollection<FoodLogEntryViewModel> RecentEntries { get; } = new();

    public AsyncRelayCommand LoadCommand { get; }

    public AsyncRelayCommand AddFoodCommand { get; }

    public AsyncRelayCommand CloseDayCommand { get; }

    public RelayCommand ToggleThemeCommand { get; }

    public string ThemeToggleText => themeService.CurrentTheme == AppTheme.Dark
        ? "Modo claro"
        : "Modo escuro";

    public RelayCommand ZoomInCommand { get; }

    public RelayCommand ZoomOutCommand { get; }

    public RelayCommand ResetZoomCommand { get; }

    public double ZoomLevel
    {
        get => settings.ZoomLevel;
        set
        {
            var zoomLevel = ClampZoomLevel(value);
            if (settings.ZoomLevel == zoomLevel)
            {
                return;
            }

            settings.ZoomLevel = zoomLevel;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ZoomPercentLabel));
            ZoomInCommand.NotifyCanExecuteChanged();
            ZoomOutCommand.NotifyCanExecuteChanged();
            SaveSettings();
        }
    }

    public string ZoomPercentLabel => $"{ZoomLevel * 100:0}%";

    public bool IsBusy
    {
        get => isBusy;
        private set
        {
            if (SetProperty(ref isBusy, value))
            {
                AddFoodCommand.NotifyCanExecuteChanged();
                CloseDayCommand.NotifyCanExecuteChanged();
            }
        }
    }

    public string CurrentDayLabel => currentDay is null
        ? "Dia atual: carregando"
        : $"Dia atual: {currentDay.Date:dd/MM/yyyy}";

    public string DatabaseStatusLabel => "Dados locais em SQLite";

    public string StatusMessage
    {
        get => statusMessage;
        private set => SetProperty(ref statusMessage, value);
    }

    public string ErrorMessage
    {
        get => errorMessage;
        private set => SetProperty(ref errorMessage, value);
    }

    public string NewFoodName
    {
        get => newFoodName;
        set => SetProperty(ref newFoodName, value);
    }

    public string NewFoodUnitLabel
    {
        get => newFoodUnitLabel;
        set => SetProperty(ref newFoodUnitLabel, value);
    }

    public string NewFoodDailyTargetQuantityText
    {
        get => newFoodDailyTargetQuantityText;
        set => SetProperty(ref newFoodDailyTargetQuantityText, value);
    }

    public string NewFoodCaloriesPerUnitText
    {
        get => newFoodCaloriesPerUnitText;
        set => SetProperty(ref newFoodCaloriesPerUnitText, value);
    }

    public string NewFoodProteinPerUnitText
    {
        get => newFoodProteinPerUnitText;
        set => SetProperty(ref newFoodProteinPerUnitText, value);
    }

    public string NewFoodCarbsPerUnitText
    {
        get => newFoodCarbsPerUnitText;
        set => SetProperty(ref newFoodCarbsPerUnitText, value);
    }

    public string NewFoodFatPerUnitText
    {
        get => newFoodFatPerUnitText;
        set => SetProperty(ref newFoodFatPerUnitText, value);
    }

    public decimal TotalCalories
    {
        get => totalCalories;
        private set => SetProperty(ref totalCalories, value);
    }

    public decimal TotalProtein
    {
        get => totalProtein;
        private set => SetProperty(ref totalProtein, value);
    }

    public decimal TotalCarbs
    {
        get => totalCarbs;
        private set => SetProperty(ref totalCarbs, value);
    }

    public decimal TotalFat
    {
        get => totalFat;
        private set => SetProperty(ref totalFat, value);
    }

    public async Task LoadAsync()
    {
        await RunWithBusyStateAsync(async () =>
        {
            currentDay = await dietDayService.GetOrCreateOpenDayAsync();
            OnPropertyChanged(nameof(CurrentDayLabel));

            await RefreshAsync();
            StatusMessage = "Pronto para registrar o dia.";
        });
    }

    private async Task AddFoodAsync()
    {
        await RunWithBusyStateAsync(async () =>
        {
            if (!TryParseNewFoodNumbers(
                    out var dailyTargetQuantity,
                    out var caloriesPerUnit,
                    out var proteinPerUnit,
                    out var carbsPerUnit,
                    out var fatPerUnit,
                    out var parseErrorMessage))
            {
                ErrorMessage = parseErrorMessage;
                return;
            }

            var validationMessage = ValidateFood(
                NewFoodName,
                NewFoodUnitLabel,
                dailyTargetQuantity,
                caloriesPerUnit,
                proteinPerUnit,
                carbsPerUnit,
                fatPerUnit);

            if (validationMessage is not null)
            {
                ErrorMessage = validationMessage;
                return;
            }

            await foodService.AddFoodAsync(
                NewFoodName,
                NewFoodUnitLabel,
                dailyTargetQuantity,
                caloriesPerUnit,
                proteinPerUnit,
                carbsPerUnit,
                fatPerUnit);

            ResetNewFoodForm();
            await RefreshAsync();
            StatusMessage = "Alimento adicionado.";
        });
    }

    private async Task SaveFoodAsync(FoodRowViewModel row)
    {
        await RunWithBusyStateAsync(async () =>
        {
            if (!row.TryGetEditableValues(
                    out var dailyTargetQuantity,
                    out var caloriesPerUnit,
                    out var proteinPerUnit,
                    out var carbsPerUnit,
                    out var fatPerUnit,
                    out var parseErrorMessage))
            {
                ErrorMessage = parseErrorMessage;
                return;
            }

            var validationMessage = ValidateFood(
                row.Name,
                row.UnitLabel,
                dailyTargetQuantity,
                caloriesPerUnit,
                proteinPerUnit,
                carbsPerUnit,
                fatPerUnit);

            if (validationMessage is not null)
            {
                ErrorMessage = validationMessage;
                return;
            }

            await foodService.UpdateFoodAsync(
                row.Id,
                row.Name,
                row.UnitLabel,
                dailyTargetQuantity,
                caloriesPerUnit,
                proteinPerUnit,
                carbsPerUnit,
                fatPerUnit);

            await RefreshAsync();
            StatusMessage = "Alimento salvo.";
        });
    }

    private async Task ToggleActiveAsync(FoodRowViewModel row)
    {
        await RunWithBusyStateAsync(async () =>
        {
            await foodService.SetFoodActiveAsync(row.Id, !row.IsActive);
            await RefreshAsync();
            StatusMessage = row.IsActive ? "Alimento desativado." : "Alimento reativado.";
        });
    }

    private async Task AddLogAsync(FoodRowViewModel row)
    {
        await RunWithBusyStateAsync(async () =>
        {
            if (currentDay is null)
            {
                ErrorMessage = "Dia atual ainda nao foi carregado.";
                return;
            }

            if (!DecimalInput.TryParsePositive(row.LogQuantityText, out var quantity))
            {
                ErrorMessage = "Informe uma quantidade valida maior que zero.";
                return;
            }

            await foodLogService.AddLogEntryAsync(currentDay.Id, row.Id, quantity);
            await RefreshAsync();
            StatusMessage = "Consumo registrado.";
        });
    }

    private async Task CloseDayAsync()
    {
        await RunWithBusyStateAsync(async () =>
        {
            currentDay = await dietDayService.CloseOpenDaysAndStartNewAsync();
            OnPropertyChanged(nameof(CurrentDayLabel));

            await RefreshAsync();
            StatusMessage = "Dia encerrado. Um novo dia foi iniciado.";
        });
    }

    private void ToggleTheme()
    {
        var theme = themeService.CurrentTheme == AppTheme.Dark ? AppTheme.Light : AppTheme.Dark;
        themeService.Apply(theme);
        OnPropertyChanged(nameof(ThemeToggleText));

        settings.Theme = theme;
        SaveSettings();
    }

    private static double ClampZoomLevel(double zoomLevel)
    {
        if (!double.IsFinite(zoomLevel))
        {
            return 1.0;
        }

        return Math.Round(Math.Clamp(zoomLevel, MinZoomLevel, MaxZoomLevel), 2);
    }

    private void SaveSettings()
    {
        try
        {
            settingsService.Save(settings);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            ErrorMessage = $"Não foi possível salvar as preferências: {ex.Message}";
        }
    }

    private async Task RefreshAsync()
    {
        if (currentDay is null)
        {
            return;
        }

        var foods = await foodService.GetFoodItemsAsync(includeInactive: true);
        var totalsByFood = await foodLogService.GetTotalsByFoodAsync(currentDay.Id);
        var entries = await foodLogService.GetEntriesForDayAsync(currentDay.Id);

        Foods.Clear();
        foreach (var food in foods)
        {
            totalsByFood.TryGetValue(food.Id, out var totals);
            Foods.Add(new FoodRowViewModel(
                food,
                totals ?? FoodConsumedTotals.Empty,
                AddLogAsync,
                SaveFoodAsync,
                ToggleActiveAsync));
        }

        RecentEntries.Clear();
        foreach (var entry in entries.Take(12))
        {
            RecentEntries.Add(new FoodLogEntryViewModel(entry));
        }

        TotalCalories = Foods.Sum(food => food.ConsumedCalories);
        TotalProtein = Foods.Sum(food => food.ConsumedProtein);
        TotalCarbs = Foods.Sum(food => food.ConsumedCarbs);
        TotalFat = Foods.Sum(food => food.ConsumedFat);
    }

    private async Task RunWithBusyStateAsync(Func<Task> action)
    {
        if (IsBusy)
        {
            return;
        }

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;
            StatusMessage = string.Empty;
            await action();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool CanRunCommand()
    {
        return !IsBusy;
    }

    private void ResetNewFoodForm()
    {
        NewFoodName = string.Empty;
        NewFoodUnitLabel = "g";
        NewFoodDailyTargetQuantityText = "0";
        NewFoodCaloriesPerUnitText = "0";
        NewFoodProteinPerUnitText = "0";
        NewFoodCarbsPerUnitText = "0";
        NewFoodFatPerUnitText = "0";
    }

    private bool TryParseNewFoodNumbers(
        out decimal dailyTargetQuantity,
        out decimal caloriesPerUnit,
        out decimal proteinPerUnit,
        out decimal carbsPerUnit,
        out decimal fatPerUnit,
        out string parseErrorMessage)
    {
        if (!DecimalInput.TryParseNonNegative(NewFoodDailyTargetQuantityText, out dailyTargetQuantity))
        {
            return FailNumberParse("Meta diaria precisa ser um numero maior ou igual a zero.", out caloriesPerUnit, out proteinPerUnit, out carbsPerUnit, out fatPerUnit, out parseErrorMessage);
        }

        if (!DecimalInput.TryParseNonNegative(NewFoodCaloriesPerUnitText, out caloriesPerUnit))
        {
            return FailNumberParse("Calorias por unidade precisam ser um numero maior ou igual a zero.", out proteinPerUnit, out carbsPerUnit, out fatPerUnit, out parseErrorMessage);
        }

        if (!DecimalInput.TryParseNonNegative(NewFoodProteinPerUnitText, out proteinPerUnit))
        {
            return FailNumberParse("Proteina por unidade precisa ser um numero maior ou igual a zero.", out carbsPerUnit, out fatPerUnit, out parseErrorMessage);
        }

        if (!DecimalInput.TryParseNonNegative(NewFoodCarbsPerUnitText, out carbsPerUnit))
        {
            return FailNumberParse("Carboidrato por unidade precisa ser um numero maior ou igual a zero.", out fatPerUnit, out parseErrorMessage);
        }

        if (!DecimalInput.TryParseNonNegative(NewFoodFatPerUnitText, out fatPerUnit))
        {
            parseErrorMessage = "Gordura por unidade precisa ser um numero maior ou igual a zero.";
            return false;
        }

        parseErrorMessage = string.Empty;
        return true;
    }

    private static string? ValidateFood(
        string name,
        string unitLabel,
        decimal dailyTargetQuantity,
        decimal caloriesPerUnit,
        decimal proteinPerUnit,
        decimal carbsPerUnit,
        decimal fatPerUnit)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return "Informe o nome do alimento.";
        }

        if (string.IsNullOrWhiteSpace(unitLabel))
        {
            return "Informe a unidade de medida.";
        }

        if (dailyTargetQuantity < 0)
        {
            return "A meta diaria nao pode ser negativa.";
        }

        if (caloriesPerUnit < 0 || proteinPerUnit < 0 || carbsPerUnit < 0 || fatPerUnit < 0)
        {
            return "Calorias e macros nao podem ser negativos.";
        }

        return null;
    }

    private static bool FailNumberParse(
        string message,
        out decimal value1,
        out decimal value2,
        out decimal value3,
        out decimal value4,
        out string errorMessage)
    {
        value1 = 0;
        value2 = 0;
        value3 = 0;
        value4 = 0;
        errorMessage = message;
        return false;
    }

    private static bool FailNumberParse(
        string message,
        out decimal value1,
        out decimal value2,
        out decimal value3,
        out string errorMessage)
    {
        value1 = 0;
        value2 = 0;
        value3 = 0;
        errorMessage = message;
        return false;
    }

    private static bool FailNumberParse(
        string message,
        out decimal value1,
        out decimal value2,
        out string errorMessage)
    {
        value1 = 0;
        value2 = 0;
        errorMessage = message;
        return false;
    }

    private static bool FailNumberParse(
        string message,
        out decimal value1,
        out string errorMessage)
    {
        value1 = 0;
        errorMessage = message;
        return false;
    }
}
