using TrackYourMeals.Models;

namespace TrackYourMeals.ViewModels;

public sealed class FoodLogEntryViewModel
{
    public FoodLogEntryViewModel(FoodLogEntry entry)
    {
        var unitLabel = entry.FoodItem?.UnitLabel ?? string.Empty;

        LoggedAtLabel = entry.LoggedAt.ToString("HH:mm");
        FoodName = entry.FoodItem?.Name ?? $"Alimento #{entry.FoodItemId}";
        QuantityLabel = $"{entry.Quantity:N2} {unitLabel}".Trim();
        Calories = entry.Quantity * entry.CaloriesPerUnitSnapshot;
        Protein = entry.Quantity * entry.ProteinPerUnitSnapshot;
        Carbs = entry.Quantity * entry.CarbsPerUnitSnapshot;
        Fat = entry.Quantity * entry.FatPerUnitSnapshot;
        Note = entry.Note ?? string.Empty;
    }

    public string LoggedAtLabel { get; }

    public string FoodName { get; }

    public string QuantityLabel { get; }

    public decimal Calories { get; }

    public decimal Protein { get; }

    public decimal Carbs { get; }

    public decimal Fat { get; }

    public string Note { get; }
}
