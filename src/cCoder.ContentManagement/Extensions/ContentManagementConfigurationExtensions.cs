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

        string path = NormalizePath(relativePath: relativePath);

        string rootPath = string.IsNullOrWhiteSpace(value: configuration.RootPath)
            ? $"Api/{documentName}"
            : configuration.RootPath;

        return string.Equals(
            a: swaggerDocumentName,
            b: documentName,
            comparisonType: StringComparison.OrdinalIgnoreCase)
            && MatchesContextRoute(path: path, rootPath: rootPath);
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