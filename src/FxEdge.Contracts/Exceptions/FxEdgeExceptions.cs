namespace FxEdge.Contracts.Exceptions;

/// <summary>
/// Base type for all domain/application-level failures raised by FxEdge. Kept in
/// Contracts (rather than Business) so that FxEdge.Endpoints can catch and translate
/// these into HTTP responses without taking a project reference on FxEdge.Business.
/// </summary>
public abstract class FxEdgeException : Exception
{
    protected FxEdgeException(string message) : base(message)
    {
    }
}

/// <summary>
/// Raised when input fails a business/domain validation rule
/// (e.g. an unsupported currency pair, or a future-dated announcement).
/// Endpoints should translate this into HTTP 400.
/// </summary>
public sealed class FxEdgeValidationException : FxEdgeException
{
    public FxEdgeValidationException(string message) : base(message)
    {
    }
}

/// <summary>
/// Raised when a requested entity (e.g. an observation by Id) does not exist.
/// Endpoints should translate this into HTTP 404.
/// </summary>
public sealed class FxEdgeNotFoundException : FxEdgeException
{
    public FxEdgeNotFoundException(string message) : base(message)
    {
    }
}

/// <summary>
/// Raised when an operation would violate a uniqueness rule
/// (e.g. a duplicate Observation for the same Currency+Feature+AnnouncementAtUtc).
/// Endpoints should translate this into HTTP 409.
/// </summary>
public sealed class FxEdgeConflictException : FxEdgeException
{
    public FxEdgeConflictException(string message) : base(message)
    {
    }
}

/// <summary>
/// Raised when a feature's structure/storage exists but its computation is
/// intentionally not yet defined (e.g. StrategySimulation's engine, pending the exact
/// tie-breaking and horizon-end-closing rules). Endpoints should translate this into
/// HTTP 501, distinct from a validation failure - the request itself is well-formed,
/// the capability just isn't implemented yet.
/// </summary>
public sealed class FxEdgeNotImplementedException : FxEdgeException
{
    public FxEdgeNotImplementedException(string message) : base(message)
    {
    }
}
