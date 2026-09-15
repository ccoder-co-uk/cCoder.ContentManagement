// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers;

namespace cCoder.ContentManagement.Services.Foundations.TemplateContents;

internal partial class TemplateContentService(
    ITemplateStreamBroker templateStreamBroker) : ITemplateContentService
{
    public ValueTask<string> ReadContentAsync(Stream source) =>
        TryCatch<string>(operation: () =>
        {
            ValidateTemplateContentOnRead(inputs: [source]);
            return templateStreamBroker.ReadAsync(source: source);
        }, isValueTask: true);
}