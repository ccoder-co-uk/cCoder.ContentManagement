// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Net.Http;
using System.Text;
using cCoder.CodeAnalysis.Exposures;

namespace cCoder.ContentManagement.Brokers;

internal sealed class WorkflowExecutionBroker(
    IHttpClientFactory httpClientFactory)
        : IWorkflowExecutionBroker, IUtilityBroker
{
    public string Execute(string baseAddress, string content)
    {
        using HttpClient httpClient = httpClientFactory.CreateClient(
            name: nameof(WorkflowExecutionBroker));

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
}