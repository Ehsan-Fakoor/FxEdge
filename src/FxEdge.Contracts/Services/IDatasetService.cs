using FxEdge.Contracts.Dtos.Dataset;
using FxEdge.Contracts.Enums;

namespace FxEdge.Contracts.Services;

/// <summary>
/// Retrieval surface for point-in-time fundamental data: single-currency snapshots,
/// a single pair's dataset row, or every pair's dataset row for a given date.
/// </summary>
public interface IDatasetService
{
    /// <summary>The 14-feature, point-in-time fundamental snapshot for a single currency.</summary>
    Task<CurrencyFundamentalSnapshotDto> GetCurrencySnapshotAsync(Currency currency, DateTime asOfUtc, CancellationToken ct = default);

    /// <summary>The full dataset row (Base snapshot, Quote snapshot, differentials) for one supported currency pair.</summary>
    Task<FundamentalDatasetRowDto> GetDatasetRowAsync(Currency baseCurrency, Currency quoteCurrency, DateTime asOfUtc, CancellationToken ct = default);

    /// <summary>The dataset row for all 28 supported currency pairs as of the same date.</summary>
    Task<IReadOnlyList<FundamentalDatasetRowDto>> GetAllPairsDatasetRowsAsync(DateTime asOfUtc, CancellationToken ct = default);
}
