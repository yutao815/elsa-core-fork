using Elsa.Extensions;
using Elsa.Workflows.Core;
using Elsa.Workflows.Core.Attributes;
using Elsa.Workflows.Runtime.Bookmarks;

namespace Elsa.WorkflowServer.Web.Activities
{
    [Activity("Custom",
      Category = "Test",
      DisplayName = "Test"
      )]
    public class TestActivity : CodeActivity
    {
        protected override ValueTask ExecuteAsync(ActivityExecutionContext context)
        {
            var id = "asdfasdfasdf";
            context.CreateBookmark(new DispatchWorkflowBookmark(id), OnResumeAsync);

            return ValueTask.CompletedTask;
        }

        private async ValueTask OnResumeAsync(ActivityExecutionContext context)
        {
            Console.WriteLine("Resume Activity");

            await context.CompleteActivityAsync();
        }
    }
}
