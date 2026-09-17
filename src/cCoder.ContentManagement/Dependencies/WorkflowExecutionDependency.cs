// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net;
using System.Text;

namespace cCoder.ContentManagement.Dependencies;

internal sealed class WorkflowExecutionDependency : IDisposable
{
    private readonly HttpClient httpClient;

    public WorkflowExecutionDependency()
    {
        httpClient = new HttpClient(handler: new HttpClientHandler
        {
            AutomaticDecompression =
                DecompressionMethods.GZip | DecompressionMethods.Deflate
        });
    }

    internal string Execute(string baseAddress, string content)
    {
        httpClient.BaseAddress = new Uri(uriString: baseAddress);
        httpClient.Timeout = TimeSpan.FromMinutes(minutes: 10);

        using StringContent requestContent = new(
            content: content,
            encoding: Encoding.UTF8,
            mediaType: "text/plain");

        using HttpResponseMessage response = httpClient.PostAsync(
                requestUri: "ExecuteScript?useDetails=true",
                content: requestContent)
            .GetAwaiter()
            .GetResult();

        return response.Content
            .ReadAsStringAsync()
            .GetAwaiter()
            .GetResult();
    }

    public void Dispose() =>
        httpClient.Dispose();
}