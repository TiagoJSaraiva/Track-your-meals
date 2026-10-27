namespace TrackYourMeals.Models;

public sealed class DietDay
{
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public DietDayStatus Status { get; set; } = DietDayStatus.Open;

    public DateTime CreatedAt { get; set; }

    public DateTime? ClosedAt { get; set; }

    public ICollection<FoodLogEntry> LogEntries { get; } = new List<FoodLogEntry>();
}
