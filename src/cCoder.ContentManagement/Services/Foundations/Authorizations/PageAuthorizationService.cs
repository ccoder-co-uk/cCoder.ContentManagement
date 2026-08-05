// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Authorizations;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Models.Exceptions;

namespace cCoder.ContentManagement.Services.Foundations.Authorizations;

internal sealed partial class PageAuthorizationService(
    IPageAuthorizationBroker pageAuthorizationBroker)
        : IPageAuthorizationService
{
    public ValueTask<HttpPageRenderContext> AuthorizeHttpPageRenderContextAsync(
        HttpPageRenderContext pageRenderContext) =>
        TryCatch(operation: async () =>
    {
        ValidateAuthorizeHttpPageRenderContextAsync(
            inputs: [pageRenderContext]);

        ValidatePageRenderContext(
            pageRenderContext: pageRenderContext,
            parameterName: "pageRenderContext");

        pageRenderContext.PageId = await pageAuthorizationBroker
            .GetAuthorizedPageIdAsync(
                domain: pageRenderContext.Domain,
                path: pageRenderContext.Path);

        if (pageRenderContext.PageId is not null)
        {
            return pageRenderContext;
        }

        pageRenderContext.PageId = await pageAuthorizationBroker
            .GetPageIdIgnoringFiltersAsync(
                domain: pageRenderContext.Domain,
                path: pageRenderContext.Path);

        if (pageRenderContext.PageId is not null)
        {
            throw new PageAccessSecurityException(
                pageRenderContext: pageRenderContext);
        }

        return pageRenderContext;

    }, isValueTask: true);
}