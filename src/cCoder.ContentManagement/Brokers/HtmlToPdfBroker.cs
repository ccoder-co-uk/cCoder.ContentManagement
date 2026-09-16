// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using iText.Html2pdf;

namespace cCoder.ContentManagement.Brokers;

internal sealed class HtmlToPdfBroker : IHtmlToPdfBroker
{
    public byte[] ConvertHtmlToPdf(string html)
    {
        using MemoryStream pdfStream = new();
        HtmlConverter.ConvertToPdf(html: html, pdfStream: pdfStream);

        return pdfStream.ToArray();
    }
}