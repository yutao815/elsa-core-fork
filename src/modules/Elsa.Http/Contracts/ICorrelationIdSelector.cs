using Microsoft.AspNetCore.Http;

namespace Elsa.Http.Contracts;

/// <summary>
/// Selects a correlation ID from an HTTP request.
/// </summary>
public interface ICorrelationIdSelector
{
    /// <summary>
    /// Gets the priority of this selector. Selectors with higher priority are invoked first.
    /// </summary>
    double Priority { get; }
    
    /// <summary>
    /// Selects a correlation ID from an HTTP request.
    /// </summary>
    /// <param name="request">The HTTP request.</param>
    /// <param name="correlationId">The correlation ID.</param>
    /// <returns>True if a correlation ID was found, otherwise false.</returns>
    bool TrySelect(HttpRequest request, out string correlationId);
}