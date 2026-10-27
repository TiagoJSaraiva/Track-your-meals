using Microsoft.EntityFrameworkCore;
using TrackYourMeals.Models;

namespace TrackYourMeals.Data;

public sealed class TrackYourMealsDbContext(DbContextOptions<TrackYourMealsDbContext> options) : DbContext(options)
{
    public DbSet<FoodItem> FoodItems => Set<FoodItem>();

    public DbSet<DietDay> DietDays => Set<DietDay>();

    public DbSet<FoodLogEntry> FoodLogEntries => Set<FoodLogEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FoodItem>(entity =>
        {
            entity.ToTable("FoodItems");

            entity.HasKey(food => food.Id);

            entity.Property(food => food.Name)
                .HasMaxLength(120)
                .IsRequired();

            entity.Property(food => food.UnitLabel)
                .HasMaxLength(24)
                .IsRequired();

            entity.Property(food => food.DailyTargetQuantity).HasPrecision(18, 4);
            entity.Property(food => food.CaloriesPerUnit).HasPrecision(18, 4);
            entity.Property(food => food.ProteinPerUnit).HasPrecision(18, 4);
            entity.Property(food => food.CarbsPerUnit).HasPrecision(18, 4);
            entity.Property(food => food.FatPerUnit).HasPrecision(18, 4);

            entity.HasIndex(food => food.Name);
        });

        modelBuilder.Entity<DietDay>(entity =>
        {
            entity.ToTable("DietDays");

            entity.HasKey(day => day.Id);

            entity.Property(day => day.Status)
                .HasConversion<int>();

            entity.HasIndex(day => day.Status);
            entity.HasIndex(day => day.Date);
        });

        modelBuilder.Entity<FoodLogEntry>(entity =>
        {
            entity.ToTable("FoodLogEntries");

            entity.HasKey(entry => entry.Id);

            entity.Property(entry => entry.Quantity).HasPrecision(18, 4);
            entity.Property(entry => entry.Note).HasMaxLength(300);
            entity.Property(entry => entry.CaloriesPerUnitSnapshot).HasPrecision(18, 4);
            entity.Property(entry => entry.ProteinPerUnitSnapshot).HasPrecision(18, 4);
            entity.Property(entry => entry.CarbsPerUnitSnapshot).HasPrecision(18, 4);
            entity.Property(entry => entry.FatPerUnitSnapshot).HasPrecision(18, 4);

            entity.HasOne(entry => entry.DietDay)
                .WithMany(day => day.LogEntries)
                .HasForeignKey(entry => entry.DietDayId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(entry => entry.FoodItem)
                .WithMany(food => food.LogEntries)
                .HasForeignKey(entry => entry.FoodItemId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(entry => entry.DietDayId);
            entity.HasIndex(entry => entry.FoodItemId);
            entity.HasIndex(entry => entry.LoggedAt);
        });
    }
}
