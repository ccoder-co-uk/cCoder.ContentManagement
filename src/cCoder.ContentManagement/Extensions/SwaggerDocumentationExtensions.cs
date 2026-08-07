// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace cCoder.ContentManagement.Extensions;

internal static class SwaggerGenOptionsExtensions
{
    internal static void AddSwaggerDocuments(
        this SwaggerGenOptions options,
        string documentName) =>
        options.SwaggerDoc(
            name: documentName,
            info: new OpenApiInfo
        {
            Title = $"{documentName} API definition",
            Version = documentName,
        });
}