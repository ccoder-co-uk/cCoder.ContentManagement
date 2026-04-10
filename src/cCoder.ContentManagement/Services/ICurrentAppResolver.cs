using App = cCoder.Data.Models.CMS.App;

namespace cCoder.ContentManagement.Services;

internal interface ICurrentAppResolver
{
    App ResolveCurrentApp();
}
