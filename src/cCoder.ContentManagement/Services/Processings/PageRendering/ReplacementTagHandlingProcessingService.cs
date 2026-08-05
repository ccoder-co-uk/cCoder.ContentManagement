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
        TagHandlingOperation operation) =>
        TryCatch(operation: () =>
    {
        ValidateTagHandlingOperationOnHandle(inputs: [operation]);

        ValidateTagHandlingOperation(
            operation: operation,
            parameterName: "operation");

        foreach (ReplacementDependency replacement in operation.Replacements)
        {
            if (string.Equals(
                a: replacement.Old,
                b: ContentSecurityPolicyNonceContract.Placeholder,
                comparisonType: StringComparison.Ordinal))
            {
                continue;
            }

            operation.Content = operation.Content.Replace(
                oldValue: replacement.Old,
                newValue: replacement.New);
        }

        return operation;
    });
}