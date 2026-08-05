// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Models.PageRendering;
using cCoder.ContentManagement.Services.Processings;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal sealed partial class CachedPageRenderOrchestrationService(
    IPageRenderCacheQueryProcessingService pageRenderCacheProcessingService,
    IPageRenderCacheMissEventProcessingService eventProcessingService)
        : ICachedPageRenderOrchestrationService
{
    public ValueTask<HttpPageRenderOperation>
        RenderHttpPageRenderOperationAsync(
            HttpPageRenderOperation operation) =>
        TryCatch<HttpPageRenderOperation>(operation: async () =>
    {
        ValidateHttpPageRenderOperationOnRenderAsync(
            inputs: [operation]);

        HttpPageRenderContext pageRenderContext = operation.Context;

        if (pageRenderContext.PageId is null)
        {
            return operation;
        }

        PageRenderCache cached = pageRenderCacheProcessingService
            .GetPageRenderCache(
                pageId: pageRenderContext.PageId.Value,
                culture: pageRenderContext.Culture,
                theme: pageRenderContext.Theme);

        if (cached is null)
        {
            await eventProcessingService.RaisePageRenderCacheMissEventAsync(
                cacheMiss: new PageRenderCacheMiss
                {
                    PageId = pageRenderContext.PageId.Value
                });

            cached = pageRenderCacheProcessingService.GetPageRenderCache(
                pageId: pageRenderContext.PageId.Value,
                culture: pageRenderContext.Culture,
                theme: pageRenderContext.Theme);

            if (cached is null)
            {
                return operation;
            }
        }

        RenderResult result = new()
        {
            AppId = cached.AppId,
            PageId = cached.PageId,
            ParentId = cached.ParentId,
            UserId = null,
            ShowOnMenus = cached.ShowOnMenus,
            Edit = false,
            Culture = pageRenderContext.Culture,
            Theme = pageRenderContext.Theme,
            Path = cached.Path,
            Title = cached.Title,
            Description = cached.Description,
            Keywords = cached.Keywords,
            HeaderHtml = cached.Header,
            BodyHtml = cached.Body,
            StatusCode = StatusCodes.Status200OK
        };

        operation.Response = new PageRenderResponse
        {
            App = new App
            {
                Id = pageRenderContext.AppId,
                TenantId = pageRenderContext.TenantId,
                Domain = pageRenderContext.Domain,
                DefaultCultureId = pageRenderContext.Culture,
                DefaultTheme = pageRenderContext.Theme,
                ConfigJson = pageRenderContext.AppConfigJson
            },
            Page = result,
            Culture = pageRenderContext.Culture,
            Theme = pageRenderContext.Theme,
            Edit = false
        };

        return operation;

    }, isValueTask: true);
}