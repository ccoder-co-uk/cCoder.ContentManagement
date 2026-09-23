// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class PageRenderDataService
{
    private static void ValidatePageRenderDataOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidatePageId(int pageId, string parameterName)
    {
        if (pageId < 1)
        {
            throw new ValidationException(
                message: parameterName + " must be greater than 0.");
        }
    }

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}