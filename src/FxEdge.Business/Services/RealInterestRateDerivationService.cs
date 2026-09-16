using FxEdge.Business.Abstractions;
using FxEdge.Business.Entities;
using FxEdge.Contracts.Enums;

namespace FxEdge.Business.Services;

/// <summary>
/// RealInterestRate is never entered directly (ObservationService rejects it) - it is
/// always derived as CentralBankInterestRate − CoreCpi. This is the only place that
/// ever creates or overwrites a RealInterestRate FundamentalObservation.
///
/// Whenever a CentralBankInterestRate or CoreCpi observation is created/edited for a
/// currency, DeriveAsync recomputes RealInterestRate for that exact same
/// AnnouncementAtUtc, using the *other* feature's point-in-time carried-forward value
/// (same carry-forward semantics as everywhere else). If the other feature has no data
/// yet as of that date, nothing is created - RealInterestRate simply isn't derivable
/// yet there, matching the rest of the system's null-safety philosophy rather than
/// fabricating a value.
///
/// Known limitation: this only recomputes RealInterestRate at the triggering
/// observation's own date. Editing an *earlier* CentralBankInterestRate/CoreCpi
/// observation does not cascade forward to re-derive later RealInterestRate rows that
/// had carried its old value forward.
/// </summary>
internal sealed class RealInterestRateDerivationService
{
    private static readonly IReadOnlySet<FundamentalFeature> TriggerFeatures = new HashSet<FundamentalFeature>
    {
        FundamentalFeature.CentralBankInterestRate,
        FundamentalFeature.CoreCpi
    };

    private readonly IObservationRepository _repository;

    public RealInterestRateDerivationService(IObservationRepository repository)
    {
        _repository = repository;
    }

    /// <summary>Whether creating/editing this feature should trigger a RealInterestRate re-derivation.</summary>
    public bool IsTrigger(FundamentalFeature feature) => TriggerFeatures.Contains(feature);

    public async Task DeriveAsync(Currency currency, DateTime announcementAtUtc, CancellationToken ct = default)
    {
        var centralBank = await _repository.GetLatestAsOfAsync(currency, FundamentalFeature.CentralBankInterestRate, announcementAtUtc, ct);
        var coreCpi = await _repository.GetLatestAsOfAsync(currency, FundamentalFeature.CoreCpi, announcementAtUtc, ct);

        if (centralBank is null || coreCpi is null)
        {
            // One side has no data yet as of this date - RealInterestRate can't be
            // derived here yet (nothing to do; no row is created).
            return;
        }

        var value = centralBank.Value - coreCpi.Value;

        var existing = await _repository.GetExactAsync(currency, FundamentalFeature.RealInterestRate, announcementAtUtc, ct);
        if (existing is null)
        {
            var derived = FundamentalObservation.Create(currency, FundamentalFeature.RealInterestRate, announcementAtUtc, value);
            await _repository.AddAsync(derived, ct);
        }
        else
        {
            existing.Overwrite(currency, FundamentalFeature.RealInterestRate, announcementAtUtc, value);
        }
    }
}
