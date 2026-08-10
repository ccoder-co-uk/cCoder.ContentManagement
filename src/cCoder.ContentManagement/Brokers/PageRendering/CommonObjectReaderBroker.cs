// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Exposures.Caching;
using cCoder.ContentManagement.Models.PageRendering;
using cCoder.Data.Models;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Rendering.Brokers;

internal sealed class CommonObjectReaderBroker(ICommonObjectCache commonObjectCache) : ICommonObjectReaderBroker
{
    public void EnsureAvailable() =>
        commonObjectCache.EnsureAvailable();

    public T[] GetAll<T>() =>
        commonObjectCache.GetAll<T>();

    public T Get<T>(string key) =>
        commonObjectCache.Get<T>(key: key);

    public void Set(string key, object item) =>
        commonObjectCache.Set(key: key, item: item);

    public IEnumerable<CommonObject> GetLatestSet() =>
        commonObjectCache.GetLatestSet();

    public IReadOnlyDictionary<string, PageRenderResource> GetResourcesByLookup() =>
        commonObjectCache.GetAll<Resource>()
        .GroupBy(keySelector: resource => BuildResourceLookupKey(key: resource.Key ?? string.Empty, name: resource.Name ?? string.Empty, culture: resource.Culture ?? string.Empty), comparer: StringComparer.OrdinalIgnoreCase)
        .ToDictionary(
keySelector: group => group.Key,
elementSelector: group => new PageRenderResource
{
    Key = group.First()
        .Key ?? string.Empty,
    Culture = group.First()
        .Culture ?? string.Empty,
    Name = group.First()
        .Name ?? string.Empty,
    DisplayName = group.First()
        .DisplayName ?? group.First()
        .Name ?? string.Empty,
    ShortDisplayName = group.First()
        .ShortDisplayName ?? group.First()
        .Name ?? string.Empty,
    Description = group.First()
        .Description ?? string.Empty
},
comparer: StringComparer.OrdinalIgnoreCase);

    public IReadOnlyDictionary<string, PageRenderComponent> GetComponentsByName() =>
        commonObjectCache.GetAll<Component>()
        .GroupBy(keySelector: component => component.Name ?? string.Empty, comparer: StringComparer.OrdinalIgnoreCase)
        .ToDictionary(
keySelector: group => group.Key,
elementSelector: group => new PageRenderComponent
{
    Id = group.First()
        .Id,
    Name = group.First()
        .Name ?? string.Empty,
    ResourceKey = group.First()
        .ResourceKey ?? string.Empty,
    Content = group.First()
        .Content ?? string.Empty,
    Script = group.First()
        .Script ?? string.Empty
},
comparer: StringComparer.OrdinalIgnoreCase);

    public IReadOnlyDictionary<string, PageRenderScript> GetScriptsByName() =>
        commonObjectCache.GetAll<Script>()
        .GroupBy(keySelector: script => script.Name ?? string.Empty, comparer: StringComparer.OrdinalIgnoreCase)
        .ToDictionary(
keySelector: group => group.Key,
elementSelector: group => new PageRenderScript
{
    Name = group.First()
        .Name ?? string.Empty,
    Content = group.First()
        .Content ?? string.Empty
},
comparer: StringComparer.OrdinalIgnoreCase);

    public IReadOnlyDictionary<string, PageRenderStyle> GetStylesByName() =>
        commonObjectCache.GetAll<Style>()
        .GroupBy(
            keySelector: style => style.Name ?? string.Empty,
            comparer: StringComparer.OrdinalIgnoreCase)
        .ToDictionary(
            keySelector: group => group.Key,
            elementSelector: group => new PageRenderStyle
            {
                Name = group.First().Name ?? string.Empty,
                Key = group.First().Key ?? string.Empty,
                Content = group.First().Content ?? string.Empty
            },
            comparer: StringComparer.OrdinalIgnoreCase);

    private static string BuildResourceLookupKey(string key, string name, string culture) =>
        $"{key}|{name}|{culture}";
}