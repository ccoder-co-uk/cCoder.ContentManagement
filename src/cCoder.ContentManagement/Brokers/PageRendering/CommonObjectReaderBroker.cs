using cCoder.ContentManagement.Exposures.Caching;
using cCoder.ContentManagement.Rendering.Models;
using Component = cCoder.Data.Models.CMS.Component;
using Resource = cCoder.Data.Models.CMS.Resource;
using Script = cCoder.Data.Models.CMS.Script;

namespace cCoder.ContentManagement.Rendering.Brokers;

internal sealed class CommonObjectReaderBroker(ICommonObjectCache commonObjectCache) : ICommonObjectReaderBroker
{
    public IReadOnlyDictionary<string, PageRenderResource> GetResourcesByLookup()
    {
        return commonObjectCache.GetAll<Resource>()
            .GroupBy(resource => BuildResourceLookupKey(resource.Key ?? string.Empty, resource.Name ?? string.Empty, resource.Culture ?? string.Empty), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                group => group.Key,
                group => new PageRenderResource
                {
                    Key = group.First().Key ?? string.Empty,
                    Culture = group.First().Culture ?? string.Empty,
                    Name = group.First().Name ?? string.Empty,
                    DisplayName = group.First().DisplayName ?? group.First().Name ?? string.Empty,
                    ShortDisplayName = group.First().ShortDisplayName ?? group.First().Name ?? string.Empty,
                    Description = group.First().Description ?? string.Empty
                },
                StringComparer.OrdinalIgnoreCase);
    }

    public IReadOnlyDictionary<string, PageRenderComponent> GetComponentsByName()
    {
        return commonObjectCache.GetAll<Component>()
            .GroupBy(component => component.Name ?? string.Empty, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                group => group.Key,
                group => new PageRenderComponent
                {
                    Id = group.First().Id,
                    Name = group.First().Name ?? string.Empty,
                    ResourceKey = group.First().ResourceKey ?? string.Empty,
                    Content = group.First().Content ?? string.Empty,
                    Script = group.First().Script ?? string.Empty
                },
                StringComparer.OrdinalIgnoreCase);
    }

    public IReadOnlyDictionary<string, PageRenderScript> GetScriptsByName()
    {
        return commonObjectCache.GetAll<Script>()
            .GroupBy(script => script.Name ?? string.Empty, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                group => group.Key,
                group => new PageRenderScript
                {
                    Name = group.First().Name ?? string.Empty,
                    Content = group.First().Content ?? string.Empty
                },
                StringComparer.OrdinalIgnoreCase);
    }

    private static string BuildResourceLookupKey(string key, string name, string culture) =>
        $"{key}|{name}|{culture}";
}
