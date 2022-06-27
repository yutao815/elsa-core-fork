using System.Net.Http;

namespace Elsa.Http.Services;

public interface IHttpRequestContentWriter
{
    bool SupportsContentType(string contentType);
    HttpContent GetContent<T>(T content, string? contentType = null);
}