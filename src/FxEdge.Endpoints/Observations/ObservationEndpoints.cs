using FxEdge.Contracts.Dtos.Observations;
using FxEdge.Contracts.Enums;
using FxEdge.Contracts.Exceptions;
using FxEdge.Contracts.Services;
using FxEdge.Endpoints.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FxEdge.Endpoints.Observations;

/// <summary>
/// HTTP surface for the Data Entry / Edit use cases: create a new observation, correct
/// an existing one in place, fetch one by Id, or list/filter them. All handlers
/// translate FxEdgeException into the matching HTTP status via FxEdgeExceptionMapper.
/// </summary>
public static class ObservationEndpoints
{
    public static IEndpointRouteBuilder MapObservationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/observations").WithTags("Observations");

        group.MapPost("/", async (CreateObservationRequest request, IObservationService service, CancellationToken ct) =>
        {
            try
            {
                var created = await service.CreateAsync(request, ct);
                return Results.Created($"/api/observations/{created.Id}", created);
            }
            catch (FxEdgeException ex)
            {
                return ex.ToProblem();
            }
        });

        group.MapPut("/{id:guid}", async (Guid id, EditObservationBody body, IObservationService service, CancellationToken ct) =>
        {
            try
            {
                var request = new EditObservationRequest(id, body.Currency, body.Feature, body.AnnouncementAtUtc, body.Value);
                var updated = await service.EditAsync(request, ct);
                return Results.Ok(updated);
            }
            catch (FxEdgeException ex)
            {
                return ex.ToProblem();
            }
        });

        group.MapGet("/{id:guid}", async (Guid id, IObservationService service, CancellationToken ct) =>
        {
            var observation = await service.GetByIdAsync(id, ct);
            return observation is null ? Results.NotFound() : Results.Ok(observation);
        });

        group.MapGet("/", async (
            Currency? currency,
            FundamentalFeature? feature,
            DateTime? fromUtc,
            DateTime? toUtc,
            IObservationService service,
            CancellationToken ct) =>
        {
            var results = await service.QueryAsync(new ObservationQuery(currency, feature, fromUtc, toUtc), ct);
            return Results.Ok(results);
        });

        return app;
    }

    /// <summary>
    /// Edit request body without Id - the Id always comes from the route, so the two
    /// can never disagree. Combined with the route Id into an EditObservationRequest
    /// before calling the service.
    /// </summary>
    private sealed record EditObservationBody(
        Currency Currency,
        FundamentalFeature Feature,
        DateTime AnnouncementAtUtc,
        decimal Value);
}
