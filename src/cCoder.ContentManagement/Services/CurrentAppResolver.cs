// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services;

internal class CurrentAppResolver(IAppService service, HttpContext httpContext = null) : ICurrentAppResolver
{
    public App ResolveCurrentApp()
    {
        string text = httpContext?.Request.Path.Value ?? string.Empty;

        if (text.Contains(value: "/webdav", comparisonType: StringComparison.OrdinalIgnoreCase) && text.Contains(value: "Core/App(", comparisonType: StringComparison.OrdinalIgnoreCase))
        {
            int num = text.IndexOf(value: "Core/App(", comparisonType: StringComparison.OrdinalIgnoreCase) + 9;
            int num2 = text.IndexOf(value: ')', startIndex: num);

            if (num2 > num)
            {
                int num3 = num;

                if (int.TryParse(s: text.Substring(startIndex: num3, length: num2 - num3), result: out var result))
                {
                    return service.GetApp(appId: result);
                }
            }
        }

        string host = httpContext?.Request.Host.Host ?? string.Empty;

        return service.GetAllApp()
            .FirstOrDefault(predicate: (App app) => app.Domain == host);
    }
}