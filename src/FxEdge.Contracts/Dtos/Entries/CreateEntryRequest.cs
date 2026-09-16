using FxEdge.Contracts.Enums;

namespace FxEdge.Contracts.Dtos.Entries;

/// <summary>
/// Request to record a manually identified chart entry point. This has no relation to
/// any automatic system detection - the user decides the entry point by looking at the
/// chart. Any TechnicalFeature not present in Technical is stored as null (not every
/// reading is always available).
/// </summary>
public sealed record CreateEntryRequest(
    Currency BaseCurrency,
    Currency QuoteCurrency,
    DateTime EntryAtUtc,
    decimal EntryPrice,
    Direction Direction,
    IReadOnlyList<TechnicalFeatureValueDto> Technical);
