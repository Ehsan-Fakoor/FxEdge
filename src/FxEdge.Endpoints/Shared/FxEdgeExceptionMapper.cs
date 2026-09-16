using FxEdge.Contracts.Exceptions;
using Microsoft.AspNetCore.Http;

namespace FxEdge.Endpoints.Shared;

/// <summary>
/// Maps the three FxEdge domain exceptions to the HTTP status codes Endpoints promises:
/// validation -> 400, not found -> 404, conflict -> 409. Kept in one place so every
/// endpoint reports errors the same way instead of re-implementing this mapping.
/// </summary>
internal static class FxEdgeExceptionMapper
{
    public static IResult ToProblem(this FxEdgeException exception) => exception switch
    {
        FxEdgeValidationException => Results.Problem(detail: exception.Message, statusCode: StatusCodes.Status400BadRequest, title: "Validation error"),
        FxEdgeNotFoundException => Results.Problem(detail: exception.Message, statusCode: StatusCodes.Status404NotFound, title: "Not found"),
        FxEdgeConflictException => Results.Problem(detail: exception.Message, statusCode: StatusCodes.Status409Conflict, title: "Conflict"),
        FxEdgeNotImplementedException => Results.Problem(detail: exception.Message, statusCode: StatusCodes.Status501NotImplemented, title: "Not implemented"),
        _ => Results.Problem(detail: exception.Message, statusCode: StatusCodes.Status500InternalServerError, title: "Unexpected error")
    };
}
