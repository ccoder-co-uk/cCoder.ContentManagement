// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace cCoder.ContentManagement.Services.Aggregations;

internal sealed partial class PageRenderCacheBuildAggregationService
{
    private static void ValidateBuildPageAsync(object[] inputs) =>
        ValidateServiceInputs(inputs: inputs);

    private static void ValidatePageId(int pageId, string parameterName)
    {
        if (pageId < 1)
        {
            throw new ValidationException(
                message: $"{parameterName} must be greater than zero.");
        }
    }
}