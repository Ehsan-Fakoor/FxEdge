using FxEdge.Contracts.Enums;
using FxEdge.Contracts.Exceptions;

namespace FxEdge.Business.ValueObjects;

/// <summary>
/// A currency pair, built from a Base and a Quote currency. Not an independent
/// fundamental data source: PairFeature = Polarity * (BaseFeature - QuoteFeature) is
/// always computed on demand from the two currencies' observations (see
/// FxEdge.Business.Services.DifferentialCalculator), never stored as its own dataset.
///
/// Restricted to exactly the 28 pairs the system supports, with a fixed Base/Quote
/// direction per pair (e.g. always EUR/USD, never USD/EUR).
/// </summary>
public sealed class CurrencyPair : IEquatable<CurrencyPair>
{
    public Currency Base { get; }
    public Currency Quote { get; }

    public string Symbol => $"{Base}/{Quote}";

    private static readonly IReadOnlyList<(Currency Base, Currency Quote)> AllowedCombinations = new[]
    {
        (Currency.EUR, Currency.USD),
        (Currency.GBP, Currency.USD),
        (Currency.USD, Currency.JPY),
        (Currency.USD, Currency.CHF),
        (Currency.AUD, Currency.USD),
        (Currency.USD, Currency.CAD),
        (Currency.NZD, Currency.USD),
        (Currency.EUR, Currency.GBP),
        (Currency.EUR, Currency.AUD),
        (Currency.EUR, Currency.NZD),
        (Currency.EUR, Currency.CAD),
        (Currency.EUR, Currency.CHF),
        (Currency.EUR, Currency.JPY),
        (Currency.GBP, Currency.AUD),
        (Currency.GBP, Currency.NZD),
        (Currency.GBP, Currency.CAD),
        (Currency.GBP, Currency.CHF),
        (Currency.GBP, Currency.JPY),
        (Currency.AUD, Currency.NZD),
        (Currency.AUD, Currency.CAD),
        (Currency.AUD, Currency.CHF),
        (Currency.AUD, Currency.JPY),
        (Currency.NZD, Currency.CAD),
        (Currency.NZD, Currency.CHF),
        (Currency.NZD, Currency.JPY),
        (Currency.CAD, Currency.CHF),
        (Currency.CAD, Currency.JPY),
        (Currency.CHF, Currency.JPY),
    };

    private static readonly HashSet<(Currency, Currency)> AllowedLookup = new(AllowedCombinations);

    public CurrencyPair(Currency @base, Currency quote)
    {
        if (!AllowedLookup.Contains((@base, quote)))
        {
            throw new FxEdgeValidationException(
                $"'{@base}/{quote}' is not one of the 28 supported currency pairs (or the Base/Quote order is reversed).");
        }

        Base = @base;
        Quote = quote;
    }

    /// <summary>All 28 supported pairs, in the fixed order the domain defines them.</summary>
    public static IReadOnlyList<CurrencyPair> All { get; } =
        AllowedCombinations.Select(c => new CurrencyPair(c.Base, c.Quote)).ToList();

    public bool Equals(CurrencyPair? other) =>
        other is not null && Base == other.Base && Quote == other.Quote;

    public override bool Equals(object? obj) => Equals(obj as CurrencyPair);

    public override int GetHashCode() => HashCode.Combine(Base, Quote);

    public override string ToString() => Symbol;
}
