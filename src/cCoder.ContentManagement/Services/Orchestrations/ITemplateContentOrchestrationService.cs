// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.ContentManagement.Services.Orchestrations;

public interface ITemplateContentOrchestrationService
{
    ValueTask<string> ReadContentAsync(Stream source);

    byte[] ConvertHtmlToPdf(string html);
}