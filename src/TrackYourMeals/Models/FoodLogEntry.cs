namespace TrackYourMeals.Models;

public sealed class FoodLogEntry
{
    public int Id { get; set; }

    public int DietDayId { get; set; }

    public DietDay? DietDay { get; set; }

    public int FoodItemId { get; set; }

    public FoodItem? FoodItem { get; set; }

    public decimal Quantity { get; set; }

    public DateTime LoggedAt { get; set; }

    public string? Note { get; set; }

    public decimal CaloriesPerUnitSnapshot { get; set; }

    public decimal ProteinPerUnitSnapshot { get; set; }

    public decimal CarbsPerUnitSnapshot { get; set; }

    public decimal FatPerUnitSnapshot { get; set; }
}
