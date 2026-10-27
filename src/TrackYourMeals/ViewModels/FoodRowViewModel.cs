using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrackYourMeals.Models;
using TrackYourMeals.Services;

namespace TrackYourMeals.ViewModels;

public sealed class FoodRowViewModel : ObservableObject
{
    private readonly Func<FoodRowViewModel, Task> addLogAsync;
    private readonly Func<FoodRowViewModel, Task> saveFoodAsync;
    private readonly Func<FoodRowViewModel, Task> toggleActiveAsync;

    private string name;
    private string unitLabel;
    private decimal dailyTargetQuantity;
    private decimal caloriesPerUnit;
    private decimal proteinPerUnit;
    private decimal carbsPerUnit;
    private decimal fatPerUnit;
    private string dailyTargetQuantityText;
    private string caloriesPerUnitText;
    private string proteinPerUnitText;
    private string carbsPerUnitText;
    private string fatPerUnitText;
    private bool isActive;
    private decimal consumedQuantity;
    private decimal consumedCalories;
    private decimal consumedProtein;
    private decimal consumedCarbs;
    private decimal consumedFat;
    private string logQuantityText = string.Empty;

    public FoodRowViewModel(
        FoodItem food,
        FoodConsumedTotals totals,
        Func<FoodRowViewModel, Task> addLogAsync,
        Func<FoodRowViewModel, Task> saveFoodAsync,
        Func<FoodRowViewModel, Task> toggleActiveAsync)
    {
        Id = food.Id;
        name = food.Name;
        unitLabel = food.UnitLabel;
        dailyTargetQuantity = food.DailyTargetQuantity;
        caloriesPerUnit = food.CaloriesPerUnit;
        proteinPerUnit = food.ProteinPerUnit;
        carbsPerUnit = food.CarbsPerUnit;
        fatPerUnit = food.FatPerUnit;
        dailyTargetQuantityText = DecimalInput.Format(food.DailyTargetQuantity);
        caloriesPerUnitText = DecimalInput.Format(food.CaloriesPerUnit);
        proteinPerUnitText = DecimalInput.Format(food.ProteinPerUnit);
        carbsPerUnitText = DecimalInput.Format(food.CarbsPerUnit);
        fatPerUnitText = DecimalInput.Format(food.FatPerUnit);
        isActive = food.IsActive;

        this.addLogAsync = addLogAsync;
        this.saveFoodAsync = saveFoodAsync;
        this.toggleActiveAsync = toggleActiveAsync;

        AddLogCommand = new AsyncRelayCommand(AddLogInternalAsync, CanAddLog);
        SaveCommand = new AsyncRelayCommand(SaveFoodInternalAsync);
        ToggleActiveCommand = new AsyncRelayCommand(ToggleActiveInternalAsync);

        SetConsumedTotals(totals);
    }

    public int Id { get; }

    public string Name
    {
        get => name;
        set => SetProperty(ref name, value);
    }

    public string UnitLabel
    {
        get => unitLabel;
        set => SetProperty(ref unitLabel, value);
    }

    public decimal DailyTargetQuantity
    {
        get => dailyTargetQuantity;
    }

    public string DailyTargetQuantityText
    {
        get => dailyTargetQuantityText;
        set
        {
            if (SetProperty(ref dailyTargetQuantityText, value) &&
                DecimalInput.TryParseNonNegative(value, out var parsed) &&
                SetProperty(ref dailyTargetQuantity, parsed, nameof(DailyTargetQuantity)))
            {
                OnPropertyChanged(nameof(RemainingQuantity));
            }
        }
    }

    public decimal CaloriesPerUnit
    {
        get => caloriesPerUnit;
    }

    public string CaloriesPerUnitText
    {
        get => caloriesPerUnitText;
        set
        {
            if (SetProperty(ref caloriesPerUnitText, value) &&
                DecimalInput.TryParseNonNegative(value, out var parsed))
            {
                SetProperty(ref caloriesPerUnit, parsed, nameof(CaloriesPerUnit));
            }
        }
    }

    public decimal ProteinPerUnit
    {
        get => proteinPerUnit;
    }

    public string ProteinPerUnitText
    {
        get => proteinPerUnitText;
        set
        {
            if (SetProperty(ref proteinPerUnitText, value) &&
                DecimalInput.TryParseNonNegative(value, out var parsed))
            {
                SetProperty(ref proteinPerUnit, parsed, nameof(ProteinPerUnit));
            }
        }
    }

    public decimal CarbsPerUnit
    {
        get => carbsPerUnit;
    }

    public string CarbsPerUnitText
    {
        get => carbsPerUnitText;
        set
        {
            if (SetProperty(ref carbsPerUnitText, value) &&
                DecimalInput.TryParseNonNegative(value, out var parsed))
            {
                SetProperty(ref carbsPerUnit, parsed, nameof(CarbsPerUnit));
            }
        }
    }

