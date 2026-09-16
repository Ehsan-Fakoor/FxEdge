namespace FxEdge.Contracts.Enums;

/// <summary>
/// The fixed set of currencies supported by the system. This list is closed by design:
/// adding a currency is a deliberate schema change, not a data-entry action, so it is
/// modeled as an enum rather than a database table.
/// </summary>
public enum Currency
{
    USD,
    EUR,
    GBP,
    AUD,
    NZD,
    CAD,
    CHF,
    JPY
}
