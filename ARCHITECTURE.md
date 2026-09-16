# FxEdge — Architecture

Personal application for collecting historical fundamental market data and preparing a
point-in-time, leakage-free dataset structure for future Machine Learning work.

**Scope of this phase:** Database, Data Models, Data Validation, Data Entry/Edit,
Data Retrieval, and Dataset structure preparation — Fundamental data (16 features,
including the derived RealInterestRate and nullable ForecastValue), manually marked
chart Entries with their frozen Fundamental Snapshot, user-entered Technical Features,
the four fully-implemented Fundamental Formula Features, and the four-layer Entry
Result / Entry Daily Candle / Strategy / StrategySimulation structure. No Machine
Learning, Training, or Prediction is implemented yet. One thing is still deliberately
deferred (structure only, errors until supplied): StrategySimulation's engine, pending
how a grid/pyramiding strategy's multi-entry behavior maps onto the available
per-Entry outcome data - see "Open question" below. No UI and no backend tests are
part of this phase either.

## Solution layout

```
FxEdge.slnx
src/
  FxEdge.Contracts/     <- enums, DTOs, service interfaces, shared exceptions (no other project deps)
  FxEdge.Business/       <- domain entities, value objects, catalog, use-case services (depends on Contracts only)
  FxEdge.DataAccess/     <- EF Core + SQLite: DbContext, configuration, repository (depends on Business)
  FxEdge.Endpoints/      <- Minimal API HTTP surface (depends on Contracts only)
  FxEdge.Host/            <- composition root: Program.cs, appsettings.json (depends on all four)
```

### Dependency direction

```
Contracts  <-- Business  <-- DataAccess ---
    ^                                       \
    |                                        Host (composition root)
     ---------- Endpoints --------------------
```

`FxEdge.Contracts` and `FxEdge.Business` have **zero external NuGet dependencies** — pure
.NET/C#. `FxEdge.Endpoints` uses only the built-in ASP.NET Core shared framework (a
`FrameworkReference`, not a NuGet package). Only `FxEdge.DataAccess` needs a real NuGet
package (`Microsoft.EntityFrameworkCore.Sqlite` + `Microsoft.EntityFrameworkCore.Design`
for the `dotnet ef` tooling), and `FxEdge.Host` pulls it in transitively. This was a
deliberate choice so that as much of the codebase as possible stays trivially portable
and dependency-free.

Endpoints never references Business or DataAccess directly — it only knows the service
interfaces declared in Contracts. Host is the only project that wires interfaces to
concrete implementations.

## FxEdge.Contracts

The shared vocabulary every other project talks in. Nothing here has any behavior.

