// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Processings;

namespace cCoder.ContentManagement.Services.Orchestrations;

internal partial class TemplateContentOrchestrationService(
    ITemplateContentProcessingService templateContentProcessingService,
    IHtmlToPdfProcessingService htmlToPdfProcessingService)
        : ITemplateContentOrchestrationService
{
    public ValueTask<string> ReadContentAsync(Stream source) =>
        TryCatch(operation: () =>
        {
            ValidateTemplateContentOnRead(inputs: [source]);
            return templateContentProcessingService.ReadContentAsync(source: source);
        }, isValueTask: true);

    public byte[] ConvertHtmlToPdf(string html) =>
        TryCatch(operation: () =>
        {
            ValidateHtmlOnConvert(inputs: [html]);
            return htmlToPdfProcessingService.ConvertHtmlToPdf(html: html);
        });
}