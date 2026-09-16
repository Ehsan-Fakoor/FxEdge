using FxEdge.Contracts.Enums;
using FxEdge.Contracts.Exceptions;
using FxEdge.Contracts.Services;
using FxEdge.Endpoints.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FxEdge.Endpoints.Dataset;

/// <summary>
/// HTTP surface for point-in-time fundamental retrieval: a single currency's 14-feature
/// snapshot, a single pair's dataset row, or every supported pair's dataset row for the
/// same date - the read side that will eventually feed dataset export for ML.
/// </summary>
public static class DatasetEndpoints
{
    public static IEndpointRouteBuilder MapDatasetEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/dataset").WithTags("Dataset");

        group.MapGet("/currencies/{currency}/snapshot", async (Currency currency, DateTime asOfUtc, IDatasetService service, CancellationToken ct) =>
        {
            try
            {
                var snapshot = await service.GetCurrencySnapshotAsync(currency, asOfUtc, ct);
                return Results.Ok(snapshot);
            }
            catch (FxEdgeException ex)
            {
                return ex.ToProblem();
            }
        });

        group.MapGet("/pairs/{baseCurrency}/{quoteCurrency}/row", async (Currency baseCurrency, Currency quoteCurrency, DateTime asOfUtc, IDatasetService service, CancellationToken ct) =>
        {
            try
            {
                var row = await service.GetDatasetRowAsync(baseCurrency, quoteCurrency, asOfUtc, ct);
                return Results.Ok(row);
            }
            catch (FxEdgeException ex)
            {
                return ex.ToProblem();
            }
        });

        group.MapGet("/rows", async (DateTime asOfUtc, IDatasetService service, CancellationToken ct) =>
        {
            var rows = await service.GetAllPairsDatasetRowsAsync(asOfUtc, ct);
            return Results.Ok(rows);
        });

        return app;
    }
}
