using Elsa.Http.Contracts;
using Microsoft.AspNetCore.Http;

namespace Elsa.Http.Selectors;

/// <summary>
/// Attempts to read the workflow instance ID from the "x-workflow-instance-id" request header.
/// </summary>
public class HeadersWorkflowInstanceIdSelector : IWorkflowInstanceIdSelector
{
    /// <inheritdoc />
    public double Priority => 10;

    /// <inheritdoc />
    public bool TrySelect(HttpRequest request, out string workflowInstanceId)
    {
        workflowInstanceId = request.Headers["x-workflow-instance-id"].ToString();
        return !string.IsNullOrWhiteSpace(workflowInstanceId);
    }
}