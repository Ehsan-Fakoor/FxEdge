using System.Text.Json.Serialization;
using FxEdge.Business.Abstractions;
using FxEdge.Business.Services;
using FxEdge.Contracts.Services;
using FxEdge.DataAccess;
using FxEdge.DataAccess.DependencyInjection;
using FxEdge.Endpoints;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Serialize/deserialize every enum (Currency, FundamentalFeature, Direction, ...) as
// its string name ("EUR", not "1") in request and response bodies - matches how
// they're stored in SQLite and is far easier to test/read by hand.
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// --- Data Access (SQLite via EF Core) ---
var connectionString = builder.Configuration.GetConnectionString("FxEdge")
    ?? "Data Source=fxedge.db";
builder.Services.AddFxEdgeDataAccess(connectionString);

// --- Business services ---
// FxEdge.Business intentionally has zero NuGet dependencies (pure domain logic), so its
// services are registered directly here rather than through a Business-owned DI
// extension method.
builder.Services.AddScoped<IObservationService, ObservationService>();
builder.Services.AddScoped<IEntryService, EntryService>();
builder.Services.AddScoped<IEntryResultService, EntryResultService>();
builder.Services.AddScoped<IEntryDailyCandleService, EntryDailyCandleService>();
builder.Services.AddScoped<IStrategyService, StrategyService>();
builder.Services.AddScoped<IStrategySimulationService, StrategySimulationService>();
builder.Services.AddScoped<IDatasetService, DatasetService>();
builder.Services.AddScoped<IEntryDatasetService, EntryDatasetService>();
builder.Services.AddSingleton<ICatalogService, CatalogService>();

// The four Fundamental Formula Features are now fully implemented - see
// FormulaFeatureCalculator for the confirmed weight tables.
builder.Services.AddSingleton<IFormulaFeatureCalculator, FormulaFeatureCalculator>();

// StrategySimulation's computation (Layer 3) is likewise not yet defined - see
// NotYetDefinedStrategySimulationEngine for exactly what's pending. Swap this
// registration once the tie-breaking and horizon-end-closing rules are settled.
builder.Services.AddSingleton<IStrategySimulationEngine, NotYetDefinedStrategySimulationEngine>();

var app = builder.Build();

// Personal, single-user, local app: apply any pending migrations automatically on
// startup instead of requiring a manual "dotnet ef database update" step every run.
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<FxEdgeDbContext>();
    dbContext.Database.Migrate();
}

app.MapFxEdgeEndpoints();

app.Run();
