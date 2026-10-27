using System.IO;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TrackYourMeals.Data;

public sealed class TrackYourMealsDbContextFactory
{
    public TrackYourMealsDbContextFactory(string? databasePath = null)
    {
        DatabasePath = databasePath ?? GetDefaultDatabasePath();
    }

    public string DatabasePath { get; }

    public TrackYourMealsDbContext CreateDbContext()
    {
        return new TrackYourMealsDbContext(CreateOptions(DatabasePath));
    }

    public void EnsureDatabaseDirectory()
    {
        var directory = Path.GetDirectoryName(DatabasePath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    public static string GetDefaultDatabasePath()
    {
        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        return Path.Combine(localAppData, "TrackYourMeals", "track-your-meals.db");
    }

    public static DbContextOptions<TrackYourMealsDbContext> CreateOptions(string databasePath)
    {
        var connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = databasePath
        }.ToString();

        return new DbContextOptionsBuilder<TrackYourMealsDbContext>()
            .UseSqlite(connectionString)
            .Options;
    }
}

public sealed class DesignTimeTrackYourMealsDbContextFactory : IDesignTimeDbContextFactory<TrackYourMealsDbContext>
{
    public TrackYourMealsDbContext CreateDbContext(string[] args)
    {
        var databasePath = args.Length > 0
            ? args[0]
            : TrackYourMealsDbContextFactory.GetDefaultDatabasePath();

        return new TrackYourMealsDbContext(TrackYourMealsDbContextFactory.CreateOptions(databasePath));
    }
}
