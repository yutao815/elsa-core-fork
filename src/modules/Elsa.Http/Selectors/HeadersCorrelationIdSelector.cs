using Elsa.Http.Contracts;
using Microsoft.AspNetCore.Http;

namespace Elsa.Http.Selectors;

/// <summary>
/// Attempts to select a correlation ID from HTTP request headers.
/// </summary>
public class HeadersCorrelationIdSelector : ICorrelationIdSelector
{
    /// <inheritdoc />
    public double Priority => 10;

    /// <inheritdoc />
    public bool TrySelect(HttpRequest request, out string correlationId)
    {
        correlationId = request.Headers["x-correlation-id"].ToString();
        return !string.IsNullOrWhiteSpace(correlationId);
    }
}