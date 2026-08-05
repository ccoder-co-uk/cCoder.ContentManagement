// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace cCoder.ContentManagement.Services.Aggregations;

internal sealed partial class PageRenderCacheBuildAggregationService
{
    private static void ValidateServiceInputs(object[] inputs)
    {
        if (inputs is null || inputs.Length == 0)
        {
            throw new ValidationException(message: "Service inputs are required.");
        }
    }
}