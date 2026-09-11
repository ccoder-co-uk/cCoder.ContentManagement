// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Models.PageRendering;

namespace cCoder.ContentManagement.Rendering.Services.Foundations;

internal sealed partial class MarkupRenderService
{
    private const string MetadataPattern =
        "\\[meta\\[(?<name>[A-Za-z\\d_\\-/. ]+)\\]\\]";

    private TagHandlingOperation RenderMetadataTagHandlingOperationCore(
        TagHandlingOperation tagHandlingOperation)
    {

        tagHandlingOperation.Content = regularExpressionBroker.Replace(
            input: tagHandlingOperation.Content,
            pattern: MetadataPattern,
            evaluator: (value, groups) =>
                tagHandlingOperation.Session.MetadataResolver(
                    arg: groups["name"]) ?? string.Empty);

        return tagHandlingOperation;
    }
}