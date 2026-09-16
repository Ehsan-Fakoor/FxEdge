using FxEdge.Contracts.Enums;
using FxEdge.Contracts.Exceptions;
using FxEdge.Contracts.Services;
using FxEdge.Endpoints.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FxEdge.Endpoints.Strategies;

/// <summary>
/// HTTP surface for running and retrieving Strategy simulations (Layer 3). As of this
/// phase, POST .../simulations returns 501 Not Implemented - see
/// FxEdge.Business.Services.NotYetDefinedStrategySimulationEngine.
/// </summary>
public static class StrategySimulationEndpoints
{
    public static IEndpointRouteBuilder MapStrategySimulationEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/strategies/{strategyId:guid}/simulations", async (Guid strategyId, ResultHorizon horizon, IStrategySimulationService service, CancellationToken ct) =>
        {
            try
            {
                var result = await service.RunAsync(strategyId, horizon, ct);
                return Results.Created($"/api/simulations/{result.Id}", result);
            }
            catch (FxEdgeException ex)
            {
                return ex.ToProblem();
            }
        }).WithTags("StrategySimulations");

        app.MapGet("/api/strategies/{strategyId:guid}/simulations", async (Guid strategyId, IStrategySimulationService service, CancellationToken ct) =>
        {
            var results = await service.QueryByStrategyAsync(strategyId, ct);
            return Results.Ok(results);
        }).WithTags("StrategySimulations");

        app.MapGet("/api/simulations/{id:guid}", async (Guid id, IStrategySimulationService service, CancellationToken ct) =>
        {
            var result = await service.GetByIdAsync(id, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        }).WithTags("StrategySimulations");

        return app;
    }
}
