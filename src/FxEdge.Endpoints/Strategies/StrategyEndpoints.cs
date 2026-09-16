using FxEdge.Contracts.Dtos.Strategies;
using FxEdge.Contracts.Exceptions;
using FxEdge.Contracts.Services;
using FxEdge.Endpoints.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FxEdge.Endpoints.Strategies;

/// <summary>
/// HTTP surface for Strategy parameter sets (Layer 2).
/// </summary>
public static class StrategyEndpoints
{
    public static IEndpointRouteBuilder MapStrategyEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/strategies").WithTags("Strategies");

        group.MapPost("/", async (CreateStrategyRequest request, IStrategyService service, CancellationToken ct) =>
        {
            try
            {
                var created = await service.CreateAsync(request, ct);
                return Results.Created($"/api/strategies/{created.Id}", created);
            }
            catch (FxEdgeException ex)
            {
                return ex.ToProblem();
            }
        });

        group.MapPut("/{id:guid}", async (Guid id, EditStrategyRequest request, IStrategyService service, CancellationToken ct) =>
        {
            try
            {
                var updated = await service.EditAsync(id, request, ct);
                return Results.Ok(updated);
            }
            catch (FxEdgeException ex)
            {
                return ex.ToProblem();
            }
        });

        group.MapGet("/{id:guid}", async (Guid id, IStrategyService service, CancellationToken ct) =>
        {
            var strategy = await service.GetByIdAsync(id, ct);
            return strategy is null ? Results.NotFound() : Results.Ok(strategy);
        });

        group.MapGet("/", async (IStrategyService service, CancellationToken ct) =>
        {
            var strategies = await service.QueryAsync(ct);
            return Results.Ok(strategies);
        });

        return app;
    }
}
