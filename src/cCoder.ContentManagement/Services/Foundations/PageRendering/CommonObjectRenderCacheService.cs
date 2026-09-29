// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using System;
using System.Collections.Generic;
using cCoder.ContentManagement.Brokers.Caching;
using cCoder.ContentManagement.Models.Caching;
using cCoder.ContentManagement.Models.PageRendering;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Rendering.Services.Foundations;

internal sealed partial class CommonObjectRenderCacheService(
    ICacheBroker cacheBroker) : ICommonObjectRenderCacheService
{
    private const string CacheKey = "ContentManagement.CommonObjects";

    public PageCacheSlice GetPageCacheSlice() =>
        TryCatch(operation: () =>
        {
            CommonObjectCacheSnapshot snapshot =
                cacheBroker.Get<CommonObjectCacheSnapshot>(key: CacheKey);

            Dictionary<string, PageRenderResource> resources = new(
                comparer: StringComparer.OrdinalIgnoreCase);

            Dictionary<string, PageRenderComponent> components = new(
                comparer: StringComparer.OrdinalIgnoreCase);

            Dictionary<string, PageRenderScript> scripts = new(
                comparer: StringComparer.OrdinalIgnoreCase);

            Dictionary<string, PageRenderStyle> styles = new(
                comparer: StringComparer.OrdinalIgnoreCase);

            foreach (object item in snapshot?.Items?.Values ?? [])
            {
                switch (item)
                {
                    case Resource resource:
                        string resourceKey = $"{resource.Key}|{resource.Name}|{resource.Culture}";

                        if (!resources.ContainsKey(key: resourceKey))
                        {
                            resources.Add(
                                key: resourceKey,
                                value: MapResource(resource: resource));
                        }

                        break;

                    case Component component:
                        string componentKey = component.Name ?? string.Empty;

                        if (!components.ContainsKey(key: componentKey))
                        {
                            components.Add(
                                key: componentKey,
                                value: MapComponent(component: component));
                        }

                        break;

                    case Script script:
                        string scriptKey = script.Name ?? string.Empty;

                        if (!scripts.ContainsKey(key: scriptKey))
                        {
                            scripts.Add(
                                key: scriptKey,
                                value: MapScript(script: script));
                        }

                        break;

                    case Style style:
                        string styleKey = style.Name ?? string.Empty;

                        if (!styles.ContainsKey(key: styleKey))
                        {
                            styles.Add(
                                key: styleKey,
                                value: MapStyle(style: style));
                        }

                        break;
                }
            }

            return new PageCacheSlice
            {
                CommonResourcesByLookup = resources,
                CommonComponentsByName = components,
                CommonScriptsByName = scripts,
                CommonStylesByName = styles
            };
        });

    private static PageRenderResource MapResource(Resource resource) =>
        new()
        {
            Key = resource.Key ?? string.Empty,
            Culture = resource.Culture ?? string.Empty,
            Name = resource.Name ?? string.Empty,
            DisplayName = resource.DisplayName ?? resource.Name ?? string.Empty,
            ShortDisplayName = resource.ShortDisplayName
                ?? resource.Name
                ?? string.Empty,
            Description = resource.Description ?? string.Empty
        };

    private static PageRenderComponent MapComponent(Component component) =>
        new()
        {
            Id = component.Id,
            Name = component.Name ?? string.Empty,
            ResourceKey = component.ResourceKey ?? string.Empty,
            Content = component.Content ?? string.Empty,
            Script = component.Script ?? string.Empty
        };

    private static PageRenderScript MapScript(Script script) =>
        new()
        {
            Name = script.Name ?? string.Empty,
            Content = script.Content ?? string.Empty
        };

    private static PageRenderStyle MapStyle(Style style) =>
        new()
        {
            Name = style.Name ?? string.Empty,
            Key = style.Key ?? string.Empty,
            Content = style.Content ?? string.Empty
        };
}