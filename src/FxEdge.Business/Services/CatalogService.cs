using FxEdge.Business.Catalog;
using FxEdge.Business.ValueObjects;
using FxEdge.Contracts.Dtos.Catalog;
using FxEdge.Contracts.Enums;
using FxEdge.Contracts.Services;

namespace FxEdge.Business.Services;

/// <inheritdoc cref="ICatalogService"/>
public sealed class CatalogService : ICatalogService
{
    public IReadOnlyList<Currency> GetCurrencies() => Enum.GetValues<Currency>();

    public IReadOnlyList<FeatureDefinitionDto> GetFeatures() =>
        FundamentalFeatureCatalog.All
            .Select(d => new FeatureDefinitionDto(d.Feature, d.DisplayNameFa, d.DisplayNameEn, d.Polarity))
            .ToList();

    public IReadOnlyList<CurrencyPairDto> GetPairs() =>
        CurrencyPair.All
            .Select(p => new CurrencyPairDto(p.Base, p.Quote, p.Symbol))
            .ToList();

    public IReadOnlyList<TechnicalFeatureDefinitionDto> GetTechnicalFeatures() =>
        TechnicalFeatureCatalog.All
            .Select(d => new TechnicalFeatureDefinitionDto(d.Feature, d.DisplayNameFa, d.DisplayNameEn))
            .ToList();
}
