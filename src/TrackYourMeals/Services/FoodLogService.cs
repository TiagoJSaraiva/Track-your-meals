using Microsoft.EntityFrameworkCore;
using TrackYourMeals.Data;
using TrackYourMeals.Models;

namespace TrackYourMeals.Services;

public sealed class FoodLogService(TrackYourMealsDbContextFactory dbContextFactory)
{
    public async Task AddLogEntryAsync(
        int dietDayId,
        int foodItemId,
        decimal quantity,
        string? note = null,
        CancellationToken cancellationToken = default)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "A quantidade precisa ser maior que zero.");
        }

        await using var context = dbContextFactory.CreateDbContext();

        var day = await context.DietDays
            .FirstOrDefaultAsync(item => item.Id == dietDayId && item.Status == DietDayStatus.Open, cancellationToken)
            ?? throw new InvalidOperationException("Dia atual não encontrado ou já encerrado.");

        var food = await context.FoodItems
            .FirstOrDefaultAsync(item => item.Id == foodItemId, cancellationToken)
            ?? throw new InvalidOperationException("Alimento não encontrado.");

        if (!food.IsActive)
        {
            throw new InvalidOperationException("Reative o alimento antes de registrar consumo.");
        }

        var entry = new FoodLogEntry
        {
            DietDayId = day.Id,
            FoodItemId = food.Id,
            Quantity = quantity,
            LoggedAt = DateTime.Now,
            Note = string.IsNullOrWhiteSpace(note) ? null : note.Trim(),
            CaloriesPerUnitSnapshot = food.CaloriesPerUnit,
            ProteinPerUnitSnapshot = food.ProteinPerUnit,
            CarbsPerUnitSnapshot = food.CarbsPerUnit,
            FatPerUnitSnapshot = food.FatPerUnit
        };

        context.FoodLogEntries.Add(entry);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyDictionary<int, FoodConsumedTotals>> GetTotalsByFoodAsync(
        int dietDayId,
        CancellationToken cancellationToken = default)
    {
        await using var context = dbContextFactory.CreateDbContext();

        var entries = await context.FoodLogEntries
            .AsNoTracking()
            .Where(entry => entry.DietDayId == dietDayId)
            .ToListAsync(cancellationToken);

        return entries
            .GroupBy(entry => entry.FoodItemId)
            .ToDictionary(
                group => group.Key,
                group => new FoodConsumedTotals(
                    group.Sum(entry => entry.Quantity),
                    group.Sum(entry => entry.Quantity * entry.CaloriesPerUnitSnapshot),
                    group.Sum(entry => entry.Quantity * entry.ProteinPerUnitSnapshot),
                    group.Sum(entry => entry.Quantity * entry.CarbsPerUnitSnapshot),
                    group.Sum(entry => entry.Quantity * entry.FatPerUnitSnapshot)));
    }

    public async Task<IReadOnlyList<FoodLogEntry>> GetEntriesForDayAsync(
        int dietDayId,
        CancellationToken cancellationToken = default)
    {
        await using var context = dbContextFactory.CreateDbContext();

        return await context.FoodLogEntries
            .AsNoTracking()
            .Include(entry => entry.FoodItem)
            .Where(entry => entry.DietDayId == dietDayId)
            .OrderByDescending(entry => entry.LoggedAt)
            .ToListAsync(cancellationToken);
    }
}
