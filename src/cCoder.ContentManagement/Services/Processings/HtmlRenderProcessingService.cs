// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Services.Foundations.Rendering;

namespace cCoder.ContentManagement.Services.Processings;

internal sealed partial class HtmlRenderProcessingService(
    IHtmlRenderService htmlRenderService)
        : IHtmlRenderProcessingService
{
    public string HtmlEncode(string value) =>
        TryCatch<string>(operation: () =>
        {
            ValidateHtmlEncode(inputs: [value]);
            return htmlRenderService.HtmlEncode(value: value);
        });
}