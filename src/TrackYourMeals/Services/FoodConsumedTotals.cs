namespace TrackYourMeals.Services;

public sealed record FoodConsumedTotals(
    decimal Quantity,
    decimal Calories,
    decimal Protein,
    decimal Carbs,
    decimal Fat)
{
    public static FoodConsumedTotals Empty { get; } = new(0, 0, 0, 0, 0);
}
