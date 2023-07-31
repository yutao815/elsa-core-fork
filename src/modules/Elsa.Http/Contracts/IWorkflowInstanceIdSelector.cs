using Microsoft.AspNetCore.Http;

namespace Elsa.Http.Contracts;

/// <summary>
/// Selects a workflow instance ID from an HTTP request.
/// </summary>
public interface IWorkflowInstanceIdSelector
{
    /// <summary>
    /// Gets the priority of this selector. Selectors with higher priority are invoked first.
    /// </summary>
    double Priority { get; }
    
    /// <summary>
    /// Selects a workflow instance ID from an HTTP request.
    /// </summary>
    /// <param name="request">The HTTP request.</param>
    /// <param name="workflowInstanceId">The workflow instance ID.</param>
    /// <returns>True if a workflow instance ID was found, otherwise false.</returns>
    bool TrySelect(HttpRequest request, out string workflowInstanceId);
}