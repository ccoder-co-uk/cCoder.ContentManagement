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
        TagHandlingOperation tagHandlingOperation) =>
        TryCatch(operation: () =>
    {
        ValidateTagHandlingOperationOnHandle(inputs: [tagHandlingOperation]);

        ValidateTagHandlingOperation(
            operation: tagHandlingOperation,
            parameterName: "operation");

        tagHandlingOperation.Content = metadataRegex.Replace(
            input: tagHandlingOperation.Content,
            evaluator: match => tagHandlingOperation.Session.MetadataResolver(
                arg: match.Groups["name"].Value) ?? string.Empty);

        return tagHandlingOperation;
    });
}