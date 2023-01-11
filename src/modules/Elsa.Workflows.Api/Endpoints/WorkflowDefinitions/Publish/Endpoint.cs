using Elsa.Abstractions;
using Elsa.Common.Models;
using Elsa.Workflows.Api.Mappers;
using Elsa.Workflows.Api.Models;
using Elsa.Workflows.Management.Services;

namespace Elsa.Workflows.Api.Endpoints.WorkflowDefinitions.Publish;

internal class Publish : ElsaEndpoint<Request, WorkflowDefinitionResponse, WorkflowDefinitionMapper>
{
    private readonly IWorkflowDefinitionStore _store;
    private readonly IWorkflowDefinitionPublisher _workflowDefinitionPublisher;

    public Publish(IWorkflowDefinitionStore store, IWorkflowDefinitionPublisher workflowDefinitionPublisher)
    {
        _store = store;
        _workflowDefinitionPublisher = workflowDefinitionPublisher;
    }

    public override void Configure()
    {
        Post("/workflow-definitions/{definitionId}/publish");
        ConfigurePermissions("publish:workflow-definitions");
    }

    public override async Task HandleAsync(Request request, CancellationToken cancellationToken)
    {
        var definition = (await _store.FindManyByDefinitionIdAsync(request.DefinitionId, VersionOptions.Latest, cancellationToken)).FirstOrDefault();

        if (definition == null)
        {
            await SendNotFoundAsync(cancellationToken);
            return;
        }

        if (definition.IsPublished)
        {
            AddError($"Workflow with id {request.DefinitionId} is already published");
            await SendErrorsAsync(cancellation: cancellationToken);
            return;
        }

        await _workflowDefinitionPublisher.PublishAsync(definition, cancellationToken);

        var response = await Map.FromEntityAsync(definition);
        await SendOkAsync(response, cancellationToken);
    }
}