    public decimal FatPerUnit
    {
        get => fatPerUnit;
    }

    public string FatPerUnitText
    {
        get => fatPerUnitText;
        set
        {
            if (SetProperty(ref fatPerUnitText, value) &&
                DecimalInput.TryParseNonNegative(value, out var parsed))
            {
                SetProperty(ref fatPerUnit, parsed, nameof(FatPerUnit));
            }
        }
    }

    public bool IsActive
    {
        get => isActive;
        set
        {
            if (SetProperty(ref isActive, value))
            {
                OnPropertyChanged(nameof(StatusText));
                OnPropertyChanged(nameof(ToggleActiveText));
                AddLogCommand.NotifyCanExecuteChanged();
            }
        }
    }

    public decimal ConsumedQuantity
    {
        get => consumedQuantity;
        private set
        {
            if (SetProperty(ref consumedQuantity, value))
            {
                OnPropertyChanged(nameof(RemainingQuantity));
            }
        }
    }

    public decimal RemainingQuantity => DailyTargetQuantity - ConsumedQuantity;

    public decimal ConsumedCalories
    {
        get => consumedCalories;
        private set => SetProperty(ref consumedCalories, value);
    }

    public decimal ConsumedProtein
    {
        get => consumedProtein;
        private set => SetProperty(ref consumedProtein, value);
    }

    public decimal ConsumedCarbs
    {
        get => consumedCarbs;
        private set => SetProperty(ref consumedCarbs, value);
    }

    public decimal ConsumedFat
    {
        get => consumedFat;
        private set => SetProperty(ref consumedFat, value);
    }

    public string LogQuantityText
    {
        get => logQuantityText;
        set => SetProperty(ref logQuantityText, value);
    }

    public string StatusText => IsActive ? "Ativo" : "Inativo";

    public string ToggleActiveText => IsActive ? "Desativar" : "Reativar";

    public AsyncRelayCommand AddLogCommand { get; }

    public AsyncRelayCommand SaveCommand { get; }

    public AsyncRelayCommand ToggleActiveCommand { get; }

    public void SetConsumedTotals(FoodConsumedTotals totals)
    {
        ConsumedQuantity = totals.Quantity;
        ConsumedCalories = totals.Calories;
        ConsumedProtein = totals.Protein;
        ConsumedCarbs = totals.Carbs;
        ConsumedFat = totals.Fat;
    }

    public bool TryGetEditableValues(
        out decimal parsedDailyTargetQuantity,
        out decimal parsedCaloriesPerUnit,
        out decimal parsedProteinPerUnit,
        out decimal parsedCarbsPerUnit,
        out decimal parsedFatPerUnit,
        out string errorMessage)
    {
        if (!DecimalInput.TryParseNonNegative(DailyTargetQuantityText, out parsedDailyTargetQuantity))
        {
            return Fail("Meta diária precisa ser um número maior ou igual a zero.", out parsedCaloriesPerUnit, out parsedProteinPerUnit, out parsedCarbsPerUnit, out parsedFatPerUnit, out errorMessage);
        }

        if (!DecimalInput.TryParseNonNegative(CaloriesPerUnitText, out parsedCaloriesPerUnit))
        {
            return Fail("Calorias por unidade precisam ser um número maior ou igual a zero.", out parsedProteinPerUnit, out parsedCarbsPerUnit, out parsedFatPerUnit, out errorMessage);
        }

        if (!DecimalInput.TryParseNonNegative(ProteinPerUnitText, out parsedProteinPerUnit))
        {
            return Fail("Proteína por unidade precisa ser um número maior ou igual a zero.", out parsedCarbsPerUnit, out parsedFatPerUnit, out errorMessage);
        }

        if (!DecimalInput.TryParseNonNegative(CarbsPerUnitText, out parsedCarbsPerUnit))
        {
            return Fail("Carboidrato por unidade precisa ser um número maior ou igual a zero.", out parsedFatPerUnit, out errorMessage);
        }

        if (!DecimalInput.TryParseNonNegative(FatPerUnitText, out parsedFatPerUnit))
        {
            errorMessage = "Gordura por unidade precisa ser um número maior ou igual a zero.";
            return false;
        }

        errorMessage = string.Empty;
        return true;
    }

    private static bool Fail(
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

    private static bool Fail(
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

    private static bool Fail(
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

    private static bool Fail(
        string message,
        out decimal value1,
        out string errorMessage)
    {
        value1 = 0;
        errorMessage = message;
        return false;
    }

    private bool CanAddLog()
    {
        return IsActive;
    }

    private Task AddLogInternalAsync()
    {
        return addLogAsync(this);
    }

    private Task SaveFoodInternalAsync()
    {
        return saveFoodAsync(this);
    }

    private Task ToggleActiveInternalAsync()
    {
        return toggleActiveAsync(this);
    }
}
