using FxEdge.Contracts.Dtos.Entries;
using FxEdge.Contracts.Enums;
using FxEdge.Contracts.Exceptions;
using FxEdge.Contracts.Services;
using FxEdge.Endpoints.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FxEdge.Endpoints.Entries;

/// <summary>
/// HTTP surface for manually marked chart entries. There is intentionally no edit or
/// delete here - an Entry's frozen Fundamental/Technical snapshots are historical and
/// must not change after the fact.
/// </summary>
public static class EntryEndpoints
{
    public static IEndpointRouteBuilder MapEntryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/entries").WithTags("Entries");

        group.MapPost("/", async (CreateEntryRequest request, IEntryService service, CancellationToken ct) =>
        {
            try
            {
                var created = await service.CreateAsync(request, ct);
                return Results.Created($"/api/entries/{created.Id}", created);
            }
            catch (FxEdgeException ex)
            {
                return ex.ToProblem();
            }
        });

        group.MapGet("/{id:guid}", async (Guid id, IEntryService service, CancellationToken ct) =>
        {
            var entry = await service.GetByIdAsync(id, ct);
            return entry is null ? Results.NotFound() : Results.Ok(entry);
        });

        group.MapGet("/", async (
            Currency? baseCurrency,
            Currency? quoteCurrency,
            DateTime? fromUtc,
            DateTime? toUtc,
            Direction? direction,
            IEntryService service,
            CancellationToken ct) =>
        {
            var results = await service.QueryAsync(new EntryQuery(baseCurrency, quoteCurrency, fromUtc, toUtc, direction), ct);
            return Results.Ok(results);
        });

        return app;
    }
}
