// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;
using cCoder.Data.Models.CMS;
using System.ComponentModel.DataAnnotations;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class CultureService
{
    private static void ValidateId(string cultureId, string parameterName) =>
        ThrowIf(condition: string.IsNullOrWhiteSpace(value: cultureId), message: parameterName + " is required.");

    private static void ValidateCulture(Culture culture, string parameterName)
    {
        if ((object)culture == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        if (string.IsNullOrWhiteSpace(value: culture.Id))
        {
            throw new ValidationException(message: parameterName + ".Id is required.");
        }

        if (string.IsNullOrWhiteSpace(value: culture.Name))
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

    private static void ValidateCultureOnGet(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateAllCultureOnGet(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateCultureOnAdd(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateCultureOnUpdate(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateDeleteAsync(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}