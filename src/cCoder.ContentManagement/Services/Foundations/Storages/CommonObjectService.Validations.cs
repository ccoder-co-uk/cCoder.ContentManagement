// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class CommonObjectService
{
    private static void ValidateCommonObjectsOnDeserialize(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAuthorization(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAppAdministration(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateComponentOnCache(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateResourceOnCache(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateScriptOnCache(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateId(int commonObjectId, string parameterName) =>
        ThrowIf(condition: commonObjectId < 1, message: parameterName + " must be greater than 0.");

    private static void ValidateCommonObject(CommonObject commonObject, string parameterName)
    {
        if (commonObject == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        if (string.IsNullOrWhiteSpace(value: commonObject.Name))
        {
            throw new ValidationException(message: parameterName + ".Name is required.");
        }

        if (string.IsNullOrWhiteSpace(value: commonObject.Type))
        {
            throw new ValidationException(message: parameterName + ".Type is required.");
        }
    }

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            throw new ValidationException(message: message);
        }
    }

    private static void ValidateCommonObjectOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateAllCommonObjectOnGet(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateCommonObjectOnAdd(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateCommonObjectOnUpdate(object[] inputs) =>
        Validate(inputs: inputs);

    private static void ValidateDeleteAsync(object[] inputs) =>
        Validate(inputs: inputs);

    private static void Validate(params object[] inputs) =>
        _ = inputs;
}