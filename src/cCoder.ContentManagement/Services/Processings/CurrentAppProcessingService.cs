// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.ContentManagement.Models;
using cCoder.Data.Models.CMS;
using System.Security;

namespace cCoder.ContentManagement.Services.Processings;

internal sealed partial class CurrentAppProcessingService(
    IAppService service) : ICurrentAppProcessingService
{
    public App ResolveCurrentApp() =>
        TryCatch<App>(operation: () =>
    {
        string text = service.GetRequestPathAppOperation(
            appOperation: new AppOperation()).Text;

        if (text.Contains(value: "/webdav", comparisonType: StringComparison.OrdinalIgnoreCase) && text.Contains(value: "Core/App(", comparisonType: StringComparison.OrdinalIgnoreCase))
        {
            int num = text.IndexOf(value: "Core/App(", comparisonType: StringComparison.OrdinalIgnoreCase) + 9;
            int num2 = text.IndexOf(value: ')', startIndex: num);

            if (num2 > num)
            {
                int num3 = num;

                if (int.TryParse(s: text.Substring(startIndex: num3, length: num2 - num3), result: out var result))
                {
                    App app = service.GetVisibleAppAppOperation(
                        appOperation: new AppOperation { AppId = result }).App;

                    if (app != null)
                    {
                        return app;
                    }

                    bool exists = service.GetUnfilteredAppAppOperation(
                        appOperation: new AppOperation { AppId = result }).App != null;

                    return exists
                        ? throw new SecurityException(message: "Access Denied!")
                        : null;
                }
            }
        }

        string host = service.GetRequestHostAppOperation(
            appOperation: new AppOperation()).Text;

        return service.GetVisibleAppsAppOperation(
            appOperation: new AppOperation()).Apps
            .FirstOrDefault(predicate: (App app) => app.Domain == host);
    });
}