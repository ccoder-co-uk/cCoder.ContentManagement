// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers.Rendering;

namespace cCoder.ContentManagement.Services.Foundations.Rendering;

internal sealed partial class HtmlRenderService(
    IContentRenderBroker contentRenderBroker)
        : IHtmlRenderService
{
    public string HtmlEncode(string value) =>
        TryCatch<string>(operation: () =>
        {
            ValidateHtmlEncode(inputs: [value]);
            return contentRenderBroker.HtmlEncode(value: value);
        });
}