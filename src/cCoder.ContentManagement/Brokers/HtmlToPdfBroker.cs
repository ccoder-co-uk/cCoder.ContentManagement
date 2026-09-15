// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;

namespace cCoder.ContentManagement.Brokers;

internal sealed class HtmlToPdfBroker(
    HtmlToPdfDependency htmlToPdfDependency) : IHtmlToPdfBroker
{
    public byte[] ConvertHtmlToPdf(string html) =>
        htmlToPdfDependency.ConvertHtmlToPdf(html: html);
}