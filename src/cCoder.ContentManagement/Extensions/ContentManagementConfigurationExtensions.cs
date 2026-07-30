// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Extensions;

internal static class ContentManagementConfigurationExtensions
{
    internal static bool ShouldIncludeInDocument(
        this ContentManagementConfiguration configuration,
        string swaggerDocumentName,
        string relativePath,
        string documentName)
    {
        if (string.IsNullOrWhiteSpace(value: relativePath))
        {
            return false;
        }

        if (string.Equals(
            a: swaggerDocumentName,
            b: "v1",
            comparisonType: StringComparison.OrdinalIgnoreCase))
        {
            swaggerDocumentName = "Core";
        }

        string path = NormalizePath(relativePath: relativePath);

        string rootPath = string.IsNullOrWhiteSpace(value: configuration.RootPath)
            ? $"Api/{documentName}"
            : configuration.RootPath;

        return string.Equals(
            a: swaggerDocumentName,
            b: "Core",
            comparisonType: StringComparison.OrdinalIgnoreCase)
            ? configuration.IncludeLegacyCoreContext
                && MatchesContextRoute(path: path, rootPath: "Api/Core")
            : MatchesContextRoute(path: path, rootPath: rootPath);
    }

    private static bool MatchesContextRoute(string path, string rootPath)
    {
        string normalizedPath = NormalizePath(relativePath: rootPath);

        return path.Equals(
            value: normalizedPath,
            comparisonType: StringComparison.OrdinalIgnoreCase)
            || path.StartsWith(
                value: $"{normalizedPath}/",
                comparisonType: StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizePath(string relativePath) =>
        relativePath.StartsWith(value: '/')
            ? relativePath
            : $"/{relativePath}";
}