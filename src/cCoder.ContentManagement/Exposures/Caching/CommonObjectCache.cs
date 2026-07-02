using System.Collections.Concurrent;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.ContentManagement.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models;

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

    public IEnumerable<CommonObject> LatestSet { get; set; } = Array.Empty<CommonObject>();

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
        LatestSet = Array.Empty<CommonObject>();
        if (!Config.Settings.ContainsKey("CacheSource") || !Config.Settings.ContainsKey("CacheSourceAppId"))
            log.LogInformation("Common object cache source settings are missing, loading from local data.");

        List<object> list = new List<object>();
        try
        {
            log.LogInformation("{Now} - Processing common object cache", DateTimeOffset.Now);
            using IServiceScope serviceScope = serviceScopeFactory.CreateScope();
            ICommonObjectBroker requiredService = serviceScope.ServiceProvider.GetRequiredService<ICommonObjectBroker>();
            IJsonBroker jsonBroker = serviceScope.ServiceProvider.GetRequiredService<IJsonBroker>();
            CommonObject[] latestCommonObjectsPaged = requiredService.GetLatestCommonObjectsPaged();
            CommonObject[] array = latestCommonObjectsPaged
                .Where(commonObject => commonObject.Type == "Core/Component")
                .ToArray();

            CommonObject[] array2 = latestCommonObjectsPaged
                .Where(commonObject => commonObject.Type == "Core/Resource")
                .ToArray();

            CommonObject[] array3 = latestCommonObjectsPaged
                .Where(commonObject => commonObject.Type == "Core/Script")
                .ToArray();

            LatestSet = array.Union(array2).Union(array3).ToArray();
            list.AddRange(array2.AsParallel()
                .WithDegreeOfParallelism(8)
                .Select(commonObject => jsonBroker.ParseJson<Resource>(commonObject.Json)));

            list.AddRange(array.AsParallel()
                .WithDegreeOfParallelism(8)
                .Select(commonObject => jsonBroker.ParseJson<Component>(commonObject.Json)));

            list.AddRange(array3.AsParallel()
                .WithDegreeOfParallelism(8)
                .Select(commonObject => jsonBroker.ParseJson<Script>(commonObject.Json)));

            log.LogInformation("{Now} - Processed common object cache", DateTimeOffset.Now);
        }
        catch (Exception ex)
        {
            log.LogError("{Message} - {StackTrace}", ex.Message, ex.StackTrace);
        }
        data.Clear();

        foreach (object item in list)
        {
            switch (item)
            {
                case Resource resource:
                    Set($"resource|{resource.Key?.ToLower() ?? string.Empty}-{resource.Name?.ToLower() ?? string.Empty}-{resource.Culture?.ToLower() ?? string.Empty}", resource);
                    break;
                case Component component:
                    Set("component|" + component.Name.ToLower(), component);
                    break;
                case Script script:
                    Set("script|" + script.Name.ToLower(), script);
                    break;
            }
        }
    }

    public T[] GetAll<T>()
    {
        return data.Values.AsParallel()
            .Where(entry => entry.Key.StartsWith(typeof(T).Name.ToLowerInvariant()))
            .Select(entry => (T)entry.Value)
            .ToArray();
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
        string[] array = data.Values
            .Where(entry => entry.AddedOn < expiryCutoff)
            .Select(entry => entry.Key)
            .ToArray();

        string[] array2 = array;
        foreach (string key in array2)
            data.TryRemove(key, out CacheEntry _);
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
