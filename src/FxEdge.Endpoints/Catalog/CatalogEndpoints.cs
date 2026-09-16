using FxEdge.Contracts.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FxEdge.Endpoints.Catalog;

/// <summary>
/// HTTP surface for the system's fixed reference data (currencies, feature
/// definitions, supported pairs), so a future UI can populate dropdowns without
/// hardcoding them.
/// </summary>
public static class CatalogEndpoints
{
    public static IEndpointRouteBuilder MapCatalogEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/catalog").WithTags("Catalog");

        group.MapGet("/currencies", (ICatalogService service) => Results.Ok(service.GetCurrencies()));

        group.MapGet("/features", (ICatalogService service) => Results.Ok(service.GetFeatures()));

        group.MapGet("/pairs", (ICatalogService service) => Results.Ok(service.GetPairs()));

        group.MapGet("/technical-features", (ICatalogService service) => Results.Ok(service.GetTechnicalFeatures()));

        return app;
    }
}
