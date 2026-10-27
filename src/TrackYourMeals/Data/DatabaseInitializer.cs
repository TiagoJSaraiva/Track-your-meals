using Microsoft.EntityFrameworkCore;

namespace TrackYourMeals.Data;

public sealed class DatabaseInitializer(TrackYourMealsDbContextFactory dbContextFactory)
{
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        dbContextFactory.EnsureDatabaseDirectory();

        await using var context = dbContextFactory.CreateDbContext();
        await context.Database.MigrateAsync(cancellationToken);
    }
}
