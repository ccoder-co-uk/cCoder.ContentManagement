// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers;

namespace cCoder.ContentManagement.Services.Foundations.TemplateContents;

internal partial class HtmlToPdfService(
    IHtmlToPdfBroker htmlToPdfBroker) : IHtmlToPdfService
{
    public byte[] ConvertHtmlToPdf(string html) =>
        TryCatch(operation: () =>
        {
            ValidateHtmlOnConvert(inputs: [html]);
            return htmlToPdfBroker.ConvertHtmlToPdf(html: html);
        });
}