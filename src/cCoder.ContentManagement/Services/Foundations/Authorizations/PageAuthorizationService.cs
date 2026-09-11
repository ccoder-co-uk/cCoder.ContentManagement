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
        HttpPageRenderContext httpPageRenderContext) =>
        TryCatch(operation: async () =>
    {
        ValidateAuthorizeHttpPageRenderContextAsync(
            inputs: [httpPageRenderContext]);

        ValidatePageRenderContext(
            pageRenderContext: httpPageRenderContext,
            parameterName: "pageRenderContext");

        PageAuthorizationResult authorization = await pageAuthorizationBroker
            .GetAuthorizedPageAsync(
                domain: httpPageRenderContext.Domain,
                path: httpPageRenderContext.Path);

        if (authorization?.PageId is not null)
        {
            ApplyAuthorization(
                pageRenderContext: httpPageRenderContext,
                authorization: authorization);

            if (httpPageRenderContext.Edit)
            {
                httpPageRenderContext.Edit = await pageAuthorizationBroker
                    .CanUpdatePageAsync(
                        appId: authorization.AppId,
                        pageId: authorization.PageId.Value);
            }

            return httpPageRenderContext;
        }

        authorization = await pageAuthorizationBroker
            .GetPageIgnoringFiltersAsync(
                domain: httpPageRenderContext.Domain,
                path: httpPageRenderContext.Path);

        if (authorization?.PageId is not null)
        {
            ApplyAuthorization(
                pageRenderContext: httpPageRenderContext,
                authorization: authorization);

            httpPageRenderContext.AccessDenied = true;

            return httpPageRenderContext;
        }

        if (authorization is not null)
        {
            ApplyAuthorization(
                pageRenderContext: httpPageRenderContext,
                authorization: authorization);
        }

        return httpPageRenderContext;

    }, isValueTask: true);

    private static void ApplyAuthorization(
        HttpPageRenderContext pageRenderContext,
        PageAuthorizationResult authorization)
    {
        pageRenderContext.PageId = authorization.PageId;
        pageRenderContext.Layout = authorization.Layout;
        pageRenderContext.AppId = authorization.AppId;
        pageRenderContext.TenantId = authorization.TenantId;
        pageRenderContext.Domain = authorization.Domain;
        pageRenderContext.AppConfigJson = authorization.AppConfigJson;
        pageRenderContext.AppDefaultTheme = authorization.DefaultTheme;

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