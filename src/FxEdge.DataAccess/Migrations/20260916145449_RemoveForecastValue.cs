using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FxEdge.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RemoveForecastValue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ForecastValue",
                table: "Observations");

            migrationBuilder.DropColumn(
                name: "BaseActualVsForecast",
                table: "EntryFundamentalFeatureSnapshots");

            migrationBuilder.DropColumn(
                name: "BaseForecastValue",
                table: "EntryFundamentalFeatureSnapshots");

            migrationBuilder.DropColumn(
                name: "QuoteActualVsForecast",
                table: "EntryFundamentalFeatureSnapshots");

            migrationBuilder.DropColumn(
                name: "QuoteForecastValue",
                table: "EntryFundamentalFeatureSnapshots");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ForecastValue",
                table: "Observations",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BaseActualVsForecast",
                table: "EntryFundamentalFeatureSnapshots",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BaseForecastValue",
                table: "EntryFundamentalFeatureSnapshots",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "QuoteActualVsForecast",
                table: "EntryFundamentalFeatureSnapshots",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "QuoteForecastValue",
                table: "EntryFundamentalFeatureSnapshots",
                type: "TEXT",
                nullable: true);
        }
    }
}
