using FxEdge.Contracts.Dtos.Entries;
using FxEdge.Contracts.Dtos.EntryResults;
using FxEdge.Contracts.Enums;

namespace FxEdge.Contracts.Dtos.Dataset;

/// <summary>
/// One row of the eventual ML dataset: an Entry's full, frozen feature set (16-feature
/// Fundamental Snapshot with raw + normalized values, 7 Technical readings, 4 Formula
/// Features) paired with its recorded outcome (MFE/MAE/ReturnAtHorizonEndAtr/
/// FirstExtremeReached) for one Horizon - the label. One row per (Entry, Horizon) that
/// has a recorded EntryResult; an Entry with results for 3 Horizons produces 3 rows,
/// each sharing the same Entry but pairing with a different Horizon's outcome.
///
/// Purely a composition of already-existing data - EntryDto and EntryResultDto are
/// embedded as-is, nothing here is recomputed or duplicated.
/// </summary>
public sealed record EntryDatasetRowDto(
    Guid EntryId,
    ResultHorizon Horizon,
    EntryDto Entry,
    EntryResultDto Result);
