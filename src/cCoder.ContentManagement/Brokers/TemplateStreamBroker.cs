// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Brokers;

internal sealed class TemplateStreamBroker(
    TemplateStreamDependency templateStreamDependency)
        : ITemplateStreamBroker
{
    public async ValueTask<string> ReadAsync(Stream source)
    {
        await source.CopyToAsync(destination: templateStreamDependency);

        return templateStreamDependency.ReadContent();
    }
}