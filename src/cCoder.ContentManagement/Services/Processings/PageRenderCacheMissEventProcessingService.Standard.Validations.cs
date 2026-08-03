// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Dependencies;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Processings;

internal sealed partial class PageRenderCacheMissEventProcessingService
{
    private static void ValidateRaisePageRenderCacheMissEventAsync(
        object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidatePageRenderCacheMiss(
        PageRenderCacheMiss cacheMiss,
        string parameterName)
    {
        if (cacheMiss is null)
        {
            throw new ValidationException(
                message: parameterName + " is required.");
        }
    }
}