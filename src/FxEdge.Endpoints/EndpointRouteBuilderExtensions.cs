using FxEdge.Endpoints.Catalog;
using FxEdge.Endpoints.Dataset;
using FxEdge.Endpoints.Entries;
using FxEdge.Endpoints.EntryDailyCandles;
using FxEdge.Endpoints.EntryResults;
using FxEdge.Endpoints.Observations;
using FxEdge.Endpoints.Strategies;
using Microsoft.AspNetCore.Routing;

namespace FxEdge.Endpoints;

/// <summary>
/// Single entry point for Host to wire up every FxEdge endpoint group.
/// </summary>
public static class EndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapFxEdgeEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapObservationEndpoints();
        app.MapEntryEndpoints();
        app.MapEntryResultEndpoints();
        app.MapEntryDailyCandleEndpoints();
        app.MapStrategyEndpoints();
        app.MapStrategySimulationEndpoints();
        app.MapDatasetEndpoints();
        app.MapEntryDatasetEndpoints();
        app.MapCatalogEndpoints();
        return app;
    }
}
