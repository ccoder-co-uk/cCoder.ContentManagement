// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections.Concurrent;
using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Brokers.Storages;
using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Models.Caching;
using cCoder.ContentManagement.Exposures.Caching;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models;

namespace cCoder.ContentManagement.Dependencies.Caching;

internal class CommonObjectCacheDependency : ICommonObjectCache, IDisposable
{
    private readonly ILogger log;

    private readonly IServiceScopeFactory serviceScopeFactory;

    private readonly System.Timers.Timer timer = new System.Timers.Timer();

    private ConcurrentDictionary<string, CacheEntry> data = new();

    private readonly object refreshLock = new();

    private bool disposed;

    private readonly ContentManagementConfiguration config;

    private CommonObject[] latestSet;

    private readonly int expiryTimeInMinutes;

    public CommonObjectCacheDependency(
        ContentManagementConfiguration config,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<CommonObjectCacheDependency> log)
    {
        latestSet = Array.Empty<CommonObject>();
        this.config = config;
        this.serviceScopeFactory = serviceScopeFactory;
        this.log = log;
        expiryTimeInMinutes = config.CacheExpiry;
        timer.Elapsed += ScanForExpiredItems;
        timer.Interval = expiryTimeInMinutes * 60 * 1000;
        timer.Start();
    }

    public void Refresh()
    {
        lock (refreshLock)
        {
            if (string.IsNullOrWhiteSpace(config.CacheSource)
                || config.CacheSourceAppId is null)
            {
                log.LogInformation(message: "Common object cache source settings are missing, loading from local data.");
            }

            List<object> list = new();
            CommonObject[] refreshedLatestSet;

            try
            {
                log.LogInformation(message: "{Now} - Processing common object cache", args: DateTimeOffset.Now);
                using IServiceScope serviceScope = serviceScopeFactory.CreateScope();
                ICommonObjectBroker requiredService = serviceScope.ServiceProvider.GetRequiredService<ICommonObjectBroker>();
                IJsonBroker jsonBroker = serviceScope.ServiceProvider.GetRequiredService<IJsonBroker>();
                CommonObject[] latestCommonObjectsPaged = requiredService.GetLatestCommonObjectsPaged();

                CommonObject[] array = latestCommonObjectsPaged
                    .Where(predicate: commonObject => commonObject.Type == "ContentManagement/Component")
                    .ToArray();

                CommonObject[] array2 = latestCommonObjectsPaged
                    .Where(predicate: commonObject => commonObject.Type == "ContentManagement/Resource")
                    .ToArray();

                CommonObject[] array3 = latestCommonObjectsPaged
                    .Where(predicate: commonObject => commonObject.Type == "ContentManagement/Script")
                    .ToArray();

                refreshedLatestSet = array.Union(second: array2)
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

                return;
            }

            ConcurrentDictionary<string, CacheEntry> refreshedData = new();

            foreach (object item in list)
            {
                switch (item)
                {
                    case Resource resource:
                        Set(
                            target: refreshedData,
                            key: $"resource|{resource.Key?.ToLower() ?? string.Empty}-{resource.Name?.ToLower() ?? string.Empty}-{resource.Culture?.ToLower() ?? string.Empty}",
                            item: resource);
                        break;
                    case Component component:
                        Set(target: refreshedData, key: "component|" + component.Name.ToLower(), item: component);
                        break;
                    case Script script:
                        Set(target: refreshedData, key: "script|" + script.Name.ToLower(), item: script);
                        break;
                }
            }

            Volatile.Write(location: ref data, value: refreshedData);
            Volatile.Write(location: ref latestSet, value: refreshedLatestSet);
        }
    }

    public IEnumerable<CommonObject> GetLatestSet() =>
        Volatile.Read(location: ref latestSet);

    public T[] GetAll<T>() =>
        Volatile.Read(location: ref data).Values.AsParallel()
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
        lock (refreshLock)
        {
            Set(target: Volatile.Read(location: ref data), key: key, item: item);
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(obj: this);
    }

    private object Get(string key)
    {
        CacheEntry value;
        return Volatile.Read(location: ref data).TryGetValue(key: key, value: out value) ? value.Value : null;
    }

    private void ScanForExpiredItems(object sender, System.Timers.ElapsedEventArgs e)
    {
        Refresh();
    }

    private static void Set(
        ConcurrentDictionary<string, CacheEntry> target,
        string key,
        object item)
    {
        string normalizedKey = key.ToLowerInvariant();

        target.AddOrUpdate(key: normalizedKey, addValueFactory: (string _) => new CacheEntry
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

    private void Dispose(bool disposing)
    {
        if (disposing && !disposed)
        {
            disposed = true;
            timer.Stop();
            timer.Dispose();
            Volatile.Write(location: ref data, value: new ConcurrentDictionary<string, CacheEntry>());
            Volatile.Write(location: ref latestSet, value: Array.Empty<CommonObject>());
        }
    }
}
