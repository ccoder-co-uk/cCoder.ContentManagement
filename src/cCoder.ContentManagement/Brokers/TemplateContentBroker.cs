// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Brokers;

internal sealed class TemplateContentBroker(
    TemplateContentDependency templateContentDependency)
        : ITemplateContentBroker
{
    public async ValueTask<string> ReadAsync(Stream source)
    {
        await source.CopyToAsync(destination: templateContentDependency);

        return templateContentDependency.ReadContent();
    }

    public byte[] ConvertHtmlToPdf(string html) =>
        templateContentDependency.ConvertHtmlToPdf(html: html);
}