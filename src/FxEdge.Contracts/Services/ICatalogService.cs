using FxEdge.Contracts.Dtos.Catalog;
using FxEdge.Contracts.Enums;

namespace FxEdge.Contracts.Services;

/// <summary>
/// Read-only access to the system's fixed reference data, so a future Data Entry UI can
/// populate dropdowns (currencies, features, pairs, technical features) without
/// hardcoding them.
/// </summary>
public interface ICatalogService
{
    IReadOnlyList<Currency> GetCurrencies();

    IReadOnlyList<FeatureDefinitionDto> GetFeatures();

    IReadOnlyList<CurrencyPairDto> GetPairs();

    IReadOnlyList<TechnicalFeatureDefinitionDto> GetTechnicalFeatures();
}
