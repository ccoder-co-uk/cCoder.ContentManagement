// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.ContentManagement.Dependencies;
using cCoder.Data.Models.CMS;
using System.ComponentModel.DataAnnotations;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class SubmissionService
{
    private static void ValidateId(Guid submissionId, string parameterName) =>
        ThrowIf(condition: submissionId == Guid.Empty, message: parameterName + " is required.");

    private static void ValidateSubmission(Submission submission, string parameterName)
    {
        if (submission == null)
        {
            throw new ValidationException(message: parameterName + " is required.");
        }

        if (submission.AppId < 1)
        {
            throw new ValidationException(message: parameterName + ".AppId must be greater than 0.");
        }
    }

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            throw new ValidationException(message: message);
        }
    }

    private static void ValidateSubmissionOnGet(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateAllSubmissionOnGet(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateSubmissionOnAdd(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateSubmissionOnUpdate(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateDeleteAsync(object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);
}