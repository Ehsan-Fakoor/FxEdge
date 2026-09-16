using FxEdge.Business.Abstractions;
using FxEdge.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FxEdge.DataAccess.DependencyInjection;

/// <summary>
/// Registers FxEdgeDbContext (SQLite) and its repositories. Host calls this once with a
/// connection string and never needs to know EF Core is involved.
/// </summary>
public static class DataAccessServiceCollectionExtensions
{
    public static IServiceCollection AddFxEdgeDataAccess(this IServiceCollection services, string sqliteConnectionString)
    {
        services.AddDbContext<FxEdgeDbContext>(options => options.UseSqlite(sqliteConnectionString));
        services.AddScoped<IObservationRepository, ObservationRepository>();
        services.AddScoped<IEntryRepository, EntryRepository>();
        services.AddScoped<IEntryResultRepository, EntryResultRepository>();
        services.AddScoped<IEntryDailyCandleRepository, EntryDailyCandleRepository>();
        services.AddScoped<IStrategyRepository, StrategyRepository>();
        services.AddScoped<IStrategySimulationRepository, StrategySimulationRepository>();
        return services;
    }
}
