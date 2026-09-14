// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using System.ComponentModel.DataAnnotations;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class ScriptService
{
    private static void ValidateId(int scriptId, string parameterName) =>
        ThrowIf(condition: scriptId < 1, message: parameterName + " must be greater than 0.");

    private static void ValidateScript(Script script, string parameterName)
    {
        if (script == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        if (script.AppId < 1)
        {
            throw new ValidationException(message: parameterName + ".AppId must be greater than 0.");
        }

        if (string.IsNullOrWhiteSpace(value: script.Name))
        {
            throw new ValidationException(message: parameterName + ".Name is required.");
        }
    }

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            throw new ValidationException(message: message);
        }
    }

    private static void ValidateScriptOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllScriptOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateScriptOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateScriptOnUpdate(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateDeleteAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}