// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Swashbuckle.AspNetCore.SwaggerGen;

namespace cCoder.ContentManagement.Brokers.Swagger;

internal interface ISwaggerDocumentationBroker
{
    void AddSwaggerDocument(
        SwaggerGenOptions newSwaggerGenOptions,
        string documentName);
}