using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FxEdge.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AllowNullValue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Entries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    BaseCurrency = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false),
                    QuoteCurrency = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false),
                    EntryAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EntryPrice = table.Column<decimal>(type: "TEXT", nullable: false),
                    Direction = table.Column<string>(type: "TEXT", maxLength: 8, nullable: false),
                    Formula1W = table.Column<decimal>(type: "TEXT", nullable: true),
                    Formula1M = table.Column<decimal>(type: "TEXT", nullable: true),
                    Formula3M = table.Column<decimal>(type: "TEXT", nullable: true),
                    Formula1Y = table.Column<decimal>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EntryDailyCandles",
                columns: table => new
                {
                    EntryId = table.Column<Guid>(type: "TEXT", nullable: false),
                    DayIndex = table.Column<int>(type: "INTEGER", nullable: false),
                    OpenAtr = table.Column<decimal>(type: "TEXT", nullable: false),
                    HighAtr = table.Column<decimal>(type: "TEXT", nullable: false),
                    LowAtr = table.Column<decimal>(type: "TEXT", nullable: false),
                    CloseAtr = table.Column<decimal>(type: "TEXT", nullable: false),
                    FirstExtreme = table.Column<string>(type: "TEXT", maxLength: 16, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntryDailyCandles", x => new { x.EntryId, x.DayIndex });
                });

            migrationBuilder.CreateTable(
                name: "EntryResults",
                columns: table => new
                {
                    EntryId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Horizon = table.Column<string>(type: "TEXT", maxLength: 16, nullable: false),
                    MaximumFavorableExcursionAtr = table.Column<decimal>(type: "TEXT", nullable: false),
                    MaximumAdverseExcursionAtr = table.Column<decimal>(type: "TEXT", nullable: false),
                    ReturnAtHorizonEndAtr = table.Column<decimal>(type: "TEXT", nullable: false),
                    FirstExtremeReached = table.Column<string>(type: "TEXT", maxLength: 16, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntryResults", x => new { x.EntryId, x.Horizon });
                });

            migrationBuilder.CreateTable(
                name: "Observations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Currency = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false),
                    Feature = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    AnnouncementAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ForecastValue = table.Column<decimal>(type: "TEXT", nullable: true),
                    Value = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Observations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Strategies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    EntrySpacingAtr = table.Column<decimal>(type: "TEXT", nullable: false),
                    TakeProfitPerEntryAtr = table.Column<decimal>(type: "TEXT", nullable: false),
                    OverallStopLossAtr = table.Column<decimal>(type: "TEXT", nullable: false),
                    MaxEntries = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Strategies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StrategySimulations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    StrategyId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Horizon = table.Column<string>(type: "TEXT", maxLength: 16, nullable: false),
                    RunAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ProfitAtr = table.Column<decimal>(type: "TEXT", nullable: false),
                    LossAtr = table.Column<decimal>(type: "TEXT", nullable: false),
                    SuccessfulEntries = table.Column<int>(type: "INTEGER", nullable: false),
                    FailedEntries = table.Column<int>(type: "INTEGER", nullable: false),
                    WinRatePercent = table.Column<decimal>(type: "TEXT", nullable: false),
                    MaxDrawdownAtr = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StrategySimulations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EntryFundamentalFeatureSnapshots",
                columns: table => new
                {
                    EntryId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Feature = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Polarity = table.Column<int>(type: "INTEGER", nullable: false),
                    BaseValue = table.Column<decimal>(type: "TEXT", nullable: true),
                    BaseForecastValue = table.Column<decimal>(type: "TEXT", nullable: true),
                    BasePreviousValue = table.Column<decimal>(type: "TEXT", nullable: true),
                    BaseActualVsForecast = table.Column<decimal>(type: "TEXT", nullable: true),
                    BaseActualVsPrevious = table.Column<decimal>(type: "TEXT", nullable: true),
                    BaseAnnouncementAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    BaseNormalizedValue = table.Column<decimal>(type: "TEXT", nullable: true),
                    QuoteValue = table.Column<decimal>(type: "TEXT", nullable: true),
                    QuoteForecastValue = table.Column<decimal>(type: "TEXT", nullable: true),
                    QuotePreviousValue = table.Column<decimal>(type: "TEXT", nullable: true),
                    QuoteActualVsForecast = table.Column<decimal>(type: "TEXT", nullable: true),
                    QuoteActualVsPrevious = table.Column<decimal>(type: "TEXT", nullable: true),
                    QuoteAnnouncementAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    QuoteNormalizedValue = table.Column<decimal>(type: "TEXT", nullable: true),
                    Difference = table.Column<decimal>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntryFundamentalFeatureSnapshots", x => new { x.EntryId, x.Feature });
                    table.ForeignKey(
                        name: "FK_EntryFundamentalFeatureSnapshots_Entries_EntryId",
                        column: x => x.EntryId,
                        principalTable: "Entries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntryTechnicalFeatureSnapshots",
                columns: table => new
                {
                    EntryId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Feature = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    Value = table.Column<decimal>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntryTechnicalFeatureSnapshots", x => new { x.EntryId, x.Feature });
                    table.ForeignKey(
                        name: "FK_EntryTechnicalFeatureSnapshots_Entries_EntryId",
                        column: x => x.EntryId,
                        principalTable: "Entries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StrategySimulationEntryOutcomes",
                columns: table => new
                {
                    StrategySimulationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    EntryId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Outcome = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    ResultAtr = table.Column<decimal>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StrategySimulationEntryOutcomes", x => new { x.StrategySimulationId, x.EntryId });
                    table.ForeignKey(
                        name: "FK_StrategySimulationEntryOutcomes_StrategySimulations_StrategySimulationId",
                        column: x => x.StrategySimulationId,
                        principalTable: "StrategySimulations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Entries_BaseCurrency_QuoteCurrency_EntryAtUtc",
                table: "Entries",
                columns: new[] { "BaseCurrency", "QuoteCurrency", "EntryAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_Observations_Currency_Feature_AnnouncementAtUtc",
                table: "Observations",
                columns: new[] { "Currency", "Feature", "AnnouncementAtUtc" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StrategySimulations_StrategyId_Horizon",
                table: "StrategySimulations",
                columns: new[] { "StrategyId", "Horizon" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EntryDailyCandles");

            migrationBuilder.DropTable(
                name: "EntryFundamentalFeatureSnapshots");

            migrationBuilder.DropTable(
                name: "EntryResults");

            migrationBuilder.DropTable(
                name: "EntryTechnicalFeatureSnapshots");

            migrationBuilder.DropTable(
                name: "Observations");

            migrationBuilder.DropTable(
                name: "Strategies");

            migrationBuilder.DropTable(
                name: "StrategySimulationEntryOutcomes");

            migrationBuilder.DropTable(
                name: "Entries");

            migrationBuilder.DropTable(
                name: "StrategySimulations");
        }
    }
}
