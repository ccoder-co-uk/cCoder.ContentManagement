// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Models.OData;

internal static class MetadataResourceDependency
{
    internal static Resource ForKeyAndCulture(
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