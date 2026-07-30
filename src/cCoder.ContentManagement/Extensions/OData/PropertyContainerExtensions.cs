// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Extensions.OData;

internal static class PropertyContainerExtensions
{
    internal static PropertyContainer Resource(
        this PropertyContainer propertyContainer,
        string keyContext,
        string culture,
        IEnumerable<Resource> resources)
    {
        Resource resource = ForKeyAndCulture(
            resources: resources,
            key: $"{keyContext}.{propertyContainer.Name}",
            culture: culture);

        return new PropertyContainer
        {
            Name = propertyContainer.Name,
            Type = propertyContainer.Type,
            ServerType = propertyContainer.ServerType,
            ServerTypeName = propertyContainer.ServerTypeName,
            Template = propertyContainer.Template,
            DisplayName = resource?.DisplayName ?? propertyContainer.DisplayName,
            ShortDisplayName =
                resource?.ShortDisplayName ?? propertyContainer.ShortDisplayName,
            Description = resource?.Description ?? propertyContainer.Description,
            IsGeneric = propertyContainer.IsGeneric,
            IsValueType = propertyContainer.IsValueType,
            IsReadOnly = propertyContainer.IsReadOnly,
            IsRequired = propertyContainer.IsRequired,
            IsSystemManaged = propertyContainer.IsSystemManaged
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