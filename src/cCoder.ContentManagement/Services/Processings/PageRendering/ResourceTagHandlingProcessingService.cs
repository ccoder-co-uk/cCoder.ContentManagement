// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Models.PageRendering;
using cCoder.ContentManagement.Models.RegularExpressions;

namespace cCoder.ContentManagement.Services.Processings.PageRendering;

internal sealed partial class ResourceTagHandlingProcessingService(
    IRegularExpressionBroker regularExpressionBroker)
    : IResourceTagHandlingProcessingService
{
    private static readonly string resourceDisplayNamePattern = CreatePattern(
        type: "resource_displayname");

    private static readonly string resourceShortDisplayNamePattern = CreatePattern(
        type: "resource_shortdisplayname");

    private static readonly string resourceDescriptionPattern = CreatePattern(
        type: "resource_description");

    public TagHandlingOperation HandleTagHandlingOperation(
        TagHandlingOperation tagHandlingOperation) =>
        TryCatch(operation: () =>
    {
        ValidateTagHandlingOperationOnHandle(inputs: [tagHandlingOperation]);

        ValidateTagHandlingOperation(
            operation: tagHandlingOperation,
            parameterName: "operation");

        if (tagHandlingOperation.Editable)
        {
            return tagHandlingOperation;
        }

        tagHandlingOperation.Content = regularExpressionBroker.Replace(
            input: tagHandlingOperation.Content,
            pattern: resourceDisplayNamePattern,
            evaluator: (value, groups) => ResolveResourceValue(
                operation: tagHandlingOperation,
                match: new RegularExpressionMatch { Value = value, Groups = groups },
                selector: resource => resource.DisplayName));

        tagHandlingOperation.Content = regularExpressionBroker.Replace(
            input: tagHandlingOperation.Content,
            pattern: resourceShortDisplayNamePattern,
            evaluator: (value, groups) => ResolveResourceValue(
                operation: tagHandlingOperation,
                match: new RegularExpressionMatch { Value = value, Groups = groups },
                selector: resource => resource.ShortDisplayName));

        tagHandlingOperation.Content = regularExpressionBroker.Replace(
            input: tagHandlingOperation.Content,
            pattern: resourceDescriptionPattern,
            evaluator: (value, groups) => ResolveResourceValue(
                operation: tagHandlingOperation,
                match: new RegularExpressionMatch { Value = value, Groups = groups },
                selector: resource => resource.Description));

        return tagHandlingOperation;
    });

    private static string ResolveResourceValue(
        TagHandlingOperation operation,
        RegularExpressionMatch match,
        Func<PageRenderResource, string> selector)
    {
        string name = match.Groups["name"].ToLowerInvariant();

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

    private static string CreatePattern(string type) =>
        $"\\[{type}\\[(?<name>[A-Za-z\\d_\\-/. ]+)\\]\\]";
}