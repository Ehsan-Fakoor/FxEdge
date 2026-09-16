using FxEdge.Contracts.Dtos.Dataset;
using FxEdge.Contracts.Enums;
using FxEdge.Contracts.Exceptions;
using FxEdge.Contracts.Services;
using FxEdge.Endpoints.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FxEdge.Endpoints.Dataset;

/// <summary>
/// HTTP surface for the combined Entry+Result ML-ready dataset export. Distinct from
/// DatasetEndpoints (which serves the live, currency-pair-level Fundamental
/// differentials) - this is Entry-centric: one row per (Entry, Horizon) that has a
/// recorded EntryResult.
/// </summary>
public static class EntryDatasetEndpoints
{
    public static IEndpointRouteBuilder MapEntryDatasetEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/dataset/entries").WithTags("EntryDataset");

        group.MapGet("/", async (
            Currency? baseCurrency,
            Currency? quoteCurrency,
            DateTime? fromUtc,
            DateTime? toUtc,
            ResultHorizon? horizon,
            IEntryDatasetService service,
            CancellationToken ct) =>
        {
            var rows = await service.QueryAsync(new EntryDatasetRowQuery(baseCurrency, quoteCurrency, fromUtc, toUtc, horizon), ct);
            return Results.Ok(rows);
        });

        group.MapGet("/{entryId:guid}", async (Guid entryId, IEntryDatasetService service, CancellationToken ct) =>
        {
            try
            {
                var rows = await service.QueryByEntryAsync(entryId, ct);
                return Results.Ok(rows);
            }
            catch (FxEdgeException ex)
            {
                return ex.ToProblem();
            }
        });

        return app;
    }
}
