// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Foundations.TemplateContents;

namespace cCoder.ContentManagement.Services.Processings;

internal partial class HtmlToPdfProcessingService(
    IHtmlToPdfService htmlToPdfService)
        : IHtmlToPdfProcessingService
{
    public byte[] ConvertHtmlToPdf(string html) =>
        TryCatch(operation: () =>
        {
            ValidateHtmlOnConvert(inputs: [html]);
            return htmlToPdfService.ConvertHtmlToPdf(html: html);
        });
}