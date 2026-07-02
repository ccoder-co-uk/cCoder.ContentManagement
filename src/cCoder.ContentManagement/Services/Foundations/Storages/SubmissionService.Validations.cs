using System.ComponentModel.DataAnnotations;
using cCoder.Data.Models.CMS;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class SubmissionService
{
    private static void ValidateId(Guid id, string parameterName) =>
        ThrowIf(id == Guid.Empty, parameterName + " is required.");

    private static void ValidateSubmission(Submission submission, string parameterName)
    {
        if (submission == null)
            throw new ValidationException(parameterName + " is required.");

        if (submission.AppId < 1)
            throw new ValidationException(parameterName + ".AppId must be greater than 0.");
    }

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
            throw new ValidationException(message);
    }
}
