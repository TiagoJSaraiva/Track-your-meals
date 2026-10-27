using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using TrackYourMeals.Data;

#nullable disable

namespace TrackYourMeals.Migrations;

[DbContext(typeof(TrackYourMealsDbContext))]
[Migration("20260516000000_InitialCreate")]
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "DietDays",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                Status = table.Column<int>(type: "INTEGER", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                ClosedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_DietDays", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "FoodItems",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Name = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                UnitLabel = table.Column<string>(type: "TEXT", maxLength: 24, nullable: false),
                DailyTargetQuantity = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                CaloriesPerUnit = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                ProteinPerUnit = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                CarbsPerUnit = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                FatPerUnit = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_FoodItems", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "FoodLogEntries",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                DietDayId = table.Column<int>(type: "INTEGER", nullable: false),
                FoodItemId = table.Column<int>(type: "INTEGER", nullable: false),
                Quantity = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                LoggedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                Note = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                CaloriesPerUnitSnapshot = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                ProteinPerUnitSnapshot = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                CarbsPerUnitSnapshot = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                FatPerUnitSnapshot = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_FoodLogEntries", x => x.Id);
                table.ForeignKey(
                    name: "FK_FoodLogEntries_DietDays_DietDayId",
                    column: x => x.DietDayId,
                    principalTable: "DietDays",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_FoodLogEntries_FoodItems_FoodItemId",
                    column: x => x.FoodItemId,
                    principalTable: "FoodItems",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_DietDays_Date",
            table: "DietDays",
            column: "Date");

        migrationBuilder.CreateIndex(
            name: "IX_DietDays_Status",
            table: "DietDays",
            column: "Status");

        migrationBuilder.CreateIndex(
            name: "IX_FoodItems_Name",
            table: "FoodItems",
            column: "Name");

        migrationBuilder.CreateIndex(
            name: "IX_FoodLogEntries_DietDayId",
            table: "FoodLogEntries",
            column: "DietDayId");

        migrationBuilder.CreateIndex(
            name: "IX_FoodLogEntries_FoodItemId",
            table: "FoodLogEntries",
            column: "FoodItemId");

        migrationBuilder.CreateIndex(
            name: "IX_FoodLogEntries_LoggedAt",
            table: "FoodLogEntries",
            column: "LoggedAt");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "FoodLogEntries");

        migrationBuilder.DropTable(
            name: "DietDays");

        migrationBuilder.DropTable(
            name: "FoodItems");
    }
}
