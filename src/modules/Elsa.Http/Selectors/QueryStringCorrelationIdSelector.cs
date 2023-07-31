using Elsa.Http.Contracts;
using Microsoft.AspNetCore.Http;

namespace Elsa.Http.Selectors;

/// <summary>
/// Attempts to select a correlation ID from the query string.
/// </summary>
public class QueryStringCorrelationIdSelector : ICorrelationIdSelector
{
    /// <inheritdoc />
    public double Priority => 20;

    /// <inheritdoc />
    public bool TrySelect(HttpRequest request, out string correlationId)
    {
        correlationId = request.Query["correlationId"].ToString();
        return !string.IsNullOrWhiteSpace(correlationId);
    }
}