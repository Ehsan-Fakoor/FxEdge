using FxEdge.Business.Abstractions;
using FxEdge.Business.Entities;
using FxEdge.Contracts.Dtos.Strategies;
using FxEdge.Contracts.Exceptions;
using FxEdge.Contracts.Services;

namespace FxEdge.Business.Services;

/// <inheritdoc cref="IStrategyService"/>
public sealed class StrategyService : IStrategyService
{
    private readonly IStrategyRepository _repository;

    public StrategyService(IStrategyRepository repository)
    {
        _repository = repository;
    }

    public async Task<StrategyDto> CreateAsync(CreateStrategyRequest request, CancellationToken ct = default)
    {
        var strategy = Strategy.Create(
            request.Name, request.EntrySpacingAtr, request.TakeProfitPerEntryAtr, request.OverallStopLossAtr, request.MaxEntries);

        await _repository.AddAsync(strategy, ct);
        await _repository.SaveChangesAsync(ct);

        return ToDto(strategy);
    }

    public async Task<StrategyDto> EditAsync(Guid id, EditStrategyRequest request, CancellationToken ct = default)
    {
        var strategy = await _repository.GetByIdAsync(id, ct)
            ?? throw new FxEdgeNotFoundException($"Strategy '{id}' was not found.");

        strategy.Overwrite(
            request.Name, request.EntrySpacingAtr, request.TakeProfitPerEntryAtr, request.OverallStopLossAtr, request.MaxEntries);
        await _repository.SaveChangesAsync(ct);

        return ToDto(strategy);
    }

    public async Task<StrategyDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var strategy = await _repository.GetByIdAsync(id, ct);
        return strategy is null ? null : ToDto(strategy);
    }

    public async Task<IReadOnlyList<StrategyDto>> QueryAsync(CancellationToken ct = default)
    {
        var strategies = await _repository.QueryAsync(ct);
        return strategies.Select(ToDto).ToList();
    }

    private static StrategyDto ToDto(Strategy strategy) => new(
        strategy.Id, strategy.Name, strategy.EntrySpacingAtr, strategy.TakeProfitPerEntryAtr,
        strategy.OverallStopLossAtr, strategy.MaxEntries);
}
