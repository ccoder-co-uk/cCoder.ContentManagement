// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Models;
using cCoder.ContentManagement.Models.PageRendering;

namespace cCoder.ContentManagement.Services.Processings.PageRendering;

internal sealed partial class ReplacementTagHandlingProcessingService
    : IReplacementTagHandlingProcessingService
{
    public TagHandlingOperation HandleTagHandlingOperation(
        TagHandlingOperation tagHandlingOperation) =>
        TryCatch(operation: () =>
    {
        ValidateTagHandlingOperationOnHandle(inputs: [tagHandlingOperation]);

        ValidateTagHandlingOperation(
            operation: tagHandlingOperation,
            parameterName: "operation");

        foreach (ReplacementDependency replacement in tagHandlingOperation.Replacements)
        {
            if (string.Equals(
                a: replacement.Old,
                b: ContentSecurityPolicyNonceContract.Placeholder,
                comparisonType: StringComparison.Ordinal))
            {
                continue;
            }

            tagHandlingOperation.Content = tagHandlingOperation.Content.Replace(
                oldValue: replacement.Old,
                newValue: replacement.New);
        }

        return tagHandlingOperation;
    });
}