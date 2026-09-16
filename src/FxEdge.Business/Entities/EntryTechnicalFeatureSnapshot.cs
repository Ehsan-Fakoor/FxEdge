using FxEdge.Contracts.Enums;

namespace FxEdge.Business.Entities;

/// <summary>
/// One technical feature's value as entered by the user at Entry time, frozen
/// permanently - technical data is historical and must not change due to later market
/// movement.
/// </summary>
public sealed class EntryTechnicalFeatureSnapshot
{
    public Guid EntryId { get; private set; }
    public TechnicalFeature Feature { get; private set; }
    public decimal? Value { get; private set; }

    // EF Core materialization constructor.
    private EntryTechnicalFeatureSnapshot()
    {
    }

    internal EntryTechnicalFeatureSnapshot(TechnicalFeature feature, decimal? value)
    {
        Feature = feature;
        Value = value;
    }
}
