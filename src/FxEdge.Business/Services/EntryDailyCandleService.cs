using FxEdge.Business.Abstractions;
using FxEdge.Business.Entities;
using FxEdge.Contracts.Dtos.EntryDailyCandles;
using FxEdge.Contracts.Exceptions;
using FxEdge.Contracts.Services;

namespace FxEdge.Business.Services;

/// <inheritdoc cref="IEntryDailyCandleService"/>
public sealed class EntryDailyCandleService : IEntryDailyCandleService
{
    private readonly IEntryDailyCandleRepository _candleRepository;
    private readonly IEntryRepository _entryRepository;

    public EntryDailyCandleService(IEntryDailyCandleRepository candleRepository, IEntryRepository entryRepository)
    {
        _candleRepository = candleRepository;
        _entryRepository = entryRepository;
    }

    public async Task<EntryDailyCandleDto> CreateAsync(Guid entryId, CreateEntryDailyCandleRequest request, CancellationToken ct = default)
    {
        await GetEntryOrThrowAsync(entryId, ct);

        var existing = await _candleRepository.GetAsync(entryId, request.DayIndex, ct);
        if (existing is not null)
        {
            throw new FxEdgeConflictException(
                $"A candle for Entry '{entryId}' at day {request.DayIndex} already exists. Use edit to correct it instead.");
        }

        var candle = EntryDailyCandle.Create(
            entryId, request.DayIndex, request.OpenAtr, request.HighAtr, request.LowAtr, request.CloseAtr, request.FirstExtreme);

        await _candleRepository.AddAsync(candle, ct);
        await _candleRepository.SaveChangesAsync(ct);

        return ToDto(candle);
    }

    public async Task<IReadOnlyList<EntryDailyCandleDto>> CreateBatchAsync(Guid entryId, CreateEntryDailyCandlesBatchRequest request, CancellationToken ct = default)
    {
        await GetEntryOrThrowAsync(entryId, ct);

        var duplicateDayIndexesInBatch = request.Candles
            .GroupBy(c => c.DayIndex)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();
        if (duplicateDayIndexesInBatch.Count > 0)
        {
            throw new FxEdgeValidationException(
                $"Batch contains duplicate DayIndex value(s): {string.Join(", ", duplicateDayIndexesInBatch)}.");
        }

        var alreadyRecorded = await _candleRepository.QueryByEntryAsync(entryId, ct);
        var alreadyRecordedDayIndexes = alreadyRecorded.Select(c => c.DayIndex).ToHashSet();
        var conflicting = request.Candles.Select(c => c.DayIndex).Where(alreadyRecordedDayIndexes.Contains).ToList();
        if (conflicting.Count > 0)
        {
            throw new FxEdgeConflictException(
                $"Entry '{entryId}' already has a candle for day(s): {string.Join(", ", conflicting)}. Use edit to correct those instead.");
        }

        // All-or-nothing: every item is domain-validated (Entity.Create throws on the
        // first invalid one) before anything is added to the repository.
        var candles = request.Candles
            .Select(c => EntryDailyCandle.Create(entryId, c.DayIndex, c.OpenAtr, c.HighAtr, c.LowAtr, c.CloseAtr, c.FirstExtreme))
            .ToList();

        await _candleRepository.AddRangeAsync(candles, ct);
        await _candleRepository.SaveChangesAsync(ct);

        return candles.Select(ToDto).ToList();
    }

    public async Task<EntryDailyCandleDto> EditAsync(Guid entryId, int dayIndex, EditEntryDailyCandleRequest request, CancellationToken ct = default)
    {
        var candle = await _candleRepository.GetAsync(entryId, dayIndex, ct)
            ?? throw new FxEdgeNotFoundException($"No candle found for Entry '{entryId}' at day {dayIndex}.");

        candle.Overwrite(request.OpenAtr, request.HighAtr, request.LowAtr, request.CloseAtr, request.FirstExtreme);
        await _candleRepository.SaveChangesAsync(ct);

        return ToDto(candle);
    }

    public async Task<EntryDailyCandleDto?> GetAsync(Guid entryId, int dayIndex, CancellationToken ct = default)
    {
        var candle = await _candleRepository.GetAsync(entryId, dayIndex, ct);
        return candle is null ? null : ToDto(candle);
    }

    public async Task<IReadOnlyList<EntryDailyCandleDto>> QueryByEntryAsync(Guid entryId, CancellationToken ct = default)
    {
        var candles = await _candleRepository.QueryByEntryAsync(entryId, ct);
        return candles.OrderBy(c => c.DayIndex).Select(ToDto).ToList();
    }

    /// <summary>
    /// Loads the Entry (throwing if it doesn't exist) and validates it has an ATR
    /// reading - OpenAtr/HighAtr/LowAtr/CloseAtr are multiples of that ATR, so
    /// recording candles without one would produce numbers with no defined
    /// price-equivalent meaning.
    /// </summary>
    private async Task<Entry> GetEntryOrThrowAsync(Guid entryId, CancellationToken ct)
    {
        var entry = await _entryRepository.GetByIdAsync(entryId, ct)
            ?? throw new FxEdgeNotFoundException($"Entry '{entryId}' was not found.");

        if (entry.AtrValue is null)
        {
            throw new FxEdgeValidationException(
                $"Entry '{entryId}' has no ATR recorded on its Technical Snapshot - " +
                "OpenAtr/HighAtr/LowAtr/CloseAtr cannot be interpreted without a reference ATR value.");
        }

        return entry;
    }

    private static EntryDailyCandleDto ToDto(EntryDailyCandle candle) => new(
        candle.EntryId, candle.DayIndex, candle.OpenAtr, candle.HighAtr, candle.LowAtr, candle.CloseAtr, candle.FirstExtreme);
}
