using System.ComponentModel.DataAnnotations;
using Submission = cCoder.Data.Models.CMS.Submission;

namespace cCoder.ContentManagement.Services.Foundations.Storages;

internal partial class SubmissionService
{
    private static void ValidateId(Guid id, string parameterName)
    {
        if (id == Guid.Empty)
        {
            throw new ValidationException(parameterName + " is required.");
        }
    }

    private static void ValidateSubmission(Submission submission, string parameterName)
    {
        if (submission == null)
        {
            throw new ValidationException(parameterName + " is required.");
        }
        if (submission.AppId < 1)
        {
            throw new ValidationException(parameterName + ".AppId must be greater than 0.");
        }
    }
}
