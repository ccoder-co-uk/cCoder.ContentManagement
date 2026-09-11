// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Models.PageRendering;

namespace cCoder.ContentManagement.Rendering.Services.Foundations;

internal sealed partial class MarkupRenderService
{
    private const string StylePattern =
        "\\[style\\[(?<name>[A-Za-z\\d_\\-/. ]+)\\]\\]";

    private TagHandlingOperation RenderStyleTagHandlingOperationCore(
        TagHandlingOperation tagHandlingOperation)
    {

        tagHandlingOperation.Content = regularExpressionBroker.Replace(
            input: tagHandlingOperation.Content,
            pattern: StylePattern,
            evaluator: (value, groups) => ResolveStyle(
                session: tagHandlingOperation.Session,
                name: groups["name"])?.Content
                ?? string.Empty);

        return tagHandlingOperation;
    }

    private static PageRenderStyle ResolveStyle(
        RenderSession session,
        string name) =>
        session.CommonStylesByName.TryGetValue(
            key: name,
            value: out PageRenderStyle style)
                ? style
                : null;
}