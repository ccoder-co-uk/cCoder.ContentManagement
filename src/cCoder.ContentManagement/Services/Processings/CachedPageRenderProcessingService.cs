// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Models.PageRendering;
using cCoder.ContentManagement.Rendering.Services.Foundations;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Processings;

internal sealed partial class CachedPageRenderProcessingService
    : ICachedPageRenderProcessingService
{
    public PageRenderCacheOperation RenderPageRenderCacheOperation(
        PageRenderCacheOperation operation) =>
        TryCatch(operation: () =>
        {
            ValidatePageRenderCacheOperationOnRender(inputs: [operation]);

            HttpPageRenderContext context = operation.RenderOperation.Context;
            PageRenderCache cached = operation.Cache;

            operation.RenderOperation.Response = new PageRenderResponse
            {
                App = new App
                {
                    Id = context.AppId,
                    TenantId = context.TenantId,
                    Domain = context.Domain,
                    DefaultCultureId = context.Culture,
                    DefaultTheme = context.Theme,
                    ConfigJson = context.AppConfigJson
                },
                Page = new PageRenderResult
                {
                    AppId = cached.AppId,
                    PageId = cached.PageId,
                    ParentId = cached.ParentId,
                    ShowOnMenus = cached.ShowOnMenus,
                    Edit = false,
                    Culture = context.Culture,
                    Theme = context.Theme,
                    Path = cached.Path,
                    Title = cached.Title,
                    Description = cached.Description,
                    Keywords = cached.Keywords,
                    HeaderHtml = MarkupRenderService
                        .MarkContentSecurityPolicyNonce(
                            markup: cached.Header),
                    BodyHtml = MarkupRenderService
                        .MarkContentSecurityPolicyNonce(
                            markup: cached.Body),
                    StatusCode = StatusCodes.Status200OK
                },
                Culture = context.Culture,
                Theme = context.Theme,
                Edit = false
            };

            return operation;
        });
}