// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace cCoder.ContentManagement.Extensions;

internal static class SwaggerDocumentationExtensions
{
    internal static void AddSwaggerDocuments(
        this SwaggerGenOptions options,
        string documentName,
        ContentManagementConfiguration newContentManagementConfiguration)
    {
        options.SwaggerDoc(name: documentName, info: new OpenApiInfo
        {
            Title = $"{documentName} API definition",
            Version = documentName,
        });

        if (newContentManagementConfiguration.IncludeLegacyCoreContext)
        {
            options.SwaggerDoc(name: "Core", info: new OpenApiInfo
            {
                Title = "Core API definition",
                Version = "Core",
            });

            options.SwaggerDoc(name: "v1", info: new OpenApiInfo
            {
                Title = "Core API definition",
                Version = "v1",
            });
        }
    }

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
        relativePath.StartsWith(value: '/') ? relativePath : $"/{relativePath}";
}