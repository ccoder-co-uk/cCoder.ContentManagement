using cCoder.Data.Models.CMS;


namespace cCoder.ContentManagement.Services;

public interface IResourceProvider
{
    Resource GetResource(string key, string culture);
}


