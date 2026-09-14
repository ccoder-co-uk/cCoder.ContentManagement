// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.ContentManagement.Services.Foundations.HttpContexts;
using cCoder.ContentManagement.Models;
using cCoder.Data.Models.CMS;
using System.Security;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal sealed partial class CurrentAppOrchestrationService(
    IAppService appService,
    IHttpContextService httpContextService) : ICurrentAppOrchestrationService
{
    public App ResolveCurrentApp() =>
        TryCatch<App>(operation: () =>
    {
        string text = httpContextService.GetRequestPath();

        if (text.Contains(value: "/webdav", comparisonType: StringComparison.OrdinalIgnoreCase) && text.Contains(value: "Core/App(", comparisonType: StringComparison.OrdinalIgnoreCase))
        {
            int num = text.IndexOf(value: "Core/App(", comparisonType: StringComparison.OrdinalIgnoreCase) + 9;
            int num2 = text.IndexOf(value: ')', startIndex: num);

            if (num2 > num)
            {
                int num3 = num;

                if (int.TryParse(s: text.Substring(startIndex: num3, length: num2 - num3), result: out var result))
                {
                    App app = appService.GetVisibleAppAppOperation(
                        appOperation: new AppOperation { AppId = result }).App;

                    if (app != null)
                    {
                        return app;
                    }

                    bool exists = appService.GetUnfilteredAppAppOperation(
                        appOperation: new AppOperation { AppId = result }).App != null;

                    return exists
                        ? throw new SecurityException(message: "Access Denied!")
                        : null;
                }
            }
        }

        string host = httpContextService.GetRequestHost();

        return appService.GetVisibleAppsAppOperation(
            appOperation: new AppOperation()).Apps
            .FirstOrDefault(predicate: (App app) => app.Domain == host);
    });
}