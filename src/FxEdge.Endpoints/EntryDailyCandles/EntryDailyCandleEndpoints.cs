using FxEdge.Contracts.Dtos.EntryDailyCandles;
using FxEdge.Contracts.Exceptions;
using FxEdge.Contracts.Services;
using FxEdge.Endpoints.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FxEdge.Endpoints.EntryDailyCandles;

/// <summary>
/// HTTP surface for an Entry's optional, opt-in daily candle series, nested under its
/// Entry. Supplements EntryResult - never required, never a replacement for it.
/// </summary>
public static class EntryDailyCandleEndpoints
{
    public static IEndpointRouteBuilder MapEntryDailyCandleEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/entries/{entryId:guid}/candles").WithTags("EntryDailyCandles");

        group.MapPost("/", async (Guid entryId, CreateEntryDailyCandleRequest request, IEntryDailyCandleService service, CancellationToken ct) =>
        {
            try
            {
                var created = await service.CreateAsync(entryId, request, ct);
                return Results.Created($"/api/entries/{entryId}/candles/{created.DayIndex}", created);
            }
            catch (FxEdgeException ex)
            {
                return ex.ToProblem();
            }
        });

        // Entering up to 40 candles one-at-a-time would be impractical - this accepts
        // many at once, all-or-nothing (see CreateEntryDailyCandlesBatchRequest).
        group.MapPost("/batch", async (Guid entryId, CreateEntryDailyCandlesBatchRequest request, IEntryDailyCandleService service, CancellationToken ct) =>
        {
            try
            {
                var created = await service.CreateBatchAsync(entryId, request, ct);
                return Results.Ok(created);
            }
            catch (FxEdgeException ex)
            {
                return ex.ToProblem();
            }
        });

        group.MapPut("/{dayIndex:int}", async (Guid entryId, int dayIndex, EditEntryDailyCandleRequest request, IEntryDailyCandleService service, CancellationToken ct) =>
        {
            try
            {
                var updated = await service.EditAsync(entryId, dayIndex, request, ct);
                return Results.Ok(updated);
            }
            catch (FxEdgeException ex)
            {
                return ex.ToProblem();
            }
        });

        group.MapGet("/{dayIndex:int}", async (Guid entryId, int dayIndex, IEntryDailyCandleService service, CancellationToken ct) =>
        {
            var candle = await service.GetAsync(entryId, dayIndex, ct);
            return candle is null ? Results.NotFound() : Results.Ok(candle);
        });

        group.MapGet("/", async (Guid entryId, IEntryDailyCandleService service, CancellationToken ct) =>
        {
            var candles = await service.QueryByEntryAsync(entryId, ct);
            return Results.Ok(candles);
        });

        return app;
    }
}
