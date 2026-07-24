// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Services.Processings;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal partial class PageRenderOrchestrationService(
    Config config,
    IPageRenderProcessingService pageRenderProcessingService,
    IAuthorizationProcessingService authorizationProcessingService)
        : IPageRenderOrchestrationService
{
    public bool IsAdminOfApp(int appId) =>
        TryCatch<bool>(operation: () =>
    {
        ValidateIsAdminOfApp(inputs: [appId]);
        return authorizationProcessingService.IsAdminOfApp(appId: appId);
    });

    public string ResolveCulture(string culture) =>
        TryCatch<string>(operation: () =>
    {
        ValidateResolveCulture(inputs: [culture]);

        RenderAuthorization authorization = authorizationProcessingService
            .ResolveRenderAuthorization(culture: culture);

        return authorization.Culture;

    });

    public bool UserCanPage(Page page, string privilege) =>
        TryCatch<bool>(operation: () =>
    {
        ValidateUserCanPage(inputs: [page, privilege]);
        ValidatePage(page: page, parameterName: "page");

        RenderAuthorization authorization = authorizationProcessingService
            .ResolveRenderAuthorization(culture: null);

        return ContentManagementModelLogic.UserCan(
            page: page,
            user: authorization.User,
            privilege: privilege);

    });

    public RenderResult RenderPageRenderResult(
        Page page,
        string theme,
        string culture,
        bool edit = false) =>
        TryCatch<RenderResult>(operation: () =>
    {
        ValidateRenderPageRenderResult(inputs: [page, theme, culture, edit]);
        ValidatePage(page: page, parameterName: "page");
        ValidateTheme(theme: theme, parameterName: "theme");

        RenderAuthorization authorization = authorizationProcessingService
            .ResolveRenderAuthorization(culture: culture);

        return ExecuteRenderPage(
            page: page,
            user: authorization.User,
            theme: theme,
            culture: authorization.Culture,
            edit: edit);

    });

    public RenderResult RenderPageUserRenderResult(Page page, User user, string theme, string culture, bool edit = false) =>
        TryCatch<RenderResult>(operation: () =>
    {
        ValidateRenderPageUserRenderResult(inputs: [page, user, theme, culture, edit]);
        ValidatePage(page: page, parameterName: "page");
        ValidateUser(user: user, parameterName: "user");
        ValidateTheme(theme: theme, parameterName: "theme");

        return ExecuteRenderPage(
            page: page,
            user: user,
            theme: theme,
            culture: culture,
            edit: edit);

    });

    private RenderResult ExecuteRenderPage(
        Page page,
        User user,
        string theme,
        string culture,
        bool edit) =>
        pageRenderProcessingService.RenderPageUserConfigRenderResult(
            page: page,
            user: user,
            config: config,
            theme: theme,
            culture: culture,
            edit: edit);

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