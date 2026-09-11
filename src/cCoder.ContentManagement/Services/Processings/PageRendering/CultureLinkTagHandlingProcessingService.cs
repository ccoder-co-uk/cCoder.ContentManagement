// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text.RegularExpressions;
using cCoder.ContentManagement.Models.PageRendering;

namespace cCoder.ContentManagement.Services.Processings.PageRendering;

internal sealed partial class CultureLinkTagHandlingProcessingService
    : ICultureLinkTagHandlingProcessingService
{
    private static readonly Regex cultureLinkRegex = new(
        pattern: "\\[culturelink\\[(?<name>[A-Za-z\\d_\\-/. ]+)\\]\\]",
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

        tagHandlingOperation.Content = cultureLinkRegex.Replace(
            input: tagHandlingOperation.Content,
            replacement: "?culture=");

        return tagHandlingOperation;
    });
}