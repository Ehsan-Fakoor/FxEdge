using FxEdge.Contracts.Dtos.EntryResults;
using FxEdge.Contracts.Enums;
using FxEdge.Contracts.Exceptions;
using FxEdge.Contracts.Services;
using FxEdge.Endpoints.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FxEdge.Endpoints.EntryResults;

/// <summary>
/// HTTP surface for an Entry's per-horizon outcome (Layer 1), nested under its Entry.
/// </summary>
public static class EntryResultEndpoints
{
    public static IEndpointRouteBuilder MapEntryResultEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/entries/{entryId:guid}/results").WithTags("EntryResults");

        group.MapPost("/", async (Guid entryId, CreateEntryResultRequest request, IEntryResultService service, CancellationToken ct) =>
        {
            try
            {
                var created = await service.CreateAsync(entryId, request, ct);
                return Results.Created($"/api/entries/{entryId}/results/{created.Horizon}", created);
            }
            catch (FxEdgeException ex)
            {
                return ex.ToProblem();
            }
        });

        group.MapPut("/{horizon}", async (Guid entryId, ResultHorizon horizon, EditEntryResultRequest request, IEntryResultService service, CancellationToken ct) =>
        {
            try
            {
                var updated = await service.EditAsync(entryId, horizon, request, ct);
                return Results.Ok(updated);
            }
            catch (FxEdgeException ex)
            {
                return ex.ToProblem();
            }
        });

        group.MapGet("/{horizon}", async (Guid entryId, ResultHorizon horizon, IEntryResultService service, CancellationToken ct) =>
        {
            var result = await service.GetAsync(entryId, horizon, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapGet("/", async (Guid entryId, IEntryResultService service, CancellationToken ct) =>
        {
            var results = await service.QueryByEntryAsync(entryId, ct);
            return Results.Ok(results);
        });

        return app;
    }
}
