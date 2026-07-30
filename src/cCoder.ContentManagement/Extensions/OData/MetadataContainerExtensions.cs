// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Extensions.OData;

internal static class MetadataContainerExtensions
{
    internal static MetadataContainer Resource(
        this MetadataContainer metadataContainer,
        string setName,
        string culture,
        IEnumerable<Resource> resources)
    {
        string cacheKey =
            $"{setName}|{metadataContainer.ServerTypeName
                .Split(separator: '.')
                .Last()}";

        Resource resource = ForKeyAndCulture(
            resources: resources,
            key: cacheKey,
            culture: culture);

        return new MetadataContainer
        {
            Type = metadataContainer.Type,
            ServerTypeName = metadataContainer.ServerTypeName,
            ServerType = metadataContainer.ServerType,
            IsValueType = metadataContainer.IsValueType,
            IsEntity = metadataContainer.IsEntity,
            IsJoinEntity = metadataContainer.IsJoinEntity,
            HasEndpoint = metadataContainer.HasEndpoint,
            IsSystemManaged = metadataContainer.IsSystemManaged,
            Category = metadataContainer.Category,
            Name = metadataContainer.Name,
            DisplayName = resource?.DisplayName ?? metadataContainer.DisplayName,
            Description = resource?.Description ?? metadataContainer.Description,
            Properties = metadataContainer.Properties
                .Select(selector: property => property.Resource(
                    keyContext: cacheKey,
                    culture: culture,
                    resources: resources))
                .ToArray()
        };
    }

    private static Resource ForKeyAndCulture(
        IEnumerable<Resource> resources,
        string key,
        string culture)
    {
        Resource[] candidates = resources?
            .Where(predicate: resource => string.Equals(
                a: resource.Key,
                b: key,
                comparisonType: StringComparison.OrdinalIgnoreCase))
            .ToArray()
            ?? [];

        return candidates
            .Where(predicate: resource => string.Equals(
                a: resource.Culture ?? string.Empty,
                b: culture ?? string.Empty,
                comparisonType: StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(
                keySelector: resource => resource.Culture?.Length ?? 0)
            .FirstOrDefault()
            ?? candidates.FirstOrDefault(
                predicate: resource =>
                    string.IsNullOrEmpty(value: resource.Culture));
    }
}