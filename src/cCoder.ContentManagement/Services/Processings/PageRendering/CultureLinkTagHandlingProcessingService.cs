// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Models.PageRendering;

namespace cCoder.ContentManagement.Services.Processings.PageRendering;

internal sealed partial class CultureLinkTagHandlingProcessingService(
    IRegularExpressionBroker regularExpressionBroker)
    : ICultureLinkTagHandlingProcessingService
{
    private const string CultureLinkPattern =
        "\\[culturelink\\[(?<name>[A-Za-z\\d_\\-/. ]+)\\]\\]";

    public TagHandlingOperation HandleTagHandlingOperation(
        TagHandlingOperation tagHandlingOperation) =>
        TryCatch(operation: () =>
    {
        ValidateTagHandlingOperationOnHandle(inputs: [tagHandlingOperation]);

        ValidateTagHandlingOperation(
            operation: tagHandlingOperation,
            parameterName: "operation");

        tagHandlingOperation.Content = regularExpressionBroker.Replace(
            input: tagHandlingOperation.Content,
            pattern: CultureLinkPattern,
            replacement: "?culture=");

        return tagHandlingOperation;
    });
}