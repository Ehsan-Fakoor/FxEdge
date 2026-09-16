using FxEdge.Business.Abstractions;
using FxEdge.Business.Entities;
using FxEdge.Contracts.Dtos.EntryResults;
using FxEdge.Contracts.Enums;
using FxEdge.Contracts.Exceptions;
using FxEdge.Contracts.Services;

namespace FxEdge.Business.Services;

/// <inheritdoc cref="IEntryResultService"/>
public sealed class EntryResultService : IEntryResultService
{
    private readonly IEntryResultRepository _entryResultRepository;
    private readonly IEntryRepository _entryRepository;

    public EntryResultService(IEntryResultRepository entryResultRepository, IEntryRepository entryRepository)
    {
        _entryResultRepository = entryResultRepository;
        _entryRepository = entryRepository;
    }

    public async Task<EntryResultDto> CreateAsync(Guid entryId, CreateEntryResultRequest request, CancellationToken ct = default)
    {
        await GetEntryOrThrowAsync(entryId, ct);

        var existing = await _entryResultRepository.GetAsync(entryId, request.Horizon, ct);
        if (existing is not null)
        {
            throw new FxEdgeConflictException(
                $"A result for Entry '{entryId}' and horizon '{request.Horizon}' already exists. Use edit to correct it instead.");
        }

        var result = EntryResult.Create(
            entryId, request.Horizon, request.MaximumFavorableExcursionAtr, request.MaximumAdverseExcursionAtr,
            request.ReturnAtHorizonEndAtr, request.FirstExtremeReached);
        await _entryResultRepository.AddAsync(result, ct);
        await _entryResultRepository.SaveChangesAsync(ct);

        return ToDto(result);
    }

    public async Task<EntryResultDto> EditAsync(Guid entryId, ResultHorizon horizon, EditEntryResultRequest request, CancellationToken ct = default)
    {
        var result = await _entryResultRepository.GetAsync(entryId, horizon, ct)
            ?? throw new FxEdgeNotFoundException($"No result found for Entry '{entryId}' and horizon '{horizon}'.");

        result.Overwrite(request.MaximumFavorableExcursionAtr, request.MaximumAdverseExcursionAtr, request.ReturnAtHorizonEndAtr, request.FirstExtremeReached);
        await _entryResultRepository.SaveChangesAsync(ct);

        return ToDto(result);
    }

    public async Task<EntryResultDto?> GetAsync(Guid entryId, ResultHorizon horizon, CancellationToken ct = default)
    {
        var result = await _entryResultRepository.GetAsync(entryId, horizon, ct);
        return result is null ? null : ToDto(result);
    }

    public async Task<IReadOnlyList<EntryResultDto>> QueryByEntryAsync(Guid entryId, CancellationToken ct = default)
    {
        var results = await _entryResultRepository.QueryByEntryAsync(entryId, ct);
        return results.Select(ToDto).ToList();
    }

    /// <summary>
    /// Loads the Entry (throwing if it doesn't exist) and validates it has an ATR
    /// reading - MFE/MAE/ReturnAtHorizonEndAtr are multiples of that ATR, so recording
    /// them without one would produce numbers with no defined price-equivalent meaning.
    /// </summary>
    private async Task<Entry> GetEntryOrThrowAsync(Guid entryId, CancellationToken ct)
    {
        var entry = await _entryRepository.GetByIdAsync(entryId, ct)
            ?? throw new FxEdgeNotFoundException($"Entry '{entryId}' was not found.");

        if (entry.AtrValue is null)
        {
            throw new FxEdgeValidationException(
                $"Entry '{entryId}' has no ATR recorded on its Technical Snapshot - MFE/MAE/ReturnAtHorizonEndAtr " +
                "cannot be interpreted without a reference ATR value.");
        }

        return entry;
    }

    private static EntryResultDto ToDto(EntryResult result) => new(
        result.EntryId, result.Horizon, result.MaximumFavorableExcursionAtr, result.MaximumAdverseExcursionAtr,
        result.ReturnAtHorizonEndAtr, result.FirstExtremeReached);
}
