// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using System;
using System.Collections.Generic;
using cCoder.ContentManagement.Models.Caching;
using cCoder.ContentManagement.Models.PageRendering;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Rendering.Services.Foundations;

internal sealed partial class MarkupRenderService
{
    private static void PopulateCommonObjects(
        RenderSession renderSession,
        CommonObjectCacheSnapshot commonObjects)
    {
        Dictionary<string, PageRenderResource> resources =
            new Dictionary<string, PageRenderResource>(StringComparer.OrdinalIgnoreCase);

        Dictionary<string, PageRenderComponent> components =
            new Dictionary<string, PageRenderComponent>(StringComparer.OrdinalIgnoreCase);

        Dictionary<string, PageRenderScript> scripts =
            new Dictionary<string, PageRenderScript>(StringComparer.OrdinalIgnoreCase);

        Dictionary<string, PageRenderStyle> styles =
            new Dictionary<string, PageRenderStyle>(StringComparer.OrdinalIgnoreCase);

        foreach (object item in commonObjects?.Items?.Values ?? [])
        {
            if (item is Resource resource)
            {
                string key = $"{resource.Key}|{resource.Name}|{resource.Culture}";

                resources.TryAdd(
                    key: key,
                    value: MapResource(resource: resource));
            }
            else if (item is Component component)
            {
                components.TryAdd(
                    key: component.Name ?? string.Empty,
                    value: MapComponent(component: component));
            }
            else if (item is Script script)
            {
                scripts.TryAdd(
                    key: script.Name ?? string.Empty,
                    value: MapScript(script: script));
            }
            else if (item is Style style)
            {
                styles.TryAdd(
                    key: style.Name ?? string.Empty,
                    value: MapStyle(style: style));
            }
        }

        renderSession.CommonResourcesByLookup = resources;
        renderSession.CommonComponentsByName = components;
        renderSession.CommonScriptsByName = scripts;
        renderSession.CommonStylesByName = styles;
    }

    private static PageRenderResource MapResource(Resource resource) =>
        new()
        {
            Key = resource.Key ?? string.Empty,
            Culture = resource.Culture ?? string.Empty,
            Name = resource.Name ?? string.Empty,
            DisplayName = resource.DisplayName
                ?? resource.Name
                ?? string.Empty,
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