// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.IO;
using System.Threading.Tasks;
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