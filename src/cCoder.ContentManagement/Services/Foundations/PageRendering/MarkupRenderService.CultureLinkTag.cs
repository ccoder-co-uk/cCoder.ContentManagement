// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Models.PageRendering;

namespace cCoder.ContentManagement.Rendering.Services.Foundations;

internal sealed partial class MarkupRenderService
{
    private const string CultureLinkPattern =
        "\\[culturelink\\[(?<name>[A-Za-z\\d_\\-/. ]+)\\]\\]";

    private TagHandlingOperation RenderCultureLinkTagHandlingOperationCore(
        TagHandlingOperation tagHandlingOperation)
    {

        tagHandlingOperation.Content = regularExpressionBroker.Replace(
            input: tagHandlingOperation.Content,
            pattern: CultureLinkPattern,
            replacement: "?culture=");

        return tagHandlingOperation;
    }
}