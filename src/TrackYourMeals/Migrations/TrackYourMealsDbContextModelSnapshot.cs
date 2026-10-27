using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TrackYourMeals.Data;

#nullable disable

namespace TrackYourMeals.Migrations;

[DbContext(typeof(TrackYourMealsDbContext))]
partial class TrackYourMealsDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder.HasAnnotation("ProductVersion", "8.0.18");

        modelBuilder.Entity("TrackYourMeals.Models.DietDay", b =>
        {
            b.Property<int>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("INTEGER");

            b.Property<DateTime?>("ClosedAt")
                .HasColumnType("TEXT");

            b.Property<DateTime>("CreatedAt")
                .HasColumnType("TEXT");

            b.Property<DateTime>("Date")
                .HasColumnType("TEXT");

            b.Property<int>("Status")
                .HasColumnType("INTEGER");

            b.HasKey("Id");

            b.HasIndex("Date");

            b.HasIndex("Status");

            b.ToTable("DietDays");
        });

        modelBuilder.Entity("TrackYourMeals.Models.FoodItem", b =>
        {
            b.Property<int>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("INTEGER");

            b.Property<decimal>("CaloriesPerUnit")
                .HasPrecision(18, 4)
                .HasColumnType("TEXT");

            b.Property<decimal>("CarbsPerUnit")
                .HasPrecision(18, 4)
                .HasColumnType("TEXT");

            b.Property<decimal>("DailyTargetQuantity")
                .HasPrecision(18, 4)
                .HasColumnType("TEXT");

            b.Property<decimal>("FatPerUnit")
                .HasPrecision(18, 4)
                .HasColumnType("TEXT");

            b.Property<bool>("IsActive")
                .HasColumnType("INTEGER");

            b.Property<string>("Name")
                .IsRequired()
                .HasMaxLength(120)
                .HasColumnType("TEXT");

            b.Property<decimal>("ProteinPerUnit")
                .HasPrecision(18, 4)
                .HasColumnType("TEXT");

            b.Property<string>("UnitLabel")
                .IsRequired()
                .HasMaxLength(24)
                .HasColumnType("TEXT");

            b.HasKey("Id");

            b.HasIndex("Name");

            b.ToTable("FoodItems");
        });

        modelBuilder.Entity("TrackYourMeals.Models.FoodLogEntry", b =>
        {
            b.Property<int>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("INTEGER");

            b.Property<decimal>("CaloriesPerUnitSnapshot")
                .HasPrecision(18, 4)
                .HasColumnType("TEXT");

            b.Property<decimal>("CarbsPerUnitSnapshot")
                .HasPrecision(18, 4)
                .HasColumnType("TEXT");

            b.Property<int>("DietDayId")
                .HasColumnType("INTEGER");

            b.Property<decimal>("FatPerUnitSnapshot")
                .HasPrecision(18, 4)
                .HasColumnType("TEXT");

            b.Property<int>("FoodItemId")
                .HasColumnType("INTEGER");

            b.Property<DateTime>("LoggedAt")
                .HasColumnType("TEXT");

            b.Property<string>("Note")
                .HasMaxLength(300)
                .HasColumnType("TEXT");

            b.Property<decimal>("ProteinPerUnitSnapshot")
                .HasPrecision(18, 4)
                .HasColumnType("TEXT");

            b.Property<decimal>("Quantity")
                .HasPrecision(18, 4)
                .HasColumnType("TEXT");

            b.HasKey("Id");

            b.HasIndex("DietDayId");

            b.HasIndex("FoodItemId");

            b.HasIndex("LoggedAt");

            b.ToTable("FoodLogEntries");
        });

        modelBuilder.Entity("TrackYourMeals.Models.FoodLogEntry", b =>
        {
            b.HasOne("TrackYourMeals.Models.DietDay", "DietDay")
                .WithMany("LogEntries")
                .HasForeignKey("DietDayId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            b.HasOne("TrackYourMeals.Models.FoodItem", "FoodItem")
                .WithMany("LogEntries")
                .HasForeignKey("FoodItemId")
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            b.Navigation("DietDay");

            b.Navigation("FoodItem");
        });

        modelBuilder.Entity("TrackYourMeals.Models.DietDay", b =>
        {
            b.Navigation("LogEntries");
        });

        modelBuilder.Entity("TrackYourMeals.Models.FoodItem", b =>
        {
            b.Navigation("LogEntries");
        });
#pragma warning restore 612, 618
    }
}
