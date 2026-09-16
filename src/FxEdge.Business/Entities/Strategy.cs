using FxEdge.Contracts.Exceptions;

namespace FxEdge.Business.Entities;

/// <summary>
/// A named, reusable ATR-based scaling strategy definition. Unlike Entry/Observation
/// (historical records), a Strategy is configuration - it is expected to be tuned over
/// time via Overwrite(), not just corrected.
/// </summary>
public sealed class Strategy
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;

    /// <summary>Distance between successive entries, in ATR.</summary>
    public decimal EntrySpacingAtr { get; private set; }

    /// <summary>Take-profit target for each individual entry, in ATR.</summary>
    public decimal TakeProfitPerEntryAtr { get; private set; }

    /// <summary>Overall stop-loss for the whole basket of entries, in ATR.</summary>
    public decimal OverallStopLossAtr { get; private set; }

    /// <summary>Maximum number of entries this strategy is allowed to place.</summary>
    public int MaxEntries { get; private set; }

    // EF Core materialization constructor.
    private Strategy()
    {
    }

    private Strategy(Guid id, string name, decimal entrySpacingAtr, decimal takeProfitPerEntryAtr, decimal overallStopLossAtr, int maxEntries)
    {
        Id = id;
        Name = name;
        EntrySpacingAtr = entrySpacingAtr;
        TakeProfitPerEntryAtr = takeProfitPerEntryAtr;
        OverallStopLossAtr = overallStopLossAtr;
        MaxEntries = maxEntries;
    }

    public static Strategy Create(string name, decimal entrySpacingAtr, decimal takeProfitPerEntryAtr, decimal overallStopLossAtr, int maxEntries)
    {
        Validate(name, entrySpacingAtr, takeProfitPerEntryAtr, overallStopLossAtr, maxEntries);
        return new Strategy(Guid.NewGuid(), name.Trim(), entrySpacingAtr, takeProfitPerEntryAtr, overallStopLossAtr, maxEntries);
    }

    public void Overwrite(string name, decimal entrySpacingAtr, decimal takeProfitPerEntryAtr, decimal overallStopLossAtr, int maxEntries)
    {
        Validate(name, entrySpacingAtr, takeProfitPerEntryAtr, overallStopLossAtr, maxEntries);
        Name = name.Trim();
        EntrySpacingAtr = entrySpacingAtr;
        TakeProfitPerEntryAtr = takeProfitPerEntryAtr;
        OverallStopLossAtr = overallStopLossAtr;
        MaxEntries = maxEntries;
    }

    private static void Validate(string name, decimal entrySpacingAtr, decimal takeProfitPerEntryAtr, decimal overallStopLossAtr, int maxEntries)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new FxEdgeValidationException("Strategy name cannot be empty.");
        }

        if (entrySpacingAtr <= 0)
        {
            throw new FxEdgeValidationException("EntrySpacingAtr must be greater than zero.");
        }

        if (takeProfitPerEntryAtr <= 0)
        {
            throw new FxEdgeValidationException("TakeProfitPerEntryAtr must be greater than zero.");
        }

        if (overallStopLossAtr <= 0)
        {
            throw new FxEdgeValidationException("OverallStopLossAtr must be greater than zero.");
        }

        if (maxEntries < 1)
        {
            throw new FxEdgeValidationException("MaxEntries must be at least 1.");
        }
    }
}
