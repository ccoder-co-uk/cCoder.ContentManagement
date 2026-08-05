// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;
using System.ComponentModel.DataAnnotations;

namespace cCoder.ContentManagement.Services.Foundations;

internal partial class RenderFileContentService
{
    private static int ValidateAppId(int appId, string parameterName)
    {
        if (appId < 1)
        {
            throw new ValidationException(message: parameterName + " must be greater than 0.");
        }

        return appId;
    }

    private static string ValidatePath(string path, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value: path))
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        return path;
    }

    private static void ValidateLatestTextContentOnGet(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}