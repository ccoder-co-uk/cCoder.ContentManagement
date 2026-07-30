// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace cCoder.ContentManagement.Extensions;

internal static class SwaggerGenOptionsExtensions
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

}