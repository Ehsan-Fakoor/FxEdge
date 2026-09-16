using FxEdge.Contracts.Enums;

namespace FxEdge.Contracts.Dtos.Catalog;

/// <summary>
/// One of the 28 fixed, direction-locked currency pairs (e.g. Base=EUR, Quote=USD -> "EUR/USD").
/// </summary>
public sealed record CurrencyPairDto(
    Currency Base,
    Currency Quote,
    string Symbol);
