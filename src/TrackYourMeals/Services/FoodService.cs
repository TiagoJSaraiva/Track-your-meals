using Microsoft.EntityFrameworkCore;
using TrackYourMeals.Data;
using TrackYourMeals.Models;

namespace TrackYourMeals.Services;

public sealed class FoodService(TrackYourMealsDbContextFactory dbContextFactory)
{
    public async Task<IReadOnlyList<FoodItem>> GetFoodItemsAsync(bool includeInactive = true, CancellationToken cancellationToken = default)
    {
        await using var context = dbContextFactory.CreateDbContext();

        var query = context.FoodItems.AsNoTracking();
        if (!includeInactive)
        {
            query = query.Where(food => food.IsActive);
        }

        return await query
            .OrderByDescending(food => food.IsActive)
            .ThenBy(food => food.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<FoodItem> AddFoodAsync(
        string name,
        string unitLabel,
        decimal dailyTargetQuantity,
        decimal caloriesPerUnit,
        decimal proteinPerUnit,
        decimal carbsPerUnit,
        decimal fatPerUnit,
        CancellationToken cancellationToken = default)
    {
        await using var context = dbContextFactory.CreateDbContext();

        var food = new FoodItem
        {
            Name = name.Trim(),
            UnitLabel = unitLabel.Trim(),
            DailyTargetQuantity = dailyTargetQuantity,
            CaloriesPerUnit = caloriesPerUnit,
            ProteinPerUnit = proteinPerUnit,
            CarbsPerUnit = carbsPerUnit,
            FatPerUnit = fatPerUnit,
            IsActive = true
        };

        context.FoodItems.Add(food);
        await context.SaveChangesAsync(cancellationToken);

        return food;
    }

    public async Task UpdateFoodAsync(
        int id,
        string name,
        string unitLabel,
        decimal dailyTargetQuantity,
        decimal caloriesPerUnit,
        decimal proteinPerUnit,
        decimal carbsPerUnit,
        decimal fatPerUnit,
        CancellationToken cancellationToken = default)
    {
        await using var context = dbContextFactory.CreateDbContext();

        var food = await context.FoodItems.FirstOrDefaultAsync(item => item.Id == id, cancellationToken)
            ?? throw new InvalidOperationException("Alimento não encontrado.");

        food.Name = name.Trim();
        food.UnitLabel = unitLabel.Trim();
        food.DailyTargetQuantity = dailyTargetQuantity;
        food.CaloriesPerUnit = caloriesPerUnit;
        food.ProteinPerUnit = proteinPerUnit;
        food.CarbsPerUnit = carbsPerUnit;
        food.FatPerUnit = fatPerUnit;

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task SetFoodActiveAsync(int id, bool isActive, CancellationToken cancellationToken = default)
    {
        await using var context = dbContextFactory.CreateDbContext();

        var food = await context.FoodItems.FirstOrDefaultAsync(item => item.Id == id, cancellationToken)
            ?? throw new InvalidOperationException("Alimento não encontrado.");

        food.IsActive = isActive;
        await context.SaveChangesAsync(cancellationToken);
    }
}
