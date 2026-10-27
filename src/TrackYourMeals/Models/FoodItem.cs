namespace TrackYourMeals.Models;

public sealed class FoodItem
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string UnitLabel { get; set; } = "g";

    public decimal DailyTargetQuantity { get; set; }

    public decimal CaloriesPerUnit { get; set; }

    public decimal ProteinPerUnit { get; set; }

    public decimal CarbsPerUnit { get; set; }

    public decimal FatPerUnit { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<FoodLogEntry> LogEntries { get; } = new List<FoodLogEntry>();
}
