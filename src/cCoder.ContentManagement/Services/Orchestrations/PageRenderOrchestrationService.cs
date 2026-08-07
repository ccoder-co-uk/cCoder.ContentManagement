// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Extensions;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Processings;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

using cCoder.ContentManagement.Services.Foundations;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal partial class PageRenderOrchestrationService(
    IPageRenderProcessingService pageRenderProcessingService,
    IAuthorizationProcessingService authorizationProcessingService)
        : IPageRenderOrchestrationService
{
    public bool IsAdminOfApp(int appId) =>
        TryCatch<bool>(operation: () =>
    {
        ValidateIsAdminOfApp(inputs: [appId]);

        return authorizationProcessingService
            .IsAdminOfAppAuthorizationContext(
                context: new AuthorizationContext
                {
                    AppId = appId
                });
    });

    public string ResolveCulture(string culture) =>
        TryCatch<string>(operation: () =>
    {
        ValidateResolveCulture(inputs: [culture]);

        RenderAuthorization authorization = authorizationProcessingService
            .ResolveRenderAuthorizationContext(
                context: new AuthorizationContext
                {
                    Culture = culture
                })
            .RenderAuthorization;

        return authorization.Culture;

    });

    public PageRenderOperation ProcessPageRenderOperation(
        PageRenderOperation operation) =>
        TryCatch<PageRenderOperation>(operation: () =>
    {
        ValidateProcessPageRenderOperation(inputs: [operation]);

        if (operation.OperationType == PageRenderOperationType.UserCanPage)
        {
            RenderAuthorization authorization = authorizationProcessingService
                .ResolveRenderAuthorizationContext(
                    context: new AuthorizationContext())
                .RenderAuthorization;

            operation.User = authorization.User;

            operation.IsAuthorized = authorizationProcessingService
                .UserCanPageAuthorizationContext(
                    context: new AuthorizationContext
                    {
                        PageAuthorization = new PageAuthorization
                        {
                            Page = operation.SourcePage,
                            User = authorization.User,
                            Privilege = operation.Privilege
                        }
                    });

            return operation;
        }

        operation.Page = operation.User == null
            ? RenderPageRenderResult(
                page: operation.SourcePage,
                theme: operation.Theme,
                culture: operation.Culture,
                edit: operation.Edit,
                headerOnly: operation.HeaderOnly,
                cacheTemplate: operation.CacheTemplate)
            : RenderPageUserRenderResult(
                page: operation.SourcePage,
                user: operation.User,
                theme: operation.Theme,
                culture: operation.Culture,
                edit: operation.Edit,
                headerOnly: operation.HeaderOnly,
                cacheTemplate: operation.CacheTemplate);

        return operation;
    });

    internal bool UserCanPage(Page page, string privilege) =>
        TryCatch<bool>(operation: () =>
    {
        ValidateUserCanPage(inputs: [page, privilege]);
        ValidatePage(page: page, parameterName: "page");

        RenderAuthorization authorization = authorizationProcessingService
            .ResolveRenderAuthorizationContext(
                context: new AuthorizationContext())
            .RenderAuthorization;

        return authorizationProcessingService.UserCanPageAuthorizationContext(
            context: new AuthorizationContext
            {
                PageAuthorization = new PageAuthorization
                {
                    Page = page,
                    User = authorization.User,
                    Privilege = privilege
                }
            });

    });

    internal PageRenderResult RenderPageRenderResult(
        Page page,
        string theme,
        string culture,
        bool edit = false,
        bool headerOnly = false,
        bool cacheTemplate = false) =>
        TryCatch<PageRenderResult>(operation: () =>
    {
        ValidateRenderPageRenderResult(inputs: [page, theme, culture, edit, headerOnly]);
        ValidatePage(page: page, parameterName: "page");
        ValidateTheme(theme: theme, parameterName: "theme");

        RenderAuthorization authorization = authorizationProcessingService
            .ResolveRenderAuthorizationContext(
                context: new AuthorizationContext
                {
                    Culture = culture
                })
            .RenderAuthorization;

        return ExecuteRenderPage(
            page: page,
            user: authorization.User,
            theme: theme,
            culture: authorization.Culture,
            edit: edit,
            headerOnly: headerOnly,
            cacheTemplate: cacheTemplate);

    });

    internal PageRenderResult RenderPageUserRenderResult(Page page, User user, string theme, string culture, bool edit = false, bool headerOnly = false, bool cacheTemplate = false) =>
        TryCatch<PageRenderResult>(operation: () =>
    {
        ValidateRenderPageUserRenderResult(inputs: [page, user, theme, culture, edit, headerOnly]);
        ValidatePage(page: page, parameterName: "page");
        ValidateUser(user: user, parameterName: "user");
        ValidateTheme(theme: theme, parameterName: "theme");

        return ExecuteRenderPage(
            page: page,
            user: user,
            theme: theme,
            culture: culture,
            edit: edit,
            headerOnly: headerOnly,
            cacheTemplate: cacheTemplate);

    });

    private PageRenderResult ExecuteRenderPage(
        Page page,
        User user,
        string theme,
        string culture,
        bool edit,
        bool headerOnly,
        bool cacheTemplate)
    {
        PageRenderOperation operation = new()
        {
            SourcePage = page,
            User = user,
            Theme = theme,
            Culture = culture,
            Edit = edit,
            HeaderOnly = headerOnly,
            CacheTemplate = cacheTemplate
        };

        return pageRenderProcessingService
            .RenderPageRenderOperation(
                operation: operation)
            .Page;
    }

    private static void ValidatePage(Page page, string parameterName) =>
        ThrowIf(condition: page == null, message: parameterName + " is required.");

    private static void ValidateUser(User user, string parameterName) =>
        ThrowIf(condition: user == null, message: parameterName + " is required.");

    private static void ValidateTheme(string theme, string parameterName) =>
        ThrowIf(condition: string.IsNullOrWhiteSpace(value: theme), message: parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            throw new ValidationException(message: message);
        }
    }
}