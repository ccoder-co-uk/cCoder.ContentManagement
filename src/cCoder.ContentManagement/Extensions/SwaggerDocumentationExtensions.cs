// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace cCoder.ContentManagement.Extensions;

internal static class SwaggerGenOptionsExtensions
{
    internal static void AddSwaggerDocuments(
        this SwaggerGenOptions options,
        string documentName) =>
        new SwaggerDocumentationBroker().AddSwaggerDocument(
            newSwaggerGenOptions: options,
            documentName: documentName);
}