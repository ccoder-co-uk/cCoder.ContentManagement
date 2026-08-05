// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text.RegularExpressions;
using cCoder.ContentManagement.Models.PageRendering;

namespace cCoder.ContentManagement.Services.Processings.PageRendering;

internal sealed partial class ResourceTagHandlingProcessingService
    : IResourceTagHandlingProcessingService
{
    private static readonly Regex resourceDisplayNameRegex = CreateRegex(
        type: "resource_displayname");

    private static readonly Regex resourceShortDisplayNameRegex = CreateRegex(
        type: "resource_shortdisplayname");

    private static readonly Regex resourceDescriptionRegex = CreateRegex(
        type: "resource_description");

    public TagHandlingOperation HandleTagHandlingOperation(
        TagHandlingOperation operation) =>
        TryCatch(operation: () =>
    {
        ValidateTagHandlingOperationOnHandle(inputs: [operation]);

        ValidateTagHandlingOperation(
            operation: operation,
            parameterName: "operation");

        if (operation.Editable)
        {
            return operation;
        }

        operation.Content = resourceDisplayNameRegex.Replace(
            input: operation.Content,
            evaluator: match => ResolveResourceValue(
                operation: operation,
                match: match,
                selector: resource => resource.DisplayName));

        operation.Content = resourceShortDisplayNameRegex.Replace(
            input: operation.Content,
            evaluator: match => ResolveResourceValue(
                operation: operation,
                match: match,
                selector: resource => resource.ShortDisplayName));

        operation.Content = resourceDescriptionRegex.Replace(
            input: operation.Content,
            evaluator: match => ResolveResourceValue(
                operation: operation,
                match: match,
                selector: resource => resource.Description));

        return operation;
    });

    private static string ResolveResourceValue(
        TagHandlingOperation operation,
        Match match,
        Func<PageRenderResource, string> selector)
    {
        string name = match.Groups["name"].Value.ToLowerInvariant();

        PageRenderResource resource = ResolveResource(
            session: operation.Session,
            key: operation.ResourceKey,
            name: name);

        return resource is null
            ? name
            : selector(arg: resource) ?? name;
    }

    private static PageRenderResource ResolveResource(
        RenderSession session,
        string key,
        string name)
    {
        string culture = ResolveCulture(session: session)
            .ToLowerInvariant();

        string normalizedKey = key.ToLowerInvariant();
        string normalizedName = name.ToLowerInvariant();

        PageRenderResource resource = ResolveResourceForKey(
            lookup: session.ResourcesByLookup,
            key: normalizedKey,
            name: normalizedName,
            culture: culture);

        if (resource is not null)
        {
            return resource;
        }

        resource = ResolveResourceForKey(
            lookup: session.CommonResourcesByLookup,
            key: normalizedKey,
            name: normalizedName,
            culture: culture);

        return resource
            ?? (string.Equals(
                a: normalizedKey,
                b: "default",
                comparisonType: StringComparison.OrdinalIgnoreCase)
                    ? null
                    : ResolveResourceForKey(
                        lookup: session.CommonResourcesByLookup,
                        key: "default",
                        name: normalizedName,
                        culture: culture));
    }

    private static PageRenderResource ResolveResourceForKey(
        IReadOnlyDictionary<string, PageRenderResource> lookup,
        string key,
        string name,
        string culture)
    {
        PageRenderResource resource = FindIndexedResource(
            lookup: lookup,
            key: key,
            name: name,
            culture: culture);

        if (resource is not null)
        {
            return resource;
        }

        if (culture.Contains(value: '-'))
        {
            resource = FindIndexedResource(
                lookup: lookup,
                key: key,
                name: name,
                culture: culture.Split(separator: '-')[0]);

            if (resource is not null)
            {
                return resource;
            }
        }

        return FindIndexedResource(
            lookup: lookup,
            key: key,
            name: name,
            culture: string.Empty);
    }

    private static PageRenderResource FindIndexedResource(
        IReadOnlyDictionary<string, PageRenderResource> lookup,
        string key,
        string name,
        string culture) =>
        lookup.TryGetValue(
            key: $"{key}|{name}|{culture}",
            value: out PageRenderResource resource)
                ? resource
                : null;

    private static string ResolveCulture(RenderSession session) =>
        !string.IsNullOrWhiteSpace(value: session.Request.Culture)
            ? session.Request.Culture
            : session.App?.DefaultCulture ?? string.Empty;

    private static Regex CreateRegex(string type) =>
        new(
            pattern: $"\\[{type}\\[(?<name>[A-Za-z\\d_\\-/. ]+)\\]\\]",
            options: RegexOptions.IgnoreCase
                | RegexOptions.Compiled
                | RegexOptions.Singleline);
}