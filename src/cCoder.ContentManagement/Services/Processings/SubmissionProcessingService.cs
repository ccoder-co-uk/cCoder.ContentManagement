// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.ContentManagement.Services.Foundations.Storages;
using cCoder.Data.Models.CMS;
using cCoder.ContentManagement.Models;

namespace cCoder.ContentManagement.Services.Processings;

internal class SubmissionProcessingService(ISubmissionService service) : ISubmissionProcessingService
{
    public Submission GetSubmission(Guid submissionId)
    {
        ValidateId(submissionId: submissionId, parameterName: "id");
        return service.GetSubmission(submissionId: submissionId);
    }

    public IQueryable<Submission> GetAllSubmission(bool ignoreFilters = false) =>
        service.GetAllSubmission(ignoreFilters: ignoreFilters);

    public ValueTask<Submission> AddSubmissionAsync(Submission newSubmission)
    {
        ValidateSubmission(submission: newSubmission, parameterName: "entity");
        return service.AddSubmissionAsync(newSubmission: newSubmission);
    }

    public ValueTask<Submission> UpdateSubmissionAsync(Submission updatedSubmission)
    {
        ValidateSubmission(submission: updatedSubmission, parameterName: "entity");
        return service.UpdateSubmissionAsync(updatedSubmission: updatedSubmission);
    }

    public ValueTask DeleteAsync(Guid submissionId)
    {
        ValidateId(submissionId: submissionId, parameterName: "id");
        return service.DeleteAsync(submissionId: submissionId);
    }

    public async ValueTask<IEnumerable<Result<Submission>>> AddOrUpdateSubmissionResult(IEnumerable<Submission> newSubmission)
    {
        ValidateSubmissions(submissions: newSubmission, parameterName: "items");
        List<Result<Submission>> results = new List<Result<Submission>>();

        foreach (Submission item in newSubmission)
        {
            try
            {
                Submission savedItem = item.Id == Guid.Empty ? await ExecuteAddSubmissionAsync(newSubmission: item) : await ExecuteUpdateSubmissionAsync(updatedSubmission: item);

                results.Add(item: new Result<Submission>
                {
                    Success = true,
                    Item = savedItem,
                    Message = item.Id == Guid.Empty ? "Added Successfully" : "Updated Successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(item: new Result<Submission>
                {
                    Success = false,
                    Item = item,
                    Message = ex.Message
                });
            }
        }

        return results;
    }

    public async ValueTask DeleteAllSubmissionAsync(IEnumerable<Submission> deletedSubmission)
    {
        ValidateSubmissions(submissions: deletedSubmission, parameterName: "items");

        foreach (Submission item in deletedSubmission)
        {
            await ExecuteDeleteAsync(submissionId: item.Id);
        }
    }

    private static void ValidateId(Guid submissionId, string parameterName) =>
        ThrowIf(condition: submissionId == Guid.Empty, message: parameterName + " is required.");

    private static void ValidateSubmission(Submission submission, string parameterName) =>
        ThrowIf(condition: submission == null, message: parameterName + " is required.");

    private static void ValidateSubmissions(IEnumerable<Submission> submissions, string parameterName) =>
        ThrowIf(condition: submissions == null, message: parameterName + " is required.");

    private static void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            throw new ValidationException(message: message);
        }
    }

    private ValueTask<Submission> ExecuteAddSubmissionAsync(Submission newSubmission)
    {
        ValidateSubmission(submission: newSubmission, parameterName: "entity");
        return service.AddSubmissionAsync(newSubmission: newSubmission);
    }

    private ValueTask ExecuteDeleteAsync(Guid submissionId)
    {
        ValidateId(submissionId: submissionId, parameterName: "id");
        return service.DeleteAsync(submissionId: submissionId);
    }

    private ValueTask<Submission> ExecuteUpdateSubmissionAsync(Submission updatedSubmission)
    {
        ValidateSubmission(submission: updatedSubmission, parameterName: "entity");
        return service.UpdateSubmissionAsync(updatedSubmission: updatedSubmission);
    }
}