using Microsoft.EntityFrameworkCore;
using TrackYourMeals.Data;
using TrackYourMeals.Models;

namespace TrackYourMeals.Services;

public sealed class DietDayService(TrackYourMealsDbContextFactory dbContextFactory)
{
    public async Task<DietDay> GetOrCreateOpenDayAsync(CancellationToken cancellationToken = default)
    {
        await using var context = dbContextFactory.CreateDbContext();

        var openDay = await context.DietDays
            .AsNoTracking()
            .Where(day => day.Status == DietDayStatus.Open)
            .OrderByDescending(day => day.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (openDay is not null)
        {
            return openDay;
        }

        var now = DateTime.Now;
        var newDay = new DietDay
        {
            Date = now.Date,
            Status = DietDayStatus.Open,
            CreatedAt = now
        };

        context.DietDays.Add(newDay);
        await context.SaveChangesAsync(cancellationToken);

        return newDay;
    }

    public async Task<DietDay> CloseOpenDaysAndStartNewAsync(CancellationToken cancellationToken = default)
    {
        await using var context = dbContextFactory.CreateDbContext();

        var now = DateTime.Now;
        var openDays = await context.DietDays
            .Where(day => day.Status == DietDayStatus.Open)
            .ToListAsync(cancellationToken);

        foreach (var openDay in openDays)
        {
            openDay.Status = DietDayStatus.Closed;
            openDay.ClosedAt = now;
        }

        var newDay = new DietDay
        {
            Date = now.Date,
            Status = DietDayStatus.Open,
            CreatedAt = now
        };

        context.DietDays.Add(newDay);
        await context.SaveChangesAsync(cancellationToken);

        return newDay;
    }
}
