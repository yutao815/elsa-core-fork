using Elsa.Workflows.Core.Activities;
using Elsa.Workflows.Core.Helpers;
using Elsa.Workflows.Runtime.Bookmarks;
using Elsa.Workflows.Runtime.Contracts;
using Elsa.Workflows.Runtime.Models;
using Elsa.WorkflowServer.Web.Activities;
using FastEndpoints;

namespace Elsa.WorkflowServer.Web.Endpoints
{
    public class ResumeWorkflowEndpoint : EndpointWithoutRequest
    {
        private readonly IWorkflowInbox _workflowInbox;
        public ResumeWorkflowEndpoint(IWorkflowInbox workflowInbox)
        {
            _workflowInbox = workflowInbox;
        }
        public override void Configure()
        {
            Get("/resume");
            AllowAnonymous();
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var message = new NewWorkflowInboxMessage
            {
                ActivityTypeName = ActivityTypeNameHelper.GenerateTypeName<TestActivity>(),
                BookmarkPayload = new DispatchWorkflowBookmark("asdfasdfasdf"),
                Input = new Dictionary<string, object>
                    {
                           
                    }
            };

            await _workflowInbox.SubmitAsync(message);
            await SendOkAsync(ct);
        }
    }
}
