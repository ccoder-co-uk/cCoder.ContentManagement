// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text.RegularExpressions;
using cCoder.ContentManagement.Models.PageRendering;

namespace cCoder.ContentManagement.Services.Processings.PageRendering;

internal sealed partial class MetadataTagHandlingProcessingService
    : IMetadataTagHandlingProcessingService
{
    private static readonly Regex metadataRegex = new(
        pattern: "\\[meta\\[(?<name>[A-Za-z\\d_\\-/. ]+)\\]\\]",
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

        operation.Content = metadataRegex.Replace(
            input: operation.Content,
            evaluator: match => operation.Session.MetadataResolver(
                arg: match.Groups["name"].Value) ?? string.Empty);

        return operation;
    });
}