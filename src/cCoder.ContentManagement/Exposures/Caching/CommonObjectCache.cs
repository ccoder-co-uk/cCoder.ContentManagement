using System.Collections.Concurrent;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.ContentManagement.Models;
using Component = cCoder.Data.Models.CMS.Component;
using Resource = cCoder.Data.Models.CMS.Resource;
using Script = cCoder.Data.Models.CMS.Script;

namespace cCoder.ContentManagement.Exposures.Caching;

internal class CommonObjectCache : ICommonObjectCache, IDisposable
{
    private sealed class CacheEntry
    {
        public required string Key { get; init; }

        public required DateTime AddedOn { get; init; }

        public required object Value { get; init; }
    }

    private readonly ILogger log;

    private readonly IServiceScopeFactory serviceScopeFactory;

    private readonly System.Timers.Timer timer = new System.Timers.Timer();

    private ConcurrentDictionary<string, CacheEntry> data = new ConcurrentDictionary<string, CacheEntry>();

    private bool disposed;

    protected Config Config { get; }

    public IEnumerable<cCoder.Data.Models.CommonObject> LatestSet { get; set; } = Array.Empty<cCoder.Data.Models.CommonObject>();

    private int ExpiryTimeInMinutes { get; }

    public CommonObjectCache(Config config, IServiceScopeFactory serviceScopeFactory, ILogger<CommonObjectCache> log)
    {
        Config = config;
        this.serviceScopeFactory = serviceScopeFactory;
        this.log = log;
        ExpiryTimeInMinutes = (config.Settings.ContainsKey("CacheExpiry") ? int.Parse(config.Settings["CacheExpiry"]) : 30);
        timer.Elapsed += ScanForExpiredItems;
        timer.Interval = ExpiryTimeInMinutes * 60 * 1000;
        timer.Start();
    }

    public void Refresh()
    {
        LatestSet = Array.Empty<cCoder.Data.Models.CommonObject>();
        if (!Config.Settings.ContainsKey("CacheSource") || !Config.Settings.ContainsKey("CacheSourceAppId"))
        {
            log.LogInformation("Common object cache source settings are missing, loading from local data.");
        }
        List<object> list = new List<object>();
        try
        {
            log.LogInformation("{Now} - Processing common object cache", DateTimeOffset.Now);
            using IServiceScope serviceScope = serviceScopeFactory.CreateScope();
            ICommonObjectBroker requiredService = serviceScope.ServiceProvider.GetRequiredService<ICommonObjectBroker>();
            IJsonBroker jsonBroker = serviceScope.ServiceProvider.GetRequiredService<IJsonBroker>();
            cCoder.Data.Models.CommonObject[] latestCommonObjectsPaged = requiredService.GetLatestCommonObjectsPaged();
            cCoder.Data.Models.CommonObject[] array = latestCommonObjectsPaged.Where((cCoder.Data.Models.CommonObject commonObject) => commonObject.Type == "Core/Component").ToArray();
            cCoder.Data.Models.CommonObject[] array2 = latestCommonObjectsPaged.Where((cCoder.Data.Models.CommonObject commonObject) => commonObject.Type == "Core/Resource").ToArray();
            cCoder.Data.Models.CommonObject[] array3 = latestCommonObjectsPaged.Where((cCoder.Data.Models.CommonObject commonObject) => commonObject.Type == "Core/Script").ToArray();
            LatestSet = array.Union(array2).Union(array3).ToArray();
            list.AddRange(from commonObject in array2.AsParallel().WithDegreeOfParallelism(8)
                          select jsonBroker.ParseJson<Resource>(commonObject.Json));
            list.AddRange(from commonObject in array.AsParallel().WithDegreeOfParallelism(8)
                          select jsonBroker.ParseJson<Component>(commonObject.Json));
            list.AddRange(from commonObject in array3.AsParallel().WithDegreeOfParallelism(8)
                          select jsonBroker.ParseJson<Script>(commonObject.Json));
            log.LogInformation("{Now} - Processed common object cache", DateTimeOffset.Now);
        }
        catch (Exception ex)
        {
            log.LogError("{Message} - {StackTrace}", ex.Message, ex.StackTrace);
        }
        data.Clear();
        list.ForEach(delegate (object item)
        {
            if (!(item is Resource resource))
            {
                if (!(item is Component component))
                {
                    if (item is Script script)
                    {
                        Set("script|" + script.Name.ToLower(), script);
                    }
                }
                else
                {
                    Set("component|" + component.Name.ToLower(), component);
                }
            }
            else
            {
                Set($"resource|{resource.Key?.ToLower() ?? string.Empty}-{resource.Name?.ToLower() ?? string.Empty}-{resource.Culture?.ToLower() ?? string.Empty}", resource);
            }
        });
    }

    public T[] GetAll<T>()
    {
        return (from entry in data.Values.AsParallel()
                where entry.Key.StartsWith(typeof(T).Name.ToLowerInvariant())
                select (T)entry.Value).ToArray();
    }

    public T Get<T>(string key)
    {
        object obj = Get(key.ToLowerInvariant());
        return (obj != null) ? ((T)obj) : default(T);
    }

    public void Set(string key, object item)
    {
        string normalizedKey = key.ToLowerInvariant();
        data.AddOrUpdate(normalizedKey, (string _) => new CacheEntry
        {
            Key = normalizedKey,
            AddedOn = DateTime.Now,
            Value = item
        }, (string _, CacheEntry _) => new CacheEntry
        {
            Key = normalizedKey,
            AddedOn = DateTime.Now,
            Value = item
        });
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private object Get(string key)
    {
        CacheEntry value;
        return data.TryGetValue(key, out value) ? value.Value : null;
    }

    private void ScanForExpiredItems(object sender, System.Timers.ElapsedEventArgs e)
    {
        DateTime expiryCutoff = DateTime.Now.AddMinutes(-ExpiryTimeInMinutes);
        string[] array = (from entry in data.Values
                          where entry.AddedOn < expiryCutoff
                          select entry.Key).ToArray();
        string[] array2 = array;
        foreach (string key in array2)
        {
            data.TryRemove(key, out CacheEntry _);
        }
    }

    private void Dispose(bool disposing)
    {
        if (disposing && !disposed)
        {
            disposed = true;
            timer.Stop();
            timer.Dispose();
            data.Clear();
            data = new ConcurrentDictionary<string, CacheEntry>();
        }
    }
}
