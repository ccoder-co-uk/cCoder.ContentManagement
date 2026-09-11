// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Brokers;
using cCoder.ContentManagement.Models.PageRendering;

namespace cCoder.ContentManagement.Services.Processings.PageRendering;

internal sealed partial class MetadataTagHandlingProcessingService(
    IRegularExpressionBroker regularExpressionBroker)
    : IMetadataTagHandlingProcessingService
{
    private const string MetadataPattern =
        "\\[meta\\[(?<name>[A-Za-z\\d_\\-/. ]+)\\]\\]";

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
            pattern: MetadataPattern,
            evaluator: (value, groups) =>
                tagHandlingOperation.Session.MetadataResolver(
                    arg: groups["name"]) ?? string.Empty);

        return tagHandlingOperation;
    });
}