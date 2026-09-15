// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;
using cCoder.CodeAnalysis.Exposures;

namespace cCoder.ContentManagement.Brokers;

internal sealed class TemplateContentBroker(
    TemplateContentDependency templateContentDependency)
        : ITemplateContentBroker, IUtilityBroker
{
    public async ValueTask<string> ReadAsync(Stream source)
    {
        await source.CopyToAsync(destination: templateContentDependency);

        return templateContentDependency.ReadContent();
    }

    public byte[] ConvertHtmlToPdf(string html) =>
        templateContentDependency.ConvertHtmlToPdf(html: html);
}
