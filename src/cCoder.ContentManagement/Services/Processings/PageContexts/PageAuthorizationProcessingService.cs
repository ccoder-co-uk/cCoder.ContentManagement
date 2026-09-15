// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Foundations.Authorizations;

namespace cCoder.ContentManagement.Services.Processings.PageContexts;

internal sealed partial class PageAuthorizationProcessingService(
    IPageAuthorizationService pageAuthorizationService)
        : IPageAuthorizationProcessingService
{
    public ValueTask<HttpPageRenderContext> AuthorizeHttpPageRenderContextAsync(
        HttpPageRenderContext httpPageRenderContext) =>
        TryCatch(operation: async () =>
    {
        ValidateAuthorizeHttpPageRenderContextAsync(
            inputs: [httpPageRenderContext]);

        return await pageAuthorizationService
            .AuthorizeHttpPageRenderContextAsync(
                pageRenderContext: httpPageRenderContext);

    }, isValueTask: true);
}