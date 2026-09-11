// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace cCoder.ContentManagement.Brokers.Swagger;

internal sealed class SwaggerDocumentationBroker
    : ISwaggerDocumentationBroker
{
    public void AddSwaggerDocument(
        SwaggerGenOptions newSwaggerGenOptions,
        string documentName) =>
        newSwaggerGenOptions.SwaggerDoc(
            name: documentName,
            info: new OpenApiInfo
            {
                Title = $"{documentName} API definition",
                Version = documentName,
            });
}