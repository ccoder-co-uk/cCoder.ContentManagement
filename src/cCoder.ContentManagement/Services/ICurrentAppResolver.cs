using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services;

internal interface ICurrentAppResolver
{
    App ResolveCurrentApp();
}
