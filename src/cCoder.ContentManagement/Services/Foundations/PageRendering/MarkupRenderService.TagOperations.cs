// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models.PageRendering;
using cCoder.ContentManagement.Models.Serialization;

namespace cCoder.ContentManagement.Rendering.Services.Foundations;

internal sealed partial class MarkupRenderService
{
    public TagHandlingOperation RenderCultureLinkTagHandlingOperation(
        TagHandlingOperation tagHandlingOperation) =>
        TryCatch(operation: () =>
        {
            ValidateTagHandlingOperation(inputs: [tagHandlingOperation]);

            return RenderCultureLinkTagHandlingOperationCore(
                tagHandlingOperation: tagHandlingOperation);
        });

    public TagHandlingOperation RenderMetadataTagHandlingOperation(
        TagHandlingOperation tagHandlingOperation) =>
        TryCatch(operation: () =>
        {
            ValidateTagHandlingOperation(inputs: [tagHandlingOperation]);

            return RenderMetadataTagHandlingOperationCore(
                tagHandlingOperation: tagHandlingOperation);
        });

    public TagHandlingOperation RenderNavigationTagHandlingOperation(
        TagHandlingOperation tagHandlingOperation) =>
        TryCatch(operation: () =>
        {
            ValidateTagHandlingOperation(inputs: [tagHandlingOperation]);

            return RenderNavigationTagHandlingOperationCore(
                tagHandlingOperation: tagHandlingOperation);
        });

    public TagHandlingOperation RenderContentTagHandlingOperation(
        TagHandlingOperation tagHandlingOperation) =>
        TryCatch(operation: () =>
        {
            ValidateTagHandlingOperation(inputs: [tagHandlingOperation]);

            return RenderContentTagHandlingOperationCore(
                tagHandlingOperation: tagHandlingOperation);
        });

    public TagHandlingOperation RenderComponentTagHandlingOperation(
        TagHandlingOperation tagHandlingOperation) =>
        TryCatch(operation: () =>
        {
            ValidateTagHandlingOperation(inputs: [tagHandlingOperation]);

            return RenderComponentTagHandlingOperationCore(
                tagHandlingOperation: tagHandlingOperation);
        });

    public TagHandlingOperation RenderScriptTagHandlingOperation(
        TagHandlingOperation tagHandlingOperation) =>
        TryCatch(operation: () =>
        {
            ValidateTagHandlingOperation(inputs: [tagHandlingOperation]);

            return RenderScriptTagHandlingOperationCore(
                tagHandlingOperation: tagHandlingOperation);
        });

    public TagHandlingOperation RenderStyleTagHandlingOperation(
        TagHandlingOperation tagHandlingOperation) =>
        TryCatch(operation: () =>
        {
            ValidateTagHandlingOperation(inputs: [tagHandlingOperation]);

            return RenderStyleTagHandlingOperationCore(
                tagHandlingOperation: tagHandlingOperation);
        });

    public TagHandlingOperation RenderDmsTagHandlingOperation(
        TagHandlingOperation tagHandlingOperation) =>
        TryCatch(operation: () =>
        {
            ValidateTagHandlingOperation(inputs: [tagHandlingOperation]);

            return RenderDmsTagHandlingOperationCore(
                tagHandlingOperation: tagHandlingOperation);
        });

    public TagHandlingOperation RenderResourceTagHandlingOperation(
        TagHandlingOperation tagHandlingOperation) =>
        TryCatch(operation: () =>
        {
            ValidateTagHandlingOperation(inputs: [tagHandlingOperation]);

            return RenderResourceTagHandlingOperationCore(
                tagHandlingOperation: tagHandlingOperation);
        });

    public TagHandlingOperation RenderExecuteTagHandlingOperation(
        TagHandlingOperation tagHandlingOperation) =>
        TryCatch(operation: () =>
        {
            ValidateTagHandlingOperation(inputs: [tagHandlingOperation]);

            return RenderExecuteTagHandlingOperationCore(
                tagHandlingOperation: tagHandlingOperation);
        });

    public TagHandlingOperation SerializeTagHandlingOperation(
        TagHandlingOperation tagHandlingOperation) =>
        TryCatch(operation: () =>
        {
            ValidateTagHandlingOperation(inputs: [tagHandlingOperation]);
            tagHandlingOperation.Content = jsonBroker.Serialize(value: tagHandlingOperation.Value);
            return tagHandlingOperation;
        });

    public TagHandlingOperation ParseJsonTagHandlingOperation(
        TagHandlingOperation tagHandlingOperation) =>
        TryCatch(operation: () =>
        {
            ValidateTagHandlingOperation(inputs: [tagHandlingOperation]);
            tagHandlingOperation.Value = jsonBroker.ParseJson(json: tagHandlingOperation.Content);
            return tagHandlingOperation;
        });

    public TagHandlingOperation NormalizeJsonTagHandlingOperation(
        TagHandlingOperation tagHandlingOperation) =>
        TryCatch(operation: () =>
        {
            ValidateTagHandlingOperation(inputs: [tagHandlingOperation]);

            if (jsonBroker.IsJsonElement(value: tagHandlingOperation.Value))
            {
                tagHandlingOperation.Value = jsonBroker.ParseJson(
                    json: jsonBroker.GetJsonRawText(
                        value: tagHandlingOperation.Value));
            }

            return tagHandlingOperation;
        });

    public TagHandlingOperation IsJsonObjectTagHandlingOperation(
        TagHandlingOperation tagHandlingOperation) =>
        TryCatch(operation: () =>
        {
            ValidateTagHandlingOperation(inputs: [tagHandlingOperation]);
            tagHandlingOperation.Condition = jsonBroker.IsJsonObject(value: tagHandlingOperation.Value);
            return tagHandlingOperation;
        });

    public TagHandlingOperation IsJsonArrayTagHandlingOperation(
        TagHandlingOperation tagHandlingOperation) =>
        TryCatch(operation: () =>
        {
            ValidateTagHandlingOperation(inputs: [tagHandlingOperation]);
            tagHandlingOperation.Condition = jsonBroker.IsJsonArray(value: tagHandlingOperation.Value);
            return tagHandlingOperation;
        });

    public TagHandlingOperation IsJsonValueTagHandlingOperation(
        TagHandlingOperation tagHandlingOperation) =>
        TryCatch(operation: () =>
        {
            ValidateTagHandlingOperation(inputs: [tagHandlingOperation]);
            tagHandlingOperation.Condition = jsonBroker.IsJsonValue(value: tagHandlingOperation.Value);
            return tagHandlingOperation;
        });

    public TagHandlingOperation GetJsonPropertiesTagHandlingOperation(
        TagHandlingOperation tagHandlingOperation) =>
        TryCatch(operation: () =>
        {
            ValidateJsonPropertiesTagHandlingOperationOnGet(
                inputs: [tagHandlingOperation]);

            tagHandlingOperation.JsonProperties = jsonBroker
                .GetJsonProperties(value: tagHandlingOperation.Value)
                .ToArray();

            return tagHandlingOperation;
        });
}