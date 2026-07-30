// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using iText.Html2pdf;

namespace cCoder.ContentManagement.Dependencies;

internal sealed class TemplateContentDependency : MemoryStream
{
    public TemplateContentDependency()
    { }

    internal string ReadContent()
    {
        return System.Text.Encoding.UTF8.GetString(bytes: ToArray());
    }

    internal byte[] ConvertHtmlToPdf(string html)
    {
        SetLength(value: 0);
        HtmlConverter.ConvertToPdf(html: html, pdfStream: this);

        return ToArray();
    }
}