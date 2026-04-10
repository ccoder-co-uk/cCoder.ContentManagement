using cCoder.ContentManagement.Services.Foundations.Storages;
using App = cCoder.Data.Models.CMS.App;

namespace cCoder.ContentManagement.Services;

internal class CurrentAppResolver(IAppService service, HttpContext httpContext = null) : ICurrentAppResolver
{
    public App ResolveCurrentApp()
    {
        string text = httpContext?.Request.Path.Value ?? string.Empty;
        if (text.Contains("/webdav", StringComparison.OrdinalIgnoreCase) && text.Contains("Core/App(", StringComparison.OrdinalIgnoreCase))
        {
            int num = text.IndexOf("Core/App(", StringComparison.OrdinalIgnoreCase) + 9;
            int num2 = text.IndexOf(')', num);
            if (num2 > num)
            {
                int num3 = num;
                if (int.TryParse(text.Substring(num3, num2 - num3), out var result))
                {
                    return service.Get(result);
                }
            }
        }
        string host = httpContext?.Request.Host.Host ?? string.Empty;
        return service.GetAll().FirstOrDefault((App app) => app.Domain == host);
    }
}
