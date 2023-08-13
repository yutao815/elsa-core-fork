using Elsa.Workflows.Core.Activities;
using Elsa.Workflows.Core.Contracts;
using Elsa.WorkflowServer.Web.Activities;
using FastEndpoints;

namespace Elsa.WorkflowServer.Web.Endpoints
{
    public class NewWorkflowEndpoint : EndpointWithoutRequest
    {
        private readonly IWorkflowRunner _workflowRunner;
        public NewWorkflowEndpoint(IWorkflowRunner workflowRunner)
        {
            _workflowRunner = workflowRunner;
        }
        public override void Configure()
        {
            Get("/new");
            AllowAnonymous();
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var workflow = new Workflow
            {
                Root = new Sequence
                {
                    Activities =
                    {
                        new WriteLine("New Workflow!"),
                        new TestActivity(),
                        new WriteLine("Finish Workflow!")
                    }
                }
            };

            // Run the workflow.
            await _workflowRunner.RunAsync(workflow);
            await SendOkAsync(ct);
        }
    }
}
