// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
        object[] items = (commonObjects?.Items?.Values ?? []).ToArray();

        renderSession.CommonResourcesByLookup = items
            .OfType<Resource>()
            .GroupBy(
                keySelector: resource =>
                    $"{resource.Key}|{resource.Name}|{resource.Culture}",
                comparer: StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                keySelector: group => group.Key,
                elementSelector: group => MapResource(
                    resource: group.First()),
                comparer: StringComparer.OrdinalIgnoreCase);

        renderSession.CommonComponentsByName = items
            .OfType<Component>()
            .GroupBy(
                keySelector: component => component.Name ?? string.Empty,
                comparer: StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                keySelector: group => group.Key,
                elementSelector: group => MapComponent(
                    component: group.First()),
                comparer: StringComparer.OrdinalIgnoreCase);

        renderSession.CommonScriptsByName = items
            .OfType<Script>()
            .GroupBy(
                keySelector: script => script.Name ?? string.Empty,
                comparer: StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                keySelector: group => group.Key,
                elementSelector: group => MapScript(
                    script: group.First()),
                comparer: StringComparer.OrdinalIgnoreCase);

        renderSession.CommonStylesByName = items
            .OfType<Style>()
            .GroupBy(
                keySelector: style => style.Name ?? string.Empty,
                comparer: StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                keySelector: group => group.Key,
                elementSelector: group => MapStyle(
                    style: group.First()),
                comparer: StringComparer.OrdinalIgnoreCase);
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