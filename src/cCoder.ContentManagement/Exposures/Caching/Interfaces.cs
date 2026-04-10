using cCoder.Data.Models;


namespace cCoder.ContentManagement.Exposures.Caching;

public interface ICommonObjectCache : IDisposable
{
    void Refresh();
    T[] GetAll<T>();
    T Get<T>(string key);
    void Set(string key, object item);
    IEnumerable<CommonObject> LatestSet { get; set; }
}

public interface IMetadataCache
{
    string Get(string key, string culture);
    string GetAll(string culture = "");
    void Rebuild();
    string ToJson(string culture);
}


