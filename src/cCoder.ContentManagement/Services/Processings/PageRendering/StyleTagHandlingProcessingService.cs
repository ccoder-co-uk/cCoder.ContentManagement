// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text.RegularExpressions;
using cCoder.ContentManagement.Models.PageRendering;

namespace cCoder.ContentManagement.Services.Processings.PageRendering;

internal sealed partial class StyleTagHandlingProcessingService
    : IStyleTagHandlingProcessingService
{
    private static readonly Regex styleRegex = new(
        pattern: "\\[style\\[(?<name>[A-Za-z\\d_\\-/. ]+)\\]\\]",
        options: RegexOptions.IgnoreCase
            | RegexOptions.Compiled
            | RegexOptions.Singleline);

    public TagHandlingOperation HandleTagHandlingOperation(
        TagHandlingOperation operation) =>
        TryCatch(operation: () =>
    {
        ValidateTagHandlingOperationOnHandle(inputs: [operation]);

        ValidateTagHandlingOperation(
            operation: operation,
            parameterName: "operation");

        operation.Content = styleRegex.Replace(
            input: operation.Content,
            evaluator: match => ResolveStyle(
                session: operation.Session,
                name: match.Groups["name"].Value)?.Content
                    ?? string.Empty);

        return operation;
    });

    private static PageRenderStyle ResolveStyle(
        RenderSession session,
        string name) =>
        session.CommonStylesByName.TryGetValue(
            key: name,
            value: out PageRenderStyle style)
                ? style
                : null;
}