// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
namespace cCoder.ContentManagement.Services.Orchestrations;

public interface IPageRenderOrchestrationService
{
    bool IsAdminOfApp(int appId);

    string ResolveCulture(string culture);

    PageRenderOperation ProcessPageRenderOperation(
        PageRenderOperation operation);
}