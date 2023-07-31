using Elsa.Http.Contracts;
using Microsoft.AspNetCore.Http;

namespace Elsa.Http.Selectors;

/// <summary>
/// Attempts to read the workflow instance ID from the "workflowInstanceId" query string.
/// </summary>
public class QueryStringWorkflowInstanceIdSelector : IWorkflowInstanceIdSelector
{
    public double Priority => 20;

    /// <inheritdoc />
    public bool TrySelect(HttpRequest request, out string workflowInstanceId)
    {
        workflowInstanceId = request.Headers["x-workflow-instance-id"].ToString();
        return !string.IsNullOrWhiteSpace(workflowInstanceId);
    }
}