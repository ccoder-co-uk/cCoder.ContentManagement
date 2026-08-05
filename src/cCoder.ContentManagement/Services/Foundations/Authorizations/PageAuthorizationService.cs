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

        PageAuthorizationResult authorization = await pageAuthorizationBroker
            .GetAuthorizedPageAsync(
                domain: pageRenderContext.Domain,
                path: pageRenderContext.Path);

        if (authorization?.PageId is not null)
        {
            ApplyAuthorization(
                pageRenderContext: pageRenderContext,
                authorization: authorization);

            if (pageRenderContext.Edit)
            {
                pageRenderContext.Edit = await pageAuthorizationBroker
                    .CanUpdatePageAsync(
                        appId: authorization.AppId,
                        pageId: authorization.PageId.Value);
            }

            return pageRenderContext;
        }

        authorization = await pageAuthorizationBroker
            .GetPageIgnoringFiltersAsync(
                domain: pageRenderContext.Domain,
                path: pageRenderContext.Path);

        if (authorization?.PageId is not null)
        {
            ApplyAuthorization(
                pageRenderContext: pageRenderContext,
                authorization: authorization);

            pageRenderContext.AccessDenied = true;

            return pageRenderContext;
        }

        if (authorization is not null)
        {
            ApplyAuthorization(
                pageRenderContext: pageRenderContext,
                authorization: authorization);
        }

        return pageRenderContext;

    }, isValueTask: true);

    private static void ApplyAuthorization(
        HttpPageRenderContext pageRenderContext,
        PageAuthorizationResult authorization)
    {
        pageRenderContext.PageId = authorization.PageId;
        pageRenderContext.AppId = authorization.AppId;
        pageRenderContext.TenantId = authorization.TenantId;
        pageRenderContext.Domain = authorization.Domain;
        pageRenderContext.AppConfigJson = authorization.AppConfigJson;

        if (string.IsNullOrWhiteSpace(value: pageRenderContext.Culture))
        {
            pageRenderContext.Culture = authorization.DefaultCulture;
        }

        if (string.IsNullOrWhiteSpace(value: pageRenderContext.Theme))
        {
            pageRenderContext.Theme = authorization.DefaultTheme;
        }
    }
}