- **`Enums/Currency.cs`** — the fixed 8-currency enum (USD, EUR, GBP, AUD, NZD, CAD, CHF, JPY).
- **`Enums/FundamentalFeature.cs`** — the fixed 16-feature enum, numbered 1–16 to match the original spec order.
- **`Enums/Direction.cs`** — `Buy` / `Sell` for a manually marked Entry.
- **`Enums/TechnicalFeature.cs`** — the fixed 7-technical-feature enum (RSI, ADX, ATR, Candle Body, Candle Range, Upper Shadow, Lower Shadow), numbered 1–7.
- **`Enums/ResultHorizon.cs`** — the fixed 3-horizon enum an Entry's outcome is tracked over: `OneWeek`, `OneMonth`, `TwoMonths`.
- **`Enums/SimulationOutcome.cs`** — how one Entry resolved within a simulation run: `TookProfit`, `HitStopLoss`, `ClosedAtHorizonEnd`.
- **`Enums/PriceExtreme.cs`** — which of an Entry's two excursions (`Favorable`/`Adverse`) was reached first within a Horizon. Resolves StrategySimulation's tie-breaking ambiguity when a strategy's take-profit and stop-loss both fall within `[MAE, MFE]`.
- **`Enums/CandleExtremeOrder.cs`** — which of a single day's raw High/Low was touched first (`HighFirst`/`LowFirst`). Unlike `PriceExtreme`, this is raw/market-terms, not trade-direction-adjusted — matches how `EntryDailyCandle` itself is stored.
- **`Exceptions/FxEdgeExceptions.cs`** — `FxEdgeValidationException` (→ HTTP 400), `FxEdgeNotFoundException` (→ 404), `FxEdgeConflictException` (→ 409), `FxEdgeNotImplementedException` (→ 501, for a structurally-complete feature whose computation isn't defined yet). Live here (not in Business) specifically so Endpoints can catch and translate them without depending on Business.
- **`Dtos/Observations/`** — `ObservationDto` (read model), `CreateObservationRequest`, `EditObservationRequest`, `ObservationQuery` (filter for listing).
- **`Dtos/Catalog/`** — `FeatureDefinitionDto`, `CurrencyPairDto`, `TechnicalFeatureDefinitionDto`: read-only reference data for a future data-entry UI.
- **`Dtos/Dataset/`** — `FeatureSnapshotDto` (one feature's point-in-time Actual/Forecast/Previous state, in raw units), `CurrencyFundamentalSnapshotDto` (one currency's 16-feature snapshot), `PairFeatureDifferentialDto` (one feature's signed, Z-score-normalized Base-minus-Quote differential, plus the raw and normalized inputs for auditability), `FundamentalDatasetRowDto` (one full pair-level dataset row, always recomputed live — the pair-centric, Fundamental-only shape of "a row of the eventual ML dataset"); `EntryDatasetRowDto`/`EntryDatasetRowQuery` (the Entry-centric counterpart — one Entry's full frozen feature set paired with one Horizon's recorded outcome, the actual ML-ready feature+label row).
- **`Dtos/Entries/`** — `CreateEntryRequest` (Base/Quote, EntryAtUtc, EntryPrice, Direction, user-supplied Technical readings), `TechnicalFeatureValueDto` (one technical feature's value, input and output), `EntryFundamentalFeatureSnapshotDto` (one feature's *frozen* dual-currency state, raw and normalized — see below), `EntryFormulaFeaturesDto` (the four Formula_1W/1M/3M/1Y values), `EntryDto` (full read model, including `AtrValue` — see below), `EntrySummaryDto` (lightweight list view), `EntryQuery` (filter for listing).
- **`Dtos/EntryResults/`** — `EntryResultDto`, `CreateEntryResultRequest`, `EditEntryResultRequest`: an Entry's manually-observed MFE, MAE, closing return at horizon end (`ReturnAtHorizonEndAtr`), and which extreme happened first (`FirstExtremeReached`) for one Horizon (Layer 1).
- **`Dtos/EntryDailyCandles/`** — `EntryDailyCandleDto`, `CreateEntryDailyCandleRequest`, `EditEntryDailyCandleRequest`, `CreateEntryDailyCandlesBatchRequest`: one Entry's optional, opt-in daily OHLC candle (raw, ATR-normalized) for one of up to 40 trading days after entry — a more precise, supplementary alternative to EntryResult's whole-horizon summary, for accurately simulating a multi-entry grid strategy's day-by-day behavior.
- **`Dtos/Strategies/`** — `StrategyDto`, `CreateStrategyRequest`, `EditStrategyRequest` (Layer 2 - a named ATR-based scaling config); `StrategySimulationDto`, `StrategySimulationEntryOutcomeDto` (Layer 3 - a computed backtest-style run and its per-entry breakdown).
- **`Services/`** — `IObservationService`, `IEntryService`, `IEntryResultService`, `IEntryDailyCandleService`, `IStrategyService`, `IStrategySimulationService`, `IDatasetService`, `IEntryDatasetService`, `ICatalogService`: the nine contracts Business implements and Endpoints consumes.

## FxEdge.Business

All domain rules and use-case logic. No EF Core, no ASP.NET Core, no I/O — everything
here is plain, testable C#.

- **`Common/DateTimeUtc.cs`** *(internal)* — shared `Normalize(DateTime)` helper that stamps `DateTimeKind.Utc` consistently; used by every entity/service that handles a UTC timestamp, so the Kind-handling logic exists in exactly one place.
- **`Entities/FundamentalObservation.cs`** — the only *independently created* entity FxEdge persists. One Currency + one Feature + one AnnouncementAtUtc + ForecastValue + Value. `Create()` produces a brand-new, application-generated `Guid` and enforces that AnnouncementAtUtc can't be in the future (Value is always the actual published figure, never a placeholder — required). `ForecastValue` is nullable: some features (or specific announcements) genuinely never have a published market forecast, and `null` here means exactly that rather than a fabricated number. `Overwrite()` exists purely to correct a mistaken entry in place — it is not part of normal historical accumulation, which always goes through `Create()`. **RealInterestRate is the one exception**: `ObservationService` rejects direct create/edit for it - see `RealInterestRateDerivationService` below, the only code path allowed to write RealInterestRate rows.
- **`Entities/Entry.cs`** — a manually marked chart entry point (no relation to any automatic detection). Aggregate root: Base/Quote currency, EntryAtUtc, EntryPrice, Direction, plus the four `Formula1W/1M/3M/1Y` fields and its two owned child collections (below). Exposes `AtrValue` (a computed lookup into `TechnicalSnapshot` for the `Atr` reading) - the single reference every "...Atr"-suffixed quantity elsewhere (`EntryResult`, `EntryDailyCandle`, and a `Strategy`'s parameters once simulated) is a multiple of. **Fully immutable once created** — there is intentionally no edit/delete for Entry in this phase, since its snapshots are historical by design. `Create()` takes everything (including the already-built child snapshots) atomically, validates EntryPrice > 0, Direction is defined, EntryAtUtc isn't in the future, and that Base/Quote form one of the 28 supported pairs.
- **`Entities/EntryFundamentalFeatureSnapshot.cs`** — one feature's fundamental state *as it stood at Entry time, frozen permanently*: BaseValue/ForecastValue/PreviousValue/ActualVsForecast/ActualVsPrevious/AnnouncementAtUtc and the same six fields for Quote, plus the frozen Polarity and Difference. 16 of these per Entry. Unlike `FundamentalDatasetRowDto` (always recomputed live), this is captured once and never changes again — even if a later `Overwrite()` corrects the underlying Observation data, an Entry that already happened keeps what was true at the time.
- **`Entities/EntryTechnicalFeatureSnapshot.cs`** — one technical feature's user-entered value at Entry time, frozen permanently. 7 of these per Entry (one per `TechnicalFeature`, value nullable if the user didn't have that reading).
- **`Entities/EntryResult.cs`** — one Entry's manually-observed outcome for one Horizon (Layer 1): MFE, MAE, `ReturnAtHorizonEndAtr` (the signed closing return relative to entry price - resolves how a trade that reached neither TP nor SL should be scored), and `FirstExtremeReached` (whether MFE or MAE happened first - resolves StrategySimulation's tie-breaking ambiguity). Validated so `ReturnAtHorizonEndAtr` always falls within `[-MAE, +MFE]` (the close can't be more extreme than the horizon's own peaks), and so the parent Entry has an `AtrValue` (these fields are meaningless without one). Unlike Entry's atomic snapshots, this is recorded separately and later - only once the horizon has actually elapsed - so it supports `Overwrite()` for correcting a misread chart value, like `FundamentalObservation`.
- **`Entities/EntryDailyCandle.cs`** — one Entry's optional, opt-in daily OHLC candle (raw price-direction terms, ATR-normalized - `OpenAtr`/`HighAtr`/`LowAtr`/`CloseAtr`) for a 1-based `DayIndex` in `[1, 40]`, plus `FirstExtreme` (`CandleExtremeOrder`: whether that day's raw High or Low was touched first). Never trade-direction-adjusted, unlike `EntryResult` - so entering it matches reading a chart directly. Validated as standard OHLC (`High >= Open, Close, Low`; `Low <= Open, Close, High`) and, like `EntryResult`, requires the parent Entry to have an `AtrValue`. Supplements `EntryResult`, never replaces it - the 3 Horizons are nested trading-day windows (1W = days 1-5, 1M = days 1-20, 2M = days 1-40), so a single up-to-40-day series covers all of them.
- **`Entities/Strategy.cs`** — a named, reusable ATR-based scaling strategy definition (Layer 2): entry spacing, per-entry take-profit, overall stop-loss, max entries. Configuration, not history, so `Overwrite()` is a normal tuning operation, not just a correction.
- **`Entities/StrategySimulation.cs`** — the aggregate root for a computed backtest-style run (Layer 3): Profit/Loss/win-rate/drawdown in ATR, plus an owned `EntryOutcomes` collection (same backing-field pattern as `Entry`'s two collections). Computed only, via `StrategySimulationService` - no `Overwrite()`, a new run is created each time instead.
- **`Entities/StrategySimulationEntryOutcome.cs`** — one Entry's resolved outcome within a simulation run (`TookProfit` / `HitStopLoss` / `ClosedAtHorizonEnd` + the ATR result it contributed).
- **`ValueObjects/CurrencyPair.cs`** — Base+Quote value object, constructor-validated against exactly the 28 supported, direction-locked pairs. Never persisted; a pair's fundamental picture is always computed from its two currencies' observations.
- **`Catalog/FundamentalFeatureCatalog.cs`** — the single fixed configuration file for the 16 features: real display names (Fa/En) and **Polarity** (+1 if a higher reading favors the currency, −1 if it doesn't — e.g. Unemployment Rate and Debt/GDP are −1, everything else is +1 per the confirmed mapping).
- **`Catalog/TechnicalFeatureCatalog.cs`** — the equivalent fixed definition file for the 7 technical features (display names only; no polarity concept applies, since these describe the traded instrument's chart, not a currency being compared against another).
- **`Abstractions/IObservationRepository.cs`** — the persistence port for `FundamentalObservation`. Implemented by DataAccess (Dependency Inversion: Business owns the interface, DataAccess is the adapter).
- **`Abstractions/IEntryRepository.cs`** — the persistence port for `Entry` (loads it together with both owned snapshot collections on `GetByIdAsync`).
- **`Abstractions/IEntryResultRepository.cs`**, **`IEntryDailyCandleRepository.cs`**, **`IStrategyRepository.cs`**, **`IStrategySimulationRepository.cs`** — persistence ports for the four Layer 1/2/3 (+ candle) entities. `IEntryDailyCandleRepository` additionally exposes `AddRangeAsync` for the batch-create path.
- **`Abstractions/IFormulaFeatureCalculator.cs`** — computes one Fundamental Formula Feature from an already-resolved map of per-feature `Difference` values (`FormulaPeriod`: OneWeek/OneMonth/ThreeMonths/OneYear). Synchronous/pure (no I/O) - `Z(Feature)` in the formula spec *is* the Entry's already-computed Difference, so there's nothing left to fetch. Public (not internal) specifically so Host can register a concrete implementation for it.
- **`Abstractions/IStrategySimulationEngine.cs`** — the computational core of Layer 3: given a `Strategy` and every `EntryResult` recorded for a Horizon, produces the aggregate metrics and per-entry outcomes (`StrategySimulationComputation`/`EntrySimulationOutcome`). Structurally wired end-to-end; see the placeholder implementation and the open question below.
- **`Services/ObservationService.cs`** — implements `IObservationService`. Owns duplicate-prevention (same Currency+Feature+AnnouncementAtUtc) for both create and edit. Rejects direct create/edit of `RealInterestRate` (→ `FxEdgeValidationException`); after successfully creating/editing `CentralBankInterestRate` or `CoreCpi`, triggers `RealInterestRateDerivationService` for that same Currency+AnnouncementAtUtc.
- **`Services/RealInterestRateDerivationService.cs`** *(internal)* — RealInterestRate is never entered directly; it's always `CentralBankInterestRate − CoreCpi`, recomputed and persisted (upserted, keyed by the exact AnnouncementAtUtc) every time either input changes for a currency, using the *other* input's point-in-time carried-forward value. If either input has no data yet as of that date, nothing is written - consistent with the system's "null rather than fabricate" rule. Known limitation: only the triggering date is recomputed - editing an *earlier* CentralBankInterestRate/CoreCpi observation does not cascade forward to re-derive later RealInterestRate rows that had carried its old value forward. Because the derived rows are ordinary `FundamentalObservation` records, `PointInTimeResolver`, `FeatureNormalizer`, and Entry snapshots all treat RealInterestRate exactly like any manually-entered feature - `ActualVsForecast`/`ActualVsPrevious` are computed for it automatically, with no special-casing anywhere else in the codebase.
- **`Services/EntryService.cs`** — implements `IEntryService`. On `CreateAsync`: validates the pair, resolves all 16 features' Base+Quote point-in-time state strictly as of `EntryAtUtc` (reusing `PointInTimeResolver` and `DifferentialCalculator` below — this is exactly what keeps the Entry's fundamental picture leakage-free), builds the 7-entry technical snapshot from whatever the caller supplied (missing features become `null`), builds a Feature→Difference map from the just-built Fundamental Snapshot and calls `IFormulaFeatureCalculator` four times against it, then hands everything to `Entry.Create()` in one atomic call.
- **`Services/EntryResultService.cs`** — implements `IEntryResultService`. Loads the parent Entry (via `IEntryRepository`) before creating a result, checking both that it exists and that it has an `AtrValue`; owns duplicate-prevention per (EntryId, Horizon).
- **`Services/EntryDailyCandleService.cs`** — implements `IEntryDailyCandleService`. Same Entry-existence/`AtrValue` check as `EntryResultService`. `CreateBatchAsync` validates every candle (including within-batch duplicate DayIndexes and conflicts with already-recorded days) before writing anything - all-or-nothing, no partial batches.
- **`Services/StrategyService.cs`** — implements `IStrategyService`; plain CRUD over `Strategy`.
- **`Services/StrategySimulationService.cs`** — implements `IStrategySimulationService`. `RunAsync` loads the `Strategy` and every `EntryResult` for the requested Horizon, delegates the actual computation to `IStrategySimulationEngine`, then persists the result as a new `StrategySimulation`.
- **`Services/PointInTimeResolver.cs`** *(internal)* — for one Currency+Feature+AsOf date, finds the observation in effect (latest with `AnnouncementAtUtc <= AsOf`, i.e. carry-forward) and the one immediately before it, then derives `ActualVsForecast` and `ActualVsPrevious`. Returns an all-null snapshot if nothing has been announced yet as of that date — this is what keeps every consumer (DatasetService *and* EntryService) leakage-free instead of fabricating data.
- **`Services/FeatureNormalizer.cs`** *(internal)* — computes the point-in-time Z-score of a Currency+Feature's current value against its own history: `(Value − Mean) / SampleStdDev`, where Mean/SampleStdDev are taken exclusively from that same Currency+Feature's observations announced at or before AsOfUtc (never later — same leakage-prevention principle as everything else). Requires at least 2 historical points (a sample standard deviation is undefined otherwise) and a non-zero StdDev; returns `null` if either doesn't hold. This — not the raw `Value` — is what `DifferentialCalculator` uses; `ForecastValue`/`PreviousValue`/`ActualVs*` are left untouched.
- **`Services/DifferentialCalculator.cs`** *(internal)* — `DifferentialValue = Polarity * (BaseNormalizedValue − QuoteNormalizedValue)` for one feature (both normalized by `FeatureNormalizer`); null-propagates if either side's normalization is unavailable. Shared by `DatasetService` (live) and `EntryService` (frozen at Entry time).
- **`Services/DatasetService.cs`** — implements `IDatasetService`. Builds a currency's 16-feature snapshot, a single pair's full dataset row (Base snapshot + Quote snapshot + 16 differentials), or every one of the 28 pairs' rows for the same date. Always recomputed live from current Observation data (contrast with Entry's frozen snapshot).
- **`Services/EntryDatasetService.cs`** — implements `IEntryDatasetService`. Purely a composition of `IEntryService` + `IEntryResultService` - no repository dependencies, no mapping logic of its own. For each matching Entry, pairs its (already-built) `EntryDto` with every recorded `EntryResultDto` (optionally filtered to one Horizon), producing one `EntryDatasetRowDto` per (Entry, Horizon) combination that has a result. N+1-ish (one `GetByIdAsync` + one `QueryByEntryAsync` per matching Entry), matching the same personal-scale tradeoff as `DatasetService.GetAllPairsDatasetRowsAsync`.
- **`Services/CatalogService.cs`** — implements `ICatalogService`; thin read-only wrapper exposing the currency list, `FundamentalFeatureCatalog`, `TechnicalFeatureCatalog`, and `CurrencyPair.All` as DTOs.
- **`Services/FormulaFeatureCalculator.cs`** — implements `IFormulaFeatureCalculator`: each of the four formulas is a fixed weighted average of `Z(Feature)` terms (weights sum to 1.00 for every horizon), where `Z(Feature)` is exactly the Polarity-adjusted `Difference` already resolved in the Entry's Fundamental Snapshot passed in - no extra I/O, pure computation. If any feature a given formula needs is missing (`Difference == null`), that formula's result is `null` (not a partial/fabricated number); other formulas that don't depend on the missing feature are unaffected.
- **`Services/NotYetDefinedStrategySimulationEngine.cs`** — the current placeholder `IStrategySimulationEngine`: always throws `FxEdgeNotImplementedException` (→ HTTP 501). Swapping this is the only change needed once the open question below is answered.

`FxEdge.Business` has **no DI-registration extension method** (unlike DataAccess). This
is deliberate: adding one would require a `Microsoft.Extensions.DependencyInjection.Abstractions`
package reference purely for that convenience, which would cost Business its
zero-dependency status. Host registers Business's services directly in `Program.cs`
instead.

## FxEdge.DataAccess

The only project that knows SQLite/EF Core exist.

- **`FxEdgeDbContext.cs`** — the single `DbContext`; exposes `DbSet<FundamentalObservation> Observations` and `DbSet<Entry> Entries`. `CurrencyPair`, every Dataset DTO, and `Entry.Pair`/`Entry.AtrValue` are computed on the fly and are never mapped to a table. `EntryFundamentalFeatureSnapshot`/`EntryTechnicalFeatureSnapshot` have no separate `DbSet<T>` — they're reachable only through `Entry`'s navigations, matching an aggregate-root-only exposure pattern.
- **`Configurations/UtcDateTimeConverters.cs`** *(internal)* — the shared `ValueConverter` pair (nullable and non-nullable) that re-stamps `DateTimeKind.Utc` on every UTC column read back from SQLite (which doesn't round-trip `DateTimeKind`). Used by every configuration below instead of each defining its own.
- **`Configurations/FundamentalObservationConfiguration.cs`** — Fluent API mapping only (no EF attributes leak into the Business entity): table `Observations`, `Id` is `ValueGeneratedNever()` (the app always supplies the Guid), `Currency`/`Feature` stored as strings for a human-readable local DB, and a unique index on `(Currency, Feature, AnnouncementAtUtc)` as a database-level backstop for the duplicate rule `ObservationService` already enforces.
- **`Configurations/EntryConfiguration.cs`** — table `Entries`; `Id` is `ValueGeneratedNever()`; `Pair` is `.Ignore()`d (computed, not persisted). Maps the two owned collections (`FundamentalSnapshot`, `TechnicalSnapshot`) via `HasMany(...).WithOne().HasForeignKey(...)` with cascade delete, and — because both are exposed only as `IReadOnlyList<T>` backed by private `List<T>` fields — explicitly sets `.Navigation(...).UsePropertyAccessMode(PropertyAccessMode.Field)` so EF Core reads/writes the backing field directly instead of the read-only property. A non-unique index on `(BaseCurrency, QuoteCurrency, EntryAtUtc)` supports filtered listing.
- **`Configurations/EntryFundamentalFeatureSnapshotConfiguration.cs`** — table `EntryFundamentalFeatureSnapshots`, composite key `(EntryId, Feature)` — exactly 16 rows per Entry.
- **`Configurations/EntryTechnicalFeatureSnapshotConfiguration.cs`** — table `EntryTechnicalFeatureSnapshots`, composite key `(EntryId, Feature)` — exactly 7 rows per Entry.
- **`Configurations/EntryResultConfiguration.cs`** — table `EntryResults`, composite key `(EntryId, Horizon)` — at most 3 rows per Entry. Independent table, not FK-linked to `Entries` (results are recorded separately and later).
- **`Configurations/EntryDailyCandleConfiguration.cs`** — table `EntryDailyCandles`, composite key `(EntryId, DayIndex)` — up to 40 rows per Entry. Independent table, same rationale as `EntryResultConfiguration`.
- **`Configurations/StrategyConfiguration.cs`** — table `Strategies`; `Id` is `ValueGeneratedNever()`.
- **`Configurations/StrategySimulationConfiguration.cs`** — table `StrategySimulations`; `Id` is `ValueGeneratedNever()`; maps the owned `EntryOutcomes` collection the same backing-field way as `EntryConfiguration`.
- **`Configurations/StrategySimulationEntryOutcomeConfiguration.cs`** — table `StrategySimulationEntryOutcomes`, composite key `(StrategySimulationId, EntryId)`.
- **`Repositories/ObservationRepository.cs`** — EF Core implementation of `IObservationRepository`: add, get-by-id, exists-check, filtered query, and the two point-in-time lookups (`GetLatestAsOfAsync`, `GetLatestBeforeAsync`).
- **`Repositories/EntryRepository.cs`** — EF Core implementation of `IEntryRepository`: add, filtered query (list views — no `Include`, kept light since `EntrySummaryDto` doesn't need the snapshots), and `GetByIdAsync` which `Include`s both `FundamentalSnapshot` and `TechnicalSnapshot`.
- **`Repositories/EntryResultRepository.cs`**, **`EntryDailyCandleRepository.cs`**, **`StrategyRepository.cs`**, **`StrategySimulationRepository.cs`** — straightforward EF Core implementations of the Layer 1/2/3 (+ candle) repository ports.
- **`DependencyInjection/DataAccessServiceCollectionExtensions.cs`** — `AddFxEdgeDataAccess(connectionString)`: registers the `DbContext` (SQLite) and all six repositories. Host calls this once and never touches EF Core types directly.
- **`FxEdgeDbContextDesignTimeFactory.cs`** — lets `dotnet ef migrations add/database update` create a `DbContext` directly against this project without needing `--startup-project` gymnastics. Only used by the CLI tooling; has no effect on the running app.

## FxEdge.Endpoints

The HTTP surface. Talks only in `Contracts` types (DTOs + service interfaces), so it has
no idea EF Core or SQLite exist.

- **`Shared/FxEdgeExceptionMapper.cs`** — maps `FxEdgeValidationException → 400`, `FxEdgeNotFoundException → 404`, `FxEdgeConflictException → 409`, `FxEdgeNotImplementedException → 501` via a `Results.Problem(...)` response. Used by every endpoint that can fail.
- **`Observations/ObservationEndpoints.cs`** — `POST /api/observations` (create), `PUT /api/observations/{id}` (correct in place — Id always comes from the route, never duplicated in the body), `GET /api/observations/{id}`, `GET /api/observations` (filterable list).
- **`Entries/EntryEndpoints.cs`** — `POST /api/entries` (create — resolves and freezes the Fundamental Snapshot, stores the Technical Snapshot, computes the Formula Features), `GET /api/entries/{id}` (full `EntryDto`), `GET /api/entries` (filterable list of `EntrySummaryDto`). No `PUT`/`DELETE` — Entries are immutable by design.
- **`EntryResults/EntryResultEndpoints.cs`** — nested under `/api/entries/{entryId}/results`: `POST /` (create), `PUT /{horizon}` (correct in place), `GET /{horizon}`, `GET /` (all results for that Entry).
- **`EntryDailyCandles/EntryDailyCandleEndpoints.cs`** — nested under `/api/entries/{entryId}/candles`: `POST /` (create one day), `POST /batch` (create many days at once, all-or-nothing), `PUT /{dayIndex}` (correct in place), `GET /{dayIndex}`, `GET /` (all candles for that Entry).
- **`Strategies/StrategyEndpoints.cs`** — `POST /api/strategies`, `PUT /api/strategies/{id}`, `GET /api/strategies/{id}`, `GET /api/strategies` (list).
- **`Strategies/StrategySimulationEndpoints.cs`** — `POST /api/strategies/{strategyId}/simulations?horizon=` (run a new simulation — currently always returns 501, see `NotYetDefinedStrategySimulationEngine`), `GET /api/strategies/{strategyId}/simulations` (past runs), `GET /api/simulations/{id}` (one run's full detail).
- **`Dataset/DatasetEndpoints.cs`** — `GET /api/dataset/currencies/{currency}/snapshot?asOfUtc=`, `GET /api/dataset/pairs/{baseCurrency}/{quoteCurrency}/row?asOfUtc=`, `GET /api/dataset/rows?asOfUtc=` (all 28 pairs at once).
- **`Dataset/EntryDatasetEndpoints.cs`** — the Entry-centric ML export: `GET /api/dataset/entries?baseCurrency=&quoteCurrency=&fromUtc=&toUtc=&horizon=` (every matching `(Entry, Horizon)` row, filterable), `GET /api/dataset/entries/{entryId}` (all rows for one Entry).
- **`Catalog/CatalogEndpoints.cs`** — `GET /api/catalog/currencies`, `/features`, `/pairs`, `/technical-features`.
- **`EndpointRouteBuilderExtensions.cs`** — `MapFxEdgeEndpoints()`: the one call Host makes to wire up every group above.

## FxEdge.Host

The composition root — the only project allowed to know about concrete
implementations.

- **`Program.cs`** — reads the `FxEdge` connection string from configuration, calls `AddFxEdgeDataAccess`, registers Business's services directly (`IObservationService`, `IEntryService`, `IEntryResultService`, `IEntryDailyCandleService`, `IStrategyService`, `IStrategySimulationService`, `IDatasetService`, `IEntryDatasetService`, `ICatalogService`, `IFormulaFeatureCalculator` → `FormulaFeatureCalculator`, and the still-placeholder `IStrategySimulationEngine`), applies any pending EF Core migrations automatically on startup (`Database.Migrate()` — reasonable for a single-user local app, avoids a manual step every run), then calls `MapFxEdgeEndpoints()`.
- **`appsettings.json`** — `ConnectionStrings:FxEdge = "Data Source=fxedge.db"`. The database file is created next to wherever the app runs from.

## Key domain rules, in one place

- **Historical/append-only**: a newly published figure for a later `AnnouncementAtUtc` always creates a brand-new `FundamentalObservation` via `Create()`. `Overwrite()` is only for fixing a mistaken entry and keeps no history of the incorrect values.
- **RealInterestRate is derived, not entered**: `ObservationService` rejects direct create/edit of it; `RealInterestRateDerivationService` recomputes it (as `CentralBankInterestRate − CoreCpi`) at the triggering observation's own date every time either input changes. Once written, it's an ordinary `FundamentalObservation` - carry-forward, normalization, and Entry snapshots treat it identically to a manually-entered feature.
- **ForecastValue can be `null`**: some features (or specific announcements) genuinely never have a published market forecast. `Value` stays required (an observation only exists once the actual figure is known); `ForecastValue` doesn't. `ActualVsForecast` becomes `null` automatically in that case (nullable-lifted subtraction), not a fabricated 0 or a crash.
- **Carry-forward point-in-time**: "the CPI as of 2026-07-10" resolves to the latest observation with `AnnouncementAtUtc <= 2026-07-10`, whatever that value was, until the next announcement supersedes it.
- **Pair fundamentals are never stored**: `EUR/USD CPI` is always `Polarity(CPI) * (EUR.CPI_normalized − USD.CPI_normalized)`, computed at read time from the two currencies' independent observation histories.
- **Values are Z-score normalized before differencing**: raw readings across the 16 features live on wildly different scales (interest rates in %, trade balance in currency units, PMI around 50, ...), so before computing a pair's differential, each side's `Value` is normalized against its own point-in-time history via `FeatureNormalizer`: `(Value − Mean) / SampleStdDev`. `ForecastValue`, `PreviousValue`, and the derived `ActualVs*` figures are left in raw units - only the value feeding `Difference` is normalized. Needs at least 2 historical points and a non-zero StdDev; `null` otherwise (same null-safety principle as everywhere else, not a fabricated 0).
- **Polarity** flips the sign for features where "higher is worse for the currency" (Unemployment Rate, Debt/GDP), so a positive differential consistently means "favors the Base currency" across all 16 features.
- **No look-ahead**: every dataset value - and now every normalization Mean/StdDev - is resolved strictly from observations with `AnnouncementAtUtc <= AsOfUtc`. If nothing had been announced yet, the field is `null`, never fabricated.
- **Entries are frozen, not live**: an Entry's Fundamental Snapshot (raw *and* normalized values, and the Difference derived from them) is resolved once, at creation, strictly from observations with `AnnouncementAtUtc <= EntryAtUtc`, then persisted as-is. Unlike `IDatasetService` (always recomputed from current data), an Entry never changes again — even a later correction to historical Observation data (or, going forward, a change to how normalization is computed) won't retroactively alter an Entry that already happened. Technical readings are equally frozen: entered once by the user at Entry time, never recalculated.
- **Entries have no automatic detection**: the entry point is always identified manually by the user on the chart; the system never infers or suggests one.
- **Formula Features are fixed, confirmed weighted averages**: `Formula1W/1M/3M/1Y` are each a weighted sum of `Z(Feature) = Difference(Feature)` (the same Polarity-adjusted Difference already frozen in the Fundamental Snapshot), computed once at Entry creation from that same snapshot - no separate normalization pass. If any feature a formula needs is missing, only *that* formula is `null`, never a fabricated partial result.
- **EntryResult is manual, not computed**: there is no price-history store in this system, so MFE/MAE/ReturnAtHorizonEndAtr per horizon are read off the chart by the user, the same way Technical Features are - not calculated by the app.
- **EntryDailyCandle is optional and supplementary, never a replacement**: `EntryResult`'s whole-horizon summary stays the quick, always-usable input; a candle series is opt-in, more laborious to enter (up to 40 days), and exists purely to let `StrategySimulation` simulate a grid/pyramiding strategy's day-by-day, multi-level behavior more precisely than endpoint statistics alone allow.
- **Candle values are raw, not trade-direction-adjusted**: unlike MFE/MAE ("Favorable"/"Adverse"), `EntryDailyCandle`'s Open/High/Low/Close are plain ATR-distance-from-entry in the market's own price direction - entering one never requires mentally adjusting for the trade's Buy/Sell direction. Direction-aware interpretation (if ever needed) happens downstream, in computation, not in the stored data.
- **Every "...Atr"-suffixed quantity shares one reference**: `Entry.AtrValue` (the `Atr` reading frozen on that Entry's TechnicalSnapshot) is the single number every ATR-multiple elsewhere - `EntryResult`'s MFE/MAE/ReturnAtHorizonEndAtr, `EntryDailyCandle`'s OHLC, and a `Strategy`'s EntrySpacingAtr/TakeProfitPerEntryAtr/OverallStopLossAtr once simulated - must be interpreted against. `EntryResultService` and `EntryDailyCandleService` both reject recording data for an Entry whose `AtrValue` is `null`, since those numbers would otherwise have no defined price-equivalent meaning.
- **Strategy is configuration, not history**: unlike Entry/Observation, editing a Strategy in place is the normal way parameters get tuned, not just a correction mechanism.
- **StrategySimulation is a placeholder for now**: the full pipeline (persistence, DTOs, endpoints) is wired, but `RunAsync` currently always throws `FxEdgeNotImplementedException` (→ HTTP 501) via `NotYetDefinedStrategySimulationEngine` - see "Open question" below (grid/pyramiding semantics - tie-breaking itself is resolved via `FirstExtremeReached`/`EntryDailyCandle.FirstExtreme`).

## Fundamental Formula Feature weights (confirmed, `FormulaFeatureCalculator`)

Each `Z(Feature)` below is the Polarity-adjusted `Difference` already resolved for that
feature in the Entry's Fundamental Snapshot. Weights sum to 1.00 within each formula.

| Formula_1W | Formula_1M | Formula_3M | Formula_1Y |
|---|---|---|---|
| 0.40 MarketRateExpectation | 0.25 MarketRateExpectation | 0.17 GdpGrowthRate | 0.20 GdpGrowthRate |
| 0.25 TwoYearBondYield | 0.20 CentralBankInterestRate | 0.15 RealInterestRate | 0.16 RealInterestRate |
| 0.20 CentralBankInterestRate | 0.18 CoreCpi | 0.13 CentralBankInterestRate | 0.15 CurrentAccount |
| 0.15 Pmi | 0.15 TwoYearBondYield | 0.13 TenYearBondYield | 0.12 TenYearBondYield |
| | 0.12 Pmi | 0.12 TradeBalance | 0.10 DebtToGdp |
| | 0.10 UnemploymentRate | 0.10 WageGrowth | 0.10 GovernmentBudget |
| | | 0.10 UnemploymentRate | 0.10 InflationRate |
| | | 0.10 Pmi | 0.07 RealSpendingPerCapita |

## Open question (blocking StrategySimulation's real engine)

`EntryResult` records four values per Entry per Horizon: MFE, MAE, `ReturnAtHorizonEndAtr`,
and `FirstExtremeReached` - enough to unambiguously simulate a **single**
take-profit/stop-loss against the *original* entry price. `EntryDailyCandle` now adds
an optional, day-by-day OHLC series (up to 40 trading days, `FirstExtreme` resolving
same-day High/Low order) - enough, when present, to simulate whether/when price
reached the *second*, *third*, ... grid levels FxEdge's `Strategy` places
`EntrySpacingAtr` apart, and in what order relative to each level's own
`TakeProfitPerEntryAtr` and the basket's `OverallStopLossAtr`.

What's still unresolved: `EntryDailyCandle` is opt-in, so not every Entry will have
it - `NotYetDefinedStrategySimulationEngine`'s replacement needs a defined fallback for
Entries that only have `EntryResult` (single-entry approximation? exclude them from
grid-aware simulations? something else?), and the exact day-by-day grid-level-crossing
algorithm itself (walking each day's OHLC against the spacing/TP/SL levels, in
`FirstExtreme` order) still needs to be specified before implementing it.

Everything around this remaining question - the `Strategy`/`EntryResult` schema,
`StrategySimulation`'s persisted shape, and every endpoint - is already final; only
`NotYetDefinedStrategySimulationEngine`'s replacement will change.

## Getting started

This sandbox could not reach `nuget.org` (only GitHub/npm/PyPI/crates domains are
reachable here), so `FxEdge.Contracts`, `FxEdge.Business`, and `FxEdge.Endpoints` were
each built and verified with `dotnet build` (0 errors) after every change, but
`FxEdge.DataAccess` and `FxEdge.Host` could not be restored/built in this environment —
their EF Core-dependent code was reviewed carefully by hand instead. On your own
machine, with normal internet access, the standard steps apply:

```bash
# from the FxEdge/ folder
dotnet restore
dotnet build

# first time only:
dotnet tool install --global dotnet-ef   # if you don't already have it
dotnet ef migrations add InitialCreate --project src/FxEdge.DataAccess --startup-project src/FxEdge.Host

# if you already had earlier migrations, add a follow-up one instead, e.g.:
dotnet ef migrations add AddEntryDailyCandles --project src/FxEdge.DataAccess --startup-project src/FxEdge.Host

# run (Program.cs also auto-applies any pending migration on every startup)
dotnet run --project src/FxEdge.Host
```

The API will listen on the ASP.NET Core default local URLs (printed to the console on
startup). `fxedge.db` is created in the working directory the app is run from.
