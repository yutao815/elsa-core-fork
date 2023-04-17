using System.Reflection;
using Elsa.Abstractions;

namespace Elsa.Workflows.Api.Endpoints.Package;

internal class GetVersion : ElsaEndpoint<Request, string>
{
    /// <inheritdoc />
    public override void Configure()
    {
        Get("/package/version");
    }

    /// <inheritdoc />
    public override async Task HandleAsync(Request request, CancellationToken cancellationToken)
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString();
        
        if (string.IsNullOrWhiteSpace(version))
            await SendNotFoundAsync(cancellationToken);
        else
            await SendOkAsync(version, cancellationToken);
    }
}

internal class Request {}