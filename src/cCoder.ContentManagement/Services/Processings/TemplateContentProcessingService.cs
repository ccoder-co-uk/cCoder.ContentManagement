// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Foundations.TemplateContents;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class TemplateContentProcessingService(
    ITemplateContentService templateContentService)
        : ITemplateContentProcessingService
{
    public ValueTask<string> ReadContentAsync(Stream source) =>
        TryCatch(operation: () =>
        {
            ValidateTemplateContentOnRead(inputs: [source]);
            return templateContentService.ReadContentAsync(source: source);
        }, isValueTask: true);
}