// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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

    public IEnumerable<CommonObject> LatestSet { get; set; }
    private int ExpiryTimeInMinutes { get; }

    public CommonObjectCache(Config config, IServiceScopeFactory serviceScopeFactory, ILogger<CommonObjectCache> log)
    {
        this.LatestSet = Array.Empty<CommonObject>();
        Config = config;
        this.serviceScopeFactory = serviceScopeFactory;
        this.log = log;
        ExpiryTimeInMinutes = (config.Settings.ContainsKey(key: "CacheExpiry") ? int.Parse(s: config.Settings["CacheExpiry"]) : 30);
        timer.Elapsed += ScanForExpiredItems;
        timer.Interval = ExpiryTimeInMinutes * 60 * 1000;
        timer.Start();
    }

    public void Refresh()
    {
        LatestSet = Array.Empty<CommonObject>();

        if (!Config.Settings.ContainsKey(key: "CacheSource") || !Config.Settings.ContainsKey(key: "CacheSourceAppId"))
        {
            log.LogInformation(message: "Common object cache source settings are missing, loading from local data.");
        }

        List<object> list = new List<object>();

        try
        {
            log.LogInformation(message: "{Now} - Processing common object cache", args: DateTimeOffset.Now);
            using IServiceScope serviceScope = serviceScopeFactory.CreateScope();
            ICommonObjectBroker requiredService = serviceScope.ServiceProvider.GetRequiredService<ICommonObjectBroker>();
            IJsonBroker jsonBroker = serviceScope.ServiceProvider.GetRequiredService<IJsonBroker>();
            CommonObject[] latestCommonObjectsPaged = requiredService.GetLatestCommonObjectsPaged();

            CommonObject[] array = latestCommonObjectsPaged
                .Where(predicate: commonObject => commonObject.Type == "Core/Component")
                .ToArray();

            CommonObject[] array2 = latestCommonObjectsPaged
                .Where(predicate: commonObject => commonObject.Type == "Core/Resource")
                .ToArray();

            CommonObject[] array3 = latestCommonObjectsPaged
                .Where(predicate: commonObject => commonObject.Type == "Core/Script")
                .ToArray();

            LatestSet = array.Union(second: array2)
                .Union(second: array3)
                .ToArray();

            list.AddRange(collection: array2.AsParallel()
                .WithDegreeOfParallelism(degreeOfParallelism: 8)
                .Select(selector: commonObject => jsonBroker.ParseJson<Resource>(json: commonObject.Json)));

            list.AddRange(collection: array.AsParallel()
                .WithDegreeOfParallelism(degreeOfParallelism: 8)
                .Select(selector: commonObject => jsonBroker.ParseJson<Component>(json: commonObject.Json)));

            list.AddRange(collection: array3.AsParallel()
                .WithDegreeOfParallelism(degreeOfParallelism: 8)
                .Select(selector: commonObject => jsonBroker.ParseJson<Script>(json: commonObject.Json)));

            log.LogInformation(message: "{Now} - Processed common object cache", args: DateTimeOffset.Now);
        }
        catch (Exception ex)
        {
            log.LogError(
                message: "{Message} - {StackTrace}",
                args: [ex.Message, ex.StackTrace]);
        }

        data.Clear();

        foreach (object item in list)
        {
            switch (item)
            {
                case Resource resource:
                    Set(key: $"resource|{resource.Key?.ToLower() ?? string.Empty}-{resource.Name?.ToLower() ?? string.Empty}-{resource.Culture?.ToLower() ?? string.Empty}", item: resource);
                    break;
                case Component component:
                    Set(key: "component|" + component.Name.ToLower(), item: component);
                    break;
                case Script script:
                    Set(key: "script|" + script.Name.ToLower(), item: script);
                    break;
            }
        }
    }

    public T[] GetAll<T>() =>
        data.Values.AsParallel()
        .Where(predicate: entry => entry.Key.StartsWith(value: typeof(T).Name.ToLowerInvariant()))
        .Select(selector: entry => (T)entry.Value)
        .ToArray();

    public T Get<T>(string key)
    {
        object obj = Get(key: key.ToLowerInvariant());
        return (obj != null) ? ((T)obj) : default(T);
    }

    public void Set(string key, object item)
    {
        string normalizedKey = key.ToLowerInvariant();

        data.AddOrUpdate(key: normalizedKey, addValueFactory: (string _) => new CacheEntry
        {
            Key = normalizedKey,
            AddedOn = DateTime.Now,
            Value = item
        }, updateValueFactory: (string _, CacheEntry _) => new CacheEntry
        {
            Key = normalizedKey,
            AddedOn = DateTime.Now,
            Value = item
        });
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(obj: this);
    }

    private object Get(string key)
    {
        CacheEntry value;
        return data.TryGetValue(key: key, value: out value) ? value.Value : null;
    }

    private void ScanForExpiredItems(object sender, System.Timers.ElapsedEventArgs e)
    {
        DateTime expiryCutoff = DateTime.Now.AddMinutes(value: -ExpiryTimeInMinutes);

        string[] array = data.Values
            .Where(predicate: entry => entry.AddedOn < expiryCutoff)
            .Select(selector: entry => entry.Key)
            .ToArray();

        string[] array2 = array;

        foreach (string key in array2)
        {
            data.TryRemove(key: key, value: out CacheEntry _);
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